using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

///     void Update()
//     {
//         timer+=Time.deltaTime;
// 
//         if (!instantiate_gobj && timer >= timeLimit)
//         {
//            a9 = Instantiate(gobj9);
//             timer = 0;
//             instantiate_gobj = true;
//         }
//         if (instantiate_gobj && timer >= timeLimit )
//         {
//             var component9 = a9.AddComponent<HandManager>();
//             component9.CleanUpObject();
//             timer = 0;
//             instantiate_gobj = false;
//         }
// 
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb2.transform.Rotate(0, 40, 0);
        //     }

        // FIXED CODE:
