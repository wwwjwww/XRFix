import os
import config
import json
import datetime
import subprocess
import csv
import shutil

def winapi_path(dos_path, encoding=None):
    if (not isinstance(dos_path, str) and
        encoding is not None):
        dos_path = dos_path.decode(encoding)
    path = os.path.abspath(dos_path)
    if path.startswith(u"\\\\"):
        return u"\\\\?\\UNC\\" + path[2:]
    return u"\\\\?\\" + path


def gather_codeql_test_results_for_exp(experiment_dir, experiment_file):

    scenario_config_file = os.path.join(experiment_dir, "scenario.json")

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")
    extra_meta_file = os.path.join(llm_programs_dir, "extrapolation_metadata.json")

    with open(scenario_config_file, 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)
        err_file_name = scenario_contents["err_detailed_info"]["file_name"]
        llm_engine_lis = scenario_contents["LLM_engine"]


    for llm_engine in llm_engine_lis:
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


def run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, llm_response_file, duplicate_of):
    scenario_config_file = os.path.join(experiment_dir, "scenario.json")
    result_path = os.path.join(experiment_dir, "result", llm_engine)

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                    experiment_file + ".llm_programs")

    count_result = os.path.join(experiment_dir, "result", llm_engine + "_result.csv")
    if os.path.exists(count_result):
        print("already exists result file, skip")
    else:
        if duplicate_of != None:
            result_file = os.path.join(result_path, "result_" + duplicate_of + ".csv")
            result_self_file = os.path.join(result_path, "result_" + llm_response_file + ".csv")
            if os.path.exists(result_file):
                with open(result_file, 'r') as f:
                    data = f.read()

                with open(result_self_file, 'w', encoding='utf8') as f1:
                    f1.write(data)
                print("Create duplicate result file for %s.\n" % (result_self_file))
            else:
                print("No duplicate result file for %s, must not compilable.\n" % (result_self_file))

        else:
            if not os.path.exists(result_path):
                os.makedirs(result_path)

            with open(scenario_config_file, 'r', encoding="utf8") as f:
                scenario_contents = json.load(f)
                check_ql = scenario_contents["check_ql"]
                project_dir = scenario_contents["project_root_dir"]

                project_name = scenario_contents["err_detailed_info"]["file_name"]
                db_name = scenario_contents["exp_dir"].split("!")[0]
                db_chg_name = db_name + "_" + llm_response_file + "_db"

                pure_file_name = (project_name.split("/")[-1]).split('.')[0]

                pure_file_name_original = pure_file_name + "_original"
                original_project_name = project_name.replace(pure_file_name+".cs", pure_file_name_original+".cs")
                #print(original_project_name)

                pure_file_path = project_dir + original_project_name

                original_err_file = project_dir + project_name

                result_original_file = os.path.join(result_path, "result_original.csv")
                result_replace_file = os.path.join(result_path, "result_" + llm_response_file + ".csv")

            with open(os.path.join(llm_programs_dir, llm_response_file), 'r', encoding='utf8') as f1:
                response_content = f1.read()

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

                    exp_absolute_path = "D:/git_upload/Unity_code_detection/code_repair_framework/"  + result_replace_file
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

def set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file):

    llm_programs_dir = os.path.join(experiment_dir, "response",
                                      experiment_file + ".llm_programs")
    extra_meta_file = os.path.join(llm_programs_dir, "extrapolation_metadata.json")
    if os.path.exists(llm_programs_dir) and os.path.exists(extra_meta_file):
        with open(extra_meta_file, 'r', encoding="utf8") as f1:
            llm_response_file = json.load(f1)
            for llm_response_need_to_check in llm_response_file:
                llm_response_file = llm_response_need_to_check["filename"]
                llm_engine = llm_response_need_to_check["engine"]
                duplicate_of = llm_response_need_to_check["duplicate_of"]
                run_codeql_test_for_exp(experiment_dir, experiment_file, llm_engine, llm_response_file, duplicate_of)

        gather_codeql_test_results_for_exp(experiment_dir, experiment_file)


if __name__ == "__main__":
    #experiment_dir = r"D:\git_upload\Unity_code_detection\code_repair_framework\experiment\cwe\constant_condition\brick_db_insert!Assets!Oculus!VR!Scripts!OVRInput.cs~1499~11~1499~23"
    #experiment_file = "OVRInput.cs"
    experiment_dir = r".\experiment\cwe\lock_this\brick_db_insert!Assets!Oculus!LipSync!Scripts!OVRLipSyncContext.cs~230~15~230~18"
    experiment_file = "OVRLipSyncContext.cs"

    set_up_scenarios_test_for_all_exp(experiment_dir, experiment_file)


