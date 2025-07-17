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
    // BUG: The name has not been updated yet
        gameObject.name = oldParent.name + "." + gameObject.name;
    }



/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Object.Destroy( gameObject );
				// 		}

				// FIXED CODE:


markdown
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿
﻿




﻿
﻿
﻿
﻿
﻿
﻿
﻿
## FIXED CODE:

Using UnityEngine;




		//-------------------------------------------------
		public Transform GetOldParent()
		{
			return oldParent;
		}
	}
}
