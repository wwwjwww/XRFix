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

    for (int i = 0; i < maxTargets; i++)
    {
        GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
        newTarget.SetActive(false);
        objectPool.Enqueue(newTarget);
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





private Queue<GameObject> objectPool = new Queue<GameObject>();

private GameObject GetPooledObject()
{
    if (objectPool.Count == 0)
    {
        return null;
    }

    GameObject target = objectPool.Dequeue();
    target.SetActive(true);
    return target;
}

void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval)
    {
        GameObject spawnedTarget = GetPooledObject();

        if (spawnedTarget != null)
        {
            Vector3 spawnPosition = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),
                UnityEngine.Random.Range(1f, 5f),
                0);

            spawnedTarget.transform.position = spawnPosition;
            objectPool.Enqueue(spawnedTarget);

            spawnTimer = 0f;
        }
    }
}

}
