import json
import os
import time
from typing import List, Optional, Sequence


def generate_LLM_experiment_responses(
    root_dir: str,
    instruct_head: str,
    contents: str,
    short_contents: str,
    experiment_filename: str,
    temperature: float,
    top_p: float,
    llm_engine: Sequence[str],
    n_choices: int,
    skip_engines: Optional[Sequence[str]] = None,
    max_tokens: int = 4096,
    unique_tag: Optional[str] = None,
    llm_interface=None,
) -> List[str]:
    """
    ICAR-XR-local response generator.

    Key differences vs the original experiment pipeline:
    - Never skips generation just because a response file exists.
    - Always writes response files with a unique suffix (timestamp_ns + optional unique_tag).
    - Returns the list of written response file paths (one per engine that ran).

    Notes:
    - If `llm_interface` is provided and has `.generate(prompt, model, temperature, max_tokens)`,
      it will be used for generation. Otherwise, the function will write the prompt files and
      return an empty list (caller can decide how to handle).
    """
    skip_engines = set(skip_engines or [])

    llm_responses_dir = os.path.join(
        root_dir,
        "response",
        experiment_filename + ".llm_responses",
    )
    os.makedirs(llm_responses_dir, exist_ok=True)

    # Always persist the prompt snapshot for traceability.
    prompt_file = os.path.join(llm_responses_dir, "prompt.txt")
    with open(prompt_file, "w", encoding="utf-8") as f:
        f.write(contents)

    ts_ns = time.time_ns()
    tag = f"_{unique_tag}" if unique_tag else ""
    written_files: List[str] = []

    for engine in llm_engine:
        if engine in skip_engines:
            continue

        # Unique, non-overwriting output name.
        response_filename = (
            f"{engine}.temp-{temperature:.2f}.top_p-{top_p:.2f}.n-{n_choices}{tag}.{ts_ns}.response.json"
        )
        response_path = os.path.join(llm_responses_dir, response_filename)

        actual_prompt_filename = f"actual_prompt{tag}.{ts_ns}.txt"
        actual_prompt_path = os.path.join(llm_responses_dir, actual_prompt_filename)

        prompt_text = contents
        data_json_str: Optional[str] = None

        if llm_interface is not None:
            # Try full prompt first; if token issues happen, fall back to short prompt.
            try:
                resp = llm_interface.generate(
                    prompt=f"{instruct_head}\n{prompt_text}" if instruct_head else prompt_text,
                    model=engine,
                    temperature=temperature,
                    max_tokens=max_tokens,
                )
                # Try to normalize into a response.json-like structure with "choices".
                # We keep it very lightweight and compatible with downstream parsing in stage1.
                if isinstance(resp, dict) and "content" in resp and isinstance(resp["content"], tuple):
                    # Some interfaces return {'content': (status_code, json_string)}
                    data_json_str = resp["content"][1]
                elif isinstance(resp, str):
                    data_json_str = json.dumps(
                        {"choices": [{"message": {"content": resp}}]},
                        ensure_ascii=False,
                    )
                else:
                    data_json_str = json.dumps(
                        {"choices": [{"message": {"content": str(resp)}}]},
                        ensure_ascii=False,
                    )
            except Exception:
                # Fall back to short prompt once.
                try:
                    prompt_text = short_contents or contents
                    resp = llm_interface.generate(
                        prompt=f"{instruct_head}\n{prompt_text}" if instruct_head else prompt_text,
                        model=engine,
                        temperature=temperature,
                        max_tokens=max_tokens,
                    )
                    if isinstance(resp, dict) and "content" in resp and isinstance(resp["content"], tuple):
                        data_json_str = resp["content"][1]
                    elif isinstance(resp, str):
                        data_json_str = json.dumps(
                            {"choices": [{"message": {"content": resp}}]},
                            ensure_ascii=False,
                        )
                    else:
                        data_json_str = json.dumps(
                            {"choices": [{"message": {"content": str(resp)}}]},
                            ensure_ascii=False,
                        )
                except Exception:
                    data_json_str = None

        if not data_json_str:
            # Nothing generated; do not create a fake response file.
            continue

        with open(response_path, "w", encoding="utf-8") as f:
            try:
                parsed = json.loads(data_json_str)
                f.write(json.dumps(parsed, indent=4, ensure_ascii=False))
            except Exception:
                # If it's not valid JSON, still persist it for debugging.
                f.write(data_json_str)

        with open(actual_prompt_path, "w", encoding="utf-8") as f:
            f.write(prompt_text)

        written_files.append(response_path)

    return written_files

