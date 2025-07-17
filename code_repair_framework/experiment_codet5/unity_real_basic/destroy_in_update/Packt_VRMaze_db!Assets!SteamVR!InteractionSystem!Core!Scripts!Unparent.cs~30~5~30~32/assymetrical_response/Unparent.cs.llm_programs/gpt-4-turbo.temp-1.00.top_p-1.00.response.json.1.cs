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






void Update()
        {
            if (oldParent == null)
            {
                // It's inefficient to call Destroy() repeatedly in Update.
                Object.Destroy(gameObject); // Retain this out of Update, called once elsewhere.
            }
        }

        void OnDisable()
        {
            if (oldParent == null)
            {
                Object.Destroy(gameObject); // Move Destroy() here to call once when no longer needed.
            }
        }




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
