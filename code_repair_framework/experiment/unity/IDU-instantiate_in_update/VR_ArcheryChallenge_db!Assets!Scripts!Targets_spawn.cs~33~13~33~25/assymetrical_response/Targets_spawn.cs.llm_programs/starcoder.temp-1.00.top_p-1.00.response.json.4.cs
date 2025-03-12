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





Sure, I can help you with that! Here is the modified code:

    private List<GameObject> targetPool;

    private int currentPoolSize;

    private void Awake()
    {
        targetPool = new List<GameObject>();
        currentPoolSize = 0;
    }

    void Update()
    {
        //Spawn the next target if the timer has reached the spawn interval
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; // Reset the timer
        }
    }

            GameObject newTarget = Instantiate(target);

        GameObject newTarget = targetPool[0];


}
