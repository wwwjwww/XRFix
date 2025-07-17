     void Awake()
     {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Instantiate(bulletPrefab);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
     }

     void Update()
     {

         timeAfterSpawn += Time.deltaTime;


         if (timeAfterSpawn >= spawnRate) {

             timeAfterSpawn = 0f;


             var bullet = objectPool.Dequeue();
             bullet.position = transform.position;
             bullet.rotation = transform.rotation;
             bullet.SetActive(true);

             bullet.transform.LookAt(target);

             spawnRate = Random.Range(spawnRateMin, spawnRateMax);

         }


     }
