using System.Collections;

/// 		void Update()
// 		{
// 			if ( ( destructTime != 0 ) && ( Time.time > destructTime ) )
// 			{
// 				if ( burstOnLifetimeEnd )
// 				{
// 					SpawnParticles( lifetimeEndParticlePrefab, lifetimeEndSound );
// 				}
// 
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Destroy( gameObject );
				// 			}
				// 		}

				// FIXED CODE:
