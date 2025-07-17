using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab; // 생성할 탄알의 원본 프리팹
    public float spawnRateMin = 0.5f; // 최소 생성 주기
    public float spawnRateMax = 3f; // 최대 생성 주기

    private Transform target; // 발사할 대상
    private float spawnRate; // 생성 주기
    private float timeAfterSpawn; // 최근 생성 시점에서 지난 시간


    void Start()
    {

        timeAfterSpawn = 0f;

        spawnRate = Random.Range(spawnRateMin, spawnRateMax);

        target = FindObjectOfType<PlayerController>().transform;

    }





void Update()
{
    timeAfterSpawn += Time.deltaTime;
 
    if(timeAfterSpawn >= spawnRate) // 생성 주기만큼 지났는가?
    {
        timeAfterSpawn = 0f;  // 기다린 시간 초기화

        // 랜덤 위치로 탄알 생성
        Vector3 spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);

        // 원본에서 탄알을 복제, 생성 위치에 배치, 회전 x
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // 발사 대상 지정
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Seek(target);
        }

        spawnRate = Random.Range(spawnRateMin, spawnRateMax); // 다음 생성 주기 설정
    }
}


}
