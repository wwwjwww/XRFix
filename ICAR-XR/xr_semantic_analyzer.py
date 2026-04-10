"""
XR Semantic Analyzer Module

Analyzes C# code for XR-specific patterns, lifecycle issues, and domain semantics.
Provides insights into game object hierarchies, lifecycle functions, and performance concerns.
"""

import re
import json
from typing import Dict, List, Tuple, Optional, Set
from dataclasses import dataclass, asdict
from enum import Enum

import config_new as config


class LifecyclePhase(Enum):
    """Unity lifecycle phases"""
    INITIALIZATION = "initialization"  # Awake, OnEnable, Start
    UPDATE = "update"  # Update, LateUpdate, FixedUpdate
    PHYSICS = "physics"  # FixedUpdate, physics callbacks
    RENDERING = "rendering"  # LateUpdate, rendering callbacks
    CLEANUP = "cleanup"  # OnDisable, OnDestroy
    INPUT = "input"  # Input callbacks
    ASYNC = "async"  # Coroutine-based


@dataclass
class LifecycleInfo:
    """Information about a lifecycle function"""
    name: str
    phase: LifecyclePhase
    frequency: str  # "once", "per_frame", "per_fixed_frame", "event"
    is_performance_critical: bool
    description: str


@dataclass
class ComponentInfo:
    """Information about a GameComponent"""
    name: str
    type: str
    purpose: str
    common_issues: List[str]


@dataclass
class GameObjectHierarchy:
    """Information about GameObject hierarchy structure"""
    root_object: str
    depth: int
    child_count: int
    total_objects: int
    components: List[ComponentInfo]
    structural_complexity: str  # simple, moderate, complex


class XRSemanticAnalyzer:
    """
    Analyzes C# code for XR domain-specific patterns and semantics.
    Provides context about lifecycle, scene graph, performance, and async operations.
    """
    
    # Lifecycle function metadata
    LIFECYCLE_FUNCTIONS = {
        'Awake': LifecycleInfo('Awake', LifecyclePhase.INITIALIZATION, 'once', False,
                              'Called when script instance is loaded'),
        'OnEnable': LifecycleInfo('OnEnable', LifecyclePhase.INITIALIZATION, 'event', False,
                                 'Called when object becomes enabled'),
        'Start': LifecycleInfo('Start', LifecyclePhase.INITIALIZATION, 'once', False,
                              'Called on first frame object is enabled'),
        'Update': LifecycleInfo('Update', LifecyclePhase.UPDATE, 'per_frame', True,
                               'Called once per frame'),
        'LateUpdate': LifecycleInfo('LateUpdate', LifecyclePhase.UPDATE, 'per_frame', True,
                                   'Called once per frame after Update'),
        'FixedUpdate': LifecycleInfo('FixedUpdate', LifecyclePhase.PHYSICS, 'per_fixed_frame', True,
                                    'Called at fixed time intervals for physics'),
        'OnDisable': LifecycleInfo('OnDisable', LifecyclePhase.CLEANUP, 'event', False,
                                  'Called when object becomes disabled'),
        'OnDestroy': LifecycleInfo('OnDestroy', LifecyclePhase.CLEANUP, 'once', False,
                                  'Called when object is destroyed'),
    }
    
    def __init__(self):
        """Initialize the XR semantic analyzer"""
        self.lifecycle_patterns = self._compile_lifecycle_patterns()
        self.performance_antipatterns = self._compile_antipatterns()
    
    def _compile_lifecycle_patterns(self) -> Dict[str, re.Pattern]:
        """Compile regex patterns for lifecycle function detection"""
        patterns = {}
        for func_name in self.LIFECYCLE_FUNCTIONS.keys():
            # Match method declarations: void MethodName() or private void MethodName()
            pattern = rf'\b(?:private|public|protected|internal)?\s+void\s+{func_name}\s*\(\s*\)'
            patterns[func_name] = re.compile(pattern)
        return patterns
    
    def _compile_antipatterns(self) -> Dict[str, re.Pattern]:
        """Compile patterns for performance antipatterns"""
        return {
            'memory_alloc_in_update': re.compile(
                r'\b(?:new\s+\w+|Instantiate|GetComponent|Resources\.Load)\b',
                re.IGNORECASE
            ),
            'scene_query_in_loop': re.compile(
                r'(?:GetComponentInChildren|FindObjectOfType|Find|FindGameObjectWithTag)\s*\(',
                re.IGNORECASE
            ),
            'string_operations': re.compile(
                r'(?:GetComponent|SendMessage|CompareTag)\s*<?\s*"',
                re.IGNORECASE
            ),
        }
    
    def analyze_code(self, code: str) -> Dict:
        """
        Comprehensive semantic analysis of code
        
        Args:
            code: C# source code to analyze
            
        Returns:
            Dictionary containing all semantic analysis results
        """
        return {
            'lifecycle_analysis': self.analyze_lifecycle_usage(code),
            'performance_analysis': self.analyze_performance_patterns(code),
            'async_analysis': self.analyze_async_operations(code),
            'scene_graph_hints': self.analyze_scene_graph_patterns(code),
            'error_prone_patterns': self.detect_error_prone_patterns(code),
            'xr_specific_concerns': self.analyze_xr_specific_concerns(code)
        }
    
    def analyze_lifecycle_usage(self, code: str) -> Dict[str, any]:
        """
        Analyze how lifecycle functions are used in code
        
        Args:
            code: C# source code
            
        Returns:
            Dictionary with lifecycle analysis results
        """
        results = {
            'functions_present': [],
            'functions_missing': [],
            'problematic_operations': [],
            'recommendations': []
        }
        
        for func_name, lifecycle_info in self.LIFECYCLE_FUNCTIONS.items():
            if self.lifecycle_patterns[func_name].search(code):
                results['functions_present'].append({
                    'name': func_name,
                    'phase': lifecycle_info.phase.value,
                    'frequency': lifecycle_info.frequency,
                    'is_performance_critical': lifecycle_info.is_performance_critical
                })
                
                # Analyze what's in each lifecycle function
                func_content = self._extract_function_body(code, func_name)
                if func_content:
                    problematic = self._analyze_function_content(func_content, func_name)
                    results['problematic_operations'].extend(problematic)
            else:
                results['functions_missing'].append(func_name)
        
        # Generate recommendations
        results['recommendations'] = self._generate_lifecycle_recommendations(results)
        
        return results
    
    def analyze_performance_patterns(self, code: str) -> Dict[str, any]:
        """
        Detect performance antipatterns in code
        
        Args:
            code: C# source code
            
        Returns:
            Dictionary with performance analysis
        """
        results = {
            'issues_found': [],
            'severity_distribution': {},
            'impact_areas': []
        }
        
        # Check for memory allocation in Update
        update_body = self._extract_function_body(code, 'Update')
        if update_body:
            allocs = self.performance_antipatterns['memory_alloc_in_update'].findall(update_body)
            if allocs:
                results['issues_found'].append({
                    'type': 'memory_allocation_in_update',
                    'severity': 'high',
                    'occurrences': len(allocs),
                    'description': 'Memory allocation detected in Update() - called every frame'
                })
        
        # Check for scene queries in loops
        loop_matches = re.finditer(r'\b(?:for|while|foreach)\s*[\(\[]', code)
        for match in loop_matches:
            loop_context = code[match.start():match.start()+500]
            if self.performance_antipatterns['scene_query_in_loop'].search(loop_context):
                results['issues_found'].append({
                    'type': 'scene_query_in_loop',
                    'severity': 'high',
                    'description': 'Scene graph queries detected in loop'
                })
        
        return results
    
    def analyze_async_operations(self, code: str) -> Dict[str, any]:
        """
        Analyze coroutine and async operations
        
        Args:
            code: C# source code
            
        Returns:
            Dictionary with async analysis
        """
        results = {
            'coroutines_found': 0,
            'async_patterns': [],
            'potential_issues': [],
            'recommendations': []
        }
        
        # Find coroutine definitions
        coroutine_pattern = re.compile(r'IEnumerator\s+(\w+)\s*\([^)]*\)')
        coroutines = coroutine_pattern.findall(code)
        results['coroutines_found'] = len(coroutines)
        
        # Analyze yield patterns
        yield_matches = re.finditer(r'yield\s+(return|break)\s+(.+?)(?:\n|;)', code)
        for match in yield_matches:
            results['async_patterns'].append({
                'type': 'yield',
                'keyword': match.group(1),
                'target': match.group(2).strip()
            })
        
        # Detect potential race conditions
        if 'StartCoroutine' in code and 'StopCoroutine' in code:
            results['potential_issues'].append({
                'type': 'coroutine_lifecycle',
                'description': 'StartCoroutine and StopCoroutine both used - possible race conditions'
            })
        
        return results
    
    def analyze_scene_graph_patterns(self, code: str) -> Dict[str, any]:
        """
        Analyze patterns related to scene graph usage
        
        Args:
            code: C# source code
            
        Returns:
            Dictionary with scene graph analysis
        """
        results = {
            'hierarchy_queries': [],
            'component_access_patterns': [],
            'optimization_opportunities': [],
            'complexity_indicators': []
        }
        
        # Detect GetComponent patterns
        getcomp_pattern = re.compile(r'GetComponent<([^>]+)>\s*\(\)')
        for match in re.finditer(getcomp_pattern, code):
            results['component_access_patterns'].append({
                'method': 'GetComponent',
                'component_type': match.group(1),
                'frequency': code.count(match.group(0))
            })
        
        # Detect FindObjectOfType and similar expensive operations
        expensive_ops = ['FindObjectOfType', 'FindObjectsOfType', 'FindGameObjectWithTag', 'Find']
        for op in expensive_ops:
            if op in code:
                results['hierarchy_queries'].append({
                    'operation': op,
                    'cost': 'expensive',
                    'occurrences': len(re.findall(rf'\b{op}\b', code))
                })
        
        # Suggest optimizations
        if results['hierarchy_queries']:
            results['optimization_opportunities'].append(
                'Cache expensive scene graph queries in Awake() or Start()'
            )
        
        return results
    
    def analyze_xr_specific_concerns(self, code: str) -> Dict[str, any]:
        """
        Analyze XR-specific concerns (VR/AR patterns)
        
        Args:
            code: C# source code
            
        Returns:
            Dictionary with XR-specific analysis
        """
        results = {
            'vr_patterns': [],
            'ar_patterns': [],
            'spatial_concerns': [],
            'frame_rate_sensitive_code': [],
            'recommendations': []
        }
        
        # Detect VR-specific patterns
        vr_keywords = ['XRRig', 'InputAction', 'XRController', 'TrackedDevice', 'HandTracking']
        for keyword in vr_keywords:
            if keyword in code:
                results['vr_patterns'].append(keyword)
        
        # Detect frame-rate sensitive code
        if 'Update' in code and ('transform.position' in code or 'transform.rotation' in code):
            results['frame_rate_sensitive_code'].append('Direct transform manipulation in Update')
        
        # Detect spatial computation in Update
        if 'Vector3.Distance' in code or 'Vector3.Lerp' in code:
            results['spatial_concerns'].append('Spatial computations detected')
        
        return results
    
    def detect_error_prone_patterns(self, code: str) -> List[Dict[str, any]]:
        """
        Detect patterns known to cause errors in XR code
        
        Args:
            code: C# source code
            
        Returns:
            List of detected error-prone patterns
        """
        patterns = []
        
        # Pattern 1: Rigidbody manipulation in Update
        if 'Rigidbody' in code and 'Update' in code:
            update_body = self._extract_function_body(code, 'Update')
            if update_body and 'transform.position' in update_body:
                patterns.append({
                    'name': 'direct_rigidbody_transform_manipulation',
                    'severity': 'high',
                    'fix': 'Use Rigidbody.velocity or Rigidbody.MovePosition() instead'
                })
        
        # Pattern 2: Unhandled null references
        if 'GetComponent' in code:
            patterns.append({
                'name': 'potential_null_reference',
                'severity': 'medium',
                'fix': 'Always check GetComponent result for null'
            })
        
        # Pattern 3: Array/List modification in iteration
        if re.search(r'foreach\s*\(\s*\w+\s+\w+\s+in\s+\w+.*?\)\s*\{', code):
            patterns.append({
                'name': 'collection_modification_in_iteration',
                'severity': 'medium',
                'fix': 'Copy collection before modifying or use indexed loop'
            })
        
        return patterns
    
    def _extract_function_body(self, code: str, func_name: str) -> Optional[str]:
        """Extract the body of a specific function"""
        pattern = rf'\b(?:private|public|protected|internal)?\s+void\s+{func_name}\s*\(\s*\)\s*\{{'
        match = re.search(pattern, code)
        if not match:
            return None
        
        start = match.end()
        # Find matching closing brace
        brace_count = 1
        pos = start
        while pos < len(code) and brace_count > 0:
            if code[pos] == '{':
                brace_count += 1
            elif code[pos] == '}':
                brace_count -= 1
            pos += 1
        
        return code[start:pos-1]
    
    def _analyze_function_content(self, func_body: str, func_name: str) -> List[Dict[str, any]]:
        """Analyze content of a specific function"""
        issues = []
        lifecycle_info = self.LIFECYCLE_FUNCTIONS.get(func_name)
        
        if lifecycle_info and lifecycle_info.is_performance_critical:
            # Check for expensive operations in performance-critical functions
            if self.performance_antipatterns['memory_alloc_in_update'].search(func_body):
                issues.append({
                    'function': func_name,
                    'issue': 'memory_allocation_in_critical_function',
                    'severity': 'high'
                })
        
        return issues
    
    def _generate_lifecycle_recommendations(self, lifecycle_results: Dict) -> List[str]:
        """Generate recommendations based on lifecycle analysis"""
        recommendations = []
        
        present_funcs = {f['name'] for f in lifecycle_results['functions_present']}
        
        # Recommend caching if using OnEnable/OnDisable pattern
        if 'OnEnable' in present_funcs and 'OnDisable' in present_funcs:
            recommendations.append(
                'OnEnable/OnDisable pattern detected - consider caching component references'
            )
        
        # Warn if problematic operations in Update
        if any(p['function'] == 'Update' for p in lifecycle_results.get('problematic_operations', [])):
            recommendations.append(
                'Performance-critical operations found in Update() - move to Awake/Start if possible'
            )
        
        return recommendations


def create_xr_context_summary(analysis: Dict) -> str:
    """
    Create a human-readable summary of XR semantic analysis
    
    Args:
        analysis: Result from analyze_code()
        
    Returns:
        Formatted summary string
    """
    summary_parts = []
    
    # Lifecycle summary
    lifecycle = analysis['lifecycle_analysis']
    if lifecycle['functions_present']:
        summary_parts.append("## Lifecycle Functions")
        for func in lifecycle['functions_present']:
            summary_parts.append(f"- {func['name']}: {func['phase']} ({func['frequency']})")
    
    # Performance issues
    perf = analysis['performance_analysis']
    if perf['issues_found']:
        summary_parts.append("\n## Performance Issues")
        for issue in perf['issues_found']:
            summary_parts.append(f"- {issue['type']} (severity: {issue['severity']})")
    
    # Async operations
    async_info = analysis['async_analysis']
    if async_info['coroutines_found'] > 0:
        summary_parts.append(f"\n## Async Operations\n- Found {async_info['coroutines_found']} coroutines")
    
    return '\n'.join(summary_parts)
