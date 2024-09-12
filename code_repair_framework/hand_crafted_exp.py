import os
import json
import config
import time
import requests
from datetime import datetime
import re
import experiment_run_codeql_test

#os.environ["OPENAI_API_KEY"] = config.OPENAI_API_KEY
apiKey = config.OPENAI_API_KEY
basicUrl = "https://chatgpt.hkbu.edu.hk/general/rest"
modelName_gpt35 = "gpt-35-turbo"
apiVersion = "2024-02-15-preview"
modelName_gpt4 = "gpt-4-turbo"


def generate_gpt4_requests_no_stop_word(instruct_head, prompt, temp, top_p, max_tokens, iteration):
    url = basicUrl + "/deployments/" + modelName_gpt4 + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": instruct_head + '\n"""\n' + prompt + '\n"""\n'}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'max_tokens': max_tokens, 'n': iteration}
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


def generate_LLM_experiment_responses(root_dir, instruct_head, contents, experiment_filename, temperature, top_p, LLM_engine, iteration, skip_engines):
    #language_key = "//cs"
    max_tokens = 2048

    llm_responses_dir = os.path.join(root_dir, "response",
                                       experiment_filename + ".llm_responses")
    if not os.path.exists(llm_responses_dir):
        os.makedirs(llm_responses_dir)

    prompt_text = contents

    prompt_file = os.path.join(llm_responses_dir, config.PROMPT_TEXT_FILENAME)
    with open(prompt_file, "w", encoding='utf8') as f:
        f.write(prompt_text)


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
                print("Response already exists. Skip.")
                skip = True
            while not skip:
                    print(
                        "Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
                            llm_responses_dir, experiment_filename, temperature, top_p, engine, max_tokens))
                    if engine == "gpt-3.5-turbo":
                        status, data = generate_gpt35_requests_no_stop_word(
                                instruct_head,
                                prompt_text,
                                temperature,
                                top_p,
                                max_tokens,
                                iteration
                            )


                        if (status == 200):
                            print("LLM responses collected.")
                            break
                        else:
                            print("Waiting 30 seconds and trying again")
                            time.sleep(30)
                            continue

                    if engine == "gpt-4-turbo":
                        status, data = generate_gpt4_requests_no_stop_word(
                            instruct_head,
                            prompt_text,
                            temperature,
                            top_p,
                            max_tokens,
                            iteration
                        )

                        if (status == 200):
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
    comment_key = "//"

    for path, dir_lis, file_lis in files:
        #print(path)
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    hand_crafted_prompt = os.path.join(path, "hand_crafted_prompt.txt")
                    prompt = scenario_contents["prompt_template"]


                    head_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".head" + file_extension
                    head_contents_path = os.path.join(path, head_contents_file_name)

                    prepend_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".prepend" + file_extension
                    prepend_contents_path = os.path.join(path, prepend_contents_file_name)
                    prepend_contents_file_name_add = scenario_contents["err_detailed_info"]['add_file_name'].split('/')[-1] + ".prepend_add" + file_extension
                    prepend_contents_path_add = os.path.join(path, prepend_contents_file_name_add)
                    prompt_contents_file_name = scenario_contents["err_detailed_info"]['file_name'].split('/')[-1] + ".prompt" + file_extension
                    prompt_contents_path = os.path.join(path, prompt_contents_file_name)
                    prompt_contents_file_name_add = scenario_contents["err_detailed_info"]['add_file_name'].split('/')[-1] + ".prompt_add" + file_extension
                    prompt_contents_path_add = os.path.join(path, prompt_contents_file_name_add)

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



                    if file_name == add_file_name:


                        prompt_head = comment_key + "Here're the buggy code lines from " + file_name + ":\n"
                        if "fix_instruction_add_prompt_assymetrical" in prompt:
                            prompt_head = "/* Here're the buggy code lines from " + file_name + ":*/\n"
                        #prompt_foot = "\n" + comment_key + " FIXED CODE:\n"
                        prompt_lines = prompt_head + prepend_contents + prompt_contents + prompt_contents_add
                        with open(hand_crafted_prompt, 'w', encoding='utf8') as f5:
                            f5.write(prompt_lines)

                    else:
                        prompt_head = "\n" + comment_key + "Here're the buggy code lines from " + file_name + ":\n"

                        prompt_mid = comment_key + "Here's the definition of function call in another component.\n" + \
                                     comment_key + "Related code from " + add_file_name + ":\n"

                        if "fix_instruction_add_prompt_assymetrical" in prompt:
                            prompt_head = "\n" + "/* Here're the buggy code lines from " + file_name + ":*/\n"

                            prompt_mid = "/* Here's the definition of function call in another component.\n" + \
                                         "Related code from " + add_file_name + ":*/\n"


                        prompt_lines = prompt_head + prepend_contents + prompt_contents + prompt_mid + prepend_contents_add + prompt_contents_add
                        with open(hand_crafted_prompt, 'w', encoding='utf8') as f5:
                            f5.write(prompt_lines)


                    print("Generating Hand Crafted Prompt.")
                    #print(prompt_lines)

                    with open(hand_crafted_prompt, 'r', encoding='utf8') as f6:
                        prompt_contents = f6.read()

                    with open(head_contents_path, 'r', encoding='utf8') as f7:
                        instruct_head = f7.read()


                    generate_LLM_experiment_responses(
                        path,
                        instruct_head,
                        prompt_contents,
                        scenario_contents["err_detailed_info"]['file_name'].split('/')[-1],
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        skip_engines = skip_engines
                    )

def basic_combine_generated_code_with_existing_append_func(comment_key, contents, append_contents, generated_text, whitespace, generate_mean_logprob_comments = False, mean_logprob = None):
    sen = "Here's the definition of function call in another component."
    if "using UnityEngine" in generated_text:
        new_contents = generated_text[:-1] + "\n" + append_contents
    else:
        new_contents = contents.split(sen)[0] + "\n" + whitespace + generated_text + "\n" + append_contents

        if "* FIXED CODE:" in contents and sen in contents:
            new_contents = contents.split(sen)[0] + "*/\n" + whitespace + generated_text + "\n" + append_contents


    return new_contents


def extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings, iteration, codex_responses_files,
                                       force=False, keep_duplicates=False,
                                       include_append=False,
                                       generate_mean_logprob_comments=False):

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                      experiment_file + ".llm_programs")
    if not os.path.exists(llm_programs_dir):
        os.makedirs(llm_programs_dir)
    if True:
        # load the contents of the prompt
        if True:
            file_path = os.path.join(experiment_dir, "response",
                                     experiment_file + ".llm_responses",
                                     "prompt.txt")
            if not os.path.exists(file_path):
                print("Nothing to extrapolate")
                return
            with open(file_path, "r", encoding='utf8') as f:
                contents = f.read()

            with open(file_path, "r", encoding='utf8') as f2:
                line_contents = f2.readlines()
            whitespace_count = len(line_contents[-1])-len(line_contents[-1].lstrip())
            whitespace = line_contents[-1][:whitespace_count]

            append_contents = ""
            if include_append:
                append_file_path = os.path.join(experiment_dir,experiment_file+".append.cs")
                if (os.path.exists(append_file_path)):
                    with open(append_file_path, "r", encoding='utf-8') as f:
                        append_contents = f.read()

        # comment key
        comment_key = "//"

        codex_responses_files.sort()

        unique_outputs = {}

        temp_top_p_regex = re.compile(
            r'^(gpt-3.5-turbo|gpt-4-turbo|code-llama|starcoder)\.temp-(\d+\.\d+).*\.top_p-(\d+\.\d+)')

        # for each experiment, concatenate the lines of the choices to the original file
        # and save in config.CODEX_PROGRAMS_DIRNAME

        new_files = []

        for codex_response_file in codex_responses_files:
            print("Extrapolating", codex_response_file)
            codex_responses_file_full = os.path.join(experiment_dir, "response",
                                                     experiment_file + ".llm_responses",
                                                     codex_response_file)

            # This would load the contents of the "actual" prompt for extrapolation, but remember
            # that the "actual" prompt is not the same as the "original" prompt, from which
            # the overall program should be derived.
            # codex_responses_file_prompt_full = codex_responses_file_full + config.ACTUAL_PROMPT_FILENAME_SUFFIX
            # with open(codex_responses_file_prompt_full, "r") as f:
            #     contents = f.read()

            # extract the temperature/top_p from the filename using regex, format is .temp-x.xx.top_p-x.xx.
            temp_top_p_match = temp_top_p_regex.search(codex_response_file)
            if temp_top_p_match is None:
                print("Could not find temp/top_p in filename", codex_response_file)
                continue

            engine = temp_top_p_match.group(1)

            temp = temp_top_p_match.group(2)
            top_p = temp_top_p_match.group(3)

            if not os.path.exists(codex_responses_file_full):
                print("This dir couldn't find the file %s"%(codex_responses_file_full))
                continue

            with open(codex_responses_file_full, "r", encoding='utf8') as f:

                codex_response = json.loads(f.read())
                # for each choice
                index = 0

                # create the codex_programs_dir if it does not exist
                if not os.path.exists(llm_programs_dir):
                    os.makedirs(llm_programs_dir)


                # check if the filename begins with "cushman-codex" or "davinci-codex"
                if codex_response_file.startswith("gpt-3.5-turbo") or codex_response_file.startswith("gpt-4-turbo"):
                    for choice in codex_response['choices']:
                        codex_programs_file = codex_response_file + "." + str(index) + "." + experiment_extension

                        choice_txt = choice['message']['content']

                        p1 = re.compile(r'```(.*?)```', re.S)
                        content = re.findall(p1, choice_txt)
                        new_choice_text = ""
                        #print(content)
                        if content != []:
                            for txt in content:
                                new_choice_text += txt
                            choice_txt = new_choice_text




                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#', "", choice_txt, re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        ####choice_txt_clean = re.search(r"```csharp\n(.*?)\n```", choice_txt, re.DOTALL)


                        choice_txt = choice_txt_clean

                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        new_contents = basic_combine_generated_code_with_existing_append_func(comment_key, contents,
                                                                                              append_contents,
                                                                                              choice_txt,
                                                                                              whitespace,
                                                                                              generate_mean_logprob_comments=generate_mean_logprob_comments)
                        # queue the codex_programs file
                        new_files.append({
                            'filename': codex_programs_file,
                            'duplicate_of': duplicate_of,
                            'extrapolate_error': extrapolate_error,
                            'experiment_extension': experiment_extension,
                            'contents': new_contents,
                            'engine': engine,
                            'temperature': temp,
                            'top_p': top_p
                        })

                        index += 1

                if codex_response_file.startswith("code-llama") or codex_response_file.startswith("starcoder"):
                    for i in range(0, iteration):
                        codex_programs_file = codex_response_file + "." + str(i) + "." + experiment_extension
                        response_id = "code_repairing_" + str(i)
                        choice_txt = codex_response[response_id]

                        '''
                        p1 = re.compile(r'```(.*?)', re.S)
                        content = re.findall(p1, choice_txt)
                        new_choice_text = ""
                        print(content)
                        if content != []:
                            for txt in content:
                                new_choice_text += txt
                        choice_txt = new_choice_text
                        '''
                        content = choice_txt.split("```")
                        if len(content) > 1:
                            choice_txt = content[1]

                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#', "", choice_txt,
                                                  re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        choice_txt = choice_txt_clean

                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        new_contents = basic_combine_generated_code_with_existing_append_func(comment_key, contents,
                                                                                              append_contents,
                                                                                              choice_txt,
                                                                                              whitespace,
                                                                                              generate_mean_logprob_comments=generate_mean_logprob_comments)
                        # queue the codex_programs file
                        new_files.append({
                            'filename': codex_programs_file,
                            'duplicate_of': duplicate_of,
                            'extrapolate_error': extrapolate_error,
                            'experiment_extension': experiment_extension,
                            'contents': new_contents,
                            'engine': engine,
                            'temperature': temp,
                            'top_p': top_p
                        })

                        index += 1


        for new_file in new_files:
            filename_full = os.path.join(llm_programs_dir, new_file['filename'])
            if not keep_duplicates and new_file["duplicate_of"] is not None:
                continue
            if force == False:
                # check if the file exists
                if os.path.exists(filename_full) or os.path.exists(filename_full + ".reject"):
                    # skip it unless it is empty
                    print("skipping an extraction, run with --force to prevent skips")
                    continue
            if new_file['extrapolate_error'] is True:
                print("skipping, extrapolation error")
                continue

            with open(filename_full, "w", encoding='utf8') as f:
                f.write(new_file['contents'])

        for new_file in new_files:
            new_file.pop('contents')
        # save the new_files to the codex_programs_dir as config.EXTRAPOLATION_METADATA_FILENAME
        with open(os.path.join(llm_programs_dir, "extrapolation_metadata.json"), "w", encoding='utf8') as f:
            f.write(json.dumps(new_files, indent=4))



def extrapolate_all_llm_choices_and_get_all_results(root, response_file):
    files = os.walk(root)
    experiment_extension = "cs"
    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    experiment_dir = path
                    experiment_file = scenario_contents["err_detailed_info"]["file_name"].split("/")[-1]
                    print("Generate LLM choices for later evaluation for %s" % (experiment_dir))
                    if scenario_contents["cwe_name"]:
                        err_name = scenario_contents["cwe_name"]
                    else:
                        err_name = scenario_contents["unity_special_name"]


                    combine_settings = config.CONTEXT_COMBINE_CWE[err_name]
                    iteration = scenario_contents["iteration"]


                    llm_response_dir = os.path.join(experiment_dir, "response",
                                                    experiment_file + ".llm_responses")
                    if os.path.exists(llm_response_dir):
                        extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings, iteration,
                                                         response_file,force=True, keep_duplicates=True,
                                                         include_append=True,generate_mean_logprob_comments=False)
                        #experiment_run_codeql_test.set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file)

                    else:
                        print("no response file, skip")


if __name__ == "__main__":
    path = r'.\experiment\unity\instantiate_destroy_in_update'
    skip_engines = []
    prepare_LLM_experiment_requests(path, skip_engines)

    codex_responses_files = ["gpt-4-turbo.temp-1.00.top_p-1.00.response.json"]
    #extrapolate_all_llm_choices_and_get_all_results(path, codex_responses_files)
