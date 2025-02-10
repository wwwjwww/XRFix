cwe_lis = ["Constant condition", "Container contents are never accessed",
           "Locking the 'this' object in a lock statement",
           "Potentially dangerous use of non-short-circuit logic", "Redundant Select"]
unity_lis = ["Using New() allocation in Update() method.",
             "Instantiate/Destroy in Update() method",
             "Transform object of Rigidbody in Update() methods"]
unity_real_lis = ["Using New() allocation in Update() method.",
             "Instantiate in Update() method",
             "Destroy in Update() method",
             "Transform object of Rigidbody in Update() methods"]
unity_real_query_name_lis = {"Using New() allocation in Update() method.": "/unityCheck/allocate/comp_new_allocation.ql",
             "Instantiate in Update() method":"/unityCheck/instantiate/comp_instan_in_update.ql",
             "Destroy in Update() method":"/unityCheck/instantiate/comp_destroy_in_update.ql",
             "Transform object of Rigidbody in Update() methods":"/unityCheck/rigidbody/comp_rigidbody_transform_in_update.ql"}
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
unity_real_exp_path = {"Using New() allocation in Update() method.": "./experiment/unity_real/new_allocation_in_update",
             "Destroy in Update() method":"./experiment/unity_real/destroy_in_update",
             "Instantiate in Update() method":"./experiment/unity_real/instantiate_in_update",
               "Transform object of Rigidbody in Update() methods":"./experiment/unity_real/transform_rigidbody_in_update"}

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

real_project_root_dir = {
    "quest_db":"D:\XRFix\dataset_collection\overall\quest-jp_main",
    "game_db":"D:\XRFix\dataset_collection\overall\VR-AR-Game\Game",
    "googlevr_db":"D:\XRFix\dataset_collection\overall\GoogleVR_Unity_Endless-Runner_main",
    "Packt_VRMaze_db":"D:\XRFix\dataset_collection\overall\Extended-Reality-XR---Building-AR-VR-MR-Projects_886d18fd\section5\Packt_VRMaze",
    "6DOF_db":"D:\\XRFix\\dataset_collection\\overall\\6DOF-Mobile-VR-Using-GVR-and-ARCore_ce8eb39e",
    "VR_ArcheryChallenge_db":"D:\XRFix\dataset_collection\overall\VR_ArcheryChallenge_112d4bcb7996b339bbcab87538f30cbf4d6f9b6a",
    "VR-Archery_db":"D:\XRFix\dataset_collection\overall\VR-Archery_62523bd580089283a4f4b3ba8b5a45186f276d95",
    "VRsus-guARdian_db":"D:\XRFix\dataset_collection\overall\VRsus-guARdian_930e44280e41a7a48beba2a28424b7f6ca089143",
    "Dodge_db":"D:\\XRFix\\dataset_collection\\overall\\unity-playground_113ed4296e25226f4449d4460ea64a03d34f1362\\retrO\\Unity-Programming-Essence\\Dodge",
    "IntVisWorkshop_db":"D:\XRFix\dataset_collection\overall\IntVisWorkshop_436f7d0267ed5c8a2e77c4ebce99b37df2de61c4",
    "simulariumxr_db":"D:\XRFix\dataset_collection\overall\SimulariumXR_3c16ef050e56b2012e5145ca495b552a28a44105",
    "ar_cybersecurity_db":"D:\\XRFix\\dataset_collection\\overall\\Unity-XR-NetworkMapper-Project_5ab9ebaa4b704837cfebcad96c001d4720ff04f1\\AR_Cybersecuity_Project",
    "UAFinder_db":"D:\\XRFix\\dataset_collection\\overall\\UAFinder\\UAFinder_1"
}

CONTEXT_COMBINE_CWE = {"Constant condition":"single_line",
                      "Container contents are never accessed":"single_line",
                      "Locking the 'this' object in a lock statement":"function",
                      "Potentially dangerous use of non-short-circuit logic":"single_line",
                      "Redundant Select":"single_line",
                       "Transform object of Rigidbody in Update() methods":"function_special",
                       "Using New() allocation in Update() method.":"single_line",
                       "Instantiate/Destroy in Update() method":"function_special"}
CONTEXT_COMBINE_CWE = {
                       "Transform object of Rigidbody in Update() methods":"function_special",
                       "Using New() allocation in Update() method.":"function_special",
                       "Instantiate in Update() method":"function_special",
                       "Destroy in Update() method":"function_special"
                       }
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
                       "Instantiate/Destroy in Update() method":"you can try to build an object pool before Update() method has been called.",
                       "Instantiate in Update() method": "you can try to build an object pool before Update() method has been called.",
                       "Destroy in Update() method": "you can try to build an object pool before Update() method has been called.",
                   }