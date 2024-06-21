import os
import config
import json
import re
import numpy as np

def basic_combine_generated_code_with_existing(comment_key, contents, append_contents, generated_text, generate_mean_logprob_comments = False, mean_logprob = None):
    #concatenate the choice to the original file
    new_contents = contents + generated_text + "\n" + append_contents
    if generate_mean_logprob_comments and mean_logprob is not None:
        new_contents = comment_key + "LM generated repair code follows. mean_logprob: " + mean_logprob + "\n" + new_contents
    return new_contents

def extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, codex_responses_files,
                                       force=False, keep_duplicates=False,
                                       include_append=False,
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
            with open(file_path, "r") as f:
                contents = f.read()

            append_contents = ""
            if include_append:
                append_file_path = os.path.join(experiment_dir, "response",
                                     experiment_file + ".llm_responses",
                                     "append.txt")
                if (os.path.exists(append_file_path)):
                    with open(append_file_path, "r") as f:
                        append_contents = f.read()

        # comment key
        comment_key = "//"

        codex_responses_files.sort()

        unique_outputs = {}

        temp_top_p_regex = re.compile(
            r'^(gpt-3.5-turbo)\.temp-(\d+\.\d+).*\.top_p-(\d+\.\d+)')

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

                        if choice_txt not in unique_outputs:
                            unique_outputs[choice_txt] = codex_programs_file
                            duplicate_of = None
                        else:
                            duplicate_of = unique_outputs[choice_txt]

                        extrapolate_error = False

                        new_contents = basic_combine_generated_code_with_existing(comment_key, contents,
                                                                                      append_contents, choice_txt,
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

            with open(filename_full, "w") as f:
                f.write(new_file['contents'])

        for new_file in new_files:
            new_file.pop('contents')
        # save the new_files to the codex_programs_dir as config.EXTRAPOLATION_METADATA_FILENAME
        with open(os.path.join(llm_programs_dir, "extrapolation_metadata.json"), "w") as f:
            f.write(json.dumps(new_files, indent=4))

def run_all_exp_and_get_all_results():
    print("coding until later")

if __name__ == "__main__":
    experiment_dir = r"D:\git_upload\Unity_code_detection\code_repair_framework\experiment\unity\instantiate_destroy_in_update\swim_db_insert!Assets!SwimControl.cs~46~25~46~42"
    experiment_file = "SwimControl.cs"
    experiment_extension = "cs"
    codex_responses_files = ["gpt-3.5-turbo.temp-0.95.top_p-1.00.response.json"]
    extrapolate_llm_choices_for_file(experiment_dir, experiment_file, experiment_extension, codex_responses_files,
                                       force=True, keep_duplicates=False,
                                       include_append=False,
                                       generate_mean_logprob_comments=False)


