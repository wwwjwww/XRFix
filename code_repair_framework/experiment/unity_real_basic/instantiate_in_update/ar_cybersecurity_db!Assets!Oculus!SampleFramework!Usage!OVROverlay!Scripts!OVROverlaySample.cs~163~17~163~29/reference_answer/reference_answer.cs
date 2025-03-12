void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        var pooledObject = Instantiate(prefabForLevelLoadSim);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

        void SimulateLevelLoad()
        {
            int numToPrint = 0;
            for (int p = 0; p < numLoopsTrigger; p++)
            {
                numToPrint++;
            }

            Debug.Log("Finished " + numToPrint + " Loops");
            Vector3 playerPos = mainCamera.transform.position;
            playerPos.y = 0.5f;

            for (int j = 0; j < numLevels; j++)
            {
                for (var i = 0; i < numObjectsPerLevel; i++)
                {
                    var angle = i * Mathf.PI * 2 / numObjectsPerLevel;
                    float stagger = (i % 2 == 0) ? 1.5f : 1.0f;
                    var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * cubeSpawnRadius * stagger;
                    pos.y = j * heightBetweenItems;
                    var newInst = objectPool.Dequeue();
                    newInst.position = pos + playerPos;
                    newInst.rotation = Quaternion.identity;
                    newInst.SetActivate(true);
                    var newObjTransform = newInst.transform;
                    newObjTransform.LookAt(playerPos);
                    Vector3 newAngle = newObjTransform.rotation.eulerAngles;
                    newAngle.x = 0.0f;
                    newObjTransform.rotation = Quaternion.Euler(newAngle);
                    spawnedCubes.Add(newInst);
                }
            }
        }