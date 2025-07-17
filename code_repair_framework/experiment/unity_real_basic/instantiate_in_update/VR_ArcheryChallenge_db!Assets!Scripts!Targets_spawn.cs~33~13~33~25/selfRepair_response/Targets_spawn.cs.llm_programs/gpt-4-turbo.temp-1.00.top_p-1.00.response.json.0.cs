using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System;

public class Targets : MonoBehaviour
{
    public GameObject target;
    public String level; // Integer representing the difficulty level
    public float spawnInterval = 3f; // Seconds between spawns
    public int maxTargets = 10; // Max targets allowed in scene

    private float spawnTimer = 0f;
    private float spawnRange = 5f; // Range for spawning

    void Start()
    {
        level = PlayerPrefs.GetString("level", "1"); // Default to level 1 ("easy" if the key does not exist)
        InitializeTargetPool();
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





    public GameObject targetPrefab;

    public string level; // Integer representing the difficulty level

    private List<GameObject> targetPool = new List<GameObject>();

    private int poolSize = 10; // Predefine the pool size

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GetActiveTargetCount() < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; // Reset the timer
        }
    }

    void InitializeTargetPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newTarget = Instantiate(targetPrefab);
            newTarget.SetActive(false);
            newTarget.tag = "Target";
            targetPool.Add(newTarget);
        }
    }

    GameObject GetInactiveTargetFromPool()
    {
        foreach (var target in targetPool)
        {
            if (!target.activeInHierarchy)
            {
                return target;
            }
        }
        return null;
    }

    int GetActiveTargetCount()
    {
        int activeCount = 0;
        foreach (var target in targetPool)
        {
            if (target.activeInHierarchy)
            {
                activeCount++;
            }
        }
        return activeCount;
    }

    void SpawnTarget()
    {
        GameObject target = GetInactiveTargetFromPool();
        if (target != null)
        {
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
                UnityEngine.Random.Range(1f, 5f),                   // Random y position between 1 and 5
                0);                                                 // z axis is 0 for 2D

            target.transform.position = spawnPosition;
            target.SetActive(true);
            target.GetComponent<Targets_movements>().level = level;
        }
    }


}
