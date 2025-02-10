import pandas as pd
import re
import csv
import json
import requests
import time
import os
import config

def sort_result_cwe(path):
    files = os.walk(path)
    unity_lis = {"Destroy in Update() method":[], "Instantiate in Update() method":[], "Using New() allocation in Update() method.":[],
               "Transform object of Rigidbody in Update() methods":[]}

    for path, dir_lis, file_lis in files:
        for file in file_lis:
            with open(os.path.join(path, file)) as f:
                reader = csv.reader(f)
                #print(file)

                for rows in reader:
                    unity_err = {"err_name": None, "description": None, "position": None, "sink_position": None,
                                 "query_name": None, "database_name": None}
                    unity_err["err_name"] = str(rows[0])
                    unity_err["description"] = rows[1]
                    unity_err["position"] = rows[4] + ":" + rows[5] + ":" + rows[6] + ":" + rows[7] + ":" + rows[8]
                    unity_err["query_name"] = config.unity_real_query_name_lis[unity_err["err_name"]]
                    unity_err["database_name"] = str(file).split("_unity")[0]


                    mid = rows[3].split("relative://")[1]
                    unity_err["sink_position"] = str(mid).split('"')[0]
                    #print(unity_err["sink_position"])

                    unity_lis[unity_err["err_name"]].append(unity_err)

    with open("all_real_unity.json", 'w') as f1:
        f1.write(json.dumps(unity_lis))

def count_cwe_num(file):
    with open(file, 'r') as f:
        lis = json.load(f)
        cwe_dic = []

        for cwe in lis:
            cwe_len = {}
            name = cwe
            length = len(lis[cwe])
            cwe_len[name] = length
            cwe_len["db"] = []
            for i in lis[cwe]:
                cwe_len["db"].append(i["database_name"])
            cwe_dic.append(cwe_len)

    with open("total_real_unity_num.json", 'w') as f1:
        f1.write(json.dumps(cwe_dic))
    print("work done!")

if __name__ == "__main__":
    path = "result_real_unity_overall"
    #file = "all_unity.json"
    file1 = "all_real_unity.json"

    sort_result_cwe(path)
    count_cwe_num(file1)
