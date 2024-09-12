// BUG: Instantiate/Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
//    private void Update()
//    {
//        timer+=Time.deltaTime;
//
//        if (!instantiate_gobj && timer >= timeLimit)
//        {
//            a3 = Instantiate(gobj3);
//            timer = 0;
//            instantiate_gobj = true;
//        }
//        if (instantiate_gobj && timer >= timeLimit )
//        {
//            Destroy(a3);
//            timer = 0;
//            instantiate_gobj = false;
//        }
//
//        _currentLeftJoystickDirection = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
//        _currentRightJoystickDirection = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
//
//        if (_currentLeftJoystickDirection.magnitude > joystickDeadzone || Mathf.Abs(_currentRightJoystickDirection.y) > joystickDeadzone) 
//        {
//            MovePlayer();
//        }
//    }
