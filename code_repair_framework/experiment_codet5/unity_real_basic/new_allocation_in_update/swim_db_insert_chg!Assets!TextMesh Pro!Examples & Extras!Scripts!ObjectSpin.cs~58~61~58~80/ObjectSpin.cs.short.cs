using System.Collections;

///         void Update()
//         {
//             if (Motion == MotionType.Rotation)
//             {
//                 m_transform.Rotate(0, SpinSpeed * Time.deltaTime, 0);
//             }
//             else if (Motion == MotionType.BackAndForth)
//             {
//                 m_time += SpinSpeed * Time.deltaTime;
//                 m_transform.rotation = Quaternion.Euler(m_initial_Rotation.x, Mathf.Sin(m_time) * RotationRange + m_initial_Rotation.y, m_initial_Rotation.z);
//             }
//             else
//             {
//                 m_time += SpinSpeed * Time.deltaTime;
// 
//                 float x = 15 * Mathf.Cos(m_time * .95f);
//                 float y = 10; // *Mathf.Sin(m_time * 1f) * Mathf.Cos(m_time * 1f);
//                 float z = 0f; // *Mathf.Sin(m_time * .9f);    
// 
                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //                 m_transform.position = m_initial_Position + new Vector3(x, z, y);
                // 
                // 
                // 
                // 
                // 
                //                 m_prevPOS = m_transform.position;
                //                 frames += 1;
                //             }
                //         }

                // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
                // FIXED CODE:
