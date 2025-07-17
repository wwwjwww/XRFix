//======= Copyright (c) Valve Corporation, All rights reserved. ===============






using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private Hand hand;
		public GameObject itemPrefab;

	

		void Start()
		{
			hand = GetComponentInParent<Hand>();
		}



/// 		void Update()
// 		{
// 			if ( itemPrefab != null )
// 			{
// 				if ( hand.controller != null )
// 				{
// 					if ( hand.controller.hasTracking )
// 					{
						// BUG: Instantiate in Update() method
						// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
						// 						GameObject objectToAttach = GameObject.Instantiate( itemPrefab );
						// 						objectToAttach.SetActive( true );
						// 						hand.AttachObject( objectToAttach );
						// 						hand.controller.TriggerHapticPulse( 800 );
						// 						Destroy( gameObject );
						// 
						// 
						// 
						// 						objectToAttach.transform.localScale = itemPrefab.transform.localScale;
						// 					}
						// 				}
						// 			}
						// 		}

						// FIXED CODE:


void Update()
 		{
 			if ( itemPrefab != null )
 			{
 				if ( hand.controller != null )
 				{
 					if ( hand.controller.hasTracking )
 					{
 						// Instantiate and attach the object after tracking
 						GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
 						objectToAttach.SetActive(true);
 						hand.AttachObject(objectToAttach);
 						hand.controller.TriggerHapticPulse(800);
 						Destroy(gameObject);
 					}
 				}
 			}
 		}


	}
}
