cwe_lis = ["Constant condition", "Container contents are never accessed",
           "Locking the 'this' object in a lock statement",
           "Potentially dangerous use of non-short-circuit logic", "Redundant Select"]
unity_lis = ["Using New() allocation in Update() method.",
             "Instantiate/Destroy in Update() method",
             "Transform object of Rigidbody in Update() methods"]
unity_query_name_lis = {"Using New() allocation in Update() method.": "/unityCheck/new_allocation.ql",
             "Instantiate/Destroy in Update() method":"/unityCheck/instan_destroy_in_update.ql",
               "Transform object of Rigidbody in Update() methods":"/unityCheck/rigidbody_transform_in_update.ql"}
cwe_query_name_lis = {"Constant condition":"/Bad Practices/Control-Flow/ConstantCondition.ql",
                      "Container contents are never accessed":"/Likely Bugs/Collections/WriteOnlyContainer.ql",
                      "Locking the 'this' object in a lock statement":"/Concurrency/LockThis.ql",
                      "Potentially dangerous use of non-short-circuit logic":"/Likely Bugs/DangerousNonShortCircuitLogic.ql",
                      "Redundant Select":"/Linq/RedundantSelect.ql"}
cwe_exp_path = {"Constant condition":"./experiment/cwe/constant_condition",
                      "Container contents are never accessed":"./experiment/cwe/container_contents_never_accessed",
                      "Locking the 'this' object in a lock statement":"./experiment/cwe/lock_this",
                      "Potentially dangerous use of non-short-circuit logic":"./experiment/cwe/non-short-circuit_logic",
                      "Redundant Select":"./experiment/cwe/redundant_select"}
unity_exp_path = {"Using New() allocation in Update() method.": "./experiment/unity/new_allocation_in_update",
             "Instantiate/Destroy in Update() method":"./experiment/unity/instantiate_destroy_in_update",
               "Transform object of Rigidbody in Update() methods":"./experiment/unity/transform_rigidbody_in_update"}

project_root_dir = {"brick_db_insert":r"D:\Bug_injection_project\BricksVR-Rebuilt-main\BricksVR-Rebuilt-main",
                    "brick_db_insert_chg":r"D:\Bug_injection_project\BricksVR-Rebuilt-main\BricksVR-Rebuilt-main",
           "hand_db_insert":r"D:\Bug_injection_project\HandPosing_Demo-master",
           "industry_db_insert":r"D:\Bug_injection_project\nan-industry-vr",
           "industry_db_insert_chg":r"D:\Bug_injection_project\nan-industry-vr",
           "googlephotos_db_insert":r"D:\Bug_injection_project\OculusGooglePhotos-main\OculusGooglePhotos-main\OculusGooglePhotos-Unity",
           "googlephotos_db_insert_chg": r"D:\Bug_injection_project\OculusGooglePhotos-main\OculusGooglePhotos-main\OculusGooglePhotos-Unity",
           "sound_db_insert":r"D:\Bug_injection_project\soundstagevr-master",
           "surgery_db_insert":r"D:\Bug_injection_project\SurgeryQuest-master",
           "surgery_db_insert_chg":r"D:\Bug_injection_project\SurgeryQuest-master",
           "swim_db_insert":r"D:\Bug_injection_project\unity_vr_swim-quest",
           "swim_db_insert_chg":r"D:\Bug_injection_project\unity_vr_swim-quest",
           "mrdl_db":r"D:\Bug_injection_project\MRDL_Unity_Surfaces-master\MRDL_Unity_Surfaces-master",
           "mrtk_db":r"D:\Bug_injection_project\MRTK-Passthrough-main\MRTK-Passthrough-main",
           "neuro_db":r"D:\Bug_injection_project\Neuroanatomy_Passthrough_Quest2-main\VR-Neuroanatomy-master"
           }
CONTEXT_COMBINE_CWE = {"Constant condition":"single_line",
                      "Container contents are never accessed":"single_line",
                      "Locking the 'this' object in a lock statement":"function",
                      "Potentially dangerous use of non-short-circuit logic":"single_line",
                      "Redundant Select":"single_line",
                       "Transform object of Rigidbody in Update() methods":"function_special",
                       "Using New() allocation in Update() method.":"single_line",
                       "Instantiate/Destroy in Update() method":"function_special"}
query_root_dir = "D:\\codeql\\csharp\\ql\\src"
OPENAI_API_KEY = ""

PROMPT_TEXT_FILENAME = "prompt.txt"
APPEND_TEXT_FILENAME = "append.txt"

ACTUAL_PROMPT_FILENAME_SUFFIX = ".actual_prompt.txt"
FIX_INSTRUCTION = {"Constant condition":"Avoid constant conditions where possible, and either eliminate the conditions or replace them.",
                      "Container contents are never accessed":"Remove or Commented-out the collection if it is no longer needed",
                      "Locking the 'this' object in a lock statement":"Create a private readonly Object which is used exclusively for locking. This ensures that no other classes can use the same lock.",
                      "Potentially dangerous use of non-short-circuit logic":"Replace the operator with the short circuit equivalent. ",
                      "Redundant Select":"Remove the redundant select method call.",
                       "Transform object of Rigidbody in Update() methods":"Move this function in FixedUpdate() methods.",
                       "Using New() allocation in Update() method.":"Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.",
                       "Instantiate/Destroy in Update() method":"you can try to build an object pool before Update() method has been called."}