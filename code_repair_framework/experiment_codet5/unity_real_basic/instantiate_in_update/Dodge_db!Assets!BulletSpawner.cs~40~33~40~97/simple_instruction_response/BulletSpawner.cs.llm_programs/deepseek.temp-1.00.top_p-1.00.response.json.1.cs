using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // The original prefab of the bullet to be created
    public float spawnRateMin = 0.5f; // Minimum spawning rate
    public float spawnRateMax = 3f; // Maximum spawning rate

    private Transform target; // The target to fire towards
    private float spawnRate; // Spawning rate
    private float timeAfterSpawn; // Time elapsed since the last creation


    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
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

        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target);

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }


}
