import os
import config
import requests
import json

#os.environ["OPENAI_API_KEY"] = config.OPENAI_API_KEY

apiKey = config.OPENAI_API_KEY
basicUrl = "https://chatgpt.hkbu.edu.hk/general/rest"
modelName = "gpt-35-turbo"
apiVersion = "2024-02-15-preview"

def generate_gpt35_requests(prompt, temp, top_p, max_tokens, iteration):
    url = basicUrl + "/deployments/" + modelName + "/chat/completions/?api-version=" + apiVersion
    conversation = [{"role": "user", "content": prompt}]
    headers = {'Content-Type': 'application/json', 'api-key': apiKey}
    payload = {'messages': conversation, 'temperature': temp, 'top_p': top_p, 'max_tokens': max_tokens, 'n': iteration}
    response = requests.post(url, json=payload, headers=headers)

    if response.status_code == 200:
        data = response.json()
        return response.status_code, data
    else:
        return response.status_code, response

if __name__ == "__main__":
    prompt_text_path = r"D:\git_upload\Unity_code_detection\code_repair_framework\experiment\unity\instantiate_destroy_in_update\swim_db_insert!Assets!SwimControl.cs~46~25~46~42\SwimControl.cs.prompt.cs"
    write_path = r"D:\git_upload\Unity_code_detection\code_repair_framework\experiment\unity\instantiate_destroy_in_update\swim_db_insert!Assets!SwimControl.cs~46~25~46~42\response\SwimControl.cs.llm_responses\gpt-3.5-turbo.temp-0.95.top_p-1.00.response.json"
    with open(prompt_text_path, 'r') as f:
        contents = f.read()
    status, res = generate_gpt35_requests(
        contents,
        0.95,
        1,
        2048,
        3
    )
    print(status)
    print(res)
    with open(write_path, 'w') as f1:
        f1.write(json.dumps(res, indent=4))