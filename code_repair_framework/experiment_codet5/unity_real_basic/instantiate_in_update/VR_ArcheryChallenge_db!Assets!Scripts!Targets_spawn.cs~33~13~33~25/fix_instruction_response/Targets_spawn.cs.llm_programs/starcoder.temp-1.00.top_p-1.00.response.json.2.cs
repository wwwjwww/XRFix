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
        m_targetPool = new Queue<GameObject>();
        level = PlayerPrefs.GetString("level"); 
        for (int i = 0; i < TargetPoolSize; i++)
        {
            var obj = new GameObject("TargetPool");
            obj.transform.SetParent(transform);
            obj.SetActive(false);
            m_targetPool.Enqueue(obj);
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





    public int TargetPoolSize = 10;

    private Queue<GameObject> m_targetPool;


}
