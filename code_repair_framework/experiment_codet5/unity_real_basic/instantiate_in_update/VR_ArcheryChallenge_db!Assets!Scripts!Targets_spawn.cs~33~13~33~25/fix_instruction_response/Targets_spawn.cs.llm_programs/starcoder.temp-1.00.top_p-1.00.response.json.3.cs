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
    private float spawnRange = 5f; // Range for spawning

    void Start()
    {
        level = PlayerPrefs.GetString("level");
        targetPool = new Queue<GameObject>();

        for (int i = 0; i < maxTargets; i++)
        {
            CreateTarget();
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





    private Queue<GameObject> targetPool;

    private string level;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnTarget();
            spawnTimer = 0f; 
        }
    }

    void SpawnTarget()
    {
        if (targetPool.Count > 0)
        {
            GameObject target = targetPool.Dequeue();
            target.transform.position = GetRandomPosition();
            target.SetActive(true);
        }
        else
        {
            CreateTarget();
        }
    }

    void CreateTarget()
    {
        GameObject newTarget = Instantiate(Resources.Load<GameObject>("Prefabs/Target")),
            transform;

        transform = newTarget.transform;
        transform.SetParent(transform, false);
        transform.localPosition = GetRandomPosition();
        transform.localRotation = Quaternion.identity;
        newTarget.SetActive(false);
        targetPool.Enqueue(newTarget);
    }


}
