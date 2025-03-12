import pandas as pd
import re
import csv
import json
import requests
import time
import config
import os

# Use regex to determine the buggy functions
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

    # if '{' is in the second line
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


        #print(vulnerable_function_end_index)
        #print(vulnerable_function_end_index_func)
        # make the append lines from start_line_index to the end
        vulnerable_file_append_lines = vulnerable_file_contents[vulnerable_function_end_index:]

        vulnerable_file_append_lines_func = vulnerable_file_contents[vulnerable_function_end_index_func:]


        return (
        vulnerable_file_prepend_lines, vulnerable_file_function_def_lines, vulnerable_file_function_pre_start_lines,
        vulnerable_file_function_start_lines_to_end, vulnerable_file_append_lines, vulnerable_file_function_end_func, vulnerable_file_append_lines_func )

    # if '{' is not in the second line
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

#create prompt contents for each bug scenario
def derive_scenarios_for_experiments(path,):
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
                            if line.startswith('using'):
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

                            # derive the language prompt and add it to the prompt
                            if scenario_contents["cwe_name"] != None:
                                bug_name = scenario_contents["cwe_name"]
                            else:
                                if scenario_contents["unity_special_name"] != None:
                                    bug_name = scenario_contents["unity_special_name"]
                            language_prompt_head = whitespace_chars + comment_char + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars + comment_char + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_head_func = whitespace_chars + comment_char + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars  + comment_char + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_head_assym = whitespace_chars + "/*" + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars  + "*" + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_head_assym_func = whitespace_chars + "*" + " BUG: " + bug_name + "\n" + \
                                                   whitespace_chars  + "*" + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_foot = '\n' + whitespace_chars + comment_char + " FIXED CODE:" + '\n'
                            language_prompt_foot_func = '\n' + whitespace_chars + comment_char + " FIXED CODE:" + '\n'
                            language_prompt_foot_assym = '\n' + whitespace_chars + "*" + " FIXED CODE:\n" + whitespace_chars + "*/\n"

                            #instruct_prompt_head = "You're an automated program repair tool. The following Csharp code is based on Unity Development. It has error of" + \
                                                   #bug_name + "The description of this bug is illustrated as below in 'MESSAGE' area. Your task is to fix the code under the 'FIXED CODE:' area. In your response, output code snippet only.\n"
                            instruct_prompt_head = "You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.\n"

                            instruct_prompt_head_2 = "You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the code under the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents.\n"
                            fix_instruction_prompt = "\n" + whitespace_chars + comment_char + config.FIX_INSTRUCTION[bug_name]
                            fix_instruction_prompt_func = "\n" + whitespace_chars + comment_char + " " + config.FIX_INSTRUCTION[bug_name]
                            fix_instruction_prompt_assym = "\n" + whitespace_chars + "* " + config.FIX_INSTRUCTION[bug_name]

                            possible_prompts.append({
                                'name': 'simple_prompt_line',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': "",
                                'fix_instruction_prompt': None,
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
                                'fix_instruction_prompt': None,
                                'language_prompt_foot': language_prompt_foot,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'fix_instruction_line',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': language_prompt_head,
                                'fix_instruction_prompt': fix_instruction_prompt,
                                'language_prompt_foot': language_prompt_foot,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'fix_instruction_line_assymetrical',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"] + vulnerable_file_function_def_lines,
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'language_prompt_head': language_prompt_head_assym,
                                'fix_instruction_prompt': fix_instruction_prompt_assym,
                                'language_prompt_foot': language_prompt_foot_assym,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': True
                            })


                            possible_prompts.append({
                                'name': 'simple_prompt_func_no_limit',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"],
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines,
                                'vulnerable_file_function_begin_func': vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': "",
                                'fix_instruction_prompt': None,
                                'language_prompt_foot': language_prompt_foot_func,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })



                            possible_prompts.append({
                                'name': 'basic_prompt_func_no_limit',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"],
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines,
                                'vulnerable_file_function_begin_func': vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines ,
                                'vulnerable_file_vulnerable_lines':  vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': language_prompt_head_func,
                                'fix_instruction_prompt': None,
                                'language_prompt_foot': language_prompt_foot_func,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })


                            possible_prompts.append({
                                'name': 'fix_instruction_func_no_limit',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"],
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines,
                                'vulnerable_file_function_begin_func': vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': language_prompt_head_func,
                                'fix_instruction_prompt': fix_instruction_prompt_func,
                                'language_prompt_foot': language_prompt_foot_func,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': False
                            })

                            possible_prompts.append({
                                'name': 'fix_instruction_func_no_limit_assymetrical',
                                'filename': scenario_contents["err_detailed_info"]['file_name'],
                                'instruct_prompt_head': instruct_prompt_head_2,
                                'derived_from_filename': vulnerable_filename_full,
                                'vulnerable_file_prompt_lines_short': defines + [
                                    "\n"],
                                'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines,
                                'vulnerable_file_function_begin_func': vulnerable_file_function_def_lines + vulnerable_file_function_pre_start_lines,
                                'vulnerable_file_vulnerable_lines': vulnerable_file_function_start_lines_to_end,
                                'vulnerable_file_function_end_func': vulnerable_file_function_end_func,
                                'vulnerable_file_append_lines': vulnerable_file_append_lines,
                                'vulnerable_file_append_lines_func': vulnerable_file_append_lines_func,
                                'language_prompt_head': language_prompt_head_assym_func,
                                'fix_instruction_prompt': fix_instruction_prompt_assym,
                                'language_prompt_foot': language_prompt_foot_assym,
                                'whitespace_chars': whitespace_chars,
                                'assymetrical_comments': True
                            })



                        else:
                            start_line_index = int(scenario_contents["err_detailed_info"]['start_line']) - 1
                            bad_line = vulnerable_file_contents[start_line_index]

                            start_line_index_add = int(scenario_contents["err_detailed_info"]['add_start_line']) - 1

                            add_vulnerable_file_full = scenario_contents["project_root_dir"] + \
                                                       scenario_contents["err_detailed_info"]["add_file_name"]

                            with open(add_vulnerable_file_full, 'r', encoding='utf8') as f_add:
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

                            vulnerable_between_lines = ""
                            add_first = None


                            if scenario_contents["err_detailed_info"]['file_name'] == scenario_contents["err_detailed_info"]['add_file_name']:
                                if len(add_vulnerable_file_prepend_lines) > len(vulnerable_file_prepend_lines):
                                    vulnerable_between_lines = add_vulnerable_file_prepend_lines[len(vulnerable_file_prepend_lines+vulnerable_file_function_def_lines+vulnerable_file_function_pre_start_lines+vulnerable_file_function_start_lines_to_end+vulnerable_file_function_end_func):]
                                    add_first = False
                                else:
                                    vulnerable_between_lines = vulnerable_file_prepend_lines[len(add_vulnerable_file_prepend_lines+add_vulnerable_file_function_def_lines+add_vulnerable_file_function_pre_start_lines+add_vulnerable_file_function_start_lines_to_end+add_vulnerable_file_function_end_func):]
                                    add_first = True


                            if scenario_contents["cwe_name"] != None:
                                bug_name = scenario_contents["cwe_name"]
                            else:
                                if scenario_contents["unity_special_name"] != None:
                                    bug_name = scenario_contents["unity_special_name"]
                            language_prompt_head = comment_char + " BUG: " + bug_name + "\n" + \
                                                   comment_char + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_head_assym = "/*" + " BUG: " + bug_name + "\n" + \
                                                   "*" + " MESSAGE: " + \
                                                   scenario_contents["err_detailed_info"]["description"] + "\n"
                            language_prompt_foot_func = '\n'+ comment_char + " FIXED CODE:" + '\n'
                            language_prompt_foot_assym = '\n'+ "*" + " FIXED CODE:\n" + '*/\n'
                            fix_instruction_prompt_func = "\n" + comment_char + " " + config.FIX_INSTRUCTION[bug_name]
                            fix_instruction_prompt_assym = "\n" + "* " + config.FIX_INSTRUCTION[bug_name]


                            instruct_prompt_head = "You're an automated program repair tool. The following C# code is based on Unity Development. Your task is to fix the codeunder the 'FIXED CODE:' area. In your response, only include your fixed code snippets. Do not output the original contents. Please only change the code from "+scenario_contents["err_detailed_info"]["file_name"]+"\n"

                            possible_prompts.append({
                                'name': 'simple_add_prompt',
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
                                'language_prompt_head': "",
                                'fix_instruction_prompt': None,
                                'language_prompt_foot': language_prompt_foot_func,
                                'add_vulnerable_file_prepend_lines': add_vulnerable_file_prepend_lines,
                                'add_vulnerable_file_prompt_lines': add_vulnerable_file_function_def_lines + add_vulnerable_file_function_pre_start_lines,
                                'add_vulnerable_file_vulnerable_lines': add_vulnerable_file_function_start_lines_to_end,
                                'add_vulnerable_file_append_lines': add_vulnerable_file_append_lines,
                                'add_vulnerable_file_function_end_func': add_vulnerable_file_function_end_func,
                                'add_vulnerable_file_append_lines_func': add_vulnerable_file_append_lines_func,
                                'whitespace_chars': whitespace_chars,
                                'add_whitespace_chars': add_whitespace_chars,
                                'assymetrical_comments': False,
                                'vulnerable_between_lines': vulnerable_between_lines,
                                'add_first': add_first
                            })


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
                                'fix_instruction_prompt': None,
                                'language_prompt_foot': language_prompt_foot_func,
                                'add_vulnerable_file_prepend_lines': add_vulnerable_file_prepend_lines,
                                'add_vulnerable_file_prompt_lines': add_vulnerable_file_function_def_lines + add_vulnerable_file_function_pre_start_lines,
                                'add_vulnerable_file_vulnerable_lines': add_vulnerable_file_function_start_lines_to_end,
                                'add_vulnerable_file_append_lines': add_vulnerable_file_append_lines,
                                'add_vulnerable_file_function_end_func': add_vulnerable_file_function_end_func,
                                'add_vulnerable_file_append_lines_func': add_vulnerable_file_append_lines_func,
                                'whitespace_chars': whitespace_chars,
                                'add_whitespace_chars': add_whitespace_chars,
                                'assymetrical_comments': False,
                                'vulnerable_between_lines': vulnerable_between_lines,
                                'add_first': add_first
                            })

                            possible_prompts.append({
                                'name': 'fix_instruction_add_prompt',
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
                                'fix_instruction_prompt': fix_instruction_prompt_func,
                                'language_prompt_foot': language_prompt_foot_func,
                                'add_vulnerable_file_prepend_lines': add_vulnerable_file_prepend_lines,
                                'add_vulnerable_file_prompt_lines': add_vulnerable_file_function_def_lines + add_vulnerable_file_function_pre_start_lines,
                                'add_vulnerable_file_vulnerable_lines': add_vulnerable_file_function_start_lines_to_end,
                                'add_vulnerable_file_append_lines': add_vulnerable_file_append_lines,
                                'add_vulnerable_file_function_end_func': add_vulnerable_file_function_end_func,
                                'add_vulnerable_file_append_lines_func': add_vulnerable_file_append_lines_func,
                                'whitespace_chars': whitespace_chars,
                                'add_whitespace_chars': add_whitespace_chars,
                                'assymetrical_comments': False,
                                'vulnerable_between_lines': vulnerable_between_lines,
                                'add_first': add_first
                            })

                            possible_prompts.append({
                                'name': 'fix_instruction_add_prompt_assymetrical',
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
                                'language_prompt_head': language_prompt_head_assym,
                                'fix_instruction_prompt': fix_instruction_prompt_assym,
                                'language_prompt_foot': language_prompt_foot_assym,
                                'add_vulnerable_file_prepend_lines': add_vulnerable_file_prepend_lines,
                                'add_vulnerable_file_prompt_lines': add_vulnerable_file_function_def_lines + add_vulnerable_file_function_pre_start_lines,
                                'add_vulnerable_file_vulnerable_lines': add_vulnerable_file_function_start_lines_to_end,
                                'add_vulnerable_file_append_lines': add_vulnerable_file_append_lines,
                                'add_vulnerable_file_function_end_func': add_vulnerable_file_function_end_func,
                                'add_vulnerable_file_append_lines_func': add_vulnerable_file_append_lines_func,
                                'whitespace_chars': whitespace_chars,
                                'add_whitespace_chars': add_whitespace_chars,
                                'assymetrical_comments': True,
                                'vulnerable_between_lines': vulnerable_between_lines,
                                'add_first': add_first
                            })

                            #print(possible_prompts["vulnerable_file_prompt_lines"])
                    #print(possible_prompts)

                    for prompt in possible_prompts:
                        file_extension = '.cs'

                        prompt_name = prompt['name']
                        if prompt_name not in scenario_contents['prompt_template']:
                            continue

                        if prompt_name == "add_prompt" or prompt_name == "fix_instruction_add_prompt" or prompt_name == "fix_instruction_add_prompt_assymetrical" or prompt_name == "simple_add_prompt":
                            vulnerable_file_prepend_lines = prompt['vulnerable_file_prepend_lines']
                            vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                            vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                            vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                            language_prompt_head = prompt['language_prompt_head']
                            language_prompt_foot = prompt['language_prompt_foot']
                            #print(language_prompt_foot)

                            whitespace_chars = prompt['whitespace_chars']
                            instruct_prompt_head = prompt["instruct_prompt_head"]
                            vulnerable_file_function_end_func = prompt["vulnerable_file_function_end_func"]
                            vulnerable_file_append_lines_func = prompt["vulnerable_file_append_lines_func"]
                            fix_instruction_prompt = prompt["fix_instruction_prompt"]

                            add_vulnerable_file_prompt_lines = prompt['add_vulnerable_file_prompt_lines']
                            add_vulnerable_file_prepend_lines = prompt['add_vulnerable_file_prepend_lines']
                            add_vulnerable_file_vulnerable_lines = prompt['add_vulnerable_file_vulnerable_lines']
                            add_vulnerable_file_append_lines = prompt['add_vulnerable_file_append_lines']
                            add_vulnerable_file_function_end_func = prompt['add_vulnerable_file_function_end_func']
                            add_vulnerable_file_append_lines_func = prompt['add_vulnerable_file_append_lines_func']
                            vulnerable_between_lines = prompt['vulnerable_between_lines']

                            add_whitespace_chars = prompt['add_whitespace_chars']
                            add_first = prompt['add_first']

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

                            vulnerable_between_lines_filename = prompt['add_filename'].split('/')[-1] + ".between" + file_extension
                            vulnerable_between_lines_filename_full = os.path.join(path, vulnerable_between_lines_filename)
                            add_first_notice_filename = prompt['add_filename'].split('/')[-1] + ".add_first.txt"
                            add_first_notice_filename_full = os.path.join(path, add_first_notice_filename)




                            with open(scenario_instruct_filename_full, 'w', encoding='utf8') as f1:
                                f1.write(instruct_prompt_head)

                            with open(scenario_prepend_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_prepend_lines)

                            # make the scenario prompt
                            with open(scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                f.write(language_prompt_head)

                                for func_line in vulnerable_file_prompt_lines:
                                    if prompt["assymetrical_comments"]:
                                        f.write("* " + func_line)
                                    else:
                                        f.write(comment_char + func_line)
                                #f.writelines(vulnerable_file_prompt_lines)

                                for line in vulnerable_file_vulnerable_lines:

                                    # TODO: add option to reproduce only the comments??

                                    if prompt["assymetrical_comments"]:
                                        f.write( "* " + line.replace('/*', '//').replace('*/', ''))
                                    else:
                                        f.write(comment_char + line)
                                for end_line in vulnerable_file_function_end_func:
                                    if prompt["assymetrical_comments"]:
                                        f.write("* " + end_line)
                                    else:
                                        f.write(comment_char + end_line)

                                #if prompt['filename'] == prompt['add_filename']:
                                    #if fix_instruction_prompt != None:
                                        #f.write(fix_instruction_prompt)

                                #f.write(language_prompt_foot)

                            # make the scenario append
                            with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_append_lines_func)

                            ####ADDITIONAL SCENARIO
                            # make the scenario prompt
                            with open(add_scenario_prepend_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(add_vulnerable_file_prepend_lines)



                            with open(add_scenario_prompt_filename_full, 'w', encoding='utf8') as f:
                                if vulnerable_file_prompt_lines[0] == add_vulnerable_file_prompt_lines[0]:
                                    if fix_instruction_prompt != None:
                                        f.write(fix_instruction_prompt)
                                    f.write(language_prompt_foot)
                                else:

                                    for add_func_line in add_vulnerable_file_prompt_lines:
                                        if prompt["assymetrical_comments"]:
                                            f.write("* " + add_func_line)
                                        else:
                                            f.write(comment_char + add_func_line)
                                    #f.writelines(add_vulnerable_file_prompt_lines)

                                    for line in add_vulnerable_file_vulnerable_lines:
                                        if prompt["assymetrical_comments"]:
                                            f.write("* " + line)
                                        else:
                                            f.write(comment_char + line)
                                    for end_line in add_vulnerable_file_function_end_func:
                                        if prompt["assymetrical_comments"]:
                                            f.write("* " + end_line)
                                        else:
                                            f.write(comment_char + end_line)
                                    if fix_instruction_prompt != None:
                                        f.write(fix_instruction_prompt)
                                    f.write(language_prompt_foot)

                            # make the scenario append
                            with open(add_scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(add_vulnerable_file_append_lines_func)

                            with open(vulnerable_between_lines_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_between_lines)

                            if add_first != None:
                                with open(add_first_notice_filename_full,'w', encoding='utf8') as f:
                                    f.writelines("add_first="+str(add_first))

                        if prompt_name == "basic_prompt_func_no_limit" or prompt_name == "fix_instruction_func_no_limit" or prompt_name == "fix_instruction_func_no_limit_assymetrical"  or prompt_name == "simple_prompt_func_no_limit":
                            vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                            vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                            vulnerable_file_prompt_lines_short = prompt["vulnerable_file_prompt_lines_short"]
                            vulnerable_file_append_lines_func = prompt['vulnerable_file_append_lines_func']
                            vulnerable_file_function_end_func = prompt['vulnerable_file_function_end_func']
                            vulnerable_file_function_begin_func = prompt['vulnerable_file_function_begin_func']
                            language_prompt_head = prompt['language_prompt_head']
                            language_prompt_foot = prompt['language_prompt_foot']
                            whitespace_chars = prompt['whitespace_chars']
                            instruct_prompt_head = prompt["instruct_prompt_head"]
                            fix_instruction_prompt = prompt["fix_instruction_prompt"]


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
                                f.write("/")
                                for func_line in vulnerable_file_function_begin_func:
                                    if prompt["assymetrical_comments"]:
                                        f.write("* " + func_line)
                                    else:
                                        f.write(comment_char + " " + func_line)
                                    #f.write(func_line)
                                f.write(language_prompt_head)

                                for line in vulnerable_file_vulnerable_lines:

                                    # TODO: add option to reproduce only the comments??

                                    if prompt["assymetrical_comments"]:
                                        f.write(
                                            whitespace_chars + "* " + line.replace('/*', '//').replace('*/',
                                                                                                        ''))
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + line)
                                for end_line in vulnerable_file_function_end_func:
                                    if prompt["assymetrical_comments"]:
                                        f.write(whitespace_chars + "* " + end_line)
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + end_line)

                                if fix_instruction_prompt != None:

                                    f.write(fix_instruction_prompt)
                                f.write(language_prompt_foot)

                            with open(scenario_short_prompt_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_prompt_lines_short)

                                f.write("/")
                                for func_line in vulnerable_file_function_begin_func:
                                    if prompt["assymetrical_comments"]:
                                        f.write("* " + func_line)
                                    else:
                                        f.write(comment_char + " " + func_line)

                                f.write(language_prompt_head)

                                for line in vulnerable_file_vulnerable_lines:
                                    if prompt["assymetrical_comments"]:
                                        f.write(whitespace_chars + "* " + line)
                                    else:
                                        f.write(whitespace_chars + comment_char  + " " + line)
                                for end_line in vulnerable_file_function_end_func:
                                    if prompt["assymetrical_comments"]:
                                        f.write(whitespace_chars + "* " + end_line)
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + end_line)

                                if fix_instruction_prompt != None:
                                    f.write(fix_instruction_prompt)
                                f.write(language_prompt_foot)

                            # make the scenario append
                            with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_append_lines_func)


                        if prompt_name == "basic_prompt_line" or prompt_name == "fix_instruction_line" or prompt_name == "fix_instruction_line_assymetrical" or prompt_name =="simple_prompt_line":
                            vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                            vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                            vulnerable_file_prompt_lines_short = prompt["vulnerable_file_prompt_lines_short"]
                            vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                            language_prompt_head = prompt['language_prompt_head']
                            language_prompt_foot = prompt['language_prompt_foot']
                            whitespace_chars = prompt['whitespace_chars']
                            instruct_prompt_head = prompt["instruct_prompt_head"]
                            fix_instruction_prompt = prompt["fix_instruction_prompt"]

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
                                            whitespace_chars + "* " + line.replace('/*', '//').replace('*/',
                                                                                                        ''))
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + line)
                                if fix_instruction_prompt != None:
                                    f.write(fix_instruction_prompt)
                                f.write(language_prompt_foot)

                            with open(scenario_short_prompt_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_prompt_lines_short)
                                f.write(language_prompt_head)

                                for line in vulnerable_file_vulnerable_lines:
                                    if prompt["assymetrical_comments"]:
                                        f.write(
                                            whitespace_chars + "* " + line.replace('/*', '//').replace('*/',
                                                                                                        ''))
                                    else:
                                        f.write(whitespace_chars + comment_char + " " + line)

                                if fix_instruction_prompt != None:
                                    f.write(fix_instruction_prompt)
                                f.write(language_prompt_foot)

                            # make the scenario append
                            with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                                f.writelines(vulnerable_file_append_lines)




if __name__ == "__main__":
    #path = r".\experiment\unity\transform_rigidbody_in_update"
    #path = (r".\experiment")
    #path = (r".\experiment")
    path = (r".\experiment\unity_real")

    derive_scenarios_for_experiments(path)
