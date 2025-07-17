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

						// you can try to build an object pool before Update() method has been called.
						// FIXED CODE:


        private bool itemSpawned = false; // Add a flag to check if the item has been spawned

        void Update()
        {
            if (itemPrefab != null && !itemSpawned) // Check if the item has not been spawned yet
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
                        itemSpawned = true; // Mark the item as spawned

                        Destroy(gameObject); // Optional: if you want to remove this component or its GameObject
                    }
                }
            }
        }


	}
}
