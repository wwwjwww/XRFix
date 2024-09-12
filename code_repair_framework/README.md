# XRFix-Code Repair Framework

The folders are organized as follows.
* ./experiments contains all generated fixes using different prompt templates
* ./result_cwe contains all located CWE-related errors that we obtained from static analysis
* ./result_unity_sort contains all located Unity-related errors that we obtained from static analysis

### Usage

All experiments are organized in the experiments sub-directory into various folders. The general experiment directory contains a scenario which is characterized/managed by the `scenario.json` file. 

When you are ready, the python scripts are as follows for solving any one scenario:
* `experiment_gen_response_*.py` - *Run this file to obtain the repair results from LLMs*
* `hand_crafted_exp*.py` - *Run this file to obtain the repair results from LLMs for hand-crafted results*
* `experiment_get_result.py` - *Run this file to run static analysis results to determine if the fix is plausible*



To generate the repair scenarios, run these two scripts after you have 'solved' the intitial faulty programs (ran the above instructions in the standalone (handcrafted) and realworld projects).
* `create_prompt_scenario.py` - *This automatically derives the scenario.jsons for experiments
* `experiment_gen_content.py` - *Use this to prepare the head.cs/append.cs/prompt.cs/short.cs according to scenario.jsons for program repair*

Once the scenarios are generated, you re-use the first instructions.

Finally, once all results are collected, you can run
* `experiment_evaluator.py` - *Run this file to evaluate the reference answer analysis results for overall outcome*.

`config.py` contains a number of important settings and options.

## On nomenclature

Note that in the framework some names differ to those used in the template. 

### Synthetic/handcrafted scenario template names

Framework name                                       | Paper name
-----------------------------------------------------|------------
"simple_prompt_*"                                  | "a"
"basic_prompt_*"                                   | "b"
"fix_instruction_*"                                | "c"
"ffix_instruction_*_assymetrical"                  | "d"