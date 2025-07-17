
 		void Update()
 		{
 			if ( ( destructTime != 0 ) && ( Time.time > destructTime ) )
 			{
 				if ( burstOnLifetimeEnd )
 				{
 					SpawnParticles( lifetimeEndParticlePrefab, lifetimeEndSound );
 				}

 				gameObject.SetActive(false);
 			}
 		}
