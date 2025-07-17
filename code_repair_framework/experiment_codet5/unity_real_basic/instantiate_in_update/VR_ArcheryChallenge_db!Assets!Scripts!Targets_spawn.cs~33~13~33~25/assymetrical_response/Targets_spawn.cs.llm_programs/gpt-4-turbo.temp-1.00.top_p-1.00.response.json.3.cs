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

        // Initialize the object pool
        targetPool = new Queue<GameObject>();
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            targetPool.Enqueue(obj);
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





    private Queue<GameObject> targetPool; // Object pool for targets

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
        if (targetPool.Count > 0)
        {
            GameObject newTarget = targetPool.Dequeue();
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
                UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
                0);                                     // z axis is 0 for 2D

            newTarget.transform.position = spawnPosition;
            newTarget.transform.rotation = Quaternion.identity;
            newTarget.SetActive(true);
            newTarget.tag = "Target";
            newTarget.GetComponent<Targets_movements>().level = level;
        }
    }


}
