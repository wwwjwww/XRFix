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

public void Start()
{
    InitObjectPool();
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





public void InitObjectPool()
{
    targetPool = GameObject.Find("TargetPool") as GameObject;
    if (targetPool != null)
    {
        targetPool.SetActive(false);
    }
    else
    {
        targetPool = new GameObject();
        targetPool.name = "TargetPool";
        targetPool.tag = "Pool";
        GameObject target = Instantiate(target) as GameObject;
        target.transform.parent = targetPool.transform;
        targetPool.SetActive(true);
    }
}

void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Vector3 spawnPosition = new Vector3(Mathf.Clamp(transform.position.x, -spawnRange, spawnRange),
                                            Mathf.Clamp(transform.position.y, 1f, 5f),
                                            0);
        GameObject newTarget = Instantiate(targetPool) as GameObject;
        newTarget.transform.position = spawnPosition;
        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
    }
}


}
