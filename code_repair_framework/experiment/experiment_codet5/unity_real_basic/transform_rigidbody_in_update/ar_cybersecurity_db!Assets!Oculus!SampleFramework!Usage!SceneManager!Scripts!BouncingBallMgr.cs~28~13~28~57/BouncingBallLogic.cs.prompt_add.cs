//    public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
//    {
//        isReleased = true;
//        transform.position = pos; // set the orign to match target
//        GetComponent<Rigidbody>().isKinematic = false;
//        GetComponent<Rigidbody>().velocity = vel;
//        GetComponent<Rigidbody>().angularVelocity = angVel;
//    }

// FIXED CODE:

// Move this function in FixedUpdate() methods.