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
        poolSize = maxTargets;
        CreateObjectPool();
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

    private int poolSize;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnTarget();
            spawnTimer = 0f;
        }
    }

    private void CreateObjectPool()
    {
        targetPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            obj.tag = "Target";
            obj.GetComponent<Targets_movements>().level = level;
            targetPool.Enqueue(obj);
        }
    }

    private void TrySpawnTarget()
    {
        if (GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
        {
            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        if (targetPool.Count > 0)
        {
            GameObject obj = targetPool.Dequeue();
            obj.transform.position = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),
                UnityEngine.Random.Range(1f, 5f),
                0
            );
            obj.SetActive(true);
        }
    }

    public void RecycleTarget(GameObject targetObj)
    {
        targetObj.SetActive(false);
        targetPool.Enqueue(targetObj);
    }


}
