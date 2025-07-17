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
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        target = FindObjectOfType<PlayerController>().transform;
        
        bulletPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private Queue<GameObject> bulletPool;

    public int poolSize = 10;

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
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            bullet.transform.LookAt(target);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10; // adjust speed as needed
        }
    }

    public void ReturnBulletToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }


}
