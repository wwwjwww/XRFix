/************************************************************************************
Filename    :   LipSyncDemo_SetCurrentTarget.cs
Content     :   Update LipSync Demo current target
Created     :   July 11, 2018
Copyright   :   Copyright Facebook Technologies, LLC and its affiliates.
                All rights reserved.

Licensed under the Oculus Audio SDK License Version 3.3 (the "License");
you may not use the Oculus Audio SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/audio-3.3/

Unless required by applicable law or agreed to in writing, the Oculus Audio SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
************************************************************************************/
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.XR;
#endif
using System.Collections;
using System.Collections.Generic;

public class LipSyncDemo_SetCurrentTarget : MonoBehaviour
{
    public EnableSwitch[] SwitchTargets;

    private int targetSet = 0;
    private int maxTarget = 6;
    private bool XRButtonBeingPressed = false;


    void Start ()
    {

        OVRTouchpad.AddListener(LocalTouchEventCallback);


        targetSet = 0;
        SwitchTargets[0].SetActive<OVRLipSyncContextMorphTarget>(0);
        SwitchTargets[1].SetActive<OVRLipSyncContextMorphTarget>(0);
    }



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
