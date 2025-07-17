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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private float timer;

    void Start()
    {
        // Initialize the timer variable at the start
        timer = enemyLifetime;
    }

    void Update()
    {
        // Decrease the timer per frame
        timer -= Time.deltaTime;

        // Check if timer has reached zero outside of the if condition
        if (timer <= 0f)
        {
            Deactivate();
        }
    }

    void Deactivate()
    {
        // Deactivate the gameObject instead of destroying it
        this.gameObject.SetActive(false);
        // Alternatively object pooling logic can be implemented here
    }




    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }

    
    
 
}
