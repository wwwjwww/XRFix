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


Doesn't Implement Update Function. ERROR


    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }
    
 
}
