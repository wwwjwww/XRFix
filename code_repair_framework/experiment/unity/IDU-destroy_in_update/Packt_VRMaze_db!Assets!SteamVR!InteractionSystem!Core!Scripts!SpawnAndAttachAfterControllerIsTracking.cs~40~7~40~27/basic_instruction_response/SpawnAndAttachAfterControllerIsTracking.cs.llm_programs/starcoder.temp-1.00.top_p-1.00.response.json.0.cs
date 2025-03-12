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
// 						GameObject objectToAttach = GameObject.Instantiate( itemPrefab );
// 						objectToAttach.SetActive( true );
// 						hand.AttachObject( objectToAttach );
// 						hand.controller.TriggerHapticPulse( 800 );
						// BUG: Destroy in Update() method
						// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
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
	if (itemPrefab!= null)
	{
		if (hand.controller!= null)
		{
			if (hand.controller.hasTracking)
			{
				// Spawn object at the palm of the hand
				Vector3 spawnPosition = hand.transform.GetChild(1).position;
				Quaternion spawnRotation = hand.transform.GetChild(1).rotation;
				GameObject objectToAttach = Instantiate(itemPrefab, spawnPosition, spawnRotation);
				objectToAttach.SetActive(true);

				// Attach the object to the hand
				hand.AttachObject(objectToAttach);
				hand.controller.TriggerHapticPulse(800);
			}
		}
	}
}


	}
}
