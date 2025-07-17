using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 

///     void Update()
//     {
// 
//         enemyLifetime -= Time.deltaTime;
//         if (enemyLifetime <= 0f) {
            //             Destroy(this.gameObject);
            //         } 
            //     }

            // FIXED CODE:


    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime >= enemyLifetime)
        {
            Destroy(this.gameObject);
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
