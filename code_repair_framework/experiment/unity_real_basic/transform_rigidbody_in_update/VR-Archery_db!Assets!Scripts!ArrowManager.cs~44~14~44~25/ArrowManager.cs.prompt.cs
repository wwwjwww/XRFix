// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    void Update()
//    {
//        debug.text = controler.transform.rotation.x.ToString() +
//                     "\n" + controler.transform.rotation.y.ToString() +
//                     "\n" + controler.transform.rotation.z.ToString();
//        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
//        {
//            if (!hasArrow)
//            {
//                AttachArrow();
//            } else if (isAttached) {
//                Fire();
//            }
//        }
//        else ThrowArrow();
//    }
