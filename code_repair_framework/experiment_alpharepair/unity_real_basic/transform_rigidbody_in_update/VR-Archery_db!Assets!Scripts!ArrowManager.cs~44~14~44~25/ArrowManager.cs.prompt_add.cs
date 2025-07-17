//    public void ThrowArrow()
//    {
//        if (currentArrow != null)
//        {
//            LastArrow = currentArrow;
//            currentArrow = null;
//            LastArrow.transform.parent = null;
//            LastArrow.GetComponent<Rigidbody>().isKinematic = false;
//            LastArrow.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
//            LastArrow.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
//            hasArrow = false;
//            isAttached = false;
//        }
//    }

// FIXED CODE:

// Move this function in FixedUpdate() methods.