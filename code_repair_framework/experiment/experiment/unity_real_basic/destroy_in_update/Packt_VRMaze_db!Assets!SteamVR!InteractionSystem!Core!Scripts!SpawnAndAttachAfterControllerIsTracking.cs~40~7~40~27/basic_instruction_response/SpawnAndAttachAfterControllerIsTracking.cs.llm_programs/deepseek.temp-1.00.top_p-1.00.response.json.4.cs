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


        private GameObject objectToAttach;

        void Update()
        {
            if (hand.controller != null && hand.controller.hasTracking && itemPrefab != null)
            {
                objectToAttach = GameObject.Instantiate(itemPrefab);
                objectToAttach.SetActive(true);
                hand.AttachObject(objectToAttach, GrabTypes.None); // or use GrabTypes.Fingers if you want 2 objects
                hand.controller.TriggerHapticPulse(800);
                objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                enabled = false; // this will disable the script after the object is created and attached
            }
        }


	}
}
