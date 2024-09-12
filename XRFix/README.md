# XRFix-Code Repair Framework

This repository contains the generated code and script framework used to generate the results in the 'XRFix: Exploring Bug Repair of XR Apps with LLMs]{XRFix: Exploring Bug Repair of Extended Reality Applications with Large Language Models' manuscript.

The folders are organized as follows.
* ./experiments contains all generated fixes using different prompt templates
* ./result_cwe contains all located CWE-related errors that we obtained from static analysis
* ./result_unity_sort contains all located Unity-related errors that we obtained from static analysis

### Installation

1. Make sure codeql-cli is installed: https://codeql.github.com/docs/codeql-cli/getting-started-with-the-codeql-cli/
2. Make sure your XR project is compilable. Build CodeQL database based on your own XR project.
2. Make sure your `$OPENAI_API_KEY` is in the environment variables
3.Make sure there is a database called 'db' in the database i.e. `create database db;`
7. Make sure that a user `create user 'python'@'%' identified by 'python';` can log in.
8. `grant all privileges on *.* to 'python'@'%';`
9. Download this repo
10. Make a python3 virtualenv, e.g. `virtualenv venv && source venv/bin/activate` (make sure it is Python3 though.)
11. `pip install -r requirements.txt`

### Usage

All experiments are organized in the experiments sub-directory into various folders. The general experiment directory contains a scenario which is characterized/managed by the `scenario.json` file. 

When you are ready, the python scripts are as follows for solving any one scenario:
* `do_collect.py` - *Run this file to collect the API results from Codex*
* `do_extrapolate.py` - *Run this file to convert the API results into files for marking. It will verify each file and reject those that cannot be interpreted/compiled.*
* `do_functional_tests.py` - *Run this file to perform functional tests of the generated files*.
* `do_mark.py` - *Run this file to perform security evaluations of the generated files.*
* `do_collate_results.py` - *Collate the individual results files for analysis*

To generate the repair scenarios, run these two scripts after you have 'solved' the intitial faulty programs (ran the above instructions in the standalone (handcrafted) and realworld projects).
* `do_gen_iterative_fix_instructions.py` - *Use this to prepare a given synthetic or real-world study for program repair*
* `do_derive_iterative_scenarios.py` - *This automatically derives the scenario folders and scenario.jsons

Once the scenarios are generated, you re-use the first instructions.

Finally, once all results are collected, you can run
* `do_combine_iterative_results.py` to combine all generated individual result files into combined json files for analysis.
* `do_tikz_heatmaps.py` will then convert the combined json files into the plots ready to be imported into the manuscript.

Each of the `do_xxxxx` files contains command line arguments which are explained within each Python file.

`config.py` contains a number of important settings and options.

## On nomenclature

Note that in the framework some names differ to those used in the template. 

### Synthetic/handcrafted scenario template names

Framework name                                       | Paper name
-----------------------------------------------------|------------
"function-noprompt"                                  | "n.h."
"simple-prompt-1"                                    | "s.1"
"simple-prompt-2"                                    | "s.2"
"function-nomessage"                                 | "c."
"function"                                           | "c.m."

### Real-world scenario template names

Framework name                                       | Paper name
-----------------------------------------------------|------------
"asan-line2line-oracle-nofunction"                   | "n.h."
"asan-line2line-oracle-simple-prompt-1"              | "s.1"
"asan-line2line-oracle-simple-prompt-2"              | "s.2"
"asan-line2line-oracle-nomessage"                    | "c."
"asan-line2line-oracle-nomessage-assymetric"         | "c.a."
"asan-line2line-oracle-nomessage-notoken-assymetric" | "c.n."

### Language model name changes:

* cushman-codex -> code-cushman-001 (in line with OpenAI's rename)
* davinci-codex -> code-davinci-001 (in line with OpenAI's rename)