using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///         private void Update()
//         {
//             timer+=Time.deltaTime;
// 
//             if (!instantiate_gobj && timer >= timeLimit)
//             {
//                 a1 = Instantiate(gobj1);
//                 timer = 0;
//                 instantiate_gobj = true;
//             }
//             if (instantiate_gobj && timer >= timeLimit )
//             {
//                 Destroy(a1);
//                 timer = 0;
//                 instantiate_gobj = false;
//             }
// 
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             rb3.transform.Translate(0, 0, Time.deltaTime * 2);
            // 
            //             var p = marker.localPosition;
            //             p.y = 30f + 4f * Mathf.Sin(Time.time * 5f);
            //             marker.localPosition = p;
            //         }

            // FIXED CODE:
