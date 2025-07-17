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
        level = PlayerPrefs.GetString("level");

        // Initialize the object pool
        targetPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            targetPool.Enqueue(obj);
        }

        // Start the spawn coroutine
        StartCoroutine(SpawnCoroutine());
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





    public string level; // Integer representing the difficulty level

    private Queue<GameObject> targetPool;

    public int poolSize = 20; // Initial pool size

    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
            {
                SpawnTarget();
            }
        }
    }

    void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),
            UnityEngine.Random.Range(1f, 5f),
            0);

        GameObject newTarget;

        if (targetPool.Count > 0)
        {
            newTarget = targetPool.Dequeue();
            newTarget.transform.position = spawnPosition;
            newTarget.transform.rotation = Quaternion.identity;
        }
        else
        {
            newTarget = Instantiate(target, spawnPosition, Quaternion.identity);
            Debug.LogWarning("Expanded target pool. Consider increasing initial pool size.");
        }

        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
        newTarget.SetActive(true);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        targetPool.Enqueue(obj);
    }


}
