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


    private void Update()
    {
        if (itemPrefab!= null && hand.inputDevice.isValid)
        {
            if (hand.inputDevice. TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement))
            {
                if (movement.magnitude > 0.1f)
                {
                    // Create the instance of the object.
                    GameObject objectToAttach = Instantiate(itemPrefab, transform);
                    objectToAttach.transform.localPosition = movement;
                    objectToAttach.transform.parent = hand.transform;
                    objectToAttach.SetActive(true);

                    // Set the initial scale of the object.
                    objectToAttach.transform.localScale = itemPrefab.transform.localScale;

                    // Play the haptic feedback.
                    hand.inputDevice.SendHapticEvent(HapticTypes.SoftMedium);
                }
            }
        }
    }


	}
}
