

 		void Update()
 		{
 			if ( itemPrefab != null )
 			{
 				if ( hand.controller != null )
 				{
 					if ( hand.controller.hasTracking )
 					{
 						GameObject objectToAttach = GameObject.Instantiate( itemPrefab );
 						objectToAttach.SetActive( true );
 						hand.AttachObject( objectToAttach );
 						hand.controller.TriggerHapticPulse( 800 );

						gameObject.SetActivate(false);

						objectToAttach.transform.localScale = itemPrefab.transform.localScale;
 					}
 				}
 			}
 		}
