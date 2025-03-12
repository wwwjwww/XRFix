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


    private float lifeTime;

    void Start()
    {
        lifeTime = enemyLifetime;
    }

    void Update()
    {
        if (lifeTime <= 0f) 
        {
            // If lifetime has passed, object gets destroyed
            Destroy(this.gameObject);
        } 
        else 
        {
            // If lifetime not yet reached, subtract current frame time from lifetime
            lifeTime -= Time.deltaTime;
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
