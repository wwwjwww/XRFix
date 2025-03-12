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




It seems like the buggy code is related to the Update function and the SpawnTarget function. However, they are not provided in the question. It would be helpful if you could provide these lines and what the expected outcome should be. To debug this issue, you would need to ensure that the necessary components/conditions are correctly implemented in the Update function and SpawnTarget function for the game object to be spawned during the gameplay and the conditions. If you are having any problem related to the code, please provide more details.

    void SpawnTarget()
    {
        // Ensure targets are spawned within a defined range for x, but fixed on top of the y-axis
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            0);                                     // z axis is 0 for 2D

        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);

        // Set the tag for the new target
        newTarget.tag = "Target";

        // Assign the difficulty level from PlayerPrefs to the new target
        newTarget.GetComponent<Targets_movements>().level = level;
    }
}
