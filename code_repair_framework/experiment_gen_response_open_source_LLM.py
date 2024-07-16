import os
import json
import config
import time
import requests
from datetime import datetime
import re

from transformers import AutoTokenizer, AutoModelForCausalLM
from tenacity import retry, stop_after_attempt, wait_random_exponential
import logging
import argparse
import torch
import warnings


def parse_arguments():
    parser = argparse.ArgumentParser()
    parser.add_argument('--access_token', default=None, type=str)
    parser.add_argument('--cache_dir', default=None, type=str)
    parser.add_argument('--checkpoint', default='codellama/CodeLlama-7b-Instruct-hf',
                        choices=['codellama/CodeLlama-7b-Instruct-hf', 'codellama/CodeLlama-13b-Instruct-hf',
                                 'codellama/CodeLlama-34b-Instruct-hf'], type=str)

    parser.add_argument('--temperature', default=1, type=float)
    parser.add_argument('--candidate_num', default=1, type=int)
    args = parser.parse_args()

    return args

@retry(wait=wait_random_exponential(min=1, max=60), stop=stop_after_attempt(6))
def generate_text(prompt, temperature, max_new_tokens,candidate_num):
    inputs = tokenizer(prompt, return_tensors='pt', add_special_tokens=False).to(device)
    outputs = model.generate(
        inputs['input_ids'],
        max_new_tokens=max_new_tokens,
        temperature=temperature,
        top_k=50,
        top_p=0.95,
        num_return_sequences=candidate_num,
        do_sample=True,
        pad_token_id=tokenizer.eos_token_id
    ).to('cpu')
    response = [tokenizer.decode(output, skip_special_tokens=True).split('[/INST]')[-1].strip()
                for output in outputs]

    return response


@retry(wait=wait_random_exponential(min=1, max=60), stop=stop_after_attempt(6))
def count_message_tokens(content):
    tokens = tokenizer(content)['input_ids']
    num_tokens = len(tokens)

    return num_tokens

def generate_codellama_requests(instruct_head, prompt, temp, top_p, max_tokens, iteration,stop_word):

    source_lang = "c#"
    contents = instruct_head + '\n"""\n' + prompt + '\n"""\n'
    prompt = f'<s>[INST] {contents} [/INST]'

    input_tokens = count_message_tokens(prompt)
    logging.info('input tokens: ' + str(input_tokens))
    if input_tokens > max_input_tokens:
        logging.warning('Over input tokens limit')
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

def generate_LLM_experiment_responses(root_dir, instruct_head, contents, short_contents, append_contents, experiment_filename, temperature, top_p, LLM_engine, iteration, include_addition, skip_engines):
    max_tokens = 2048

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

    if not include_addition:
        for engine in LLM_engine:
            if engine in skip_engines:
                continue

            codex_responses_file = os.path.join(llm_responses_dir,
                                                "%s.temp-%.2f.top_p-%.2f.response.json" % (engine, temperature, top_p))
            print(codex_responses_file)
            print("Generating '%s' responses for %s" % (engine, experiment_filename))
            skip = False
            if os.path.exists(codex_responses_file):
                print("Response already exists. Skip.")
                skip = True
            while not skip:
                    print(
                        "Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
                            llm_responses_dir, experiment_filename, temperature, top_p, engine, max_tokens))
                    if engine == "code-llama":
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
                            print("Waiting 30 seconds and trying again")
                            time.sleep(30)
                            continue


            if not skip:
                # create codex_responses file in the experiment dir
                with open(codex_responses_file, "w") as f3:
                    f3.write(json.dumps(data, indent=4))

                actual_prompt_file = os.path.join(llm_responses_dir, "actual_prompt.txt")
                with open(actual_prompt_file, "w", encoding='utf8') as f:
                    f.write(prompt_text)



def prepare_LLM_experiment_requests(path, skip_engines=[]):
    file_extension = '.cs'
    files = os.walk(path)

    for path, dir_lis, file_lis in files:
        #print(path)
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
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        include_addition,
                        skip_engines = skip_engines
                    )


if __name__ == '__main__':
    args = parse_arguments()

    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
    print('Device:', device)


    tokenizer = AutoTokenizer.from_pretrained(
        args.checkpoint,
        use_fast=True,
        trust_remote_code=True,
        token=args.access_token,
        cache_dir=args.cache_dir
    )
    model = AutoModelForCausalLM.from_pretrained(
        args.checkpoint,
        torch_dtype=torch.float16,
        # load_in_4bit=True,
        low_cpu_mem_usage=True,
        trust_remote_code=True,
        device_map='auto',
        token=args.access_token,
        cache_dir=args.cache_dir
    )
    print(f'Memory footprint: {model.get_memory_footprint() / 1e6:.2f} MB')
    candidate_num = args.candidate_num
    temperature = args.temperature
    max_input_tokens = tokenizer.model_max_length  # 1000000000000000019884624838656
    # The maximum numbers of tokens to generate, ignoring the number of tokens in the prompt.
    max_new_tokens = 2048  # max_output_tokens

    path = r'.\experiment\cwe'
    skip_engines = []
    prepare_LLM_experiment_requests(path, skip_engines)

