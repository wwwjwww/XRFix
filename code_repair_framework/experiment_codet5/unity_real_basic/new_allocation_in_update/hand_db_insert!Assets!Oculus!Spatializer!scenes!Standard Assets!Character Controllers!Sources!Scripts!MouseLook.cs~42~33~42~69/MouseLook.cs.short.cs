using UnityEngine;
using System.Collections;

/// 	void Update ()
// 	{
// 		if (axes == RotationAxes.MouseXAndY)
// 		{
// 			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
// 			
// 			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
// 			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
// 			
			// BUG: Using New() allocation in Update() method.
			// MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
			// 			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			// 		}
			// 		else if (axes == RotationAxes.MouseX)
			// 		{
			// 			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			// 		}
			// 		else
			// 		{
			// 			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			// 			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			// 			
			// 			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			// 		}
			// 	}

			// Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
			// FIXED CODE:
