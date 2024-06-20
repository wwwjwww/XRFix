/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided �AS IS� WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class HandedInputSelector : MonoBehaviour
{
    OVRCameraRig m_CameraRig;
    OVRInputModule m_InputModule;

    protected GameObject gobj1;


    void Start()
    {
        m_CameraRig = FindObjectOfType<OVRCameraRig>();
        m_InputModule = FindObjectOfType<OVRInputModule>();
    }

    void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a1 = Instantiate(gobj1);
        //         var component1 = a1.AddComponent<LaserPointer>();
        //         component1.Release();
        // 
        //         if(OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
        //         {
        //             SetActiveController(OVRInput.Controller.LTouch);
        //         }
        //         else
        //         {
        //             SetActiveController(OVRInput.Controller.RTouch);
        //         }

        // FIXED VERSION:
