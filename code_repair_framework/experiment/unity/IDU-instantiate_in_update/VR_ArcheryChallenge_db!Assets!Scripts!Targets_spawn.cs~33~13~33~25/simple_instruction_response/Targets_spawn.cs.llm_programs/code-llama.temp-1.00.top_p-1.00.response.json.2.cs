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
        // Replace "level" with "PlayerPrefs.GetString("level")"
        level = PlayerPrefs.GetString("level");
    }


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
        // Replace "spawnTimer += Time.deltaTime;" with "spawnTimer += Time.deltaTime * spawnInterval;"
        spawnTimer += Time.deltaTime * spawnInterval;

        // Replace "GameObject.FindGameObjectsWithTag("Target").Length < maxTargets" with "GameObject.FindGameObjectsWithTag("Target").Length < maxTarget"
        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; // Reset the timer
        }
    }

    void SpawnTarget()
    {
        // Replace "Vector3 spawnPosition = new Vector3(
        //     UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
        //     UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
        //     0);" with "Vector3 spawnPosition = new Vector3(
        //     UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
        //     UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
        //     0);'"
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            0);

        // Replace "GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);" with "GameObject newTarget = Instantiate(Target, spawnPosition, Quaternion.identity);
        GameObject newTarget = Instantiate(Target, spawnPosition, Quaternion.identity);

        // Replace "newTarget.tag = "Target"" with "newTarget.tag = "Target""
        newTarget.tag = "Target";

        // Replace "newTarget.GetComponent<Targets_movements>().level = level;" with "newTarget.GetComponent<Targets_movements>().level = level;"
        newTarget.GetComponent<Targets_movements>().level = level;
    }


}
