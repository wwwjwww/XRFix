from codebleu import calc_codebleu
import os
import config
import json
import re


def evaluate_codebleu(path, llm_response_dir, response_file):
    reference_path = os.path.join(path, "reference_answer")
    reference_lis = []
    all_refer_result = {}
    return_result = []

    files = os.walk(reference_path)
    for path, dir_lis, file_lis in files:
        for file in file_lis:
            #print(file)

            with open(os.path.join(path, file), 'r', encoding="utf8") as f1:
                reference_lis.append(f1.read())
                all_refer_result[f1.read()] = []


    #print(reference_lis)
    for reference in reference_lis:
        result_dic = []
        with open(os.path.join(llm_response_dir, response_file), 'r', encoding="utf8") as f:
            codex_response = json.loads(f.read())

            if response_file.startswith("gpt-3.5-turbo") or response_file.startswith("gpt-4-turbo"):
                for choice in codex_response['choices']:

                    choice_txt = choice['message']['content']

                    p1 = re.compile(r'```(.*?)\n```', re.S)
                    content = re.findall(p1, choice_txt)
                    new_choice_text = ""
                    # print(content)
                    if content != []:
                        for txt in content:
                            new_choice_text += txt
                        choice_txt = new_choice_text

                    choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#', "", choice_txt,
                                              re.DOTALL)
                    choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                    prediction = choice_txt_clean

                    result = calc_codebleu([reference], [prediction], lang="c_sharp", weights=(0.1, 0.1, 0.4, 0.4),
                                           tokenizer=None)
                    result_dic.append(result)

            if response_file.startswith("code-llama") or response_file.startswith("starcoder"):
                for i in range(0, 5):
                    response_id = "code_repairing_" + str(i)
                    choice_txt = codex_response[response_id]

                    content = choice_txt.split("```")
                    if len(content) > 1:
                        choice_txt = content[1]

                    choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#', "", choice_txt, re.DOTALL)
                    choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                    prediction = choice_txt_clean
                    result = calc_codebleu([reference], [prediction], lang="c_sharp", weights=(0.1, 0.1, 0.4, 0.4),
                                           tokenizer=None)
                    result_dic.append(result)
            all_refer_result[reference] = result_dic

    max = 0
    #print(all_refer_result)
    for reference in reference_lis:
        #print(all_refer_result[reference])
        sum = 0
        for res in all_refer_result[reference]:
            sum += res["codebleu"]

        if sum > max:
            max = sum
            return_result = all_refer_result[reference]


    print(return_result)

    return return_result

def get_all_llm_codebleu_evaluation(root, eval_dir, response_file):
    files = os.walk(root)
    experiment_extension = "cs"
    all_result = []

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            if file == "scenario.json":
                with open(os.path.join(path, file), 'r', encoding="utf8") as f:
                    scenario_contents = json.load(f)

                    experiment_dir = path
                    experiment_file = scenario_contents["err_detailed_info"]["file_name"].split("/")[-1]
                    print("Evaluation on LLM choices for CodeBLEU for %s" % (experiment_dir))
                    if scenario_contents["cwe_name"]:
                        err_name = scenario_contents["cwe_name"]
                    else:
                        err_name = scenario_contents["unity_special_name"]

                    combine_settings = config.CONTEXT_COMBINE_CWE[err_name]


                    llm_response_dir = os.path.join(experiment_dir, eval_dir,
                                                    experiment_file + ".llm_responses")
                    iteration = scenario_contents["iteration"]

                    if os.path.exists(llm_response_dir):
                        result_dic = evaluate_codebleu(experiment_dir, llm_response_dir, response_file)
                        for result in result_dic:
                            all_result.append(result)


                    else:
                        print("no response file, skip")

    score_sum = 0
    for n in all_result:
        score_sum += n["codebleu"]

    score_avg = score_sum / len(all_result)
    print(len(all_result))
    print(score_avg)


if __name__ == "__main__":
    '''
        "RWT":.\\experiment\\unity\\transform_rigidbody_in_update
        "RCC":".\experiment\cwe\constant_condition"
        "RLC":".\experiment\cwe\\non-short-circuit_logic"
        "RD":".\experiment\cwe\lock_this"
        "RHA":".\experiment\cwe\container_contents_never_accessed"
        "RS":".\experiment\cwe\\redundant_select"
        "IDU":".\experiment\\unity\instantiate_destroy_in_update"
        "NAU": ".\experiment\\unity\\new_allocation_in_update"
    '''

    path_lis = [r'.\experiment']
    eval_dir = "fix_instruction_response"

    #codex_responses_files = "gpt-3.5-turbo.temp-1.00.top_p-1.00.response.json"
    #codex_responses_files = "code-llama.temp-1.00.top_p-1.00.response.json"
    #codex_responses_files = "gpt-4-turbo.temp-1.00.top_p-1.00.response.json"
    codex_responses_files = "starcoder.temp-1.00.top_p-1.00.response.json"
    for path in path_lis:
        get_all_llm_codebleu_evaluation(path, eval_dir, codex_responses_files)
