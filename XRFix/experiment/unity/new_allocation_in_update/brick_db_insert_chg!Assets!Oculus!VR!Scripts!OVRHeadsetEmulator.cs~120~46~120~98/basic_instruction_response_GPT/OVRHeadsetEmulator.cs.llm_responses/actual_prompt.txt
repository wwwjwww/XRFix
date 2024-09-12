using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	void Update () {
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 				manager.headPoseRelativeOffsetRotation = new Vector3(emulatedRoll, emulatedYaw, emulatedPitch);

				// FIXED CODE:
