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

            // Schedule for destruction if oldParent is null in Start
            if (oldParent == null)
            {
                shouldDestroy = true;
            }
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


        bool shouldDestroy = false;

        void Update()
        {
            if (shouldDestroy)
            {
                Object.Destroy(gameObject);
            }
        }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
