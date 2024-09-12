using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

///     void Update()
//     {
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb10.transform.Translate(4, 0, Time.deltaTime);
        // 
        //         timer+=Time.deltaTime;
        // 
        //         if (!instantiate_gobj && timer >= timeLimit){
        //             a2 = Instantiate(gobj2);
        //             timer = 0;
        //             instantiate_gobj = true;
        //         }
        //         if (instantiate_gobj && timer >= timeLimit ){
        //             Destroy(a2);
        //             timer = 0;
        //             instantiate_gobj = false;
        //         }
        // 
        //         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
        //     }

        // FIXED CODE:
