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
        objectPool = GetComponent<ObjectPool>();
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
    if(timeAfterSpawn >= spawnRate)
    {
        timeAfterSpawn = 0f;
        GameObject bullet = ObjectPool.Instance.GetBullet();
        bullet.transform.LookAt(target);
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }
}

    private ObjectPool objectPool;

    void Update()
    {
        if(timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            GameObject bullet = objectPool.GetBullet();
            bullet.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }

        private List<GameObject> bulletObjects;

        private int maxPoolSize;

        public GameObject GetBullet()
        {
            if(bulletObjects.Count == 0)
            {
                return Instantiate(bulletPrefab);
            }
            else
            {
                return bulletObjects.RemoveAt(0);
            }
        }


}
