using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*     void Update()
*     {
* 
*         timeAfterSpawn += Time.deltaTime;
* 
* 
*         if (timeAfterSpawn >= spawnRate) {
* 
*             timeAfterSpawn = 0f;
* 
* 
* 
            * BUG: Instantiate in Update() method
            * MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            *             GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            * 
            *             bullet.transform.LookAt(target);
            * 
            *             spawnRate = Random.Range(spawnRateMin, spawnRateMax);
            * 
            *         }
            * 
            *         
            *     }

            * you can try to build an object pool before Update() method has been called.
            * FIXED CODE:
            */
