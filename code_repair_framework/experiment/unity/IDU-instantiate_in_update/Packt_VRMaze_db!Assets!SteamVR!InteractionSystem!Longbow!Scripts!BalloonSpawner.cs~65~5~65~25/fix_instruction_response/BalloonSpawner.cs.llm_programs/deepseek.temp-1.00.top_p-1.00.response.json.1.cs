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

			balloonPool = new List<GameObject>();
			for (int i = 0; i < poolAmount; i++)
			{
				GameObject balloon = Instantiate(balloonPrefab, transform.position, transform.rotation);
				balloon.SetActive(false);
				balloonPool.Add(balloon);
			}

			if (autoSpawn && spawnAtStartup)
			{
				SpawnBalloon(color);
				nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
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







		private List<GameObject> balloonPool;

		private int poolAmount = 10;  // Adjust depending on your game's performance requirements

		void Update()
		{
			if ((Time.time > nextSpawnTime) && autoSpawn && balloonPool.Count > 0)
			{
				SpawnBalloon(color);
				nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
			}
		}

		public void SpawnBalloon(Balloon.BalloonColor color)
        {
			if (balloonPool.Count > 0)
			{
				GameObject balloon = balloonPool[0];
				balloonPool.RemoveAt(0);
				balloon.SetActive(true);
				balloon.transform.position = transform.position;
				balloon.transform.rotation = transform.rotation;
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
			}
		}




		//-------------------------------------------------
		public void SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			SpawnBalloon( (Balloon.BalloonColor)color );
		}
	}
}
