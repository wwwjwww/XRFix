		void Awake()
        {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = GameObject.Instantiate( balloonPrefab );
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
        }

		public GameObject SpawnBalloon( Balloon.BalloonColor color = Balloon.BalloonColor.Red )
		{
			if ( balloonPrefab == null )
			{
				return null;
			}
			var balloon = objectPool.Dequeue() as GameObject;
			balloon.position = transform.position;
			balloon.rotation = transform.rotation;
			balloon.SetActive(true);
			balloon.transform.localScale = new Vector3( scale, scale, scale );
			if ( attachBalloon )
			{
				balloon.transform.parent = transform;
			}

			if ( sendSpawnMessageToParent )
			{
				if ( transform.parent != null )
				{
					transform.parent.SendMessage( "OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver );
				}
			}

			if ( playSounds )
			{
				if ( inflateSound != null )
				{
					inflateSound.Play();
				}
				if ( stretchSound != null )
				{
					stretchSound.Play();
				}
			}
			balloon.GetComponentInChildren<Balloon>().SetColor( color );
			if ( spawnDirectionTransform != null )
			{
				balloon.GetComponentInChildren<Rigidbody>().AddForce( spawnDirectionTransform.forward * spawnForce );
			}

			return balloon;
		}
