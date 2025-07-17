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





    private Stack<GameObject> targetPool;

    private void InitializeTargetPool()
    {
        targetPool = new Stack<GameObject>();

        for (int i = 0; i < maxTargets; i++)
        {
            GameObject obj = Instantiate(target);
            obj.SetActive(false);
            targetPool.Push(obj);
        }
    }

    private IEnumerator SpawnTargets()
    {
        while (true)
        {
            if (targetPool.Count > 0 && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
            {
                SpawnTarget();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnTarget()
    {
        Vector3 spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  
            UnityEngine.Random.Range(1f, 5f),       
            0);                                     

        GameObject newTarget = targetPool.Pop();
        newTarget.transform.position = spawnPosition;
        newTarget.transform.rotation = Quaternion.identity;
        newTarget.SetActive(true);

        newTarget.tag = "Target";
        newTarget.GetComponent<Targets_movements>().level = level;
    }

    public void ReturnTargetToPool(GameObject obj)
    {
        obj.SetActive(false);
        targetPool.Push(obj);
    }


}
