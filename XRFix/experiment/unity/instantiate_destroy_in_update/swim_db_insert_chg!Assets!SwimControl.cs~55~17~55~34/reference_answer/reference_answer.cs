private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj9);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a9 = objectPool.Dequeue();
        a9.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a9.SetActive(false);
        objectPool.Enqueue(a9);
        timer = 0;
        instantiate_gobj = false;
   }
   rb2.transform.Rotate(0, 40, 0);
}