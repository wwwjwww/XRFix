//======= Copyright (c) Valve Corporation, All rights reserved. ===============






using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
		private Hand hand;
		public GameObject itemPrefab;

	

		private void Start()
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
			if (itemPrefab!= null)
			{
				if (hand.controller!= null)
				{
					if (hand.controller.hasTracking)
					{
						// Create an instance of the prefab at the hand's tip.
						GameObject objectToAttach = Instantiate(itemPrefab, hand.transform.position, Quaternion.identity);

						// Set the scale of the instance to the original prefab's scale.
						objectToAttach.transform.localScale = itemPrefab.transform.localScale;

						// Attach the instance to the hand.
						hand.AttachObject(objectToAttach);

						// Trigger a haptic pulse on the controller.
						hand.controller.TriggerHapticPulse(800);

						// Destroy this game object after the instance is attached.
						Destroy(gameObject);
					}
				}
			}
		}


	}
}
