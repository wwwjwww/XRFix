//		public GameObject SpawnBalloon( Balloon.BalloonColor color = Balloon.BalloonColor.Red )
//		{
//			if ( balloonPrefab == null )
//			{
//				return null;
//			}
//			GameObject balloon = Instantiate( balloonPrefab, transform.position, transform.rotation ) as GameObject;
//			balloon.transform.localScale = new Vector3( scale, scale, scale );
//			if ( attachBalloon )
//			{
//				balloon.transform.parent = transform;
//			}
//
//			if ( sendSpawnMessageToParent )
//			{
//				if ( transform.parent != null )
//				{
//					transform.parent.SendMessage( "OnBalloonSpawned", balloon, SendMessageOptions.DontRequireReceiver );
//				}
//			}
//
//			if ( playSounds )
//			{
//				if ( inflateSound != null )
//				{
//					inflateSound.Play();
//				}
//				if ( stretchSound != null )
//				{
//					stretchSound.Play();
//				}
//			}
//			balloon.GetComponentInChildren<Balloon>().SetColor( color );
//			if ( spawnDirectionTransform != null )
//			{
//				balloon.GetComponentInChildren<Rigidbody>().AddForce( spawnDirectionTransform.forward * spawnForce );
//			}
//
//			return balloon;
//		}

// FIXED CODE:
