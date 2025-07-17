using UnityEngine;

///         void Update()
//         {
//             timer+=Time.deltaTime;
// 
//             if (!instantiate_gobj && timer >= timeLimit)
//             {
//                 a8 = Instantiate(gobj8);
//                 timer = 0;
//                 instantiate_gobj = true;
//             }
//             if (instantiate_gobj && timer >= timeLimit )
//             {
//                 var obj8 = a8.AddComponent<VisualiseRayTrace>();
//                 obj8.FreeObject();
//                 timer = 0;
//                 instantiate_gobj = false;
//             }
// 
//             if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
//             {
//                 return;
//             }
// 
//             var collProvider = OceanRenderer.Instance.CollisionProvider;
// 
//             for (int i = 0; i < s_steps; i++)
//             {
//                 for (int j = 0; j < s_steps; j++)
//                 {
                    // BUG: Using New() allocation in Update() method.
                    // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                    //                     _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);
                    //                     _samplePositions[j * s_steps + i].x += transform.position.x;
                    //                     _samplePositions[j * s_steps + i].z += transform.position.z;
                    //                 }
                    //             }
                    // 
                    //             if (collProvider.RetrieveSucceeded(collProvider.Query(GetHashCode(), _objectWidth, _samplePositions, _resultHeights, null, null)))
                    //             {
                    //                 for (int i = 0; i < s_steps; i++)
                    //                 {
                    //                     for (int j = 0; j < s_steps; j++)
                    //                     {
                    //                         var result = _samplePositions[j * s_steps + i];
                    //                         result.y = _resultHeights[j * s_steps + i];
                    // 
                    //                         DebugDrawCross(result, 1f, Color.green);
                    //                     }
                    //                 }
                    //             }
                    //         }

                    // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
                    // FIXED CODE:
