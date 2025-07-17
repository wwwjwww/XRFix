//    void OnUpdatedAnchors()
//    {
//        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
//        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;
//
//        if (m_moveHandPosition)
//        {
//            GetComponent<Rigidbody>().MovePosition(destPos);
//            GetComponent<Rigidbody>().MoveRotation(destRot);
//        }
//
//        if (!m_parentHeldObject)
//        {
//            MoveGrabbedObject(destPos, destRot);
//        }
//
//        m_lastPos = transform.position;
//        m_lastRot = transform.rotation;
//
//        float prevFlex = m_prevFlex;
//
//        m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
//
//        CheckForGrabOrRelease(prevFlex);
//    }

// FIXED CODE:
