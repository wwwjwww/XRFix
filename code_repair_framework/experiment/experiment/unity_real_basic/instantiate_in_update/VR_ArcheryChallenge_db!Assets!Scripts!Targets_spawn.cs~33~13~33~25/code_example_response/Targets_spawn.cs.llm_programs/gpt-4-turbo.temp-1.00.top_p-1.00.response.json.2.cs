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
        targetPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            targetPool.Add(obj);
            obj.tag = "Target";
        }

        string level = PlayerPrefs.GetString("level", "easy");

        foreach (GameObject obj in targetPool)
        {
            obj.GetComponent<Targets_movements>().level = level;
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





    public int poolSize = 10; // Pool size for targets

    private List<GameObject> targetPool;

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
        for (int i = 0; i < targetPool.Count; i++)
        {
            if (!targetPool[i].activeInHierarchy)
            {
                Vector3 spawnPosition = new Vector3(
                    UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
                    UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
                    0);                                     // z axis is 0 for 2D

                targetPool[i].transform.position = spawnPosition;
                targetPool[i].SetActive(true);
                break;
            }
        }
    }


}
