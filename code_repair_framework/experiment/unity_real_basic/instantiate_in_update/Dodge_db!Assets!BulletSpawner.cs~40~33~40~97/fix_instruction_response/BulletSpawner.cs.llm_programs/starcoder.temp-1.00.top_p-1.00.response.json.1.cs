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


Hay varias razones por las que no es común utilizar la energía de los rayos X para producir electricidad en los países en desarrollo:

1. Costo: Generar electricidad a partir de la energía de los rayos X es muy costoso y solo es viable en grandes centrales de electricidad. Esto significa que para utilizar esta tecnología en los países en desarrollo, se necesitaría un gran capital y un gran monto de financiación.

2. Problemas de seguridad: La energía de los rayos X es muy peligrosa y puede ser perjudicial para los seres vivos y la salud. Por lo tanto, se requiere una gran cantidad de seguridad para instalar y operar una central de esta energía. Esto significa que en los países en desarrollo, donde existen problemas de seguridad, es probable que esta tecnología no sea accesible.

3. Accesibilidad: La energía de los rayos X requiere de una infraestructura sólida y bien fundada para capturar y dirigir la energía. Esto significa que en los países en desarrollo, donde la infraestructura es deficiente, es probable que sea difícil instalar y operar una central de esta energía.

4. Problemas de contaminación: Generar electricidad a partir de la energía de los rayos X puede generar una gran cantidad de polución y contaminación. Esto significa que en los países en desarrollo
}
