using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 




    float currentTime = 0f;

    float lifetime = enemyLifetime;

    void Update()
    {
        currentTime += Time.deltaTime;
        
        if (currentTime >= lifetime)
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
