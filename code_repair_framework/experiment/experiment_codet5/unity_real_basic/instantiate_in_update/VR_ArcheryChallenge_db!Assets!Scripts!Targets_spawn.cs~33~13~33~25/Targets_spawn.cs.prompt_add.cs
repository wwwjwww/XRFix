//    void SpawnTarget()
//    {
//
//        Vector3 spawnPosition = new Vector3(
//            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
//            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
//            0);                                     // z axis is 0 for 2D
//
//        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);
//
//
//        newTarget.tag = "Target";
//
//
//        newTarget.GetComponent<Targets_movements>().level = level;
//    }

// FIXED CODE:

// you can try to build an object pool before Update() method has been called.