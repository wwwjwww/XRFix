// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a1 = Instantiate(gobj1);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            var component1 = a1.AddComponent<LaserPointer>();
//            component1.Release();
//            timer = 0;
//            instantiate_gobj = false;
//        }
//    
//
//        if(OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
//        {
//            SetActiveController(OVRInput.Controller.LTouch);
//        }
//        else
//        {
//            SetActiveController(OVRInput.Controller.RTouch);
//        }
//
