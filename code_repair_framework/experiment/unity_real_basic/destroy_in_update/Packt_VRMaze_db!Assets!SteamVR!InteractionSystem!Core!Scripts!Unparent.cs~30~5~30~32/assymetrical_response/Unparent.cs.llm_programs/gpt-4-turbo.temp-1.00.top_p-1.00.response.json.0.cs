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






        void OnDisable()
        {
            if (oldParent == null)
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
