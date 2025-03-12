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

        // Pre-instantiate target objects and store them in the pool
        for (int i = 0; i < maxTargets; i++)
        {
            GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
            newTarget.tag = "Target";
            newTarget.SetActive(false); // Deactivate the target
            newTarget.GetComponent<Targets_movements>().level = level;
            targetPool.Enqueue(newTarget);
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





    private Queue<GameObject> targetPool = new Queue<GameObject>();

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            ActivateTarget();
            spawnTimer = 0f; 
        }
    }

    void ActivateTarget()
    {
        if (targetPool.Count > 0)
        {
            GameObject targetToActivate = targetPool.Dequeue();
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),  
                UnityEngine.Random.Range(1f, 5f),       
                0);
            targetToActivate.transform.position = spawnPosition;
            targetToActivate.SetActive(true);
            targetPool.Enqueue(targetToActivate); // Re-enqueue to allow reuse
        }
    }


}
