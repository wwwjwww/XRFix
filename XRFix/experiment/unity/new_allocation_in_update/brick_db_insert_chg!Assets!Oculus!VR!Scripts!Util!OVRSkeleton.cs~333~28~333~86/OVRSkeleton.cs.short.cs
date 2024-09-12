using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	private void Update()
	{
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 				transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);

				// FIXED CODE:
