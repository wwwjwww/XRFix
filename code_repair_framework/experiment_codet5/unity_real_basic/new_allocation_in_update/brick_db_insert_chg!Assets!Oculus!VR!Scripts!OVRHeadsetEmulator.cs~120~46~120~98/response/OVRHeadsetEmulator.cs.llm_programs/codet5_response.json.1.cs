/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Master SDK License Version 1.0 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/oculusmastersdk-1.0/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRHeadsetEmulator : MonoBehaviour {
	public enum OpMode
	{
		Off,
		EditorOnly,
		AlwaysOn
	}

	public OpMode opMode = OpMode.EditorOnly;
	public bool resetHmdPoseOnRelease = true;
	public bool resetHmdPoseByMiddleMouseButton = true;

	public KeyCode[] activateKeys = new KeyCode[] { KeyCode.LeftControl, KeyCode.RightControl };

	public KeyCode[] pitchKeys = new KeyCode[] { KeyCode.LeftAlt, KeyCode.RightAlt };

	OVRManager manager;

	const float MOUSE_SCALE_X = -2.0f;
	const float MOUSE_SCALE_X_PITCH = -2.0f;
	const float MOUSE_SCALE_Y = 2.0f;
	const float MOUSE_SCALE_HEIGHT = 1.0f;
	const float MAX_ROLL = 85.0f;

	private bool lastFrameEmulationActivated = false;

	private Vector3 recordedHeadPoseRelativeOffsetTranslation;
	private Vector3 recordedHeadPoseRelativeOffsetRotation;

	private bool hasSentEvent = false;
	private bool emulatorHasInitialized = false;

	private CursorLockMode previousCursorLockMode = CursorLockMode.None;


	void Start () {
	}


/// 	void Update () {
// 		if (!emulatorHasInitialized)
// 		{
// 			if (OVRManager.OVRManagerinitialized)
// 			{
// 				previousCursorLockMode = Cursor.lockState;
// 				manager = OVRManager.instance;
// 				recordedHeadPoseRelativeOffsetTranslation = manager.headPoseRelativeOffsetTranslation;
// 				recordedHeadPoseRelativeOffsetRotation = manager.headPoseRelativeOffsetRotation;
// 				emulatorHasInitialized = true;
// 				lastFrameEmulationActivated = false;
// 			}
// 			else
// 				return;
// 		}
// 		bool emulationActivated = IsEmulationActivated();
// 		if (emulationActivated)
// 		{
// 			if (!lastFrameEmulationActivated)
// 			{
// 				previousCursorLockMode = Cursor.lockState;
// 				Cursor.lockState = CursorLockMode.Locked;
// 			}
// 
// 			if (!lastFrameEmulationActivated && resetHmdPoseOnRelease)
// 			{
// 				manager.headPoseRelativeOffsetTranslation = recordedHeadPoseRelativeOffsetTranslation;
// 				manager.headPoseRelativeOffsetRotation = recordedHeadPoseRelativeOffsetRotation;
// 			}
// 
// 			if (resetHmdPoseByMiddleMouseButton && Input.GetMouseButton(2))
// 			{
// 				manager.headPoseRelativeOffsetTranslation = Vector3.zero;
// 				manager.headPoseRelativeOffsetRotation = Vector3.zero;
// 			}
// 			else
// 			{
// 				Vector3 emulatedTranslation = manager.headPoseRelativeOffsetTranslation;
// 				float deltaMouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
// 				float emulatedHeight = deltaMouseScrollWheel * MOUSE_SCALE_HEIGHT;
// 				emulatedTranslation.y += emulatedHeight;
// 				manager.headPoseRelativeOffsetTranslation = emulatedTranslation;
// 
// 				float deltaX = Input.GetAxis("Mouse X");
// 				float deltaY = Input.GetAxis("Mouse Y");
// 
// 				Vector3 emulatedAngles = manager.headPoseRelativeOffsetRotation;
// 				float emulatedRoll = emulatedAngles.x;
// 				float emulatedYaw = emulatedAngles.y;
// 				float emulatedPitch = emulatedAngles.z;
// 				if (IsTweakingPitch())
// 				{
// 					emulatedPitch += deltaX * MOUSE_SCALE_X_PITCH;
// 				}
// 				else
// 				{
// 					emulatedRoll += deltaY * MOUSE_SCALE_Y;
// 					emulatedYaw += deltaX * MOUSE_SCALE_X;
// 				}
// 
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 				manager.headPoseRelativeOffsetRotation = new Vector3(emulatedRoll, emulatedYaw, emulatedPitch);
				// 			}
				// 
				// 			if (!hasSentEvent)
				// 			{
				// 				OVRPlugin.SendEvent("headset_emulator", "activated");
				// 				hasSentEvent = true;
				// 			}
				// 		}
				// 		else
				// 		{
				// 			if (lastFrameEmulationActivated)
				// 			{
				// 				Cursor.lockState = previousCursorLockMode;
				// 
				// 				recordedHeadPoseRelativeOffsetTranslation = manager.headPoseRelativeOffsetTranslation;
				// 				recordedHeadPoseRelativeOffsetRotation = manager.headPoseRelativeOffsetRotation;
				// 
				// 				if (resetHmdPoseOnRelease)
				// 				{
				// 					manager.headPoseRelativeOffsetTranslation = Vector3.zero;
				// 					manager.headPoseRelativeOffsetRotation = Vector3.zero;
				// 				}
				// 			}
				// 		}
				// 		lastFrameEmulationActivated = emulationActivated;
				// 	}

				// Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
				// FIXED CODE:


resultSet resultSet resultSet resultSet resultSet resultSetrecordedHeadPoseRelativeOffsetTranslation=manager.headPoseRelativeOffsetTranslation; resultSetrecordedHeadPoseRelativeOffsetRotation=manager.headPoseRelativeOffsetRotation;  resultSet resultSet resultSet resultSet resultSetPreviousCursorLockMode=Cursor.lockState; resultSetPreviousCursor.lockState=CursorLockMode.Locked; 


	bool IsEmulationActivated()
	{
		if (opMode == OpMode.Off)
		{
			return false;
		}
		else if (opMode == OpMode.EditorOnly && !Application.isEditor)
		{
			return false;
		}

		foreach (KeyCode key in activateKeys)
		{
			if (Input.GetKey(key))
				return true;
		}

		return false;
	}

	bool IsTweakingPitch()
	{
		if (!IsEmulationActivated())
			return false;

		foreach (KeyCode key in pitchKeys)
		{
			if (Input.GetKey(key))
				return true;
		}

		return false;
	}
}
