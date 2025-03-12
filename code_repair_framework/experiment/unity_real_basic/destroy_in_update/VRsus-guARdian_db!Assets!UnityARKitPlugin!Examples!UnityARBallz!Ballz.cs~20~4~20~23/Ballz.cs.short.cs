using System.Collections.Generic;
using UnityEngine;

/* 	void Update () {
* 
* 		if (Mathf.Abs (startingY - transform.position.y) > yDistanceThreshold) {
			* BUG: Destroy in Update() method
			* MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
			* 			Destroy (gameObject);
			* 		}
			* 	}

			* you can try to build an object pool before Update() method has been called.
			* FIXED CODE:
			*/
