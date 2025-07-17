using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 




    private void Start()
    {
        // FIXED CODE: Set the enemy's lifetime based on the random value generated in the previous code snippet
        enemyLifetime = Random.Range(1f, 10f);
    }

    private void Update()
    {
        // FIXED CODE: Update the enemy's lifetime by decrementing it by 1 second every frame, and destroy the enemy when its lifetime reaches 0
        enemyLifetime -= Time.deltaTime;
        if (enemyLifetime <= 0)
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
