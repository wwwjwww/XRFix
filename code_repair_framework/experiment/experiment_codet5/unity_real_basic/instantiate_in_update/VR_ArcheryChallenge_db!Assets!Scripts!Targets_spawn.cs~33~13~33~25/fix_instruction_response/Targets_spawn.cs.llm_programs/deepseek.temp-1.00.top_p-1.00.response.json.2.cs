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

        for(int i = 0; i < maxTargets; i++) // new loop to generate pool
        {
            GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
            newTarget.SetActive(false); // deactivate initially
            targetPool.Add(newTarget); // add to pool
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





    private List<GameObject> targetPool = new List<GameObject>(); // new line

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GetActiveTargetsCount() < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; 
        }
    }

    void SpawnTarget()
    {
        foreach(GameObject tar in targetPool) // go through pool
        {
            if (!tar.activeInHierarchy) // if not already active
            {
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),  
                    UnityEngine.Random.Range(1f, 5f),       
                    0);                                     

                tar.SetActive(true); // activate object
                tar.transform.position = spawnPosition; // set position
                tar.tag = "Target"; // reset tag
                tar.GetComponent<Targets_movements>().level = level; // reset level
                return; // exit function
            }
        }
    }

    int GetActiveTargetsCount() // new function to count active targets
    {
        int count = 0;
        foreach(GameObject tar in targetPool)
        {
            if(tar.activeInHierarchy) count++;
        }
        return count;
    }


}
