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
            CreatePool();
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


        private List<GameObject> pool;

        private int poolSize = 5;

        void CreatePool()
        {
            pool = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(itemPrefab);
                obj.SetActive(false);
                pool.Add(obj);
            }
        }

        void Update()
        {
            if (itemPrefab != null)
            {
                if (hand.controller != null)
                {
                    if (hand.controller.hasTracking)
                    {
                        GameObject objectToAttach = GetPooledObject();
                        if (objectToAttach != null)
                        {
                            objectToAttach.SetActive(true);
                            objectToAttach.transform.position = hand.transform.position;
                            objectToAttach.transform.rotation = hand.transform.rotation;
                            hand.AttachObject(objectToAttach);
                            hand.controller.TriggerHapticPulse(800);

                            // Scale the object based on the original prefab's scale
                            objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                        }
                    }
                }
            }
        }

        GameObject GetPooledObject()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    return pool[i];
                }
            }

            // If there is no inactive gameObject in the pool, then add a new one to the pool and return it.
            GameObject obj = Instantiate(itemPrefab);
            obj.SetActive(false);
            pool.Add(obj);
            return obj;
        }


	}
}
