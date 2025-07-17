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
			if (itemPrefab!= null)
			{
				hand.OnInteract += AttachObject;
			}
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


		private static ObjectPool<GameObject> itemPool; // create an object pool to reduce the frequency of Instantiate() method

		void Awake()
		{
			// create an object pool for itemPrefab with initial capacity of 5
			itemPool = new ObjectPool<GameObject>(itemPrefab, 5);

			hand = GetComponentInParent<Hand>();
		}


	}
}
