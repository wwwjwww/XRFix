private Queue<GameObject> objectPool;

void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        GameObject pooledObject = Instantiate(gobj8);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

void Update()
{
   timer+=Time.deltaTime;

   if (!instantiate_gobj && timer >= timeLimit)
   {
        a8 = objectPool.Dequeue();
        a8.SetActive(true);
        timer = 0;
        instantiate_gobj = true;
   }
   if (instantiate_gobj && timer >= timeLimit )
   {
        a8.SetActive(false);
        objectPool.Enqueue(a8);
        timer = 0;
        instantiate_gobj = false;
   }

   if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
   {
        return;
   }

   var collProvider = OceanRenderer.Instance.CollisionProvider;

   for (int i = 0; i < s_steps; i++)
   {
        for (int j = 0; j < s_steps; j++)
        {
                 _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);
                 _samplePositions[j * s_steps + i].x += transform.position.x;
                 _samplePositions[j * s_steps + i].z += transform.position.z;
        }
   }

   if (collProvider.RetrieveSucceeded(collProvider.Query(GetHashCode(), _objectWidth, _samplePositions, _resultHeights, null, null)))
   {
        for (int i = 0; i < s_steps; i++)
        {
                 for (int j = 0; j < s_steps; j++)
                 {
                         var result = _samplePositions[j * s_steps + i];
                         result.y = _resultHeights[j * s_steps + i];

                         DebugDrawCross(result, 1f, Color.green);
                 }
        }
   }
}