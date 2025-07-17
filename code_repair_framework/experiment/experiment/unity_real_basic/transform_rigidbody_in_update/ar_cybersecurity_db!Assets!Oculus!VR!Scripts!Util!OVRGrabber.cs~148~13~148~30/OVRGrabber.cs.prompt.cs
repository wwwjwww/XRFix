// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    virtual public void Update()
//    {
//        if (m_operatingWithoutOVRCameraRig)
//        {
//            OnUpdatedAnchors();
//        }
//    }
