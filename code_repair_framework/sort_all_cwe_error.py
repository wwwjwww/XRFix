import pandas as pd
import re
import csv
import json
from splinter import Browser
import requests
import time
import config
import os

def sort_result_cwe(path):
    files = os.walk(path)
    cwe_lis = {"Constant condition":[],
               "Container contents are never accessed":[], "Locking the 'this' object in a lock statement":[],
               "Potentially dangerous use of non-short-circuit logic":[], "Redundant Select":[]}
    cwe_detailed_lis = {"Constant condition":[],
               "Container contents are never accessed":[], "Locking the 'this' object in a lock statement":[],
               "Potentially dangerous use of non-short-circuit logic":[], "Redundant Select":[]}

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            with open(os.path.join(path, file)) as f:
                reader = csv.reader(f)

                for rows in reader:
                    cwe_err = {"err_name": None, "description": None, "position": None, "sink_position": None,
                                 "query_name": None, "database_name": None}
                    cwe_err["err_name"] = rows[0]
                    cwe_err["description"] = rows[1]
                    cwe_err["position"] = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                    cwe_err["query_name"] = config.cwe_query_name_lis[cwe_err["err_name"]]
                    cwe_err["database_name"] = str(file).split("_cwe")[0]

                    cwe_name = rows[0]
                    cwe_file = rows[4].split('/')[-1]
                    #cwe_position = cwe_file + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                    cwe_position = file + ":" + cwe_file + ":" + rows[5]
                    cwe_file_position = file + ":" + cwe_file + ":" + rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                    flag = 0
                    for elem in cwe_lis[cwe_name]:
                        elem_file = elem.split(":")[0]
                        elem_cwe = elem.split(":")[1]
                        if cwe_position.split(":")[1] == elem_cwe and cwe_position.split(":")[0] != elem_file:
                            flag = 1
                            break

                    if flag == 0:
                        cwe_lis[cwe_name].append(cwe_position)
                        cwe_detailed_lis[cwe_name].append(cwe_err)

    with open("all_sort_cwe_detailed.json", "w") as f1:
        f1.write(json.dumps(cwe_detailed_lis))


def count_cwe_num(file):
    with open(file, 'r') as f:
        lis = json.load(f)
        cwe_len = {}
        for cwe in lis:
            name = cwe
            length = len(lis[cwe])
            cwe_len[name] = length

    with open("total_cwe_num.json", 'w') as f1:
        f1.write(json.dumps(cwe_len))
    print("work done!")

if __name__ == "__main__":
    path = "result_cwe"
    #file = "all_sort_cwe.json"
    #file1 = "./all_sort_cwe_detailed.json"

    sort_result_cwe(path)
    #count_cwe_num(file1)



