 		void Awake()
        {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = GameObject.Instantiate( itemPrefab );
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
        }

 		void Update()
 		{
 			if ( itemPrefab != null )
 			{
 				if ( hand.controller != null )
 				{
 					if ( hand.controller.hasTracking )
 					{
 					    var objectToAttach = objectPool.Dequeue();
 						objectToAttach.SetActive( true );
 						hand.AttachObject( objectToAttach );
 						hand.controller.TriggerHapticPulse( 800 );
 						Destroy( gameObject );


 						objectToAttach.transform.localScale = itemPrefab.transform.localScale;
 					}
 				}
 			}
 		}
