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
            gameObject.name = oldParent.gameObject.name + "." + gameObjectgameObject.name;
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
                Object.Destroy(gameObject);
        }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
