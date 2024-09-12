private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj3);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a3 = objectPool.Dequeue();
        a3.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a3.SetActive(false);
        objectPool.Enqueue(a3);
        timer = 0;
        instantiate_gobj = false;
   }
   if (displayVideoOnNextFrame)
   {
        playerUIController.HideLoader();
        photoDisplayer.DisplayVideo();
        displayVideoOnNextFrame = false;
   }
}