using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyLifetime = 6f;
    
 




cs
using System.Collections;

    protected float _enemyLifetime;

    void Start()
    {
        _enemyLifetime = enemyLifetime;
    }

    void Update()
    {
        if (_enemyLifetime <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            _enemyLifetime -= Time.deltaTime;
        }
    }

    public float GetEnemyLifetime()
    {
        return _enemyLifetime;
    }




    // Remove (Destroy) Enemy on hit
    private void OnTriggerEnter(Collider other) {
        if (other.transform.name == "BikeColliderTarget") {
            //Debug.Log("Enemy hit bike");
            Destroy(this.gameObject);
        }
    }
    
 
}
