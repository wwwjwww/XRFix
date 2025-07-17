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


    private static GameObject enemyPool;

    private static ObjectPool<Enemy> enemyPoolObject;

    private static int maxEnemies = 5;

    void Start()
    {
        enemyPool = new GameObject("EnemyPool");
        enemyPoolObject = new ObjectPool<Enemy>(enemyPool,
            () =>
            {
                var prefab = Resources.Load<GameObject>("Prefabs/Enemy");
                var instantiated = Instantiate(prefab);
                instantiated.name = "Enemy";
                return instantiated.GetComponent<Enemy>();
            },
            maxEnemies);
    }

    void Update()
    {
        enemyLifetime -= Time.deltaTime;
        if (enemyLifetime <= 0f)
        {
            enemyPoolObject.Release(this);
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
