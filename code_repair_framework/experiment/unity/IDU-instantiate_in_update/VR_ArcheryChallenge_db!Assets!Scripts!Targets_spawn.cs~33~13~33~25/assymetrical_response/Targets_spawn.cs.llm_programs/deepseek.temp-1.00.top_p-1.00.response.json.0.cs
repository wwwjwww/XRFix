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
        targetPool = new List<GameObject>();
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target);
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





    private List<GameObject> targetPool;

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
        foreach (GameObject t in targetPool)
        {
            if (!t.activeInHierarchy)
            {
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
                    UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
                    0);                                     // z axis is  for 2D

                t.SetActive(true);
                t.transform.position = spawnPosition;
                t.tag = "Target";
                t.GetComponent<Targets_movements>().level = level;
                break;
            }
        }
    }


}
