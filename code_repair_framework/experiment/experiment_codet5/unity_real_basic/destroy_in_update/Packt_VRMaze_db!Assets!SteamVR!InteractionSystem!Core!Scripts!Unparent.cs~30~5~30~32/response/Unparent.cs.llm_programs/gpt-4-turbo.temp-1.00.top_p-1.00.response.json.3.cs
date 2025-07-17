//======= Copyright (c) Valve Corporation, All rights reserved. ===============





using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class Unparent : MonoBehaviour
	{
		Transform oldParent;


		void Start()
		{
			oldParent = transform.parent;
			transform.parent = null;
			gameObject.name = oldParent.gameObject.name + "." + gameObject.name;
		}



/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Object.Destroy( gameObject );
				// 		}

				// you can try to build an object pool before Update() method has been called.
				// FIXED CODE:


void Update()
        {
            if (oldParent == null)
            {
                // Add game object to a pool for later reuse instead of destroying it
                ObjectPool.Instance.AddToPool(gameObject);
                gameObject.SetActive(false);
            }
        }

    private static ObjectPool instance;

    private Queue<GameObject> pool = new Queue<GameObject>();

    public void AddToPool(GameObject obj)
    {
        pool.Enqueue(obj);
    }

    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Handle if pool is empty, e.g., instantiate a new object
            return null;
        }
    }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
