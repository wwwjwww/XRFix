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
    // ... your existing code ...
    
    // Instantiate a pool of targets initially
    targetPool = new Queue<GameObject>(maxTargets);
    for (int i = 0; i < maxTargets; i++)
    {
        GameObject newTarget = Instantiate(target, Vector3.zero, Quaternion.identity);
        newTarget.SetActive(false); // Deactivates the object to start
        targetPool.Enqueue(newTarget); // Adds the object to the pool
    }
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





void Update()
{
    spawnTimer += Time.deltaTime;

    if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
    {
        if (targetPool.Count > 0) // Ensure we have an available object in the pool
        {
            GameObject newTarget = targetPool.Dequeue(); // Retrieve an object from the pool
            newTarget.SetActive(true); // Activates the object

            // Adjust spawn position and other target properties
            newTarget.transform.position = new Vector3(
                UnityEngine.Random.Range(-spawnRange, spawnRange),  
                UnityEngine.Random.Range(1f, 5f),       
                0);                                         

            newTarget.tag = "Target";
            newTarget.GetComponent<Targets_movements>().level = level;

            spawnTimer = 0f; // Reset the timer
        }
    }
}


}
