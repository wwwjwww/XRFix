import os
import config
import json
import re
import numpy as np
import experiment_run_codeql_test
def basic_combine_generated_code_with_existing_single_line(comment_key, contents, append_contents, generated_text, whitespace, generate_mean_logprob_comments = False, mean_logprob = None):
    #concatenate the choice to the original file
    #print(contents.split('\n')[:-1])
    new_contents = contents + whitespace+ generated_text + "\n" + append_contents

    if generate_mean_logprob_comments and mean_logprob is not None:
        new_contents = comment_key + "LM generated repair code follows. mean_logprob: " + mean_logprob + "\n" + new_contents
    return new_contents

def basic_combine_generated_code_with_existing_func(comment_key, contents, append_contents, generated_text, whitespace, generate_mean_logprob_comments = False, mean_logprob = None):
    #concatenate the choice to the original file
    new_contents = contents +"\n" + whitespace +"\n" + generated_text + "\n" + append_contents

    if generate_mean_logprob_comments and mean_logprob is not None:
        new_contents = comment_key + "LM generated repair code follows. mean_logprob: " + mean_logprob + "\n" + new_contents
    return new_contents

def basic_combine_generated_code_with_existing_append_func(comment_key, contents, append_contents, generated_text, whitespace, generate_mean_logprob_comments = False, mean_logprob = None):
    if "using" in generated_text:
        new_contents = generated_text[:-1] + "\n" + append_contents
    else:
        new_contents = contents + "\n" + whitespace + "\n" + generated_text + "\n" + append_contents
    return new_contents



def extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings, codex_responses_files,
                                       iteration, force=False,
                                       keep_duplicates=False, include_append=False,
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
            with open(file_path, "r", encoding='utf-8') as f:
                contents = f.read()

            with open(file_path, "r", encoding='utf-8') as f2:
                line_contents = f2.readlines()
            whitespace_count = len(line_contents[-1])-len(line_contents[-1].lstrip())
            whitespace = line_contents[-1][:whitespace_count]

            append_contents = ""
            if include_append:
                append_file_path = os.path.join(experiment_dir, "response",
                                     experiment_file + ".llm_responses",
                                     "append.txt")
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

            with open(codex_responses_file_full, "r") as f:

                codex_response = json.loads(f.read())
                # for each choice
                index = 0

                # create the codex_programs_dir if it does not exist
                if not os.path.exists(llm_programs_dir):
                    os.makedirs(llm_programs_dir)


                # check if the filename begins with "cushman-codex" or "davinci-codex"
                if codex_response_file.startswith("gpt-3.5-turbo"):
                    for choice in codex_response['choices']:
                        codex_programs_file = codex_response_file + "." + str(index) + "." + experiment_extension

                        choice_txt = choice['message']['content']

                        p1 = re.compile(r'```(.*?)\n```', re.S)
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


                        if combine_settings == "single_line":
                            new_contents = basic_combine_generated_code_with_existing_single_line(comment_key, contents,
                                                                                      append_contents,
                                                                                      choice_txt,
                                                                                      whitespace,
                                                                                      generate_mean_logprob_comments=generate_mean_logprob_comments)
                        else:
                            new_contents = basic_combine_generated_code_with_existing_func(comment_key, contents,
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

                if codex_response_file.startswith("gpt-4-turbo"):
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


                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#', "", choice_txt, re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        ####choice_txt_clean = re.search(r"```csharp\n(.*?)\n```", choice_txt, re.DOTALL)


                        choice_txt = choice_txt_clean

                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False


                        if combine_settings == "single_line":
                            new_contents = basic_combine_generated_code_with_existing_single_line(comment_key, contents,
                                                                                      append_contents,
                                                                                      choice_txt,
                                                                                      whitespace,
                                                                                      generate_mean_logprob_comments=generate_mean_logprob_comments)
                        if combine_settings == "function":
                            new_contents = basic_combine_generated_code_with_existing_func(comment_key, contents,
                                                                                                  append_contents,
                                                                                                  choice_txt,
                                                                                                  whitespace,
                                                                                                  generate_mean_logprob_comments=generate_mean_logprob_comments)
                        if combine_settings == "function_special":
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


                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#', "", choice_txt, re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        choice_txt = choice_txt_clean

                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        if combine_settings == "single_line":
                            new_contents = basic_combine_generated_code_with_existing_single_line(comment_key, contents,
                                                                                      append_contents,
                                                                                      choice_txt,
                                                                                      whitespace,
                                                                                      generate_mean_logprob_comments=generate_mean_logprob_comments)
                        else:
                            new_contents = basic_combine_generated_code_with_existing_func(comment_key, contents,
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
        with open(os.path.join(llm_programs_dir, "extrapolation_metadata.json"), "w") as f:
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


                    llm_response_dir = os.path.join(experiment_dir, "response",
                                                    experiment_file + ".llm_responses")
                    iteration = scenario_contents["iteration"]

                    if True:
                        if os.path.exists(llm_response_dir):
                            #extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings,
                                                             #response_file,iteration,force=True, keep_duplicates=True,
                                                             #include_append=True,generate_mean_logprob_comments=False)
                            experiment_run_codeql_test.set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file)

                        else:
                            print("no response file, skip")



if __name__ == "__main__":
    path_lis = [r'.\experiment']

    codex_responses_files = ["gpt-4-turbo.temp-1.00.top_p-1.00.response.json"]
    for path in path_lis:
        extrapolate_all_llm_choices_and_get_all_results(path, codex_responses_files)




