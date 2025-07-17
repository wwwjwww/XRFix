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


// 							GameObject objectToAttach = ObjectPool.Instance.GetPooledObject( itemPrefab.name );
// 							objectToAttach.SetActive( true );
// 							hand.AttachObject( objectToAttach );
// 							hand.controller.TriggerHapticPulse( 800 );
// 							ObjectPool.Instance.ReturnPooledObjectToPool( objectToAttach );

// 							You need to add a Object Pool script to your project. Then you can replace the second code block with the following lines.

// 							ObjectPool objectPool = ObjectPool.Instance;
// 							GameObject objectToAttach = objectPool.GetPooledObject( itemPrefab.name );
// 							objectToAttach.SetActive( true );
// 							hand.AttachObject( objectToAttach );
// 							hand.controller.TriggerHapticPulse( 800 );
// 							objectPool.ReturnPooledObjectToPool( objectToAttach );
	}
}
