using UnityEngine;
using System.Collections;

	void Update ()
	{
			// BUG: Using New() allocation in Update() method.
			// MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
			// 			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);

			// FIXED CODE:
