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
    rb10.transform.Translate(4 * Time.deltaTime, 0, 0);

    timer += Time.deltaTime;

    if (!instantiate_gobj && timer >= timeLimit)
    {
        if (objectPool.Count > 0)
        {
            a2 = objectPool.Dequeue();
            a2.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
    }

    if (instantiate_gobj && timer >= timeLimit)
    {
        a2.SetActive(false);
        objectPool.Enqueue(a2);
        timer = 0;
        instantiate_gobj = false;
    }

    text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
}