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


    private float currentLifetime = 0f;

    void Start()
    {
        currentLifetime = enemyLifetime;
    }

    void Update()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0f) {
            gameObject.SetActive(false);
            // You can put the gameObject back into the pool instead of destroying it
            // Or you can do something else to release the gameObject
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
