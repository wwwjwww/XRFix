"""
ICAR-XR Main Entry Point

Example usage and integration of the Iterative Context-Augmented Repair Framework
for XR Applications.
"""

import json
import logging
import sys
import os
from pathlib import Path

# Setup logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

from iterative_repair_engine import IterativeRepairEngine
from llm_interface import create_llm_interface
from experiment_scenario_processor import ExperimentScenarioProcessor


def _has_existing_repair_history(experiment_dir: str) -> bool:
    """
    Return True if experiment_dir/repair_results contains any *_history.json file.
    """
    export_dir = Path(experiment_dir) / 'repair_results'
    if not export_dir.is_dir():
        return False
    return any(p.is_file() and p.name.endswith('_history.json') for p in export_dir.iterdir())

def example_repair_workflow(scenario_path: str = None, llm_model: str = "gpt-4"):
    """
    Example workflow demonstrating ICAR-XR framework
    
    Args:
        scenario_path: Path to scenario.json file. If None, uses default demo scenarios.
        llm_model: LLM model to use (default: gpt-4)
    """
    
    print("=" * 70)
    print("ICAR-XR: Iterative Context-Augmented Repair Framework for XR")
    print("=" * 70)
    print()
    
    # ========================================================================
    # Setup: Initialize components
    # ========================================================================
    print("Setting up ICAR-XR components...")
    print()
    
    # Create LLM interface (can be GPT, StarCoder, etc.)
    llm = create_llm_interface(llm_model)
    
    # Create repair engine
    engine = IterativeRepairEngine(
        llm_interface=llm,
        config_dict={
            'max_context_tokens': 4000,
            'max_repair_iterations': 3,
            'validation_strategy': {
                'method': 'both',
                'strict_mode': True
            }
        }
    )
    
    # Require scenario_path for demo mode
    if not scenario_path:
        print("Error: Demo mode requires a scenario.json file.")
        print("Please provide a scenario file using --scenario option.")
        print("\nExample:")
        print("  python ICAR-XR/main.py --mode demo --scenario ./experiment/unity_real_basic/destroy_in_update/6DOF_db!Assets!Dodge!Scripts!ARAnchoring.cs~38~13~38~63/scenario.json")
        return None
    
    return _run_scenario_based_demo(engine, scenario_path, llm_model)


def _run_scenario_based_demo(engine: IterativeRepairEngine, scenario_path: str, llm_model: str):
    """
    Run demo using real scenario.json data
    
    Args:
        engine: Repair engine instance
        scenario_path: Path to scenario.json file
        llm_model: LLM model to use
    """
    print("\n" + "=" * 70)
    print("DEMO: Using Real Experiment Scenario")
    print("=" * 70)
    print()
    
    # Initialize scenario processor
    processor = ExperimentScenarioProcessor()
    
    # Load scenario
    experiment_dir = os.path.dirname(os.path.abspath(scenario_path))
    scenario_data = processor.load_scenario(scenario_path)
    
    if not scenario_data:
        print(f"Failed to load scenario from: {scenario_path}")
        return None
    
    print(f"Loaded scenario from: {scenario_path}")
    print(f"Experiment directory: {experiment_dir}")
    print()
    
    # Prepare repair context
    context = processor.prepare_repair_context(scenario_data, experiment_dir)
    
    # Load additional code files
    additional_files = processor.load_additional_code_files(
        experiment_dir,
        context['experiment_file'],
        context['include_addition']
    )
    context.update(additional_files)
    
    # Determine if function-based merge
    is_function = None
    if context['include_addition']:
        file_name = scenario_data['err_detailed_info']['file_name']
        add_file_name = scenario_data['err_detailed_info'].get('add_file_name', '')
        is_function = (file_name == add_file_name)
    context['is_function'] = is_function
    
    # Prepare merge context
    merge_strategy = context.get('combine_settings')
    merge_context = {
        'comment_key': '//',
        'prepend_contents': context.get('prepend_contents', ''),
        'append_contents': context.get('append_contents', ''),
        'whitespace': context.get('whitespace', ''),
        'add_contents': context.get('add_contents', ''),
        'add_whitespace': context.get('add_whitespace', ''),
        'between_contents': context.get('between_contents', ''),
        'add_first': context.get('add_first', ''),
        'add_append_contents': context.get('add_append_contents', ''),
        'add_prepend_contents': context.get('add_prepend_contents', ''),
        'include_addition': context.get('include_addition', False),
        'is_function': is_function,
        'target_file': context.get('target_file', ''),
        'add_file': context.get('add_file', '')
    }
    
    # Display scenario information
    print("Scenario Information:")
    print(f"  Error Type: {context['error_type']}")
    print(f"  File: {context['experiment_file']}")
    print(f"  Location: Line {context['error_location'].get('line', 'N/A')}")
    print(f"  Project Root: {context['project_root_dir']}")
    print(f"  CodeQL Query: {context.get('codeql_query', 'N/A')}")
    print()
    
    # Display vulnerable code snippet
    print("Vulnerable Code Snippet:")
    print("-" * 70)
    code_preview = context['code_snippet'][:500]  # Show first 500 chars
    print(code_preview)
    if len(context['code_snippet']) > 500:
        print("... (truncated)")
    print("-" * 70)
    print()
    
    # Run repair
    print("Running ICAR-XR repair pipeline...")
    result = engine.repair(
        code_snippet=context['code_snippet'],
        error_type=context['error_type'],
        error_location=context['error_location'],
        llm_model=llm_model,
        validation_context=context
    )
    
    print()
    print("Repair Results:")
    print(f"  Success: {result['success']}")
    print(f"  Status: {result['status']}")
    print(f"  Iterations: {result['iterations']}")
    print(f"  Time: {result['total_time']:.2f} seconds")
    print()
    
    if result['success']:
        print("Repair successful!")
        print("Final Patch Preview:")
        print("-" * 70)
        patch_preview = result['final_patch'][:500] if result['final_patch'] else "N/A"
        print(patch_preview)
        if result['final_patch'] and len(result['final_patch']) > 500:
            print("... (truncated)")
        print("-" * 70)
    else:
        print("Repair failed")
        if result.get('attempts'):
            print("\nAttempt Summary:")
            for attempt in result['attempts']:
                print(f"  Iteration {attempt['iteration']}: {attempt['validation_status']}")
    
    print()
    
    # Export repair history under experiment directory
    export_base_dir = Path(context.get('experiment_dir', '.'))
    export_dir = export_base_dir / 'repair_results'
    export_dir.mkdir(exist_ok=True, parents=True)
    
    issue_id = result['issue_id']
    export_file = export_dir / f'{issue_id}_history.json'
    if engine.export_repair_history(issue_id, str(export_file)):
        print(f"Exported repair history to: {export_file}")
    
    return result


def batch_process_experiments(experiment_root: str, llm_model: str = 'gpt-4',
                              filter_func=None):
    """
    Batch process experiment scenarios from XRFix experiment folders
    
    Args:
        experiment_root: Root directory containing experiment folders
        llm_model: LLM model to use for repair
        filter_func: Optional function to filter scenarios
    """
    print("=" * 70)
    print("ICAR-XR: Batch Processing Experiment Scenarios")
    print("=" * 70)
    print()
    
    # Initialize components
    print("Setting up ICAR-XR components...")
    llm = create_llm_interface(llm_model)
    engine = IterativeRepairEngine(
        llm_interface=llm,
        config_dict={
            'max_context_tokens': 256000,
            'max_repair_iterations': 3,
            'validation_strategy': {
                'method': 'both',
                'strict_mode': True
            }
        }
    )
    
    # Process scenarios
    print(f"Searching for scenarios in: {experiment_root}")
    processor = ExperimentScenarioProcessor(experiment_root)
    scenarios = processor.find_all_scenarios()
    
    print(f"Found {len(scenarios)} scenarios to process")
    print()
    
    results = []
    for idx, scenario_info in enumerate(scenarios, 1):
        scenario_data = scenario_info['scenario_data']
        experiment_dir = scenario_info['experiment_dir']
        
        # Apply filter if provided
        if filter_func and not filter_func(scenario_data):
            continue

        # Skip scenarios that already have exported repair history.
        # If experiment_dir/repair_results exists and contains *_history.json, treat it as processed.
        if _has_existing_repair_history(experiment_dir):
            print(f"\n[{idx}/{len(scenarios)}] Skipping (existing repair history): {experiment_dir}")
            continue
        
        print(f"\n[{idx}/{len(scenarios)}] Processing: {experiment_dir}")
        
        try:
            # Prepare repair context
            context = processor.prepare_repair_context(scenario_data, experiment_dir)
            
            # Load additional code files
            additional_files = processor.load_additional_code_files(
                experiment_dir,
                context['experiment_file'],
                context['include_addition']
            )
            context.update(additional_files)
            
            # Determine if function-based merge
            is_function = None
            if context['include_addition']:
                file_name = scenario_data['err_detailed_info']['file_name']
                add_file_name = scenario_data['err_detailed_info'].get('add_file_name', '')
                is_function = (file_name == add_file_name)
            context['is_function'] = is_function
            
            # Add LLM engine for result tracking
            context['llm_engine'] = llm_model
            
            # Run repair
            print(f"  Error Type: {context['error_type']}")
            print(f"  File: {context['experiment_file']}")
            
            result = engine.repair(
                code_snippet=context['code_snippet'],
                error_type=context['error_type'],
                error_location=context['error_location'],
                llm_model=llm_model,
                validation_context=context
            )
            
            # Export repair history under experiment directory
            export_base_dir = Path(context.get('experiment_dir', '.'))
            export_dir = export_base_dir / 'repair_results'
            export_dir.mkdir(exist_ok=True, parents=True)
            issue_id = result['issue_id']
            export_file = export_dir / f'{issue_id}_history.json'
            if engine.export_repair_history(issue_id, str(export_file)):
                print(f"  Exported repair history to: {export_file}")
            
            # Store result
            result['experiment_dir'] = experiment_dir
            result['experiment_file'] = context['experiment_file']
            results.append(result)
            
            print(f"  Result: {'Success' if result['success'] else 'Failed'}")
            print(f"  Iterations: {result['iterations']}")
            print(f"  Time: {result['total_time']:.2f}s")
            
        except Exception as e:
            print(f"  Error processing scenario: {e}")
            import traceback
            traceback.print_exc()
            results.append({
                'experiment_dir': experiment_dir,
                'success': False,
                'error': str(e)
            })
    
    # Summary
    print("\n" + "=" * 70)
    print("BATCH PROCESSING SUMMARY")
    print("=" * 70)
    successful = sum(1 for r in results if r.get('success', False))
    print(f"Total scenarios: {len(results)}")
    print(f"Successful repairs: {successful}")
    print(f"Failed repairs: {len(results) - successful}")
    print(f"Success rate: {successful/len(results)*100:.1f}%" if results else "0%")
    
    return results


if __name__ == "__main__":
    import argparse
    
    parser = argparse.ArgumentParser(
        description='ICAR-XR: Iterative Context-Augmented Repair Framework for XR'
    )
    parser.add_argument('--mode', choices=['demo', 'batch'], default='demo',
                       help='Run mode: demo or batch')
    parser.add_argument('--llm', choices=['gpt-4-turbo', 'gpt-5.4', 'gemini-3-pro-preview'], default='gpt-4-turbo',
                       help='LLM model to use')
    parser.add_argument('--scenario', type=str, default=None,
                       help='Path to scenario.json file (for demo mode with real experiment data)')
    parser.add_argument('--experiment-root', type=str, default='./experiment',
                       help='Root directory containing experiment folders (for batch mode)')
    
    args = parser.parse_args()
    
    if args.mode == 'batch':
        batch_process_experiments(
            experiment_root=args.experiment_root,
            llm_model=args.llm
        )
    else:
        example_repair_workflow(scenario_path=args.scenario, llm_model=args.llm)
