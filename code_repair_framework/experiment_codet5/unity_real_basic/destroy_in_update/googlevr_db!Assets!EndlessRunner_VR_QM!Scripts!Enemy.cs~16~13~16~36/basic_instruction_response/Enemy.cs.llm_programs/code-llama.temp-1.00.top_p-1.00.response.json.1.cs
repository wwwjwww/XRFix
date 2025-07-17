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


    void Update()
    {
        enemyLifetime -= Time.deltaTime;
        if (enemyLifetime <= 0f)
        {
            // Instantiate a new WaitForSeconds object to wait before destroying the enemy
            WaitForSeconds delay = new WaitForSeconds(1f);
            // Use coroutines to destroy the enemy after a certain amount of time
            StartCoroutine(DestroyEnemy(delay));
        }
    }

    IEnumerator DestroyEnemy(WaitForSeconds delay)
    {
        // Wait for the specified amount of time
        yield return delay;
        // Destroy the enemy
        Destroy(this.gameObject);
    }




    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }
    
 
}
