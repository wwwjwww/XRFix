import os
import json
import logging
import csv
import config
import time
import requests
from datetime import datetime
import re
import experiment_run_codeql_test_with_feedback
from experiment_get_result import determine_which_merge_strategy, extrapolate_all_llm_choices_and_get_all_results
import subprocess
import shutil
import experiment_gen_response
import openai
import tiktoken
from experiment_evaluator import long_path_exists

from codeql_analysis_helper import (
    analyze_codeql_execution,
    generate_improved_fix,
    setup_logging,
    get_prompt_contents
)


#os.environ["OPENAI_API_KEY"] = config.OPENAI_API_KEY
apiKey = config.OPENAI_API_KEY_HKBU

basicUrl = "https://genai.hkbu.edu.hk/general/rest"
modelName_gpt35 = "gpt-3.5-turbo"
apiVersion = "2024-02-15-preview"
apiVersion_2 = "2024-10-21"
modelName_gpt4 = "gpt-4o"
modelName_gpt4o = 'gpt-4-o'

def count_prompt_token(max_tokens, model, prompt):
    tokenizer = tiktoken.encoding_for_model(model)
    tokens = tokenizer.encode(prompt)
    if len(tokens) > max_tokens:
        print("Warning: The prompt exceeds the maximum token limit. Token length:" + str(len(tokens)))
        return False
    else:
        print("The prompt is within the token limit:" + str(len(tokens)))
        return True

def generate_gpt4_requests_no_stop_word(instruct_head, prompt, temp, top_p, max_tokens, iteration):
    contents = instruct_head + '\n"""\n' + prompt + '\n"""\n'
    is_within_limit = count_prompt_token(max_tokens, 'gpt-4o', contents)

    if is_within_limit:
        try:
            openai.base_url = config.basicUrl_gpt35
            openai.api_key = config.OPENAI_API_KEY
            response = openai.chat.completions.create(
                model=modelName_gpt4,
                messages=[
                    {"role": "user", "content": contents}
                ],
                temperature=temp,
                max_tokens=max_tokens,
                n=iteration
            )
            data = response.model_dump_json()
            status = 200
            return status, data
        except Exception as e:
            return str(e), {}
    else:
        return "exceed max tokens", {}



def generate_gpt35_requests_no_stop_word(instruct_head, prompt, temp, top_p, max_tokens, iteration):
    contents = instruct_head + '\n"""\n' + prompt + '\n"""\n'
    is_within_limit = count_prompt_token(max_tokens, 'gpt-3.5-turbo', contents)

    if is_within_limit:
        try:
            openai.base_url = config.basicUrl_gpt35
            openai.api_key = config.OPENAI_API_KEY
            response = openai.chat.completions.create(
                model=modelName_gpt35,
                messages=[
                    {"role": "user", "content": contents}
                ],
                temperature=temp,
                max_tokens=max_tokens,
                n=iteration
            )
            data = response.model_dump_json()
            status = 200
            return status, data
        except Exception as e:
            return str(e), {}
    else:
        return "exceed max tokens", {}


def add_run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, improved_fix, include_append=True):
    scenario_config_file = os.path.join(experiment_dir, "scenario.json")
    result_path = os.path.join(experiment_dir, "result", llm_engine)
    llm_responses_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_responses")

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")
    llm_response_file = str("%s.temp-1.00.top_p-1.00.response_feedback_fix.json" % (llm_engine))


    append_contents = ""
    whitespace = ""
    add_whitespace = ""
    add_contents = ""
    add_first = ""
    add_append_contents = ""
    add_prepend_contents = ""
    contents = ""

    file_path = os.path.join(experiment_dir, "response",
                                     experiment_file + ".llm_responses",
                                     "prompt.txt")

    comment_key = "//"

    count_result = os.path.join(experiment_dir, "result", llm_engine + "_result.csv")
    if os.path.exists(count_result):
        with open(count_result, 'r') as f:

            with open(scenario_config_file, 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    experiment_dir = path
                    experiment_file = scenario_contents["err_detailed_info"]["file_name"].split("/")[-1]
                    print("Generate LLM choices for later evaluation for %s" % (experiment_dir))
                    if scenario_contents["cwe_name"]:
                        err_name = scenario_contents["cwe_name"]
                    else:
                        err_name = scenario_contents["unity_special_name"]

                    combine_settings = config.CONTEXT_COMBINE_CWE[err_name]

                    include_addition = scenario_contents["include_addition"]
                    file_name = scenario_contents["err_detailed_info"]["file_name"]
                    is_function = None

                    if include_addition:
                        add_file_name = scenario_contents["err_detailed_info"]["add_file_name"]
                        if file_name == add_file_name:
                            is_function = True
                        else:
                            is_function = False
                        prompt_file_path = os.path.join(experiment_dir,
                                                        experiment_file + ".prompt.cs")
                        add_prompt_file_path = os.path.join(experiment_dir,
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

                    else:
                        with open(file_path, "r", encoding='utf-8') as f:
                            contents = f.read()


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

                    check_ql = scenario_contents["check_ql"]
                    project_dir = scenario_contents["project_root_dir"]
                    project_name = scenario_contents["err_detailed_info"]["file_name"]
                    db_name = scenario_contents["exp_dir"].split("!")[0]
                    db_chg_name = db_name + "_" + llm_response_file + "_db"

                    if True:
                        pure_file_name = (project_name.split("/")[-1]).split('.')[0]

                        pure_file_name_original = pure_file_name + "_original"
                        original_project_name = project_name.replace(pure_file_name + ".cs",
                                                                     pure_file_name_original + ".cs")
                        # print(original_project_name)

                        pure_file_path = project_dir + original_project_name

                        original_err_file = project_dir + project_name

                        result_original_file = os.path.join(result_path, "result_original.csv")
                        result_replace_file = os.path.join(result_path, "result_" + llm_response_file + ".csv")

                        response_content = determine_which_merge_strategy(combine_settings, include_addition, is_function, comment_key, prepend_contents, contents, add_contents,
                                                                                             append_contents,
                                                                                             improved_fix,
                                                                                             whitespace, add_whitespace,
                                                                                             between_contents,
                                                                                             add_first,
                                                                                             add_append_contents,
                                                                                             add_prepend_contents)

                        if not os.path.exists(result_original_file):
                            cmd = 'codeql database analyze "{}" "{}" --format=csv --output="{}" --rerun'
                            cmd = cmd.format((project_dir + "/" + db_name),
                                             check_ql,
                                             result_original_file)
                            print(cmd)
                            subprocess.run(cmd, shell=True)

                        if not os.path.exists(pure_file_path):
                            print("replace now")
                            print(original_err_file)
                            print(pure_file_path)

                            os.rename(original_err_file, pure_file_path)

                            with open(original_err_file, 'w', encoding="utf-8") as f2:
                                f2.write(response_content)
                                f2.close()

                            exp_absolute_path = "D:/XRFix/XRFix/" + result_replace_file
                            cmd1 = 'cd {} && codeql database create {} --language=csharp --overwrite && codeql database analyze {} "{}" --format=csv --output="{}"'
                            cmd1 = cmd1.format(project_dir,
                                               (db_chg_name),
                                               (db_chg_name),
                                               check_ql,
                                               exp_absolute_path
                                               )
                            print(cmd1)
                            subprocess.run(cmd1, shell=True)

                        if os.path.exists(pure_file_path) and os.path.exists(original_err_file):
                            os.remove(original_err_file)
                            os.rename(pure_file_path, original_err_file)

                        if os.path.exists((project_dir + "/" + db_chg_name)):
                            shutil.rmtree((project_dir + "/" + db_chg_name), ignore_errors=True)


def generate_LLM_experiment_responses(root_dir, instruct_head, contents, short_contents, experiment_filename, temperature, top_p, LLM_engine, iteration, skip_engines):
    #language_key = "//cs"
    max_tokens = 4096

    llm_responses_dir = os.path.join(root_dir, "response",
                                       experiment_filename + ".llm_responses")

    if not os.path.exists(llm_responses_dir):
        os.makedirs(llm_responses_dir)

    prompt_text = contents


    with open(os.path.join(root_dir, "scenario.json"), 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)

        experiment_dir = root_dir
        experiment_file = scenario_contents["err_detailed_info"]["file_name"].split("/")[-1]


    prompt_file = os.path.join(llm_responses_dir, config.PROMPT_TEXT_FILENAME)
    with open(prompt_file, "w", encoding='utf8') as f:
        f.write(prompt_text)


    if True:
        for engine in LLM_engine:
            if engine in skip_engines:
                continue

            codex_responses_file = os.path.join(llm_responses_dir,
                                                "%s.temp-%.2f.top_p-%.2f.response.json" % (engine, temperature, top_p))
            print(f"Starting feedback loop for {llm_responses_dir}")

            print("Generating '%s' responses for %s" % (engine, experiment_filename))


            skip = False

            if os.path.exists(codex_responses_file):
                print("Response already exists. Skip.")
                skip = True
            failure = 0

            while not skip and failure <= 5:
                print("Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
                    llm_responses_dir, experiment_filename, temperature, top_p, engine, max_tokens))

                if engine == "gpt-3.5-turbo":
                    res_file = ["gpt-3.5-turbo.temp-1.00.top_p-1.00.response.json"]
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
                        if (status == "exceed max tokens"):
                            prompt_text = short_contents

                        failure += 1

                        print("Waiting 30 seconds and trying again")
                        time.sleep(30)
                        continue

                if engine == "gpt-4-turbo":
                    res_file = ["gpt-4-turbo.temp-1.00.top_p-1.00.response.json"]
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
                        time.sleep(15)
                        break
                    else:
                        if (status == "exceed max tokens"):
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

                print("Successfully generated response, proceeding with feedback loop")

                if True:


                    has_successful_fix = False
                    # Process the response and prepare for CodeQL analysis
                    extrapolate_all_llm_choices_and_get_all_results(experiment_dir, res_file)


                    # Run CodeQL test and log execution
                    print("Running CodeQL analysis")


                    test_results = experiment_run_codeql_test_with_feedback.set_up_scenarios_test_with_feedback(
                            experiment_dir,
                            experiment_file,
                            engine,
                            None
                        )


                    # Check results
                    result_file = os.path.join(experiment_dir, "result", f"{engine}_result.csv")
                    if not os.path.exists(result_file):
                        raise FileNotFoundError("CodeQL result file not found")

                    with open(result_file, 'r') as f:
                        results = json.load(f)

                        # First check if any response successfully fixed the issue
                        for result in results:

                            if result.get("is_vulnerable") == False:
                                has_successful_fix = True
                                print(f"Found successful fix in response {result['llm_response_file']}")
                                break

                        if has_successful_fix:
                            print(f"Successfully fixed bug")
                            break

                        # If fix failed, analyze CodeQL execution log
                        else:
                            # Check results
                            result_file = os.path.join(experiment_dir, "result", f"{engine}_result.csv")
                            if not os.path.exists(result_file):
                                raise FileNotFoundError("CodeQL result file not found")

                            with open(result_file, 'r') as f:
                                results = json.load(f)
                            # Read extrapolation metadata to identify unique responses
                            metadata_file = os.path.join(experiment_dir, "response", 
                                                       experiment_file + ".llm_programs",
                                                       "extrapolation_metadata.json")

                            codex_responses_file_full = os.path.join(experiment_dir, "response",
                                                     experiment_file + ".llm_responses",
                                                     res_file[0])
                            
                            with open(metadata_file, 'r', encoding='utf8') as f:
                                metadata = json.load(f)
                            
                            # Track which files we've analyzed to avoid duplicates
                            analyzed_files = set()
                            index = 0
                            is_fixed = False
                            trial = 0


                            for entry in metadata:

                                    print("Correct the answer with feedback trail: "+str(trial))
                                    # Skip if this is a duplicate response
                                    if entry['duplicate_of'] is not None:
                                        index += 1
                                        continue

                                    # Skip if we've already analyzed this file
                                    if entry['filename'] in analyzed_files:
                                        index += 1
                                        continue

                                    analyzed_files.add(entry['filename'])

                                    # Read the response file to get the fixed code
                                    with open(codex_responses_file_full, 'r', encoding='utf8') as f:
                                        codex_response = json.loads(f.read())
                                        try:
                                            codex_response = json.loads(codex_response)
                                        except:
                                            codex_response = codex_response

                                        fixed_code = codex_response['choices'][index]['message']['content']



                                    # Read the original code from scenario.json
                                    original_code = short_contents

                                    # Get the codeql execution log for this response
                                    log_file = os.path.join(experiment_dir, "logs",
                                                          res_file[0] + "." + str(index) , "codeql_execution.log")


                                    if os.path.exists(log_file):
                                        with open(log_file, 'r', encoding='utf8') as f:
                                            codeql_log = f.read()

                                        # Analyze CodeQL execution log
                                        codeql_analysis = analyze_codeql_execution(
                                            codeql_log,
                                            original_code,
                                            fixed_code,
                                            results[index]
                                        )

                                        if codeql_analysis:

                                            print("Generated CodeQL analysis")

                                            # Generate improved fix based on analysis
                                            improved_fix = generate_improved_fix(
                                                original_code,
                                                fixed_code,
                                                codeql_analysis
                                            )

                                            if improved_fix:
                                                p1 = re.compile(r'```(.*?)```', re.S)
                                                content = re.findall(p1, improved_fix)
                                                new_choice_text = ""
                                                # print(content)
                                                if content != []:
                                                    for txt in content:
                                                        new_choice_text += txt
                                                    improved_fix = new_choice_text

                                                choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#',
                                                                          "", improved_fix, re.DOTALL)
                                                choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                                                improved_fix_code = choice_txt_clean
                                                improve_fix_file =  os.path.join(experiment_dir, "response",
                                                         experiment_file + ".llm_responses",
                                                         res_file[0].split('.json')[0]+"_feedback_fix"+'.json')

                                                with open(improve_fix_file, 'w', encoding='utf8') as f:
                                                    f.write(str(improved_fix_code))

                                                is_fixed = add_additional_result_to_evaluate(experiment_dir, experiment_file, engine, improved_fix_code)
                                                if is_fixed:
                                                    print("Generate correct answer with feedback.")
                                                    break
                                    trial += 1


def gather_codeql_test_results_for_feedback_exp(experiment_dir, experiment_file, llm_engine):

    scenario_config_file = os.path.join(experiment_dir, "scenario.json")

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")
    extra_meta_file = os.path.join(llm_programs_dir, "extrapolation_metadata.json")

    with open(scenario_config_file, 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)
        err_file_name = scenario_contents["err_detailed_info"]["file_name"]

    if True:
        result_path = os.path.join(experiment_dir, "result", llm_engine)
        count_result = os.path.join(experiment_dir, "result", llm_engine + "_result.csv")
        original_err_file = os.path.join(experiment_dir, "result", llm_engine, "result_original.csv")

        files = os.walk(result_path)
        count_original_err = []
        all_llm_result = []

        with open(extra_meta_file, 'r', encoding="utf8") as f1:
            llm_response_file = json.load(f1)
            for llm_response_need_to_check in llm_response_file:
                if llm_response_need_to_check["engine"] == llm_engine:
                    each_file_result = {}
                    each_file_result["llm_response_file"] = llm_response_need_to_check["filename"]
                    each_file_result["is_vulnerable"] = None
                    each_file_result["is_compilable"] = False
                    each_file_result["temperature"] = llm_response_need_to_check["temperature"]
                    each_file_result["top_p"] = llm_response_need_to_check["top_p"]
                    all_llm_result.append(each_file_result)

            feedback_file_result = {}
            feedback_file_result["llm_response_file"] = llm_response_file[0]["filename"].split('.json')[0]+"_feedback_fix"+'.json'
            feedback_file_result["is_vulnerable"] = None
            feedback_file_result["is_compilable"] = False
            feedback_file_result["temperature"] = llm_response_file[0]["temperature"]
            feedback_file_result["top_p"] = llm_response_file[0]["top_p"]

            all_llm_result.append(feedback_file_result)

        with open(original_err_file, 'r') as f1:
            reader = csv.reader(f1)
            for rows in reader:
                if rows[4] == err_file_name:
                    position = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                    count_original_err.append(position)

        for path, dir_lis, file_lis in files:
            for file in file_lis:
                if "original" not in file:
                    response_file = (file.replace('.csv', '')).split('result_')[-1]
                    count_file_result = []

                    with (open(os.path.join(path, file)) as f):
                        reader = csv.reader(f)
                        for rows in reader:
                            if rows[4] == err_file_name:
                                position = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                                count_file_result.append(position)

                    for res in all_llm_result:
                        if res["llm_response_file"] == response_file:
                            #res["is_vulnerable"] = False
                            res["is_compilable"] = True
                            if len(count_file_result) == 0:
                                res["is_vulnerable"] = False
                            else:
                                if res["is_vulnerable"] != True and len(count_file_result) < len(count_original_err):
                                    res["is_vulnerable"] = False
                                else:
                                    res["is_vulnerable"] = True

        with open(count_result, 'w') as f2:
            f2.write(json.dumps(all_llm_result))

def add_additional_result_to_evaluate(experiment_dir, experiment_file, llm_engine, add_improve_fix=None):
    """
    Set up and run CodeQL tests for all experiment scenarios
    Args:
        experiment_dir: Experiment directory
        experiment_file: Experiment file name
        llm_engine: LLM engine name
        log_file: Optional file object for logging
    """
    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")
    feedback_response_file = str("%s.temp-1.00.top_p-1.00.response_feedback_fix.json" % (llm_engine))
    feedback_result = os.path.join(experiment_dir, "result",
                                    llm_engine ,  "result_"+feedback_response_file+".csv")

    all_result = os.path.join(experiment_dir, "result",
                                    llm_engine + "_result.csv")
    #add_run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, add_improve_fix)
    if long_path_exists(feedback_result):
        gather_codeql_test_results_for_feedback_exp(experiment_dir, experiment_file, llm_engine)
        if os.path.exists(all_result):
            with open(all_result, 'r') as f:
                results = json.load(f)
                has_successful_fix = False

                # First check if any response successfully fixed the issue
                for result in results:
                    print(result)
                    if result.get("is_vulnerable") == False:
                        has_successful_fix = True
                        logging.info(f"Found successful fix in response {result['llm_response_file']}")
                        break

                if has_successful_fix:
                    logging.info(f"Successfully fixed bug")
                    return True

    return  False

def generate_feedback_response(root_dir, instruct_head, contents, short_contents, experiment_filename, temperature, top_p, LLM_engine, iteration, skip_engines):
    #language_key = "//cs"
    max_tokens = 4096

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
                                                "%s.temp-%.2f.top_p-%.2f.response_feedback.json" % (engine, temperature, top_p))
            print(codex_responses_file)
            print("Generating '%s' responses for %s" % (engine, experiment_filename))
            skip = False
            if os.path.exists(codex_responses_file):
                print("Response already exists. Skip.")
                skip = True
            failure = 0

            while not skip and failure <= 5:
                print("Attempting responses for folder: %s ,file:%s,temp:%.2f,top_p:%.2f,engine:%s,max_tokens:%d" % (
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
                        if (status == "exceed max tokens"):
                            prompt_text = short_contents

                        failure += 1

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
                        time.sleep(15)
                        break
                    else:
                        if (status == "exceed max tokens"):
                            prompt_text = short_contents

                        failure += 1
                        print("Waiting 30 seconds and trying again")
                        time.sleep(30)
                        continue

            if not skip and data != {}:
                # create codex_responses file in the experiment dir
                with open(codex_responses_file, "w") as f3:
                    f3.write(json.dumps(data, indent=4))

                actual_prompt_file = os.path.join(llm_responses_dir, "actual_prompt_feedback.txt")
                with open(actual_prompt_file, "w", encoding='utf8') as f:
                    f.write(prompt_text)



def hand_crafted_prompt_response(path, skip_engines=[]):
    file_extension = '.cs'
    files = os.walk(path)
    comment_key = "//"

    for path, dir_lis, file_lis in files:
        #print(path)
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

                    add_prompt_short_lines_file_name = \
                        scenario_contents["err_detailed_info"]['add_file_name'].split('/')[
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
                    if os.path.exists(add_prompt_short_lines_path):
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
                        scenario_contents["err_detailed_info"]['file_name'].split('/')[-1],
                        scenario_contents["temperature"],
                        scenario_contents["top_p"],
                        scenario_contents["LLM_engine"],
                        scenario_contents["iteration"],
                        skip_engines
                    )

def decide_include_addition(path):
    ile_extension = '.cs'
    files = os.walk(path)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                if scenario_contents["include_addition"]:
                    hand_crafted_prompt_response(path)
                else:
                    prepare_LLM_experiment_requests(path)


if __name__ == "__main__":
    root = r'.\experiment'
    files = os.walk(root)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)
                    experiment_dir = path
                    experiment_file = scenario_contents["err_detailed_info"]["file_name"].split("/")[-1]
                    add_additional_result_to_evaluate(experiment_dir, experiment_file, "gpt-4-turbo")

    #root = r'.\experiment\cwe\constant_condition\hand_db_insert!Assets!Oculus!VR!Scripts!OVRPlugin.cs~1012~10~1012~27'
    #decide_include_addition(root)
    #with open(os.path.join(path, "response", "GvrAudioSource.cs.llm_responses", "gpt-4-turbo.temp-1.00.top_p-1.00.response_feedback_fix.json")) as f:
     #   improved_fix = f.read()
    #print(add_additional_result_to_evaluate(path, "GvrAudioSource.cs", "gpt-4-turbo", improved_fix))
