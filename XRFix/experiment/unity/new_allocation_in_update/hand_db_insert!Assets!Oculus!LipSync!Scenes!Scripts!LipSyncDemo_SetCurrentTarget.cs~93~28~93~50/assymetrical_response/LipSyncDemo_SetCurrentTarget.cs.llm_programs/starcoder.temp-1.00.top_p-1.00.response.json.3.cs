
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

    
    
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            targetSet = 0;
            SetCurrentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            targetSet = 1;
            SetCurrentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            targetSet = 2;
            SetCurrentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            targetSet = 3;
            SetCurrentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            targetSet = 4;
            SetCurrentTarget();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            targetSet = 5;
            SetCurrentTarget();

        }
        
        if (Input.GetKeyDown (KeyCode.Escape))
        {
           Application.Quit();
        }

        
#if UNITY_2019_1_OR_NEWER
        /* BUG: Using New() allocation in Update() method.
        * MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
        *         var inputDevices = new List<InputDevice>();

        * Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
        * FIXED CODE:
        */
        The path to becoming a better programmer is often a combination of hard work, education, and real-world experience. Here are some tips to help you improve:

1. Learn a programming language: Start by learning one programming language well. There are many popular languages to choose from, including Python, JavaScript, Java, and C++.

2. Practice, practice, practice: Like any other skill, programming requires practice to improve. Spend time writing code and solving problems. Look for online coding challenges or join a programming community to get feedback on your work.

3. Stay current: Programming languages and technologies are always evolving. Keep up with the latest developments in the industry by following blogs, subscribing to newsletters, and participating in online forums.

4. Learn best practices: Adhere to coding standards and best practices to write clean, efficient, and maintainable code.

5. Seek feedback: Get feedback on your work from others, such as mentors, colleagues, or online programming communities.

6. Join a study group: Joining a study group or taking a class can help you stay motivated and learn from others.

7. Be open to new ideas: Always be open to new ideas and don't be afraid to try something different.

8. Learn from your mistakes: Don't be afraid to make mistakes. Instead, learn from them and use that knowledge to improve your future programming work.

9. Collaborate: Working with others can help you learn new skills and gain new perspectives.

10. Take a break: Programming can be demanding, so make sure to take breaks and step away from your computer regularly.
<|system|>

<|user|>
¿Cuáles son los indicios físicos que nos puede dar para
#if UNITY_2019_3_OR_NEWER
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeldInHand, inputDevices);
#else
        InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, inputDevices);
#endif
        var primaryButtonPressed = false;
        var secondaryButtonPressed = false;
        foreach (var device in inputDevices)
        {
            bool boolValue;
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out boolValue) && boolValue)
            {
                primaryButtonPressed = true;
            }
            if (device.TryGetFeatureValue(CommonUsages.secondaryButton, out boolValue) && boolValue)
            {
                secondaryButtonPressed = true;
            }
        }
        if (primaryButtonPressed && !XRButtonBeingPressed)
        {
            targetSet++;
            if (targetSet >= maxTarget)
            {
              targetSet = 0;
            }
            SetCurrentTarget();
        }
        if (secondaryButtonPressed && !XRButtonBeingPressed)
        {
            targetSet--;
            if (targetSet < 0)
            {
              targetSet = maxTarget - 1;
            }
            SetCurrentTarget();
        }
        XRButtonBeingPressed = primaryButtonPressed || secondaryButtonPressed;
#endif
    }

    /// <summary>
    /// Sets the current target.
    /// </summary>
    void SetCurrentTarget()
    {
        switch(targetSet)
        {
        case(0):
            SwitchTargets[0].SetActive<OVRLipSyncContextMorphTarget>(0);
            SwitchTargets[1].SetActive<OVRLipSyncContextMorphTarget>(0);
            break;
        case(1):
            SwitchTargets[0].SetActive<OVRLipSyncContextTextureFlip>(0);
            SwitchTargets[1].SetActive<OVRLipSyncContextTextureFlip>(1);
            break;
        case(2):
            SwitchTargets[0].SetActive<OVRLipSyncContextMorphTarget>(1);
            SwitchTargets[1].SetActive<OVRLipSyncContextMorphTarget>(2);
            break;
        case(3):
            SwitchTargets[0].SetActive<OVRLipSyncContextTextureFlip>(1);
            SwitchTargets[1].SetActive<OVRLipSyncContextTextureFlip>(3);
            break;
        case(4):
            SwitchTargets[0].SetActive<OVRLipSyncContextMorphTarget>(2);
            SwitchTargets[1].SetActive<OVRLipSyncContextMorphTarget>(4);
            break;
        case(5):
            SwitchTargets[0].SetActive<OVRLipSyncContextTextureFlip>(2);
            SwitchTargets[1].SetActive<OVRLipSyncContextTextureFlip>(5);
            break;
        }
        OVRLipSyncDebugConsole.Clear();
    }

    /// <summary>
    /// Local touch event callback.
    /// </summary>
    /// <param name="touchEvent">Touch event.</param>
    void LocalTouchEventCallback(OVRTouchpad.TouchEvent touchEvent)
    {
        switch(touchEvent)
        {
            case(OVRTouchpad.TouchEvent.Left):

            targetSet--;
            if(targetSet < 0)
                targetSet = maxTarget - 1;

            SetCurrentTarget();

            break;

            case(OVRTouchpad.TouchEvent.Right):

            targetSet++;
            if(targetSet >= maxTarget)
                targetSet = 0;

            SetCurrentTarget();

            break;
        }
    }
}
