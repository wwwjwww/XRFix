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


		private GameObject spawnedObject;

		void Update()
		{
			if (itemPrefab != null)
			{
				if (hand.controller != null)
				{
					if (hand.controller.hasTracking)
					{
						spawnedObject = GameObject.Instantiate(itemPrefab);
						spawnedObject.SetActive(true);
						hand.AttachObject(spawnedObject);
						hand.controller.TriggerHapticPulse(800);

						spawnedObject.transform.position = this.transform.position; // Reposition the spawned object
						spawnedObject.transform.rotation = this.transform.rotation; // Reposition the spawned object

						spawnedObject.transform.parent = this.transform; // Make the spawned object a child of this object

						this.enabled = false; // Disable this script after it has been spawned
					}
				}
					}
		}


	}
}
