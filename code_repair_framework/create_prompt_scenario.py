
import json
import config
import os

def create_exp_scenario(file1, type):
    '''
    scenario_json = {
        'original_dir': None,
        'project_root_dir': None,
        'exp_dir': None,
        'temperature': 0.95,
        'top_p': 1,
        'LLM_engine': ["gpt-3.5-turbo"],
        'iteration': 5,
        'cwe_name': None,
        'unity_special_name': None,
        'check_ql': None,
        'prompt_template': ["basic_prompt"],
        'stop_word': None,
        'include_addition': False,
        'err_detailed_info': {},
    }
    '''

    if type == "cwe":
        with open(file1, 'r') as f1:
            all_cwe_lis = json.load(f1)
            for err_name in config.cwe_lis:
                all_cwe_detailed_info = all_cwe_lis[err_name]
                for cwe_detailed_info in all_cwe_detailed_info:
                    scenario_json = {}
                    scenario_json["temperature"] = 1
                    scenario_json["top_p"] = 1
                    scenario_json["LLM_engine"] = ["gpt-4-turbo"]
                    #scenario_json["LLM_engine"] = ["gpt-3.5-turbo"]
                    #scenario_json["LLM_engine"] = ["starcoder"]
                    #scenario_json["LLM_engine"] = ["code-llama"]
                    scenario_json["iteration"] = 5
                    #scenario_json["prompt_template"] = ["simple_prompt_func_no_limit"]
                    scenario_json["prompt_template"] = ["basic_prompt_func_no_limit"]
                    #scenario_json["prompt_template"] = ["fix_instruction_func_no_limit"]
                    #scenario_json["prompt_template"] = ["fix_instruction_func_no_limit_assymetrical"]

                    scenario_json["original_dir"] = config.cwe_exp_path[err_name]
                    scenario_json["project_root_dir"] = config.project_root_dir[cwe_detailed_info["database_name"]]
                    scenario_json["exp_dir"] = str(str(cwe_detailed_info["database_name"]+cwe_detailed_info["position"]).replace('/','!')).replace(':','~')
                    scenario_json["cwe_name"] = err_name
                    scenario_json["unity_special_name"] = None
                    combine_settings = config.CONTEXT_COMBINE_CWE[err_name]
                    if combine_settings == "single_line":
                        #scenario_json["prompt_template"] = ["simple_prompt_line"]
                        scenario_json["prompt_template"] = ["basic_prompt_line"]
                        #scenario_json["prompt_template"] = ["fix_instruction_line"]
                        #scenario_json["prompt_template"] = ["fix_instruction_line_assymetrical"]


                    scenario_json["check_ql"] = config.query_root_dir+cwe_detailed_info["query_name"]
                    scenario_json["err_detailed_info"] = {}
                    scenario_json["include_addition"] = False

                    scenario_json["err_detailed_info"]["description"] = cwe_detailed_info["description"]
                    scenario_json["err_detailed_info"]["file_name"] = cwe_detailed_info["position"].split(":")[0]
                    scenario_json["err_detailed_info"]["start_line"] = cwe_detailed_info["position"].split(":")[1]
                    scenario_json["err_detailed_info"]["start_column"] = cwe_detailed_info["position"].split(":")[2]
                    scenario_json["err_detailed_info"]["end_line"] = cwe_detailed_info["position"].split(":")[3]
                    scenario_json["err_detailed_info"]["end_column"] = cwe_detailed_info["position"].split(":")[4]

                    create_file_path = scenario_json["original_dir"] + "/" + scenario_json["exp_dir"]
                    if not os.path.exists(create_file_path):
                        os.makedirs(create_file_path)
                    with open(os.path.join(create_file_path, "scenario.json"), 'w') as f:
                        f.write(json.dumps(scenario_json))

    else:
        with open(file1, 'r') as f2:
            all_unity_lis = json.load(f2)
            for err_name in config.unity_lis:
                if err_name == "Instantiate/Destroy in Update() method":
                    continue
                all_unity_detailed_info = all_unity_lis[err_name]
                for unity_detailed_info in all_unity_detailed_info:
                    scenario_json = {}
                    scenario_json["temperature"] = 1
                    scenario_json["top_p"] = 1
                    #scenario_json["LLM_engine"] = ["starcoder"]
                    #scenario_json["LLM_engine"] = ["code-llama"]
                    #scenario_json["LLM_engine"] = ["gpt-3.5-turbo"]
                    scenario_json["LLM_engine"] = ["gpt-4-turbo"]
                    scenario_json["iteration"] = 5
                    #scenario_json["prompt_template"] = ["simple_prompt_func_no_limit"]
                    scenario_json["prompt_template"] = ["basic_prompt_func_no_limit"]
                    #scenario_json["prompt_template"] = ["fix_instruction_func_no_limit"]
                    #scenario_json["prompt_template"] = ["fix_instruction_func_no_limit_assymetrical"]

                    scenario_json["original_dir"] = config.unity_exp_path[err_name]
                    scenario_json["project_root_dir"] = config.project_root_dir[unity_detailed_info["database_name"]]
                    scenario_json["exp_dir"] = str(str(unity_detailed_info["database_name"]+unity_detailed_info["position"]).replace('/','!')).replace(':','~')
                    scenario_json["unity_special_name"] = err_name
                    scenario_json["cwe_name"] = None
                    combine_settings = config.CONTEXT_COMBINE_CWE[err_name]
                    if combine_settings == "single_line":
                        #scenario_json["prompt_template"] = ["simple_prompt_line"]
                        scenario_json["prompt_template"] = ["basic_prompt_line"]
                        #scenario_json["prompt_template"] = ["fix_instruction_line"]
                        #scenario_json["prompt_template"] = ["fix_instruction_line_assymetrical"]

                    scenario_json["check_ql"] = config.query_root_dir+config.unity_query_name_lis[err_name]
                    scenario_json["err_detailed_info"] = {}
                    scenario_json["include_addition"] = False

                    scenario_json["err_detailed_info"]["description"] = unity_detailed_info["description"]
                    scenario_json["err_detailed_info"]["file_name"] = unity_detailed_info["position"].split(":")[0]
                    scenario_json["err_detailed_info"]["start_line"] = unity_detailed_info["position"].split(":")[1]
                    scenario_json["err_detailed_info"]["start_column"] = unity_detailed_info["position"].split(":")[2]
                    scenario_json["err_detailed_info"]["end_line"] = unity_detailed_info["position"].split(":")[3]
                    scenario_json["err_detailed_info"]["end_column"] = unity_detailed_info["position"].split(":")[4]

                    if (unity_detailed_info["sink_position"] != None):
                        scenario_json["prompt_template"] = ["add_prompt"]
                        scenario_json["include_addition"] = True
                        scenario_json["err_detailed_info"]["add_file_name"] = unity_detailed_info["sink_position"].split(":")[0]
                        scenario_json["err_detailed_info"]["add_start_line"] = unity_detailed_info["sink_position"].split(":")[1]
                        scenario_json["err_detailed_info"]["add_start_column"] = unity_detailed_info["sink_position"].split(":")[2]
                        scenario_json["err_detailed_info"]["add_end_line"] = unity_detailed_info["sink_position"].split(":")[3]
                        scenario_json["err_detailed_info"]["add_end_column"] = unity_detailed_info["sink_position"].split(":")[4]

                    create_file_path = scenario_json["original_dir"] + "/" + scenario_json["exp_dir"]
                    if not os.path.exists(create_file_path):
                        os.makedirs(create_file_path)
                    with open(os.path.join(create_file_path, "scenario.json"), 'w') as f:
                        f.write(json.dumps(scenario_json, indent=4))



def create_exp_scenario_instantiate(file1):
    with open(file1, 'r') as f2:
        all_unity_lis = json.load(f2)
        for err_name in config.unity_lis:
            all_unity_detailed_info = all_unity_lis[err_name]
            for unity_detailed_info in all_unity_detailed_info:

                scenario_json = {}
                scenario_json["temperature"] = 1
                scenario_json["top_p"] = 1
                #scenario_json["LLM_engine"] = ["starcoder"]
                #scenario_json["LLM_engine"] = ["code-llama"]
                #scenario_json["LLM_engine"] = ["gpt-3.5-turbo"]
                scenario_json["LLM_engine"] = ["gpt-4-turbo"]
                scenario_json["iteration"] = 5
                scenario_json["prompt_template"] = ["simple_add_prompt"]
                #scenario_json["prompt_template"] = ["add_prompt"]
                #scenario_json["prompt_template"] = ["fix_instruction_add_prompt"]
                #scenario_json["prompt_template"] = ["fix_instruction_add_prompt_assymetrical"]

                scenario_json["original_dir"] = config.unity_exp_path[err_name]
                scenario_json["project_root_dir"] = config.project_root_dir[unity_detailed_info["database_name"]]
                scenario_json["exp_dir"] = str(
                    str(unity_detailed_info["database_name"] + unity_detailed_info["position"]).replace('/',
                                                                                                        '!')).replace(
                    ':', '~')
                scenario_json["unity_special_name"] = err_name
                scenario_json["cwe_name"] = None
                combine_settings = config.CONTEXT_COMBINE_CWE[err_name]
                if combine_settings == "single_line":
                    scenario_json["prompt_template"] = ["simple_prompt_line"]
                    #scenario_json["prompt_template"] = ["basic_prompt_line"]
                    # scenario_json["prompt_template"] = ["fix_instruction_line"]
                    #scenario_json["prompt_template"] = ["fix_instruction_line_assymetrical"]


                scenario_json["check_ql"] = config.query_root_dir + config.unity_query_name_lis[err_name]
                scenario_json["err_detailed_info"] = {}
                scenario_json["include_addition"] = False

                scenario_json["err_detailed_info"]["description"] = unity_detailed_info["description"]
                scenario_json["err_detailed_info"]["file_name"] = unity_detailed_info["position"].split(":")[0]
                scenario_json["err_detailed_info"]["start_line"] = unity_detailed_info["position"].split(":")[1]
                scenario_json["err_detailed_info"]["start_column"] = unity_detailed_info["position"].split(":")[2]
                scenario_json["err_detailed_info"]["end_line"] = unity_detailed_info["position"].split(":")[3]
                scenario_json["err_detailed_info"]["end_column"] = unity_detailed_info["position"].split(":")[4]

                if (unity_detailed_info["sink_position"] != None):

                    #scenario_json["prompt_template"] = ["fix_instruction_add_prompt"]
                    scenario_json["include_addition"] = True
                    scenario_json["err_detailed_info"]["add_file_name"] = \
                    unity_detailed_info["sink_position"].split(":")[0]
                    scenario_json["err_detailed_info"]["add_start_line"] = \
                    unity_detailed_info["sink_position"].split(":")[1]
                    scenario_json["err_detailed_info"]["add_start_column"] = \
                    unity_detailed_info["sink_position"].split(":")[2]
                    scenario_json["err_detailed_info"]["add_end_line"] = \
                    unity_detailed_info["sink_position"].split(":")[3]
                    scenario_json["err_detailed_info"]["add_end_column"] = \
                    unity_detailed_info["sink_position"].split(":")[4]

                create_file_path = scenario_json["original_dir"] + "/" + scenario_json["exp_dir"]
                if not os.path.exists(create_file_path):
                    os.makedirs(create_file_path)
                with open(os.path.join(create_file_path, "scenario.json"), 'w') as f:
                    f.write(json.dumps(scenario_json, indent=4))

def create_exp_scenario_real_unity(file1):
    with open(file1, 'r') as f2:
        all_unity_lis = json.load(f2)
        for err_name in config.unity_real_lis:
            all_unity_detailed_info = all_unity_lis[err_name]
            for unity_detailed_info in all_unity_detailed_info:

                scenario_json = {}
                scenario_json["temperature"] = 1
                scenario_json["top_p"] = 1
                #scenario_json["LLM_engine"] = ["starcoder"]
                #scenario_json["LLM_engine"] = ["code-llama"]
                #scenario_json["LLM_engine"] = ["gpt-3.5-turbo"]
                scenario_json["LLM_engine"] = ["gpt-4-turbo"]
                scenario_json["iteration"] = 5


                scenario_json["original_dir"] = config.unity_real_exp_path[err_name]
                scenario_json["project_root_dir"] = config.real_project_root_dir[unity_detailed_info["database_name"]]
                scenario_json["exp_dir"] = str(
                    str(unity_detailed_info["database_name"] + unity_detailed_info["position"]).replace('/',
                                                                                                        '!')).replace(
                    ':', '~')
                scenario_json["unity_special_name"] = err_name
                scenario_json["cwe_name"] = None
                scenario_json["check_ql"] = config.query_root_dir + config.unity_real_query_name_lis[err_name]
                scenario_json["err_detailed_info"] = {}
                scenario_json["include_addition"] = False

                scenario_json["err_detailed_info"]["description"] = unity_detailed_info["description"]
                scenario_json["err_detailed_info"]["file_name"] = unity_detailed_info["position"].split(":")[0]
                scenario_json["err_detailed_info"]["start_line"] = unity_detailed_info["position"].split(":")[1]
                scenario_json["err_detailed_info"]["start_column"] = unity_detailed_info["position"].split(":")[2]
                scenario_json["err_detailed_info"]["end_line"] = unity_detailed_info["position"].split(":")[3]
                scenario_json["err_detailed_info"]["end_column"] = unity_detailed_info["position"].split(":")[4]

                if (unity_detailed_info["sink_position"] != None):

                    #scenario_json["prompt_template"] = ["fix_instruction_add_prompt"]
                    scenario_json["err_detailed_info"]["add_file_name"] = \
                    unity_detailed_info["sink_position"].split(":")[0]
                    scenario_json["err_detailed_info"]["add_start_line"] = \
                    unity_detailed_info["sink_position"].split(":")[1]
                    scenario_json["err_detailed_info"]["add_start_column"] = \
                    unity_detailed_info["sink_position"].split(":")[2]
                    scenario_json["err_detailed_info"]["add_end_line"] = \
                    unity_detailed_info["sink_position"].split(":")[3]
                    scenario_json["err_detailed_info"]["add_end_column"] = \
                    unity_detailed_info["sink_position"].split(":")[4]

                    scenario_json["prompt_template"] = ["simple_add_prompt"]
                    scenario_json["include_addition"] = True
                    # scenario_json["prompt_template"] = ["add_prompt"]
                    # scenario_json["prompt_template"] = ["fix_instruction_add_prompt"]
                    # scenario_json["prompt_template"] = ["fix_instruction_add_prompt_assymetrical"]

                if scenario_json["err_detailed_info"]["add_file_name"] == scenario_json["err_detailed_info"]["file_name"]:
                    if scenario_json["err_detailed_info"]["start_line"] == scenario_json["err_detailed_info"]["add_start_line"]:
                        scenario_json["prompt_template"] = ["simple_prompt_func_no_limit"]
                        scenario_json["include_addition"] = False
                        # scenario_json["prompt_template"] = ["basic_prompt_func_no_limit"]
                        # scenario_json["prompt_template"] = ["fix_instruction_func_no_limit"]
                        # scenario_json["prompt_template"] = ["fix_instruction_func_no_limit_assymetrical"]

                create_file_path = scenario_json["original_dir"] + "/" + scenario_json["exp_dir"]
                if not os.path.exists(create_file_path):
                    os.makedirs(create_file_path)
                with open(os.path.join(create_file_path, "scenario.json"), 'w') as f:
                    f.write(json.dumps(scenario_json, indent=4))


if __name__ == "__main__":
    '''
    file1 = "all_sort_cwe_detailed.json"
    file2 = "all_sort_unity.json"
    file3 = "all_sort_unity_instantiate.json"

    create_exp_scenario(file1, "cwe")
    create_exp_scenario(file2, "unity")
    create_exp_scenario_instantiate(file3)
    '''
    file = "all_real_unity.json"
    create_exp_scenario_real_unity(file)
