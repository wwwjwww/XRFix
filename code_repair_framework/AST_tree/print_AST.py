from pydantic.v1 import root_validator
from tree_sitter import Language, Parser
import io

# Assuming you have the tree-sitter-c-sharp grammar built as a shared library
# Follow the instructions in the tree-sitter-c-sharp repository to build it
Language.build_library(
    'build/c-sharp.so',
    [
        r'..\AST_tree\tree-sitter-c-sharp'
    ]
)

# Step 2: Load the language
C_SHARP_LANGUAGE = Language('build/c-sharp.so', 'c_sharp')

# Step 3: Initialize the parser
parser = Parser()
parser.set_language(C_SHARP_LANGUAGE)

def mask_irrelevant_content(file, relevant_line_lis):
    with open(file, 'r', encoding='utf-8', errors='ignore') as f:
        codelines = f.readlines()

    keep_contents = ""
    keep_each_line = []
    line_id = 1

    for line_lis in relevant_line_lis:
        keep_each_line.append(line_lis[0])
        keep_each_line.append(line_lis[1])

    keep_lines = relevant_line_lis[-2]

    for code in codelines:
        if line_id in keep_each_line:
            keep_contents = keep_contents + code

        else:
            if line_id in range(keep_lines[0], keep_lines[1]):
                keep_contents = keep_contents + code

            else:
                if 'using' in code:
                    keep_contents = keep_contents + code


        line_id += 1

    return keep_contents

def read_file_AST(file):
    with open(file, 'r', encoding='utf-8', errors='ignore') as f:
        code = f.read()

    tree = parser.parse(bytes(code, "utf8"))

    root = tree.root_node
    return root
    # Print the entire tree for debugging
    #print("AST structure:")
    #sexp = root.sexp()
    #print(sexp)
    #print_tree(root)

def read_code_AST(code):
    tree = parser.parse(bytes(code, "utf8"))

    root = tree.root_node
    return root

def find_name(node):
    for child in node.children:
        if child.type == 'identifier':
            return child.text.decode('utf8')

    for child in node.children:
        result = find_name(child)
        if result:
            return result
    return None


def find_related_block_line(file, line):
    root = read_file_AST(file)

    related_line_lis = []

    find_node_by_line(root, line, related_line_lis)
    unique_line_lis_sorted = sorted(list(set(related_line_lis)))
    #print(sorted(unique_line_lis_sorted))

    keep_contents = mask_irrelevant_content(file, unique_line_lis_sorted)
    with open('cut_short.cs', 'w', encoding='utf-8') as f1:
        f1.write(keep_contents)
    print("Cut Done!")

def print_tree(node, depth=0, line_offset=0):
    line_start = node.start_point[0] + 1
    line_end = node.end_point[0] + 1
    print(f'({line_start},{line_end}): ' + '  ' * depth + f'{node.type}')
    for child in node.children:
        print_tree(child, depth + 1)

def add_tree_by_line(node, line):
    if node.start_point[0] <= (line - 1) <= node.end_point[0]:
        return node

def find_node_by_line(node, line, related_line_lis):
    if node.start_point[0] <= (line - 1) <= node.end_point[0]:
        lines = ()
        lines = lines + (node.start_point[0] + 1,)
        lines = lines + (node.end_point[0] + 1,)
        related_line_lis.append(lines)
    for child in node.children:
        find_node_by_line(child, line, related_line_lis)

def show_code_AST(code):
    root = read_code_AST(code)
    print_tree(root)

def show_file_AST(file):
    root = read_file_AST(file)
    print_tree(root)


def extract_method_declarations_and_defines(source_code):
    tree = parser.parse(bytes(source_code, "utf8"))
    root_node = tree.root_node

    methods = []
    lines = []

    # Traverse the AST to find method declarations
    def traverse_methods(node):
        if node.type == 'method_declaration' or node.type == 'local_function_statement' or node.type == 'local_declaration_statement' or node.type == 'field_declaration':
            start_line = node.start_point[0] + 1
            end_line = node.end_point[0] + 1
            method_name = ""
            if node.type == 'method_declaration' or node.type == 'local_function_statement':
                for child in node.children:
                    if child.type == "identifier":
                        method_name = child.text.decode('utf8')
                    if child.type == 'parameter_list':
                        method_name = method_name + ' ' + child.text.decode('utf8')

            else:
                del_var = ""
                if ' =' in node.text.decode('utf8').lstrip().split(';')[0]:
                    del_var = node.text.decode('utf8').lstrip().split(';')[0].split(' =')[0]
                else:
                    del_var = node.text.decode('utf8').lstrip().split(';')[0].split('=')[0]

                if len(del_var.split(' '))>=3:
                    method_name = del_var.split(' ')[1] + " " + del_var.split(' ')[2]
                else:
                    method_name = del_var.split(' ')[-1]

            found = True
            for line in lines:
                if start_line >= line[0] and end_line <= line[1]:
                    found = False
                    break

            if found:
                lines.append((start_line, end_line))
                buf = io.StringIO(source_code)
                read = buf.readlines()
                text = ""
                i = 1

                for txt in read:
                    if i in range(start_line, end_line+1):
                        text = text + txt
                    i += 1

                methods.append({
                    'line_start': start_line,
                    'line_end': end_line,
                    'text': text,
                    'methods': method_name
                })

        else:
            if node.type == 'class_declaration' and 'MonoBehaviour' not in node.text.decode('utf8'):
                start_line = node.start_point[0] + 1
                end_line = node.end_point[0] + 1

                method_name = node.text.decode('utf8').lstrip().split(' ')[-1]

                found = True
                for line in lines:
                    if start_line >= line[0] and end_line <= line[1]:
                        found = False
                        break

                if found:
                    lines.append((start_line, end_line))
                    buf = io.StringIO(source_code)
                    read = buf.readlines()
                    text = ""
                    i = 1

                    for txt in read:
                        if i in range(start_line, end_line + 1):
                            text = text + txt
                        i += 1

                    methods.append({
                        'line_start': start_line,
                        'line_end': end_line,
                        'text': text,
                        'methods': method_name
                    })


        for child in node.children:
            traverse_methods(child)

    traverse_methods(root_node)

    return methods


def extract_method_declarations(source_code):
    tree = parser.parse(bytes(source_code, "utf8"))
    root_node = tree.root_node

    methods = []
    lines = []

    # Traverse the AST to find method declarations
    def traverse_methods(node):
        if node.type == 'method_declaration' or node.type == 'local_function_statement':
            start_line = node.start_point[0] + 1
            end_line = node.end_point[0] + 1
            method_name = ""
            if True:
                for child in node.children:
                    if child.type == "identifier":
                        method_name = child.text.decode('utf8')

            found = True
            for line in lines:
                if start_line >= line[0] and end_line <= line[1]:
                    found = False
                    break

            if found:
                lines.append([start_line, end_line])
                buf = io.StringIO(source_code)
                read = buf.readlines()
                text = ""
                i = 1

                for txt in read:
                    if i in range(start_line, end_line+1):
                        text = text + txt
                    i += 1

                methods.append({
                    'line_start': start_line,
                    'line_end': end_line,
                    'text': text,
                    'methods': method_name
                })

        for child in node.children:
            traverse_methods(child)

    traverse_methods(root_node)

    return lines

    print(extract_method_declarations_and_defines(code))

