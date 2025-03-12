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
			if ( balloonPrefab == null )
			{
				return;
			}

			if ( autoSpawn && spawnAtStartup )
			{
				SpawnBalloon( color );
				nextSpawnTime = Random.Range( minSpawnTime, maxSpawnTime ) + Time.time;
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







void Update()
{
	if (balloonPrefab == null)
	{
		return;
	}

	if (Time.time > nextSpawnTime && autoSpawn)
	{
		// Call SpawnBalloon() method and pass in the desired color.
		SpawnBalloon(color);
		// Update the nextSpawnTime variable with a new random value.
		nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
	}
}

public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
{
	if (balloonPrefab == null)
	{
		return null;
	}

	// Create a new instance of the balloon prefab and assign it to a variable.
	GameObject balloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
	// Set the scale of the balloon.
	balloon.transform.localScale = new Vector3(scale, scale, scale);
	// If attachBalloon is true, attach the balloon to the parent transform.
	if (attachBalloon)
	{
		balloon.transform.parent = transform;
	}

	if (sendSpawnMessageToParent)
	{
		// If the parent transform is not null, send a message to it.
		if (transform.parent != null)
		{
			transform.parent.SendMessage("OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver);
		}
	}

	if (playSounds)
	{
		// Play the inflate sound effect.
		if (inflateSound != null)
		{
			inflateSound.Play();
		}
		// Play the stretch sound effect.
		if (stretchSound != null)
		{
			stretchSound.Play();
		}
	}

	// Set the color of the Balloon component.
	balloon.GetComponentInChildren<Balloon>().SetColor(color);
	// If there is a spawnDirectionTransform, add a force to the balloon.
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
