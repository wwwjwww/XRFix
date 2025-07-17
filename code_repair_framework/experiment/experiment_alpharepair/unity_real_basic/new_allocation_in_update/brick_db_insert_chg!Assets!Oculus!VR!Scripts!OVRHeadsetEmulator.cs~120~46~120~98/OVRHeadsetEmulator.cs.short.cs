using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
