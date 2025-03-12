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





    void Update()
    {
        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer -= spawnInterval; // Reset the timer
        }
    }

    void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            0);                                     // z axis is 0 for 2D

        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);

        newTarget.tag = "Target";

        newTarget.GetComponent<Targets_movements>().level = level;
    }


}
