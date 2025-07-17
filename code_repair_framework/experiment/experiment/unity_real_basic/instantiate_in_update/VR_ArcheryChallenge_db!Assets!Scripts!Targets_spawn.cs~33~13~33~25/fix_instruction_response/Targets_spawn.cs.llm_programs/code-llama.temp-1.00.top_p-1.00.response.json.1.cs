using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System;

public class Targets : MonoBehaviour
{
    public GameObject target;
    public String level;
    public float spawnInterval = 3f;
    public int maxTargets = 10;

    private float spawnTimer = 0f;
    private float spawnRange = 5f;

    void Start()
    {
        level = PlayerPrefs.GetString("level");
        
        // Build an object pool for the target objects
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target);
            newTarget.tag = "Target";
            newTarget.GetComponent<Targets_movements>().level = level;
        }
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
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f;
        }
    }

    void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),
            UnityEngine.Random.Range(1f, 5f),
            0);

        // Instantiate a new target from the object pool
        GameObject newTarget = GameObject.FindGameObjectWithTag("Target").GetComponent<Targets_movements>().Instantiate();

        newTarget.transform.position = spawnPosition;
        newTarget.transform.rotation = Quaternion.identity;

        newTarget.tag = "Target";

        GameObject.FindGameObjectWithTag("Target").GetComponent<Targets_movements>().level = level;
    }


}
