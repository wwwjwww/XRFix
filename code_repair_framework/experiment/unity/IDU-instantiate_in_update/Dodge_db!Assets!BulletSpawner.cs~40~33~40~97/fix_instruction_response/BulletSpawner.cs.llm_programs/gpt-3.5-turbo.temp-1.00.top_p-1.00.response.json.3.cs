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


void Update()
{
    timeAfterSpawn += Time.deltaTime;

    if (timeAfterSpawn >= spawnRate)
    {
        timeAfterSpawn = 0f;

        StartCoroutine(SpawnBullet());
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
    }
}

IEnumerator SpawnBullet()
{
    GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
    bullet.transform.LookAt(target);
    yield return null;
}


}
