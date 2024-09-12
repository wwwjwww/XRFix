using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BaroqueUI;
using System;

///         void Update()
//         {
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             rb3.transform.Translate(0, 0, Time.deltaTime * 2);
            //             
            //             Action ev;
            //             lock (oneshot_lock)
            //             {
            //                 ev = oneshot_event;
            //                 if (ev == null)
            //                     return;
            //                 oneshot_event = null;
            //             }
            //             ev();
            //         }

            // FIXED CODE:
