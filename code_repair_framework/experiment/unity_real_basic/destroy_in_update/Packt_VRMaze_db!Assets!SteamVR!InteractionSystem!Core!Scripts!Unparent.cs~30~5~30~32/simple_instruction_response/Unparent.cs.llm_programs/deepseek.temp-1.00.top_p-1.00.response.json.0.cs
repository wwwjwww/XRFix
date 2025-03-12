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
            gameObject.name = oldParent.gameObject.name + "." + gameObject.name; // remove s from gameObject to make it work
        }



/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// 				Object.Destroy( gameObject );
				// 		}

				// FIXED CODE:


        void Update()
        {
            if (oldParent == null)
                Destroy(gameObject); // use Destroy instead of Object.Destroy to avoid a null reference
        }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
