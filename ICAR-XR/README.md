# ICAR-XR: Iterative Context-Augmented Repair Framework for XR

## Overview

ICAR-XR is an advanced code repair framework specifically designed for Extended Reality (XR) applications. It transforms traditional one-shot code repair into an **iterative, context-aware, feedback-driven process** that mirrors how expert developers debug and fix code.

### Key Innovation

Unlike existing approaches that ask LLMs to perform repairs based only on static analyzer output, ICAR-XR implements a **three-stage closed-loop reasoning process**:

1. **Initial Repair & Verification** - Fast one-shot attempt
2. **XR-Specific Semantic Context Enhancement** - Rich context extraction
3. **Iterative Re-Repair** - Informed refinement with full domain understanding

## Architecture

```
Original Code with Vulnerability
            ↓
    ┌───────────────────┐
    │   STAGE 1: Fast   │
    │   Initial Repair  │ ← Basic LLM prompt
    └────────┬──────────┘
             ↓
        Validate
             ↓
        Success? ──Yes──→ DONE
             ↓ No
    ┌───────────────────────────────────────┐
    │     STAGE 2: Context Enhancement      │
    │  • Lifecycle Context (function analysis)
    │  • Scene Graph Context (hierarchy)     │
    │  • Async Context (coroutines)          │
    │  • Call Chain Context (callers/callees)│
    │  • Performance Context (optimization)  │
    └────────┬──────────────────────────────┘
             ↓
    ┌───────────────────────────────────────┐
    │    STAGE 3: Iterative Re-Repair       │
    │  Enhanced Context + Failure Feedback   │
    │  + Expert Hints + Alternatives         │
    └────────┬──────────────────────────────┘
             ↓
        Validate
             ↓
    Success or Max Iterations? ──Yes──→ DONE
             ↓ No
    Refine Context → Loop back to STAGE 3
```

## Core Components

### 1. **Iterative Repair Engine** (`iterative_repair_engine.py`)
Main orchestration engine that manages the three-stage repair process.

```python
from iterative_repair_engine import IterativeRepairEngine
from llm_interface import create_llm_interface

llm = create_llm_interface("gpt-4")
engine = IterativeRepairEngine(llm_interface=llm)

result = engine.repair(
    code_snippet=code,
    error_type='Using New() allocation in Update() method.',
    error_location={'file': 'script.cs', 'line': 15, 'function': 'Update'},
    llm_model='gpt-4'
)
```

### 2. **Context Extractor** (`context_extractor.py`)
Extracts multi-dimensional XR-specific contexts:

- **LifecycleContextExtractor**: Provides complete class code with lifecycle function annotations
- **SceneGraphContextExtractor**: Describes GameObject hierarchy and component structure
- **AsyncContextExtractor**: Provides coroutine code with yield point highlights
- **CallChainContextExtractor**: Upstream and downstream function calls
- **PerformanceContextExtractor**: Performance impact analysis and optimization hints

### 3. **XR Semantic Analyzer** (`xr_semantic_analyzer.py`)
Analyzes C# code for XR-specific patterns and domain semantics:

```python
from xr_semantic_analyzer import XRSemanticAnalyzer

analyzer = XRSemanticAnalyzer()
analysis = analyzer.analyze_code(code_snippet)

# Returns:
# - lifecycle_analysis: Which lifecycle functions are used
# - performance_analysis: Memory allocations, scene queries, etc.
# - async_analysis: Coroutine patterns and yields
# - scene_graph_hints: Component access patterns
# - error_prone_patterns: Known problematic patterns
# - xr_specific_concerns: VR/AR specific issues
```

### 4. **Code Validator** (`validator.py`)
Validates repairs using CodeQL and compilation checks:

```python
from validator import CodeValidator, ValidationFeedbackGenerator

validator = CodeValidator(config={'method': 'codeql_and_compilation'})
result = validator.validate(original_code, fixed_code, error_type)

feedback_gen = ValidationFeedbackGenerator()
feedback = feedback_gen.generate_feedback(
    original_code, failed_fix, validation_result
)
```

### 5. **LLM Interface** (`llm_interface.py`)
Unified interface for multiple LLM backends:

```python
from llm_interface import create_llm_interface

# Supports multiple models:
llm_gpt = create_llm_interface("gpt-4")
llm_starcoder = create_llm_interface("starcoder")
llm_mock = create_llm_interface("mock")  # For testing

response = llm.generate(
    prompt=prompt_text,
    model="gpt-4",
    temperature=0.7,
    max_tokens=2000
)
```

## Context Types and Information

### Lifecycle Context
```
=== LIFECYCLE CONTEXT ===
This code contains the following Unity lifecycle functions:

• Update (per_frame)
  PERFORMANCE CRITICAL - called every frame
  Code:
    void Update()
    {
      // ...
    }

RECOMMENDATIONS:
• OnEnable/OnDisable pattern detected - consider caching component references
• Performance-critical operations found in Update() - move to Awake/Start if possible
```

### Scene Graph Context
```
=== SCENE GRAPH CONTEXT ===
GameObject and Component Structure:

Components attached to this GameObject:
• Transform
  (Handles position, rotation, scale)
• Rigidbody
  (Physics simulation and collision)

Hierarchy traversal detected (GetComponentInChildren)
   Consider implications for performance and caching

Structural Complexity: moderate
With complex hierarchies, prefer caching over repeated queries
```

### Async Context
```
=== ASYNC CONTEXT ===
Found 1 coroutine(s):

Coroutine: UpdatePositions
----------------------------------------
  foreach (Transform child in children)
  {
      child.position += Vector3.up;
>>> yield return new WaitForSeconds(0.1f);  [YIELD POINT]
  }

Async Patterns:
• StartCoroutine calls: 2
• Yield statements: 1

State Management Tips:
• Ensure coroutines properly initialize and cleanup state
• Be aware of timing between yield points
• Consider race conditions if multiple coroutines access same data
```

## XR Domain-Specific Keywords

The framework tracks and understands:

### Lifecycle Keywords
`Awake`, `OnEnable`, `Start`, `Update`, `LateUpdate`, `FixedUpdate`, `OnDisable`, `OnDestroy`

### Scene Graph Keywords
`GameObject`, `Transform`, `GetComponent`, `GetComponentInChildren`, `Find`, `Instantiate`, `Destroy`

### Performance Keywords
`new`, `Instantiate`, `Destroy`, `List.Clear`, `GetComponent`, `FindObjectOfType`, `Resources.Load`

### Async Operations
`Coroutine`, `yield`, `WaitForSeconds`, `WaitForEndOfFrame`, `async`, `await`

## Configuration

See `config.py` for comprehensive configuration options:

```python
# Maximum repair iterations
MAX_REPAIR_ITERATIONS = 3

# Maximum context size (tokens)
MAX_CONTEXT_TOKENS = 4000

# Context types to extract
CONTEXT_TYPES = {
    'lifecycle_context': {'enabled': True, 'priority': 1},
    'scene_graph_context': {'enabled': True, 'priority': 2},
    'async_context': {'enabled': True, 'priority': 3},
    # ... more types
}

# Validation strategy
VALIDATION_STRATEGY = {
    'method': 'codeql_and_compilation',
    'strict_mode': True,
    'allow_new_warnings': False
}

# Feature flags
FEATURES = {
    'adaptive_context_selection': True,
    'incremental_context_loading': True,
    'context_compression': True,
    'repair_history_tracking': True
}
```

## Usage Examples

### Example 1: Memory Allocation in Update()

```python
vulnerable_code = '''
void Update()
{
    List<Vector3> path = new List<Vector3>();  // BUG!
    for (int i = 0; i < 100; i++)
    {
        path.Add(new Vector3(i, 0, 0));
    }
}
'''

result = engine.repair(
    code_snippet=vulnerable_code,
    error_type='Using New() allocation in Update() method.',
    error_location={'file': 'script.cs', 'line': 3, 'function': 'Update'}
)

# ICAR-XR will:
# 1. Try one-shot repair
# 2. If failed, extract lifecycle context showing Update() is per-frame
# 3. Extract performance context suggesting object pooling
# 4. Iteratively refine with better understanding
```

### Example 2: Rigidbody Transform Manipulation

```python
vulnerable_code = '''
void Update()
{
    transform.position += new Vector3(x, 0, z) * speed * Time.deltaTime;  // BUG!
}
'''

result = engine.repair(
    code_snippet=vulnerable_code,
    error_type='Transform object of Rigidbody in Update() methods',
    error_location={'file': 'script.cs', 'line': 3, 'function': 'Update'}
)

# ICAR-XR will:
# 1. Try basic fix
# 2. Extract scene graph context showing Rigidbody components
# 3. Provide physics-specific context about proper movement methods
# 4. Suggest Rigidbody.velocity or MovePosition() instead
```

## Repair History Tracking

The framework tracks all repair attempts and can export detailed histories:

```python
# Get repair history for an issue
history = engine.get_repair_history(issue_id)

# Export to JSON
engine.export_repair_history(
    issue_id='script.cs_15',
    output_file='repair_history.json'
)

# History includes:
# - All repair attempts with patches
# - Validation results at each stage
# - Contexts used at each iteration
# - Timing information
# - LLM model and parameters used
```

## Integration with XRFix

ICAR-XR extends the existing XRFix framework:

- Inherits error detection from XRFix (CWE + Unity-specific)
- Integrates with XRFix's CodeQL validation infrastructure
- Extends XRFix's LLM interfaces (GPT, StarCoder, etc.)
- Adds context-aware repair on top of one-shot approaches

## Performance Metrics

The framework provides detailed metrics:

```python
result = engine.repair(...)

print(f"Successful: {result['success']}")
print(f"Iterations needed: {result['iterations']}")
print(f"Total time: {result['total_time']:.2f}s")
print(f"Final status: {result['status']}")

# Detailed attempt history
for attempt in result['attempts']:
    print(f"Iteration {attempt['iteration']}: {attempt['validation_status']}")
```

## Advanced Features

### Adaptive Context Selection
Dynamically selects which contexts to include based on token budget and relevance.

### Incremental Context Loading
Loads contexts progressively to handle large codebases.

### Context Compression
Compresses contexts to save tokens while preserving information.

### Parallel Repair Attempts (Experimental)
Tries multiple repair strategies in parallel for faster convergence.

## Error Types Supported

### CWE (Common Weakness Enumeration)
- Constant condition
- Container contents are never accessed
- Locking the 'this' object
- Potentially dangerous non-short-circuit logic
- Redundant Select

### Unity-Specific
- Using New() allocation in Update() method
- Instantiate/Destroy in Update() method
- Transform object of Rigidbody in Update() methods

(Extensible for additional error types)

## Testing

Run the demonstration:

```bash
python main.py --mode demo --llm gpt-4
```

View the quick start guide:

```bash
python main.py --mode guide
```

## License

Part of the XRFix research framework.


## Contact & Support

For questions or issues, please refer to the main XRFix repository.
