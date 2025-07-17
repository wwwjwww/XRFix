using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 




public Rigidbody rb;

public float speed = 5f;

private void Start()
{
    rb = GetComponent<Rigidbody>();
}

private void FixedUpdate()
{
    rb.AddForce(transform.forward * speed); // Adjust the direction by modifying 'transform.forward'.
}

private void Start()
{
    Destroy(gameObject, enemyLifetime);
}




    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }
    
 
}
