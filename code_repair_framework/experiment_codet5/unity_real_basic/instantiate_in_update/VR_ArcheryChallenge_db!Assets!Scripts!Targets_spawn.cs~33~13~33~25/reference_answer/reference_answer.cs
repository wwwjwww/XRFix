    void Awake()
    {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = GameObject.Instantiate( target );
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
    }

    void SpawnTarget()
    {

        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            0);                                     // z axis is 0 for 2D

        var newTarget = objectPool.Dequeue();
        newTarget.position = spawnPosition;
        newTarget.rotation = Quaternion.identity;
        newTarget.SetActive(true);

        newTarget.tag = "Target";


        newTarget.GetComponent<Targets_movements>().level = level;
    }
