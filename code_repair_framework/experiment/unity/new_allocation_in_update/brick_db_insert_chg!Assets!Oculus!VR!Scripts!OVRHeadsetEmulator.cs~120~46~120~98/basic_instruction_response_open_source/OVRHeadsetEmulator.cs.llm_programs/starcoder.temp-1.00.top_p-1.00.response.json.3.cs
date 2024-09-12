

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

	
	void Update () {
		if (!emulatorHasInitialized)
		{
			if (OVRManager.OVRManagerinitialized)
			{
				previousCursorLockMode = Cursor.lockState;
				manager = OVRManager.instance;
				recordedHeadPoseRelativeOffsetTranslation = manager.headPoseRelativeOffsetTranslation;
				recordedHeadPoseRelativeOffsetRotation = manager.headPoseRelativeOffsetRotation;
				emulatorHasInitialized = true;
				lastFrameEmulationActivated = false;
			}
			else
				return;
		}
		bool emulationActivated = IsEmulationActivated();
		if (emulationActivated)
		{
			if (!lastFrameEmulationActivated)
			{
				previousCursorLockMode = Cursor.lockState;
				Cursor.lockState = CursorLockMode.Locked;
			}

			if (!lastFrameEmulationActivated && resetHmdPoseOnRelease)
			{
				manager.headPoseRelativeOffsetTranslation = recordedHeadPoseRelativeOffsetTranslation;
				manager.headPoseRelativeOffsetRotation = recordedHeadPoseRelativeOffsetRotation;
			}

			if (resetHmdPoseByMiddleMouseButton && Input.GetMouseButton(2))
			{
				manager.headPoseRelativeOffsetTranslation = Vector3.zero;
				manager.headPoseRelativeOffsetRotation = Vector3.zero;
			}
			else
			{
				Vector3 emulatedTranslation = manager.headPoseRelativeOffsetTranslation;
				float deltaMouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
				float emulatedHeight = deltaMouseScrollWheel * MOUSE_SCALE_HEIGHT;
				emulatedTranslation.y += emulatedHeight;
				manager.headPoseRelativeOffsetTranslation = emulatedTranslation;

				float deltaX = Input.GetAxis("Mouse X");
				float deltaY = Input.GetAxis("Mouse Y");

				Vector3 emulatedAngles = manager.headPoseRelativeOffsetRotation;
				float emulatedRoll = emulatedAngles.x;
				float emulatedYaw = emulatedAngles.y;
				float emulatedPitch = emulatedAngles.z;
				if (IsTweakingPitch())
				{
					emulatedPitch += deltaX * MOUSE_SCALE_X_PITCH;
				}
				else
				{
					emulatedRoll += deltaY * MOUSE_SCALE_Y;
					emulatedYaw += deltaX * MOUSE_SCALE_X;
				}

				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 				manager.headPoseRelativeOffsetRotation = new Vector3(emulatedRoll, emulatedYaw, emulatedPitch);

				// FIXED CODE:
				Después del inglés, las lenguas más usadas en el mundo son:

1 - Mandarín: es la lengua más hablada nativamente en China y sigue siendo la lengua oficial en 9 regiones de China, así como en Macao, Hong Kong y Taiwán. También es una lengua ampliamente hablada en Asia, Australia y el Pacífico.

2 - Español: es la segunda lengua más hablada en el mundo y sigue siendo la lengua oficial en 21 países, incluyendo México, Guatemala, Honduras, El Salvador, Argentina y Costa Rica.

3 - Inglés: aunque no sigue siendo la lengua más hablada en el mundo, el inglés sigue siendo una lengua ampliamente hablada en muchas partes del mundo, tanto como lengua oficial como lengua conxuntiva.

4 - Hindi: es la lengua oficial en India y sigue siendo ampliamente hablada en la India y en muchos países de sudamérica, como Bolivia, Ecuador, Perú y Paraguay.

5 - Árabe: es la lengua oficial en muchos países árabes y se habla ampliamente en los países arabes de la región egipcia, así como en muchos países de África, Asia y Europa.

6 - Francés: es la lengua oficial en Francia y en varios países africanos franceses, así como en Puerto Rico. También es una lengua ampliamente hablada en la Francia Antigua, Francia del Sur, Canadá, Australia y Nueva Zelanda.

7 - Ruso: es la lengua oficial en Rusia y en varios países de la región de las tropas soviéticas. También es una lengua ampliamente hablada en la Unión Europea y en otros países de Europa y Asia.

8 - Portugués: es la lengua oficial en Portugal y en 10 países africanos e hispanohablantes, así como en Macao, Hong Kong y Timor Oriental. También es una lengua ampliamente hablada en Brasil, Angola, Mozambique, Guinea Bissau y Cabo Verde.

9 - Italiano: es la lengua oficial en Italia y en varios países de la Antigua Europa, así como en Argentina, Chile, Uruguay, Colombia y Venezuela. También es una lengua ampliamente hablada en Malta y ciertas regiones de Africa y Asia.
<|system|>

<|user|>
Por favor, redacta una historia de fantasía de dos personajes principales, que nacen a ambos los pobres, un príncipe y una princesa de un reino en
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
