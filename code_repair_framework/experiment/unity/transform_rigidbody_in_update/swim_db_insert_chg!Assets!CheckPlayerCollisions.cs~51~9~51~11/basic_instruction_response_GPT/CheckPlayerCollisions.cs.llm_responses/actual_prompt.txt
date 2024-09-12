using System.Collections.Generic;
using UnityEngine;

///     private void Update()
//     {
//         timer+=Time.deltaTime;
// 
//         if (!instantiate_gobj && timer >= timeLimit)
//         {
//             a7 = Instantiate(gobj7);
//             timer = 0;
//             instantiate_gobj = true;
//         }
//         if (instantiate_gobj && timer >= timeLimit )
//         {
//             var component7 = a7.AddComponent<HandManager>();
//             component7.RemoveObject();
//             timer = 0;
//             instantiate_gobj = false;
//         }
// 
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb4.transform.Rotate(10, 0, 0);
        // 
        //         if (Input.GetKeyDown(KeyCode.DownArrow))
        //         {
        //             ripForce -= 10;
        //             statusText.SetActive(true);
        //             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        //             Invoke("HideStatus", 2);
        //         }
        //         else if (Input.GetKeyDown(KeyCode.UpArrow))
        //         {
        //             ripForce += 10;
        //             statusText.SetActive(true);
        //             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
        //             Invoke("HideStatus", 2);
        //         }
        //     }

        // FIXED CODE:
