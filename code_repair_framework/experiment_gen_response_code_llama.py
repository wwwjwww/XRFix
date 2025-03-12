import os
import json
import config
import time
import requests
from datetime import datetime
import re

from transformers import AutoTokenizer, AutoModelForCausalLM,BitsAndBytesConfig, LlamaForCausalLM
from tenacity import retry, stop_after_attempt, wait_random_exponential
import logging
import argparse
import torch
import warnings
from accelerate import dispatch_model
from accelerate.utils import get_balanced_memory, infer_auto_device_map

os.environ["CUDA_VISIBLE_DEVICES"] = "0,1"

def parse_arguments():
    parser = argparse.ArgumentParser()
    parser.add_argument('--run_path', default=None, type=str)
    parser.add_argument('--access_token', default=None, type=str)
    parser.add_argument('--cache_dir', default=r"./models", type=str)
    parser.add_argument('--checkpoint', default='codellama/CodeLlama-7b-Instruct-hf',
                        choices=['meta-llama/CodeLlama-7b-Instruct-hf','codellama/CodeLlama-7b-Instruct-hf',
                                 'codellama/CodeLlama-13b-Instruct-hf','codellama/CodeLlama-34b-Instruct-hf'], type=str)

    parser.add_argument('--temperature', default=1, type=float)
    parser.add_argument('--candidate_num', default=1, type=int)
    args = parser.parse_args()

    return args

@retry(wait=wait_random_exponential(min=1, max=60), stop=stop_after_attempt(6))
def generate_text(prompt, temperature, max_new_tokens,candidate_num):
    inputs = tokenizer(prompt, return_tensors='pt', add_special_tokens=False).to(device)
    with torch.no_grad():
    	outputs = model.generate(
        	inputs['input_ids'],
        	max_new_tokens=max_new_tokens,
        	temperature=temperature,
        	top_k=50,
        	num_return_sequences=candidate_num,
        	do_sample=True,
        	pad_token_id=tokenizer.eos_token_id
    	).to('cpu')
    	response = [tokenizer.decode(output, skip_special_tokens=True).split('[/INST]')[-1].strip()
        	        for output in outputs]
    torch.cuda.empty_cache()
    torch.cuda.ipc_collect()

    return response


@retry(wait=wait_random_exponential(min=1, max=60), stop=stop_after_attempt(6))
def count_message_tokens(content):
    tokens = tokenizer(content)['input_ids']
    num_tokens = len(tokens)

    return num_tokens


def hand_crafted_prompt_response(path, max_token, skip_engines=[]):
    file_extension = '.cs'
    files = os.walk(path)
    comment_key = "//"

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    hand_crafted_prompt_long = os.path.join(path, "hand_crafted_prompt_long.txt")
                    hand_crafted_prompt_short = os.path.join(path, "hand_crafted_prompt_short.txt")
                    hand_crafted_prompt = os.path.join(path, "hand_crafted_prompt.txt")
                    prompt = scenario_contents["prompt_template"]
                    short_contents = ""

                    head_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                  -1] + ".head" + file_extension
                    head_contents_path = os.path.join(path, head_contents_file_name)

                    prepend_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                     -1] + ".prepend" + file_extension
                    prepend_contents_path = os.path.join(path, prepend_contents_file_name)
                    prepend_contents_file_name_add = scenario_contents["err_detailed_info"]['add_file_name'].split('/')[
                                                         -1] + ".prepend_add" + file_extension
                    prepend_contents_path_add = os.path.join(path, prepend_contents_file_name_add)
                    prompt_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                    -1] + ".prompt" + file_extension
                    prompt_contents_path = os.path.join(path, prompt_contents_file_name)
                    prompt_contents_file_name_add = scenario_contents["err_detailed_info"]['add_file_name'].split('/')[
                                                        -1] + ".prompt_add" + file_extension
                    prompt_contents_path_add = os.path.join(path, prompt_contents_file_name_add)
                    prompt_between_lines_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                         -1] + ".between" + file_extension
                    prompt_between_lines_path = os.path.join(path, prompt_between_lines_file_name)
                    prompt_add_first_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                     -1] + ".add_first.txt"
                    prompt_add_first_path = os.path.join(path, prompt_add_first_file_name)
                    prompt_short_lines_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[
                                                         -1] + ".short" + file_extension
                    prompt_short_lines_path = os.path.join(path, prompt_short_lines_file_name)

                    add_prompt_short_lines_file_name = scenario_contents["err_detailed_info"]['add_file_name'].split('/')[
                                                         -1] + ".short" + file_extension
                    add_prompt_short_lines_path = os.path.join(path, add_prompt_short_lines_file_name)

                    include_addition = scenario_contents["include_addition"]
                    file_name = scenario_contents["err_detailed_info"]["file_name"]
                    add_file_name = scenario_contents["err_detailed_info"]["add_file_name"]

                    with open(prepend_contents_path, 'r', encoding='utf8') as f1:
                        prepend_contents = f1.read()
                    with open(prompt_contents_path, 'r', encoding='utf8') as f2:
                        prompt_contents = f2.read()
                    with open(prepend_contents_path_add, 'r', encoding='utf8') as f3:
                        prepend_contents_add = f3.read()
                    with open(prompt_contents_path_add, 'r', encoding='utf8') as f4:
                        prompt_contents_add = f4.read()
                    with open(prompt_short_lines_path, 'r', encoding='utf8') as f7:
                        prompt_short_lines = f7.read()
                    if os.path.exists(prompt_add_first_path):
                        with open(prompt_between_lines_path, 'r', encoding='utf8') as f5:
                            prompt_between_lines = f5.read()
                        with open(prompt_add_first_path, 'r', encoding='utf8') as f6:
                            add_first_str = f6.read()
                    else:
                        with open(add_prompt_short_lines_path, 'r', encoding='utf8') as f7:
                            add_prompt_short_lines = f7.read()

                    if file_name == add_file_name:

                        prompt_head = comment_key + "Here're the buggy code lines from " + file_name + ":\n"
                        if "fix_instruction_add_prompt_assymetrical" in prompt:
                            prompt_head = "/* Here're the buggy code lines from " + file_name + ":*/\n"
                        # prompt_foot = "\n" + comment_key + " FIXED CODE:\n"
                        if "False" in add_first_str:
                            prompt_lines_long = prompt_head + prepend_contents + prompt_contents + prompt_between_lines + prompt_contents_add
                            prompt_lines_short = prompt_head + prompt_short_lines + prompt_contents + prompt_contents_add
                        else:
                            prompt_lines_long = prompt_head + prepend_contents + prompt_contents_add + prompt_between_lines + prompt_contents
                            prompt_lines_short = prompt_head + prompt_short_lines + prompt_contents_add + prompt_contents

                        with open(hand_crafted_prompt_long, 'w', encoding='utf8') as f5:
                            f5.write(prompt_lines_long)
                        with open(hand_crafted_prompt_short, 'w', encoding='utf8') as f6:
                            f6.write(prompt_lines_short)

                        prompt_contents = prompt_lines_long
                        short_contents = prompt_lines_short

                    else:
                        prompt_head = comment_key + "Here're the buggy code lines from " + file_name + ":\n"

                        prompt_mid = comment_key + "Here's the definition of function call in another component.\n" + \
                                     comment_key + "Related code from " + add_file_name + ":\n"


                        prompt_lines = prompt_head + prepend_contents + prompt_contents + prompt_mid + prepend_contents_add + prompt_contents_add
                        with open(hand_crafted_prompt, 'w', encoding='utf8') as f5:
                            f5.write(prompt_lines)

                        short_contents = prompt_head + prompt_short_lines + prompt_contents + prompt_mid + add_prompt_short_lines + prompt_contents_add

                        prompt_contents = prompt_lines

                        with open(hand_crafted_prompt_short, 'w', encoding='utf8') as f6:
                            f6.write(short_contents)

                    with open(head_contents_path, 'r', encoding='utf8') as f7:
                        instruct_head = f7.read()


                    generate_LLM_experiment_responses(
                        path,
                        instruct_head,
                        prompt_contents,
                        short_contents,
                        "",
                        scenario_contents["err_detailed_info"]['file_name'].split('/')[-1],
                        max_token,
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        include_addition,
                        skip_engines = skip_engines
                    )


def generate_codellama_requests(instruct_head, prompt, temp, top_p, max_tokens, iteration,stop_word):

    source_lang = "c#"
    contents = instruct_head+ "Please wrap your code answer using ```:" + '\n' + prompt + '\n'
    prompt = f'<s>[INST] {contents} [/INST]'

    input_tokens = count_message_tokens(prompt)
    print('input tokens: ' + str(input_tokens))
    logging.info('input tokens: ' + str(input_tokens))
    if input_tokens > max_tokens:
        print('Over input tokens limit')
        logging.warning('Over input tokens limit')
        status = "EXCEED MAX TOKEN"
        result = {}
        return status, result
    try:
        response = generate_text(
            prompt=prompt,
            temperature=temp,
            max_new_tokens=max_tokens,
            candidate_num=iteration
        )
        logging.info('response: ' + str(response))

        if response is not None:
            repair_outcome = response
            status = "SUCCESS"
        else:
            logging.warning('Respond content is none.')
            print('Respond content is none.')
            repair_outcome = []
            status = "FAILED"

    except Exception as e:
        logging.error('Failed to generate text: ' + e.__str__())
        repair_outcome = []
        status = "FAILED"

    result = {}

    for i, generated_code in enumerate(repair_outcome):
        output_tokens = count_message_tokens(generated_code)
        logging.info('output tokens: ' + str(output_tokens))
        if output_tokens > max_new_tokens:
            logging.warning('Over total tokens limit ')
            generated_code = ''
        logging.info('Code repairing in: ' + source_lang + ' :' + generated_code)
        result['code_repairing_' + str(i)] = generated_code

    if len(repair_outcome) < candidate_num:
        for i in range(candidate_num - len(repair_outcome)):
            result['code_repairing_' + str(i + len(repair_outcome))] = ''
    return status, result

def generate_LLM_experiment_responses(root_dir, instruct_head, contents, short_contents, append_contents, experiment_filename, max_tokens, temperature, top_p, LLM_engine, iteration, include_addition, skip_engines):

    llm_responses_dir = os.path.join(root_dir, "response",
                                       experiment_filename + ".llm_responses")
    if not os.path.exists(llm_responses_dir):
        os.makedirs(llm_responses_dir)

    prompt_text = contents

    prompt_file = os.path.join(llm_responses_dir, config.PROMPT_TEXT_FILENAME)
    with open(prompt_file, "w", encoding='utf8') as f:
        f.write(prompt_text)

    if append_contents is not None and append_contents != "":
        append_text = append_contents
        append_file = os.path.join(llm_responses_dir, config.APPEND_TEXT_FILENAME)
        with open(append_file, "w", encoding='utf8') as f1:
            f1.write(append_text)

    if True:
        for engine in LLM_engine:
            if engine in skip_engines:
                continue

            codex_responses_file = os.path.join(llm_responses_dir,
                                                "%s.temp-%.2f.top_p-%.2f.response.json" % (engine, temperature, top_p))
            print(codex_responses_file)
            print("Generating '%s' responses for %s" % (engine, experiment_filename))
            skip = False
            if os.path.exists(codex_responses_file):
                skip = True
            failure = 0
            while not skip and failure <= 3:
                    print(
                        "Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
                            llm_responses_dir, experiment_filename, temperature, top_p, engine, max_tokens))
                    if engine == "simple_instruction_result_open_source":
                        status, data = generate_codellama_requests(
                            instruct_head,
                            prompt_text,
                            temperature,
                            top_p,
                            max_tokens,
                            iteration,
                            "\n\t}"
                        )

                        if (status == "SUCCESS"):
                            print("LLM responses collected.")
                            break
                        else:
                            if (status == "EXCEED MAX TOKEN"):
                                prompt_text = short_contents

                            failure += 1
                            print("Waiting 30 seconds and trying again")
                            time.sleep(30)
                            continue


            if not skip and data != {}:
                # create codex_responses file in the experiment dir
                with open(codex_responses_file, "w") as f3:
                    f3.write(json.dumps(data, indent=4))

                actual_prompt_file = os.path.join(llm_responses_dir, "actual_prompt.txt")
                with open(actual_prompt_file, "w", encoding='utf8') as f:
                    f.write(prompt_text)



def prepare_LLM_experiment_requests(path, max_token, skip_engines=[]):
    file_extension = '.cs'
    files = os.walk(path)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    prompt_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".prompt" + file_extension
                    prompt_contents_path = os.path.join(path, prompt_contents_file_name)
                    append_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".append" + file_extension
                    append_contents_path = os.path.join(path, append_contents_file_name)
                    head_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".head" + file_extension
                    head_contents_path = os.path.join(path, head_contents_file_name)
                    short_prompt_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".short" + file_extension
                    short_prompt_contents_path = os.path.join(path, short_prompt_contents_file_name)

                    include_addition = scenario_contents["include_addition"]

                    with open(prompt_contents_path, 'r', encoding='utf8') as f1:
                        prompt_contents = f1.read()
                        prompt_contents_clean = re.sub(r'/\*.*?\*/', '', prompt_contents, flags=re.DOTALL)

                    with open(append_contents_path, 'r', encoding='utf8') as f2:
                        append_contents = f2.read()

                    with open(head_contents_path, 'r', encoding='utf8') as f3:
                        instruct_head = f3.read()

                    with open(short_prompt_contents_path, 'r', encoding='utf8') as f4:
                        short_prompt_contents = f4.read()
                        short_prompt_contents_clean = re.sub(r'/\*.*?\*/', '', short_prompt_contents, flags=re.DOTALL)


                    generate_LLM_experiment_responses(
                        path,
                        instruct_head,
                        prompt_contents_clean,
                        short_prompt_contents_clean,
                        append_contents,
                        scenario_contents["err_detailed_info"]['file_name'].split('/')[-1],
                        max_token,
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        include_addition,
                        skip_engines = skip_engines
                    )


def decide_include_addition(path, max_token, skip_engines=[]):
    ile_extension = '.cs'
    files = os.walk(path)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                if scenario_contents["include_addition"]:
                    hand_crafted_prompt_response(path, max_token, skip_engines)
                else:
                    prepare_LLM_experiment_requests(path, max_token, skip_engines)


if __name__ == '__main__':
    args = parse_arguments()

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

    #max_memory = {0: '20GiB', 1: '20GiB'}
    
    print('Device:', device)
    q_config = BitsAndBytesConfig(
            load_in_4bit=True,
            bnb_4bit_quant_type="nf4",
            bnb_4bit_compute_dtype=torch.float16
            )

    tokenizer = AutoTokenizer.from_pretrained(
        args.checkpoint,
        trust_remote_code=True,
        cache_dir=args.cache_dir,
        use_fast=True
    )
    model = AutoModelForCausalLM.from_pretrained(
        args.checkpoint,
        trust_remote_code=True,
        device_map='auto',
        low_cpu_mem_usage=True,
        cache_dir=args.cache_dir,
        quantization_config=q_config,
    )
   
    #device_map = infer_auto_device_map(model, max_memory=max_memory)
    #print(device_map)
    #model = dispatch_model(model, device_map=device_map)

    #print(f'Memory footprint: {model.get_memory_footprint() / 1e6:.2f} MB')
    candidate_num = args.candidate_num
    temperature = args.temperature
    max_input_tokens = tokenizer.model_max_length  # 1000000000000000019884624838656
    # The maximum numbers of tokens to generate, ignoring the number of tokens in the prompt.
    max_new_tokens = 4096  # max_output_tokens

    path = args.run_path
    skip_engines = []
    decide_include_addition(path, max_new_tokens, skip_engines)
