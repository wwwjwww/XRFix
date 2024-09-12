using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Node = UnityEngine.XR.XRNode;

	private static bool IsValidOpenVRDevice(uint deviceId)
	{
		// BUG: Constant condition
		// MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
		// 		return (deviceId >= 0 && deviceId < OVR.OpenVR.OpenVR.k_unMaxTrackedDeviceCount);

		// FIXED CODE:
