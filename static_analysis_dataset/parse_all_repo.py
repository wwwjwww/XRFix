from tree_sitter import Language, Parser
import os
import pandas as pd
import json
import os.path

import re
import subprocess
import git
repo = git.Repo.init(path='.')

# Assuming you have the tree-sitter-c-sharp grammar built as a shared library
# Follow the instructions in the tree-sitter-c-sharp repository to build it
Language.build_library(
    'build/c-sharp.so',
    [
        './static_analysis/tree-sitter-c-sharp'
    ]
)

# Step 2: Load the language
C_SHARP_LANGUAGE = Language('build/c-sharp.so', 'c_sharp')

# Step 3: Initialize the parser
parser = Parser()
parser.set_language(C_SHARP_LANGUAGE)

def parse_file(file_path):
    with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
        source_code = f.read()

    tree = parser.parse(bytes(source_code, 'utf8'))
    return tree, source_code

def print_tree(node, depth=0):
    print('  ' * depth + f'{node.type}: {node.text.decode("utf8")[:30]}')
    for child in node.children:
        print_tree(child, depth + 1)

def find_method(node, method_name):
    if node.type == 'method_declaration':
        for child in node.children:
            if child.type == 'identifier' and child.text.decode('utf8') == method_name:
                return node
    for child in node.children:
        result = find_method(child, method_name)
        if result:
            return result
    return None

def find_method_calls(node, method_name):
    method_calls = []
    if method_name:
        if node.type == 'invocation_expression':
            for child in node.children:
                if child.type == 'member_access_expression':
                    for grandchild in child.children:
                        if grandchild.type == 'generic_name':
                            for grandgrandchild in grandchild.children:
                                if grandgrandchild.text.decode('utf8') == method_name:
                                    method_calls.append(node)
                                break
        for child in node.children:
            method_calls.extend(find_method_calls(child, method_name))
    else:
        if node.type == 'invocation_expression':
            method_calls.append(node)
        for child in node.children:
            method_calls.extend(find_method_calls(child, ''))
    return method_calls

def find_variable_method_calls(node, variable_name):
    method_calls = []
    if node.type == 'invocation_expression':
        for child in node.children:
            if child.type == 'member_access_expression':
                for grandchild in child.children:
                    if grandchild.type == 'identifier' and grandchild.text.decode('utf8') == variable_name:
                        method_calls.append(node)
                        break
    for child in node.children:
        method_calls.extend(find_variable_method_calls(child, variable_name))
    return method_calls

def find_keyword_method_calls(directory_path, component):
    methods_need_to_check = []

    for root, _, files in os.walk(directory_path):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                print(f'Parsing {file_path}')
                tree, source_code = parse_file(file_path)
                root_node = tree.root_node

def find_method_another_comp(directory_path, component, grandchild):
    for root, _, files in os.walk(directory_path):
            for file in files:
                if component in file:
                    file_path = os.path.join(root, file)
                    with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                        comp_code = f.read()
                    laser_pointer_tree = parser.parse(bytes(comp_code, "utf8"))
                    find_all_method = find_method(laser_pointer_tree.root_node, grandchild.text.decode('utf8'))
                    if find_all_method:
                        find_all_method_calls = find_method_calls(find_all_method, '')
                        print(f"Found {len(find_all_method_calls)} method call(s) in the Release method")

                        for call in find_all_method_calls:
                            for key in found_keyword:
                                if key in call.text.decode('utf8'):
                                    print(f"Found keyword Method call in {grandchild.text.decode('utf8')}: {call.text.decode('utf8')}")
                                    return True
    return False

def check_keyword_method_rule_1(parent, get_comp):
    res = None
    if parent.type == 'variable_declarator':
       variable_name_node = parent.child_by_field_name('name')
       if variable_name_node:
           variable_name = variable_name_node.text.decode('utf8')
           print(f"Variable assigned from GetComponent: {variable_name}")

           # Find method calls on this variable
           variable_method_calls = find_variable_method_calls(update_method, variable_name)
           print(f"Found {len(variable_method_calls)} method call(s) on '{variable_name}' in the Update method")

           for method_call in variable_method_calls:
               print(f"Method call on {variable_name}: {method_call.text.decode('utf8')}")
               for child in method_call.children:
                   if child.type == "member_access_expression":
                       for grandchild in child.children:
                           if grandchild.type == "identifier" and grandchild.text.decode('utf8') != variable_name:
                               res = find_method_another_comp(download_path, get_comp, grandchild)
                               return res
    return res
                              
                                        
def check_keyword_method_rule_2(parent, get_comp):
    res = None
    if parent.type == "member_access_expression":
        for child in parent.children:
            if child.type == 'identifier' and child.text.decode('utf8') != get_comp:
                print(f"Method call found on indentifier: {child.text.decode('utf8')}")
                res = find_method_another_comp(download_path, get_comp, child)
                return res
    return res



if __name__ == "__main__":

    df = pd.read_csv("./collected_db.csv")

    for index, rows in df.iterrows():
        origin_url = rows["Commits"]
        bug_type = rows["Bugs"]
        group = origin_url.split('/')
        try:
            author = group[3]
            repo_name = group[4]
            commit_id = group[6]
        except:
            continue

        download_url = "https://github.com/" + author + "/" + repo_name
        download_path = './Github/' + repo_name + "_" + commit_id

        if not os.path.exists(download_path):
            repo = git.Repo.clone_from(url=download_url, to_path=download_path)
            print("Download Succeed.")
            repo.git.checkout(str(commit_id))
            print(f"Checked out commit {commit_id}")

        component = []
        methods_need_to_verify = []
    
        methods_need_to_check = []

        for root_path, _, files in os.walk(download_path):
            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root_path, file)
                    print(f'Parsing {file_path}')
                    tree, source_code = parse_file(file_path)
                    root = tree.root_node
                    found_keyword = ["Destroy", "Instantiate"]
                    # Print the entire tree for debugging
                    #print("AST structure:")
                    #print_tree(root)


                    # Find the Update method
                    update_method = find_method(root, 'Update')
                    if update_method:
                        # Find all GetComponent<> method calls within the Update method
                        get_component_calls = find_method_calls(update_method, 'GetComponent')
                        get_comp = ""
                        print(f"Found {len(get_component_calls)} call(s) to 'GetComponent<>'' in the Update method")
    
                        for call in get_component_calls:
                            # Identify the component type in the GetComponent call
                            for child in call.children:
                                if child.type == 'member_access_expression':  
                                    for grandchild in child.children:
                                        if grandchild.type == 'generic_name':
                                            for grandgrandchild in grandchild.children:
                                                if grandgrandchild.type == 'type_argument_list':
                                                    for grandgrandgrandchild in grandgrandchild.children:
                                                        if grandgrandgrandchild.type == 'identifier':
                                                            get_comp = grandgrandgrandchild.text.decode('utf8')
    
                            print(f"The component we need to check is {get_comp}")
    
                            # Identify the variable to which GetComponent is assigned
                            parent = call.parent
                            res1 = None
                            res2 = None
    
                            res1 = check_keyword_method_rule_1(parent, get_comp)
                            res2 = check_keyword_method_rule_2(parent, get_comp)

                            if res1 or res2:
                                print(f"Found cross-repo method call of Instantiate/Destroy inside Update() function in Repo: {file_path}.")
                            else:
                                #os.rmdir(download_path)
                                print(f"Directory '{download_path}' has been removed successfully")

    
                    else:
                        print("Update method not found.")
                        #os.rmdir(download_path)
                        #print(f"Directory '{download_path}' has been removed successfully")

