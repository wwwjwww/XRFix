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

    // Populate object pool
    for (int i = 0; i < maxTargets; i++)
    {
        GameObject newObject = Instantiate(target, Vector3.zero, Quaternion.identity);
        newObject.SetActive(false);
        objectPool.Enqueue(newObject);
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

void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval && objectPool.Count > 0)
    {
        GameObject newTarget = objectPool.Dequeue();
        SpawnTarget(newTarget);
        spawnTimer = 0f;
    }
}

void SpawnTarget(GameObject newTarget)
{
    newTarget.SetActive(true);
    
    Vector3 spawnPosition = new Vector3(
        UnityEngine.Random.Range(-spawnRange, spawnRange),
        UnityEngine.Random.Range(1f, 5f),
        0);

    newTarget.transform.position = spawnPosition;

    newTarget.tag = "Target";

    newTarget.GetComponent<Targets_movements>().level = level;
}

}
