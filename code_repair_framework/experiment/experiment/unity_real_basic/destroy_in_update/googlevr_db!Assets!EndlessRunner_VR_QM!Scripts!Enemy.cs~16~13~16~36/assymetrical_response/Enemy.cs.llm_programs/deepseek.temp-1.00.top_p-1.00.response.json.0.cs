using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 




    float spawnTime;

    void Start()
    {
        //Remember the time we were spawned
        spawnTime = Time.time;
    }

    void Update()
    {
        //Destroy the enemy if its lifetime (spawntime + lifetime) has been exceeded
        if (Time.time > spawnTime + enemyLifetime)
        {
            Destroy(gameObject);
        }
    }




    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }
    
 
}
