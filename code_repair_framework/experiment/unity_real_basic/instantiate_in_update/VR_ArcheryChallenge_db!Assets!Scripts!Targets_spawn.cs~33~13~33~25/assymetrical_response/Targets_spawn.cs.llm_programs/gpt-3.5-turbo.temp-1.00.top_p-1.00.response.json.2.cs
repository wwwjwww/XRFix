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
    level = PlayerPrefs.GetString("level"); // Default to level 1 ("easy")
    PopulateTargetPool();
}


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     void Update()
*     {
*         spawnTimer += Time.deltaTime;
* 
* 
*         if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
*         {
*             SpawnTarget();
*             spawnTimer = 0f; // Reset the timer
*         }
*     }





private List<GameObject> targetPool = new List<GameObject>();

void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval && targetPool.Count < maxTargets)
    {
        SpawnTarget();
        spawnTimer = 0f; // Reset the timer
    }
}

void PopulateTargetPool()
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
    GameObject newTarget = GetAvailableTarget();
    newTarget.transform.position = new Vector3(
        UnityEngine.Random.Range(-spawnRange, spawnRange),
        UnityEngine.Random.Range(1f, 5f),
        0
    );
    newTarget.SetActive(true);
    newTarget.GetComponent<Targets_movements>().level = level;
}

GameObject GetAvailableTarget()
{
    for (int i = 0; i < targetPool.Count; i++)
    {
        if (!targetPool[i].activeSelf)
        {
            return targetPool[i];
        }
    }
    return null;
}


}
