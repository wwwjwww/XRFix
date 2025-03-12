using UnityEngine;
using System.Collections;

/* 		void Update()
* 		{
* 			if ( ( burnTime != 0 ) && ( Time.time > ( ignitionTime + burnTime ) ) && isBurning )
* 			{
* 				isBurning = false;
* 				if ( customParticles != null )
* 				{
* 					customParticles.Stop();
* 				}
* 				else
* 				{
					* BUG: Destroy in Update() method
					* MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
					* 					Destroy( fireObject );
					* 				}
					* 			}
					* 		}

					* you can try to build an object pool before Update() method has been called.
					* FIXED CODE:
					*/
