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

        for (int i = 0; i < maxTargets; i++)
        {
            SpawnTarget();
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





    void SpawnTarget()
    {
        
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  
            UnityEngine.Random.Range(1f, 5f),       
            0);                                     

        GameObject newTarget = Instantiate(target, spawnPosition, Quaternion.identity);

        
        newTarget.tag = "Target";

        
        newTarget.GetComponent<Targets_movements>().level = level;
    }


}
