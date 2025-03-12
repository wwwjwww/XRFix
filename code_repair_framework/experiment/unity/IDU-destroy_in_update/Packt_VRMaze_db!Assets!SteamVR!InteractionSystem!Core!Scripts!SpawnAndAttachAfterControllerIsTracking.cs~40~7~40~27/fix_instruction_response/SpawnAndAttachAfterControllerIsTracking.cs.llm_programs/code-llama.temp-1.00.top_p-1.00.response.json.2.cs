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
    // Create the object pool
    GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
    objectToAttach.SetActive(true);
    hand.AttachObject(objectToAttach);
    hand.controller.TriggerHapticPulse(800);
    // Add the object to the object pool
    objectPool.Add(objectToAttach);
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
    if (hand.controller != null && hand.controller.hasTracking)
    {
        // Check if the object is already attached
        if (!hand.IsObjectAttached())
        {
            // Get an object from the pool
            GameObject objectToAttach = objectPool.Pop();
            // Attach the object to the hand
            hand.AttachObject(objectToAttach);
            // Trigger the haptic pulse
            hand.controller.TriggerHapticPulse(800);
        }
    }
}


	}
}
