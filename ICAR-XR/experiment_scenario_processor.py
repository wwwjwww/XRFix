"""
Experiment Scenario Processor

Processes experiment scenarios from XRFix experiment folders.
Traverses experiment directories, loads scenario.json files, and prepares
them for ICAR-XR iterative repair processing.
"""

import os
import json
import logging
from typing import Dict, List, Optional, Tuple
from pathlib import Path

import sys
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import config as base_config



class ExperimentScenarioProcessor:
    """
    Processes experiment scenarios from XRFix experiment folders.
    Handles scenario.json loading, code file reading, and context preparation.
    """
    
    def __init__(self, experiment_root: str = None):
        """
        Initialize the scenario processor
        
        Args:
            experiment_root: Root directory containing experiment folders.
                            If None, uses default from config or current directory.
        """
        self.experiment_root = experiment_root or getattr(base_config, 'experiment_root', './experiment')
        self.logger = self._setup_logging()
        
    def _setup_logging(self) -> logging.Logger:
        """Setup logging"""
        logger = logging.getLogger('ICAR-XR.ScenarioProcessor')
        if not logger.handlers:
            handler = logging.StreamHandler()
            formatter = logging.Formatter(
                '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
            )
            handler.setFormatter(formatter)
            logger.addHandler(handler)
            logger.setLevel(logging.INFO)
            # Prevent propagation to root logger to avoid duplicate messages
            logger.propagate = False
        return logger
    
    def find_all_scenarios(self, root_dir: str = None) -> List[Dict]:
        """
        Find all scenario.json files in experiment directories
        
        Args:
            root_dir: Root directory to search. If None, uses self.experiment_root
            
        Returns:
            List of scenario dictionaries with metadata
        """
        root_dir = root_dir or self.experiment_root
        scenarios = []
        
        self.logger.info(f"Searching for scenarios in: {root_dir}")
        
        for root, dirs, files in os.walk(root_dir):
            if 'scenario.json' in files:
                scenario_path = os.path.join(root, 'scenario.json')
                scenario_data = self.load_scenario(scenario_path)
                if scenario_data:
                    scenarios.append({
                        'scenario_path': scenario_path,
                        'experiment_dir': root,
                        'scenario_data': scenario_data
                    })
        
        self.logger.info(f"Found {len(scenarios)} scenarios")
        return scenarios
    
    def load_scenario(self, scenario_path: str) -> Optional[Dict]:
        """
        Load and parse a scenario.json file
        
        Args:
            scenario_path: Path to scenario.json file
            
        Returns:
            Parsed scenario dictionary or None if failed
        """
        try:
            with open(scenario_path, 'r', encoding='utf8') as f:
                scenario = json.load(f)
            return scenario
        except Exception as e:
            self.logger.error(f"Error loading scenario {scenario_path}: {e}")
            return None
    
    def prepare_repair_context(self, scenario_data: Dict, experiment_dir: str) -> Dict:
        """
        Prepare repair context from scenario data
        
        Args:
            scenario_data: Parsed scenario.json contents
            experiment_dir: Experiment directory path
            
        Returns:
            Dictionary with repair context including:
            - code_snippet: Vulnerable code snippet
            - error_type: Type of error
            - error_location: Location information
            - project_context: Project-related information
            - codeql_query: CodeQL query path
        """
        err_info = scenario_data.get('err_detailed_info', {})
        file_name = err_info.get('file_name', '')
        experiment_file = file_name.split('/')[-1] if '/' in file_name else file_name
        
        # Determine error type
        if scenario_data.get('cwe_name'):
            error_type = scenario_data['cwe_name']
        elif scenario_data.get('unity_special_name'):
            error_type = scenario_data['unity_special_name']
        else:
            error_type = 'Unknown'
        
        # Load code snippet from prompt file
        code_snippet = self._load_code_snippet(experiment_dir, experiment_file)
        
        # Extract error location from scenario
        error_location = self._extract_error_location(err_info)
        
        # Get CodeQL query
        codeql_query = scenario_data.get('check_ql', '')
        
        # Prepare project context
        project_root_dir = scenario_data.get('project_root_dir', '')
        target_file_path = None
        if project_root_dir and file_name:
            # Construct absolute path
            target_file_path = os.path.join(project_root_dir, file_name.lstrip('/\\'))
        
        # Get baseline database path if available
        exp_dir = scenario_data.get('exp_dir', '')
        baseline_db_path = None
        if exp_dir and project_root_dir:
            db_name = exp_dir.split('!')[0]
            baseline_db_path = os.path.join(project_root_dir, db_name)
            if not os.path.exists(baseline_db_path):
                baseline_db_path = None

        combine_settings = getattr(base_config, 'CONTEXT_COMBINE_CWE', {})
        
        return {
            'code_snippet': code_snippet,
            'error_type': error_type,
            'error_location': error_location,
            'project_root_dir': project_root_dir,
            'target_file': file_name,
            'target_file_path': target_file_path,
            'baseline_db_path': baseline_db_path,
            'codeql_query': codeql_query,
            'experiment_dir': experiment_dir,
            'experiment_file': experiment_file,
            'exp_dir': exp_dir,  # Add exp_dir for database path extraction
            'include_addition': scenario_data.get('include_addition', False),
            'combine_settings':combine_settings[error_type],
            'scenario_data': scenario_data
        }
    
    def _load_code_snippet(self, experiment_dir: str, experiment_file: str) -> str:
        """
        Load code snippet from prompt file
        
        Args:
            experiment_dir: Experiment directory
            experiment_file: Experiment file name
            
        Returns:
            Code snippet string
        """
        # Fallback: try .prompt.cs file
        prompt_cs_file = os.path.join(experiment_dir, experiment_file + '.prompt.cs')
        if os.path.exists(prompt_cs_file):
            with open(prompt_cs_file, 'r', encoding='utf8') as f:
                return f.read()
        
        # Last resort: try to read from project file
        self.logger.warning(f"Could not find prompt file for {experiment_file}, returning empty string")
        return ""
    
    def _extract_error_location(self, err_info: Dict) -> Dict:
        """
        Extract error location information from scenario
        
        Args:
            scenario_data: Full scenario data
            err_info: Error detailed info section
            
        Returns:
            Dictionary with error location information
        """
        file_name = err_info.get('file_name', '')
        experiment_file = file_name.split('/')[-1] if '/' in file_name else file_name
        
        # Extract line/column info from err_info

        line_info = {
            'line': int(err_info.get('start_line')) if err_info.get('start_line') else None,
            'end_line': int(err_info.get('end_line')) if err_info.get('end_line') else None,
            'column_start': int(err_info.get('start_column')) if err_info.get('start_column') else None,
            'column_end': int(err_info.get('end_column')) if err_info.get('end_column') else None,
            'column': int(err_info.get('start_column')) if err_info.get('start_column') else None  # For compatibility
        }

        
        return {
            'file': experiment_file,
            'file_path': file_name,
            'function': err_info.get('function_name', 'Update'),  # Default to Update for Unity
            'add_file_path': err_info.get('add_file_name', ''),
            'add_start_line': int(err_info.get('add_start_line')) if err_info.get('add_start_line') else None,
            'add_end_line': int(err_info.get('add_end_line')) if err_info.get('add_end_line') else None,
            'add_start_column': int(err_info.get('add_start_column')) if err_info.get('add_start_column') else None,
            'add_end_column': int(err_info.get('add_end_column')) if err_info.get('add_end_column') else None,
            **line_info
        }
    
    def load_additional_code_files(self, experiment_dir: str, experiment_file: str, 
                                   include_addition: bool) -> Dict[str, str]:
        """
        Load additional code files needed for code merging
        
        Args:
            experiment_dir: Experiment directory
            experiment_file: Experiment file name
            include_addition: Whether additional files are included
            
        Returns:
            Dictionary with code file contents:
            - prepend_contents
            - append_contents
            - add_contents (if include_addition)
            - add_prepend_contents (if include_addition)
            - add_append_contents (if include_addition)
            - between_contents (if include_addition)
            - add_first (if include_addition)
        """
        files = {}
        file_extension = '.cs'
        
        # Load prepend file
        prepend_path = os.path.join(experiment_dir, experiment_file + '.prepend' + file_extension)
        if os.path.exists(prepend_path):
            with open(prepend_path, 'r', encoding='utf8') as f:
                files['prepend_contents'] = f.read()
        else:
            files['prepend_contents'] = ''
        
        # Load append file
        append_path = os.path.join(experiment_dir, experiment_file + '.append' + file_extension)
        if os.path.exists(append_path):
            with open(append_path, 'r', encoding='utf8') as f:
                files['append_contents'] = f.read()
        else:
            files['append_contents'] = ''
        
        # Load whitespace from prompt file
        prompt_path = os.path.join(experiment_dir, experiment_file + '.prompt' + file_extension)
        files['whitespace'] = ''
        if os.path.exists(prompt_path):
            with open(prompt_path, 'r', encoding='utf8') as f:
                files['prompt_contents'] = f.read()
                lines = f.readlines()
                if lines:
                    last_line = lines[-1]
                    whitespace_count = len(last_line) - len(last_line.lstrip())
                    files['whitespace'] = last_line[:whitespace_count]
        
        if include_addition:
            # Load add files
            add_prompt_path = os.path.join(experiment_dir, experiment_file + '.prompt_add' + file_extension)
            if os.path.exists(add_prompt_path):
                with open(add_prompt_path, 'r', encoding='utf8') as f:
                    files['add_contents'] = f.read()
                with open(add_prompt_path, 'r', encoding='utf8') as f:
                    add_lines = f.readlines()
                    if add_lines:
                        last_line = add_lines[-1]
                        add_whitespace_count = len(last_line) - len(last_line.lstrip())
                        files['add_whitespace'] = last_line[:add_whitespace_count]
            else:
                files['add_contents'] = ''
                files['add_whitespace'] = ''
            
            # Load add_prepend
            add_prepend_path = os.path.join(experiment_dir, experiment_file + '.prepend_add' + file_extension)
            if os.path.exists(add_prepend_path):
                with open(add_prepend_path, 'r', encoding='utf8') as f:
                    files['add_prepend_contents'] = f.read()
            else:
                files['add_prepend_contents'] = ''
            
            # Load add_append
            add_append_path = os.path.join(experiment_dir, experiment_file + '.append_add' + file_extension)
            if os.path.exists(add_append_path):
                with open(add_append_path, 'r', encoding='utf8') as f:
                    files['add_append_contents'] = f.read()
            else:
                files['add_append_contents'] = ''
            
            # Load between and add_first
            between_path = os.path.join(experiment_dir, experiment_file + '.between' + file_extension)
            if os.path.exists(between_path):
                with open(between_path, 'r', encoding='utf8') as f:
                    files['between_contents'] = f.read()
                
                add_first_path = os.path.join(experiment_dir, experiment_file + '.add_first.txt')
                if os.path.exists(add_first_path):
                    with open(add_first_path, 'r', encoding='utf8') as f:
                        files['add_first'] = f.read()
                else:
                    files['add_first'] = ''
            else:
                files['between_contents'] = ''
                files['add_first'] = ''
        else:
            files['add_contents'] = ''
            files['add_prepend_contents'] = ''
            files['add_append_contents'] = ''
            files['between_contents'] = ''
            files['add_first'] = ''
            files['add_whitespace'] = ''
        
        return files


def process_experiment_scenarios(experiment_root: str, 
                                 callback=None,
                                 filter_func=None) -> List[Dict]:
    """
    Convenience function to process all scenarios in experiment root
    
    Args:
        experiment_root: Root directory containing experiment folders
        callback: Optional callback function to process each scenario
        filter_func: Optional function to filter scenarios
        
    Returns:
        List of processed scenario contexts
    """
    processor = ExperimentScenarioProcessor(experiment_root)
    scenarios = processor.find_all_scenarios()
    
    processed = []
    for scenario_info in scenarios:
        scenario_data = scenario_info['scenario_data']
        experiment_dir = scenario_info['experiment_dir']
        
        # Apply filter if provided
        if filter_func and not filter_func(scenario_data):
            continue
        
        # Prepare repair context
        context = processor.prepare_repair_context(scenario_data, experiment_dir)
        
        # Load additional code files
        additional_files = processor.load_additional_code_files(
            experiment_dir,
            context['experiment_file'],
            context['include_addition']
        )
        context.update(additional_files)
        
        # Call callback if provided
        if callback:
            callback(context)
        
        processed.append(context)
    
    return processed


if __name__ == "__main__":
    # Example usage
    processor = ExperimentScenarioProcessor('./experiment')
    scenarios = processor.find_all_scenarios()
    
    print(f"Found {len(scenarios)} scenarios")
    for scenario_info in scenarios[:5]:  # Show first 5
        context = processor.prepare_repair_context(
            scenario_info['scenario_data'],
            scenario_info['experiment_dir']
        )
        print(f"\nScenario: {context['experiment_file']}")
        print(f"Error Type: {context['error_type']}")
        print(f"Location: {context['error_location']}")
