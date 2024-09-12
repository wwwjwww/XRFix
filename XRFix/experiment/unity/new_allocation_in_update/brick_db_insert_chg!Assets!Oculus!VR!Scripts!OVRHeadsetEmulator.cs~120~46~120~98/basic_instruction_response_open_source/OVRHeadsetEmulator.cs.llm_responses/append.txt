			}

			if (!hasSentEvent)
			{
				OVRPlugin.SendEvent("headset_emulator", "activated");
				hasSentEvent = true;
			}
		}
		else
		{
			if (lastFrameEmulationActivated)
			{
				Cursor.lockState = previousCursorLockMode;

				recordedHeadPoseRelativeOffsetTranslation = manager.headPoseRelativeOffsetTranslation;
				recordedHeadPoseRelativeOffsetRotation = manager.headPoseRelativeOffsetRotation;

				if (resetHmdPoseOnRelease)
				{
					manager.headPoseRelativeOffsetTranslation = Vector3.zero;
					manager.headPoseRelativeOffsetRotation = Vector3.zero;
				}
			}
		}
		lastFrameEmulationActivated = emulationActivated;
	}

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
