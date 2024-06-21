import os
import json
import config
import time
import requests

#os.environ["OPENAI_API_KEY"] = config.OPENAI_API_KEY
apiKey = config.OPENAI_API_KEY
basicUrl = "https://chatgpt.hkbu.edu.hk/general/rest"
modelName = "gpt-35-turbo"
apiVersion = "2024-02-15-preview"

def generate_gpt35_requests(prompt, temp, top_p, max_tokens, iteration):
    url = basicUrl + "/deployments/" + modelName + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": prompt}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'top_p': top_p, 'max_tokens': max_tokens, 'n': iteration}
    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 200:
        data = response.json()
        return response.status_code, data
    else:
        return response.status_code, response

def generate_LLM_experiment_responses(root_dir, contents, append_contents, experiment_filename, temperature, top_p, LLM_engine, iteration, skip_engines):
    language_key = "//cs"
    max_tokens = 2048
    iteration = 3

    llm_responses_dir = os.path.join(root_dir, "response",
                                       experiment_filename + ".llm_responses")
    if not os.path.exists(llm_responses_dir):
        os.makedirs(llm_responses_dir)

    prompt_text = language_key + "\n" + contents

    prompt_file = os.path.join(llm_responses_dir, config.PROMPT_TEXT_FILENAME)
    with open(prompt_file, "w") as f:
        f.write(prompt_text)

    if append_contents is not None and append_contents != "":
        append_text = append_contents
        append_file = os.path.join(llm_responses_dir, config.APPEND_TEXT_FILENAME)
        with open(append_file, "w") as f:
            f.write(append_text)

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
            while not skip:
                    print(
                        "Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
                            llm_responses_dir, experiment_filename, temperature, top_p, engine, max_tokens))
                    if engine == "gpt-3.5-turbo":
                        status, data = generate_gpt35_requests(
                            prompt_text,
                            temperature,
                            top_p,
                            max_tokens,
                            iteration
                        )

                        if(status == 200):
                            print("LLM responses collected.")
                            break
                        else:
                            if(status == 406):
                                if len(prompt_text.split("\n")) > 15:
                                    print("Removing some tokens and trying again")
                                    actual_prompt_text = prompt_text.split("\n")[15:]  # remove first 6 lines
                                    prompt_text = language_key + "\n" + "\n".join(actual_prompt_text)
                                    time.sleep(15)
                                    continue

                            else:
                                print("Waiting 30 seconds and trying again")
                                time.sleep(30)
                                continue

            if not skip:
                # create codex_responses file in the experiment dir
                with open(codex_responses_file, "w") as f:
                    f.write(json.dumps(data, indent=4))
                break

            actual_prompt_file = codex_responses_file + ".actual_prompt.txt"
            with open(actual_prompt_file, "w") as f:
                f.write(prompt_text)

def prepare_LLM_experiment_requests(path, skip_engines=[]):
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

                    with open(prompt_contents_path, 'r', encoding='utf8') as f1:
                        prompt_contents = f1.read()

                    with open(append_contents_path, 'r', encoding='utf8') as f2:
                        append_contents = f2.read()

                    generate_LLM_experiment_responses(
                        path,
                        prompt_contents,
                        append_contents,
                        scenario_contents["err_detailed_info"]['file_name'].split('/')[-1],
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        skip_engines = skip_engines
                    )

if __name__ == "__main__":
    path = './experiment'
    skip_engines = []
    prepare_LLM_experiment_requests(path, skip_engines)
