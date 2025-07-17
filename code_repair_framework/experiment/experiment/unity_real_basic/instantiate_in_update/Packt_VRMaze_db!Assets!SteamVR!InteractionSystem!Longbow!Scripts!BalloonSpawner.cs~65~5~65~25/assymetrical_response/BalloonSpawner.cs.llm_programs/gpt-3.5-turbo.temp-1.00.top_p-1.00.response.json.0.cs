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

    if (autoSpawn && spawnAtStartup)
    {
        SpawnBalloon(color);
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
    }

    // Initialize object pool
    for (int i = 0; i < 10; i++)
    {
        GameObject newBalloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
        newBalloon.SetActive(false);
        balloonPool.Add(newBalloon);
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







private List<GameObject> balloonPool = new List<GameObject>();

void Update()
{
    if (balloonPrefab == null)
    {
        return;
    }

    if ((Time.time > nextSpawnTime) && autoSpawn)
    {
        GameObject spawnedBalloon = GetPooledBalloon();
        spawnedBalloon.SetActive(true);
        Balloon balloonComponent = spawnedBalloon.GetComponent<Balloon>();
        balloonComponent.SetColor(color);
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
    }
}

private GameObject GetPooledBalloon()
{
    for (int i = 0; i < balloonPool.Count; i++)
    {
        if (!balloonPool[i].activeInHierarchy)
        {
            return balloonPool[i];
        }
    }
    GameObject newBalloon = Instantiate(balloonPrefab, transform.position, transform.rotation) as GameObject;
    newBalloon.SetActive(false);
    balloonPool.Add(newBalloon);
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
