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


cpp
void Update()
{
    if (itemPrefab != null)
    {
        if (hand.controller != null)
        {
            if (hand.controller.hasTracking)
            {
                // Spawn the object prefab and attach it to the hand
                GameObject objectToAttach = Instantiate(itemPrefab);
                hand.AttachObject(objectToAttach, Hand.AttachmentFlags.SnapOnAll);

                // Trigger a haptic pulse
                hand.controller.TriggerHapticPulse(800);

                // Destroy this script (you can optionally keep it if you want to re-spawn the prefab later)
                Destroy(this);
            }
        }
    }
}


	}
}
