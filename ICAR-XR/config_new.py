"""
ICAR-XR Configuration Module

Extends XRFix config with ICAR-XR specific parameters for iterative repair
and context augmentation.
"""

import os
import sys

# Add parent directory to path to import XRFix config
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import config as base_config


# ============================================================================
# ICAR-XR Specific Configuration
# ============================================================================

# Maximum number of repair iterations
MAX_REPAIR_ITERATIONS = 3

# Model context windows (max tokens)
CONTEXT_WINDOWS = {
    "qwen3-235b-a22b": 120000,
    "gemini-2.5-pro": 896000,
    "gemini-2.5-pro-preview": 1000000,
    "gemini-3-pro-preview": 1000000,
    "deepseek-v3.2": 128000,
    "claude-sonnet-4-5-20250929": 200000,
    "gpt-5.2": 400000,
    "gpt-5": 400000,
    "kimi-k2-250905": 256000,
    "deepseek-v3.1": 128000,
    "claude-sonnet-4-20250514": 200000,
    "gpt-4o": 128000,
}

# Token count correction factors for models where tiktoken doesn't accurately match the API tokenizer
# Based on observed discrepancies: API token count / estimated token count
TOKEN_COUNT_MULTIPLIERS = {
    "claude-sonnet-4-20250514": 1.24,  # Observed: 200,379 / 161,997 ≈ 1.236
}

# Confidence threshold for considering a repair successful
SUCCESS_CONFIDENCE_THRESHOLD = 0.85

# ============================================================================
# Context Extraction Configuration
# ============================================================================

# Context types to extract (prioritized order)
CONTEXT_TYPES = {
    'lifecycle_context': {
        'enabled': True,
        'priority': 1,
        'description': 'Provides complete class code and highlights function lifecycle positions',
        'methods': ['Awake', 'OnEnable', 'Start', 'Update', 'LateUpdate', 'OnDisable', 'OnDestroy']
    },
    'scene_graph_context': {
        'enabled': True,
        'priority': 2,
        'description': 'Describes GameObject hierarchy, components, and structural complexity',
        'analysis_depth': 3  # Number of levels to traverse in hierarchy
    },
    'async_context': {
        'enabled': True,
        'priority': 3,
        'description': 'Provides coroutine code with yield highlights and async state',
        'keywords': ['StartCoroutine', 'yield', 'WaitForSeconds', 'WaitForEndOfFrame', 'async', 'await']
    },
    'call_chain_context': {
        'enabled': True,
        'priority': 4,
        'description': 'Upstream and downstream call chains',
        'depth': 2  # Number of levels to trace calls
    },
    'performance_context': {
        'enabled': True,
        'priority': 5,
        'description': 'Performance impact analysis and optimization hints',
        'metrics': ['memory_allocation', 'frame_budget', 'gc_pressure']
    }
}

# Context selection overrides by error type
# If an error type is present here, only the specified context types are enabled.
CONTEXT_SELECTION_BY_ERROR = {
    "Instantiate in Update() method": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': False
    },
    "Destroy in Update() method": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': False
    },
    "Repetitive allocation of YieldInstruction in a loop": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': True
    },
    "Using GetComponentsInChildren in Update() method.": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': True,
        'async_context': False
    },
    "Using GameObject.Find() in Update() method.": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': False
    },
    "Transform object of Rigidbody in Update() methods": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': False
    },
    "Using New() allocation in Update() method.": {
        'lifecycle_context': True,
        'call_chain_context': True,
        'performance_context': True,
        'scene_graph_context': False,
        'async_context': False
    }
}

# ============================================================================
# Validation Configuration
# ============================================================================

VALIDATION_STRATEGY = {
    'method': 'codeql',  # codeql, compilation, both, gradual
    'strict_mode': True,  # Whether to enforce strict validation
    'allow_new_warnings': False,  # Whether to accept repairs that introduce new warnings
    'accept_partial_fix': False  # Whether to accept repairs that partially fix the issue
}

# ============================================================================
# Context Storage Configuration
# ============================================================================

# Directory structure for storing contexts
CONTEXT_STORAGE = {
    'enabled': True,
    'base_directory': './icar_xr_contexts',
    'cache_duration': 3600,  # Cache context for 1 hour
    'version_control': True  # Track context versions
}

# ============================================================================
# Logging Configuration
# ============================================================================

LOGGING_CONFIG = {
    'level': 'INFO',
    'format': '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    'file': './icar_xr_repair.log',
    'console_output': True,
    'detailed_context_logging': True  # Log all extracted contexts
}

# ============================================================================
# Feature Flags
# ============================================================================

FEATURES = {
    'adaptive_context_selection': True,  # Dynamically select which contexts to include
    'parallel_repair_attempts': False,  # Try multiple repairs in parallel (experimental)
    'incremental_context_loading': True,  # Load context progressively
    'context_compression': True,  # Compress context to save tokens
    'repair_history_tracking': True,  # Track repair attempts and failures
}

# ============================================================================
# XR Domain Specific Keywords and Patterns
# ============================================================================

XR_KEYWORDS = {
    'lifecycle': [
        'Awake', 'OnEnable', 'Start', 'Update', 'LateUpdate', 'FixedUpdate',
        'OnDisable', 'OnDestroy', 'OnApplicationQuit', 'OnTriggerEnter',
        'OnTriggerStay', 'OnTriggerExit', 'OnCollisionEnter', 'OnCollisionStay',
        'OnCollisionExit', 'OnMouseDown', 'OnMouseUp', 'OnMouseDrag'
    ],
    'scene_graph': [
        'GameObject', 'Transform', 'GetComponent', 'GetComponentInChildren',
        'GetComponentInParent', 'Find', 'FindGameObjectWithTag', 'Instantiate',
        'Destroy', 'SetActive', 'parent', 'children', 'childCount', 'GetChild'
    ],
    'performance': [
        'new', 'Instantiate', 'Destroy', 'List.Clear', 'Dictionary.Clear',
        'GetComponent', 'FindObjectOfType', 'Resources.Load', 'yield',
        'Coroutine', 'StartCoroutine', 'StopCoroutine'
    ],
    'async_operations': [
        'Coroutine', 'yield', 'WaitForSeconds', 'WaitForEndOfFrame',
        'WaitForFixedUpdate', 'WaitUntil', 'async', 'await', 'Task'
    ]
}

# Error type mappings specific to XR
XR_ERROR_TYPES = {
    'memory_leak': ['new', 'Instantiate', 'GetComponent'],
    'performance_bottleneck': ['Update', 'LateUpdate', 'FixedUpdate'],
    'lifecycle_violation': ['OnDestroy', 'OnDisable'],
    'async_issues': ['StartCoroutine', 'yield', 'Task'],
    'scene_graph_issues': ['GetComponentInChildren', 'FindObjectOfType', 'transform.Find']
}

# ============================================================================
# CodeQL Query Configuration
# ============================================================================

# Unity-specific CodeQL queries (with compiler-friendly paths)
UNITY_REAL_QUERY_MAPPING = {
    "Using New() allocation in Update() method.": "/unityCheck/allocate/comp_new_allocation.ql",
    "Instantiate in Update() method": "/unityCheck/instantiate/comp_instan_in_update.ql",
    "Destroy in Update() method": "/unityCheck/instantiate/comp_destroy_in_update.ql",
    "Transform object of Rigidbody in Update() methods": "/unityCheck/rigidbody/comp_rigidbody_transform_in_update.ql",
    "Instantiate/Destroy in Update() method": "/unityCheck/instan_destroy_in_update.ql"
}

# Simplified Unity CodeQL queries
UNITY_QUERY_MAPPING = {
    "Using New() allocation in Update() method.": "/unityCheck/new_allocation.ql",
    "Instantiate/Destroy in Update() method": "/unityCheck/instan_destroy_in_update.ql",
    "Transform object of Rigidbody in Update() methods": "/unityCheck/rigidbody_transform_in_update.ql"
}

# CWE-related CodeQL queries
CWE_QUERY_MAPPING = {
    "Constant condition": "/Bad Practices/Control-Flow/ConstantCondition.ql",
    "Container contents are never accessed": "/Likely Bugs/Collections/WriteOnlyContainer.ql",
    "Locking the 'this' object in a lock statement": "/Concurrency/LockThis.ql",
    "Potentially dangerous use of non-short-circuit logic": "/Likely Bugs/DangerousNonShortCircuitLogic.ql",
    "Redundant Select": "/Linq/RedundantSelect.ql"
}

# Combined query mapping for all error types
QUERY_MAPPING = {
    **UNITY_REAL_QUERY_MAPPING,
    **CWE_QUERY_MAPPING
}

# ============================================================================
# Inherit base XRFix config if available
# ============================================================================


CWE_LIST = getattr(base_config, 'cwe_lis', [])
UNITY_ERROR_LIST = getattr(base_config, 'unity_lis', [])
ERROR_LIST = getattr(base_config, 'err_lis', [])
PROMPT_TEXT_FILENAME = getattr(base_config, 'PROMPT_TEXT_FILENAME', '')


OPENAI_API_KEY = "sk-4Ua8mCUTJBexQhXkD4812cCbC67b46CaBd557f522d7408E8"
OPENAI_API_KEY_HKBU = "a0169233-e03d-4175-a1fa-de05979a0e41"
basicUrl_gpt35 = 'https://api.bltcy.ai/v1/'