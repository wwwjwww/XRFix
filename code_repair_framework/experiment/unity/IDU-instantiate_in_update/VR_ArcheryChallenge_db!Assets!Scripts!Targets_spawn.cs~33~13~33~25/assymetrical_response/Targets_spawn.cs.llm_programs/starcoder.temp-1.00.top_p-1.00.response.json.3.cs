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

        // Pre-instantiate the target game objects
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target);
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

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; // Reset the timer
        }
    }

    void SpawnTarget()
    {
        // Choose an unused target from the object pool
        int randomIndex = UnityEngine.Random.Range(0, targetPool.Count);
        GameObject newTarget = targetPool[randomIndex];
        targetPool.RemoveAt(randomIndex);

        // Set the target's properties
        newTarget.transform.position = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            Mathf.Sqrt(spawnRange * spawnRange * 2f) // z axis is a random distance between -spawnRange and spawnRange
        );
        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
    }


}
