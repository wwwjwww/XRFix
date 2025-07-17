using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // The bullet prefab to instantiate
    public float spawnRateMin = 0.5f; // Minimum spawn rate
    public float spawnRateMax = 3f; // Maximum spawn rate

    private Transform target; // Target to shoot at
    private float spawnRate; // Random spawn rate
    private float timeAfterSpawn; // Time elapsed since last spawn


    void Start()
    {
        // Initialize variables
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        // Find target
        target = FindObjectOfType<PlayerController>().transform;
    }


///     void Update()
//     {
// 
//         timeAfterSpawn += Time.deltaTime;
// 
// 
//         if (timeAfterSpawn >= spawnRate) {
// 
//             timeAfterSpawn = 0f;
// 
// 
// 
            //             GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            // 
            //             bullet.transform.LookAt(target);
            // 
            //             spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            // 
            //         }
            // 
            //         
            //     }

            // FIXED CODE:


    void Update()
    {
        timeAfterSpawn += Time.deltaTime;

        // Check if enough time has passed since last spawn
        if (timeAfterSpawn >= spawnRate) 
        {
            // Reset timer and spawn bullet
            timeAfterSpawn = 0f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }


}
