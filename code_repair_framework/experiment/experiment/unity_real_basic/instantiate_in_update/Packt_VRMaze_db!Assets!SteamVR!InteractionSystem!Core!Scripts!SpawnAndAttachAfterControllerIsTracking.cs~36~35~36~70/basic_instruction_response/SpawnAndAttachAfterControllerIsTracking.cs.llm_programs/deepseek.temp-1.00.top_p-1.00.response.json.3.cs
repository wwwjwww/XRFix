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


        private float lastUpdateTime;

        public float updateInterval = 0.2f; // Interval at which Update() is called

        void Update()
        {
            if (Time.time - lastUpdateTime < updateInterval)
                return;

            lastUpdateTime = Time.time;
            
            if (itemPrefab != null && hand.controller != null && hand.controller.hasTracking)
            {
                SpawnAndAttach();
            }
        }

        void SpawnAndAttach()
        {
            GameObject objectToAttach = Instantiate(itemPrefab);
            objectToAttach.SetActive(true);
            hand.AttachObject(objectToAttach);
            hand.controller.TriggerHapticPulse(800);
            Destroy(gameObject);

            objectToAttach.transform.localScale = itemPrefab.transform.localScale;
        }


	}
}
