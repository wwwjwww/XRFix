# XRFix
This repository contains the generated code and script framework used to generate the results in the 'XRFix: Exploring Bug Repair of Extended Reality Applications with Large Language Models' manuscript.

The folders are organized as follows.
* ./Customized_codeql contains our specially designed queries for XR-related errors.
* ./Customized_UnityLint conatins our customized UnityLint.
* ./code_repair_framework contains our LLM-assisted code repair framework.

### Environment Requirement
1. CodeQL-cli version: 2.16.3, official guidance: https://codeql.github.com/docs/codeql-cli/getting-started-with-the-codeql-cli/
2. Python 3.12.3
3. Make sure your XR projects are compilable. Compile them in Unity Editor and keep all the files in the same project directory after compilation.

### Usage
#### UnityLint for asset files
Please first enter the repository and use the following command line to analyse asset files.
*Windows*
```bash
ShellStarter.exe -d <dirPath>
```

*MacOS / Linux*
```bash
mono ShellStarter.exe -d <dirPath>
```

where the **Required Argument** is:
```bash
-p, --project       Project directory.
```

Furthermore, you can use the following **Optional Arguments**:
```bash
-v, --verbose      Display Log on the standard output.

--help             Display this help screen.

--version          Display version information.
```

#### CodeQL for Unity files
Please download the CodeQL Github repository to your workspace consisting to your codeql-cli version. Then, add the queries in CodeQL repository.
https://github.com/github/codeql

*Set-up*
1. Add files in ./Customized_codeql/codeql-suites to ./codeql/csharp/ql/src/codeql-suites
2. Add files in ./Customized_codeql/suite-helpers to ./codeql/misc/suite-helpers
3. Add  ./Customized_codeql/unityCheck to ./codeql/csharp/ql/src

*Run the following command to perform static analysis*
```bash
codeql database analyze <database> ./codeql/csharp/ql/src/codeql-suites/special_select_check.qls
codeql database analyze <database> ./codeql/csharp/ql/src/codeql-suites/unity-check.qls
```

#### code repair framework
Please see README.md files in ./code_repair_framework to see more details.



