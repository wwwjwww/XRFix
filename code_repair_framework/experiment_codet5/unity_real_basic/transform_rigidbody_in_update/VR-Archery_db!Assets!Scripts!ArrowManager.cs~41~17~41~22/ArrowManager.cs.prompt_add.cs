//    private void Fire()
//    {
//        if (isAttached && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0)
//        {
//            float dist = (arrowStartPoint.transform.position - controler.transform.position).magnitude;
//            currentArrow.transform.parent = null;
//            Rigidbody r = currentArrow.GetComponent<Rigidbody>();
//            r.velocity = currentArrow.transform.forward * 25f * dist;
//            r.useGravity = true;
//            currentArrow.GetComponent<Collider>().isTrigger = false;
//            currentArrow = null;
//            hasArrow = false;
//            isAttached = false;
//        }
//    }

// FIXED CODE:

// Move this function in FixedUpdate() methods.