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

						// you can try to build an object pool before Update() method has been called.
						// FIXED CODE:


        private bool itemSpawned = false;  // Flag to check if item is already spawned

        void Update()
        {
            if (!itemSpawned && itemPrefab != null)
            {
                if (hand.controller != null)
                {
                    if (hand.controller.hasTracking)
                    {
                        GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
                        objectToAttach.SetActive(true);
                        hand.AttachObject(objectToAttach);
                        hand.controller.TriggerHapticPulse(800);

                        objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                        itemSpawned = true;  // Set the flag to true after spawning and attaching the item
                    }
                }
            }
        }


	}
}
