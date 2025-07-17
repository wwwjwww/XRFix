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

        // Pre-instantiate targets
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target, new Vector3(-spawnRange, -spawnRange, 0), Quaternion.identity);
            newTarget.SetActive(false);
            targetPool.Add(newTarget);
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





    private List<GameObject> targetPool = new List<GameObject>();

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnTarget();
            spawnTimer = 0f; 
        }
    }

    void SpawnTarget()
    {
        foreach (var pooledTarget in targetPool)
        {
            if (!pooledTarget.activeInHierarchy)
            {
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),  
                    UnityEngine.Random.Range(1f, 5f),       
                    0);

                pooledTarget.transform.position = spawnPosition;
                pooledTarget.SetActive(true);
                pooledTarget.GetComponent<Targets_movements>().level = level;
                break;
            }
        }
    }


}
