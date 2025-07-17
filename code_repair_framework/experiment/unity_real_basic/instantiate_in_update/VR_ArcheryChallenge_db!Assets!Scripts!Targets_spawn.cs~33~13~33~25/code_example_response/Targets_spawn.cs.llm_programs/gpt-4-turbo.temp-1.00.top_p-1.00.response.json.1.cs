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
        InitializeTargetPool();
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





    private List<GameObject> targetPool;

    private int poolSize = 20; 

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
            spawnTimer = 0f;
        }
    }

    private void InitializeTargetPool()
    {
        targetPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            targetPool.Add(obj);
        }
    }

    private GameObject GetPooledTarget()
    {
        foreach (GameObject obj in targetPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        
        // Expand the pool if needed
        GameObject extraObj = Instantiate(target);
        extraObj.SetActive(false);
        targetPool.Add(extraObj);
        Debug.LogWarning("Expanded target pool. Consider increasing initial pool size.");
        return extraObj;
    }

    private void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  
            UnityEngine.Random.Range(1f, 5f),       
            0);                                    
        
        GameObject newTarget = GetPooledTarget();
        newTarget.transform.position = spawnPosition;
        newTarget.transform.rotation = Quaternion.identity;
        newTarget.SetActive(true);

        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
    }


}
