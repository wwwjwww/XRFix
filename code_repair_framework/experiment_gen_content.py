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
    assymetric_comment_char_start = '/*'
    assymetric_comment_char_end = '*/'
    detect_function_regex = r'\b(?:private|public|protected|internal|void)(\s+\b\w+)*\[*\]*(\s+\b\w+)*(\w+\s*<[^>]*>\s+)*(where\s+\w+\s*:\s*\w+,)*\s+\b\w+\s*\([^)]*\)\s*{'
    vulnerable_file_contents_str = ''.join(vulnerable_file_contents)
    vulnerable_file_contents_str = vulnerable_file_contents_str.replace('\r', '')

    # find the end of the function openers using the detect_function_regex
    function_def_boundaries = []
    for match in re.finditer(detect_function_regex, vulnerable_file_contents_str, re.MULTILINE):
        function_def_boundaries.append([match.start(), match.end()])

    #print(function_def_boundaries)

    function_def_num_newlines = []
    for function_def_boundary in function_def_boundaries:
        function_def_num_newlines.append(
            [
                vulnerable_file_contents_str.count('\n', 0, function_def_boundary[0]),
                vulnerable_file_contents_str.count('\n', 0, function_def_boundary[1])
            ]
        )
        # print(function_def_num_newlines)
    # print(start_line_index)
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

    # get the number of whitespace chars in the first line of the function index
    vulnerable_function_line = vulnerable_file_contents[vulnerable_function_start_line_num]
    vulnerable_function_line_stripped = vulnerable_function_line.lstrip()

    #print(vulnerable_file_contents[vulnerable_function_start_line_num], vulnerable_file_contents[vulnerable_function_start_line_num+5])

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
        if(i == len(vulnerable_file_contents)-1):
            vulnerable_function_end_index = i
        else:
            vulnerable_function_end_index = i + 1

    #print("Vulnerable function end line:", vulnerable_file_contents[vulnerable_function_end_index])

    #print(vulnerable_function_end_index)
    if vulnerable_file_contents[vulnerable_function_end_index].strip() == '}':
        vulnerable_function_end_index += 1  # we'll include the closing brace in the vulnerable function for C

    # make the prompt lines from 0 to vulnerable_function_index+1 (line after the function)
    vulnerable_file_prepend_lines = vulnerable_file_contents[0:vulnerable_function_start_line_num]

    # IMPORTANT: we assume that the vulnerable function has a newline between function start "{" or ":" and the body
    vulnerable_file_function_def_lines = vulnerable_file_contents[
                                         vulnerable_function_start_line_num:vulnerable_function_end_line_num + 1]

    # make the vulnerable lines from prompt_line_end_index to start_line_index
    vulnerable_file_function_pre_start_lines = vulnerable_file_contents[
                                               vulnerable_function_end_line_num + 1:start_line_index]

    # start line onwards
    vulnerable_file_function_start_lines_to_end = vulnerable_file_contents[
                                                  start_line_index:vulnerable_function_end_index]

    # make the append lines from start_line_index to the end
    vulnerable_file_append_lines = vulnerable_file_contents[vulnerable_function_end_index:]

    #print("function starts on line " + str(vulnerable_function_start_line_num))
    #print(vulnerable_file_function_def_lines)

    return (vulnerable_file_prepend_lines, vulnerable_file_function_def_lines, vulnerable_file_function_pre_start_lines,
            vulnerable_file_function_start_lines_to_end, vulnerable_file_append_lines)

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

                        # identify the function of the bad line
                        start_line_index = int(scenario_contents["err_detailed_info"]['start_line']) - 1
                        bad_line = vulnerable_file_contents[start_line_index]

                        bad_line_whitespace_count = len(bad_line) - len(bad_line.lstrip())

                        whitespace_chars = bad_line[:bad_line_whitespace_count]

                        (vulnerable_file_prepend_lines, vulnerable_file_function_def_lines,
                         vulnerable_file_function_pre_start_lines, vulnerable_file_function_start_lines_to_end,
                         vulnerable_file_append_lines) = get_vulnerable_function_lines(vulnerable_file_contents,
                                                                                       start_line_index)

                        # get the first word of the vulnerable lines
                        first_vulnerable_word = ""
                        for i in range(len(vulnerable_file_function_pre_start_lines)):
                            split = vulnerable_file_function_pre_start_lines[i].strip().split()
                            if len(split) > 0:
                                first_vulnerable_word = split[0]
                                break

                        #derive the language prompt and add it to the prompt
                        if scenario_contents["cwe_name"] != None:
                            bug_name = scenario_contents["cwe_name"]
                        else:
                            if scenario_contents["unity_special_name"] != None:
                                bug_name = scenario_contents["unity_special_name"]
                        language_prompt_head = whitespace_chars + comment_char + " BUG: " +bug_name + "\n" + \
                                whitespace_chars + comment_char + " MESSAGE: " +  scenario_contents["err_detailed_info"]["description"] + "\n"
                        language_prompt_foot = "\n" + whitespace_chars + comment_char + " FIXED VERSION:" + "\n"

                        possible_prompts.append({
                            'name': 'basic_prompt',
                            'filename': scenario_contents["err_detailed_info"]['file_name'],
                            'derived_from_filename': vulnerable_filename_full,
                            'vulnerable_file_prompt_lines': vulnerable_file_prepend_lines + vulnerable_file_function_def_lines,
                            'vulnerable_file_vulnerable_lines': vulnerable_file_function_pre_start_lines + vulnerable_file_function_start_lines_to_end,
                            'vulnerable_file_append_lines': vulnerable_file_append_lines,
                            'language_prompt_head': language_prompt_head,
                            'language_prompt_foot': language_prompt_foot,
                            'whitespace_chars': whitespace_chars,
                            'assymetrical_comments': False
                        })

                    for prompt in possible_prompts:
                        file_extension = '.cs'

                        prompt_name = prompt['name']
                        if prompt_name not in scenario_contents['prompt_template']:
                            continue

                        vulnerable_file_prompt_lines = prompt['vulnerable_file_prompt_lines']
                        vulnerable_file_vulnerable_lines = prompt['vulnerable_file_vulnerable_lines']
                        vulnerable_file_append_lines = prompt['vulnerable_file_append_lines']
                        language_prompt_head = prompt['language_prompt_head']
                        language_prompt_foot = prompt['language_prompt_foot']
                        whitespace_chars = prompt['whitespace_chars']

                        scenario_prompt_filename = prompt['filename'].split('/')[-1] + ".prompt" + file_extension
                        scenario_prompt_filename_full = os.path.join(path, scenario_prompt_filename)
                        scenario_append_filename = prompt['filename'].split('/')[-1] + ".append" + file_extension
                        scenario_append_filename_full = os.path.join(path, scenario_append_filename)

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
                            f.write(language_prompt_foot)

                        # make the scenario append
                        with open(scenario_append_filename_full, 'w', encoding='utf8') as f:
                            f.writelines(vulnerable_file_append_lines)

if __name__ == "__main__":
    path = "./experiment"
    derive_scenarios_for_experiments(path)
