"""
Validator Module

Validates code fixes using CodeQL and compilation checks.
Provides feedback and severity assessment for failed repairs.
"""

import os
import re
import json
import subprocess
import csv
import tempfile
import shutil
import sys
import time
from typing import Dict, List, Tuple, Optional
from enum import Enum
from dataclasses import dataclass

# Import FileLock for concurrent access safety (optional)
from filelock import FileLock
FILELOCK_AVAILABLE = True


# Add parent directory to path to import AST utilities
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from AST_tree import print_AST
from experiment_get_result_new import determine_which_merge_strategy



class VulnerabilityStatus(Enum):
    """Vulnerability detection status"""
    FIXED = "fixed"
    PARTIALLY_FIXED = "partially_fixed"
    NOT_FIXED = "not_fixed"
    COMPILABLE_NOT_FIXED = "compilable_not_fixed"
    COMPILATION_FAILED = "compilation_failed"
    DELETED_ERROR_CODE = "llm_fixed_by_delete_error_code"
    UNKNOWN = "unknown"


@dataclass
class ValidationResult:
    """Result of code validation"""
    is_valid: bool
    status: VulnerabilityStatus
    message: str
    vulnerabilities_found: List[Dict]
    compilation_errors: List[str]
    warnings: List[str]
    metadata: Dict


class CodeValidator:
    """
    Validates code fixes using multiple strategies.
    Inherits from and extends XRFix's validation approach.
    """
    
    def __init__(self, config: Optional[Dict] = None):
        """
        Initialize validator
        
        Args:
            config: Validation configuration
        """
        self.config = config or {}
        self.method = self.config.get('method', 'codeql')
        self.strict_mode = self.config.get('strict_mode', True)
        self.allow_new_warnings = self.config.get('allow_new_warnings', False)
        self.query_root_dir = self.config.get('query_root_dir', '')
        self.codeql_binary = self.config.get('codeql_binary', 'codeql')
        
        # Setup logger
        import logging
        self.logger = logging.getLogger('ICAR-XR.Validator')
        
        # Load CodeQL query mappings from config module
        try:
            import config as config_module
            self.query_mapping = getattr(config_module, 'QUERY_MAPPING', {})
            self.unity_query_mapping = getattr(config_module, 'UNITY_QUERY_MAPPING', {})
            self.cwe_query_mapping = getattr(config_module, 'CWE_QUERY_MAPPING', {})
            self.unity_real_query_mapping = getattr(config_module, 'UNITY_REAL_QUERY_MAPPING', {})
            if not self.query_root_dir:
                self.query_root_dir = getattr(config_module, 'QUERY_ROOT_DIR', '') or getattr(config_module, 'query_root_dir', '')
            
            # Also try to import from base config
            import sys
            import os
            sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
            try:
                import config as base_config_module
                if not self.query_root_dir:
                    self.query_root_dir = getattr(base_config_module, 'query_root_dir', '')
            except:
                pass
        except ImportError:
            self.query_mapping = {}
            self.unity_query_mapping = {}
            self.cwe_query_mapping = {}
            self.unity_real_query_mapping = {}
    
    def validate(self, original_code: str, fixed_code: str,
                error_type: str, codeql_query: Optional[str] = None,
                validation_context: Optional[Dict] = None,
                merge_strategy: Optional[str] = None,
                merge_context: Optional[Dict] = None) -> ValidationResult:
        """
        Validate a code fix
        
        Args:
            original_code: Original code with vulnerability (may be snippet or full file)
            fixed_code: Fixed code snippet to validate (needs to be merged)
            error_type: Type of error being fixed
            codeql_query: Optional CodeQL query to run
            validation_context: Optional project validation context.
                Supported keys:
                - project_root_dir: Absolute project root path for CodeQL DB creation
                - target_file_path: Absolute path of vulnerable file in project
                - target_file: Relative or absolute vulnerable file path
                - baseline_db_path: Optional existing CodeQL DB for original baseline
            merge_strategy: Strategy for merging fixed code ('single_line', 'function', 'function_special')
            merge_context: Context for merging (prepend_contents, append_contents, etc.)
            
        Returns:
            ValidationResult with detailed feedback
        """
        results = {
            'codeql': None,
            'compilation': None,
            'warnings': []
        }
        
        # Merge fixed code with original code if merge strategy is provided
        merged_fixed_code = fixed_code
        if merge_strategy and merge_context:
            merged_fixed_code = self._merge_fixed_code_with_original(
                original_code, fixed_code, merge_strategy, merge_context
            )
        
        # If codeql_query not provided, try to get it from config based on error_type
        if not codeql_query:
            codeql_query = self._get_codeql_query_from_error_type(error_type)
            # Also try to get from validation_context
            if not codeql_query and validation_context:
                codeql_query = validation_context.get('codeql_query')
        
        # Optionally record merged code for each patch/iteration
        self._record_merged_patch_if_needed(
            merged_fixed_code, validation_context
        )

        # Run validation based on configured method
        # NOTE: For ICAR-XR we prioritize real CodeQL validation instead of syntax-only checks.
        if self.method in ['both', 'codeql']:
            if codeql_query:
                codeql_result = self._run_codeql_validation(
                    original_code, merged_fixed_code, codeql_query, validation_context
                )
                if validation_context and codeql_result is not None:
                    codeql_result['project_root_dir'] = validation_context.get('project_root_dir')
                    codeql_result['target_file_path'] = validation_context.get('target_file_path')
                results['codeql'] = codeql_result
                if codeql_result and codeql_result.get('compilation') is not None:
                    results['compilation'] = codeql_result.get('compilation')
            else:
                results['warnings'].append(
                    f"No CodeQL query configured for error type: {error_type}"
                )
        elif self.method == 'compilation':
            # Kept only for backward compatibility.
            results['compilation'] = self._run_compilation_validation(merged_fixed_code)
        
        # Run tree-sitter structure comparison check
        structure_check_result = self._check_code_structure_integrity(
            original_code, merged_fixed_code, validation_context
        )
        results['structure_check'] = structure_check_result
        
        # Synthesize results
        return self._synthesize_validation_result(
            results, original_code, merged_fixed_code
        )
    
    def _run_codeql_validation(self, original_code: str, fixed_code: str,
                              query: str, validation_context: Optional[Dict] = None) -> Dict:
        """
        Run CodeQL validation (inherits from XRFix approach)
        
        Args:
            original_code: Original vulnerable code
            fixed_code: Fixed code
            query: CodeQL query to run
            
        Returns:
            Dictionary with CodeQL results
        """
        self.logger.info("=" * 70)
        self.logger.info("Starting CodeQL Validation")
        self.logger.info("=" * 70)
        self.logger.info(f"Query: {query}")
        
        resolved_query = self._resolve_query_path(query)
        if not resolved_query or not os.path.exists(resolved_query):
            self.logger.error(f"CodeQL query not found: {query}")
            self.logger.error(f"Resolved path: {resolved_query}")
            return {
                'original_vulnerabilities': [],
                'fixed_vulnerabilities': [],
                'vulnerability_reduction': 0.0,
                'query_used': resolved_query or query,
                'execution_error': f"CodeQL query not found: {query}",
                'original_count': 0,
                'fixed_count': 0
            }

        self.logger.info(f"Resolved Query Path: {resolved_query}")
        
        # Require validation_context for project-level validation
        if not validation_context:
            return {
                'original_vulnerabilities': [],
                'fixed_vulnerabilities': [],
                'vulnerability_reduction': 0.0,
                'query_used': resolved_query,
                'execution_error': 'validation_context is required for CodeQL validation',
                'original_count': 0,
                'fixed_count': 0
            }
        
        # Use project-level validation (replacing target file)
        project_result = self._run_codeql_validation_on_project(
            original_code=original_code,
            fixed_code=fixed_code,
            query_path=resolved_query,
            validation_context=validation_context
        )
        
        if project_result is not None:
            return project_result
        
        # If project validation failed, return error
        return {
            'original_vulnerabilities': [],
            'fixed_vulnerabilities': [],
            'vulnerability_reduction': 0.0,
            'query_used': resolved_query,
            'execution_error': 'Project-level validation failed',
            'original_count': 0,
            'fixed_count': 0
        }

    def _record_merged_patch_if_needed(self, merged_code: str,
                                       validation_context: Optional[Dict]) -> None:
        """
        Save merged code for each patch/iteration into the LLM programs folder.
        Mirrors experiment_get_result_new.py output layout:
        {experiment_dir}/response/{experiment_file}.llm_programs
        """
        if not validation_context:
            return

        experiment_dir = validation_context.get('experiment_dir')
        experiment_file = validation_context.get('experiment_file')
        patch_index = validation_context.get('patch_index')
        iteration = validation_context.get('iteration')
        llm_model = validation_context.get('llm_model', 'llm')

        if not experiment_dir or not experiment_file or patch_index is None:
            return

        llm_programs_dir = os.path.join(
            experiment_dir,
            "response",
            experiment_file + ".llm_programs"
        )
        os.makedirs(llm_programs_dir, exist_ok=True)

        # Use iteration in filename to avoid overwriting multi-iteration results.
        if iteration is None:
            filename = f"{llm_model}_patch_{patch_index}.cs"
        else:
            filename = f"{llm_model}_patch_{patch_index}_iter_{iteration}.cs"

        output_path = os.path.join(llm_programs_dir, filename)
        with open(output_path, "w", encoding="utf-8") as f:
            f.write(merged_code)

    def _run_codeql_validation_on_project(self, original_code: str, fixed_code: str,
                                          query_path: str, validation_context: Dict) -> Optional[Dict]:
        """
        Run CodeQL validation against the real project by replacing the target file.

        This mirrors the experiment workflow from experiment_run_codeql_test.py:
        1) Analyze original baseline (existing DB or temporary DB from project)
        2) Backup original file (rename to _original)
        3) Replace target file with fixed code
        4) Create/analyze changed DB
        5) Restore original file
        """
        project_root = os.path.abspath(validation_context.get('project_root_dir'))
        target_file_path = os.path.abspath(validation_context.get('target_file_path'))
        baseline_db_path = os.path.abspath(validation_context.get('baseline_db_path')) if validation_context.get('baseline_db_path') else None

        resolved_query = self._resolve_query_path(query_path)
        if not resolved_query or not os.path.exists(resolved_query):
            return {
                'original_vulnerabilities': [],
                'fixed_vulnerabilities': [],
                'vulnerability_reduction': 0.0,
                'query_used': query_path,
                'execution_error': f"CodeQL query not found: {query_path}",
                'original_count': 0,
                'fixed_count': 0
            }

        # Build backup file path following experiment_run_codeql_test.py logic
        # Format: {pure_file_name}_original.cs
        target_file_dir = os.path.dirname(target_file_path)
        target_file_name = os.path.basename(target_file_path)
        pure_file_name = os.path.splitext(target_file_name)[0]  # Remove extension
        file_extension = os.path.splitext(target_file_name)[1]  # Get extension (.cs)
        pure_file_name_original = pure_file_name + "_original"
        backup_file_path = os.path.join(target_file_dir, pure_file_name_original + file_extension)
        
        # Use FileLock if available for concurrent access safety (following experiment_run_codeql_test.py)
        lock_path = os.path.join(project_root, ".repo.lock")
        
        lock_context = FileLock(lock_path)

        with lock_context:
            
            try:
                experiment_dir = validation_context.get('experiment_dir', tempfile.gettempdir())
                llm_engine = validation_context.get('llm_engine', 'icar-xr')
                result_dir = os.path.join(experiment_dir, 'result', llm_engine)
                os.makedirs(result_dir, exist_ok=True)

                patch_index = validation_context.get('patch_index')
                iteration = validation_context.get('iteration')
                ts_ns = time.time_ns()
                suffix_parts = []
                if patch_index is not None:
                    suffix_parts.append(f"patch{patch_index}")
                if iteration is not None:
                    suffix_parts.append(f"iter{iteration}")
                suffix_parts.append(str(ts_ns))
                suffix = "_".join(suffix_parts)

                # Baseline (original) results are identical across patches/iterations when
                # `baseline_db_path` and `query` are unchanged, so keep a stable filename to
                # avoid redundant duplicates in experiment outputs.
                result_original_csv = os.path.join(result_dir, "result_original.csv")
                result_fixed_csv = os.path.join(result_dir, f"result_fixed_{suffix}.csv")

                # Extract database name from exp_dir
                db_name = None
                exp_dir = validation_context.get('exp_dir', '')
                
                if exp_dir:
                    db_name = exp_dir.split('!')[0]

                original_result = {'rows': [], 'count': 0, 'error': None}
                if baseline_db_path and os.path.exists(baseline_db_path):
                    original_result = self._run_codeql_analyze_database(
                        db_dir=baseline_db_path,
                        query_path=resolved_query,
                        output_csv=result_original_csv
                    )

                # Backup and replace file following experiment_run_codeql_test.py logic
                if not os.path.exists(backup_file_path):
                    if os.path.exists(target_file_path):
                        os.rename(target_file_path, backup_file_path)

                # Write fixed code to target file location
                with open(target_file_path, 'w', encoding='utf-8') as f:
                    f.write(fixed_code)

                db_chg_name = db_name + "_fixed_db"
                db_chg_path = os.path.join(project_root, db_chg_name)

                # Analyze fixed code
                fixed_result = self._run_codeql_for_project(
                    project_root=project_root,
                    db_dir=db_chg_path,
                    query_path=resolved_query,
                    output_csv=result_fixed_csv
                )
                fixed_result['fixed_line_contents'] = self._build_line_content_map(
                    fixed_result.get('rows', []),
                    project_root
                )

                # Clean up temporary DB
                if os.path.exists(db_chg_path):
                    shutil.rmtree(db_chg_path, ignore_errors=True)

                fixed_compilation = fixed_result.get('compilation')

                if original_result.get('error') or fixed_result.get('error'):
                    return {
                        'original_vulnerabilities': original_result.get('rows', []),
                        'fixed_vulnerabilities': fixed_result.get('rows', []),
                        'vulnerability_reduction': 0.0,
                        'query_used': resolved_query,
                        'execution_error': (
                            original_result.get('error') or fixed_result.get('error')
                        ),
                        'original_count': original_result.get('count', 0),
                        'fixed_count': fixed_result.get('count', 0),
                        'compilation': fixed_compilation
                    }

                original_count = original_result.get('count', 0)
                fixed_count = fixed_result.get('count', 0)

                reduction = 0.0
                if original_count > 0:
                    reduction = max(0.0, (original_count - fixed_count) / original_count)

                # Log summary
                if original_count > 0 and fixed_count == 0:
                    self.logger.info(f"CodeQL: {original_count} -> {fixed_count} vulnerabilities (FIXED)")
                elif fixed_count < original_count:
                    self.logger.info(f"CodeQL: {original_count} -> {fixed_count} vulnerabilities (PARTIAL)")
                elif fixed_count == original_count:
                    self.logger.info(f"CodeQL: {original_count} -> {fixed_count} vulnerabilities (NOT FIXED)")
                else:
                    self.logger.info(f"CodeQL: {original_count} -> {fixed_count} vulnerabilities")

                return {
                    'original_vulnerabilities': original_result.get('rows', []),
                    'fixed_vulnerabilities': fixed_result.get('rows', []),
                    'fixed_line_contents': fixed_result.get('fixed_line_contents', {}),
                    'vulnerability_reduction': reduction,
                    'query_used': resolved_query,
                    'execution_error': None,
                    'original_count': original_count,
                    'fixed_count': fixed_count,
                    'compilation': fixed_compilation
                }

            except Exception as e:
                self.logger.error(f"CodeQL validation error: {e}")
                return None
            
            finally:
                # Always restore file content (critical: must restore original file)
                if backup_file_path and os.path.exists(backup_file_path):
                    if os.path.exists(target_file_path):
                        try:
                            os.remove(target_file_path)
                        except Exception:
                            pass
                    try:
                        os.rename(backup_file_path, target_file_path)
                    except Exception as e:
                        self.logger.error(f"Failed to restore file: {e}")


    def _run_codeql_for_project(self, project_root: str, db_dir: str,
                                query_path: str, output_csv: str) -> Dict:
        """Create a CodeQL DB from a project root and analyze with the given query."""
        create_cmd = [
            self.codeql_binary,
            'database',
            'create',
            db_dir,
            '--language=csharp',
            '--source-root',
            project_root,
            '--overwrite'
        ]

        analyze_cmd = [
            self.codeql_binary,
            'database',
            'analyze',
            db_dir,
            query_path,
            '--format=csv',
            '--output',
            output_csv,
            '--rerun'
        ]

        return self._execute_codeql_commands(create_cmd, analyze_cmd, output_csv)

    def _run_codeql_analyze_database(self, db_dir: str, query_path: str,
                                     output_csv: str) -> Dict:
        """Analyze an existing CodeQL DB with the given query."""
        analyze_cmd = [
            self.codeql_binary,
            'database',
            'analyze',
            db_dir,
            query_path,
            '--format=csv',
            '--output',
            output_csv,
            '--rerun'
        ]

        return self._execute_codeql_commands(None, analyze_cmd, output_csv)

    def _execute_codeql_commands(self, create_cmd: Optional[List[str]],
                                 analyze_cmd: List[str], output_csv: str) -> Dict:
        """Execute CodeQL command sequence and parse CSV output."""
        compilation_result = None
        try:
            if create_cmd:
                self.logger.info(f"CodeQL database create: {' '.join(create_cmd)}")
                subprocess.run(create_cmd, check=True, capture_output=True, text=True)
                compilation_result = {
                    'compiles': True,
                    'errors': []
                }

            self.logger.info(f"CodeQL database analyze: {' '.join(analyze_cmd)}")
            subprocess.run(analyze_cmd, check=True, capture_output=True, text=True)
        except FileNotFoundError:
            error_msg = f"CodeQL CLI not found: {self.codeql_binary}"
            self.logger.error(error_msg)
            return {
                'rows': [],
                'count': 0,
                'error': error_msg,
                'compilation': {
                    'compiles': False,
                    'errors': [error_msg]
                }
            }
        except subprocess.CalledProcessError as e:
            stderr = (e.stderr or '').strip()
            stdout = (e.stdout or '').strip()
            # Prefer the most relevant part of the output: lines that contain "error".
            # This makes downstream logs/JSON easier to read.
            combined_output = "\n".join([stderr, stdout]).strip()
            error_lines = [
                line for line in combined_output.splitlines()
                if "error" in line.lower()
            ]
            err_msg = "\n".join(error_lines).strip() if error_lines else (stderr if stderr else stdout)
            self.logger.error(f"CodeQL command failed: {err_msg[:200]}")
            if create_cmd and compilation_result is None:
                compilation_result = {
                    'compiles': False,
                    'errors': [err_msg]
                }
            return {
                'rows': [],
                'count': 0,
                'error': f"CodeQL command failed: {err_msg}",
                'compilation': compilation_result
            }

        rows = []
        if os.path.exists(output_csv):
            with open(output_csv, 'r', encoding='utf-8', newline='') as f:
                reader = csv.reader(f)
                for row in reader:
                    if row:
                        rows.append(row)

        return {
            'rows': rows,
            'count': len(rows),
            'error': None,
            'compilation': compilation_result
        }

    def _build_line_content_map(self, rows: List[List[str]],
                                project_root_dir: Optional[str]) -> Dict[str, str]:
        content_map: Dict[str, str] = {}
        if not rows or not project_root_dir:
            return content_map
        for row in rows:
            if len(row) <= 5:
                continue
            file_path = row[4]
            line_str = row[5] if len(row) > 5 else ""
            try:
                line_num = int(line_str)
            except (TypeError, ValueError):
                continue
            resolved_path = file_path
            if project_root_dir:
                if not os.path.isabs(file_path):
                    resolved_path = os.path.join(project_root_dir, file_path.lstrip("/\\"))
                elif not os.path.exists(file_path) and file_path.startswith(("/", "\\")):
                    resolved_path = os.path.join(project_root_dir, file_path.lstrip("/\\"))
            if not os.path.exists(resolved_path):
                continue
            try:
                with open(resolved_path, "r", encoding="utf-8", errors="ignore") as f:
                    lines = f.readlines()
                if 1 <= line_num <= len(lines):
                    key = f"{file_path}:{line_str}"
                    content_map[key] = lines[line_num - 1].rstrip()
            except Exception:
                continue
        return content_map

    def _resolve_query_path(self, query: str) -> str:
        """Resolve query path against configured query root."""
        if not query:
            return query

        if os.path.isabs(query) and os.path.exists(query):
            return query

        # Query mappings commonly start with '/...'; remove leading slash for safe join.
        query_rel = query.lstrip('/\\')
        if self.query_root_dir:
            candidate = os.path.join(self.query_root_dir, query_rel)
            if os.path.exists(candidate):
                return candidate

        return query

    def _run_compilation_validation(self, code: str) -> Dict:
        """
        Check if code compiles (syntax validation)
        
        Args:
            code: Code to validate
            
        Returns:
            Dictionary with compilation results
        """
        result = {
            'compiles': True,
            'errors': [],
            'warnings': [],
            'error_count': 0,
            'warning_count': 0
        }
        
        # Basic syntax checking
        syntax_errors = self._check_syntax(code)
        if syntax_errors:
            result['compiles'] = False
            result['errors'] = syntax_errors
            result['error_count'] = len(syntax_errors)
        
        return result
    
    def _check_syntax(self, code: str) -> List[str]:
        """
        Basic C# syntax checking
        
        Args:
            code: Code to check
            
        Returns:
            List of syntax errors found
        """
        errors = []
        
        # Check for unmatched braces
        open_braces = code.count('{') - code.count('}')
        if open_braces != 0:
            errors.append(f"Unmatched braces: {abs(open_braces)} extra")
        
        # Check for unmatched parentheses
        open_parens = code.count('(') - code.count(')')
        if open_parens != 0:
            errors.append(f"Unmatched parentheses: {abs(open_parens)} extra")
        
        # Check for unterminated strings
        quote_count = code.count('"') - code.count('\\"')
        if quote_count % 2 != 0:
            errors.append("Unterminated string literal")
        
        return errors
    
    def _merge_fixed_code_with_original(self, original_code: str, fixed_code: str,
                                       merge_strategy: str, merge_context: Dict) -> str:
        """
        Merge fixed code snippet with original code using specified strategy.
        This mirrors the logic from experiment_get_result.py
        
        Args:
            original_code: Original full code
            fixed_code: Fixed code snippet
            merge_strategy: 'single_line', 'function', or 'function_special'
            merge_context: Dictionary with prepend_contents, append_contents, etc.
            
        Returns:
            Merged code string
        """
        comment_key = merge_context.get('comment_key', '//')
        prepend_contents = merge_context.get('prepend_contents', '')
        append_contents = merge_context.get('append_contents', '')
        whitespace = merge_context.get('whitespace', '')
        add_contents = merge_context.get('add_contents', '')
        add_whitespace = merge_context.get('add_whitespace', '')
        between_contents = merge_context.get('between_contents', '')
        add_first = merge_context.get('add_first', '')
        add_append_contents = merge_context.get('add_append_contents', '')
        add_prepend_contents = merge_context.get('add_prepend_contents', '')
        include_addition = merge_context.get('include_addition', False)
        prompt_contents = merge_context.get('prompt_contents', '')
        
        # Determine if function-based merge
        is_function = merge_context.get('is_function')
        if is_function is None and include_addition:
            # Try to infer from file names
            target_file = merge_context.get('target_file', '')
            add_file = merge_context.get('add_file', '')
            is_function = (target_file == add_file)
        
        if not include_addition:
            contents = original_code
        else:
            contents = prompt_contents
        
        
        return determine_which_merge_strategy(
            merge_strategy, include_addition, is_function,
            comment_key, prepend_contents, contents, add_contents,
            append_contents, fixed_code, whitespace, add_whitespace,
            between_contents, add_first, add_append_contents,
            add_prepend_contents
        )
    
    def _get_codeql_query_from_error_type(self, error_type: str) -> Optional[str]:
        """
        Get CodeQL query path from error type using config mappings
        
        Args:
            error_type: Type of error (e.g., "Using New() allocation in Update() method.")
            
        Returns:
            CodeQL query path or None if not found
        """
        # Try to find query in combined mapping first
        if error_type in self.query_mapping:
            return self.query_mapping[error_type]
        
        # Then try specific mappings
        if error_type in self.unity_real_query_mapping:
            return self.unity_real_query_mapping[error_type]
        
        if error_type in self.unity_query_mapping:
            return self.unity_query_mapping[error_type]
        
        if error_type in self.cwe_query_mapping:
            return self.cwe_query_mapping[error_type]
        
        return None
    
    def _check_code_structure_integrity(self, original_code: str, fixed_code: str,
                                        validation_context: Optional[Dict] = None) -> Dict:
        """
        Check code structure integrity using tree-sitter to detect missing function calls.
        
        Compares the original code and fixed code to detect if LLM fixed the issue
        by simply deleting error code (missing function calls).
        
        Args:
            original_code: Original code with vulnerability (may be snippet)
            fixed_code: Fixed/merged code to check
            validation_context: Optional validation context with target_file_path
            
        Returns:
            Dictionary with structure check results:
            - has_missing_function_calls: bool
            - missing_function_calls: List[str] - list of missing function call signatures
            - error: Optional[str] - error message if check failed
        """
        result = {
            'has_missing_function_calls': False,
            'missing_function_calls': [],
            'error': None
        }
        
        try:
            # If we have target_file_path, try to read the original file for comparison
            # Otherwise use the provided original_code
            original_code_to_check = original_code
            if validation_context:
                target_file_path = validation_context.get('target_file_path')
                if target_file_path and os.path.exists(target_file_path):
                    # Try to read original file (it might be backed up)
                    backup_path = self._get_backup_file_path(target_file_path)
                    if backup_path and os.path.exists(backup_path):
                        with open(backup_path, 'r', encoding='utf-8', errors='ignore') as f:
                            original_code_to_check = f.read()
                    elif os.path.exists(target_file_path):
                        # If no backup, the current file might be the original
                        # But we should use the provided original_code instead
                        pass
            
            # Extract function calls from original and fixed code
            original_calls = self._extract_function_calls(original_code_to_check)
            fixed_calls = self._extract_function_calls(fixed_code)
            
            # Find missing function calls
            # We compare call signatures (method name + parameter count)
            original_call_set = set(original_calls)
            fixed_call_set = set(fixed_calls)
            
            missing_calls = original_call_set - fixed_call_set
            
            # Filter out calls that might be intentionally removed (e.g., error-prone calls)
            # We focus on significant function calls that shouldn't be missing
            significant_missing = self._filter_significant_missing_calls(
                missing_calls, original_code, fixed_code
            )
            
            if significant_missing:
                result['has_missing_function_calls'] = True
                result['missing_function_calls'] = list(significant_missing)
                self.logger.warning(
                    f"Structure check detected {len(significant_missing)} missing function calls: "
                    f"{', '.join(list(significant_missing)[:5])}"
                )
        
        except Exception as e:
            self.logger.error(f"Structure integrity check failed: {e}")
            result['error'] = str(e)
            # On error, don't fail the check (fail-safe)
        
        return result
    
    def _extract_function_calls(self, code: str) -> List[str]:
        """
        Extract function call signatures from code using tree-sitter.
        
        Args:
            code: Source code to analyze
            
        Returns:
            List of function call signatures (format: "method_name(param_count)")
        """
        calls = []
        
        try:
            # Use tree-sitter to parse the code
            root = print_AST.read_code_AST(code)
            
            # Traverse AST to find invocation expressions
            def traverse_invocations(node):
                # invocation_expression is the node type for method calls in C#
                if node.type == 'invocation_expression':
                    # Find the method name (usually in member_access_expression or identifier)
                    method_name = self._extract_method_name_from_invocation(node, code)
                    param_count = self._count_parameters(node)
                    
                    if method_name:
                        call_signature = f"{method_name}({param_count})"
                        calls.append(call_signature)
                
                # Recursively traverse children
                for child in node.children:
                    traverse_invocations(child)
            
            traverse_invocations(root)
        
        except Exception as e:
            self.logger.debug(f"Error extracting function calls: {e}")
            # Fallback: try simple regex-based extraction
            calls = self._extract_function_calls_fallback(code)
        
        return calls
    
    def _extract_method_name_from_invocation(self, invocation_node, code: str) -> Optional[str]:
        """
        Extract method name from an invocation_expression node.
        
        Args:
            invocation_node: tree-sitter invocation_expression node
            code: Original source code
            
        Returns:
            Method name or None
        """
        # invocation_expression structure:
        # - member_access_expression (for obj.Method())
        #   - identifier (object)
        #   - identifier (method)
        # - identifier (for direct Method())
        
        for child in invocation_node.children:
            if child.type == 'member_access_expression':
                # Get the last identifier (method name)
                identifiers = [c for c in child.children if c.type == 'identifier']
                if identifiers:
                    method_name = identifiers[-1].text.decode('utf-8')
                    return method_name
            elif child.type == 'identifier':
                # Direct method call
                method_name = child.text.decode('utf-8')
                return method_name
        
        return None
    
    def _count_parameters(self, invocation_node) -> int:
        """
        Count parameters in an invocation expression.
        
        Args:
            invocation_node: tree-sitter invocation_expression node
            
        Returns:
            Number of parameters
        """
        # Look for argument_list in children
        for child in invocation_node.children:
            if child.type == 'argument_list':
                # Count comma-separated arguments
                # Each argument is a child node (excluding commas)
                args = [c for c in child.children if c.type != ',']
                return len(args)
        
        return 0
    
    def _extract_function_calls_fallback(self, code: str) -> List[str]:
        """
        Fallback function call extraction using regex (less accurate but more robust).
        
        Args:
            code: Source code
            
        Returns:
            List of function call signatures
        """
        calls = []
        
        # Simple regex pattern for method calls: identifier(...)
        # This is a simplified approach and may have false positives
        pattern = r'(\w+)\s*\([^)]*\)'
        matches = re.finditer(pattern, code)
        
        for match in matches:
            method_name = match.group(1)
            # Count parameters by counting commas + 1
            param_text = match.group(0)[match.group(0).find('(')+1:match.group(0).rfind(')')]
            param_count = len([p for p in param_text.split(',') if p.strip()]) if param_text.strip() else 0
            call_signature = f"{method_name}({param_count})"
            calls.append(call_signature)
        
        return calls
    
    def _filter_significant_missing_calls(self, missing_calls: set, 
                                         original_code: str, fixed_code: str) -> set:
        """
        Filter out insignificant missing calls (e.g., error-prone calls that should be removed).
        
        We want to detect when LLM deletes important function calls, not when it
        legitimately removes error-prone code. However, if too many calls are missing,
        it likely indicates deletion-based fixing.
        
        Args:
            missing_calls: Set of missing function call signatures
            original_code: Original code
            fixed_code: Fixed code
            
        Returns:
            Set of significant missing calls
        """
        if not missing_calls:
            return set()
        
        significant = set()

        
        # Extract all calls from both codes for comparison
        original_all_calls = self._extract_function_calls(original_code)
        fixed_all_calls = self._extract_function_calls(fixed_code)
        
        # Calculate the ratio of missing calls
        total_original_calls = len(set(original_all_calls))
        missing_count = len(missing_calls)
        
        # If a significant portion of calls are missing (>20%), it's likely deletion
        if total_original_calls > 0:
            missing_ratio = missing_count / total_original_calls
            if missing_ratio > 0.2:  # More than 20% of calls missing
                # This indicates significant deletion
                significant = missing_calls
                return significant
        
        return significant
    
    def _get_backup_file_path(self, target_file_path: str) -> Optional[str]:
        """
        Get the backup file path for a target file.
        
        Args:
            target_file_path: Path to the target file
            
        Returns:
            Backup file path or None
        """
        target_file_dir = os.path.dirname(target_file_path)
        target_file_name = os.path.basename(target_file_path)
        pure_file_name = os.path.splitext(target_file_name)[0]
        file_extension = os.path.splitext(target_file_name)[1]
        pure_file_name_original = pure_file_name + "_original"
        backup_file_path = os.path.join(target_file_dir, pure_file_name_original + file_extension)
        return backup_file_path if os.path.exists(backup_file_path) else None
    
    def _synthesize_validation_result(self, results: Dict, 
                                     original_code: str,
                                     fixed_code: str) -> ValidationResult:
        """
        Synthesize validation results into a single ValidationResult
        
        Args:
            results: Dictionary with individual validation results
            original_code: Original code
            fixed_code: Fixed code
            
        Returns:
            Synthesized ValidationResult
        """
        is_valid = True
        status = VulnerabilityStatus.UNKNOWN
        vulnerabilities = []
        compilation_errors = []
        warnings = []
        
        # Check CodeQL results (primary validation path)
        original_vulnerabilities = []
        fixed_vulnerabilities = []
        fixed_line_contents = {}
        project_root_dir = None
        target_file_path = None
        if results['codeql']:
            codeql_result = results['codeql']
            original_vulnerabilities = codeql_result.get('original_vulnerabilities', [])
            fixed_vulnerabilities = codeql_result.get('fixed_vulnerabilities', [])
            fixed_line_contents = codeql_result.get('fixed_line_contents', {})
            project_root_dir = codeql_result.get('project_root_dir')
            target_file_path = codeql_result.get('target_file_path')

            if codeql_result.get('execution_error'):
                is_valid = False
                status = VulnerabilityStatus.NOT_FIXED
                warnings.append(codeql_result['execution_error'])
            else:
                original_count = codeql_result.get('original_count', 0)
                fixed_count = codeql_result.get('fixed_count', 0)

                vulnerabilities = codeql_result.get('fixed_vulnerabilities', [])

                # Same idea as experiment logic: success if vulnerability count is reduced, best when fixed_count == 0.
                if fixed_count == 0 and original_count > 0:
                    is_valid = True
                    status = VulnerabilityStatus.FIXED
                elif fixed_count < original_count:
                    is_valid = True
                    status = VulnerabilityStatus.PARTIALLY_FIXED
                elif fixed_count == 0 and original_count == 0:
                    # Query did not reproduce on original code; keep non-failing result.
                    is_valid = True
                    status = VulnerabilityStatus.FIXED
                    warnings.append(
                        "CodeQL query returned 0 findings on original and fixed code; result may be inconclusive."
                    )
                else:
                    is_valid = False
                    status = VulnerabilityStatus.NOT_FIXED
        elif self.method in ['both', 'codeql']:
            # Expected CodeQL but it did not run.
            is_valid = False
            status = VulnerabilityStatus.NOT_FIXED
            warnings.append("CodeQL validation did not run.")

        # Check compilation result after CodeQL decision
        if not is_valid and results['compilation']:
            comp_result = results['compilation']
            if not comp_result.get('compiles', False):
                status = VulnerabilityStatus.COMPILATION_FAILED
                compilation_errors = comp_result.get('errors', [])
            else:
                status = VulnerabilityStatus.COMPILABLE_NOT_FIXED
                warnings.append("Code compiles but vulnerabilities remain.")
        
        # Check structure integrity (tree-sitter comparison)
        # This check overrides CodeQL success if function calls are missing
        if results.get('structure_check'):
            structure_result = results['structure_check']
            if structure_result.get('has_missing_function_calls', False):
                # Even if CodeQL says it's fixed, if function calls are missing, it's a deletion fix
                is_valid = False
                status = VulnerabilityStatus.DELETED_ERROR_CODE
                missing_calls = structure_result.get('missing_function_calls', [])
                warnings.append(
                    f"Structure check detected missing function calls: {', '.join(missing_calls[:5])}"
                    + (f" and {len(missing_calls) - 5} more" if len(missing_calls) > 5 else "")
                )
        
        # Determine message
        message = self._generate_validation_message(
            status, vulnerabilities, compilation_errors
        )
        
        return ValidationResult(
            is_valid=is_valid,
            status=status,
            message=message,
            vulnerabilities_found=vulnerabilities,
            compilation_errors=compilation_errors,
            warnings=warnings,
            metadata={
                'validation_method': self.method,
                'strict_mode': self.strict_mode,
                'original_vulnerabilities': original_vulnerabilities,
                'fixed_vulnerabilities': fixed_vulnerabilities,
                'fixed_line_contents': fixed_line_contents,
                'project_root_dir': project_root_dir,
                'target_file_path': target_file_path
            }
        )
    
    def _generate_validation_message(self, status: VulnerabilityStatus,
                                    vulnerabilities: List,
                                    errors: List) -> str:
        """Generate human-readable validation message"""
        messages = {
            VulnerabilityStatus.FIXED: "Code fix appears to be successful",
            VulnerabilityStatus.PARTIALLY_FIXED: f"Code fix partially successful ({len(vulnerabilities)} vulnerabilities remain)",
            VulnerabilityStatus.NOT_FIXED: f"Code fix failed ({len(vulnerabilities)} vulnerabilities remain)",
            VulnerabilityStatus.COMPILABLE_NOT_FIXED: f"Code fix failed but compiles ({len(vulnerabilities)} vulnerabilities remain)",
            VulnerabilityStatus.COMPILATION_FAILED: f"Code does not compile ({len(errors)} errors)",
            VulnerabilityStatus.DELETED_ERROR_CODE: "LLM fixed by deleting error code (function calls missing)",
            VulnerabilityStatus.UNKNOWN: "Unknown validation status"
        }
        return messages.get(status, "Unknown validation status")


class ValidationFeedbackGenerator:
    """
    Generates detailed feedback for failed repairs,
    identifying root causes and suggesting improvements.
    """
    
    def __init__(self):
        pass
    
    def generate_feedback(self, original_code: str, failed_fix: str,
                         validation_result: ValidationResult) -> Dict:
        """
        Generate detailed feedback for a failed repair attempt
        
        Args:
            original_code: Original vulnerable code
            failed_fix: Failed fix attempt
            validation_result: Result from validation
            
        Returns:
            Dictionary with feedback information
        """
        original_rows = validation_result.metadata.get('original_vulnerabilities', [])
        fixed_rows = validation_result.metadata.get('fixed_vulnerabilities', [])
        fixed_line_contents = validation_result.metadata.get('fixed_line_contents', {})
        project_root_dir = validation_result.metadata.get('project_root_dir')
        target_file_path = validation_result.metadata.get('target_file_path')
        target_basename = os.path.basename(target_file_path) if target_file_path else ""

        if target_basename:
            original_rows = [
                row for row in original_rows
                if len(row) > 4 and os.path.basename(row[4]) == target_basename
            ]
            fixed_rows = [
                row for row in fixed_rows
                if len(row) > 4 and os.path.basename(row[4]) == target_basename
            ]

        original_set = {tuple(row) for row in original_rows}
        new_issues = [row for row in fixed_rows if tuple(row) not in original_set]

        failure_lines = []
        if new_issues:
            for row in new_issues:
                file_path = row[4] if len(row) > 5 else ""
                line_str = row[5] if len(row) > 6 else ""
                func_name = self._find_function_at_line(file_path, line_str, project_root_dir)

                line_key = f"{file_path}:{line_str}"
                row_basename = os.path.basename(file_path) if file_path else ""
                if target_basename and row_basename == target_basename:
                    line_content = fixed_line_contents.get(line_key, "")
                else:
                    line_content = self._get_line_content(file_path, line_str, project_root_dir)
                failure_lines.append(
                    f"file: {file_path} line: {line_str} function: {func_name or 'Unknown'} code: {line_content}"
                )

        return {
            'failure_reason': "\n".join(failure_lines)
        }

    def _find_function_at_line(self, file_path: str, line_str: str,
                               project_root_dir: Optional[str]) -> Optional[str]:
        line_num = int(line_str)


        resolved_path = file_path
        if project_root_dir:
            resolved_path = os.path.join(project_root_dir, file_path.lstrip("/\\"))

        if not os.path.exists(resolved_path):
            return None

        with open(resolved_path, "r", encoding="utf-8", errors="ignore") as f:
            lines = f.readlines()

        func_regex = re.compile(
            r'^\s*(?:public|private|protected|internal|static|virtual|override|async|sealed|new|extern|unsafe|partial|\s)+'
            r'\s+[\w<>\[\],\s]+\s+([A-Za-z_]\w*)\s*\('
        )

        functions = []
        i = 0
        while i < len(lines):
            line = lines[i]
            match = func_regex.search(line)
            if not match:
                i += 1
                continue
            name = match.group(1)
            brace_count = line.count("{") - line.count("}")
            j = i
            if brace_count == 0:
                while j + 1 < len(lines):
                    j += 1
                    brace_count += lines[j].count("{") - lines[j].count("}")
                    if "{" in lines[j]:
                        break
            if brace_count <= 0:
                i += 1
                continue
            while j + 1 < len(lines) and brace_count > 0:
                j += 1
                brace_count += lines[j].count("{") - lines[j].count("}")
            functions.append((name, i + 1, j + 1))
            i = j + 1

        for name, start, end in functions:
            if start <= line_num <= end:
                return name
        return None

    def _get_line_content(self, file_path: str, line_str: str,
                          project_root_dir: Optional[str]) -> str:
        line_num = int(line_str)

        resolved_path = file_path
        if project_root_dir:
            resolved_path = os.path.join(project_root_dir, file_path.lstrip("/\\"))
        print(resolved_path)
        print(line_num)

        with open(resolved_path, "r", encoding="utf-8", errors="ignore") as f:
            lines = f.readlines()

        if 1 <= line_num <= len(lines):
            return lines[line_num - 1].rstrip()
       
        return ""
    
  
  
