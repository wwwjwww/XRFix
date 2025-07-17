using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    private Transform target;
    private float spawnRate; // 생성 주기
    private float timeAfterSpawn; // 최근 생성 시점에서 지난 시간


    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        StartCoroutine(SpawnCoroutine());
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


    IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            float spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            yield return new WaitForSeconds(spawnRate);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.transform.LookAt(target);
        }
    }


}
