import os
import config
import json
import datetime
import subprocess
import csv

def gather_codeql_test_results_for_exp(experiment_dir, llm_engine):
    result_path = os.path.join(experiment_dir, "result", llm_engine)
    count_result = os.path.join(experiment_dir, "result", llm_engine+"_result.csv")
    original_err_file = os.path.join(experiment_dir, "result", llm_engine, "result_original.csv")

    scenario_config_file = os.path.join(experiment_dir, "scenario.json")
    with open(scenario_config_file, 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)
        err_file_name = scenario_contents["err_detailed_info"]["file_name"]

    files = os.walk(result_path)
    count_original_err = []
    all_llm_result = []

    with open(original_err_file, 'r') as f1:
        reader = csv.reader(f1)
        for rows in reader:
            if rows[4] == err_file_name:
                position = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                count_original_err.append(position)

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if "original" not in file:
                count_file_result = []
                each_file_result = {}
                each_file_result["llm_response_file"] = (file.replace('.csv', '')).split('result_')[-1]

                with (open(os.path.join(path, file)) as f):
                    reader = csv.reader(f)
                    for rows in reader:
                        if rows[4] == err_file_name:
                            position = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                            count_file_result.append(position)

                if (len(count_file_result) < len(count_original_err)):
                    each_file_result["is_vulnerable"] = False
                else:
                    each_file_result["is_vulnerable"] = True
                all_llm_result.append(each_file_result)

    print(all_llm_result)
    with open(count_result, 'w') as f2:
        f2.write(json.dumps(all_llm_result))

def run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, llm_response_file):
    scenario_config_file = os.path.join(experiment_dir, "scenario.json")
    result_path = os.path.join(experiment_dir, "result", llm_engine)

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")

    if not os.path.exists(result_path):
        os.makedirs(result_path)

    with open(scenario_config_file, 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)
        check_ql = scenario_contents["check_ql"]
        project_dir = scenario_contents["project_root_dir"]

        project_name = scenario_contents["err_detailed_info"]["file_name"]
        db_name = scenario_contents["exp_dir"].split("!")[0]
        db_chg_name = db_name + "_"+llm_response_file + "_db"

        pure_file_name = (project_name.split("/")[-1]).split('.')[0]

        pure_file_name_original = pure_file_name + "_original"
        original_project_name = project_name.replace(pure_file_name, pure_file_name_original)


        pure_file_path = project_dir+ original_project_name

        original_err_file = project_dir + project_name

        result_original_file = os.path.join(result_path, "result_original.csv")
        result_replace_file = os.path.join(result_path, "result_"+llm_response_file+".csv")

    with open(os.path.join(llm_programs_dir, llm_response_file), 'r', encoding='utf8') as f1:
        response_content = f1.read()
        if not os.path.exists(result_original_file):
            cmd = "codeql database analyze {} {} --format=csv --output={} --rerun"
            cmd = cmd.format((project_dir + "/" + db_name),
                             check_ql,
                             result_original_file)
            subprocess.run(cmd, shell=True)

        if not os.path.exists(pure_file_path):
            os.rename(original_err_file, pure_file_path)

        with open(original_err_file, 'w') as f2:
            f2.write(response_content)
            cmd1 = "codeql database create {} --language=csharp --overwrite --source-root {} && codeql database analyze {} {} --format=csv --output={} && rm -rf {}"
            cmd1 = cmd1.format((project_dir + "/" + db_chg_name),
                               project_dir,
                               (project_dir + "/" + db_chg_name),
                               check_ql,
                               result_replace_file,
                               (project_dir + "/" + db_chg_name)
                               )
            subprocess.run(cmd1, shell=True)


def set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file):

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                      experiment_file + ".llm_programs")
    extra_meta_file = os.path.join(llm_programs_dir, "extrapolation_metadata.json")


    with open(extra_meta_file, 'r', encoding="utf8") as f1:
        llm_response_file = json.load(f1)
        for llm_response_need_to_check in llm_response_file:
            llm_response_file = llm_response_need_to_check["filename"]
            llm_engine = llm_response_need_to_check["engine"]
            run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, llm_response_file)

            gather_codeql_test_results_for_exp(experiment_dir, experiment_file, llm_engine)

if __name__ == "__main__":
    path = r"D:\git_upload\Unity_code_detection\code_repair_framework\experiment\unity\instantiate_destroy_in_update\swim_db_insert!Assets!SwimControl.cs~46~25~46~42"

    exp_file = "./SwimControl.cs"
    llm_engine = "gpt-3.5-turbo"
    #set_up_scenarios_test_for_all_exp(path, exp_file)
    gather_codeql_test_results_for_exp(path, llm_engine)


