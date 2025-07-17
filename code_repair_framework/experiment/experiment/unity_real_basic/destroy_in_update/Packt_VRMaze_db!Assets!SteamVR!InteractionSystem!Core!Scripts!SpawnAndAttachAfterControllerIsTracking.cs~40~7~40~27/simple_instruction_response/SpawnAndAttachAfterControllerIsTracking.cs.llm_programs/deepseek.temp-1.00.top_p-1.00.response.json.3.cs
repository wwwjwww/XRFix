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
            hand = GetComponent<Hand>(); // GetComponentInParent<Hand>(); was changed to GetComponent<Hand>() as it seems like you want the script to be attached to a hand instead of a child object
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
                    if (hand.controller.isTracking) // hasTracking has been replaced with isTracking
                    {
                        GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
                        objectToAttach.SetActive(true);
                        hand.AttachObject(objectToAttach);
                        hand.controller.TriggerHapticPulse(800);
                        objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                    }
                }
            }
        }


	}
}
