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
            if (itemPrefab != null)
            {
                if (hand.controller != null)
                {
                    if (hand.controller.hasTracking)
                    {
                        // Spawn and attach the object
                        GameObject objectToAttach = Instantiate(itemPrefab, hand.transform);
                        objectToAttach.SetActive(true);
                        objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                        hand.AttachObject(objectToAttach);
                        hand.controller.TriggerHapticPulse(800);
                        Destroy(gameObject);
                    }
                }
            }
        }


	}
}
