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

            if (autoSpawn)
            {
                if (spawnAtStartup)
                {
                    SpawnBalloon(color);
                }
                StartSpawnCoroutine();
            }
        }




// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//		void Update()
//		{
//			if ( balloonPrefab == null )
//			{
//				return;
//			}
//
//			if ( ( Time.time > nextSpawnTime ) && autoSpawn )
//			{
//				SpawnBalloon( color );
//				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
//			}
//		}







        private Coroutine spawnCoroutine;

        void OnDisable()
        {
            StopSpawnCoroutine();
        }

        private void StartSpawnCoroutine()
        {
            if (spawnCoroutine == null)
            {
                spawnCoroutine = StartCoroutine(SpawnBalloonCoroutine());
            }
        }

        private void StopSpawnCoroutine()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnBalloonCoroutine()
        {
            while (autoSpawn)
            {
                nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
                yield return new WaitForSeconds(nextSpawnTime);

                if (balloonPrefab != null)
                {
                    SpawnBalloon(color);
                }
            }
        }

        public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
        {
            if (balloonPrefab == null)
            {
                return null;
            }

            GameObject balloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
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
