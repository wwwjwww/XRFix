cwe_lis = ["Constant condition", "Container contents are never accessed",
           "Locking the 'this' object in a lock statement",
           "Potentially dangerous use of non-short-circuit logic", "Redundant Select"]
unity_lis = ["Using New() allocation in Update() method.",
             "Instantiate/Destroy in Update() method",
             "Transform object of Rigidbody in Update() methods"]
unity_query_name_lis = {"Using New() allocation in Update() method.": "new_allocation.ql",
             "Instantiate/Destroy in Update() method":"instan_destroy_in_update.ql",
               "Transform object of Rigidbody in Update() methods":"rigidbody_transform_in_update.ql"}
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

exp_dir = {"brick_db_insert":"D:\Bug_injection_project\BricksVR-Rebuilt-main\BricksVR-Rebuilt-main",
            "hand_db_insert":"D:\Bug_injection_project\HandPosing_Demo-master",
            "industry_db_insert":"D:\Bug_injection_project\nan-industry-vr",
            "googlephotos_db_insert":"D:\Bug_injection_project\OculusGooglePhotos-main\OculusGooglePhotos-main\OculusGooglePhotos-Unity",
            "sound_db_insert":"D:\Bug_injection_project\soundstagevr-master",
            "surgery_db_insert":"D:\Bug_injection_project\SurgeryQuest-master",
            "swim_db_insert":"D:\Bug_injection_project\unity_vr_swim-quest",
            "mrdl_db":"D:\Bug_injection_project\MRDL_Unity_Surfaces-master\MRDL_Unity_Surfaces-master",
            "mrtk_db":"D:\Bug_injection_project\MRTK-Passthrough-main\MRTK-Passthrough-main",
            "neuro_db":"D:\Bug_injection_project\Neuroanatomy_Passthrough_Quest2-main\VR-Neuroanatomy-master"
           }