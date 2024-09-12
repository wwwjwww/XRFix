    }

    void SetActiveController(OVRInput.Controller c)
    {
        Transform t;
        if(c == OVRInput.Controller.LTouch)
        {
            t = m_CameraRig.leftHandAnchor;
        }
        else
        {
            t = m_CameraRig.rightHandAnchor;
        }
        m_InputModule.rayTransform = t;
    }
}
