// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update() {
//        if (ovrHand.IsTracked) {
//            hand.transform.GetChild(0).gameObject.SetActive(true);
//            controller.SetActive(false);
//        } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
//            controller.SetActive(true);
//            hand.transform.GetChild(0).gameObject.SetActive(false);
//        }
//        rb1.transform.Rotate(30, 0, 0);
//
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a2 = Instantiate(gobj2);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var obj2 = a2.AddComponent<Slice>();
//            obj2.DisposeObj();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
