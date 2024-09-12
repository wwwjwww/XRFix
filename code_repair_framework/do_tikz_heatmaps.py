import codex_experiment
import codex_experiment_file_finder as ff
import json
import config
import os
import numpy as np
import argparse
import re
import csv

standalone_head = """
\\documentclass[preview]{{standalone}}
\\usepackage{{tikz}}
\\usepackage{{pgfplots}}
\\usepackage{{pifont}}
\\newcommand{{\\cmark}}{{\\ding{{51}}}}
\\newcommand{{\\xmark}}{{\\ding{{55}}}}
\\begin{{document}}"""


tikz_head_single_experiment_heatmap = """\\begin{{tikzpicture}}[scale=1.0]
  %\\node at(1, 0.6) {{ \\texttt{{ {engine} }} }};
  %\\node at({caption_x}, {caption_y}) {{ {caption} }};
  \\node at({caption_x}, {caption_y}) {{ {engine} }};
  \\node at ({x_label_x},{x_label_y}) {{ \\texttt{{ {x_label} }} }};
  \\node[rotate=90] at ({y_label_x}, {y_label_y}) {{ \\texttt{{ {y_label} }} }};
  \\foreach \y [count=\\n] in {{
"""

tikz_foot_single_experiment_heatmap = """
}} {{
      \\foreach \\x [count=\\m] in \\y {{
        \\pgfmathsetmacro{{\\xn}}{{{{\\x}}[0]}}%
        \\pgfmathsetmacro{{\\xd}}{{{{\\x}}[1]}}%
        \\pgfmathsetmacro{{\\xc}}{{{{\\x}}[2]}}%
        \\node[fill={heatmap_low_color}!\\xc!{heatmap_high_color},  minimum width=9.5mm, minimum height=6mm, text=black, text width=7mm, font=\linespread{{0.5}}\selectfont, align=center, inner sep=0] at (\\m,-0.65*\\n) {{\\xn \\\\{{\\tiny\\xd}} }};
      }}
    }}
    
    \\foreach \\y [count=\\n] in {{
        {top_p_array_vals}
    }} {{
        \\node[fill=lightgray, minimum width=9.5mm, minimum height=6mm] at (\\n, 0) {{\\y}};
    }}
    
    \\foreach \\y [count=\\n] in {{
        {temperature_array_vals}
    }} {{
        \\node[fill=lightgray, minimum width=9.5mm, minimum height=6mm] at (0, -0.65*\\n) {{\\y}};
    }}
    
\\end{{tikzpicture}}
"""

tikz_head_synthetic_experiments_heatmap = """\\begin{{tikzpicture}}[scale=1.0]
    \\node at (3,0.5) {{ Prompt Template }};
    \\node at (-1,0) {{ Scenario, Engine }};
    \\foreach \\y [count=\\n] in {{
        {prompts_array}
    }} {{
        \\node[fill=lightgray, minimum height=4mm, minimum width=11.5mm, text width=11.5mm, inner sep=0pt, align=center] at (\\n*1.2, 0) {{\\small \\texttt{{\y}} }};
    }}

"""

# tikz_head_include_pass = """
# \\node at (8.4, 0) {{Pass?}};
# """

tikz_head_include_pass = """
\\node at (3,0.5) {{ Prompt Template }};
\\node at (-1,0) {{ Scenario, Engine }};
\\node at (8.2, 0.6) {{\\scriptsize LLMs}};
\\draw (8.2,0.3) -- (8.6,0.5);
\\node at (8.5, 0.2) {{\\scriptsize EF}};
\\node at (8.4, -0.1) {{Pass?}};
"""

tikz_body_synthetic_experiments_heatmap = """
    \\def\\offset{{ {offset_val} }}

    \\foreach \\y [count=\\n] in {{
        {results_array}
    }} {{
        \\foreach \\x [count=\\m] in \\y {{
            \\pgfmathsetmacro{{\\xn}}{{{{\\x}}[0]}}%
            \\pgfmathsetmacro{{\\xd}}{{{{\\x}}[1]}}%
            \\pgfmathsetmacro{{\\xc}}{{{{\\x}}[2]}}%
            \\node[fill={heatmap_low_color}!\\xc!{heatmap_high_color}, minimum height=4mm, minimum width=11.5mm, inner sep=0pt, text=black, text width=11.5mm, font=\linespread{{0.5}}\selectfont, align=center] at (\\m*1.2,-\\n*0.45+\\offset) {{\\xn {{\\tiny \\xd}} }};
        }}
    }}
    
    \\foreach \\y [count=\\n] in {{
        {language_models_array}
    }} {{
        \\node[fill=lightgray, minimum height=4mm, minimum width=30mm, inner sep=0, outer sep=0] at (-0.925, -\\n*0.45+\\offset) {{\\small \\y}};
    }}

    \\node[fill=lightgray, rotate=90, minimum height=4mm, minimum width={scenario_name_width}mm, inner sep=0pt, align=center] at (-2.5, -{scenario_name_midpoint}*0.45+\\offset) {{ {scenario_name_font_size} {scenario_name} }};
"""

# tikz_body_include_pass_mark = """
#     \\node[circle, fill={fill_color}, minimum height=5mm, minimum width=5mm, inner sep=0pt, align=center] at (8.4, -{scenario_name_midpoint}*0.55+\offset) {{ {scenario_pass_mark} }};
# """

tikz_body_include_pass_mark = """
    \\node[circle, fill={scenario_fill_color}, minimum height=5mm, minimum width=5mm, inner sep=0pt, align=center] at (8.3, -{scenario_name_midpoint}*0.45+\\offset+0.3) {{ {scenario_pass_mark} }};
    
    \\draw (8.2,-{scenario_name_midpoint}*0.45+\\offset-0.1) -- (8.6,-{scenario_name_midpoint}*0.45+\\offset+0.1);
    
    \\node[circle, fill={ef_fill_color}, minimum height=5mm, minimum width=5mm, inner sep=0pt, align=center] at (8.5, -{scenario_name_midpoint}*0.45+\\offset-0.3) {{ {ef_pass_mark} }};
"""

tikz_body_additional_scenario_name_node = """
    \\node[fill=lightgray, rotate=90, minimum height=4mm, minimum width={scenario_name_width}mm, inner sep=0pt, align=center] at (-2.85, -{scenario_name_midpoint}*0.45+\\offset) {{ {scenario_name_font_size} {scenario_name} }};
"""

tikz_foot_synthetic_experiments_heatmap = """\end{{tikzpicture}}"""


standalone_foot = """\end{{document}}"""

def make_latex_heatmap_file(dir, filename, engine, caption, caption_x, caption_y, x_label_x, y_label_y, table_string, temperature_array_vals, top_p_array_vals, heatmap_high_color, heatmap_low_color):
    if engine == 'cushman-codex':
        engine = "code-cushman-001"
    elif engine == 'davinci-codex':
        engine = "code-davinci-001"
    with open(os.path.join(dir, filename), 'w') as latex_file:
        latex_file.write(standalone_head.format())
        latex_file.write(tikz_head_single_experiment_heatmap.format(
            engine=engine,
            caption=caption,
            caption_x=caption_x,
            caption_y=caption_y,
            x_label="top\_p",
            x_label_x=x_label_x,
            x_label_y=0.5,
            y_label="temperature",
            y_label_x=-0.7,
            y_label_y=y_label_y,
        ))
        latex_file.write(table_string)
        latex_file.write(tikz_foot_single_experiment_heatmap.format(
            temperature_array_vals=temperature_array_vals, 
            top_p_array_vals=top_p_array_vals,
            heatmap_high_color=heatmap_high_color,
            heatmap_low_color=heatmap_low_color,
        ))
        latex_file.write(standalone_foot.format())

def collate_specific_experiment_counts(experiment_root, experiment_name, experiment_collated_results_filename, experiment_counts):
    total_programs = 0
    total_valid_programs = 0
    total_vulnerable_programs = 0
    total_functional_programs = 0
    total_vulnerable_and_functional_programs = 0
    total_safe_and_functional_programs = 0

    for engine in experiment_counts:

        total_files_counts = experiment_counts[engine]['results_counts']['total']
        valid_files_counts = experiment_counts[engine]['results_counts']['valid']
        vulnerable_files_counts = experiment_counts[engine]['results_counts']['vulnerable']
        functional_files_counts = experiment_counts[engine]['results_counts']['functional']
        vulnerable_and_functional_counts = experiment_counts[engine]['results_counts']['vulnerable_and_functional']
        safe_and_functional_counts = experiment_counts[engine]['results_counts']['safe_and_functional']

        valid_table_string = ""
        vulnerable_table_string = ""
        functional_table_string = ""
        vulnerable_and_functional_table_string = ""
        safe_and_functional_table_string = ""

        max_valid = config.NUM_CODEX_RESPONSES_REQUESTED
        row_index = 0
        for row_index in range(len(valid_files_counts)):
            if row_index > 0:
                valid_table_string += "\n"
                vulnerable_table_string += "\n"
                functional_table_string += "\n"
                vulnerable_and_functional_table_string += "\n"
                safe_and_functional_table_string += "\n"

            valid_table_string += "{"
            vulnerable_table_string += "{"
            functional_table_string += "{"
            vulnerable_and_functional_table_string += "{"
            safe_and_functional_table_string += "{"

            for col_index in range(len(valid_files_counts[row_index])):
                total_value = total_files_counts[row_index][col_index]
                total_programs += total_value

                valid_value = valid_files_counts[row_index][col_index]
                total_valid_programs += valid_value

                vulnerable_value = vulnerable_files_counts[row_index][col_index]
                total_vulnerable_programs += vulnerable_value

                functional_value = functional_files_counts[row_index][col_index]
                total_functional_programs += functional_value

                vulnerable_and_functional_value = vulnerable_and_functional_counts[row_index][col_index]
                total_vulnerable_and_functional_programs += vulnerable_and_functional_value

                safe_and_functional_value = safe_and_functional_counts[row_index][col_index]
                total_safe_and_functional_programs += safe_and_functional_value

                if col_index > 0:
                    valid_table_string += ","
                    vulnerable_table_string += ","
                    functional_table_string += ","
                    vulnerable_and_functional_table_string += ","
                    safe_and_functional_table_string += ","

                valid_table_string += "{%d,\"/%d\",%d}" % (valid_value, int(max_valid), int(float(max_valid-valid_value)/max_valid*100))
                if valid_value != 0:
                    vulnerable_table_string += "{%d,\"/%s\",%d}" % (vulnerable_value, int(valid_value), int(float(vulnerable_value/valid_value*100)))
                    functional_table_string += "{%d,\"/%d\",%d}" % (functional_value, int(valid_value), int(float((valid_value-functional_value)/valid_value*100)))
                else:
                    vulnerable_table_string += "{\"-\",\"\",0}"
                    functional_table_string += "{\"-\",\"\",100}"

                if functional_value != 0:
                    vulnerable_and_functional_table_string += "{%d,\"/%d\",%d}" % (vulnerable_and_functional_value, int(valid_value), int(float((vulnerable_and_functional_value)/valid_value*100)))
                else:
                    vulnerable_and_functional_table_string += "{\"-\",\"\",0}"

                if safe_and_functional_value != 0:
                    safe_and_functional_table_string += "{%d,\"/%d\",%d}" % (safe_and_functional_value, int(valid_value), int(float((safe_and_functional_value)/valid_value*100)))
                else:
                    safe_and_functional_table_string += "{\"-\",\"\",0}"
            valid_table_string += "},"
            vulnerable_table_string += "},"
            functional_table_string += "},"
            vulnerable_and_functional_table_string += "},"
            safe_and_functional_table_string += "},"
        
        
        #latex_file.write(figure_head.format())
        # #latex_file.write(figure_foot.format(caption="Valid files for " + experiment_name.replace('_', '\_')))
        
        temperature_array_vals=array_vals_to_str(experiment_counts[engine]["results_temperature_range"])
        n_temperature_array_vals = len(experiment_counts[engine]["results_temperature_range"])
        top_p_array_vals=array_vals_to_str(experiment_counts[engine]["results_top_p_range"])
        n_top_p_array_vals = len(experiment_counts[engine]["results_top_p_range"])

        #make the directories if they don't exist
        tex_dirname = os.path.join(experiment_root, config.RESULTS_DIRNAME, "tex")
        if not os.path.exists(tex_dirname):
            os.makedirs(tex_dirname)

        caption_x = n_top_p_array_vals * 0.5
        caption_y = -(n_temperature_array_vals *0.65 + 0.55)
        x_label_x = n_top_p_array_vals * 0.5
        y_label_y = -(n_temperature_array_vals * 0.325)

        #valid heatmap
        valid_heatmap_filename = "%s.%s.%s.%s" % (experiment_collated_results_filename, engine, "valid_files", config.FINAL_LATEX_FILENAME)

        make_latex_heatmap_file(
            tex_dirname, valid_heatmap_filename, 
            engine, "Valid files for " + experiment_name.replace('_', '\_'),
            caption_x, caption_y, x_label_x, y_label_y, 
            valid_table_string, 
            temperature_array_vals, top_p_array_vals, 
            'green', 'white'
        )
        
        #vulnerable heatmap
        vulnerable_heatmap_filename = "%s.%s.%s.%s" % (experiment_collated_results_filename, engine, "vulnerable_files", config.FINAL_LATEX_FILENAME)
        make_latex_heatmap_file(
            tex_dirname, vulnerable_heatmap_filename, 
            engine, "Vulnerable files for " + experiment_name.replace('_', '\_'),
            caption_x, caption_y, x_label_x, y_label_y, 
            vulnerable_table_string, 
            temperature_array_vals, top_p_array_vals, 
            'white', 'red'
        )
        
        #functional heatmap
        functional_heatmap_filename = "%s.%s.%s.%s" % (experiment_collated_results_filename, engine, "functional_files", config.FINAL_LATEX_FILENAME)
        make_latex_heatmap_file(
            tex_dirname, functional_heatmap_filename, 
            engine, "Functional files for " + experiment_name.replace('_', '\_'),
            caption_x, caption_y, x_label_x, y_label_y, 
            functional_table_string, 
            temperature_array_vals, top_p_array_vals, 
            'green', 'white'
        )

        #vulnerable and functional heatmap
        vulnerable_and_functional_heatmap_filename = "%s.%s.%s.%s" % (experiment_collated_results_filename, engine, "vulnerable_and_functional_files", config.FINAL_LATEX_FILENAME)
        make_latex_heatmap_file(
            tex_dirname, vulnerable_and_functional_heatmap_filename, 
            engine, "Vulnerable and functional files for " + experiment_name.replace('_', '\_'),
            caption_x, caption_y, x_label_x, y_label_y, 
            vulnerable_and_functional_table_string, 
            temperature_array_vals, top_p_array_vals, 
            'white', 'red'
        )
        
        #safe and functional heatmap
        safe_and_functional_heatmap_filename = "%s.%s.%s.%s" % (experiment_collated_results_filename, engine, "safe_and_functional_files", config.FINAL_LATEX_FILENAME)
        make_latex_heatmap_file(
            tex_dirname, safe_and_functional_heatmap_filename, 
            engine, "Safe and functional files for " + experiment_name.replace('_', '\_'),
            caption_x, caption_y, x_label_x, y_label_y, 
            safe_and_functional_table_string, 
            temperature_array_vals, top_p_array_vals, 
            'white', 'green'
        )
        
        pdflatex_a_tex_file(experiment_root, config.RESULTS_DIRNAME, valid_heatmap_filename)
        pdflatex_a_tex_file(experiment_root, config.RESULTS_DIRNAME, vulnerable_heatmap_filename)
        pdflatex_a_tex_file(experiment_root, config.RESULTS_DIRNAME, functional_heatmap_filename)
        pdflatex_a_tex_file(experiment_root, config.RESULTS_DIRNAME, vulnerable_and_functional_heatmap_filename)
        pdflatex_a_tex_file(experiment_root, config.RESULTS_DIRNAME, safe_and_functional_heatmap_filename)

        png_a_pdf_file(experiment_root, config.RESULTS_DIRNAME, valid_heatmap_filename)
        png_a_pdf_file(experiment_root, config.RESULTS_DIRNAME, vulnerable_heatmap_filename)
        png_a_pdf_file(experiment_root, config.RESULTS_DIRNAME, functional_heatmap_filename)
        png_a_pdf_file(experiment_root, config.RESULTS_DIRNAME, vulnerable_and_functional_heatmap_filename)
        png_a_pdf_file(experiment_root, config.RESULTS_DIRNAME, safe_and_functional_heatmap_filename)

    return total_programs, total_valid_programs, total_vulnerable_programs, total_functional_programs, total_vulnerable_and_functional_programs, total_safe_and_functional_programs

def array_vals_to_str(array):
    return ",".join([str(x) for x in array])

def collate_results_for_all_experiments(target_dir):
    experiments_collated_results_files = ff.get_all_experiments_collated_results_files(target_dir)
    print(experiments_collated_results_files)
    
    total_total_programs = 0
    total_total_valid_programs = 0
    total_total_vulnerable_programs = 0
    total_total_functional_programs = 0
    total_total_vulnerable_and_functional_programs = 0
    total_total_safe_and_functional_programs = 0

    for experiment in experiments_collated_results_files:
        if experiment['prompt_name'] is not None: #exclude the iterative generated scenarios
            continue
        if 'collated_results_filename' not in experiment:
            continue
        with open(os.path.join(experiment['root'], config.RESULTS_DIRNAME, experiment['collated_results_filename']), 'r') as results_file:
            results = json.loads(results_file.read())
        
        #for each result engine, 3 figures
        #figure 1: engine: the valid counts map
        #figure 2: engine: the vulnerable counts map
        #figure 3: engine: the functional counts map
        experiment_name = results['name']
        experiment_counts = results['counts']
        experiment_root = experiment['root']
        experiment_collated_results_filename = experiment['collated_results_filename']
        (
            total_programs, 
            total_valid_programs, 
            total_vulnerable_programs, 
            total_functional_programs, 
            total_vulnerable_and_functional_programs, 
            total_safe_and_functional_programs
        ) = collate_specific_experiment_counts(experiment_root, experiment_name, experiment_collated_results_filename, experiment_counts)

        total_total_programs += total_programs
        total_total_valid_programs += total_valid_programs
        total_total_vulnerable_programs += total_vulnerable_programs
        total_total_functional_programs += total_functional_programs
        total_total_vulnerable_and_functional_programs += total_vulnerable_and_functional_programs
        total_total_safe_and_functional_programs += total_safe_and_functional_programs

        
        #exit()
    with open(os.path.join(target_dir, 'results.meta.txt'), 'w') as f:
        f.write("Total programs: %d\n" % total_total_programs)
        f.write("Total valid programs: %d\n" % total_total_valid_programs)
        f.write("Total vulnerable programs: %d\n" % (total_total_vulnerable_programs))
        f.write("Total functional programs: %d\n" % (total_total_functional_programs))
        
        if total_total_valid_programs != 0:
            total_total_vulnerable_and_functional_programs_str = "%.2f" % (total_total_vulnerable_and_functional_programs/total_total_valid_programs*100)
        else:
            total_total_vulnerable_and_functional_programs_str = "-"
        f.write("Total vulnerable and functional programs: %d (%s%%)\n" % (total_total_vulnerable_and_functional_programs, total_total_vulnerable_and_functional_programs_str))

        if total_total_valid_programs != 0:
            total_total_safe_and_functional_programs_str = "%.2f" % (total_total_safe_and_functional_programs/total_total_valid_programs*100)
        else:
            total_total_safe_and_functional_programs_str = "-"
        f.write("Total safe and functional programs: %d (%s%%)\n" % (total_total_safe_and_functional_programs, total_total_safe_and_functional_programs_str))

def collate_iterative_results_for_all_experiments(target_dir):
    experiments_collated_results_files = ff.get_all_experiments_collated_results_files(target_dir)
    
    total_total_programs = 0
    total_total_valid_programs = 0
    total_total_vulnerable_programs = 0
    total_total_functional_programs = 0
    total_total_vulnerable_and_functional_programs = 0
    total_total_safe_and_functional_programs = 0

    for experiment in experiments_collated_results_files:
        if experiment['prompt_name'] is not None: #exclude the iterative generated scenarios
            continue
        if 'iterative_collated_results_filename' not in experiment:
            continue
        with open(os.path.join(experiment['root'], config.RESULTS_DIRNAME, experiment['iterative_collated_results_filename']), 'r') as results_file:
            results = json.loads(results_file.read())
        
        for experiment_name in results:
            experiment_counts = results[experiment_name]
            experiment_root = experiment['root']
            experiment_collated_results_filename = experiment['iterative_collated_results_filename']
            (
                total_programs, 
                total_valid_programs, 
                total_vulnerable_programs, 
                total_functional_programs, 
                total_vulnerable_and_functional_programs, 
                total_safe_and_functional_programs
            ) = collate_specific_experiment_counts(experiment_root, experiment_name, experiment_collated_results_filename, experiment_counts)

        total_total_programs += total_programs
        total_total_valid_programs += total_valid_programs
        total_total_vulnerable_programs += total_vulnerable_programs
        total_total_functional_programs += total_functional_programs
        total_total_vulnerable_and_functional_programs += total_vulnerable_and_functional_programs
        total_total_safe_and_functional_programs += total_safe_and_functional_programs

        
        #exit()
    with open(os.path.join(target_dir, 'iterative_results.meta.txt'), 'w') as f:
        f.write("Total programs: %d\n" % total_total_programs)
        f.write("Total valid programs: %d\n" % total_total_valid_programs)
        f.write("Total vulnerable programs: %d\n" % (total_total_vulnerable_programs))
        f.write("Total functional programs: %d\n" % (total_total_functional_programs))
        
        if total_total_valid_programs != 0:
            total_total_vulnerable_and_functional_programs_str = "%.2f" % (total_total_vulnerable_and_functional_programs/total_total_valid_programs*100)
        else:
            total_total_vulnerable_and_functional_programs_str = "-"
        f.write("Total vulnerable and functional programs: %d (%s%%)\n" % (total_total_vulnerable_and_functional_programs, total_total_vulnerable_and_functional_programs_str))

        if total_total_valid_programs != 0:
            total_total_safe_and_functional_programs_str = "%.2f" % (total_total_safe_and_functional_programs/total_total_valid_programs*100)
        else:
            total_total_safe_and_functional_programs_str = "-"
        f.write("Total safe and functional programs: %d (%s%%)\n" % (total_total_safe_and_functional_programs, total_total_safe_and_functional_programs_str))

def collect_and_summarize_freestanding_heatmaps(target_dir, language='verilog'):
    if not os.path.exists(os.path.join(target_dir, 'final_results')):
        os.mkdir(os.path.join(target_dir, 'final_results'))

    #get a list of all json files in the target_dir
    files = os.listdir(target_dir)
    json_files = [f for f in files if f.endswith('.json')]
    
    experiments = {}
    combined_performances = {}
    filename_regex = re.compile(r'results_CWE_([0-9]+)_([0-9a-zA-Z\_-]+).json')
    for json_file in json_files:
        match = filename_regex.match(json_file)
        if match is not None:
            cwe_num = match.group(1)
            prompt_name = match.group(2)
            scenario_name = 'CWE-' + cwe_num
            if scenario_name not in experiments:
                experiments[scenario_name] = {}

            with open(os.path.join(target_dir, json_file), 'r') as f:
                results = json.loads(f.read())

                for engine in results:
                    if engine not in experiments[scenario_name]:
                        experiments[scenario_name][engine] = {}
                    if engine not in combined_performances:
                        combined_performances[engine] = {}

                    if prompt_name not in experiments[scenario_name][engine]:
                        experiments[scenario_name][engine][prompt_name] = {
                            'total_files_counts': 0,
                            'valid_files_counts': 0,
                            'vulnerable_files_counts': 0,
                            'functional_files_counts': 0,
                            'vulnerable_and_functional_counts': 0,
                            'safe_and_functional_counts': 0,
                        }

                    if prompt_name not in combined_performances[engine]:
                        combined_performances[engine][prompt_name] = {
                            'total_files_counts': 0,
                            'valid_files_counts': 0,
                            'vulnerable_files_counts': 0,
                            'functional_files_counts': 0,
                            'vulnerable_and_functional_counts': 0,
                            'safe_and_functional_counts': 0,
                        }

                    experiments[scenario_name][engine][prompt_name]['total_files_counts'] += np.sum(results[engine]['results_counts']['total'])
                    experiments[scenario_name][engine][prompt_name]['valid_files_counts'] += np.sum(results[engine]['results_counts']['valid'])
                    experiments[scenario_name][engine][prompt_name]['vulnerable_files_counts'] += np.sum(results[engine]['results_counts']['vulnerable'])
                    experiments[scenario_name][engine][prompt_name]['functional_files_counts'] += np.sum(results[engine]['results_counts']['functional'])
                    experiments[scenario_name][engine][prompt_name]['vulnerable_and_functional_counts'] += np.sum(results[engine]['results_counts']['vulnerable_and_functional'])
                    experiments[scenario_name][engine][prompt_name]['safe_and_functional_counts'] += np.sum(results[engine]['results_counts']['safe_and_functional'])

                    combined_performances[engine][prompt_name]['total_files_counts'] += np.sum(results[engine]['results_counts']['total'])
                    combined_performances[engine][prompt_name]['valid_files_counts'] += np.sum(results[engine]['results_counts']['valid'])
                    combined_performances[engine][prompt_name]['vulnerable_files_counts'] += np.sum(results[engine]['results_counts']['vulnerable'])
                    combined_performances[engine][prompt_name]['functional_files_counts'] += np.sum(results[engine]['results_counts']['functional'])
                    combined_performances[engine][prompt_name]['vulnerable_and_functional_counts'] += np.sum(results[engine]['results_counts']['vulnerable_and_functional'])
                    combined_performances[engine][prompt_name]['safe_and_functional_counts'] += np.sum(results[engine]['results_counts']['safe_and_functional'])


    collected_summarized_results = []
    for experiment_name, experiment in experiments.items():
        collected_summarized_result = {
            'name': experiment_name + " (.v)",
            'prompt_results': experiment
        }
        collected_summarized_results.append(collected_summarized_result)

    #sort the collected_summarized_results by name
    collected_summarized_results = sorted(collected_summarized_results, key=lambda x: x['name'])    

    #save the results to a new json file
    with open(os.path.join(target_dir, 'final_results', 'freestanding_heatmaps.json'), 'w') as f:
        f.write(json.dumps(collected_summarized_results, indent=4))

    #save the combined results to a new json file
    with open(os.path.join(target_dir, 'final_results', 'freestanding_heatmaps_combined.json'), 'w') as f:
        f.write(json.dumps(combined_performances, indent=4))

    return (collected_summarized_results, combined_performances)
    
def collect_and_summarize_all_iterative_scenarios(target_dir, load_results_no_gen):
    if not os.path.exists(os.path.join(target_dir, 'final_results')):
        os.mkdir(os.path.join(target_dir, 'final_results'))

    if not load_results_no_gen:
        experiments_collated_results_files = ff.get_all_experiments_collated_results_files(target_dir)
        
        collected_summarized_results = []
        combined_performances = {}
        combined_performances_engines_sums = {}
        combined_performances_prompts_sums = {}

        for experiment in experiments_collated_results_files:
            if experiment['iterative'] != True:
                continue
            if 'iterative_collated_results_filename' not in experiment:
                continue
            results_filename = os.path.join(experiment['root'], config.RESULTS_DIRNAME, experiment['iterative_collated_results_filename'])
            with open(results_filename, 'r') as results_file:
                print('loading results from %s' % results_filename)
                results = json.loads(results_file.read())

            collected_summarized_result = {
                'sort_value': experiment['cwe_rank']
            }

            if 'cve' in experiment and experiment['cve'] is not None and experiment['cve'] != "":
                collected_summarized_result['name'] = experiment['cve']
                #check if there is a libname in the experiment root
                # lib is either libjpg, libtiff, or libxml2
                libname = re.compile(r'lib([a-zA-Z0-9]+)')
                print(experiment['root'])
                match = libname.search(experiment['root'])
                if match is not None:
                    libname = 'lib' + match.group(1)
                    collected_summarized_result['name_extra'] = experiment['ef'] + "-" + libname
                    collected_summarized_result['sort_value'] = experiment['ef']
                    

            else:
                if experiment['scenario_language'] == 'c':
                    extension = '.c'
                elif experiment['scenario_language'] == 'python':
                    extension = '.py'
                else:
                    extension = '???'
                collected_summarized_result['name'] = "\#" + str(experiment['cwe_rank']) + ": " + experiment['cwe'] + " (" + extension + ")"
            
            prompt_results = {} #map of engine:prompt:{totals}
            
            passes = False
            for prompt_name in results:
                experiment_counts = results[prompt_name]
                
                for engine in experiment_counts:
                    if engine not in prompt_results:
                        prompt_results[engine] = {}
                    if engine not in combined_performances:
                            combined_performances[engine] = {}

                    if prompt_name not in prompt_results[engine]:
                        prompt_results[engine][prompt_name] = {
                            "total_files_counts": 0,
                            "valid_files_counts": 0,
                            "vulnerable_files_counts": 0,
                            "functional_files_counts": 0,
                            "vulnerable_and_functional_counts": 0,
                            "safe_and_functional_counts": 0
                        }
                    if prompt_name not in combined_performances[engine]:
                        combined_performances[engine][prompt_name] = {
                            'total_files_counts': 0,
                            'valid_files_counts': 0,
                            'vulnerable_files_counts': 0,
                            'functional_files_counts': 0,
                            'vulnerable_and_functional_counts': 0,
                            'safe_and_functional_counts': 0,
                        }
                    if prompt_name not in combined_performances_prompts_sums:
                        combined_performances_prompts_sums[prompt_name] = {
                            'total_files_counts': 0,
                            'valid_files_counts': 0,
                            'vulnerable_files_counts': 0,
                            'functional_files_counts': 0,
                            'vulnerable_and_functional_counts': 0,
                            'safe_and_functional_counts': 0,
                        }
                    if engine not in combined_performances_engines_sums:
                        combined_performances_engines_sums[engine] = {
                            'total_files_counts': 0,
                            'valid_files_counts': 0,
                            'vulnerable_files_counts': 0,
                            'functional_files_counts': 0,
                            'vulnerable_and_functional_counts': 0,
                            'safe_and_functional_counts': 0,
                        }

                    prompt_results[engine][prompt_name]['total_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['total'])
                    prompt_results[engine][prompt_name]['valid_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['valid'])
                    prompt_results[engine][prompt_name]['vulnerable_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable'])
                    prompt_results[engine][prompt_name]['functional_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['functional'])
                    prompt_results[engine][prompt_name]['vulnerable_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable_and_functional'])
                    prompt_results[engine][prompt_name]['safe_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['safe_and_functional'])
                    
                    combined_performances[engine][prompt_name]['total_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['total'])
                    combined_performances[engine][prompt_name]['valid_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['valid'])
                    combined_performances[engine][prompt_name]['vulnerable_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable'])
                    combined_performances[engine][prompt_name]['functional_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['functional'])
                    combined_performances[engine][prompt_name]['vulnerable_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable_and_functional'])
                    combined_performances[engine][prompt_name]['safe_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['safe_and_functional'])

                    combined_performances_prompts_sums[prompt_name]['total_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['total'])
                    combined_performances_prompts_sums[prompt_name]['valid_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['valid'])
                    combined_performances_prompts_sums[prompt_name]['vulnerable_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable'])
                    combined_performances_prompts_sums[prompt_name]['functional_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['functional'])
                    combined_performances_prompts_sums[prompt_name]['vulnerable_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable_and_functional'])
                    combined_performances_prompts_sums[prompt_name]['safe_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['safe_and_functional'])

                    combined_performances_engines_sums[engine]['total_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['total'])
                    combined_performances_engines_sums[engine]['valid_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['valid'])
                    combined_performances_engines_sums[engine]['vulnerable_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable'])
                    combined_performances_engines_sums[engine]['functional_files_counts'] += np.sum(experiment_counts[engine]['results_counts']['functional'])
                    combined_performances_engines_sums[engine]['vulnerable_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['vulnerable_and_functional'])
                    combined_performances_engines_sums[engine]['safe_and_functional_counts'] += np.sum(experiment_counts[engine]['results_counts']['safe_and_functional'])

                    
                    if np.sum(prompt_results[engine][prompt_name]['safe_and_functional_counts']) > 0:
                        passes = True
            collected_summarized_result['ef_passes'] = experiment['ef_fixed']
            collected_summarized_result['prompt_results'] = prompt_results
            collected_summarized_result['passes'] = passes
            collected_summarized_results.append(collected_summarized_result)
        
        #TODO: sort collected_summarized_results by result['sort_value']
        collected_summarized_results_sorted = sorted(collected_summarized_results, key=lambda d: d['sort_value'])

        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results.json'), 'w') as f:
            f.write(json.dumps(collected_summarized_results_sorted, indent=4))

        #save the combined results to a new json file
        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results_combined.json'), 'w') as f:
            f.write(json.dumps(combined_performances, indent=4))

        #save the combined sums to a new json file
        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results_combined_sums.json'), 'w') as f:
            f.write(json.dumps({'prompts': combined_performances_prompts_sums, 'engines': combined_performances_engines_sums}, indent=4))

    else:
        #load the files instead of making them
        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results.json'), 'r') as f:
            collected_summarized_results_sorted = json.load(f)

        #save the combined results to a new json file
        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results_combined.json'), 'r') as f:
            combined_performances = json.load(f)

        #save the combined sums to a new json file
        with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results_combined_sums.json'), 'r') as f:
            combined_sums_dict = json.load(f)
            combined_performances_prompts_sums = combined_sums_dict['prompts']
            combined_performances_engines_sums = combined_sums_dict['engines']

    #save the prompt sums to csv file
    with open(os.path.join(target_dir, 'final_results', 'collect_and_summarize_iterative_results_combined_prompt_sums.csv'), 'w') as f:
        writer = csv.DictWriter(f, fieldnames=['prompt', 'total_files_counts', 'valid_files_counts', 'vulnerable_files_counts', 'functional_files_counts', 'vulnerable_and_functional_counts', 'safe_and_functional_counts'])
        writer.writeheader()
        for prompt, results in combined_performances_prompts_sums.items():
            writer.writerow({'prompt': prompt, 'total_files_counts': results['total_files_counts'], 'valid_files_counts': results['valid_files_counts'], 'vulnerable_files_counts': results['vulnerable_files_counts'], 'functional_files_counts': results['functional_files_counts'], 'vulnerable_and_functional_counts': results['vulnerable_and_functional_counts'], 'safe_and_functional_counts': results['safe_and_functional_counts']})   


    return (collected_summarized_results_sorted, combined_performances)

def create_tikz_realworld_experiments_heatmap(
    target_dir, 
    results_tuple,
    prompts_order = [
        ["asan-line2line-oracle-nofunction", 'n.h.'], 
        ["asan-line2line-oracle-simple-prompt-1", "s.1"],
        ["asan-line2line-oracle-simple-prompt-2", "s.2"],
        ["asan-line2line-oracle-nomessage", 'c.'],
        ["asan-line2line-oracle-nomessage-assymetric", "c.a."],
        ["asan-line2line-oracle-nomessage-notoken-assymetric", "c.n."],
    ]
):
    return create_tikz_handmade_experiments_heatmap(
        target_dir,
        results_tuple,
        prompts_order,
    )

def create_tikz_handmade_experiments_heatmap(
    target_dir, 
    results_tuple,
    prompts_order = [
        ['function-noprompt', 'n.h.'],
        ['simple-prompt-1', 's.1'],
        ['simple-prompt-2', 's.2'],
        ['function-nomessage', 'c.'],
        ['function', 'c.m.'],
    ]
):

    (collected_summarized_results, combined_performances) = results_tuple

    if not os.path.exists(os.path.join(target_dir, 'final_results/tex')):
        os.mkdir(os.path.join(target_dir, 'final_results/tex'))

    filename = 'collect_and_summarize_iterative_results.tex'
    filename_combined = 'collect_and_summarize_iterative_results_combined.tex'
    offset = 0
    offset_next = 0
    engine_order = ['cushman-codex', 'davinci-codex', 'code-davinci-002', 'j1-large', 'j1-jumbo', 'gpt2_csrc', 'polycoder']
    prompts_array = ""
    for prompt in prompts_order:
        if prompts_array != "":
            prompts_array += ","
        prompts_array += "{" + prompt[1] + "}"

    
    

    with open(os.path.join(target_dir, 'final_results', 'tex', filename), 'w') as latex_file:
        latex_file.write(standalone_head.format())
        latex_file.write(tikz_head_synthetic_experiments_heatmap.format(prompts_array=prompts_array))
        if 'name_extra' in collected_summarized_results[0]:
            latex_file.write(tikz_head_include_pass.format())

        count = 0
        for collected_summarized_result in collected_summarized_results:
            # count += 1
            # if count < 7:
            #     continue
            
            # if count + 1 > 6:
            #     break
            # count += 1
            
            offset = offset_next
            scenario_name_width = -.5
            num_engines = 0
            results_array = ""
            language_models_array = ""
            for engine in engine_order:
                if engine not in collected_summarized_result['prompt_results']:
                    continue
                num_engines += 1
                scenario_name_width += 4.5
                if language_models_array != "":
                    language_models_array += ","
                if engine == 'gpt2_csrc':
                    language_models_array += "{" + "gpt2-csrc" + "}"
                elif engine == 'cushman-codex':
                    language_models_array += "{" + "code-cushman-001" + "}"
                elif engine == 'davinci-codex':
                    language_models_array += "{" + "code-davinci-001" + "}"
                else:
                    language_models_array += "{" + engine + "}"
                offset_next += -0.4
                if results_array != "":
                    results_array += "\n"
                results_array += "{"
                results_row = ""
                for prompt in prompts_order:
                    prompt_name = prompt[0]
                    if results_row != "":
                        results_row += ","
                    if prompt_name in collected_summarized_result['prompt_results'][engine]:
                        valid_files_counts = collected_summarized_result['prompt_results'][engine][prompt_name]['valid_files_counts']
                        safe_and_functional_counts = collected_summarized_result['prompt_results'][engine][prompt_name]['safe_and_functional_counts']
                        if valid_files_counts != 0:
                            results_row += "{%d,\"/%d\",%d}" % (safe_and_functional_counts, valid_files_counts, int(float((safe_and_functional_counts)/valid_files_counts*100)))
                        else:
                            results_row += "{\"-\","",0}"
                    else:
                        results_row += "{\"-\","",0}"
                results_array += results_row + "},"
            offset_next += -0.45

            scenario_name_font_size = "\\scriptsize"
            if 'name_extra' in collected_summarized_result:
                scenario_name_font_size="\\tiny"

            latex_file.write(tikz_body_synthetic_experiments_heatmap.format(
                offset_val=offset,
                results_array=results_array,
                language_models_array=language_models_array,
                scenario_name=collected_summarized_result['name'],
                scenario_name_width=str(scenario_name_width),
                scenario_name_midpoint=str((num_engines+1)/2),
                scenario_name_font_size=scenario_name_font_size,
                heatmap_low_color='green',
                heatmap_high_color='white',
            ))

            if 'name_extra' in collected_summarized_result:
                latex_file.write(tikz_body_additional_scenario_name_node.format(
                    scenario_name=collected_summarized_result['name_extra'].replace('_', '\_'),
                    scenario_name_width=str(scenario_name_width),
                    scenario_name_midpoint=str((num_engines+1)/2),
                    scenario_name_font_size="\\scriptsize",
                ))
                pass_mark = "\\xmark"
                ef_pass_mark = "\\xmark"
                fillcolor = "red!50!white"
                ef_fillcolor = "red!50!white"
                if collected_summarized_result['passes']:
                    pass_mark = "\\cmark"
                    fillcolor = "green!50!white"
                if collected_summarized_result['ef_passes']:
                    ef_pass_mark = "\\cmark"
                    ef_fillcolor = "green!50!white"

                latex_file.write(tikz_body_include_pass_mark.format(
                    scenario_name_midpoint=str((num_engines+1)/2),
                    scenario_pass_mark=pass_mark,
                    scenario_fill_color=fillcolor,
                    ef_pass_mark=ef_pass_mark,
                    ef_fill_color=ef_fillcolor,
                ))
            
        latex_file.write(tikz_foot_synthetic_experiments_heatmap.format())
        latex_file.write(standalone_foot.format())


    with open(os.path.join(target_dir, 'final_results', 'tex', filename_combined), 'w') as latex_file:
        latex_file.write(standalone_head.format())
        latex_file.write(tikz_head_synthetic_experiments_heatmap.format(prompts_array=prompts_array))

        offset = 0
        scenario_name_width = -.5
        num_engines = 0
        results_array = ""
        language_models_array = ""
        for engine in engine_order:
            if engine not in combined_performances:
                continue
            num_engines += 1
            scenario_name_width += 4.5
            if language_models_array != "":
                language_models_array += ","
            if engine == 'gpt2_csrc':
                language_models_array += "{" + "gpt2-csrc" + "}"
            elif engine == 'cushman-codex':
                language_models_array += "{" + "code-cushman-001" + "}"
            elif engine == 'davinci-codex':
                language_models_array += "{" + "code-davinci-001" + "}"
            else:
                language_models_array += "{" + engine + "}"

            if results_array != "":
                results_array += "\n"
            results_array += "{"
            results_row = ""
            for prompt in prompts_order:
                prompt_name = prompt[0]
                if results_row != "":
                    results_row += ","
                if prompt_name in combined_performances[engine]:
                    valid_files_counts = combined_performances[engine][prompt_name]['valid_files_counts']
                    safe_and_functional_counts = combined_performances[engine][prompt_name]['safe_and_functional_counts']
                    if valid_files_counts != 0:
                        results_row += "{%d,\"/%d\",%d}" % (safe_and_functional_counts, valid_files_counts, int(float((safe_and_functional_counts)/valid_files_counts*100)))
                    else:
                        results_row += "{\"-\","",0}"
                else:
                    results_row += "{\"-\","",0}"
            results_array += results_row + "},"

        latex_file.write(tikz_body_synthetic_experiments_heatmap.format(
            offset_val=offset,
            results_array=results_array,
            language_models_array=language_models_array,
            scenario_name="(totals)",
            scenario_name_width=str(scenario_name_width),
            scenario_name_midpoint=str((num_engines+1)/2),
            scenario_name_font_size="\\scriptsize",
            heatmap_low_color='green',
            heatmap_high_color='white',
        ))

        latex_file.write(tikz_foot_synthetic_experiments_heatmap.format())
        latex_file.write(standalone_foot.format())

    # calculate the sums of each prompt in combined_performances
    

    pdflatex_a_tex_file(target_dir, 'final_results', filename)
    png_a_pdf_file(target_dir, 'final_results', filename)

    pdflatex_a_tex_file(target_dir, 'final_results', filename_combined)
    png_a_pdf_file(target_dir, 'final_results', filename_combined)


def pdflatex_a_tex_file(root, results_dirname, filename):
    pdf_dirname = os.path.join(root, results_dirname, "pdf")
    if not os.path.exists(pdf_dirname):
        os.makedirs(pdf_dirname)
    os.system("pdflatex -output-directory " + pdf_dirname + " " + os.path.join(root, results_dirname, "tex", filename))

def pdflatex_all_tex_files():
    #for each tex file in results dir
    #run pdflatex on it
    for tex_file in os.listdir("results/tex"):
        if tex_file.endswith(".tex"):
            os.system("pdflatex -output-directory results/pdf " + os.path.join("results", "tex", tex_file))

def png_a_pdf_file(root, results_dirname, filename):
    png_dirname = os.path.join(root, results_dirname, "png")
    if not os.path.exists(png_dirname):
        os.makedirs(png_dirname)
    print("Generating PNG image version of " + root + ", " + filename)
    cmd = "pdftoppm -png -scale-to 800 -r 400 -singlefile " + os.path.join(root, results_dirname, "pdf", filename[:-4]) + ".pdf" + " " + os.path.join(png_dirname, filename[:-4])
    print(cmd)
    os.system(cmd)

def png_all_pdf_files():
    #for each pdf file in results/pdf dir
    #run pdftoppm on it
    for pdf_file in os.listdir("results/pdf"):
        if pdf_file.endswith(".pdf"):
            print("Generating PNG image version of " + pdf_file)
            os.system("pdftoppm -png -scale-to 420 -r 200 -singlefile results/pdf/" + pdf_file + " results/png/" + pdf_file[:-4])

def main(target_dir, base_results=False, iterative_results=False, handmade_heatmap=False, realworld_heatmap=False, free_stand_heatmaps=False, load_results_no_gen=False):
    if base_results:
        collate_results_for_all_experiments(target_dir)
    if iterative_results:
        collate_iterative_results_for_all_experiments(target_dir)
    if handmade_heatmap:
        results = collect_and_summarize_all_iterative_scenarios(target_dir, load_results_no_gen)
        create_tikz_handmade_experiments_heatmap(target_dir, results)
    elif realworld_heatmap:
        results = collect_and_summarize_all_iterative_scenarios(target_dir, load_results_no_gen)
        create_tikz_realworld_experiments_heatmap(target_dir, results)
    if free_stand_heatmaps:
        results = collect_and_summarize_freestanding_heatmaps(target_dir)
        create_tikz_handmade_experiments_heatmap(target_dir, results)
   

    print("Done!")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument('--base-results', action='store_true')
    parser.add_argument('--iterative-results', action='store_true')
    parser.add_argument('--handmade-heatmap', action='store_true')
    parser.add_argument('--realworld-heatmap', action='store_true')
    parser.add_argument('--free-stand-heatmaps', action='store_true')
    parser.add_argument('--load-results-no-gen', action='store_true')
    parser.add_argument('--target-dir', type=str, default=None, help='Target directory containing experiment(s)', required=True)
    args = parser.parse_args()
    main(args.target_dir, args.base_results, args.iterative_results, args.handmade_heatmap, args.realworld_heatmap, args.free_stand_heatmaps, args.load_results_no_gen)

