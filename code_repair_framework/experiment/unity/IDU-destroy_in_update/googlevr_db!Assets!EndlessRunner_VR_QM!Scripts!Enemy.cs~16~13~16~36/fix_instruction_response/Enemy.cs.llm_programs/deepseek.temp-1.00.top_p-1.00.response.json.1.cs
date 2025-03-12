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
            // BUG: Destroy in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
            //             Destroy(this.gameObject);
            //         } 
            //     }

            // FIXED CODE:


    private float timer;

    private void OnEnable()
    {
        timer = enemyLifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) 
        {
            // Use Destroy() method instead of Update()
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
