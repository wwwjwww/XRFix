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

			// Start a coroutine to handle cleanup
			cleanupCoroutine = StartCoroutine(CheckAndDestroy());
		}



/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Object.Destroy( gameObject );
				// 		}

				// FIXED CODE:


Coroutine cleanupCoroutine;

		IEnumerator CheckAndDestroy()
		{
			// Wait until the end of the frame to check the parent
			yield return new WaitForEndOfFrame();

			// Check periodically if the old parent is destroyed
			while (oldParent != null)
			{
				yield return null; // Wait for the next frame
			}

			// Once parent is null, destroy this game object
			Destroy(gameObject);
		}




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
