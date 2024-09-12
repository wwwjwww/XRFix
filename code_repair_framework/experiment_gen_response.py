import os
import json
import config
import time
import requests
from datetime import datetime
import re

#os.environ["OPENAI_API_KEY"] = config.OPENAI_API_KEY
apiKey = config.OPENAI_API_KEY
basicUrl = "https://chatgpt.hkbu.edu.hk/general/rest"
modelName_gpt35 = "gpt-35-turbo"
apiVersion = "2024-02-15-preview"
modelName_gpt4 = "gpt-4-turbo"


def generate_gpt35_requests(instruct_head, prompt, temp, top_p, max_tokens, iteration,stop_word):
    url = basicUrl + "/deployments/" + modelName_gpt35 + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": instruct_head + '\n"""\n' + prompt + '\n"""\n'}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'max_tokens': max_tokens, 'n': iteration, 'stop': stop_word}
    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 200:
        data = response.json()
        return response.status_code, data
    else:
        return response.status_code, response

def generate_gpt4_requests(instruct_head, prompt, temp, top_p, max_tokens, iteration,stop_word):
    url = basicUrl + "/deployments/" + modelName_gpt4 + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": instruct_head + '\n"""\n' + prompt + '\n"""\n'}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'max_tokens': max_tokens, 'n': iteration, 'stop': stop_word}
    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 200:
        data = response.json()
        return response.status_code, data
    else:
        return response.status_code, response

def generate_gpt35_requests_no_stop_word(instruct_head, prompt, temp, top_p, max_tokens, iteration):
    url = basicUrl + "/deployments/" + modelName_gpt35 + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": instruct_head + '\n"""\n' + prompt + '\n"""\n'}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'max_tokens': max_tokens, 'n': iteration}
    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 200:
        data = response.json()
        return response.status_code, data
    else:
        return response.status_code, response


def generate_LLM_experiment_responses(root_dir, instruct_head, contents, short_contents, append_contents, experiment_filename, temperature, top_p, LLM_engine, iteration, include_addition, skip_engines):
    #language_key = "//cs"
    max_tokens = 2048

    #llm_exp_response = os.path.join(root_dir, "response")
    #if os.path.exists(llm_exp_response):
        #print("Already generate response before.")
        #os.rename(llm_exp_response, os.path.join(root_dir, "response_old"+"_"+datetime.now()))

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
                    if engine == "gpt-3.5-turbo":
                        if "new_allocation_in_update" in root_dir:
                            print("Call GPT-3.5-Turbo with no stop words.")
                            status, data = generate_gpt35_requests_no_stop_word(
                                instruct_head,
                                prompt_text,
                                temperature,
                                top_p,
                                max_tokens,
                                iteration
                            )

                        else:
                            status, data = generate_gpt35_requests(
                                instruct_head,
                                prompt_text,
                                temperature,
                                top_p,
                                max_tokens,
                                iteration,
                                "\n\t}"
                            )

                        if (status == 200):
                            print("LLM responses collected.")
                            break
                        else:
                            prompt_text = short_contents
                            print("Waiting 30 seconds and trying again")
                            time.sleep(30)
                            continue

                    if engine == "gpt-4-turbo":
                        status, data = generate_gpt4_requests(
                            instruct_head,
                            prompt_text,
                            temperature,
                            top_p,
                            max_tokens,
                            iteration,
                            "\n\t}"
                        )

                        if (status == 200):
                            print("LLM responses collected.")
                            break
                        else:
                            prompt_text = short_contents
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
                        prompt_contents_clean = re.sub(r'/\*\*.*?\*\*/', '', prompt_contents, flags=re.DOTALL)

                    with open(append_contents_path, 'r', encoding='utf8') as f2:
                        append_contents = f2.read()

                    with open(head_contents_path, 'r', encoding='utf8') as f3:
                        instruct_head = f3.read()

                    with open(short_prompt_contents_path, 'r', encoding='utf8') as f4:
                        short_prompt_contents = f4.read()
                        short_prompt_contents_clean = re.sub(r'/\*\*.*?\*\*/', '', short_prompt_contents, flags=re.DOTALL)


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

if __name__ == "__main__":
    #path = r'.\experiment\unity\transform_rigidbody_in_update'
    path_lis = [ r'.\experiment\cwe', r'.\experiment\unity\new_allocation_in_update', r'.\experiment\unity\transform_rigidbody_in_update']

    skip_engines = []
    for path in path_lis:
        prepare_LLM_experiment_requests(path, skip_engines)
