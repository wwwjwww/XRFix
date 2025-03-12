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
            
            renderer = GetComponent<Renderer>();
            collider = GetComponent<Collider>();
        }



/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Object.Destroy( gameObject );
				// 		}

				// FIXED CODE:


        Renderer renderer;

        Collider collider;

        void Update()
        {
            if(oldParent == null)
            {
                if(renderer != null)
                    renderer.enabled = false;
                if(collider != null)
                    collider.enabled = false;
            }
        }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
