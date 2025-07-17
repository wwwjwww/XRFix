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



public void Start()
{
    if (balloonPrefab == null)
    {
        return;
    }

    if (autoSpawn && spawnAtStartup)
    {
        SpawnBalloon(color);
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
    }

    // Initialize the object pool
    for (int i = 0; i < 10; i++)
    {
        GameObject newBalloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
        newBalloon.SetActive(false);
        balloonPool.Add(newBalloon);
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







List<GameObject> balloonPool = new List<GameObject>();

public void Update()
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

public GameObject SpawnBalloon(Balloon.BalloonColor color = Balloon.BalloonColor.Red)
{
    if (balloonPrefab == null)
    {
        return null;
    }

    // Check if there are deactivated balloons in the pool to reuse
    foreach (GameObject balloon in balloonPool)
    {
        if (!balloon.activeInHierarchy)
        {
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

            return balloon;
        }
    }

    // If no deactivated balloons are found, instantiate a new one
    GameObject newBalloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
    newBalloon.transform.localScale = new Vector3(scale, scale, scale);
    if (attachBalloon)
    {
        newBalloon.transform.parent = transform;
    }

    if (sendSpawnMessageToParent)
    {
        if (transform.parent != null)
        {
            transform.parent.SendMessage("OnBalloonSpawned", newBalloon, SendMessageOptions.DontRequireReceiver);
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
    newBalloon.GetComponentInChildren<Balloon>().SetColor(color);
    if (spawnDirectionTransform != null)
    {
        newBalloon.GetComponentInChildren<Rigidbody>().AddForce(spawnDirectionTransform.forward * spawnForce);
    }

    balloonPool.Add(newBalloon); // Add the new balloon to the pool

    return newBalloon;
}



		//-------------------------------------------------
		public void SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			SpawnBalloon( (Balloon.BalloonColor)color );
		}
	}
}
