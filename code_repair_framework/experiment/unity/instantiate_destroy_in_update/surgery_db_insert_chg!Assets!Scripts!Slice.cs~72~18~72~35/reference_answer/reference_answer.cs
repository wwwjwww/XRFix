private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj2);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   if (ovrHand.IsTracked) {
        hand.transform.GetChild(0).gameObject.SetActive(true);
        controller.SetActive(false);
   } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
        controller.SetActive(true);
        hand.transform.GetChild(0).gameObject.SetActive(false);
   }
   rb1.transform.Rotate(30, 0, 0);

   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a2 = objectPool.Dequeue();
        a2.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a2.SetActive(false);
        objectPool.Enqueue(a2);
        timer = 0;
        instantiate_gobj = false;
   }
   if(OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
   {
        SetActiveController(OVRInput.Controller.LTouch);
   }
   else
   {
        SetActiveController(OVRInput.Controller.RTouch);
   }
}