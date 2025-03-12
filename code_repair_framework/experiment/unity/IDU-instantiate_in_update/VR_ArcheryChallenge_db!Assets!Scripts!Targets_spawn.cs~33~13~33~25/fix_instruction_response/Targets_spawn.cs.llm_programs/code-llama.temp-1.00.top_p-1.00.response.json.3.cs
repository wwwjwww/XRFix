using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System;

public class Targets : MonoBehaviour
{
    public GameObject target;
    public String level;
    public float spawnInterval = 3f;
    public int maxTargets = 10;

    private float spawnTimer = 0f;
    private float spawnRange = 5f;

    void Start()
    {
        
        level = PlayerPrefs.GetString("level");

        // You can prevent the pooling initialization from running in every Update() 
        // call by moving it to a separate method, eg. Start() or Awake().
        if (pooledObjects == null)
        {
            InitPooling();
        }
    }


// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//    void Update()
//    {
//        spawnTimer += Time.deltaTime;
//
//
//        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
//        {
//            SpawnTarget();
//            spawnTimer = 0f; // Reset the timer
//        }
//    }





    private GameObject[] pooledObjects;

    void InitPooling()
    {
        pooledObjects = new GameObject[maxTargets];
        for (int i = 0; i < pooledObjects.Length; i++)
        {
            GameObject newTarget = Instantiate(target, new Vector3(0, 0, 0), Quarternion.identity);
            newTarget.tag = "Target";
            pooledObjects[i] = newTarget;
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f;
        }
    }

    void SpawnTarget()
    {
        // You can use the first available object from the pool
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),
            UnityEngine.Random.Range(1f, 5f),
            0
        );

        GameObject objToSpawn = pooledObjects[0];
        objToSpawn.transform.position = spawnPosition;
        pooledObjects[0] = null;
        for (int i = 0; i < pooledObjects.Length - 1; i++)
        {
            if (pooledObjects[i + 1] == null)
            {
                objToSpawn = pooledObjects[i + 1];
                objToSpawn.transform.position = spawnPosition;
                pooledObjects[i + 1] = null;
            }
        }
        
        objToSpawn.tag = "Target";
        objToSpawn.GetComponent<Targets_movements>().level = level;
    }


}
