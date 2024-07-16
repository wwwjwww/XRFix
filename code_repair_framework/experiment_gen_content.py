import pandas as pd
import re
import csv
import json
from splinter import Browser
import requests
import time
import config
import os

def get_vulnerable_function_lines(vulnerable_file_contents, start_line_index):
    comment_char = '//'
    detect_function_regex = r'\b((?:private|public|protected|internal|void)(\s+\b\w+)*\[*\]*(\s+\b\w+)*(\w+\s*<[^>]*>\s+)*(where\s+\w+\s*:\s*\w+,)*\s+\b\w+\s*\([^)]*\)\s*{)|\b((?:private|public|protected|internal)(\s+\b\w+)*\[*\]*(\s+\b\w+)*(\w+\s*<[^>]*>\s+)*\s+\b\w+\s*{)|\b((?:private|public|protected|internal|void)(\s+\b\w+(\.)+\b\w+)+\s+\b\w+\s*\([^)]*\)\s*{)|\b((?:private|public|protected|internal|void)+\s+\b\w+\s+\b\w+\s+\b\w+\s*=\s*(.*))|\b(?:private|public|protected|internal|void)\s+\b\w+<.*>(\s+\b\w)*\s*|\b(?:private|public|protected|internal|void)(\s+\b\w)*(.*)'

    vulnerable_file_contents_str = ''.join(vulnerable_file_contents)
    #vulnerable_file_contents_str_clean = re.sub(r'/\*.*?\*/|//.*?\n', '', vulnerable_file_contents_str, flags=re.DOTALL)

    vulnerable_file_contents_str = vulnerable_file_contents_str.replace('\r', '')

    # find the end of the function openers using the detect_function_regex
    function_def_boundaries = []
    for match in re.finditer(detect_function_regex, vulnerable_file_contents_str, re.MULTILINE):
        function_def_boundaries.append([match.start(), match.end()])

    #for match in function_def_boundaries:
        #print(vulnerable_file_contents_str[match[0]:match[1]])


    function_def_num_newlines = []
    for function_def_boundary in function_def_boundaries:
        function_def_num_newlines.append(
            [
                vulnerable_file_contents_str.count('\n', 0, function_def_boundary[0]),
                vulnerable_file_contents_str.count('\n', 0, function_def_boundary[1])
            ]
        )

    closest_newline_index = 0
    closest_newline_value = 0
    for i in range(len(function_def_num_newlines)):
        if function_def_num_newlines[i][1] > start_line_index:
            break
        if function_def_num_newlines[i][1] > closest_newline_value:
            closest_newline_value = function_def_num_newlines[i][1]
            closest_newline_index = i


    vulnerable_function_start_line_num = function_def_num_newlines[closest_newline_index][0]
    vulnerable_function_end_line_num = function_def_num_newlines[closest_newline_index][1]

    if vulnerable_function_start_line_num != vulnerable_function_end_line_num or "{" in vulnerable_file_contents[vulnerable_function_end_line_num]:
        # get the number of whitespace chars in the first line of the function index
        vulnerable_function_line = vulnerable_file_contents[vulnerable_function_start_line_num]
        vulnerable_function_line_stripped = vulnerable_function_line.lstrip()
        #print(vulnerable_function_line)
        #print(vulnerable_function_line_stripped)

        vulnerable_function_whitespace_count = len(vulnerable_function_line) - len(vulnerable_function_line_stripped)

        #print("Vulnerable function no. of whitespace chars:", vulnerable_function_whitespace_count)

        # if language == 'python':
        # starting at the end function line number, go forwards until you find a non-comment line that has the same indentation level
        vulnerable_function_end_index = 0

        for i in range(vulnerable_function_end_line_num + 1, len(vulnerable_file_contents)):
            line = vulnerable_file_contents[i]
            line_stripped = line.lstrip()

            if len(line_stripped.rstrip()) == 0:
                continue
            if line_stripped[0:len(comment_char)] == comment_char:
                continue

            if len(line) - len(line_stripped) == vulnerable_function_whitespace_count:
                break
            if (i == len(vulnerable_file_contents) - 1):
                vulnerable_function_end_index = i
            else:
                vulnerable_function_end_index = i + 1


        #print(vulnerable_function_end_index)
        #if vulnerable_function_end_index > function_def_num_newlines[closest_newline_index + 1][0]:
        vulnerable_function_end_index_func = vulnerable_function_end_index
        index_next = closest_newline_index + 1
        if index_next < len(function_def_num_newlines):
            function_def_num_start = function_def_num_newlines[closest_newline_index + 1][0]
            function_def_num_end = function_def_num_newlines[closest_newline_index + 1][1]
            if vulnerable_function_end_index_func > function_def_num_start and function_def_num_end != function_def_num_start:
                vulnerable_function_end_index_func = function_def_num_start
        vulnerable_function_end_index_func = vulnerable_function_end_index_func + 1


        #print("Vulnerable function end line:", vulnerable_file_contents[vulnerable_function_end_index])

        #print(vulnerable_function_end_index)
        #if vulnerable_file_contents[vulnerable_function_end_index].strip() == '}':
           #vulnerable_function_end_index += 1  # we'll include the closing brace in the vulnerable function for C
        vulnerable_function_end_index = start_line_index + 1

        vulnerable_file_contents_clean = []
        for contents in vulnerable_file_contents:
            contents_clean = re.sub(r'//.*?\n', '\n', contents, flags=re.DOTALL)
            vulnerable_file_contents_clean.append(contents_clean)

        # make the prompt lines from 0 to vulnerable_function_index+1 (line after the function)
        vulnerable_file_prepend_lines = vulnerable_file_contents_clean[0:vulnerable_function_start_line_num]

        # vulnerable_file_prepend_lines = vulnerable_file_prepend_lines_clean

        # IMPORTANT: we assume that the vulnerable function has a newline between function start "{" or ":" and the body
        vulnerable_file_function_def_lines = vulnerable_file_contents_clean[
                                             vulnerable_function_start_line_num:vulnerable_function_end_line_num + 1]


        # make the vulnerable lines from prompt_line_end_index to start_line_index
        vulnerable_file_function_pre_start_lines = vulnerable_file_contents_clean[
                                                   vulnerable_function_end_line_num + 1:start_line_index]


        # start line onwards
        vulnerable_file_function_start_lines_to_end = vulnerable_file_contents_clean[
                                                      start_line_index:vulnerable_function_end_index]

        vulnerable_file_function_end_func= vulnerable_file_contents_clean[
                                                      vulnerable_function_end_index:vulnerable_function_end_index_func]


        # make the append lines from start_line_index to the end
        vulnerable_file_append_lines = vulnerable_file_contents[vulnerable_function_end_index:]

        vulnerable_file_append_lines_func = vulnerable_file_contents[vulnerable_function_end_index_func:]

        #print(vulnerable_function_end_index)
        #print("function starts on line " + str(vulnerable_function_start_line_num))
        #print(vulnerable_file_function_def_lines)

        return (
        vulnerable_file_prepend_lines, vulnerable_file_function_def_lines, vulnerable_file_function_pre_start_lines,
        vulnerable_file_function_start_lines_to_end, vulnerable_file_append_lines, vulnerable_file_function_end_func, vulnerable_file_append_lines_func )

    else:
        # get the number of whitespace chars in the first line of the function index
        vulnerable_function_line = vulnerable_file_contents[vulnerable_function_start_line_num]
        vulnerable_function_line_stripped = vulnerable_function_line.lstrip()

        vulnerable_function_whitespace_count = len(vulnerable_function_line) - len(vulnerable_function_line_stripped)

        # print("Vulnerable function no. of whitespace chars:", vulnerable_function_whitespace_count)

        # if language == 'python':
        # starting at the end function line number, go forwards until you find a non-comment line that has the same indentation level
        vulnerable_function_end_index = start_line_index + 1

        vulnerable_file_contents_clean = []
        for contents in vulnerable_file_contents:
            contents_clean = re.sub(r'//.*?\n', '\n', contents, flags=re.DOTALL)
            vulnerable_file_contents_clean.append(contents_clean)

        # make the prompt lines from 0 to vulnerable_function_index+1 (line after the function)
        vulnerable_file_prepend_lines = vulnerable_file_contents_clean[0:vulnerable_function_start_line_num]

        # vulnerable_file_prepend_lines = vulnerable_file_prepend_lines_clean

        # IMPORTANT: we assume that the vulnerable function has a newline between function start "{" or ":" and the body
        vulnerable_file_function_def_lines = []

        # make the vulnerable lines from prompt_line_end_index to start_line_index
        vulnerable_file_function_pre_start_lines = vulnerable_file_contents_clean[
                                                   vulnerable_function_end_line_num + 1:start_line_index]

        # start line onwards
        vulnerable_file_function_start_lines_to_end = vulnerable_file_contents_clean[
                                                      start_line_index:vulnerable_function_end_index]

        # make the append lines from start_line_index to the end
        vulnerable_file_append_lines = vulnerable_file_contents[vulnerable_function_end_index:]
        # print(vulnerable_function_end_index)
        # print("function starts on line " + str(vulnerable_function_start_line_num))
        vulnerable_file_function_end_func = []
        vulnerable_file_append_lines_func = []


        return (
            vulnerable_file_prepend_lines, vulnerable_file_function_def_lines, vulnerable_file_function_pre_start_lines,
            vulnerable_file_function_start_lines_to_end, vulnerable_file_append_lines, vulnerable_file_function_end_func, vulnerable_file_append_lines_func)


def derive_scenarios_for_experiments(path):
    comment_char = '//'
    files = os.walk(path)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            possible_prompts = []
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    print("===============start new============")
                    print(os.path.join(path, file))
                    scenario_contents = json.load(f)
                    vulnerable_filename_full = scenario_contents["project_root_dir"]+scenario_contents["err_detailed_info"]["file_name"]

                    with open(vulnerable_filename_full, 'r', encoding="utf8") as f1:
                        vulnerable_file_contents = f1.readlines()

                        defines = []
                        for line in vulnerable_file_contents:
                            if line.startswith('using') or "MonoBehaviour" in line:
                                defines.append(line)

                        if not scenario_contents["include_addition"]:
                            # identify the function of the bad line
                            start_line_index = int(scenario_contents["err_detailed_info"]['start_line']) - 1
                            bad_line = vulnerable_file_contents[start_line_index]


                            bad_line_whitespace_count = len(bad_line) - len(bad_line.lstrip())

                            whitespace_chars = bad_line[:bad_line_whitespace_count]


                            (vulnerable_file_prepend_lines, vulnerable_file_function_def_lines,
                             vulnerable_file_function_pre_start_lines, vulnerable_file_function_start_lines_to_end,
                             vulnerable_file_append_lines, vulnerable_file_function_end_func, vulnerable_file_append_lines_func) = get_vulnerable_function_lines(vulnerable_file_contents,
                                                                                           start_line_index)

                            # get the first word of the vulnerable lines
                            first_vulnerable_word = ""
                            for i in range(len(vulnerable_file_function_pre_start_lines)):
                                split = vulnerable_file_function_pre_start_lines[i].strip().split()
                                if len(split) > 0:
                                    first_vulnerable_word = split[0]
                                    break

                            # derive the language prompt and add it to the prompt
                            if scenario_contents["cwe_name"] != None:
                                bug_name = scenario_contents["cwe_name"]
                            else:
                                if scenario_contents["unity_special_name"] != None:
                                    bug_name = scenario_contents["unity_special_name"]
                            language_prompt_head = whitespace_chars + comment_char + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars + comment_char + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_foot = '\n' + whitespace_chars + comment_char + " FIXED CODE:" + '\n'
                            language_prompt_foot_func = '\n' + comment_char + " FIXED CODE:" + '\n'

                            #instruct_prompt_head = "You're an automated program repair tool. The following Csharp code is based on Unity Development. It has error of" + \
                                                   #bug_name + "The description of this bug is illustrated as below in 'MESSAGE' area. Your task is to fix the code under the 'FIXED CODE:' area. In your response, output code snippet only.\n"
                            instruct_prompt_head = "You're an automated program repair tool. The following Csharp code is based on Unity Development. It has error of: " + \
                            bug_name + ". The description of this bug is illustrated as below in 'MESSAGE' area. Your task is to fix the code under the 'FIXED CODE:' area. In your response, output code snippet only. Do not generate other irelevant code.\n"

                            instruct_prompt_head_2 = "You're an automated program repair tool. The following Csharp code is based on Unity Development. It has error of: " + \
                                                   bug_name + ". The description of this bug is illustrated as below in 'MESSAGE' area. Your task is to fix the code under the 'FIXED CODE:' area. \n"

                            possible_prompts.append({
                                'name': 'basic_prompt',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + ["\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_pre_start_lines + vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': language_prompt_head,
                                'language_prompt_foot': language_prompt_foot,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'basic_prompt_line',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines+vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': language_prompt_head,
                                'language_prompt_foot': language_prompt_foot,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'basic_prompt_line_hand',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': language_prompt_head,
                                'language_prompt_foot': language_prompt_foot,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'basic_prompt_func_hand',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': language_prompt_head,
                                'language_prompt_foot': language_prompt_foot_func,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                        else:
                            start_line_index = int(scenario_contents["err_detailed_info"]['start_line']) - 1
                            bad_line = vulnerable_file_contents[start_line_index]

                            start_line_index_add = int(scenario_contents["err_detailed_info"]['add_start_line']) - 1

                            add_vulnerable_file_full = scenario_contents["project_root_dir"] + \
                                                       scenario_contents["err_detailed_info"]["add_file_name"]

                            with open(add_vulnerable_file_full, 'r') as f_add:
                                add_vulnerable_contents = f_add.readlines()

                            bad_line_add = add_vulnerable_contents[start_line_index_add]

                            bad_line_whitespace_count = len(bad_line) - len(bad_line.lstrip())
                            add_bad_line_whitespace_count = len(bad_line_add) - len(bad_line_add.lstrip())

                            whitespace_chars = bad_line[:bad_line_whitespace_count]
                            add_whitespace_chars = bad_line_add[:add_bad_line_whitespace_count]

                            (vulnerable_file_prepend_lines, vulnerable_file_function_def_lines,
                             vulnerable_file_function_pre_start_lines, vulnerable_file_function_start_lines_to_end,
                             vulnerable_file_append_lines, vulnerable_file_function_end_func,
                             vulnerable_file_append_lines_func) = get_vulnerable_function_lines(vulnerable_file_contents,
                                                                                           start_line_index)

                            (add_vulnerable_file_prepend_lines, add_vulnerable_file_function_def_lines,
                             add_vulnerable_file_function_pre_start_lines, add_vulnerable_file_function_start_lines_to_end,
                             add_vulnerable_file_append_lines, add_vulnerable_file_function_end_func,
                             add_vulnerable_file_append_lines_func) = get_vulnerable_function_lines(add_vulnerable_contents,
                                                                                           start_line_index_add)

                            if scenario_contents["cwe_name"] != None:
                                bug_name = scenario_contents["cwe_name"]
                            else:
                                if scenario_contents["unity_special_name"] != None:
                                    bug_name = scenario_contents["unity_special_name"]
                            language_prompt_head = whitespace_chars + comment_char + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars + comment_char + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_foot = '\n' + whitespace_chars + comment_char + " FIXED CODE:" + '\n'
                            language_prompt_foot_func = '\n' + comment_char + " FIXED CODE:" + '\n'

                            instruct_prompt_head = "You're an automated program repair tool. The following Csharp code is based on Unity Development. It has error of " + \
                                                   bug_name + ". The description of this bug is illustrated as below in 'MESSAGE' area. Your task is to fix the code.\n"

                            possible_prompts.append({
                                'name': 'add_prompt',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'add_filename': scenario_contents["err_detailed_info"]['add_file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prepend_lines': vulnerable_file_prepend_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': language_prompt_head,
                                'language_prompt_foot': language_prompt_foot_func,
                                'add_vulnerable_file_prepend_lines': add_vulnerable_file_prepend_lines,
                                'add_vulnerable_file_prompt_lines': add_vulnerable_file_function_def_lines + add_vulnerable_file_function_pre_start_lines,
                                'add_vulnerable_file_vulnerable_lines': add_vulnerable_file_function_start_lines_to_end,
                                'add_vulnerable_file_append_lines': add_vulnerable_file_append_lines,
                                'add_vulnerable_file_function_end_func': add_vulnerable_file_function_end_func,
                                'add_vulnerable_file_append_lines_func': add_vulnerable_file_append_lines_func,
                                'whitespace_chars': whitespace_chars,
                                'add_whitespace_chars': add_whitespace_chars,
                                'assymetrical_comments': False
                            })
                            #print(possible_prompts["vulnerable_file_prompt_lines"])

                    for prompt in possible_prompts:
                        file_extension = '.cs'

                        prompt_name = prompt['name']
                        if prompt_name not in scenario_contents['prompt_template']:
                            continue

                        if prompt_name == "add_prompt":
                            vulnerable_file_prepend_lines = prompt['vulnerable_file_prepend_lines']
                            vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                            vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                            vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                            language_prompt_head = prompt['language_prompt_head']
                            language_prompt_foot = prompt['language_prompt_foot']
                            whitespace_chars = prompt['whitespace_chars']
                            instruct_prompt_head = prompt["instruct_prompt_head"]
                            vulnerable_file_function_end_func = prompt["vulnerable_file_function_end_func"]
                            vulnerable_file_append_lines_func = prompt["vulnerable_file_append_lines_func"]

                            add_vulnerable_file_prompt_lines = prompt['add_vulnerable_file_prompt_lines']
                            add_vulnerable_file_prepend_lines = prompt['add_vulnerable_file_prepend_lines']
                            add_vulnerable_file_vulnerable_lines = prompt['add_vulnerable_file_vulnerable_lines']
                            add_vulnerable_file_append_lines = prompt['add_vulnerable_file_append_lines']
                            add_vulnerable_file_function_end_func = prompt['add_vulnerable_file_function_end_func']
                            add_vulnerable_file_append_lines_func = prompt['add_vulnerable_file_append_lines_func']

                            add_whitespace_chars = prompt['add_whitespace_chars']

                            scenario_instruct_filename = prompt['filename'].split('/')[-1] + ".head" + file_extension
                            scenario_instruct_filename_full = os.path.join(path, scenario_instruct_filename)
                            scenario_prepend_filename = prompt['filename'].split('/')[-1] + ".prepend" + file_extension
                            scenario_prepend_filename_full = os.path.join(path, scenario_prepend_filename)
                            scenario_prompt_filename = prompt['filename'].split('/')[-1] + ".prompt" + file_extension
                            scenario_prompt_filename_full = os.path.join(path, scenario_prompt_filename)
                            scenario_append_filename = prompt['filename'].split('/')[-1] + ".append" + file_extension
                            scenario_append_filename_full = os.path.join(path, scenario_append_filename)
                            # print(scenario_prompt_filename_full)

                            add_scenario_prepend_filename = prompt['add_filename'].split('/')[-1] + ".prepend_add" + file_extension
                            add_scenario_prepend_filename_full = os.path.join(path, add_scenario_prepend_filename)
                            add_scenario_prompt_filename = prompt['add_filename'].split('/')[-1] + ".prompt_add" + file_extension
                            add_scenario_prompt_filename_full = os.path.join(path, add_scenario_prompt_filename)
                            add_scenario_append_filename = prompt['add_filename'].split('/')[-1] + ".append_add" + file_extension
                            add_scenario_append_filename_full = os.path.join(path, add_scenario_append_filename)


                            with open(scenario_instruct_filename_full, 'w', encoding='utf8') as f1:
                                f1.write(instruct_prompt_head)

                            with open(scenario_prepend_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_prepend_lines)

                            # make the scenario prompt
                            with open(scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_prompt_lines)
                                f.write(language_prompt_head)
                                for line in vulnerable_file_vulnerable_lines:

                                    # TODO: add option to reproduce only the comments??

                                    if prompt["assymetrical_comments"]:
                                        f.write(whitespace_chars + " * " + line.replace('/*', '//').replace('*/', ''))
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + line)
                                for end_line in vulnerable_file_function_end_func:
                                    f.write(end_line)
                                f.write(language_prompt_foot)

                            # make the scenario append
                            with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_append_lines_func)

                            ####ADDITIONAL SCENARIO
                            # make the scenario prompt
                            with open(add_scenario_prepend_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(add_vulnerable_file_prepend_lines)

                            with open(add_scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(add_vulnerable_file_prompt_lines)
                                f.write(language_prompt_head)

                                for line in add_vulnerable_file_vulnerable_lines:
                                    f.write(add_whitespace_chars + comment_char + " " + line)
                                for end_line in add_vulnerable_file_function_end_func:
                                    f.write(end_line)
                                f.write(language_prompt_foot)

                            # make the scenario append
                            with open(add_scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(add_vulnerable_file_append_lines_func)

                        else:
                            if prompt_name == "basic_prompt":
                                vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                                vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                                vulnerable_file_prompt_lines_short = prompt["vulnerable_file_prompt_lines_short"]
                                vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                                language_prompt_head = prompt['language_prompt_head']
                                language_prompt_foot = prompt['language_prompt_foot']
                                whitespace_chars = prompt['whitespace_chars']
                                instruct_prompt_head = prompt["instruct_prompt_head"]

                                scenario_instruct_filename = prompt['filename'].split('/')[
                                                                 -1] + ".head" + file_extension
                                scenario_instruct_filename_full = os.path.join(path, scenario_instruct_filename)
                                scenario_prompt_filename = prompt['filename'].split('/')[
                                                               -1] + ".prompt" + file_extension
                                scenario_prompt_filename_full = os.path.join(path, scenario_prompt_filename)
                                scenario_append_filename = prompt['filename'].split('/')[
                                                               -1] + ".append" + file_extension
                                scenario_append_filename_full = os.path.join(path, scenario_append_filename)
                                scenario_short_prompt_filename = prompt['filename'].split('/')[
                                                                     -1] + ".short" + file_extension
                                scenario_short_prompt_filename_full = os.path.join(path, scenario_short_prompt_filename)
                                # print(scenario_prompt_filename_full)

                                with open(scenario_instruct_filename_full, 'w', encoding='utf8') as f1:
                                    f1.write(instruct_prompt_head)

                                # make the scenario prompt
                                with open(scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                    f.writelines(vulnerable_file_prompt_lines)
                                    f.write(language_prompt_head)
                                    for line in vulnerable_file_vulnerable_lines:

                                        # TODO: add option to reproduce only the comments??

                                        if prompt["assymetrical_comments"]:
                                            f.write(
                                                whitespace_chars + " * " + line.replace('/*', '//').replace('*/', ''))
                                        else:
                                            f.write(whitespace_chars + comment_char + " " + line)
                                    f.write(language_prompt_foot)

                                with open(scenario_short_prompt_filename_full, 'w', encoding='utf8') as f:
                                    f.writelines(vulnerable_file_prompt_lines_short)
                                    f.write(language_prompt_head)
                                    for line in vulnerable_file_vulnerable_lines:
                                        f.write(whitespace_chars + comment_char + " " + line)
                                    f.write(language_prompt_foot)

                                # make the scenario append
                                with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                    f.writelines(vulnerable_file_append_lines)
                            else:
                                if prompt_name == "basic_prompt_func_hand":
                                    vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                                    vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                                    vulnerable_file_prompt_lines_short = prompt["vulnerable_file_prompt_lines_short"]
                                    vulnerable_file_append_lines_func = prompt['vulnerable_file_append_lines_func']
                                    vulnerable_file_function_end_func = prompt['vulnerable_file_function_end_func']
                                    language_prompt_head = prompt['language_prompt_head']
                                    language_prompt_foot = prompt['language_prompt_foot']
                                    whitespace_chars = prompt['whitespace_chars']
                                    instruct_prompt_head = prompt["instruct_prompt_head"]

                                    scenario_instruct_filename = prompt['filename'].split('/')[
                                                                     -1] + ".head" + file_extension
                                    scenario_instruct_filename_full = os.path.join(path, scenario_instruct_filename)
                                    scenario_prompt_filename = prompt['filename'].split('/')[
                                                                   -1] + ".prompt" + file_extension
                                    scenario_prompt_filename_full = os.path.join(path, scenario_prompt_filename)
                                    scenario_append_filename = prompt['filename'].split('/')[
                                                                   -1] + ".append" + file_extension
                                    scenario_append_filename_full = os.path.join(path, scenario_append_filename)
                                    scenario_short_prompt_filename = prompt['filename'].split('/')[
                                                                         -1] + ".short" + file_extension
                                    scenario_short_prompt_filename_full = os.path.join(path,
                                                                                       scenario_short_prompt_filename)
                                    # print(scenario_prompt_filename_full)

                                    with open(scenario_instruct_filename_full, 'w', encoding='utf8') as f1:
                                        f1.write(instruct_prompt_head)

                                    # make the scenario prompt
                                    with open(scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_prompt_lines)
                                        f.write(language_prompt_head)
                                        for line in vulnerable_file_vulnerable_lines:

                                            # TODO: add option to reproduce only the comments??

                                            if prompt["assymetrical_comments"]:
                                                f.write(
                                                    whitespace_chars + " * " + line.replace('/*', '//').replace('*/',
                                                                                                                ''))
                                            else:
                                                f.write(whitespace_chars + comment_char + " " + line)
                                        for end_line in vulnerable_file_function_end_func:
                                            f.write(end_line)
                                        f.write(language_prompt_foot)

                                    with open(scenario_short_prompt_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_prompt_lines_short)
                                        f.write(language_prompt_head)
                                        for line in vulnerable_file_vulnerable_lines:
                                            f.write(whitespace_chars + comment_char + " " + line)
                                        for end_line in vulnerable_file_function_end_func:
                                            f.write(end_line)
                                        f.write(language_prompt_foot)

                                    # make the scenario append
                                    with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_append_lines_func)

                                else:
                                    vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                                    vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                                    vulnerable_file_prompt_lines_short = prompt["vulnerable_file_prompt_lines_short"]
                                    vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                                    language_prompt_head = prompt['language_prompt_head']
                                    language_prompt_foot = prompt['language_prompt_foot']
                                    whitespace_chars = prompt['whitespace_chars']
                                    instruct_prompt_head = prompt["instruct_prompt_head"]

                                    scenario_instruct_filename = prompt['filename'].split('/')[
                                                                     -1] + ".head" + file_extension
                                    scenario_instruct_filename_full = os.path.join(path, scenario_instruct_filename)
                                    scenario_prompt_filename = prompt['filename'].split('/')[
                                                                   -1] + ".prompt" + file_extension
                                    scenario_prompt_filename_full = os.path.join(path, scenario_prompt_filename)
                                    scenario_append_filename = prompt['filename'].split('/')[
                                                                   -1] + ".append" + file_extension
                                    scenario_append_filename_full = os.path.join(path, scenario_append_filename)
                                    scenario_short_prompt_filename = prompt['filename'].split('/')[
                                                                         -1] + ".short" + file_extension
                                    scenario_short_prompt_filename_full = os.path.join(path,
                                                                                       scenario_short_prompt_filename)
                                    # print(scenario_prompt_filename_full)

                                    with open(scenario_instruct_filename_full, 'w', encoding='utf8') as f1:
                                        f1.write(instruct_prompt_head)

                                    # make the scenario prompt
                                    with open(scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_prompt_lines)
                                        f.write(language_prompt_head)
                                        for line in vulnerable_file_vulnerable_lines:

                                            # TODO: add option to reproduce only the comments??

                                            if prompt["assymetrical_comments"]:
                                                f.write(
                                                    whitespace_chars + " * " + line.replace('/*', '//').replace('*/',
                                                                                                                ''))
                                            else:
                                                f.write(whitespace_chars + comment_char + " " + line)
                                        f.write(language_prompt_foot)

                                    with open(scenario_short_prompt_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_prompt_lines_short)
                                        f.write(language_prompt_head)
                                        for line in vulnerable_file_vulnerable_lines:
                                            f.write(whitespace_chars + comment_char + " " + line)
                                        f.write(language_prompt_foot)

                                    # make the scenario append
                                    with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                        f.writelines(vulnerable_file_append_lines)




if __name__ == "__main__":
    #path = r".\experiment\cwe\container_contents_never_accessed"
    #path = r".\experiment\cwe\container_contents_never_accessed"
    path = r".\experiment\unity\instantiate_destroy_in_update"

    derive_scenarios_for_experiments(path)
