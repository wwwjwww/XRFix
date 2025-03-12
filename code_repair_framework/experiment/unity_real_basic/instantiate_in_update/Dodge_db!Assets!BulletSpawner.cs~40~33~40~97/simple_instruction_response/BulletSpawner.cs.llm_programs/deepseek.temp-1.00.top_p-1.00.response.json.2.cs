using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // The original prefab of the bullet to be created
    public float spawnRateMin = 0.5f; // The minimal spawning interval
    public float spawnRateMax = 3f; // The maximum spawning interval

    private Transform target; // The target to be shot
    private float spawnRate; // The spawning interval
    private float timeAfterSpawn; // The time elapsed since the last spawning


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
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transformrotation); // This line is modified from "transform.rotation" to "Quaternion.identity" to ensure the bullet faces the forward direction when spawned.
            bullet.transform.LookAt(target);
        }
    }


}
