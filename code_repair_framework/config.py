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