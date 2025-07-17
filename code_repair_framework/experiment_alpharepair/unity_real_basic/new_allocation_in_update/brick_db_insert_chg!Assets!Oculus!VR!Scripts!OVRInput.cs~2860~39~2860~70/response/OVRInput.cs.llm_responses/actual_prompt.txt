using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Node = UnityEngine.XR.XRNode;

/// 		public override Controller Update()
// 		{
// 			if (!initialized)
// 			{
// 				return Controller.None;
// 			}
// 
			// BUG: Using New() allocation in Update() method.
			// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
			// 			OVRPlugin.ControllerState4 state = new OVRPlugin.ControllerState4();
			// 
			// 			bool result = OVR_GamepadController_Update();
			// 
			// 			if (result)
			// 				state.ConnectedControllers = (uint)Controller.Gamepad;
			// 
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.A))
			// 				state.Buttons |= (uint)RawButton.A;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.B))
			// 				state.Buttons |= (uint)RawButton.B;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.X))
			// 				state.Buttons |= (uint)RawButton.X;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Y))
			// 				state.Buttons |= (uint)RawButton.Y;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Up))
			// 				state.Buttons |= (uint)RawButton.DpadUp;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Down))
			// 				state.Buttons |= (uint)RawButton.DpadDown;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Left))
			// 				state.Buttons |= (uint)RawButton.DpadLeft;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Right))
			// 				state.Buttons |= (uint)RawButton.DpadRight;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Start))
			// 				state.Buttons |= (uint)RawButton.Start;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.Back))
			// 				state.Buttons |= (uint)RawButton.Back;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.LStick))
			// 				state.Buttons |= (uint)RawButton.LThumbstick;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.RStick))
			// 				state.Buttons |= (uint)RawButton.RThumbstick;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.LeftShoulder))
			// 				state.Buttons |= (uint)RawButton.LShoulder;
			// 			if (OVR_GamepadController_GetButton((int)ButtonGPC.RightShoulder))
			// 				state.Buttons |= (uint)RawButton.RShoulder;
			// 
			// 			state.LThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.LeftXAxis);
			// 			state.LThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.LeftYAxis);
			// 			state.RThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.RightXAxis);
			// 			state.RThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.RightYAxis);
			// 			state.LIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.LeftTrigger);
			// 			state.RIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.RightTrigger);
			// 
			// 			if (state.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LIndexTrigger;
			// 			if (state.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LHandTrigger;
			// 			if (state.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LThumbstickUp;
			// 			if (state.LThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LThumbstickDown;
			// 			if (state.LThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LThumbstickLeft;
			// 			if (state.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.LThumbstickRight;
			// 
			// 			if (state.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RIndexTrigger;
			// 			if (state.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RHandTrigger;
			// 			if (state.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RThumbstickUp;
			// 			if (state.RThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RThumbstickDown;
			// 			if (state.RThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RThumbstickLeft;
			// 			if (state.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
			// 				state.Buttons |= (uint)RawButton.RThumbstickRight;
			// 
			// 			previousState = currentState;
			// 			currentState = state;
			// 
			// 			return ((Controller)currentState.ConnectedControllers & controllerType);
			// 		}

			// Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
			// FIXED CODE:
