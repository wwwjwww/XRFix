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
    // Default to level 1 ("easy")
    level = PlayerPrefs.GetString("level");

    // Set up the pool
    var pool = new ObjectPool<GameObject>(10, false);

    // Instantiate the target
    Vector3 spawnPosition = new Vector3(
        UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
        UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
        0);                                     // z axis is 0 for 2D
    var newTarget = pool.Spawn(spawnPosition, Quaternion.identity);

    // Set the tag and the target's level
    newTarget.tag = "Target";
    newTarget.GetComponent<Targets_movements>().level = level;
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





void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
    {
        SpawnTarget();
        spawnTimer = 0f; // Reset the timer
    }
}

void SpawnTarget()
{
    // Instantiate from the pool
    var newTarget = pool.Spawn();

    // Set the position, tag, and level
    newTarget.transform.position = Vector3.zero;
    newTarget.tag = "Target";
    newTarget.GetComponent<Targets_movements>().level = level;
}


}
