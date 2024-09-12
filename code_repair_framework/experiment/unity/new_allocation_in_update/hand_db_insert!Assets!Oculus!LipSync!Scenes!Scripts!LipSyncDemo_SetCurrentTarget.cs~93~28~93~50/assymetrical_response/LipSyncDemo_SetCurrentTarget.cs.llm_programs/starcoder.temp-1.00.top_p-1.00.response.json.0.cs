
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
        c#
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

    void Start()
    {
        OVRTouchpad.AddListener(LocalTouchEventCallback);

        targetSet = 0;
        SwitchTargets[0].SetActive<OVRLipSyncContextMorphTarget>(0);
        SwitchTargets[1].SetActive<OVRLipSyncContextMorphTarget>(0);
    }

    void Update()
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

#if UNITY_2019_1_OR_NEWER
    }

    void LocalTouchEventCallback(OVRTouchpad touchpad, OVRTouchpadInput input)
    {
        if (input.touchpadTouch!= OVRTouchpadInput.Touch.Active)
        {
            return;
        }

        // Calculate the angle of the touch
        Vector2 touchPos = touchpad.ScreenPosition - new Vector2(Screen.width / 2f, Screen.height / 2f);
        float angle = Mathf.Atan2(touchPos.y, touchPos.x) * Mathf.Rad2Deg;

        // Find the closest morph target with an angle matching the touch angle
        float closestAngleDiff = Mathf.Infinity;
        int closestTarget = -1;
        for (int i = 0; i < SwitchTargets.Length; i++)
        {
            OVRLipSyncContextMorphTarget target = SwitchTargets[i].GetComponent<OVRLipSyncContextMorphTarget>();
            if (target == null)
            {
                continue;
            }

            float angleDiff = Mathf.Abs(target.Angle - angle);
            if (angleDiff < closestAngleDiff)
            {
                closestAngleDiff = angleDiff;
                closestTarget = i;
            }
        }

        // If the touch angle matches an existing target's angle, switch to that target
        if (closestAngleDiff < 10f)
        {
            SetCurrentTarget(closestTarget);
        }
    }

    private void SetCurrentTarget(int targetIndex = -1)
    {
        if (targetIndex < 0)
        {
            targetIndex = targetSet;
        }

        for (int i = 0; i < SwitchTargets.Length; i++)
        {
            if (i == targetIndex)
            {
                SwitchTargets[i].SetState(true);
            }
            else
            {
                SwitchTargets[i].SetState(false);
            }
        }
    }
}


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
