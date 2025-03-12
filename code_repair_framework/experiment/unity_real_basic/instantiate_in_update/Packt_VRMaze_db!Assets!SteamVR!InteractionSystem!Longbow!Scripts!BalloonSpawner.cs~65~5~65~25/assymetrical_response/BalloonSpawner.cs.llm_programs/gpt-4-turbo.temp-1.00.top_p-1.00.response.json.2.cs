//======= Copyright (c) Valve Corporation, All rights reserved. ===============





using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class BalloonSpawner : MonoBehaviour
	{
        public float minSpawnTime = 5f;
        public float maxSpawnTime = 15f;
        private float nextSpawnTime;
        public GameObject balloonPrefab;

        public bool autoSpawn = true;
        public bool spawnAtStartup = true;

        public bool playSounds = true;
        public SoundPlayOneshot inflateSound;
        public SoundPlayOneshot stretchSound;

        public bool sendSpawnMessageToParent = false;

        public float scale = 1f;

        public Transform spawnDirectionTransform;
        public float spawnForce;

        public bool attachBalloon = false;

        public Balloon.BalloonColor color = Balloon.BalloonColor.Random;



        void Start()
        {
            if (balloonPrefab == null)
            {
                return;
            }

            InitializeBalloonPool();
            
            if (autoSpawn && spawnAtStartup)
            {
                SpawnBalloon(color);
                nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
            }
        }




/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
* 		void Update()
* 		{
* 			if ( balloonPrefab == null )
* 			{
* 				return;
* 			}
* 
* 			if ( ( Time.time > nextSpawnTime ) && autoSpawn )
* 			{
* 				SpawnBalloon( color );
* 				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
* 			}
* 		}







        private Queue<GameObject> balloonPool;

        public int poolSize = 10;

        void Update()
        {
            if (balloonPrefab == null)
            {
                return;
            }

            if ((Time.time > nextSpawnTime) && autoSpawn)
            {
                SpawnBalloon(color);
                nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
            }
        }

        private void InitializeBalloonPool()
        {
            balloonPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject balloon = Instantiate(balloonPrefab);
                balloon.SetActive(false);
                balloonPool.Enqueue(balloon);
            }
        }

        public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
        {
            if (balloonPool.Count == 0)
            {
                Debug.LogWarning("Balloon pool is empty. Consider increasing pool size.");
                return null;
            }

            GameObject balloon = balloonPool.Dequeue();
            balloon.transform.position = transform.position;
            balloon.transform.rotation = transform.rotation;
            balloon.SetActive(true);
            balloon.transform.localScale = new Vector3(scale, scale, scale);

            if (attachBalloon)
            {
                balloon.transform.parent = transform;
            }

            if (sendSpawnMessageToParent)
            {
                if (transform.parent != null)
                {
                    transform.parent.SendMessage("OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver);
                }
            }

            if (playSounds)
            {
                if (inflateSound != null)
                {
                    inflateSound.Play();
                }
                if (stretchSound != null)
                {
                    stretchSound.Play();
                }
            }
            balloon.GetComponentInChildren<Balloon>().SetColor(color);
            if (spawnDirectionTransform != null)
            {
                balloon.GetComponentInChildren<Rigidbody>().AddForce(spawnDirectionTransform.forward * spawnForce);
            }

            balloonPool.Enqueue(balloon);
            return balloon;
        }




		//-------------------------------------------------
		public void SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			SpawnBalloon( (Balloon.BalloonColor)color );
		}
	}
}
