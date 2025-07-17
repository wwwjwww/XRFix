# XRFix-Code Repair Framework

The folders are organized as follows.
* ./experiment* contains all generated fixes using different prompt templates
    - ./experiment contains results of XRFix prompt engineering and Self-Repair results
    - ./experiment_alpharepair contains results of our baselines AlphaRepair
    - ./experiment_codet5 contains results of our baselines Fine-tuned CodeT5
* ./result_cwe contains all located CWE-related errors that we obtained from static analysis
* ./result_unity contains all located Unity-related errors that we obtained from static analysis

### Usage

All experiments are organized in the experiments sub-directory into various folders. The general experiment directory contains a scenario which is characterized/managed by the `scenario.json` file. 

When you are ready, the python scripts are as follows for solving any one scenario:
* `experiment_gen_response_*.py` - *Run this file to obtain the repair results from LLMs*
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

### Prompt template names

Framework name                                       | Paper name
-----------------------------------------------------|------------
"simple_prompt_*"                                  | "a"
"basic_prompt_*"                                   | "b"
"fix_instruction_*"                                | "c"
"fix_instruction_*_assymetrical"                  | "d"
"code_example_*"                                   | "e"
