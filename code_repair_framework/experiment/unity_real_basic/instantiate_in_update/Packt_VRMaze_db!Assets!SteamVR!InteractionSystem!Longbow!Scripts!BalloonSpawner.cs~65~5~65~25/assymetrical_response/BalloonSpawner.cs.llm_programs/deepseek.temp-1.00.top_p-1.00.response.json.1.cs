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
            if (spawnAtStartup)
            {
                SpawnBalloonIfTime();
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







        void SpawnBalloonIfTime()
        {
            if ((Time.time > nextSpawnTime) && autoSpawn && balloonPrefab != null)
            {
                SpawnBalloon(color);
                nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime) + Time.time;
            }
        }

        void Update()
        {
            SpawnBalloonIfTime();
        }




		//-------------------------------------------------
		public void SpawnBalloonFromEvent( int color )
		{
			// Copy of SpawnBalloon using int because we can't pass in enums through the event system
			SpawnBalloon( (Balloon.BalloonColor)color );
		}
	}
}
