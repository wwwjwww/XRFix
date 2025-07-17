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
				// 				Object.Destroy( gameObject );
				// 		}

				// FIXED CODE:


The provided code is a simple script for Unity that unparents an object from its parent. In the Start method, it saves the old parent of the object and sets the new parent as null. Then it changes the name of the gameObject to be composed of the name of the old parent and the name of the game object.




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
