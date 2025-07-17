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



    void Start() {
        if ( balloonPrefab == null ) {
            return;
        }

        if ( autoSpawn && spawnAtStartup ) {
            SpawnBalloon(color);
            nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
        }

        objectPool = new ObjectPool<GameObject>();
        objectPool.SetSize(5);
        objectPool.Initialize();
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







Here are the fixed code snippets:

// FIXED CODE:
import ObjectPool from "ObjectPool";

    private ObjectPool<GameObject> objectPool;

    void Update() {
        if ( objectPool == null ) {
            return;
        }

        if ( objectPool.GetActiveCount() >= 5 ) {
            return;
        }

        if ( balloonPrefab == null ) {
            return;
        }

        if ( ( Time.time > nextSpawnTime ) && autoSpawn ) {
            SpawnBalloon(color);
            nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
        }
    }

    public GameObject SpawnBalloon( Balloon.BalloonColor color = Balloon.BalloonColor.Red ) {
        if ( balloonPrefab == null ) {
            return null;
        }
        GameObject balloon = Instantiate(balloonPrefab);
        balloon.transform.localScale = new Vector3(scale, scale, scale);
        if (attachBalloon) {
            balloon.transform.parent = transform;
        }
        balloon.GetComponentInChildren<Balloon>().SetColor(color);
        if (objectPool != null) {
            objectPool.SetActive(true);
            objectPool.SetParent(balloon);
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
