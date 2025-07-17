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
        InitializePool();
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





    private List<GameObject> targetPool = new List<GameObject>();

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f; 
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
            newTarget.SetActive(false);
            targetPool.Add(newTarget);
        }
    }

    void SpawnTarget()
    {
        for (int i = 0; i < maxTargets; i++)
        {
            if (!targetPool[i].activeInHierarchy)
            {
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),  
                    UnityEngine.Random.Range(1f, 5f),       
                    0);                                     

                targetPool[i].SetActive(true);
                targetPool[i].transform.position = spawnPosition;
                targetPool[i].tag = "Target";
                targetPool[i].GetComponent<Targets_movements>().level = level;
                return;
            }
        }
    }


}
