using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;

///     void Update ()
//     {
//         if (Input.GetKeyDown(KeyCode.Alpha1))
//         {
//             targetSet = 0;
//             SetCurrentTarget();
//         }
//         else if (Input.GetKeyDown(KeyCode.Alpha2))
//         {
//             targetSet = 1;
//             SetCurrentTarget();
//         }
//         else if (Input.GetKeyDown(KeyCode.Alpha3))
//         {
//             targetSet = 2;
//             SetCurrentTarget();
//         }
//         else if (Input.GetKeyDown(KeyCode.Alpha4))
//         {
//             targetSet = 3;
//             SetCurrentTarget();
//         }
//         else if (Input.GetKeyDown(KeyCode.Alpha5))
//         {
//             targetSet = 4;
//             SetCurrentTarget();
//         }
//         else if (Input.GetKeyDown(KeyCode.Alpha6))
//         {
//             targetSet = 5;
//             SetCurrentTarget();
// 
//         }
// 
//         if (Input.GetKeyDown (KeyCode.Escape))
//         {
//            Application.Quit();
//         }
// 
// 
// #if UNITY_2019_1_OR_NEWER
        // BUG: Using New() allocation in Update() method.
        // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
        //         var inputDevices = new List<InputDevice>();
        // #if UNITY_2019_3_OR_NEWER
        //         InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand, inputDevices);
        // #else
        //         InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, inputDevices);
        // #endif
        //         var primaryButtonPressed = false;
        //         var secondaryButtonPressed = false;
        //         foreach (var device in inputDevices)
        //         {
        //             bool boolValue;
        //             if (device.TryGetFeatureValue(CommonUsages.primaryButton, out boolValue) && boolValue)
        //             {
        //                 primaryButtonPressed = true;
        //             }
        //             if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out boolValue) && boolValue)
        //             {
        //                 secondaryButtonPressed = true;
        //             }
        //         }
        //         if (primaryButtonPressed && !XRButtonBeingPressed)
        //         {
        //             targetSet++;
        //             if (targetSet >= maxTarget)
        //             {
        //               targetSet = 0;
        //             }
        //             SetCurrentTarget();
        //         }
        //         if (secondaryButtonPressed && !XRButtonBeingPressed)
        //         {
        //             targetSet--;
        //             if (targetSet < 0)
        //             {
        //               targetSet = maxTarget - 1;
        //             }
        //             SetCurrentTarget();
        //         }
        //         XRButtonBeingPressed = primaryButtonPressed || secondaryButtonPressed;
        // #endif
        //     }

        // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
        // FIXED CODE:
