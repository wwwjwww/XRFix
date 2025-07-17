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
				Debug.LogError("BalloonPrefab is not set.");
				return;
			}

			if (autoSpawn && spawnAtStartup)
			{
				SpawnBalloon(color);
				nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
			}
		}




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

		public GameObject SpawnBalloon(Balloon.BalloonColor colorParam = Balloon.BalloonColor.Red)
		{
			if (balloonPrefab == null)
			{
				Debug.LogError("BalloonPrefab is not set.");
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
			balloon.GetComponentInChildren<Balloon>().SetColor(colorParam);
			if (spawnDirectionTransform != null)
			{
				Rigidbody balloonRigidbody = balloon.GetComponentInChildren<Rigidbody>();
				if (balloonRigidbody != null)
				{
					balloonRigidbody.AddForce(spawnDirectionTransform.forward * spawnForce);
				}
				else
				{
					Debug.LogError("Balloon does not have a Rigidbody component.");
				}
			}
			else
			{
				Debug.LogError("SpawnDirectionTransform is not set.");
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
