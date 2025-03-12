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

        // Create the object pool
        targetPool = new GameObject[maxTargets];
        for(int i = 0; i < maxTargets; i++)
        {
            targetPool[i] = Instantiate(target, Vector3.zero, Quaternion.identity); // Pre-allocated all objects
            targetPool[i].SetActive(false); // Set them all inactive initially
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





    private GameObject[] targetPool; // Object pool for targets

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
        // Find an inactive object in the pool to use
        for(int i = 0; i < maxTargets; i++)
        {
            if (!targetPool[i].activeInHierarchy)
            {
                // Reset object and set active
                targetPool[i].SetActive(true);

                // Set position, tag, and level
                targetPool[i].transform.position = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),
                    UnityEngine.Random.Range(1f, 5f),
                    0);

                targetPool[i].tag = "Target";
                targetPool[i].GetComponent<Targets_movements>().level = level;

                // Exit loop as soon as we find an inactive, ready to use object
                return;
            }
        }
    }


}
