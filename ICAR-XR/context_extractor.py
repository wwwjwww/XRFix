"""
Context Extractor Module

Extracts multi-dimensional XR-specific semantic contexts from C# code
for augmenting LLM repair prompts. Handles lifecycle, scene graph, async,
and call chain context extraction.
"""

import re
import json
import os
import csv
import subprocess
import time
from textwrap import dedent
from typing import Dict, List, Optional, Tuple, Any
from dataclasses import dataclass, asdict
from abc import ABC, abstractmethod
import tiktoken
import config_new as config
import logging

from xr_semantic_analyzer import XRSemanticAnalyzer, LifecyclePhase


logger = logging.getLogger('ICAR-XR')


def count_prompt_token(max_tokens, model, prompt):
    tokenizer = tiktoken.encoding_for_model(model)
    tokens = tokenizer.encode(prompt)
    multiplier = getattr(config, 'TOKEN_COUNT_MULTIPLIERS', {}).get(model, 1.0)
    adjusted_len = int(len(tokens) * multiplier)
    if adjusted_len > max_tokens:
        print("Warning: The prompt exceeds the maximum token limit. Token length:" + str(adjusted_len))
        return False
    else:
        print("The prompt is within the token limit:" + str(adjusted_len))
        return True


@dataclass
class ExtractedContext:
    """Container for extracted context information"""
    context_type: str
    content: str
    priority: int
    token_count: int
    metadata: Dict[str, Any]


class ContextExtractor(ABC):
    """Base class for context extractors"""
    
    @abstractmethod
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """Extract context from code"""
        pass


class LifecycleContextExtractor(ContextExtractor):
    """
    Extracts lifecycle context - provides complete class code with annotations
    highlighting which lifecycle functions contain the vulnerability.
    """
    
    def __init__(self, semantic_analyzer: XRSemanticAnalyzer):
        self.semantic_analyzer = semantic_analyzer
    
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """
        Extract lifecycle context with annotations
        
        Args:
            code: Source code containing the vulnerability
            metadata: Information about vulnerability location
            
        Returns:
            ExtractedContext with lifecycle information
        """
        lifecycle_analysis = self.semantic_analyzer.analyze_lifecycle_usage(code)
        
        context_text = self._build_lifecycle_context(code, lifecycle_analysis, metadata)
        
        return ExtractedContext(
            context_type='lifecycle_context',
            content=context_text,
            priority=1,
            token_count=self._estimate_tokens(context_text),
            metadata={
                'functions_present': lifecycle_analysis['functions_present'],
                'functions_missing': lifecycle_analysis['functions_missing'],
                'problematic_operations': lifecycle_analysis['problematic_operations']
            }
        )
    
    def _build_lifecycle_context(self, code: str, analysis: Dict, metadata: Dict) -> str:
        """Build annotated lifecycle context"""
        lines = []
        lines.append("** LIFECYCLE CONTEXT **")
        lines.append("The functions below are lifecycle functions for original vulnerable code:")
        lines.append("")
        
        # List lifecycle functions with annotations
        for idx, func in enumerate(analysis['functions_present'], start=1):
            lines.append(f"{idx}. {func['name']} ({func['frequency']})")
            if func['is_performance_critical']:
                lines.append("  PERFORMANCE CRITICAL - called every frame")
            
            # Extract and show the function
            func_content = self._extract_function(code, func['name'])
            if func_content:
                lines.append("  Code:")
                for line in func_content.split('\n'):
                    lines.append(f"    {line}")
            lines.append("")
        
        # Add recommendations
        if analysis['recommendations']:
            lines.append("RECOMMENDATIONS:")
            for rec in analysis['recommendations']:
                lines.append(f"• {rec}")
        
        return '\n'.join(lines)
    
    def _extract_function(self, code: str, func_name: str) -> Optional[str]:
        """Extract a specific function from code"""
        pattern = rf'(?:private|public|protected|internal)?\s+\w+\s+{func_name}\s*\([^)]*\)\s*\{{'
        match = re.search(pattern, code)
        if not match:
            return None
        
        start = match.start()
        # Find matching closing brace
        brace_count = 1
        pos = match.end()
        while pos < len(code) and brace_count > 0:
            if code[pos] == '{':
                brace_count += 1
            elif code[pos] == '}':
                brace_count -= 1
            pos += 1
        
        return code[start:pos]
    
    def _estimate_tokens(self, text: str) -> int:
        """Estimate token count (roughly 4 chars per token)"""
        return len(text) // 4


class SceneGraphContextExtractor(ContextExtractor):
    """
    Extracts scene graph context - describes GameObject hierarchy, components,
    and structural complexity to help LLM understand relationship complexity.
    """
    
    def __init__(self):
        self.component_descriptions = {
            'Transform': 'Handles position, rotation, scale',
            'Rigidbody': 'Physics simulation and collision',
            'Collider': 'Collision detection (Trigger or regular)',
            'Animator': 'Animation state management',
            'Renderer': 'Renders visual mesh',
            'Camera': 'Renders game view',
            'AudioSource': 'Plays audio',
            'ParticleSystem': 'Particle effects',
            'Light': 'Lighting',
        }
    
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """
        Extract scene graph context from code
        
        Args:
            code: Source code
            metadata: May contain info about GameObject class names
            
        Returns:
            ExtractedContext with scene graph information
        """
        context_text = self._build_scene_graph_context(code, metadata)
        
        return ExtractedContext(
            context_type='scene_graph_context',
            content=context_text,
            priority=2,
            token_count=self._estimate_tokens(context_text),
            metadata={
                'components_detected': self._detect_components(code),
                'hierarchy_complexity': self._estimate_complexity(code)
            }
        )
    
    def _build_scene_graph_context(self, code: str, metadata: Dict) -> str:
        """Build scene graph context description"""
        lines = []
        lines.append("** SCENE GRAPH CONTEXT **")
        lines.append("The GameObject and Component Structure for original vulnerable code is as follows:")
        lines.append("")
        
        # Detect components used
        components = self._detect_components(code)
        if components:
            lines.append("Components attached to this GameObject:")
            for comp in components:
                lines.append(f"• {comp}")
                if comp in self.component_descriptions:
                    lines.append(f"  ({self.component_descriptions[comp]})")
            lines.append("")
        
        # Analyze GetComponent usage
        getcomp_patterns = re.findall(r'GetComponent<([^>]+)>', code)
        if getcomp_patterns:
            lines.append("Component Access Patterns:")
            for comp in set(getcomp_patterns):
                lines.append(f"• Accessing {comp}")
            lines.append("")
        
        # Analyze hierarchy traversal
        if 'GetComponentInChildren' in code:
            lines.append("Hierarchy traversal detected (GetComponentInChildren)")
            lines.append("Consider implications for performance and caching")
            lines.append("")
        
        # Estimate complexity
        complexity = self._estimate_complexity(code)
        lines.append(f"Structural Complexity: {complexity}")
        if complexity in ['moderate', 'complex']:
            lines.append("With complex hierarchies, prefer caching over repeated queries")
        
        return '\n'.join(lines)
    
    def _detect_components(self, code: str) -> List[str]:
        """Detect component types used in code"""
        components = set()
        
        # Check direct field declarations
        for comp_type in self.component_descriptions.keys():
            if re.search(rf'\b{comp_type}\b', code):
                components.add(comp_type)
        
        # Check GetComponent<T> calls
        getcomp_matches = re.findall(r'GetComponent<([^>]+)>', code)
        components.update(getcomp_matches)
        
        return sorted(list(components))
    
    def _estimate_complexity(self, code: str) -> str:
        """Estimate scene graph complexity"""
        # Count GetComponent calls (indicates complexity)
        getcomp_count = len(re.findall(r'GetComponent', code))
        hierarchy_queries = len(re.findall(r'GetComponentInChildren|Find', code))
        
        if hierarchy_queries > 0:
            return 'complex'
        elif getcomp_count > 3:
            return 'moderate'
        else:
            return 'simple'
    
    def _estimate_tokens(self, text: str) -> int:
        return len(text) // 4


class AsyncContextExtractor(ContextExtractor):
    """
    Extracts async context - provides coroutine code with yield highlights
    and async state information to help LLM understand timing and sequencing.
    """
    
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """
        Extract async context from code
        
        Args:
            code: Source code
            metadata: Additional context information
            
        Returns:
            ExtractedContext with async information
        """
        coroutines = self._extract_coroutines(code)
        context_text = self._build_async_context(code, coroutines, metadata)
        
        return ExtractedContext(
            context_type='async_context',
            content=context_text,
            priority=3,
            token_count=self._estimate_tokens(context_text),
            metadata={
                'coroutines_count': len(coroutines),
                'has_yield': 'yield' in code,
                'has_concurrent_operations': 'StartCoroutine' in code
            }
        )
    
    def _extract_coroutines(self, code: str) -> List[Tuple[str, str]]:
        """Extract coroutine definitions from code"""
        coroutines = []
        
        # Pattern: IEnumerator MethodName() { ... }
        pattern = r'IEnumerator\s+(\w+)\s*\([^)]*\)\s*\{{'
        for match in re.finditer(pattern, code):
            coro_name = match.group(1)
            # Extract function body
            start = match.end()
            brace_count = 1
            pos = start
            while pos < len(code) and brace_count > 0:
                if code[pos] == '{':
                    brace_count += 1
                elif code[pos] == '}':
                    brace_count -= 1
                pos += 1
            
            coro_body = code[start:pos-1]
            coroutines.append((coro_name, coro_body))
        
        return coroutines
    
    def _build_async_context(self, code: str, coroutines: List[Tuple[str, str]], 
                            metadata: Dict) -> str:
        """Build async context description"""
        lines = []
        lines.append("** ASYNC CONTEXT **")
        
        if coroutines:
            lines.append(f"The coroutines for original vulnerable code are as follows:")
            lines.append("")
            
            for coro_name, coro_body in coroutines:
                lines.append(f"Coroutine: {coro_name}")
                lines.append("-" * 40)
                
                # Show the coroutine with yield points highlighted
                annotated_body = self._annotate_yields(coro_body)
                for line in annotated_body.split('\n')[:20]:  # Show first 20 lines
                    lines.append(f"  {line}")
                
                if len(coro_body.split('\n')) > 20:
                    lines.append("  ...")
                
                lines.append("")
        
        # Analyze async patterns
        lines.append("Async Patterns:")
        
        start_coro_count = len(re.findall(r'StartCoroutine', code))
        if start_coro_count > 0:
            lines.append(f"• StartCoroutine calls: {start_coro_count}")
        
        yield_count = len(re.findall(r'yield\s+return', code))
        if yield_count > 0:
            lines.append(f"• Yield statements: {yield_count}")
        
        if 'WaitForSeconds' in code:
            lines.append("• Uses WaitForSeconds (time-based delays)")
        
        if 'WaitUntil' in code:
            lines.append("• Uses WaitUntil (conditional waiting)")
        
        lines.append("")
        lines.append("State Management Tips:")
        lines.append("• Ensure coroutines properly initialize and cleanup state")
        lines.append("• Be aware of timing between yield points")
        lines.append("• Consider race conditions if multiple coroutines access same data")
        
        return '\n'.join(lines)
    
    def _annotate_yields(self, code: str) -> str:
        """Annotate yield points in code"""
        lines = []
        for line in code.split('\n'):
            if 'yield' in line:
                lines.append(f">>> {line}  [YIELD POINT]")
            else:
                lines.append(line)
        return '\n'.join(lines)
    
    def _estimate_tokens(self, text: str) -> int:
        return len(text) // 4


class CallChainContextExtractor(ContextExtractor):
    """
    Extracts call chain context - provides upstream and downstream
    function calls to help LLM understand broader implications.
    """
    
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """
        Extract call chain context
        
        Args:
            code: Source code
            metadata: May contain function name and depth info
            
        Returns:
            ExtractedContext with call chain information
        """
        call_chain_data = self._extract_call_chain_from_codeql(metadata)
        context_text = self._build_call_chain_context(call_chain_data)
        logger.debug(
            "call_chain_context built: functions=%s, errors=%s, error_function=%s",
            len(call_chain_data.get('functions', [])),
            len(call_chain_data.get('errors', [])),
            call_chain_data.get('error_function')
        )

        return ExtractedContext(
            context_type='call_chain_context',
            content=context_text,
            priority=4,
            token_count=self._estimate_tokens(context_text),
            metadata=call_chain_data
        )
    
    def _build_call_chain_context(self, chain_data: Dict) -> str:
        """Build call chain context description"""
        lines = []
        lines.append("** CALL CHAIN CONTEXT **")

        functions = chain_data.get('functions', [])
        if functions:
            lines.append("The functions below are call chain functions for original vulnerable codeinferred from CodeQL dataflow results:")
            for idx, func in enumerate(functions, start=1):
                func_name = func.get('name') if isinstance(func, dict) else str(func)
                file_path = func.get('file_path') if isinstance(func, dict) else None
                if file_path:
                    lines.append(f"{idx}. {func_name}() [file: {os.path.basename(file_path)}]")
                else:
                    lines.append(f"{idx}. {func_name}()")
                snippet = func.get('snippet') if isinstance(func, dict) else None
                if snippet:
                    lines.append("```csharp")
                    lines.append(snippet)
                    lines.append("```")
        else:
            lines.append("No call chain functions inferred from CodeQL dataflow.")

        if chain_data.get('errors'):
            lines.append("")
            lines.append("Errors:")
            for err in chain_data['errors']:
                lines.append(f"• {err}")

        return '\n'.join(lines)

    def _extract_call_chain_from_codeql(self, metadata: Dict) -> Dict:
        """Run CodeQL dataflow query and map line index=5 to function names."""
        errors = []
        error_location = metadata.get('error_location', {})
        source_path = error_location.get('file_path', '')
        target_path = error_location.get('add_file_path', '')
        source_line = error_location.get('line')
        logger.debug(
            "dataflow input: source_path=%s, source_line=%s, target_path=%s",
            source_path, source_line, target_path
        )

        if not source_path or source_line is None:
            logger.warning("dataflow skipped: missing source_path or source_line")
            return {'functions': [], 'errors': ['Missing source file path or line in metadata.']}

        source_name = os.path.basename(source_path.replace('\\', '/'))
        target_name = os.path.basename(target_path.replace('\\', '/')) if target_path else ''

        db_path = metadata.get('baseline_db_path')
        project_root = metadata.get('project_root_dir')
        exp_dir = metadata.get('experiment_dir')
        exp_dir_name = metadata.get('exp_dir')

        if not db_path and project_root and exp_dir_name:
            db_name = exp_dir_name.split('!')[0]
            candidate = os.path.join(project_root, db_name)
            if os.path.exists(candidate):
                db_path = candidate

        if not db_path or not os.path.exists(db_path):
            logger.warning("dataflow skipped: CodeQL db not found: %s", db_path)
            return {'functions': [], 'errors': ['CodeQL database path not found.']}

        output_dir = exp_dir if exp_dir else os.getcwd()
        os.makedirs(output_dir, exist_ok=True)
        ts_ns = time.time_ns()
        safe_source = re.sub(r'[^A-Za-z0-9_.-]+', '_', source_name or 'source')
        safe_target = re.sub(r'[^A-Za-z0-9_.-]+', '_', target_name or 'target')
        output_csv = os.path.join(output_dir, f"out_{safe_source}_{source_line}_{safe_target}_{ts_ns}.csv")

        query = self._build_dataflow_query(source_name, int(source_line), target_name)
        query_dir = r"D:\codeql\csharp\ql\src\unityCheck"
        os.makedirs(query_dir, exist_ok=True)
        query_path = os.path.join(query_dir, f"temp_search_{safe_source}_{source_line}_{safe_target}_{ts_ns}.ql")
        with open(query_path, "w", encoding="utf-8") as f:
            f.write(query)

        codeql_bin = metadata.get('codeql_binary', 'codeql')
        search_path = metadata.get('codeql_search_path')

        cmd = [
            codeql_bin,
            "database",
            "analyze",
            db_path,
            query_path,
            "--format=csv",
            "--output",
            output_csv,
            "--rerun",
        ]
        if search_path:
            cmd.extend(["--search-path", search_path])

        try:
            subprocess.run(cmd, check=True)
        except Exception as exc:
            logger.exception("dataflow CodeQL analyze failed")
            return {'functions': [], 'errors': [f"CodeQL analyze failed: {exc}"]}

        functions = self._functions_from_csv(output_csv, project_root, metadata)
        logger.debug("dataflow raw functions=%s", functions)
        return {
            'functions': functions,
            'errors': errors,
            'output_csv': output_csv,
            'error_function': None
        }

    def _build_dataflow_query(self, source_name: str, source_line: int, target_name: str) -> str:
        return dedent(
            f"""
            /**
             * @id cs/dataflow
             * @kind path-problem
             * @name Data flow from targetPath to Destroy() method
             * @description This query finds data flow paths from an target method call to a Destroy()
             */
            import csharp
            import semmle.code.csharp.dataflow.DataFlow
            import semmle.code.csharp.dataflow.TaintTracking
            import DataFlow::PathGraph

            string sourcePath() {{ result = "{source_name}" }}
            int sourceLine() {{ result = {source_line} }}
            string targetPath() {{ result = "{target_name}" }}

            class InstantiateSource extends DataFlow::ExprNode {{
                InstantiateSource() {{
                    this.getLocation().getFile().getRelativePath().matches("%" + sourcePath() + "%") and
                    this.getLocation().getStartLine() = sourceLine()
                }}
            }}

            class InstantiateSink extends DataFlow::ExprNode {{
                InstantiateSink(){{
                    this.getLocation().getFile().getRelativePath().matches("%" + targetPath() + "%")
                    and
                    (exists(MethodCall call | this.asExpr() = call.getQualifier() or this.asExpr() = call)
                    or
                    exists(Access ta, MethodCall call | this.asExpr() = ta and call = ta.getParent().getParent()))
                }}
            }}

            predicate isComponentTainted(Expr expSrc, Expr expDest) {{
                exists(MethodCall call |
                    call.getTarget().getName().toLowerCase().matches("addcomponent%") or
                    call.getTarget().getName().toLowerCase().matches("getcomponent%") |
                    expSrc = call.getQualifier() and call = expDest
                )
            }}

            class InstantiateCheck extends TaintTracking::Configuration{{
                InstantiateCheck() {{
                    this = "InstantiateCheck"
                }}
                override predicate isSource(DataFlow::Node source){{
                    source instanceof InstantiateSource
                }}
                override predicate isSink(DataFlow::Node sink){{
                    sink instanceof InstantiateSink
                }}
                override predicate isAdditionalTaintStep(DataFlow::Node node1, DataFlow::Node node2){{
                    isComponentTainted(node1.asExpr(), node2.asExpr())
                }}
            }}

            from DataFlow::PathNode source, DataFlow::PathNode sink, InstantiateCheck ick
            where ick.hasFlowPath(source, sink)
            select sink.getNode(), source, sink, "Data flows from $@ to $@.", source.getNode(), "source", sink.getNode(), "sink"
            """
        ).strip()

    def _functions_from_csv(self, csv_path: str, project_root: Optional[str],
                            metadata: Optional[Dict] = None) -> List[Dict[str, str]]:
        functions: List[Dict[str, str]] = []
        if not os.path.exists(csv_path):
            return functions

        error_location = (metadata or {}).get('error_location', {})
        source_path = error_location.get('file_path', '')
        source_line = error_location.get('line')
        target_path = error_location.get('add_file_path', '')
        target_line = error_location.get('add_start_line')

        def _normalize(p: str) -> str:
            return p.replace("\\", "/").strip()

        source_path_norm = _normalize(source_path)
        target_path_norm = _normalize(target_path)
        reached_cutoff = False

        with open(csv_path, "r", encoding="utf-8", newline="") as f:
            reader = csv.reader(f)
            for row in reader:
                if len(row) <= 5:
                    continue
                # CSV format: description, file_path, start_line, start_col, end_line, end_col
                file_path = row[4]
                line_str = row[5]
                try:
                    line_num = int(line_str)
                except ValueError:
                    continue
                full_path = self._resolve_file_path(file_path, project_root)
                func_name, func_snippet = self._get_function_snippet_at_line(full_path, line_num)
                if func_name:
                    functions.append({
                        'name': func_name,
                        'snippet': func_snippet or "",
                        'file_path': full_path
                    })

                csv_path_norm = _normalize(file_path)
                if source_path_norm and csv_path_norm == source_path_norm and source_line is not None:
                    if line_num == int(source_line):
                        reached_cutoff = True
                elif target_path_norm and csv_path_norm == target_path_norm and target_line is not None:
                    if line_num == int(target_line):
                        reached_cutoff = True

                if reached_cutoff:
                    break

        # Deduplicate while keeping order
        seen = set()
        unique: List[Dict[str, str]] = []
        for func in functions:
            key = (func.get('name'), func.get('file_path'))
            if key not in seen:
                seen.add(key)
                unique.append(func)
        return unique

    def _resolve_file_path(self, file_path: str, project_root: Optional[str]) -> str:
        file_path = os.path.join(project_root, file_path.lstrip("/\\"))
        return file_path

    def _find_function_at_line(self, file_path: str, line_num: int) -> Optional[str]:
        if not os.path.exists(file_path):
            return None
        with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
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

    def _get_function_snippet_at_line(self, file_path: str, line_num: int) -> Tuple[Optional[str], Optional[str]]:
        if not os.path.exists(file_path):
            return None, None
        with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
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
                snippet = "".join(lines[start - 1:end]).rstrip()
                return name, snippet
        return None, None
    
    def _estimate_tokens(self, text: str) -> int:
        return len(text) // 4


class PerformanceContextExtractor(ContextExtractor):
    """
    Extracts performance context - provides performance impact analysis
    and optimization hints.
    """
    
    def extract(self, code: str, metadata: Dict) -> ExtractedContext:
        """
        Extract performance context
        
        Args:
            code: Source code
            metadata: Vulnerability information
            
        Returns:
            ExtractedContext with performance information
        """
        context_text = self._build_performance_context(code)
        
        return ExtractedContext(
            context_type='performance_context',
            content=context_text,
            priority=5,
            token_count=self._estimate_tokens(context_text),
            metadata={'performance_issues': self._detect_issues(code)}
        )
    
    def _build_performance_context(self, code: str) -> str:
        """Build performance context description"""
        lines = []
        lines.append("=== PERFORMANCE CONTEXT ===")
        
        issues = self._detect_issues(code)
        
        if not issues:
            lines.append("No obvious performance issues detected")
            return '\n'.join(lines)
        
        lines.append("Performance Issues Found:")
        lines.append("")
        
        for issue in issues:
            lines.append(f"{issue['description']}")
            lines.append(f"   Severity: {issue['severity']}")
            lines.append(f"   Impact: {issue['impact']}")
            lines.append("")
        
        lines.append("Optimization Recommendations:")
        lines.append("• Cache component references when possible")
        lines.append("• Move allocations from Update/LateUpdate to Awake/Start")
        lines.append("• Use object pooling for frequently instantiated objects")
        lines.append("• Batch scene graph queries")
        
        return '\n'.join(lines)
    
    def _detect_issues(self, code: str) -> List[Dict]:
        """Detect performance issues"""
        issues = []
        
        # Check for allocations in Update
        if 'Update' in code and 'new ' in code:
            issues.append({
                'description': 'Memory allocation in Update()',
                'severity': 'high',
                'impact': 'Called every frame - causes GC pressure'
            })
        
        # Check for GetComponent in loops
        if 'foreach' in code and 'GetComponent' in code:
            issues.append({
                'description': 'GetComponent() in loop',
                'severity': 'high',
                'impact': 'Repeated component lookups'
            })
        
        return issues
    
    def _estimate_tokens(self, text: str) -> int:
        return len(text) // 4


class MultiContextManager:
    """
    Manages extraction and ranking of multiple context types,
    ensuring token limits are respected.
    """
    
    def __init__(self, max_tokens: int = 4000, model: str = "gpt-4"):
        self.max_tokens = max_tokens
        self.model = model
        self.semantic_analyzer = XRSemanticAnalyzer()
        self.extractors = {
            'lifecycle_context': LifecycleContextExtractor(self.semantic_analyzer),
            'scene_graph_context': SceneGraphContextExtractor(),
            'async_context': AsyncContextExtractor(),
            'call_chain_context': CallChainContextExtractor(),
            #'performance_context': PerformanceContextExtractor(),
        }

    def _get_tokenizer(self, model: str):
        try:
            return tiktoken.encoding_for_model(model)
        except KeyError:
            return tiktoken.get_encoding("cl100k_base")

    def _token_length(self, text: str, model: str) -> int:
        tokenizer = self._get_tokenizer(model)
        return len(tokenizer.encode(text))

    def _estimate_tokens(self, text: str) -> int:
        token_len = self._token_length(text, self.model)
        multiplier = getattr(config, 'TOKEN_COUNT_MULTIPLIERS', {}).get(self.model, 1.0)
        adjusted_len = int(token_len * multiplier)
        count_prompt_token(self.max_tokens, self.model, text)
        return adjusted_len

    def _truncate_text_to_tokens(self, text: str, max_tokens: int) -> str:
        if max_tokens <= 0:
            return ""
        tokenizer = self._get_tokenizer(self.model)
        tokens = tokenizer.encode(text)
        if len(tokens) <= max_tokens:
            return text
        truncated = tokenizer.decode(tokens[:max_tokens]).rstrip()
        return truncated + "\n[TRUNCATED]"

    def _truncate_call_chain_context(self, context: ExtractedContext) -> ExtractedContext:
        if context.token_count <= self.max_tokens:
            return context

        chain_data = context.metadata or {}
        functions = chain_data.get('functions', []) or []
        errors = chain_data.get('errors', []) or []

        lines = []
        lines.append("** CALL CHAIN CONTEXT **")
        lines.append("The functions below are call chain functions for original vulnerable code inferred from CodeQL dataflow results:")

        if functions:
            for func in functions:
                func_name = func.get('name') if isinstance(func, dict) else str(func)
                candidate_lines = lines + [f"• {func_name}()"]
                snippet = func.get('snippet') if isinstance(func, dict) else None
                if snippet:
                    candidate_lines.extend(["```csharp", snippet, "```"])
                candidate_text = "\n".join(candidate_lines)
                if self._estimate_tokens(candidate_text) > self.max_tokens:
                    break
                lines.append(f"• {func_name}()")
                if snippet:
                    lines.extend(["```csharp", snippet, "```"])
        else:
            lines.append("No call chain functions inferred from CodeQL dataflow.")

        if errors:
            candidate_lines = lines + ["", "Errors:"]
            candidate_text = "\n".join(candidate_lines)
            if self._estimate_tokens(candidate_text) <= self.max_tokens:
                lines.extend(["", "Errors:"])
                for err in errors:
                    candidate_lines = lines + [f"• {err}"]
                    candidate_text = "\n".join(candidate_lines)
                    if self._estimate_tokens(candidate_text) > self.max_tokens:
                        break
                    lines.append(f"• {err}")

        truncated_text = "\n".join(lines)
        if self._estimate_tokens(truncated_text) > self.max_tokens:
            truncated_text = self._truncate_text_to_tokens(truncated_text, self.max_tokens)

        context.content = truncated_text
        context.token_count = self._estimate_tokens(truncated_text)
        logger.debug(
            "call_chain_context truncated: tokens=%s, max_tokens=%s",
            context.token_count, self.max_tokens
        )
        return context

    def _truncate_context(self, context: ExtractedContext) -> ExtractedContext:
        if context.token_count <= self.max_tokens:
            return context
        if context.context_type == 'call_chain_context':
            return self._truncate_call_chain_context(context)
        truncated = self._truncate_text_to_tokens(context.content, self.max_tokens)
        context.content = truncated
        context.token_count = self._estimate_tokens(truncated)
        return context
    
    def extract_all_contexts(self, code: str, metadata: Dict) -> Dict[str, ExtractedContext]:
        """
        Extract all available contexts, prioritized and token-limited
        
        Args:
            code: Source code to analyze
            metadata: Additional context information
            
        Returns:
            Dictionary of context_type -> ExtractedContext, ordered by priority
        """
        contexts = {}
        model = metadata.get('llm_model')
        if model:
            self.model = model
        used_tokens = 0
        
        # Extract all contexts
        all_extracted = []
        for context_type, extractor in self.extractors.items():
            try:
                context = extractor.extract(code, metadata)
                context.token_count = self._estimate_tokens(context.content)
                context = self._truncate_context(context)
                all_extracted.append(context)
            except Exception as e:
                print(f"Error extracting {context_type}: {e}")
        
        # Sort by priority and add while respecting token limit
        all_extracted.sort(key=lambda x: x.priority)
        
        for context in all_extracted:
            if used_tokens + context.token_count <= self.max_tokens:
                contexts[context.context_type] = context
                used_tokens += context.token_count
            else:
                break  # Stop adding contexts when token limit reached
        
        return contexts

    def extract_selected_contexts(self, code: str, metadata: Dict,
                                  selection: Dict[str, bool]) -> Dict[str, ExtractedContext]:
        contexts = {}
        model = metadata.get('llm_model')
        if model:
            self.model = model
        for context_type, extractor in self.extractors.items():
            if selection.get(context_type, False):
                try:
                    context = extractor.extract(code, metadata)
                    context.token_count = self._estimate_tokens(context.content)
                    context = self._truncate_context(context)
                    contexts[context.context_type] = context
                except Exception as e:
                    print(f"Error extracting {context_type}: {e}")
        return contexts
    
    def format_contexts_for_prompt(self, contexts: Dict[str, ExtractedContext]) -> str:
        """
        Format extracted contexts for inclusion in LLM prompt
        
        Args:
            contexts: Dictionary of extracted contexts
            
        Returns:
            Formatted string suitable for LLM prompt
        """
        def _has_meaningful_content(context: ExtractedContext) -> bool:
            if not context or not isinstance(context.content, str):
                return False
            if not context.content.strip():
                return False

            meta = context.metadata or {}

            # lifecycle_context: if no lifecycle functions, no recommendations, no problematic ops -> skip
            if context.context_type == 'lifecycle_context':
                funcs = meta.get('functions_present') or []
                recs = meta.get('recommendations') or []
                ops = meta.get('problematic_operations') or []
                if len(funcs) == 0 and len(recs) == 0 and len(ops) == 0:
                    return False

            # async_context: if no coroutine and no yield/concurrency signals -> skip
            if context.context_type == 'async_context':
                if (
                    (meta.get('coroutines_count', 0) or 0) == 0
                    and not meta.get('has_yield', False)
                    and not meta.get('has_concurrent_operations', False)
                ):
                    return False

            # call_chain_context: if no functions and no errors -> skip
            if context.context_type == 'call_chain_context':
                funcs = meta.get('functions') or []
                errs = meta.get('errors') or []
                if len(funcs) == 0 and len(errs) == 0:
                    return False

            # scene_graph_context: if nothing detected and complexity is simple -> skip
            if context.context_type == 'scene_graph_context':
                comps = meta.get('components_detected') or []
                complexity = (meta.get('hierarchy_complexity') or "").strip().lower()
                has_get_component = 'GetComponent' in context.content
                if len(comps) == 0 and not has_get_component and complexity in ("", "simple"):
                    return False

            # Generic: avoid including known placeholder-only sections
            meaningful_lines = [
                ln.strip()
                for ln in context.content.splitlines()
                if ln.strip() and not ln.strip().startswith("**")
            ]
            if not meaningful_lines:
                return False
            placeholder_prefixes = (
                "No call chain functions inferred",
                "No coroutine",
                "No coroutines",
            )
            if all(any(ln.startswith(p) for p in placeholder_prefixes) for ln in meaningful_lines):
                return False

            return True

        included_blocks: List[str] = []
        for context_type in ['lifecycle_context', 'scene_graph_context',
                             'async_context', 'call_chain_context']:
            context = contexts.get(context_type)
            if context and _has_meaningful_content(context):
                included_blocks.append(context.content.strip())

        if not included_blocks:
            return ""

        lines: List[str] = ["* XR DOMAIN CONTEXT INFORMATION *", ""]
        for block in included_blocks:
            lines.append(block)
            lines.append("")

        return '\n'.join(lines).rstrip()
