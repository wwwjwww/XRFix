using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // The original prefab for the bullet
    public float spawnRateMin = 0.5f; // Minimum spawn rate
    public float spawnRateMax = 3f; // Maximum spawn rate

    private Transform target; // The target to shoot at
    private float spawnRate; // The spawn rate
    private float timeAfterSpawn; // The time after the last spawn


    void Start()
    {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        target = FindObjectOfType<PlayerController>().transform;

        SpawnBullet(); // Spawn the bullet at the start
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
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
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
            SpawnBullet();
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }

    void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.transform.LookAt(target);
    }


}
