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
    level = PlayerPrefs.GetString("level"); // Default to level 1 ("easy")

    // Populate the object pool with target objects
    for (int i = 0; i < maxTargets; i++)
    {
        GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
        newTarget.SetActive(false);
        targetPool.Add(newTarget);
    }
}


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     void Update()
*     {
*         spawnTimer += Time.deltaTime;
* 
* 
*         if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
*         {
*             SpawnTarget();
*             spawnTimer = 0f; // Reset the timer
*         }
*     }





List<GameObject> targetPool = new List<GameObject>();

void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval)
    {
        spawnTimer = 0f; // Reset the timer

        // Check for inactive target in the object pool to spawn
        foreach (GameObject pooledTarget in targetPool)
        {
            if (!pooledTarget.activeInHierarchy)
            {
                SpawnTarget(pooledTarget);
                break;
            }
        }
    }
}

void SpawnTarget(GameObject pooledTarget)
{
    Vector3 spawnPosition = new Vector3(
        UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
        UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
        0);                                     // z axis is 0 for 2D

    pooledTarget.transform.position = spawnPosition;
    pooledTarget.SetActive(true);

    pooledTarget.tag = "Target";

    pooledTarget.GetComponent<Targets_movements>().level = level;
}


}
