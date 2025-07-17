
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
