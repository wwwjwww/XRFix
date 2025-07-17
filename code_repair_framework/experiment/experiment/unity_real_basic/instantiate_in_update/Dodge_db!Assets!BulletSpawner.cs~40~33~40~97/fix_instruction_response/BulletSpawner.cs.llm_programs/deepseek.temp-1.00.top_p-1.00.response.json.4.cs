using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; 
    public float spawnRateMin = 0.5f; 
    public float spawnRateMax = 3f; 

    private Transform target; 
    private float spawnRate; 
    private float timeAfterSpawn; 


    void Start()
    {
        bulletPool = new List<GameObject>();
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        target = FindObjectOfType<PlayerController>().transform;

        // Create a pool of bullets
        for(int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
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


    public int poolSize = 10;

    private List<GameObject> bulletPool;

    private int currentBullet = 0;

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
        if(bulletPool[currentBullet].activeInHierarchy == false)
        {
            bulletPool[currentBullet].SetActive(true);
            bulletPool[currentBullet].transform.position = transform.position;
            bulletPool[currentBullet].transform.LookAt(target);
        }
        currentBullet++;
        if(currentBullet >= bulletPool.Count)
        {
            currentBullet = 0;
        }
    }


}
