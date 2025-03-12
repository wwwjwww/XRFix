import os

from Tools.scripts.pathfix import add_flags

import config
import json
import re
import numpy as np
import experiment_run_codeql_test
from AST_tree import print_AST
import argparse

def parse_arguments():
    parser = argparse.ArgumentParser()
    parser.add_argument('--response_file', default=None, type=str)
    args = parser.parse_args()

    return args

def basic_combine_generated_code_with_existing_single_line(comment_key, contents, append_contents, generated_text, whitespace):
    #concatenate the choice to the original file
    #print(contents.split('\n')[:-1])
    new_contents = contents + whitespace+ generated_text + "\n" + append_contents


    return new_contents

def basic_combine_generated_code_with_existing_func(comment_key, contents, append_contents, generated_text, whitespace):
    #concatenate the choice to the original file
    function_declaration = print_AST.extract_method_declarations_and_defines(generated_text)
    function_append = ""

    if function_declaration == []:
        new_contents = contents + "\n" + whitespace + "\n" + generated_text + "\n" + append_contents
        return new_contents
    else:
        origin_func = print_AST.extract_method_declarations_and_defines(contents)
        origin_append_contents = print_AST.extract_method_declarations_and_defines(append_contents)
        rm_lis = []

        for func_chg in function_declaration:
            for orig in origin_func:
                if func_chg["methods"] == orig["methods"]:
                    contents = contents.replace(orig["text"], func_chg["text"])
                    rm_lis.append(func_chg)
                    # function_declaration.remove(func_chg)

        for func_chg in function_declaration:
            for orig_ap in origin_append_contents:
                if func_chg["methods"] == orig_ap["methods"]:
                    #append_contents = append_contents.replace(orig_ap["text"], func_chg["text"])
                    rm_lis.append(func_chg)

    for function in function_declaration:
        if function not in rm_lis:
            function_append += function["text"] + "\n"
    new_contents = contents + "\n" + whitespace + "\n" + function_append + "\n" + append_contents
    return new_contents


def special_combine_generated_code_with_component(comment_key, prepend_contents, contents, append_contents, generated_text, whitespace):
    if True:
        function_declaration = print_AST.extract_method_declarations_and_defines(generated_text)
        function_append = ""
        if function_declaration == []:
            new_contents = contents + "\n" + whitespace + "\n" + generated_text + "\n" + append_contents
            return new_contents

        else:
            before_contents = prepend_contents + "\n" + contents + "\n"
            origin_func = print_AST.extract_method_declarations_and_defines(prepend_contents)
            origin_append_contents = print_AST.extract_method_declarations_and_defines(append_contents)

            rm_lis = []

            for func_chg in function_declaration:
                for orig in origin_func:
                    if func_chg["methods"] == orig["methods"]:
                        before_contents = before_contents.replace(orig["text"], func_chg["text"])
                        rm_lis.append(func_chg)
                        #function_declaration.remove(func_chg)

            for func_chg in function_declaration:
                for orig_ap in origin_append_contents:
                    if func_chg["methods"] == orig_ap["methods"]:
                        #append_contents = append_contents.replace(orig_ap["text"], func_chg["text"])
                        rm_lis.append(func_chg)

        for function in function_declaration:
            if function not in rm_lis:
                function_append += function["text"] + "\n"
        new_contents = before_contents + "\n" + whitespace + "\n" + function_append + "\n" + append_contents
        return new_contents


def special_combine_generated_code_with_function(comment_key, prepend_contents, contents, add_contents, append_contents, generated_text, whitespace, add_whitespace, between_contents, add_first, add_append_contents, add_prepend_contents):
    function_declaration = print_AST.extract_method_declarations_and_defines(generated_text)
    function_append = ""
    if function_declaration == []:
        new_contents = prepend_contents + "\n" + contents + "\n" + between_contents + "\n" + whitespace + "\n"  +  generated_text + "\n" + append_contents
        return new_contents

    if "True" in add_first:
        before_contents = add_prepend_contents + "\n" + add_contents + "\n" + between_contents
        origin_func = print_AST.extract_method_declarations_and_defines(before_contents)
        origin_append_contents = print_AST.extract_method_declarations_and_defines(append_contents)


        rm_lis = []

        for func_chg in function_declaration:
            for orig in origin_func:
                if func_chg["methods"] == orig["methods"]:
                    print(func_chg["methods"])
                    before_contents = before_contents.replace(orig["text"], func_chg["text"])
                    rm_lis.append(func_chg)

        for func_chg in function_declaration:
            for orig_ap in origin_append_contents:
                if func_chg["methods"] == orig_ap["methods"]:
                    #append_contents = append_contents.replace(orig_ap["text"], func_chg["text"])
                    rm_lis.append(func_chg)

        for function in function_declaration:
            if function not in rm_lis:
                function_append += function["text"] + "\n"

        new_contents = before_contents + "\n" + add_whitespace + "\n"  + function_append + "\n" + append_contents
    else:
        before_contents = prepend_contents + "\n" + contents + "\n" + between_contents + "\n"
        origin_func = print_AST.extract_method_declarations_and_defines(before_contents)

        origin_add_append_contents = print_AST.extract_method_declarations_and_defines(add_append_contents)

        rm_lis = []

        for func_chg in function_declaration:
            for orig in origin_func:
                if func_chg["methods"] == orig["methods"]:
                    before_contents = before_contents.replace(orig["text"], func_chg["text"])
                    rm_lis.append(func_chg)

        for func_chg in function_declaration:
            for orig_ap in origin_add_append_contents:
                if func_chg["methods"] == orig_ap["methods"]:
                    rm_lis.append(func_chg)

        for function in function_declaration:
            if function not in rm_lis:
                function_append += function["text"] + "\n"

        new_contents = before_contents + "\n" + whitespace + "\n"  + function_append + "\n" + add_append_contents

    return

def determine_which_merge_strategy(combine_settings, include_addition, is_function, comment_key, prepend_contents, contents, add_contents,
                                                                                         append_contents,
                                                                                         choice_txt,
                                                                                         whitespace, add_whitespace,
                                                                                         between_contents,
                                                                                         add_first,
                                                                                         add_append_contents,
                                                                                         add_prepend_contents):
    if combine_settings == "single_line":
        new_contents = basic_combine_generated_code_with_existing_single_line(comment_key, contents,
                                                                              append_contents,
                                                                              choice_txt,
                                                                              whitespace)
    if combine_settings == "function":
        new_contents = basic_combine_generated_code_with_existing_func(comment_key, contents,
                                                                       append_contents,
                                                                       choice_txt,
                                                                       whitespace)
    if combine_settings == "function_special":
        if include_addition:
            if is_function:
                new_contents = special_combine_generated_code_with_function(comment_key, prepend_contents, contents,
                                                                            add_contents,
                                                                            append_contents,
                                                                            choice_txt,
                                                                            whitespace, add_whitespace,
                                                                            between_contents,
                                                                            add_first,
                                                                            add_append_contents,
                                                                            add_prepend_contents)

            else:
                new_contents = special_combine_generated_code_with_component(comment_key, prepend_contents, contents,
                                                                             append_contents,
                                                                             choice_txt,
                                                                             whitespace)

        else:
            new_contents = basic_combine_generated_code_with_existing_func(comment_key, contents,
                                                                           append_contents,
                                                                           choice_txt,
                                                                           whitespace)

    return new_contents



def extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings, codex_responses_files,
                                       iteration, include_addition, is_function, force=False,
                                       keep_duplicates=False, include_append=False):

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                      experiment_file + ".llm_programs")
    if not os.path.exists(llm_programs_dir):
        os.makedirs(llm_programs_dir)

    file_path = os.path.join(experiment_dir, "response",
                                     experiment_file + ".llm_responses",
                                     "prompt.txt")
    if not os.path.exists(file_path):
        print("Nothing to extrapolate")
        return
    if not include_addition:
        with open(file_path, "r", encoding='utf-8') as f:
            contents = f.read()

        with open(file_path, "r", encoding='utf-8') as f2:
            line_contents = f2.readlines()

    else:
        prompt_file_path = os.path.join(experiment_dir,
                                     experiment_file + ".prompt.cs")
        add_prompt_file_path= os.path.join(experiment_dir,
                                     experiment_file + ".prompt_add.cs")
        if (os.path.exists(prompt_file_path)):
            with open(prompt_file_path, "r", encoding='utf-8') as f:
                contents = f.read()
            with open(prompt_file_path, "r", encoding='utf-8') as f2:
                line_contents = f2.readlines()

            whitespace_count = len(line_contents[-1]) - len(line_contents[-1].lstrip())
            whitespace = line_contents[-1][:whitespace_count]

        if (os.path.exists(add_prompt_file_path)):
            with open(add_prompt_file_path, "r", encoding='utf-8') as f3:
                add_contents = f3.read()
            with open(add_prompt_file_path, "r", encoding='utf-8') as f4:
                add_line_contents = f4.readlines()

            add_whitespace_count = len(add_line_contents[-1]) - len(add_line_contents[-1].lstrip())
            add_whitespace = add_line_contents[-1][:add_whitespace_count]


    append_contents = ""
    whitespace = ""
    add_whitespace = ""
    if include_append:
        append_file_path = os.path.join(experiment_dir,
                                     experiment_file + ".append.cs")
        if (os.path.exists(append_file_path)):
            with open(append_file_path, "r", encoding='utf-8') as f:
                append_contents = f.read()

        add_append_file_path = os.path.join(experiment_dir,
                                     experiment_file + ".append_add.cs")
        if (os.path.exists(add_append_file_path)):
            with open(add_append_file_path, "r", encoding='utf-8') as f1:
                add_append_contents = f1.read()

        add_prepend_file_path = os.path.join(experiment_dir,
                                     experiment_file + ".prepend_add.cs")
        if (os.path.exists(add_prepend_file_path)):
            with open(add_prepend_file_path, "r", encoding='utf-8') as f2:
                add_prepend_contents = f2.read()

    prepend_file_path = os.path.join(experiment_dir,
                                            experiment_file + ".prepend.cs")
    prepend_contents = ""
    if os.path.exists(prepend_file_path):
        with open(prepend_file_path, "r", encoding='utf-8') as f3:
            prepend_contents = f3.read()

    between_file_path = os.path.join(experiment_dir,
                                            experiment_file + '.between.cs')
    between_contents = ""
    if os.path.exists(between_file_path):
        with open(between_file_path, "r", encoding='utf-8') as f4:
            between_contents = f4.read()
        with open(os.path.join(experiment_dir, experiment_file + '.add_first.txt')) as f5:
            add_first = f5.read()


    if True:

        # comment key
        comment_key = "//"

        #codex_responses_files.sort()

        unique_outputs = {}

        temp_top_p_regex = re.compile(
            r'^(gpt-3.5-turbo|gpt-4-turbo|simple_instruction_result_open_source|starcoder|deepseek)\.temp-(\d+\.\d+).*\.top_p-(\d+\.\d+)')

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
                if codex_response_file.startswith("gpt-3.5-turbo") or codex_response_file.startswith("gpt-4-turbo"):
                    #if codex_response_file.startswith("gpt-3.5-turbo"):
                    try:
                        codex_response = json.loads(codex_response)
                    except:
                        codex_response = codex_response
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

                        if choice_txt == "":
                            choice_txt = "NO USEFUL OUTPUT"
                        if "Update" not in choice_txt:
                            choice_txt = "Doesn't Implement Update Function. ERROR"


                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        new_contents = determine_which_merge_strategy(combine_settings, include_addition, is_function, comment_key, prepend_contents, contents, add_contents,
                                                                                         append_contents,
                                                                                         choice_txt,
                                                                                         whitespace, add_whitespace,
                                                                                         between_contents,
                                                                                         add_first,
                                                                                         add_append_contents,
                                                                                         add_prepend_contents)




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

                if codex_response_file.startswith("code-llama") or codex_response_file.startswith("starcoder") or codex_response_file.startswith("deepseek"):
                    if codex_response == {}:
                        print("No response for this repo:"+experiment_dir)
                        continue

                    for i in range(0, iteration):
                        codex_programs_file = codex_response_file + "." + str(i) + "." + experiment_extension
                        response_id = "code_repairing_" + str(i)
                        choice_txt = codex_response[response_id]

                        p1 = re.compile(r'```(.*?)```', re.S)
                        content = re.findall(p1, choice_txt)
                        new_choice_text = ""
                        # print(content)
                        if content != []:
                            for txt in content:
                                new_choice_text += txt
                            choice_txt = new_choice_text
                        else:
                            content = choice_txt.split("```")
                            if len(content) > 1:
                                choice_txt = content[1]

                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#', "", choice_txt, re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        choice_txt = choice_txt_clean

                        if choice_txt == "":
                            choice_txt = "NO USEFUL OUTPUT"
                        if "Update" not in choice_txt:
                            choice_txt = "Doesn't Implement Update Function. ERROR"


                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        new_contents = determine_which_merge_strategy(combine_settings, include_addition, is_function,
                                                                      comment_key, prepend_contents, contents,
                                                                      add_contents,
                                                                      append_contents,
                                                                      choice_txt,
                                                                      whitespace, add_whitespace,
                                                                      between_contents,
                                                                      add_first,
                                                                      add_append_contents,
                                                                      add_prepend_contents)

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
    llm_engine = response_file[0].split('.temp')[0]
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
                    file_name = scenario_contents["err_detailed_info"]["file_name"]

                    include_addition = scenario_contents["include_addition"]
                    is_function = None

                    if include_addition:
                        add_file_name = scenario_contents["err_detailed_info"]["add_file_name"]
                        if file_name == add_file_name:
                            is_function = True
                        else:
                            is_function = False

                    if True:
                        if os.path.exists(os.path.join(llm_response_dir, response_file[0])):
                            extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, combine_settings,
                                                             response_file,iteration,include_addition,is_function, force=True, keep_duplicates=True,
                                                             include_append=True)
                            experiment_run_codeql_test.set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file, llm_engine)

                        else:
                            print("no response file, skip")

if __name__ == "__main__":
    path_lis = [r'.\experiment\cwe']

    codex_response_files = ["gpt-3.5-turbo.temp-1.00.top_p-1.00.response.json"]

    for path in path_lis:
        extrapolate_all_llm_choices_and_get_all_results(path, codex_response_files)





