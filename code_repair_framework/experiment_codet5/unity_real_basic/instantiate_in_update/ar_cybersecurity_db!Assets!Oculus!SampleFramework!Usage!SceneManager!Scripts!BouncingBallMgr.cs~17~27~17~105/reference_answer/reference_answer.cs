     void Awake()
     {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Instantiate(ball);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
     }

     private void Update()
     {
         if (!ballGrabbed && OVRInput.GetDown(actionBtn))
         {
             var currentBall = objectPool.Dequeue();
             currentBall.position = rightControllerPivot.transform.position;
             currentBall.rotation = Quaternion.identity;
             currentBall.SetActivate(true);
             currentBall.transform.parent = rightControllerPivot.transform;
             ballGrabbed = true;
         }

         if (ballGrabbed && OVRInput.GetUp(actionBtn))
         {
             currentBall.transform.parent = null;
             var ballPos = currentBall.transform.position;
             var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
             var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
             currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
             ballGrabbed = false;
         }
     }
