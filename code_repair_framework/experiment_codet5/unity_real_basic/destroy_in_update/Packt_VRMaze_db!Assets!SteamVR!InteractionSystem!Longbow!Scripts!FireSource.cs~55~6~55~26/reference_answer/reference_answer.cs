
 		void Update()
 		{
 			if ( ( burnTime != 0 ) && ( Time.time > ( ignitionTime + burnTime ) ) && isBurning )
 			{
 				isBurning = false;
 				if ( customParticles != null )
 				{
 					customParticles.Stop();
				}
 				else
 				{
 					fireObject.SetActivate(false);
 				}
 			}
 		}

