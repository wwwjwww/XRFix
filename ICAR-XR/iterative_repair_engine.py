"""
Iterative Repair Engine Module

Core orchestration engine that implements the three-stage ICAR-XR framework:
1. Initial Repair & Verification
2. XR-Specific Semantic Context Enhancement
3. Iterative Re-Repair

Manages the closed-loop feedback process for progressive repair refinement.
"""

import json
import logging
import re
import os
from typing import Dict, List, Optional, Tuple, Any
from dataclasses import dataclass, asdict
from enum import Enum
import time

import config_new as config

from context_extractor import (
    MultiContextManager, LifecycleContextExtractor,
    SceneGraphContextExtractor, AsyncContextExtractor,
    CallChainContextExtractor, PerformanceContextExtractor
)
from validator import CodeValidator, ValidationFeedbackGenerator, ValidationResult
from validator import VulnerabilityStatus
from xr_semantic_analyzer import XRSemanticAnalyzer


from icar_llm_responses import generate_LLM_experiment_responses
EXPERIMENT_GPT_AVAILABLE = True

class RepairStage(Enum):
    """Repair process stages"""
    INITIAL_REPAIR = "initial_repair"
    CONTEXT_ENHANCEMENT = "context_enhancement"
    ITERATIVE_REPAIR = "iterative_repair"
    FINAL_VALIDATION = "final_validation"


@dataclass
class RepairAttempt:
    """Records a single repair attempt"""
    iteration: int
    stage: RepairStage
    patch: str
    context_used: Dict[str, str]
    validation_result: Optional[ValidationResult]
    timestamp: float
    llm_model: str


@dataclass
class RepairHistory:
    """Complete repair history for an issue"""
    issue_id: str
    original_code: str
    error_type: str
    attempts: List[RepairAttempt]
    final_patch: Optional[str]
    final_status: str
    total_iterations: int
    total_time: float


class IterativeRepairEngine:
    """
    Core engine implementing the ICAR-XR three-stage repair framework.
    
    Orchestrates:
    1. Initial one-shot repair
    2. Context extraction and enhancement
    3. Iterative re-repair with feedback
    """
    
    def __init__(self, llm_interface, codeql_interface=None, config_dict: Dict = None):
        """
        Initialize the repair engine
        
        Args:
            llm_interface: Interface to LLM for code generation
            codeql_interface: Interface to CodeQL for validation
            config_dict: Configuration dictionary
        """
        self.llm = llm_interface
        self.codeql = codeql_interface
        self.config = config_dict or {}
        
        # Initialize components
        self.semantic_analyzer = XRSemanticAnalyzer()
        self.context_manager = MultiContextManager(
            max_tokens=self.config.get('max_context_tokens', 4000)
        )
        self.validator = CodeValidator(
            config=self.config.get('validation_strategy', {})
        )
        self.feedback_generator = ValidationFeedbackGenerator()
        
        # Logging setup
        self.logger = self._setup_logging()
        
        # History tracking
        self.repair_histories: Dict[str, RepairHistory] = {}

    def _write_multicontext_log(self, formatted_ctx: str,
                                error_location: Optional[Dict],
                                iteration: int) -> None:
        storage_cfg = getattr(config, 'CONTEXT_STORAGE', {}) or {}
        if not storage_cfg.get('enabled', False):
            return

        base_dir = storage_cfg.get('base_directory', './icar_xr_contexts')
        os.makedirs(base_dir, exist_ok=True)

        file_path = error_location.get('file_path', '') if error_location else ''
        line_no = error_location.get('line', '') if error_location else ''
        file_base = os.path.basename(file_path) if file_path else 'unknown'
        safe_file_base = re.sub(r'[^A-Za-z0-9_.-]+', '_', file_base)
        ts_s = int(time.time())
        ts_ns = time.time_ns()

        # Use nanosecond timestamp to avoid collisions within the same second.
        log_name = f"multicontext_{safe_file_base}_{line_no}_iter{iteration}_{ts_ns}.txt"
        log_path = os.path.join(base_dir, log_name)

        header = [
            f"timestamp: {ts_s}",
            f"timestamp_ns: {ts_ns}",
            f"iteration: {iteration}",
            f"file: {file_path}",
            f"line: {line_no}",
            ""
        ]

        try:
            with open(log_path, "w", encoding="utf-8") as f:
                f.write("\n".join(header))
                f.write(str(formatted_ctx))
            self.logger.debug("Multicontext written to %s", log_path)
        except Exception as exc:
            self.logger.warning("Failed to write multicontext log: %s", exc)

    def _write_stage3_prompt_log(self, prompt: str,
                                 error_location: Optional[Dict],
                                 iteration: int,
                                 llm_model: Optional[str]) -> None:
        storage_cfg = getattr(config, 'CONTEXT_STORAGE', {}) or {}
        if not storage_cfg.get('enabled', False):
            return

        base_dir = storage_cfg.get('base_directory', './icar_xr_contexts')
        os.makedirs(base_dir, exist_ok=True)

        file_path = error_location.get('file_path', '') if error_location else ''
        line_no = error_location.get('line', '') if error_location else ''
        file_base = os.path.basename(file_path) if file_path else 'unknown'
        safe_file_base = re.sub(r'[^A-Za-z0-9_.-]+', '_', file_base)
        ts_s = int(time.time())
        ts_ns = time.time_ns()
        model_tag = re.sub(r'[^A-Za-z0-9_.-]+', '_', llm_model) if llm_model else 'unknown'

        # Use nanosecond timestamp to avoid collisions within the same second.
        log_name = f"stage3_prompt_{safe_file_base}_{line_no}_iter{iteration}_{model_tag}_{ts_ns}.txt"
        log_path = os.path.join(base_dir, log_name)

        header = [
            f"timestamp: {ts_s}",
            f"timestamp_ns: {ts_ns}",
            f"iteration: {iteration}",
            f"model: {llm_model}",
            f"file: {file_path}",
            f"line: {line_no}",
            ""
        ]

        try:
            with open(log_path, "w", encoding="utf-8") as f:
                f.write("\n".join(header))
                f.write(str(prompt))
            self.logger.debug("Stage 3 prompt written to %s", log_path)
        except Exception as exc:
            self.logger.warning("Failed to write Stage 3 prompt log: %s", exc)
    
    def _setup_logging(self) -> logging.Logger:
        """Setup logging"""
        logger = logging.getLogger('ICAR-XR')
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
    
    def _build_experiment_prompt(self, experiment_dir: str, validation_context: Dict) -> Tuple[str, str, str]:
        """
        Build prompt from experiment context, following hand_crafted_prompt_response logic.
        
        Args:
            experiment_dir: Experiment directory path
            validation_context: Validation context with scenario data
            
        Returns:
            Tuple of (instruct_head, prompt_contents, short_contents)
        """
        file_extension = '.cs'
        comment_key = "//"
        
        # Load scenario.json if available
        scenario_path = os.path.join(experiment_dir, 'scenario.json')
        if not os.path.exists(scenario_path):
            self.logger.warning(f"scenario.json not found at {scenario_path}, using fallback prompt")
            return "", "", ""
        

        with open(scenario_path, 'r', encoding='utf8') as f:
            scenario_contents = json.load(f)

        # Get file names
        err_info = scenario_contents.get("err_detailed_info", {}) or {}
        file_name = err_info["file_name"]

        include_addition = bool(scenario_contents.get("include_addition", False))
        # Some scenarios do not have an additional file. Treat as same-file mode.
        add_file_name = err_info.get("add_file_name") or file_name
        experiment_file = file_name.split('/')[-1] if '/' in file_name else file_name

        # If include_addition is disabled, follow the same prompt preparation logic as:
        # experiment_gen_response_GPT.decide_include_addition -> experiment_gen_response.prepare_LLM_experiment_requests
        if not include_addition:
            head_contents_file_name = experiment_file + ".head" + file_extension
            head_contents_path = os.path.join(experiment_dir, head_contents_file_name)

            prompt_contents_file_name = experiment_file + ".prompt" + file_extension
            prompt_contents_path = os.path.join(experiment_dir, prompt_contents_file_name)

            short_prompt_contents_file_name = experiment_file + ".short" + file_extension
            short_prompt_contents_path = os.path.join(experiment_dir, short_prompt_contents_file_name)

            instruct_head = ""
            if os.path.exists(head_contents_path):
                with open(head_contents_path, 'r', encoding='utf8') as f:
                    instruct_head = f.read()

            prompt_contents = ""
            if os.path.exists(prompt_contents_path):
                with open(prompt_contents_path, 'r', encoding='utf8') as f:
                    prompt_contents = f.read()
            prompt_contents = re.sub(r'/\*\*.*?\*\*/', '', prompt_contents, flags=re.DOTALL)

            short_contents = ""
            if os.path.exists(short_prompt_contents_path):
                with open(short_prompt_contents_path, 'r', encoding='utf8') as f:
                    short_contents = f.read()
            short_contents = re.sub(r'/\*\*.*?\*\*/', '', short_contents, flags=re.DOTALL)

            return instruct_head, prompt_contents, short_contents
        
        # Build file paths
        head_contents_file_name = experiment_file + ".head" + file_extension
        head_contents_path = os.path.join(experiment_dir, head_contents_file_name)
        
        prepend_contents_file_name = experiment_file + ".prepend" + file_extension
        prepend_contents_path = os.path.join(experiment_dir, prepend_contents_file_name)
        
        prepend_contents_file_name_add = add_file_name.split('/')[-1] + ".prepend_add" + file_extension
        prepend_contents_path_add = os.path.join(experiment_dir, prepend_contents_file_name_add)
        
        prompt_contents_file_name = experiment_file + ".prompt" + file_extension
        prompt_contents_path = os.path.join(experiment_dir, prompt_contents_file_name)
        
        prompt_contents_file_name_add = add_file_name.split('/')[-1] + ".prompt_add" + file_extension
        prompt_contents_path_add = os.path.join(experiment_dir, prompt_contents_file_name_add)
        
        prompt_between_lines_file_name = experiment_file + ".between" + file_extension
        prompt_between_lines_path = os.path.join(experiment_dir, prompt_between_lines_file_name)
        
        prompt_add_first_file_name = experiment_file + ".add_first.txt"
        prompt_add_first_path = os.path.join(experiment_dir, prompt_add_first_file_name)
        
        prompt_short_lines_file_name = experiment_file + ".short" + file_extension
        prompt_short_lines_path = os.path.join(experiment_dir, prompt_short_lines_file_name)
        
        add_prompt_short_lines_file_name = add_file_name.split('/')[-1] + ".short" + file_extension
        add_prompt_short_lines_path = os.path.join(experiment_dir, add_prompt_short_lines_file_name)
        
        
        prompt = scenario_contents.get("prompt_template", "")

        
        # Load file contents
        prepend_contents = ""
        prompt_contents = ""
        prepend_contents_add = ""
        prompt_contents_add = ""
        prompt_short_lines = ""
        prompt_between_lines = ""
        add_first_str = ""
        add_prompt_short_lines = ""
        
        if os.path.exists(prepend_contents_path):
            with open(prepend_contents_path, 'r', encoding='utf8') as f:
                prepend_contents = f.read()
        if os.path.exists(prompt_contents_path):
            with open(prompt_contents_path, 'r', encoding='utf8') as f:
                prompt_contents = f.read()
        if os.path.exists(prepend_contents_path_add):
            with open(prepend_contents_path_add, 'r', encoding='utf8') as f:
                prepend_contents_add = f.read()
        if os.path.exists(prompt_contents_path_add):
            with open(prompt_contents_path_add, 'r', encoding='utf8') as f:
                prompt_contents_add = f.read()
        if os.path.exists(prompt_short_lines_path):
            with open(prompt_short_lines_path, 'r', encoding='utf8') as f:
                prompt_short_lines = f.read()
        if os.path.exists(prompt_between_lines_path):
            with open(prompt_between_lines_path, 'r', encoding='utf8') as f:
                prompt_between_lines = f.read()
        if os.path.exists(prompt_add_first_path):
            with open(prompt_add_first_path, 'r', encoding='utf8') as f:
                add_first_str = f.read()
        if os.path.exists(add_prompt_short_lines_path):
            with open(add_prompt_short_lines_path, 'r', encoding='utf8') as f:
                add_prompt_short_lines = f.read()
        
        # Build prompt based on file_name == add_file_name
        short_contents = ""
        if file_name == add_file_name:
            prompt_head = comment_key + "Here're the buggy code lines from " + file_name + ":\n"
            if "fix_instruction_add_prompt_assymetrical" in prompt:
                prompt_head = "/* Here're the buggy code lines from " + file_name + ":*/\n"
            
            if "False" in add_first_str:
                prompt_lines_long = prompt_head + prepend_contents + prompt_contents + prompt_between_lines + prompt_contents_add
                prompt_lines_short = prompt_head + prompt_short_lines + prompt_contents + prompt_contents_add
            else:
                prompt_lines_long = prompt_head + prepend_contents + prompt_contents_add + prompt_between_lines + prompt_contents
                prompt_lines_short = prompt_head + prompt_short_lines + prompt_contents_add + prompt_contents
            
            prompt_contents = prompt_lines_long
            short_contents = prompt_lines_short
        else:
            prompt_head = comment_key + "Here're the buggy code lines from " + file_name + ":\n"
            prompt_mid = comment_key + "Here's the definition of function call in another component.\n" + \
                         comment_key + "Related code from " + add_file_name + ":\n"
            
            prompt_lines = prompt_head + prepend_contents + prompt_contents + prompt_mid + prepend_contents_add + prompt_contents_add
            short_contents = prompt_head + prompt_short_lines + prompt_contents + prompt_mid + add_prompt_short_lines + prompt_contents_add
            prompt_contents = prompt_lines
        
        # Load instruct_head
        instruct_head = ""
        if os.path.exists(head_contents_path):
            with open(head_contents_path, 'r', encoding='utf8') as f:
                instruct_head = f.read()
        
        return instruct_head, prompt_contents, short_contents
    
    def _extract_code_from_llm_response(self, response: Any) -> str:
        """
        Extract and clean code from LLM response.
        
        Handles GPT-4 response structure:
        - response format: {'content': (200, '{"choices":[{"message":{"content":"..."}}]}'), ...}
        - Extract content from choices[0]['message']['content']
        - Content may contain markdown code blocks (```csharp...``` or ```...```)
        - Also handles triple-quoted strings ('''...''')
        
        Args:
            response: LLM response dict with format {'content': (200, json_string), ...}
            
        Returns:
            Extracted and cleaned code string
        """
        # Handle response format: {'content': (200, json_string), ...}
        choice_txt = None
        response_json = None
        
        # Extract content from response dict
        if isinstance(response, dict) and 'content' in response:
            content_value = response['content']
            # content is a tuple (status_code, json_string)
            if isinstance(content_value, tuple) and len(content_value) == 2:
                try:
                    response_json = json.loads(content_value[1])
                except (json.JSONDecodeError, TypeError):
                    return "NO USEFUL OUTPUT"
        
        # Extract choice text from choices[0]['message']['content']
        if response_json and isinstance(response_json, dict):
            if 'choices' in response_json and len(response_json['choices']) > 0:
                choice = response_json['choices'][0]
                if 'message' in choice and 'content' in choice['message']:
                    choice_txt = choice['message']['content']
        
        if not choice_txt:
            return "NO USEFUL OUTPUT"
        
        # Extract code from markdown code blocks (```...```)
        p1 = re.compile(r'```(.*?)```', re.S)
        content = re.findall(p1, choice_txt)
        new_choice_text = ""
        
        if content != []:
            # Concatenate all code blocks found
            for txt in content:
                new_choice_text += txt
            choice_txt = new_choice_text
        else:
            # Try splitting by ```
            content = choice_txt.split("```")
            if len(content) > 1:
                choice_txt = content[1]
            else:
                # Try triple-quoted strings ('''...''')
                p2 = re.compile(r"'''(.*?)'''", re.S)
                content = re.findall(p2, choice_txt)
                new_choice_text = ""
                if content != []:
                    for txt in content:
                        new_choice_text += txt
                    choice_txt = new_choice_text
                else:
                    # Try splitting by '''
                    content = choice_txt.split("'''")
                    if len(content) > 1:
                        choice_txt = content[1]
        
        # Clean up code block markers
        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#', "", choice_txt, re.DOTALL)
        # Remove leading whitespace
        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)
        
        if choice_txt_clean == "":
            choice_txt_clean = "NO USEFUL OUTPUT"
        
        return choice_txt_clean
    
    def repair(self, code_snippet: str, error_type: str, 
              error_location: Dict, llm_model: str = "gpt-4",
              validation_context: Optional[Dict] = None) -> Dict:
        """
        Execute the complete ICAR-XR repair pipeline
        
        Args:
            code_snippet: Source code containing the vulnerability
            error_type: Type of error (CWE or Unity-specific)
            error_location: Location info (file, line, column)
            llm_model: LLM model to use for repair
            
        Returns:
            Dictionary with repair results
        """
        start_time = time.time()
        issue_id = f"{error_location.get('file', 'unknown')}_{error_location.get('line', 0)}"
        
        self.logger.info(f"ICAR-XR: {issue_id} | {error_type}")

        # Use provided validation_context or create default
        if validation_context is None:
            validation_context = {}
        
        # Merge with default values
        default_context = {
            'project_root_dir': self.config.get('project_root_dir'),
            'baseline_db_path': self.config.get('baseline_db_path'),
            'target_file': error_location.get('file') if error_location else None,
            'target_file_path': error_location.get('file_path') if error_location else None,
            'file_encoding': self.config.get('file_encoding', 'utf-8')
        }
        validation_context = {**default_context, **validation_context}
        
        # Initialize repair history
        history = RepairHistory(
            issue_id=issue_id,
            original_code=code_snippet,
            error_type=error_type,
            attempts=[],
            final_patch=None,
            final_status='pending',
            total_iterations=0,
            total_time=0.0
        )
        
        # STAGE 1: Initial Repair & Verification (Generate Multiple Patches)
        self.logger.info("Stage 1: Initial Repair")
        
        # Get number of patches to generate (default from config or scenario)
        num_patches = self.config.get('num_initial_patches', 1)
        
        patches_v1 = self._stage_1_initial_repair(
            code_snippet, error_type, error_location, llm_model,
            validation_context, num_patches
        )
        
        # Prepare merge context if available
        merge_strategy = None
        merge_context = None
        if validation_context:
            merge_strategy = validation_context.get('combine_settings')
            if merge_strategy:
                merge_context = {
                    'comment_key': '//',
                    'prepend_contents': validation_context.get('prepend_contents', ''),
                    'append_contents': validation_context.get('append_contents', ''),
                    'whitespace': validation_context.get('whitespace', ''),
                    'add_contents': validation_context.get('add_contents', ''),
                    'add_whitespace': validation_context.get('add_whitespace', ''),
                    'between_contents': validation_context.get('between_contents', ''),
                    'add_first': validation_context.get('add_first', ''),
                    'add_append_contents': validation_context.get('add_append_contents', ''),
                    'add_prepend_contents': validation_context.get('add_prepend_contents', ''),
                    'include_addition': validation_context.get('include_addition', False),
                    'is_function': validation_context.get('is_function'),
                    'target_file': validation_context.get('target_file', ''),
                    'add_file': validation_context.get('add_file', '')
                }
        
        # Process each patch through the repair pipeline
        best_patch = None
        best_result = None
        best_success = False
        stop_on_first_success = self.config.get('stop_on_first_success', True)
        max_iterations = self.config.get('max_repair_iterations', 4)
        max_stage1_retries = int(self.config.get('max_stage1_retries', 3))
        stage1_retry_on_statuses = {
            VulnerabilityStatus.NOT_FIXED,
            VulnerabilityStatus.COMPILATION_FAILED,
            VulnerabilityStatus.DELETED_ERROR_CODE
        }
        
        self.logger.info(f"Processing {len(patches_v1)} patch(es)...")
        
        for patch_idx, patch_v1 in enumerate(patches_v1, 1):
            self.logger.info(f"Patch {patch_idx}/{len(patches_v1)}")
            patch_succeeded = False
            
            stage1_retry = 0
            patch_v1_current = patch_v1

            while True:
                # Record Stage 1 attempt (including retries)
                attempt_1 = RepairAttempt(
                    iteration=1,
                    stage=RepairStage.INITIAL_REPAIR,
                    patch=patch_v1_current,
                    context_used={'stage1_retry': str(stage1_retry)} if stage1_retry else {},
                    validation_result=None,
                    timestamp=time.time(),
                    llm_model=llm_model
                )

                # Validate initial repair
                validation_context_with_patch = dict(validation_context or {})
                validation_context_with_patch.update({
                    'patch_index': patch_idx,
                    'iteration': 1,
                    'llm_model': llm_model,
                    'stage1_retry': stage1_retry
                })
                validation_result_1 = self.validator.validate(
                    code_snippet, patch_v1_current, error_type,
                    codeql_query=validation_context_with_patch.get('codeql_query') if validation_context_with_patch else None,
                    validation_context=validation_context_with_patch,
                    merge_strategy=merge_strategy,
                    merge_context=merge_context
                )
                attempt_1.validation_result = validation_result_1
                history.attempts.append(attempt_1)

                # If initial repair succeeds, record and continue to check other patches
                if validation_result_1.is_valid:
                    self.logger.info(f"Patch {patch_idx}: FIXED (Stage 1)")
                    if best_patch is None or not best_success:
                        best_patch = patch_v1_current
                        best_result = validation_result_1
                        best_success = True
                        history.final_patch = patch_v1_current
                        status_suffix = f"_retry_{stage1_retry}" if stage1_retry else ""
                        history.final_status = f'success_stage_1_patch_{patch_idx}{status_suffix}'
                        history.total_iterations = 1
                    patch_succeeded = True
                    break

                # If Stage 1 failed with certain statuses, repeat Stage 1 (request new LLM patch)
                if (
                    getattr(validation_result_1, 'status', None) in stage1_retry_on_statuses
                    and stage1_retry < max_stage1_retries
                ):
                    stage1_retry += 1
                    self.logger.info(
                        f"Stage 1 retry {stage1_retry}/{max_stage1_retries} due to status={getattr(validation_result_1, 'status', None)}"
                    )
                    new_patches = self._stage_1_initial_repair(
                        code_snippet, error_type, error_location, llm_model,
                        validation_context, 1
                    )
                    if not new_patches:
                        self.logger.warning("Stage 1 retry requested but no patch was generated; stopping retries.")
                        break
                    patch_v1_current = new_patches[0]
                    continue

                # Stop retry loop: either not in retry statuses, or retry budget exhausted
                break
            
            if patch_succeeded:
                if stop_on_first_success:
                    break
                continue

            # If Stage 1 ended with a retry-status failure, do NOT enter Stage 2; move to next Stage 1 patch.
            if getattr(validation_result_1, 'status', None) in stage1_retry_on_statuses:
                self.logger.info(
                    f"Stage 1 ended with status={getattr(validation_result_1, 'status', None)}; skipping Stage 2 and moving to next patch."
                )
                continue
            
            # STAGE 2: XR-Specific Semantic Context Enhancement
            self.logger.info("Stage 2: Context Enhancement")
            enhanced_contexts = self._stage_2_context_enhancement(
                code_snippet, error_location, validation_result_1, error_type, validation_context,
                llm_model
            )
            
            # STAGE 3: Iterative Re-Repair
            patch_v_final = patch_v1_current
            
            for iteration in range(2, max_iterations + 1):
                
                self.logger.info(f"Stage 3: Iterative Repair (Iteration {iteration})")
                
                # Generate repair with enhanced context
                patch_v_i = self._stage_3_iterative_repair(
                    code_snippet, error_type, patch_v1,
                    validation_result_1, enhanced_contexts,
                    iteration, llm_model
                )
                
                # Record attempt
                attempt_i = RepairAttempt(
                    iteration=iteration,
                    stage=RepairStage.ITERATIVE_REPAIR,
                    patch=patch_v_i,
                    context_used=list(enhanced_contexts.keys()),
                    validation_result=None,
                    timestamp=time.time(),
                    llm_model=llm_model
                )
                
                # Validate iterative repair
                validation_context_with_patch = dict(validation_context or {})
                validation_context_with_patch.update({
                    'patch_index': patch_idx,
                    'iteration': iteration,
                    'llm_model': llm_model
                })
                validation_result_i = self.validator.validate(
                    code_snippet, patch_v_i, error_type,
                    codeql_query=validation_context_with_patch.get('codeql_query') if validation_context_with_patch else None,
                    validation_context=validation_context_with_patch,
                    merge_strategy=merge_strategy,
                    merge_context=merge_context
                )
                attempt_i.validation_result = validation_result_i
                history.attempts.append(attempt_i)
                
                # Check success
                if validation_result_i.is_valid:
                    self.logger.info(f"Patch {patch_idx}: FIXED (Iteration {iteration})")
                    patch_v_final = patch_v_i
                    if best_patch is None or not best_success:
                        best_patch = patch_v_final
                        best_result = validation_result_i
                        best_success = True
                        history.final_patch = patch_v_final
                        history.final_status = f'success_iteration_{iteration}_patch_{patch_idx}'
                        history.total_iterations = iteration
                    patch_succeeded = True
                    break
            
            if patch_succeeded and stop_on_first_success:
                break

            # Update best patch if this one is better
            if not best_success and (best_patch is None or patch_v_final != patch_v1):
                best_patch = patch_v_final
        
        # Set final status if no patch succeeded
        if history.final_patch is None:
            history.final_patch = best_patch if best_patch else (patches_v1[0] if patches_v1 else None)
            history.final_status = 'failed_all_iterations'
            history.total_iterations = max_iterations
        
        history.total_time = time.time() - start_time
        
        # Always update the history dictionary to ensure latest run is recorded
        self.repair_histories[issue_id] = history
        status_prefix = "SUCCESS" if history.final_status.startswith('success') else "FAIL"
        self.logger.info(f"{status_prefix} {issue_id}: {history.final_status} | {history.total_iterations} iterations | {history.total_time:.1f}s")
        
        # Return results
        success = history.final_status.startswith('success')
        return self._format_repair_result(history, success)
    
    def _stage_1_initial_repair(self, code_snippet: str, error_type: str,
                               error_location: Dict, llm_model: str,
                               validation_context: Optional[Dict] = None,
                               num_patches: int = 1) -> List[str]:
        """
        STAGE 1: Generate initial repair patches (multiple patches)
        
        Args:
            code_snippet: Source code
            error_type: Error type
            error_location: Error location
            llm_model: LLM model to use
            validation_context: Validation context with experiment_dir
            num_patches: Number of patches to generate
            
        Returns:
            List of patches
        """
        patches = []
        
        # Check if we have experiment_dir and can use experiment prompt building
        experiment_dir = validation_context.get('experiment_dir') if validation_context else None

        max_tokens = config.CONTEXT_WINDOWS.get(llm_model, 4096)
        
        if experiment_dir and EXPERIMENT_GPT_AVAILABLE and os.path.exists(experiment_dir):
            instruct_head, prompt_contents, short_contents = self._build_experiment_prompt(
                experiment_dir, validation_context
            )
            
            if prompt_contents:
                experiment_file = validation_context.get('experiment_file', 'unknown')
                if '/' in experiment_file:
                    experiment_file = experiment_file.split('/')[-1]
                
                scenario_path = os.path.join(experiment_dir, 'scenario.json')
                iteration = num_patches
                
                if os.path.exists(scenario_path):
                    try:
                        with open(scenario_path, 'r', encoding='utf8') as f:
                            scenario = json.load(f)
                            temperature = scenario.get('temperature', 0.7)
                            top_p = scenario.get('top_p', 1.0)
                    except Exception:
                        pass
                
                response_paths = generate_LLM_experiment_responses(
                    experiment_dir,
                    instruct_head,
                    prompt_contents,
                    short_contents,
                    experiment_file,
                    temperature,
                    top_p,
                    [llm_model],
                    iteration,
                    skip_engines=[],
                    max_tokens=max_tokens,
                    unique_tag=f"stage1_p{validation_context.get('patch_index')}_r{validation_context.get('stage1_retry')}_i{validation_context.get('iteration')}",
                    llm_interface=self.llm,
                )

                if response_paths:
                    response_file_path = response_paths[-1]
                    with open(response_file_path, 'r', encoding='utf8') as f:
                        response_data = json.loads(f.read())

                    for idx, choice in enumerate(response_data.get('choices', [])):
                        choice_txt = choice.get('message', {}).get('content', '')

                        p1 = re.compile(r'```(.*?)```', re.S)
                        content = re.findall(p1, choice_txt)
                        if content:
                            choice_txt = ''.join(content)

                        choice_txt_clean = re.sub(r'```csharp\n|```C#|```|"""\n|"""|csharp|C#|c#', "",
                                                  choice_txt, re.DOTALL)
                        choice_txt_clean = re.sub(r'^\s+', '', choice_txt_clean, re.DOTALL)

                        if choice_txt_clean and choice_txt_clean != "":
                            patches.append(choice_txt_clean)
                            pass
        
        if not patches:
            self.logger.error("No patches generated - repair cannot proceed")

        self.logger.info(f"Generated {len(patches)} patch(es)")
        return patches

    def _scenario_has_nonempty_add_file_name(
        self, validation_context: Optional[Dict]
    ) -> Optional[bool]:
        """
        If experiment_dir/scenario.json is readable, returns whether err_detailed_info
        contains a non-empty add_file_name string.
        Returns None when scenario cannot be loaded (caller keeps config defaults).
        """
        if not validation_context:
            return None
        experiment_dir = validation_context.get('experiment_dir')
        if not experiment_dir:
            return None
        scenario_path = os.path.join(experiment_dir, 'scenario.json')
        if not os.path.isfile(scenario_path):
            return None
        try:
            with open(scenario_path, 'r', encoding='utf8') as f:
                scenario = json.load(f)
        except Exception:
            return None
        info = scenario.get('err_detailed_info')
        if not isinstance(info, dict):
            return False
        name = info.get('add_file_name')
        if not isinstance(name, str) or not name.strip():
            return False
        return True
    
    def _stage_2_context_enhancement(self, code_snippet: str,
                                    error_location: Dict,
                                    initial_validation: ValidationResult,
                                    error_type: str,
                                    validation_context: Optional[Dict],
                                    llm_model: Optional[str] = None) -> Dict[str, Any]:
        """
        STAGE 2: Extract and build XR-specific semantic context
        
        Extracts on-demand:
        - Lifecycle context (class code with function annotations)
        - Scene graph context (GameObject hierarchy implications)
        - Async context (coroutine and timing info)
        - Call chain context (caller/callee relationships)
        - Performance context (optimization opportunities)
        
        Args:
            code_snippet: Source code
            error_location: Error location
            initial_validation: Results from initial repair validation
            
        Returns:
            Dictionary of extracted contexts
        """
        # Prepare metadata for context extraction
        metadata = {
            'error_location': error_location,
            'error_type': initial_validation.status.value,
            'vulnerable_function': error_location.get('function', 'Update'),
            'call_chain_depth': 2
        }
        if llm_model:
            metadata['llm_model'] = llm_model
        if validation_context:
            metadata.update({
                'baseline_db_path': validation_context.get('baseline_db_path'),
                'project_root_dir': validation_context.get('project_root_dir'),
                'experiment_dir': validation_context.get('experiment_dir'),
                'exp_dir': validation_context.get('exp_dir'),
                'codeql_binary': validation_context.get('codeql_binary', self.validator.codeql_binary),
                'codeql_search_path': validation_context.get('codeql_search_path')
            })
        
        # Extract contexts based on configured selection for this error type
        selection_map = getattr(config, 'CONTEXT_SELECTION_BY_ERROR', {})
        selection = selection_map.get(error_type)
        print("metadata", metadata)
        if selection:
            has_add_file = self._scenario_has_nonempty_add_file_name(validation_context)
            if has_add_file is False and selection.get('call_chain_context'):
                selection = dict(selection)
                selection['call_chain_context'] = False
                self.logger.info(
                    "Stage 2: scenario err_detailed_info has no add_file_name; "
                    "skipping call_chain_context despite config."
                )
            contexts = self.context_manager.extract_selected_contexts(
                code_snippet, metadata, selection
            )
        else:
            contexts = self.context_manager.extract_all_contexts(
                code_snippet, metadata
            )
        
        # Enhance with semantic analysis
        semantic_analysis = self.semantic_analyzer.analyze_code(code_snippet)
        
        # Generate feedback from validation failure
        failure_feedback = self.feedback_generator.generate_feedback(
            code_snippet, code_snippet,  # Original patch
            initial_validation
        )
        instruct_head = ""
        experiment_dir = validation_context.get('experiment_dir') if validation_context else None
        if experiment_dir and os.path.exists(experiment_dir):
            instruct_head, _, _ = self._build_experiment_prompt(
                experiment_dir, validation_context
            )
        
        return {
            'contexts': contexts,
            'semantic_analysis': semantic_analysis,
            'failure_feedback': failure_feedback,
            'formatted_contexts': self.context_manager.format_contexts_for_prompt(contexts),
            'error_location': error_location,
            'instruct_head': instruct_head
        }
    
    def _stage_3_iterative_repair(self, code_snippet: str, error_type: str,
                                 initial_patch: str, initial_validation: ValidationResult,
                                 enhanced_contexts: Dict, iteration: int,
                                 llm_model: str) -> str:
        """
        STAGE 3: Generate improved repair using enriched context
        
        Args:
            code_snippet: Original code
            error_type: Error type
            initial_patch: Previous repair attempt
            initial_validation: Validation result of previous attempt
            enhanced_contexts: Enhanced context information
            iteration: Current iteration number
            llm_model: LLM model to use
            
        Returns:
            Improved patch
        """
        # Build comprehensive prompt for iterative repair
        # Ensure all values are strings
        failure_reason = enhanced_contexts.get('failure_feedback', {}).get('failure_reason', "")
        instruct_head = enhanced_contexts.get('instruct_head', "")
        
        prompt_parts = [
            f"{instruct_head}",
            "",
            "** ORIGINAL VULNERABLE CODE **",
            "```csharp",
            str(code_snippet),
            "```",
            "",
            "** PREVIOUS REPAIR ATTEMPT **",
            "```csharp",
            str(initial_patch),
            "```",
            "",
            "** REASON WHY THE PREVIOUS REPAIR FAILED **",
            str(initial_validation.message),
            "",
            "** FAILURE REASON **",
            str(failure_reason),
            ""
        ]
        
        # Add formatted contexts (ensure it's a string)
        if 'formatted_contexts' in enhanced_contexts:
            formatted_ctx = enhanced_contexts['formatted_contexts']
            if formatted_ctx:
                self._write_multicontext_log(
                    formatted_ctx,
                    enhanced_contexts.get('error_location'),
                    iteration
                )
                prompt_parts.append(str(formatted_ctx))
        
        prompt_parts.extend([
            "",
            f"** TASK (ITERATION {iteration}) **",
            f"** INSTRUCTIONS **",
            "1. You can modify the previous patch or generate a new patch to fix the vulnerability. Please infer from the failure reasons to avoid the mistakes in the previous patch.",
            "2. Use the call chain context to understand the complete call graph of the vulnerable code.",
            "Return ONLY the patch, no explanation."
        ])
        
        prompt = '\n'.join(prompt_parts)
        
        self.logger.debug(f"Stage 3 iteration {iteration} prompt length: {len(prompt)}")
        self._write_stage3_prompt_log(
            prompt,
            enhanced_contexts.get('error_location'),
            iteration,
            llm_model
        )
        
        # Call LLM for improved repair
        max_tokens = config.CONTEXT_WINDOWS.get(llm_model, 2000)
        try:
            response = self.llm.generate(
                prompt=prompt,
                model=llm_model,
                temperature=0.5,  # Lower temperature for more focused repair
                max_tokens=max_tokens
            )
            
            # Extract code from response
            patch = self._extract_code_from_llm_response(response)
            if patch == "NO USEFUL OUTPUT":
                raise ValueError("LLM response contains no valid code")
            
        except Exception as e:
            self.logger.error(f"LLM call failed in iteration {iteration}: {e}")
            patch = initial_patch  # Fallback to previous patch
        
        return patch
    

    
    def _format_repair_result(self, history: RepairHistory, success: bool) -> Dict:
        """
        Format repair results for return
        
        Args:
            history: Complete repair history
            success: Whether repair was successful
            
        Returns:
            Formatted result dictionary
        """
        return {
            'success': success,
            'issue_id': history.issue_id,
            'error_type': history.error_type,
            'final_patch': history.final_patch,
            'iterations': history.total_iterations,
            'total_time': history.total_time,
            'status': history.final_status,
            'attempts': [
                {
                    'iteration': a.iteration,
                    'stage': a.stage.value,
                    'timestamp': a.timestamp,
                    'validation_status': a.validation_result.status.value if a.validation_result else None
                }
                for a in history.attempts
            ],
            'history': history
        }
    
    def get_repair_history(self, issue_id: str) -> Optional[RepairHistory]:
        """Get repair history for an issue"""
        return self.repair_histories.get(issue_id)
    
    def export_repair_history(self, issue_id: str, output_file: str) -> bool:
        """Export repair history to JSON file"""
        history = self.get_repair_history(issue_id)
        if not history:
            self.logger.warning(f"No repair history found for {issue_id}")
            return False
        
        self.logger.info(f"Exporting repair history for {issue_id}: status={history.final_status}, iterations={history.total_iterations}, attempts={len(history.attempts)}")
        
        # Convert to serializable format
        history_dict = {
            'issue_id': history.issue_id,
            'error_type': history.error_type,
            'final_status': history.final_status,
            'total_iterations': history.total_iterations,
            'total_time': history.total_time,
            'attempts': [
                {
                    'iteration': a.iteration,
                    'stage': a.stage.value,
                    'llm_model': a.llm_model,
                    'validation': {
                        'status': a.validation_result.status.value if a.validation_result else None,
                        'is_valid': a.validation_result.is_valid if a.validation_result else None,
                        'message': a.validation_result.message if a.validation_result else None
                    } if a.validation_result else None
                }
                for a in history.attempts
            ]
        }
        
        try:
            with open(output_file, 'w') as f:
                json.dump(history_dict, f, indent=2)
            return True
        except Exception as e:
            self.logger.error(f"Failed to export history: {e}")
            return False
