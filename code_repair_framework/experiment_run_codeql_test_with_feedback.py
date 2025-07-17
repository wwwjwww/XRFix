import os
import config
import json
import datetime
import subprocess
import csv
import shutil
import logging
from typing import Optional, TextIO, Dict, List, Any

from codeql_analysis_helper import setup_logging
from experiment_run_codeql_test import gather_codeql_test_results_for_exp
from experiment_get_result import determine_which_merge_strategy


def winapi_path(dos_path: str, encoding: Optional[str] = None) -> str:
    """Convert DOS path to Windows API path format."""
    if (not isinstance(dos_path, str) and encoding is not None):
        dos_path = dos_path.decode(encoding)
    path = os.path.abspath(dos_path)
    if path.startswith(u"\\\\"):
        return u"\\\\?\\UNC\\" + path[2:]
    return u"\\\\?\\" + path


def filter_error_lines(output: str) -> str:
    """Filter lines containing 'error' (case insensitive) from the output"""
    if not output:
        return ""
    error_lines = []
    for line in output.split('\n'):
        if 'error' in line.lower():
            error_lines.append(line)
    return '\n'.join(error_lines)

def run_codeql_test_with_feedback(
    experiment_dir: str,
    experiment_file: str,
    llm_engine: str,
    llm_response_file: str,
    duplicate_of: Optional[str],
    log_file: Optional[TextIO] = None
) -> Dict[str, Any]:
    """
    Run CodeQL test with enhanced feedback and logging.
    
    Args:
        experiment_dir: Directory containing experiment files
        experiment_file: Name of the experiment file
        llm_engine: Name of the LLM engine used
        llm_response_file: Name of the LLM response file
        duplicate_of: Name of the duplicate file if applicable
        log_file: Optional file object for logging
        
    Returns:
        Dictionary containing test results and feedback
    """
    if log_file:
        log_file.write(f"Running CodeQL test for {llm_response_file}\n")
    
    result_path = os.path.join(experiment_dir, "result", llm_engine)
    count_result = os.path.join(experiment_dir, "result", llm_engine + "_result.csv")
    
    # Handle existing results
    if os.path.exists(count_result):
        if log_file:
            log_file.write("Result file already exists, skipping\n")
        return {"status": "skipped", "message": "Result file already exists"}
    
    # Handle duplicates
    if duplicate_of is not None:
        result_file = os.path.join(result_path, f"result_{duplicate_of}.csv")
        result_self_file = os.path.join(result_path, f"result_{llm_response_file}.csv")
        
        if os.path.exists(result_file):
            with open(result_file, 'r') as f:
                data = f.read()
            with open(result_self_file, 'w', encoding='utf8') as f1:
                f1.write(data)
            return {"status": "duplicate", "message": f"Created duplicate result file for {result_self_file}"}
        else:
            return {"status": "error", "message": f"No duplicate result file found for {result_self_file}"}
    
    # Create result directory if needed
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    # Load scenario configuration
    with open(os.path.join(experiment_dir, "scenario.json"), 'r', encoding="utf8") as f:
        scenario_contents = json.load(f)
        check_ql = scenario_contents["check_ql"]
        project_dir = scenario_contents["project_root_dir"]
        project_name = scenario_contents["err_detailed_info"]["file_name"]
        db_name = scenario_contents["exp_dir"].split("!")[0]
        db_chg_name = f"{db_name}_{llm_response_file}_db"
    
    # Process files
    pure_file_name = os.path.splitext(os.path.basename(project_name))[0]
    pure_file_name_original = f"{pure_file_name}_original"
    original_project_name = project_name.replace(f"{pure_file_name}.cs",
                                               f"{pure_file_name_original}.cs")
    
    pure_file_path = project_dir + original_project_name
    original_err_file = project_dir + project_name
    
    result_original_file = os.path.join(result_path, "result_original.csv")
    result_replace_file = os.path.join(result_path, f"result_{llm_response_file}.csv")
    
    llm_programs_dir = os.path.join(experiment_dir, "response",
                                   f"{experiment_file}.llm_programs")
    
    test_results = {"status": "success", "steps": []}
    
    try:
        # Read response content
        with open(os.path.join(llm_programs_dir, llm_response_file), 'r', encoding='utf8') as f1:
            response_content = f1.read()
        
        # Run original analysis if needed
        if not os.path.exists(result_original_file):
            cmd = f'codeql database analyze "{os.path.join(project_dir, db_name)}" "{check_ql}" --format=csv --output="{result_original_file}" --rerun'
            if log_file:
                log_file.write(f"Running original analysis: {cmd}\n")
            
            # Capture and log the output
            process = subprocess.Popen(cmd, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
            stdout, stderr = process.communicate()
            
            if log_file:
                log_file.write("CodeQL Analysis Output:\n")
                # Filter and write error lines from stdout
                stdout_errors = filter_error_lines(stdout)
                if stdout_errors:
                    log_file.write(f"STDOUT Errors:\n{stdout_errors}\n")
                # Filter and write error lines from stderr
                stderr_errors = filter_error_lines(stderr)
                if stderr_errors:
                    log_file.write(f"STDERR Errors:\n{stderr_errors}\n")
            
            test_results["steps"].append({
                "action": "original_analysis", 
                "status": "completed",
                "stdout": stdout,
                "stderr": stderr
            })
        
        # Replace and analyze modified file
        if not os.path.exists(pure_file_path):
            if log_file:
                log_file.write("Replacing file for analysis\n")
            
            os.rename(original_err_file, pure_file_path)
            
            with open(original_err_file, 'w', encoding="utf-8") as f2:
                f2.write(response_content)
            
            exp_absolute_path = os.path.abspath(result_replace_file)
            cmd1 = f'cd {project_dir} && codeql database create {db_chg_name} --language=csharp --overwrite && codeql database analyze {db_chg_name} "{check_ql}" --format=csv --output="{exp_absolute_path}"'
            
            if log_file:
                log_file.write(f"Running modified analysis: {cmd1}\n")
            
            # Capture and log the output
            process = subprocess.Popen(cmd1, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
            stdout, stderr = process.communicate()
            
            if log_file:
                log_file.write("CodeQL Analysis Output:\n")
                # Filter and write error lines from stdout
                stdout_errors = filter_error_lines(stdout)
                if stdout_errors:
                    log_file.write(f"STDOUT Errors:\n{stdout_errors}\n")
                # Filter and write error lines from stderr
                stderr_errors = filter_error_lines(stderr)
                if stderr_errors:
                    log_file.write(f"STDERR Errors:\n{stderr_errors}\n")
            
            test_results["steps"].append({
                "action": "modified_analysis", 
                "status": "completed",
                "stdout": stdout,
                "stderr": stderr
            })
        
        # Cleanup
        if os.path.exists(pure_file_path) and os.path.exists(original_err_file):
            os.remove(original_err_file)
            os.rename(pure_file_path, original_err_file)
            test_results["steps"].append({"action": "cleanup", "status": "completed"})
        
        if os.path.exists(os.path.join(project_dir, db_chg_name)):
            shutil.rmtree(os.path.join(project_dir, db_chg_name), ignore_errors=True)
            test_results["steps"].append({"action": "database_cleanup", "status": "completed"})

    except Exception as e:
        error_msg = str(e)
        if log_file:
            log_file.write(f"Error during test execution: {error_msg}\n")
        test_results["status"] = "error"
        test_results["error"] = error_msg
    
    return test_results

def set_up_scenarios_test_with_feedback(
    experiment_dir: str,
    experiment_file: str,
    llm_engine: str,
    log_file: Optional[TextIO] = None
) -> Dict[str, Any]:
    """
    Set up and run CodeQL tests for all experiment scenarios with enhanced feedback.
    
    Args:
        experiment_dir: Directory containing experiment files
        experiment_file: Name of the experiment file
        llm_engine: Name of the LLM engine used
        log_file: Optional file object for logging
        
    Returns:
        Dictionary containing overall test results and feedback
    """
    overall_results = {
        "status": "initialized",
        "experiment_file": experiment_file,
        "llm_engine": llm_engine,
        "results": [],
        "timestamp": datetime.datetime.now().isoformat()
    }
    
    llm_programs_dir = os.path.join(experiment_dir, "response",
                                   f"{experiment_file}.llm_programs")
    extra_meta_file = os.path.join(llm_programs_dir, "extrapolation_metadata.json")
    
    if not (os.path.exists(llm_programs_dir) and os.path.exists(extra_meta_file)):
        error_msg = "Required directories or metadata file not found"
        if log_file:
            log_file.write(f"Error: {error_msg}\n")
        overall_results["status"] = "error"
        overall_results["error"] = error_msg
        return overall_results
    
    try:
        # Process each response file
        with open(extra_meta_file, 'r', encoding="utf8") as f1:
            llm_response_files = json.load(f1)
            for response_info in llm_response_files:
                llm_response_file = response_info["filename"]
                duplicate_of = response_info.get("duplicate_of")
                temperature = response_info.get("temperature", 1.0)
                top_p = response_info.get("top_p", 1.0)
                
                if log_file:
                    log_file.write(f"Processing response file: {llm_response_file}\n")
                
                # Setup logging for this specific response
                feedback_log_file, codeql_log_file = setup_logging(
                    experiment_dir=experiment_dir,
                    response_file=llm_response_file
                )
                
                # Open the response-specific log file
                with open(codeql_log_file, 'w', encoding='utf8') as response_log:
                    # Run test for each response
                    test_result = run_codeql_test_with_feedback(
                        experiment_dir,
                        experiment_file,
                        llm_engine,
                        llm_response_file,
                        duplicate_of,
                        log_file=response_log
                    )
                    
                    overall_results["results"].append({
                        "response_file": llm_response_file,
                        "duplicate_of": duplicate_of,
                        "result": test_result
                    })

        gather_codeql_test_results_for_exp(experiment_dir, experiment_file, llm_engine)
        
        if log_file:
            log_file.write("Completed CodeQL test setup and execution\n")
        
    except Exception as e:
        error_msg = str(e)
        if log_file:
            log_file.write(f"Error during test execution: {error_msg}\n")
        overall_results["status"] = "error"
        overall_results["error"] = error_msg
    
    return overall_results

if __name__ == "__main__":
    # Example usage
    experiment_dir = r".\experiment\unity_real_basic\destroy_in_update\Packt_VRMaze_db!Assets!SteamVR!InteractionSystem!Longbow!Scripts!FireSource.cs~55~6~55~26"
    #experiment_dir = r".\experiment\cwe\constant_condition\hand_db_insert!Assets!Oculus!VR!Scripts!OVRPlugin.cs~1012~10~1012~27"
    #experiment_dir = r".\experiment\cwe\constant_condition\brick_db_insert!Assets!Oculus!VR!Scripts!OVRInput.cs~1499~11~1499~23"
    experiment_file = "FireSource.cs"
    llm_engine = "gpt-4-turbo"
    
    with open("codeql_test_log.txt", "w", encoding="utf8") as log_file:
        results = set_up_scenarios_test_with_feedback(
            experiment_dir,
            experiment_file,
            llm_engine,
            log_file
        )
        print(json.dumps(results, indent=2)) 