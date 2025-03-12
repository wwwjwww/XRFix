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
    StartCoroutine(SpawnTargets());
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





private GameObject newTarget;

IEnumerator SpawnTargets()
{
    while (GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
    {
        yield return new WaitForSeconds(spawnInterval);
        
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  
            UnityEngine.Random.Range(1f, 5f),       
            0);                                     

        newTarget = Instantiate(target, spawnPosition, Quaternionentity.identity);
        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
    }
}


}
