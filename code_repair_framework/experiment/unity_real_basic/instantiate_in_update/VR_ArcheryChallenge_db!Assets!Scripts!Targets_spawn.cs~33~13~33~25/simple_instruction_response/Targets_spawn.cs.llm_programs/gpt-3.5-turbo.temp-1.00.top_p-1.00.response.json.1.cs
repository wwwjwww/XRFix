using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System;

public class Targets : MonoBehaviour
{
    public GameObject target;
    public String level; // Integer representing the difficulty level
    public float spawnInterval = 3f; 
    public int maxTargets = 10; 

    private float spawnTimer = 0f;
    private float spawnRange = 5f; 

    void Start()
    {
        level = PlayerPrefs.GetString("level", "easy"); 
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





    public string level; 

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

        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);

        newTarget.tag = "Target";

        newTarget.GetComponent<Target_movements>().level = level;
    }


}
