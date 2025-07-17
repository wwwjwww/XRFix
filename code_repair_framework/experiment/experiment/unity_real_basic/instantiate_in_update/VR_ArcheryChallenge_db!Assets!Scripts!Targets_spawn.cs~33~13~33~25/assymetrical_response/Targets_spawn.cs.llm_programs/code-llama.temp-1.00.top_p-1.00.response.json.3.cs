using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using System;

public class Targets : MonoBehaviour
{
        public GameObject target;
        public String level; // Integer representing the difficulty level
        public float spawnInterval = 3f; // Seconds between spawns
        public int maxTargets = 10; // Max targets allowed in scene

        private float spawnTimer = 0f;
        private float spawnRange = 5f; // Range for spawning

        void Start()
        {

            level = PlayerPrefs.GetString("level"); // Default to level 1 ("easy")

            // Initialize the object pool before Update() is called
            var pool = new GameObjectPool<GameObject>("Target Pool", 10);
            for (int i = 0; i <  pool.Capacity; i++)
            {
                pool.Spawn(target.GetComponent<Targets_movements>().level, new Vector3());
            }

        }


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     void Update()
*     {
*         spawnTimer += Time.deltaTime;
* 
* 
*         if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
*         {
*             SpawnTarget();
*             spawnTimer = 0f; // Reset the timer
*         }
*     }





        void Update()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval && GameObject.FindGameObjectsWithTag("Target").Length < maxTargets)
            {
                SpawnTarget(null);
                spawnTimer = 0f; // Reset the timer
            }
        }

        void SpawnTarget(GameObject newTarget)
        {
            string targetLevel = "easy"; // Default to level 1 ("easy")
            if (newTarget != null)
            {
                targetLevel = newTarget.GetComponent<Targets_movements>().level;
            }

            var spawnPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRange, spawnRange),  // Random x position
            UnityEngine.Random.Range(1f, 5f),       // Random y position between 1 and 5
            0);                                     // z axis is 0 for 2D

            var pool = new GameObjectPool<GameObject>("Target Pool", 10);
            pool.Spawn(targetLevel, spawnPosition);

            pool.GameObjects[0].GetComponent<Targets_movements>().level = level;
        }


}
