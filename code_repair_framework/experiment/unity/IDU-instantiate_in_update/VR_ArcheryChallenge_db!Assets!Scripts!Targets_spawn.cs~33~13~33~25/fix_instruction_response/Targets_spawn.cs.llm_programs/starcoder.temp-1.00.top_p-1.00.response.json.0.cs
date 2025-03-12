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

    private void Start()
    {
        _targetPool = new ObjectPool(target, maxTargets);
        _spawnTimer = 0f;
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





    private ObjectPool _targetPool;

    private float _spawnTimer;

    void Update()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnInterval)
        {
            SpawnTarget();
            _spawnTimer = 0f;
        }
    }

    private void SpawnTarget()
    {
        var newTarget = _targetPool.GetObject();
        if (newTarget == null)
        {
            return;
        }

        var spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),
            UnityEngine.Random.Range(1f, 5f),
            0);

        newTarget.transform.position = spawnPosition;

        newTarget.transform.SetParent(transform);

        newTarget.SetActive(true);

        var movements = newTarget.GetComponent<Targets_movements>();
        if (movements!= null)
        {
            movements.level = level;
        }
    }


}
