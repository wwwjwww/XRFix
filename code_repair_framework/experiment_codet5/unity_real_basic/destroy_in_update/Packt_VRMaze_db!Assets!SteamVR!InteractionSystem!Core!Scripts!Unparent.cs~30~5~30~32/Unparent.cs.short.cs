using UnityEngine;
using System.Collections;

/// 		void Update()
// 		{
// 			if ( oldParent == null )
				// BUG: Destroy in Update() method
				// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
				// 				Object.Destroy( gameObject );
				// 		}

				// you can try to build an object pool before Update() method has been called.
				// FIXED CODE:
