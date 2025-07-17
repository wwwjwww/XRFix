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


void Awake()
{
    if (itemPrefab != null)
    {
        GameObject objectToAttach = Instantiate(itemPrefab);
        objectToAttach.SetActive(false);
        objectToAttach.transform.localScale = itemPrefab.transform.localScale;
        objectPool = new Queue<GameObject>();
        objectPool.Enqueue(objectToAttach);
    }
}

void Update()
{
    if (itemPrefab != null && hand.controller != null && hand.controller.hasTracking)
    {
        if (objectPool.Count > 0)
        {
            GameObject objectToAttach = objectPool.Dequeue();
            objectToAttach.SetActive(true);
            objectToAttach.transform.position = transform.position;
            objectToAttach.transform.rotation = transform.rotation;
            hand.AttachObject(objectToAttach, string.Empty, EAttachmentType.SnapToHand, false);
            hand.controller.TriggerHapticPulse(800);
            StartCoroutine(WaitToReturnObject(objectToAttach));
        }
    }
}

IEnumerator WaitToReturnObject(GameObject obj)
{
    yield return new WaitForSeconds(10f);
    obj.SetActive(false);
    objectPool.Enqueue(obj);
}


	}
}
