

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using Node = UnityEngine.XR.XRNode;




public static class OVRInput
{
	[Flags]
	
	public enum Button
	{
		None                      = 0,          
		One                       = 0x00000001, 
		Two                       = 0x00000002, 
		Three                     = 0x00000004, 
		Four                      = 0x00000008, 
		Start                     = 0x00000100, 
		Back                      = 0x00000200, 
		PrimaryShoulder           = 0x00001000, 
		PrimaryIndexTrigger       = 0x00002000, 
		PrimaryHandTrigger        = 0x00004000, 
		PrimaryThumbstick         = 0x00008000, 
		PrimaryThumbstickUp       = 0x00010000, 
		PrimaryThumbstickDown     = 0x00020000, 
		PrimaryThumbstickLeft     = 0x00040000, 
		PrimaryThumbstickRight    = 0x00080000, 
		PrimaryTouchpad           = 0x00000400, 
		SecondaryShoulder         = 0x00100000, 
		SecondaryIndexTrigger     = 0x00200000, 
		SecondaryHandTrigger      = 0x00400000, 
		SecondaryThumbstick       = 0x00800000, 
		SecondaryThumbstickUp     = 0x01000000, 
		SecondaryThumbstickDown   = 0x02000000, 
		SecondaryThumbstickLeft   = 0x04000000, 
		SecondaryThumbstickRight  = 0x08000000, 
		SecondaryTouchpad         = 0x00000800, 
		DpadUp                    = 0x00000010, 
		DpadDown                  = 0x00000020, 
		DpadLeft                  = 0x00000040, 
		DpadRight                 = 0x00000080, 
		Up                        = 0x10000000, 
		Down                      = 0x20000000, 
		Left                      = 0x40000000, 
		Right      = unchecked((int)0x80000000),
		Any                       = ~None,      
	}

	[Flags]
	
	public enum RawButton
	{
		None                      = 0,          
		A                         = 0x00000001, 
		B                         = 0x00000002, 
		X                         = 0x00000100, 
		Y                         = 0x00000200, 
		Start                     = 0x00100000, 
		Back                      = 0x00200000, 
		LShoulder                 = 0x00000800, 
		LIndexTrigger             = 0x10000000, 
		LHandTrigger              = 0x20000000, 
		LThumbstick               = 0x00000400, 
		LThumbstickUp             = 0x00000010, 
		LThumbstickDown           = 0x00000020, 
		LThumbstickLeft           = 0x00000040, 
		LThumbstickRight          = 0x00000080, 
		LTouchpad                 = 0x40000000, 
		RShoulder                 = 0x00000008, 
		RIndexTrigger             = 0x04000000, 
		RHandTrigger              = 0x08000000, 
		RThumbstick               = 0x00000004, 
		RThumbstickUp             = 0x00001000, 
		RThumbstickDown           = 0x00002000, 
		RThumbstickLeft           = 0x00004000, 
		RThumbstickRight          = 0x00008000, 
		RTouchpad  = unchecked((int)0x80000000),
		DpadUp                    = 0x00010000, 
		DpadDown                  = 0x00020000, 
		DpadLeft                  = 0x00040000, 
		DpadRight                 = 0x00080000, 
		Any                       = ~None,      
	}

	[Flags]
	
	public enum Touch
	{
		None                      = 0,                            
		One                       = Button.One,                   
		Two                       = Button.Two,                   
		Three                     = Button.Three,                 
		Four                      = Button.Four,                  
		PrimaryIndexTrigger       = Button.PrimaryIndexTrigger,   
		PrimaryThumbstick         = Button.PrimaryThumbstick,     
		PrimaryThumbRest          = 0x00001000,                   
		PrimaryTouchpad           = Button.PrimaryTouchpad,       
		SecondaryIndexTrigger     = Button.SecondaryIndexTrigger, 
		SecondaryThumbstick       = Button.SecondaryThumbstick,   
		SecondaryThumbRest        = 0x00100000,                   
		SecondaryTouchpad         = Button.SecondaryTouchpad,     
		Any                       = ~None,                        
	}

	[Flags]
	
	public enum RawTouch
	{
		None                      = 0,                            
		A                         = RawButton.A,                  
		B                         = RawButton.B,                  
		X                         = RawButton.X,                  
		Y                         = RawButton.Y,                  
		LIndexTrigger             = 0x00001000,                   
		LThumbstick               = RawButton.LThumbstick,        
		LThumbRest                = 0x00000800,                   
		LTouchpad                 = RawButton.LTouchpad,          
		RIndexTrigger             = 0x00000010,                   
		RThumbstick               = RawButton.RThumbstick,        
		RThumbRest                = 0x00000008,                   
		RTouchpad                 = RawButton.RTouchpad,          
		Any                       = ~None,                        
	}

	[Flags]
	
	
	public enum NearTouch
	{
		None                      = 0,          
		PrimaryIndexTrigger       = 0x00000001, 
		PrimaryThumbButtons       = 0x00000002, 
		SecondaryIndexTrigger     = 0x00000004, 
		SecondaryThumbButtons     = 0x00000008, 
		Any                       = ~None,      
	}

	[Flags]
	
	public enum RawNearTouch
	{
		None                      = 0,          
		LIndexTrigger             = 0x00000001, 
		LThumbButtons             = 0x00000002, 
		RIndexTrigger             = 0x00000004, 
		RThumbButtons             = 0x00000008, 
		Any                       = ~None,      
	}

	[Flags]
	
	public enum Axis1D
	{
		None                      = 0,     
		PrimaryIndexTrigger       = 0x01,  
		PrimaryHandTrigger        = 0x04,  
		SecondaryIndexTrigger     = 0x02,  
		SecondaryHandTrigger      = 0x08,  
		Any                       = ~None, 
	}

	[Flags]
	
	public enum RawAxis1D
	{
		None                      = 0,     
		LIndexTrigger             = 0x01,  
		LHandTrigger              = 0x04,  
		RIndexTrigger             = 0x02,  
		RHandTrigger              = 0x08,  
		Any                       = ~None, 
	}

	[Flags]
	
	public enum Axis2D
	{
		None                      = 0,     
		PrimaryThumbstick         = 0x01,  
		PrimaryTouchpad           = 0x04,  
		SecondaryThumbstick       = 0x02,  
		SecondaryTouchpad         = 0x08,  
		Any                       = ~None, 
	}

	[Flags]
	
	public enum RawAxis2D
	{
		None                      = 0,     
		LThumbstick               = 0x01,  
		LTouchpad                 = 0x04,  
		RThumbstick               = 0x02,  
		RTouchpad                 = 0x08,  
		Any                       = ~None, 
	}

	[Flags]
	
	public enum OpenVRButton : ulong
	{
		None                      = 0,
		Two                       = 0x0002,
		Thumbstick                = 0x100000000,
		Grip                      = 0x0004,
	}

	[Flags]
	
	public enum Controller
	{
		None                      = OVRPlugin.Controller.None,           
		LTouch                    = OVRPlugin.Controller.LTouch,         
		RTouch                    = OVRPlugin.Controller.RTouch,         
		Touch                     = OVRPlugin.Controller.Touch,          
		Remote                    = OVRPlugin.Controller.Remote,         
		Gamepad                   = OVRPlugin.Controller.Gamepad,        
		Hands                     = OVRPlugin.Controller.Hands,          
		LHand                     = OVRPlugin.Controller.LHand,          
		RHand                     = OVRPlugin.Controller.RHand,          
		Active                    = OVRPlugin.Controller.Active,         
		All                       = OVRPlugin.Controller.All,            
	}

	public enum Handedness
	{
		Unsupported	              = OVRPlugin.Handedness.Unsupported,
		LeftHanded                = OVRPlugin.Handedness.LeftHanded,
		RightHanded               = OVRPlugin.Handedness.RightHanded,
	}

	private static readonly float AXIS_AS_BUTTON_THRESHOLD = 0.5f;
	private static readonly float AXIS_DEADZONE_THRESHOLD = 0.2f;
	private static List<OVRControllerBase> controllers;
	private static Controller activeControllerType = Controller.None;
	private static Controller connectedControllerTypes = Controller.None;
	private static OVRPlugin.Step stepType = OVRPlugin.Step.Render;
	private static int fixedUpdateCount = 0;


	private static bool _pluginSupportsActiveController = false;
	private static bool _pluginSupportsActiveControllerCached = false;
	private static System.Version _pluginSupportsActiveControllerMinVersion = new System.Version(1, 9, 0);
	private static bool pluginSupportsActiveController
	{
		get
		{
			if (!_pluginSupportsActiveControllerCached)
			{
				bool isSupportedPlatform = true;
#if (UNITY_ANDROID && !UNITY_EDITOR) || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
				isSupportedPlatform = false;
#endif
				_pluginSupportsActiveController = isSupportedPlatform && (OVRPlugin.version >= _pluginSupportsActiveControllerMinVersion);
				_pluginSupportsActiveControllerCached = true;
			}

			return _pluginSupportsActiveController;
		}
	}

	
	
	
	static OVRInput()
	{
		controllers = new List<OVRControllerBase>
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			new OVRControllerGamepadAndroid(),
			new OVRControllerTouch(),
			new OVRControllerLTouch(),
			new OVRControllerRTouch(),
			new OVRControllerHands(),
			new OVRControllerLHand(),
			new OVRControllerRHand(),
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			new OVRControllerGamepadMac(),
#else
			new OVRControllerGamepadPC(),
			new OVRControllerTouch(),
			new OVRControllerLTouch(),
			new OVRControllerRTouch(),
			new OVRControllerHands(),
			new OVRControllerLHand(),
			new OVRControllerRHand(),
			new OVRControllerRemote(),
#endif
		};

		InitHapticInfo();
	}

	
	
	
	public static void Update()
	{
		connectedControllerTypes = Controller.None;
		stepType = OVRPlugin.Step.Render;
		fixedUpdateCount = 0;

		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			UpdateXRControllerNodeIds();
			UpdateXRControllerHaptics();
		}

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			connectedControllerTypes |= controller.Update();

			if ((connectedControllerTypes & controller.controllerType) != 0)
			{
				RawButton rawButtonMask = RawButton.Any;
				RawTouch rawTouchMask = RawTouch.Any;

				if (Get(rawButtonMask, controller.controllerType)
					|| Get(rawTouchMask, controller.controllerType))
				{
					activeControllerType = controller.controllerType;
				}
			}
		}

		if ((activeControllerType == Controller.LTouch) || (activeControllerType == Controller.RTouch))
		{
			if ((connectedControllerTypes & Controller.Touch) == Controller.Touch)
			{
				
				activeControllerType = Controller.Touch;
			}
		}

		if ((activeControllerType == Controller.LHand) || (activeControllerType == Controller.RHand))
		{
			if ((connectedControllerTypes & Controller.Hands) == Controller.Hands)
			{
				
				activeControllerType = Controller.Hands;
			}
		}

		if ((connectedControllerTypes & activeControllerType) == 0)
		{
			activeControllerType = Controller.None;
		}

		if ( OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && pluginSupportsActiveController)
		{
			Controller localActiveController = activeControllerType;

			
			connectedControllerTypes = (OVRInput.Controller)OVRPlugin.GetConnectedControllers();
			activeControllerType = (OVRInput.Controller)OVRPlugin.GetActiveController();

			
			if (activeControllerType == Controller.None && ((localActiveController & Controller.Hands) != 0))
			{
				activeControllerType = localActiveController;
			}
		}
		else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			activeControllerType = connectedControllerTypes;
		}
	}

	
	
	
	public static void FixedUpdate()
	{
		stepType = OVRPlugin.Step.Physics;

		double predictionSeconds = (double)fixedUpdateCount * Time.fixedDeltaTime / Mathf.Max(Time.timeScale, 1e-6f);
		fixedUpdateCount++;

		OVRPlugin.UpdateNodePhysicsPoses(0, predictionSeconds);
	}

	
	
	
	
	public static bool GetControllerOrientationTracked(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandLeft);
			case Controller.RTouch:
			case Controller.RHand:
				return OVRPlugin.GetNodeOrientationTracked(OVRPlugin.Node.HandRight);
			default:
				return false;
		}
	}

	
	
	
	
	public static bool GetControllerOrientationValid(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.HandLeft);
			case Controller.RTouch:
			case Controller.RHand:
				return OVRPlugin.GetNodeOrientationValid(OVRPlugin.Node.HandRight);
			default:
				return false;
		}
	}


	
	
	
	
	public static bool GetControllerPositionTracked(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandLeft);
			case Controller.RTouch:
			case Controller.RHand:
				return OVRPlugin.GetNodePositionTracked(OVRPlugin.Node.HandRight);
			default:
				return false;
		}
	}

	
	
	
	
	public static bool GetControllerPositionValid(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.HandLeft);
			case Controller.RTouch:
			case Controller.RHand:
				return OVRPlugin.GetNodePositionValid(OVRPlugin.Node.HandRight);
			default:
				return false;
		}
	}

	
	
	
	
	public static Vector3 GetLocalControllerPosition(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, stepType).ToOVRPose().position;
				else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					return openVRControllerDetails[0].localPosition;
				else
				{
					Vector3 retVec;
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandLeft, stepType, out retVec))
						return retVec;
					return Vector3.zero;				
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, stepType).ToOVRPose().position;
				else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					return openVRControllerDetails[1].localPosition;
				else
				{
					Vector3 retVec;
					if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.Position, OVRPlugin.Node.HandRight, stepType, out retVec))
						return retVec;
					return Vector3.zero;
				}
			default:
				return Vector3.zero;
		}
	}

	
	
	
	
	public static Vector3 GetLocalControllerVelocity(OVRInput.Controller controllerType)
	{
		Vector3 velocity = Vector3.zero;

		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.HandLeft, stepType, out velocity))
				{
					return velocity;
				}
				else
				{
					return Vector3.zero;
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.Velocity, OVRPlugin.Node.HandRight, stepType, out velocity))
				{
					return velocity;
				}
				else
				{
					return Vector3.zero;
				}
			default:
				return Vector3.zero;
		}
	}

	
	
	
	
	public static Vector3 GetLocalControllerAcceleration(OVRInput.Controller controllerType)
	{
		Vector3 accel = Vector3.zero;

		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.HandLeft, stepType, out accel))
				{
					return accel;
				}
				else
				{
					return Vector3.zero;
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.Acceleration, OVRPlugin.Node.HandRight, stepType, out accel))
				{
					return accel;
				}
				else
				{
					return Vector3.zero;
				}
			default:
				return Vector3.zero;
		}
	}

	
	
	
	
	public static Quaternion GetLocalControllerRotation(OVRInput.Controller controllerType)
	{
		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, stepType).ToOVRPose().orientation;
				else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					return openVRControllerDetails[0].localOrientation;
				else
				{
					Quaternion retQuat;
					if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.LeftHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandLeft, stepType, out retQuat))
						return retQuat;
					return Quaternion.identity;
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
					return OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, stepType).ToOVRPose().orientation;
				else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
					return openVRControllerDetails[1].localOrientation;
				else
				{
					Quaternion retQuat;
					if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(Node.RightHand, NodeStatePropertyType.Orientation, OVRPlugin.Node.HandRight, stepType, out retQuat))
						return retQuat;
					return Quaternion.identity;
				}
			default:
				return Quaternion.identity;
		}
	}

	
	
	
	
	public static Vector3 GetLocalControllerAngularVelocity(OVRInput.Controller controllerType)
	{
		Vector3 velocity = Vector3.zero;

		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.HandLeft, stepType, out velocity))
				{
					return velocity;
				}
				else
				{
					return Vector3.zero;
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.HandRight, stepType, out velocity))
				{
					return velocity;
				}
				else
				{
					return Vector3.zero;
				}
			default:
				return Vector3.zero;
		}
	}

	
	
	
	
	public static Vector3 GetLocalControllerAngularAcceleration(OVRInput.Controller controllerType)
	{
		Vector3 accel = Vector3.zero;

		switch (controllerType)
		{
			case Controller.LTouch:
			case Controller.LHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.LeftHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.HandLeft, stepType, out accel))
				{
					return accel;
				}
				else
				{
					return Vector3.zero;
				}
			case Controller.RTouch:
			case Controller.RHand:
				if (OVRNodeStateProperties.GetNodeStatePropertyVector3(Node.RightHand, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.HandRight, stepType, out accel))
				{
					return accel;
				}
				else
				{
					return Vector3.zero;
				}
			default:
				return Vector3.zero;
		}
	}

	
	
	
	public static Handedness GetDominantHand()
	{
		return (Handedness) OVRPlugin.GetDominantHand();
	}

	
	
	
	
	public static bool Get(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButton(virtualMask, RawButton.None, controllerMask);
	}

	
	
	
	
	public static bool Get(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButton(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButton(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.currentState.Buttons & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	
	
	
	
	public static bool GetDown(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonDown(virtualMask, RawButton.None, controllerMask);
	}

	
	
	
	
	public static bool GetDown(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonDown(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonDown(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		bool down = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.previousState.Buttons & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawButton)controller.currentState.Buttons & resolvedMask) != 0)
					&& (((RawButton)controller.previousState.Buttons & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	
	
	
	
	public static bool GetUp(Button virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonUp(virtualMask, RawButton.None, controllerMask);
	}

	
	
	
	
	public static bool GetUp(RawButton rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedButtonUp(Button.None, rawMask, controllerMask);
	}

	private static bool GetResolvedButtonUp(Button virtualMask, RawButton rawMask, Controller controllerMask)
	{
		bool up = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawButton resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawButton)controller.currentState.Buttons & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawButton)controller.currentState.Buttons & resolvedMask) == 0)
					&& (((RawButton)controller.previousState.Buttons & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	
	
	
	
	public static bool Get(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouch(virtualMask, RawTouch.None, controllerMask);
	}

	
	
	
	
	public static bool Get(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouch(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouch(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.currentState.Touches & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	
	
	
	
	public static bool GetDown(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchDown(virtualMask, RawTouch.None, controllerMask);
	}

	
	
	
	
	public static bool GetDown(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchDown(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchDown(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		bool down = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.previousState.Touches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawTouch)controller.currentState.Touches & resolvedMask) != 0)
					&& (((RawTouch)controller.previousState.Touches & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	
	
	
	
	public static bool GetUp(Touch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchUp(virtualMask, RawTouch.None, controllerMask);
	}

	
	
	
	
	public static bool GetUp(RawTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedTouchUp(Touch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedTouchUp(Touch virtualMask, RawTouch rawMask, Controller controllerMask)
	{
		bool up = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawTouch)controller.currentState.Touches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawTouch)controller.currentState.Touches & resolvedMask) == 0)
					&& (((RawTouch)controller.previousState.Touches & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	
	
	
	
	public static bool Get(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouch(virtualMask, RawNearTouch.None, controllerMask);
	}

	
	
	
	
	public static bool Get(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouch(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouch(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.currentState.NearTouches & resolvedMask) != 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	
	
	
	
	public static bool GetDown(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchDown(virtualMask, RawNearTouch.None, controllerMask);
	}

	
	
	
	
	public static bool GetDown(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchDown(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchDown(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		bool down = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.previousState.NearTouches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawNearTouch)controller.currentState.NearTouches & resolvedMask) != 0)
					&& (((RawNearTouch)controller.previousState.NearTouches & resolvedMask) == 0))
				{
					down = true;
				}
			}
		}

		return down;
	}

	
	
	
	
	public static bool GetUp(NearTouch virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchUp(virtualMask, RawNearTouch.None, controllerMask);
	}

	
	
	
	
	public static bool GetUp(RawNearTouch rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedNearTouchUp(NearTouch.None, rawMask, controllerMask);
	}

	private static bool GetResolvedNearTouchUp(NearTouch virtualMask, RawNearTouch rawMask, Controller controllerMask)
	{
		bool up = false;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawNearTouch resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if (((RawNearTouch)controller.currentState.NearTouches & resolvedMask) != 0)
				{
					return false;
				}

				if ((((RawNearTouch)controller.currentState.NearTouches & resolvedMask) == 0)
					&& (((RawNearTouch)controller.previousState.NearTouches & resolvedMask) != 0))
				{
					up = true;
				}
			}
		}

		return up;
	}

	
	
	
	
	public static float Get(Axis1D virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis1D(virtualMask, RawAxis1D.None, controllerMask);
	}

	
	
	
	
	public static float Get(RawAxis1D rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis1D(Axis1D.None, rawMask, controllerMask);
	}

	private static float GetResolvedAxis1D(Axis1D virtualMask, RawAxis1D rawMask, Controller controllerMask)
	{
		float maxAxis = 0.0f;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
				controller.shouldApplyDeadzone = false;

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawAxis1D resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if ((RawAxis1D.LIndexTrigger & resolvedMask) != 0)
				{
					float axis = controller.currentState.LIndexTrigger;

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis1D.RIndexTrigger & resolvedMask) != 0)
				{
					float axis = controller.currentState.RIndexTrigger;

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis1D.LHandTrigger & resolvedMask) != 0)
				{
					float axis = controller.currentState.LHandTrigger;

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis1D.RHandTrigger & resolvedMask) != 0)
				{
					float axis = controller.currentState.RHandTrigger;

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
			}
		}

		return maxAxis;
	}

	
	
	
	
	public static Vector2 Get(Axis2D virtualMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis2D(virtualMask, RawAxis2D.None, controllerMask);
	}

	
	
	
	
	public static Vector2 Get(RawAxis2D rawMask, Controller controllerMask = Controller.Active)
	{
		return GetResolvedAxis2D(Axis2D.None, rawMask, controllerMask);
	}

	private static Vector2 GetResolvedAxis2D(Axis2D virtualMask, RawAxis2D rawMask, Controller controllerMask)
	{
		Vector2 maxAxis = Vector2.zero;

		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
				controller.shouldApplyDeadzone = false;

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				RawAxis2D resolvedMask = rawMask | controller.ResolveToRawMask(virtualMask);

				if ((RawAxis2D.LThumbstick & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentState.LThumbstick.x,
						controller.currentState.LThumbstick.y);

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis2D.LTouchpad & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentState.LTouchpad.x,
						controller.currentState.LTouchpad.y);

					
					

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis2D.RThumbstick & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentState.RThumbstick.x,
						controller.currentState.RThumbstick.y);

					if (controller.shouldApplyDeadzone)
						axis = CalculateDeadzone(axis, AXIS_DEADZONE_THRESHOLD);

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
				if ((RawAxis2D.RTouchpad & resolvedMask) != 0)
				{
					Vector2 axis = new Vector2(
						controller.currentState.RTouchpad.x,
						controller.currentState.RTouchpad.y);

					
					

					maxAxis = CalculateAbsMax(maxAxis, axis);
				}
			}
		}

		return maxAxis;
	}

	
	
	
	public static Controller GetConnectedControllers()
	{
		return connectedControllerTypes;
	}

	
	
	
	public static bool IsControllerConnected(Controller controller)
	{
		return (connectedControllerTypes & controller) == controller;
	}

	
	
	
	public static Controller GetActiveController()
	{
		return activeControllerType;
	}

	private static void StartVibration(float amplitude, float duration, Node controllerNode)
	{
		int index = (controllerNode == Node.LeftHand) ? 0 : 1;
		hapticInfos[index].hapticsDurationPlayed = 0.0f;
		hapticInfos[index].hapticAmplitude = amplitude;
		hapticInfos[index].hapticsDuration = duration;
		hapticInfos[index].playingHaptics = (amplitude != 0.0f);
		hapticInfos[index].node = controllerNode;
		if (amplitude <= 0.0f || duration <= 0.0f)
		{
			hapticInfos[index].playingHaptics = false;
		}
	}

	private static int NUM_HAPTIC_CHANNELS = 2;
	private static HapticInfo[] hapticInfos;

	private static float OPENVR_MAX_HAPTIC_AMPLITUDE = 4000.0f;
	private static float HAPTIC_VIBRATION_DURATION_SECONDS = 2.0f;
	private static String OPENVR_TOUCH_NAME = "oculus_touch";
	private static String OPENVR_VIVE_CONTROLLER_NAME = "vive_controller";
	private static String OPENVR_WINDOWSMR_CONTROLLER_NAME = "holographic_controller";

	[Flags]
	
	public enum OpenVRController : ulong
	{
		Unknown = 0,
		OculusTouch = 1,
		ViveController = 2,
		WindowsMRController = 3
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OpenVRControllerDetails
	{
		public OVR.OpenVR.VRControllerState_t state;
		public OpenVRController controllerType;
		public uint deviceID;
		public Vector3 localPosition;           
		public Quaternion localOrientation;     
	}

	public static OpenVRControllerDetails[] openVRControllerDetails = new OpenVRControllerDetails[2];

	private class HapticInfo
	{
		public bool playingHaptics;
		public float hapticsDurationPlayed;
		public float hapticsDuration;
		public float hapticAmplitude;
		public Node node;
	}

	
	
	
	public static void SetOpenVRLocalPose(Vector3 leftPos, Vector3 rightPos, Quaternion leftRot, Quaternion rightRot)
	{
		openVRControllerDetails[0].localPosition = leftPos;
		openVRControllerDetails[0].localOrientation = leftRot;
		openVRControllerDetails[1].localPosition = rightPos;
		openVRControllerDetails[1].localOrientation = rightRot;
	}

	
	
	
	public static string GetOpenVRStringProperty(OVR.OpenVR.ETrackedDeviceProperty prop, uint deviceId = OVR.OpenVR.OpenVR.k_unTrackedDeviceIndex_Hmd)
	{

		OVR.OpenVR.ETrackedPropertyError error = OVR.OpenVR.ETrackedPropertyError.TrackedProp_Success;
		OVR.OpenVR.CVRSystem system = OVR.OpenVR.OpenVR.System;
		if (system != null)
		{
			uint capacity = system.GetStringTrackedDeviceProperty(deviceId, prop, null, 0, ref error);
			if (capacity > 1)
			{
				var result = new System.Text.StringBuilder((int)capacity);
				system.GetStringTrackedDeviceProperty(deviceId, prop, result, capacity, ref error);
				return result.ToString();
			}
			return (error != OVR.OpenVR.ETrackedPropertyError.TrackedProp_Success) ? error.ToString() : "<unknown>";
		}
		return "";
	}

	
	
	
	private static void UpdateXRControllerNodeIds()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			openVRControllerDetails[0].deviceID = OVR.OpenVR.OpenVR.k_unMaxTrackedDeviceCount;
			openVRControllerDetails[1].deviceID = OVR.OpenVR.OpenVR.k_unMaxTrackedDeviceCount;

			OVR.OpenVR.CVRSystem system = OVR.OpenVR.OpenVR.System;

			if (system != null)
			{
				for (uint id = 0; id < OVR.OpenVR.OpenVR.k_unMaxTrackedDeviceCount; id++)
				{
					OVR.OpenVR.ETrackedDeviceClass deviceClass = system.GetTrackedDeviceClass(id);
					if (deviceClass == OVR.OpenVR.ETrackedDeviceClass.Controller && system.IsTrackedDeviceConnected(id))
					{
						OpenVRController controllerType;
						String controllerName = GetOpenVRStringProperty(OVR.OpenVR.ETrackedDeviceProperty.Prop_ControllerType_String, id);
						if (controllerName == OPENVR_TOUCH_NAME)
							controllerType = OpenVRController.OculusTouch;
						else if (controllerName == OPENVR_VIVE_CONTROLLER_NAME)
							controllerType = OpenVRController.ViveController;
						else if (controllerName == OPENVR_WINDOWSMR_CONTROLLER_NAME)
							controllerType = OpenVRController.WindowsMRController;
						else
							controllerType = OpenVRController.Unknown;

						OVR.OpenVR.ETrackedControllerRole role = system.GetControllerRoleForTrackedDeviceIndex(id);
						if (role == OVR.OpenVR.ETrackedControllerRole.LeftHand)
						{
							system.GetControllerState(id, ref openVRControllerDetails[0].state, (uint)Marshal.SizeOf(typeof(OVR.OpenVR.VRControllerState_t)));
							openVRControllerDetails[0].deviceID = id;
							openVRControllerDetails[0].controllerType = controllerType;
							connectedControllerTypes |= Controller.LTouch;
						}
						else if (role == OVR.OpenVR.ETrackedControllerRole.RightHand)
						{
							system.GetControllerState(id, ref openVRControllerDetails[1].state, (uint)Marshal.SizeOf(typeof(OVR.OpenVR.VRControllerState_t)));
							openVRControllerDetails[1].deviceID = id;
							openVRControllerDetails[1].controllerType = controllerType;
							connectedControllerTypes |= Controller.RTouch;
						}
					}
				}
			}
		}
	}

	
	
	
	private static void UpdateXRControllerHaptics()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			for (int i = 0; i < NUM_HAPTIC_CHANNELS; i++)
			{
				if (hapticInfos[i].playingHaptics)
				{
					hapticInfos[i].hapticsDurationPlayed += Time.deltaTime;

					PlayHapticImpulse(hapticInfos[i].hapticAmplitude, hapticInfos[i].node);

					if (hapticInfos[i].hapticsDurationPlayed >= hapticInfos[i].hapticsDuration)
					{
						hapticInfos[i].playingHaptics = false;
					}
				}
			}

		}
	}

	private static void InitHapticInfo()
	{
		hapticInfos = new HapticInfo[NUM_HAPTIC_CHANNELS];
		for (int i = 0; i < NUM_HAPTIC_CHANNELS; i++)
		{
			hapticInfos[i] = new HapticInfo();
		}
	}

	private static void PlayHapticImpulse(float amplitude, Node deviceNode)
	{
		OVR.OpenVR.CVRSystem system = OVR.OpenVR.OpenVR.System;
		if (system != null && amplitude != 0.0f)
		{
			uint controllerId = (deviceNode == Node.LeftHand) ? openVRControllerDetails[0].deviceID : openVRControllerDetails[1].deviceID;

			if (IsValidOpenVRDevice(controllerId))
				system.TriggerHapticPulse(controllerId, 0, (char)(OPENVR_MAX_HAPTIC_AMPLITUDE * amplitude));
		}
	}

	private static bool IsValidOpenVRDevice(uint deviceId)
	{
		return (deviceId >= 0 && deviceId < OVR.OpenVR.OpenVR.k_unMaxTrackedDeviceCount);
	}

	
	
	
	
	public static void SetControllerVibration(float frequency, float amplitude, Controller controllerMask = Controller.Active)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
		{
			if ((controllerMask & Controller.Active) != 0)
				controllerMask |= activeControllerType;

			for (int i = 0; i < controllers.Count; i++)
			{
				OVRControllerBase controller = controllers[i];

				if (ShouldResolveController(controller.controllerType, controllerMask))
				{
					controller.SetControllerVibration(frequency, amplitude);
				}
			}
		}
		else if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			if (controllerMask == Controller.LTouch || controllerMask == Controller.RTouch)
			{
				Node controllerNode = (controllerMask == Controller.LTouch) ? Node.LeftHand : Node.RightHand;
				StartVibration(amplitude, HAPTIC_VIBRATION_DURATION_SECONDS, controllerNode);
			}
		}
	}

	
	
	
	
	
	public static byte GetControllerBatteryPercentRemaining(Controller controllerMask = Controller.Active)
	{
		if ((controllerMask & Controller.Active) != 0)
			controllerMask |= activeControllerType;

		byte battery = 0;

		for (int i = 0; i < controllers.Count; i++)
		{
			OVRControllerBase controller = controllers[i];

			if (ShouldResolveController(controller.controllerType, controllerMask))
			{
				battery = controller.GetBatteryPercentRemaining();
				break;
			}
		}

		return battery;
	}

	private static Vector2 CalculateAbsMax(Vector2 a, Vector2 b)
	{
		float absA = a.sqrMagnitude;
		float absB = b.sqrMagnitude;

		if (absA >= absB)
			return a;
		return b;
	}

	private static float CalculateAbsMax(float a, float b)
	{
		float absA = (a >= 0) ? a : -a;
		float absB = (b >= 0) ? b : -b;

		if (absA >= absB)
			return a;
		return b;
	}

	private static Vector2 CalculateDeadzone(Vector2 a, float deadzone)
	{
		if (a.sqrMagnitude <= (deadzone * deadzone))
			return Vector2.zero;

		a *= ((a.magnitude - deadzone) / (1.0f - deadzone));

		if (a.sqrMagnitude > 1.0f)
			return a.normalized;
		return a;
	}

	private static float CalculateDeadzone(float a, float deadzone)
	{
		float mag = (a >= 0) ? a : -a;

		if (mag <= deadzone)
			return 0.0f;

		a *= (mag - deadzone) / (1.0f - deadzone);

		if ((a * a) > 1.0f)
			return (a >= 0) ? 1.0f : -1.0f;
		return a;
	}

	private static bool ShouldResolveController(Controller controllerType, Controller controllerMask)
	{
		bool isValid = false;

		if ((controllerType & controllerMask) == controllerType)
		{
			isValid = true;
		}

		
		if (((controllerMask & Controller.Touch) == Controller.Touch)
			&& ((controllerType & Controller.Touch) != 0)
			&& ((controllerType & Controller.Touch) != Controller.Touch))
		{
			isValid = false;
		}

		
		if (((controllerMask & Controller.Hands) == Controller.Hands)
			&& ((controllerType & Controller.Hands) != 0)
			&& ((controllerType & Controller.Hands) != Controller.Hands))
		{
			isValid = false;
		}

		return isValid;
	}

	private abstract class OVRControllerBase
	{
		public class VirtualButtonMap
		{
			public RawButton None                     = RawButton.None;
			public RawButton One                      = RawButton.None;
			public RawButton Two                      = RawButton.None;
			public RawButton Three                    = RawButton.None;
			public RawButton Four                     = RawButton.None;
			public RawButton Start                    = RawButton.None;
			public RawButton Back                     = RawButton.None;
			public RawButton PrimaryShoulder          = RawButton.None;
			public RawButton PrimaryIndexTrigger      = RawButton.None;
			public RawButton PrimaryHandTrigger       = RawButton.None;
			public RawButton PrimaryThumbstick        = RawButton.None;
			public RawButton PrimaryThumbstickUp      = RawButton.None;
			public RawButton PrimaryThumbstickDown    = RawButton.None;
			public RawButton PrimaryThumbstickLeft    = RawButton.None;
			public RawButton PrimaryThumbstickRight   = RawButton.None;
			public RawButton PrimaryTouchpad          = RawButton.None;
			public RawButton SecondaryShoulder        = RawButton.None;
			public RawButton SecondaryIndexTrigger    = RawButton.None;
			public RawButton SecondaryHandTrigger     = RawButton.None;
			public RawButton SecondaryThumbstick      = RawButton.None;
			public RawButton SecondaryThumbstickUp    = RawButton.None;
			public RawButton SecondaryThumbstickDown  = RawButton.None;
			public RawButton SecondaryThumbstickLeft  = RawButton.None;
			public RawButton SecondaryThumbstickRight = RawButton.None;
			public RawButton SecondaryTouchpad        = RawButton.None;
			public RawButton DpadUp                   = RawButton.None;
			public RawButton DpadDown                 = RawButton.None;
			public RawButton DpadLeft                 = RawButton.None;
			public RawButton DpadRight                = RawButton.None;
			public RawButton Up                       = RawButton.None;
			public RawButton Down                     = RawButton.None;
			public RawButton Left                     = RawButton.None;
			public RawButton Right                    = RawButton.None;

			public RawButton ToRawMask(Button virtualMask)
			{
				RawButton rawMask = 0;

				if (virtualMask == Button.None)
					return RawButton.None;

				if ((virtualMask & Button.One) != 0)
					rawMask |= One;
				if ((virtualMask & Button.Two) != 0)
					rawMask |= Two;
				if ((virtualMask & Button.Three) != 0)
					rawMask |= Three;
				if ((virtualMask & Button.Four) != 0)
					rawMask |= Four;
				if ((virtualMask & Button.Start) != 0)
					rawMask |= Start;
				if ((virtualMask & Button.Back) != 0)
					rawMask |= Back;
				if ((virtualMask & Button.PrimaryShoulder) != 0)
					rawMask |= PrimaryShoulder;
				if ((virtualMask & Button.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Button.PrimaryHandTrigger) != 0)
					rawMask |= PrimaryHandTrigger;
				if ((virtualMask & Button.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Button.PrimaryThumbstickUp) != 0)
					rawMask |= PrimaryThumbstickUp;
				if ((virtualMask & Button.PrimaryThumbstickDown) != 0)
					rawMask |= PrimaryThumbstickDown;
				if ((virtualMask & Button.PrimaryThumbstickLeft) != 0)
					rawMask |= PrimaryThumbstickLeft;
				if ((virtualMask & Button.PrimaryThumbstickRight) != 0)
					rawMask |= PrimaryThumbstickRight;
				if ((virtualMask & Button.PrimaryTouchpad) != 0)
					rawMask |= PrimaryTouchpad;
				if ((virtualMask & Button.SecondaryShoulder) != 0)
					rawMask |= SecondaryShoulder;
				if ((virtualMask & Button.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Button.SecondaryHandTrigger) != 0)
					rawMask |= SecondaryHandTrigger;
				if ((virtualMask & Button.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;
				if ((virtualMask & Button.SecondaryThumbstickUp) != 0)
					rawMask |= SecondaryThumbstickUp;
				if ((virtualMask & Button.SecondaryThumbstickDown) != 0)
					rawMask |= SecondaryThumbstickDown;
				if ((virtualMask & Button.SecondaryThumbstickLeft) != 0)
					rawMask |= SecondaryThumbstickLeft;
				if ((virtualMask & Button.SecondaryThumbstickRight) != 0)
					rawMask |= SecondaryThumbstickRight;
				if ((virtualMask & Button.SecondaryTouchpad) != 0)
					rawMask |= SecondaryTouchpad;
				if ((virtualMask & Button.DpadUp) != 0)
					rawMask |= DpadUp;
				if ((virtualMask & Button.DpadDown) != 0)
					rawMask |= DpadDown;
				if ((virtualMask & Button.DpadLeft) != 0)
					rawMask |= DpadLeft;
				if ((virtualMask & Button.DpadRight) != 0)
					rawMask |= DpadRight;
				if ((virtualMask & Button.Up) != 0)
					rawMask |= Up;
				if ((virtualMask & Button.Down) != 0)
					rawMask |= Down;
				if ((virtualMask & Button.Left) != 0)
					rawMask |= Left;
				if ((virtualMask & Button.Right) != 0)
					rawMask |= Right;

				return rawMask;
			}
		}

		public class VirtualTouchMap
		{
			public RawTouch None                      = RawTouch.None;
			public RawTouch One                       = RawTouch.None;
			public RawTouch Two                       = RawTouch.None;
			public RawTouch Three                     = RawTouch.None;
			public RawTouch Four                      = RawTouch.None;
			public RawTouch PrimaryIndexTrigger       = RawTouch.None;
			public RawTouch PrimaryThumbstick         = RawTouch.None;
			public RawTouch PrimaryThumbRest          = RawTouch.None;
			public RawTouch PrimaryTouchpad           = RawTouch.None;
			public RawTouch SecondaryIndexTrigger     = RawTouch.None;
			public RawTouch SecondaryThumbstick       = RawTouch.None;
			public RawTouch SecondaryThumbRest        = RawTouch.None;
			public RawTouch SecondaryTouchpad         = RawTouch.None;

			public RawTouch ToRawMask(Touch virtualMask)
			{
				RawTouch rawMask = 0;

				if (virtualMask == Touch.None)
					return RawTouch.None;

				if ((virtualMask & Touch.One) != 0)
					rawMask |= One;
				if ((virtualMask & Touch.Two) != 0)
					rawMask |= Two;
				if ((virtualMask & Touch.Three) != 0)
					rawMask |= Three;
				if ((virtualMask & Touch.Four) != 0)
					rawMask |= Four;
				if ((virtualMask & Touch.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Touch.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Touch.PrimaryThumbRest) != 0)
					rawMask |= PrimaryThumbRest;
				if ((virtualMask & Touch.PrimaryTouchpad) != 0)
					rawMask |= PrimaryTouchpad;
				if ((virtualMask & Touch.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Touch.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;
				if ((virtualMask & Touch.SecondaryThumbRest) != 0)
					rawMask |= SecondaryThumbRest;
				if ((virtualMask & Touch.SecondaryTouchpad) != 0)
					rawMask |= SecondaryTouchpad;

				return rawMask;
			}
		}

		public class VirtualNearTouchMap
		{
			public RawNearTouch None                      = RawNearTouch.None;
			public RawNearTouch PrimaryIndexTrigger       = RawNearTouch.None;
			public RawNearTouch PrimaryThumbButtons       = RawNearTouch.None;
			public RawNearTouch SecondaryIndexTrigger     = RawNearTouch.None;
			public RawNearTouch SecondaryThumbButtons     = RawNearTouch.None;

			public RawNearTouch ToRawMask(NearTouch virtualMask)
			{
				RawNearTouch rawMask = 0;

				if (virtualMask == NearTouch.None)
					return RawNearTouch.None;

				if ((virtualMask & NearTouch.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & NearTouch.PrimaryThumbButtons) != 0)
					rawMask |= PrimaryThumbButtons;
				if ((virtualMask & NearTouch.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & NearTouch.SecondaryThumbButtons) != 0)
					rawMask |= SecondaryThumbButtons;

				return rawMask;
			}
		}

		public class VirtualAxis1DMap
		{
			public RawAxis1D None                      = RawAxis1D.None;
			public RawAxis1D PrimaryIndexTrigger       = RawAxis1D.None;
			public RawAxis1D PrimaryHandTrigger        = RawAxis1D.None;
			public RawAxis1D SecondaryIndexTrigger     = RawAxis1D.None;
			public RawAxis1D SecondaryHandTrigger      = RawAxis1D.None;

			public RawAxis1D ToRawMask(Axis1D virtualMask)
			{
				RawAxis1D rawMask = 0;

				if (virtualMask == Axis1D.None)
					return RawAxis1D.None;

				if ((virtualMask & Axis1D.PrimaryIndexTrigger) != 0)
					rawMask |= PrimaryIndexTrigger;
				if ((virtualMask & Axis1D.PrimaryHandTrigger) != 0)
					rawMask |= PrimaryHandTrigger;
				if ((virtualMask & Axis1D.SecondaryIndexTrigger) != 0)
					rawMask |= SecondaryIndexTrigger;
				if ((virtualMask & Axis1D.SecondaryHandTrigger) != 0)
					rawMask |= SecondaryHandTrigger;

				return rawMask;
			}
		}

		public class VirtualAxis2DMap
		{
			public RawAxis2D None                      = RawAxis2D.None;
			public RawAxis2D PrimaryThumbstick         = RawAxis2D.None;
			public RawAxis2D PrimaryTouchpad           = RawAxis2D.None;
			public RawAxis2D SecondaryThumbstick       = RawAxis2D.None;
			public RawAxis2D SecondaryTouchpad         = RawAxis2D.None;

			public RawAxis2D ToRawMask(Axis2D virtualMask)
			{
				RawAxis2D rawMask = 0;

				if (virtualMask == Axis2D.None)
					return RawAxis2D.None;

				if ((virtualMask & Axis2D.PrimaryThumbstick) != 0)
					rawMask |= PrimaryThumbstick;
				if ((virtualMask & Axis2D.PrimaryTouchpad) != 0)
					rawMask |= PrimaryTouchpad;
				if ((virtualMask & Axis2D.SecondaryThumbstick) != 0)
					rawMask |= SecondaryThumbstick;
				if ((virtualMask & Axis2D.SecondaryTouchpad) != 0)
					rawMask |= SecondaryTouchpad;

				return rawMask;
			}
		}

		public Controller controllerType = Controller.None;
		public VirtualButtonMap buttonMap = new VirtualButtonMap();
		public VirtualTouchMap touchMap = new VirtualTouchMap();
		public VirtualNearTouchMap nearTouchMap = new VirtualNearTouchMap();
		public VirtualAxis1DMap axis1DMap = new VirtualAxis1DMap();
		public VirtualAxis2DMap axis2DMap = new VirtualAxis2DMap();
		public OVRPlugin.ControllerState4 previousState = new OVRPlugin.ControllerState4();
		public OVRPlugin.ControllerState4 currentState = new OVRPlugin.ControllerState4();
		public bool shouldApplyDeadzone = true;

		public OVRControllerBase()
		{
			ConfigureButtonMap();
			ConfigureTouchMap();
			ConfigureNearTouchMap();
			ConfigureAxis1DMap();
			ConfigureAxis2DMap();
		}

		public virtual Controller Update()
		{
			OVRPlugin.ControllerState4 state;

			if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR && ( (controllerType & Controller.Touch) != 0) )
				state = GetOpenVRControllerState(controllerType);
			else
				state = OVRPlugin.GetControllerState4((uint)controllerType);

			if (state.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LIndexTrigger;
			if (state.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LHandTrigger;
			if (state.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickUp;
			if (state.LThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickDown;
			if (state.LThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickLeft;
			if (state.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickRight;

			if (state.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RIndexTrigger;
			if (state.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RHandTrigger;
			if (state.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickUp;
			if (state.RThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickDown;
			if (state.RThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickLeft;
			if (state.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickRight;

			previousState = currentState;
			currentState = state;

			return ((Controller)currentState.ConnectedControllers & controllerType);
		}

		private OVRPlugin.ControllerState4 GetOpenVRControllerState(Controller controllerType)
		{
			OVRPlugin.ControllerState4 state = new OVRPlugin.ControllerState4();

			if ((controllerType & Controller.LTouch) == Controller.LTouch && IsValidOpenVRDevice(openVRControllerDetails[0].deviceID))
			{
				OVR.OpenVR.VRControllerState_t leftControllerState = openVRControllerDetails[0].state;
				if ((leftControllerState.ulButtonPressed & ((ulong)OpenVRButton.Two)) == (ulong)OpenVRButton.Two)
					state.Buttons |= (uint)RawButton.Y;
				if ((leftControllerState.ulButtonPressed & ((ulong)OpenVRButton.Thumbstick)) == (ulong)OpenVRButton.Thumbstick)
					state.Buttons |= (uint)RawButton.LThumbstick;

				state.LIndexTrigger = leftControllerState.rAxis1.x;

				if (openVRControllerDetails[0].controllerType == OpenVRController.OculusTouch || openVRControllerDetails[0].controllerType == OpenVRController.ViveController)
				{
					state.LThumbstick.x = leftControllerState.rAxis0.x;
					state.LThumbstick.y = leftControllerState.rAxis0.y;
				}
				else if (openVRControllerDetails[0].controllerType == OpenVRController.WindowsMRController)
				{
					state.LThumbstick.x = leftControllerState.rAxis2.x;
					state.LThumbstick.y = leftControllerState.rAxis2.y;
				}

				if (openVRControllerDetails[0].controllerType == OpenVRController.OculusTouch)
					state.LHandTrigger = leftControllerState.rAxis2.x;
				else if (openVRControllerDetails[0].controllerType == OpenVRController.ViveController || openVRControllerDetails[0].controllerType == OpenVRController.WindowsMRController)
					state.LHandTrigger = ((leftControllerState.ulButtonPressed & ((ulong)OpenVRButton.Grip)) == ((ulong)OpenVRButton.Grip)) ? 1 : 0;

			}

			if ((controllerType & Controller.RTouch) == Controller.RTouch && IsValidOpenVRDevice(openVRControllerDetails[1].deviceID))
			{
				OVR.OpenVR.VRControllerState_t rightControllerState = openVRControllerDetails[1].state;
				if ((rightControllerState.ulButtonPressed & ((ulong)OpenVRButton.Two)) == (ulong)OpenVRButton.Two)
					state.Buttons |= (uint)RawButton.B;
				if ((rightControllerState.ulButtonPressed & ((ulong)OpenVRButton.Thumbstick)) == (ulong)OpenVRButton.Thumbstick)
					state.Buttons |= (uint)RawButton.RThumbstick;

				state.RIndexTrigger = rightControllerState.rAxis1.x;

				if (openVRControllerDetails[1].controllerType == OpenVRController.OculusTouch || openVRControllerDetails[1].controllerType == OpenVRController.ViveController)
				{
					state.RThumbstick.x = rightControllerState.rAxis0.x;
					state.RThumbstick.y = rightControllerState.rAxis0.y;
				}
				else if (openVRControllerDetails[1].controllerType == OpenVRController.WindowsMRController)
				{
					state.RThumbstick.x = rightControllerState.rAxis2.x;
					state.RThumbstick.y = rightControllerState.rAxis2.y;
				}

				if (openVRControllerDetails[1].controllerType == OpenVRController.OculusTouch)
					state.RHandTrigger = rightControllerState.rAxis2.x;
				else if (openVRControllerDetails[1].controllerType == OpenVRController.ViveController || openVRControllerDetails[1].controllerType == OpenVRController.WindowsMRController)
					state.RHandTrigger = ((rightControllerState.ulButtonPressed & ((ulong)OpenVRButton.Grip)) == ((ulong)OpenVRButton.Grip)) ? 1 : 0;

			}

			return state;
		}

		public virtual void SetControllerVibration(float frequency, float amplitude)
		{
			OVRPlugin.SetControllerVibration((uint)controllerType, frequency, amplitude);
		}

		public virtual byte GetBatteryPercentRemaining()
		{
			return 0;
		}

		public abstract void ConfigureButtonMap();
		public abstract void ConfigureTouchMap();
		public abstract void ConfigureNearTouchMap();
		public abstract void ConfigureAxis1DMap();
		public abstract void ConfigureAxis2DMap();

		public RawButton ResolveToRawMask(Button virtualMask)
		{
			return buttonMap.ToRawMask(virtualMask);
		}

		public RawTouch ResolveToRawMask(Touch virtualMask)
		{
			return touchMap.ToRawMask(virtualMask);
		}

		public RawNearTouch ResolveToRawMask(NearTouch virtualMask)
		{
			return nearTouchMap.ToRawMask(virtualMask);
		}

		public RawAxis1D ResolveToRawMask(Axis1D virtualMask)
		{
			return axis1DMap.ToRawMask(virtualMask);
		}

		public RawAxis2D ResolveToRawMask(Axis2D virtualMask)
		{
			return axis2DMap.ToRawMask(virtualMask);
		}
	}

	private class OVRControllerTouch : OVRControllerBase
	{
		public OVRControllerTouch()
		{
			controllerType = Controller.Touch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.RHandTrigger;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.A;
			touchMap.Two                       = RawTouch.B;
			touchMap.Three                     = RawTouch.X;
			touchMap.Four                      = RawTouch.Y;
			touchMap.PrimaryIndexTrigger       = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.LThumbstick;
			touchMap.PrimaryThumbRest          = RawTouch.LThumbRest;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.RIndexTrigger;
			touchMap.SecondaryThumbstick       = RawTouch.RThumbstick;
			touchMap.SecondaryThumbRest        = RawTouch.RThumbRest;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.RIndexTrigger;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.RThumbButtons;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.RHandTrigger;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			byte leftBattery = currentState.LBatteryPercentRemaining;
			byte rightBattery = currentState.RBatteryPercentRemaining;
			byte minBattery = (leftBattery <= rightBattery) ? leftBattery : rightBattery;

			return minBattery;
		}
	}

	private class OVRControllerLTouch : OVRControllerBase
	{
		public OVRControllerLTouch()
		{
			controllerType = Controller.LTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.X;
			buttonMap.Two                      = RawButton.Y;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.LHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.X;
			touchMap.Two                       = RawTouch.Y;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.LIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.LThumbstick;
			touchMap.PrimaryThumbRest          = RawTouch.LThumbRest;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.LIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.LThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.LHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			return currentState.LBatteryPercentRemaining;
		}
	}

	private class OVRControllerRTouch : OVRControllerBase
	{
		public OVRControllerRTouch()
		{
			controllerType = Controller.RTouch;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.None;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.RIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.RHandTrigger;
			buttonMap.PrimaryThumbstick        = RawButton.RThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.RThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.RThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.RThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.RThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.RThumbstickUp;
			buttonMap.Down                     = RawButton.RThumbstickDown;
			buttonMap.Left                     = RawButton.RThumbstickLeft;
			buttonMap.Right                    = RawButton.RThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.A;
			touchMap.Two                       = RawTouch.B;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.RIndexTrigger;
			touchMap.PrimaryThumbstick         = RawTouch.RThumbstick;
			touchMap.PrimaryThumbRest          = RawTouch.RThumbRest;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.RIndexTrigger;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.RThumbButtons;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.RIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.RHandTrigger;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.RThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			return currentState.RBatteryPercentRemaining;
		}
	}

	private class OVRControllerHands : OVRControllerBase
	{
		public OVRControllerHands()
		{
			controllerType = Controller.Hands;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.None;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.None;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.None;
			buttonMap.PrimaryThumbstickUp      = RawButton.None;
			buttonMap.PrimaryThumbstickDown    = RawButton.None;
			buttonMap.PrimaryThumbstickLeft    = RawButton.None;
			buttonMap.PrimaryThumbstickRight   = RawButton.None;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.None;
			buttonMap.Down                     = RawButton.None;
			buttonMap.Left                     = RawButton.None;
			buttonMap.Right                    = RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			byte leftBattery = currentState.LBatteryPercentRemaining;
			byte rightBattery = currentState.RBatteryPercentRemaining;
			byte minBattery = (leftBattery <= rightBattery) ? leftBattery : rightBattery;

			return minBattery;
		}
	}

	private class OVRControllerLHand : OVRControllerBase
	{
		public OVRControllerLHand()
		{
			controllerType = Controller.LHand;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.X;
			buttonMap.Two                      = RawButton.None;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.None;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.None;
			buttonMap.PrimaryThumbstickUp      = RawButton.None;
			buttonMap.PrimaryThumbstickDown    = RawButton.None;
			buttonMap.PrimaryThumbstickLeft    = RawButton.None;
			buttonMap.PrimaryThumbstickRight   = RawButton.None;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.None;
			buttonMap.Down                     = RawButton.None;
			buttonMap.Left                     = RawButton.None;
			buttonMap.Right                    = RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			return currentState.LBatteryPercentRemaining;
		}
	}

	private class OVRControllerRHand : OVRControllerBase
	{
		public OVRControllerRHand()
		{
			controllerType = Controller.RHand;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.None;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.None;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.None;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.None;
			buttonMap.PrimaryThumbstickUp      = RawButton.None;
			buttonMap.PrimaryThumbstickDown    = RawButton.None;
			buttonMap.PrimaryThumbstickLeft    = RawButton.None;
			buttonMap.PrimaryThumbstickRight   = RawButton.None;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.None;
			buttonMap.DpadDown                 = RawButton.None;
			buttonMap.DpadLeft                 = RawButton.None;
			buttonMap.DpadRight                = RawButton.None;
			buttonMap.Up                       = RawButton.None;
			buttonMap.Down                     = RawButton.None;
			buttonMap.Left                     = RawButton.None;
			buttonMap.Right                    = RawButton.None;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override byte GetBatteryPercentRemaining()
		{
			return currentState.RBatteryPercentRemaining;
		}
	}

	private class OVRControllerRemote : OVRControllerBase
	{
		public OVRControllerRemote()
		{
			controllerType = Controller.Remote;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.Start;
			buttonMap.Two                      = RawButton.Back;
			buttonMap.Three                    = RawButton.None;
			buttonMap.Four                     = RawButton.None;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.Back;
			buttonMap.PrimaryShoulder          = RawButton.None;
			buttonMap.PrimaryIndexTrigger      = RawButton.None;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.None;
			buttonMap.PrimaryThumbstickUp      = RawButton.None;
			buttonMap.PrimaryThumbstickDown    = RawButton.None;
			buttonMap.PrimaryThumbstickLeft    = RawButton.None;
			buttonMap.PrimaryThumbstickRight   = RawButton.None;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.None;
			buttonMap.SecondaryIndexTrigger    = RawButton.None;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.None;
			buttonMap.SecondaryThumbstickUp    = RawButton.None;
			buttonMap.SecondaryThumbstickDown  = RawButton.None;
			buttonMap.SecondaryThumbstickLeft  = RawButton.None;
			buttonMap.SecondaryThumbstickRight = RawButton.None;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.DpadUp;
			buttonMap.DpadDown                 = RawButton.DpadDown;
			buttonMap.DpadLeft                 = RawButton.DpadLeft;
			buttonMap.DpadRight                = RawButton.DpadRight;
			buttonMap.Up                       = RawButton.DpadUp;
			buttonMap.Down                     = RawButton.DpadDown;
			buttonMap.Left                     = RawButton.DpadLeft;
			buttonMap.Right                    = RawButton.DpadRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                  = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger   = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons   = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                     = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger      = RawAxis1D.None;
			axis1DMap.PrimaryHandTrigger       = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger    = RawAxis1D.None;
			axis1DMap.SecondaryHandTrigger     = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                     = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick        = RawAxis2D.None;
			axis2DMap.PrimaryTouchpad          = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick      = RawAxis2D.None;
			axis2DMap.SecondaryTouchpad        = RawAxis2D.None;
		}
	}

	private class OVRControllerGamepadPC : OVRControllerBase
	{
		public OVRControllerGamepadPC()
		{
			controllerType = Controller.Gamepad;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.Back;
			buttonMap.PrimaryShoulder          = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.DpadUp;
			buttonMap.DpadDown                 = RawButton.DpadDown;
			buttonMap.DpadLeft                 = RawButton.DpadLeft;
			buttonMap.DpadRight                = RawButton.DpadRight;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}
	}

	private class OVRControllerGamepadMac : OVRControllerBase
	{
		
		private enum AxisGPC
		{
			None = -1,
			LeftXAxis = 0,
			LeftYAxis,
			RightXAxis,
			RightYAxis,
			LeftTrigger,
			RightTrigger,
			DPad_X_Axis,
			DPad_Y_Axis,
			Max,
		};

		
		public enum ButtonGPC
		{
			None = -1,
			A = 0,
			B,
			X,
			Y,
			Up,
			Down,
			Left,
			Right,
			Start,
			Back,
			LStick,
			RStick,
			LeftShoulder,
			RightShoulder,
			Max
		};

		private bool initialized = false;

		public OVRControllerGamepadMac()
		{
			controllerType = Controller.Gamepad;

			initialized = OVR_GamepadController_Initialize();
		}

		~OVRControllerGamepadMac()
		{
			if (!initialized)
				return;

			OVR_GamepadController_Destroy();
		}

		public override Controller Update()
		{
			if (!initialized)
			{
				return Controller.None;
			}

			// BUG: Using New() allocation in Update() method.
			// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
			// 			OVRPlugin.ControllerState4 state = new OVRPlugin.ControllerState4();

			// FIXED CODE:
			Here's the code with the bug fixed:

	public override Controller Update()
	{
		// BUG: Using New() allocation in Update() method.
		// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
		// 			OVRPlugin.ControllerState4 state = new OVRPlugin.ControllerState4();

		// FIXED CODE:
		OVRPlugin.ControllerState4 state = OVRPlugin.new_ControllerState4();
	}

			bool result = OVR_GamepadController_Update();

			if (result)
				state.ConnectedControllers = (uint)Controller.Gamepad;

			if (OVR_GamepadController_GetButton((int)ButtonGPC.A))
				state.Buttons |= (uint)RawButton.A;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.B))
				state.Buttons |= (uint)RawButton.B;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.X))
				state.Buttons |= (uint)RawButton.X;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Y))
				state.Buttons |= (uint)RawButton.Y;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Up))
				state.Buttons |= (uint)RawButton.DpadUp;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Down))
				state.Buttons |= (uint)RawButton.DpadDown;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Left))
				state.Buttons |= (uint)RawButton.DpadLeft;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Right))
				state.Buttons |= (uint)RawButton.DpadRight;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Start))
				state.Buttons |= (uint)RawButton.Start;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.Back))
				state.Buttons |= (uint)RawButton.Back;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.LStick))
				state.Buttons |= (uint)RawButton.LThumbstick;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.RStick))
				state.Buttons |= (uint)RawButton.RThumbstick;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.LeftShoulder))
				state.Buttons |= (uint)RawButton.LShoulder;
			if (OVR_GamepadController_GetButton((int)ButtonGPC.RightShoulder))
				state.Buttons |= (uint)RawButton.RShoulder;

			state.LThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.LeftXAxis);
			state.LThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.LeftYAxis);
			state.RThumbstick.x = OVR_GamepadController_GetAxis((int)AxisGPC.RightXAxis);
			state.RThumbstick.y = OVR_GamepadController_GetAxis((int)AxisGPC.RightYAxis);
			state.LIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.LeftTrigger);
			state.RIndexTrigger = OVR_GamepadController_GetAxis((int)AxisGPC.RightTrigger);

			if (state.LIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LIndexTrigger;
			if (state.LHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LHandTrigger;
			if (state.LThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickUp;
			if (state.LThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickDown;
			if (state.LThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickLeft;
			if (state.LThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.LThumbstickRight;

			if (state.RIndexTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RIndexTrigger;
			if (state.RHandTrigger >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RHandTrigger;
			if (state.RThumbstick.y >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickUp;
			if (state.RThumbstick.y <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickDown;
			if (state.RThumbstick.x <= -AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickLeft;
			if (state.RThumbstick.x >= AXIS_AS_BUTTON_THRESHOLD)
				state.Buttons |= (uint)RawButton.RThumbstickRight;

			previousState = currentState;
			currentState = state;

			return ((Controller)currentState.ConnectedControllers & controllerType);
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.Back;
			buttonMap.PrimaryShoulder          = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.DpadUp;
			buttonMap.DpadDown                 = RawButton.DpadDown;
			buttonMap.DpadLeft                 = RawButton.DpadLeft;
			buttonMap.DpadRight                = RawButton.DpadRight;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}

		public override void SetControllerVibration(float frequency, float amplitude)
		{
			int gpcNode = 0;
			float gpcFrequency = frequency * 200.0f; //Map frequency from 0-1 CAPI range to 0-200 GPC range
			float gpcStrength = amplitude;

			OVR_GamepadController_SetVibration(gpcNode, gpcStrength, gpcFrequency);
		}

		private const string DllName = "OVRGamepad";

		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Initialize();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Destroy();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_Update();
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern float OVR_GamepadController_GetAxis(int axis);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_GetButton(int button);
		[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool OVR_GamepadController_SetVibration(int node, float strength, float frequency);
	}

	private class OVRControllerGamepadAndroid : OVRControllerBase
	{
		public OVRControllerGamepadAndroid()
		{
			controllerType = Controller.Gamepad;
		}

		public override void ConfigureButtonMap()
		{
			buttonMap.None                     = RawButton.None;
			buttonMap.One                      = RawButton.A;
			buttonMap.Two                      = RawButton.B;
			buttonMap.Three                    = RawButton.X;
			buttonMap.Four                     = RawButton.Y;
			buttonMap.Start                    = RawButton.Start;
			buttonMap.Back                     = RawButton.Back;
			buttonMap.PrimaryShoulder          = RawButton.LShoulder;
			buttonMap.PrimaryIndexTrigger      = RawButton.LIndexTrigger;
			buttonMap.PrimaryHandTrigger       = RawButton.None;
			buttonMap.PrimaryThumbstick        = RawButton.LThumbstick;
			buttonMap.PrimaryThumbstickUp      = RawButton.LThumbstickUp;
			buttonMap.PrimaryThumbstickDown    = RawButton.LThumbstickDown;
			buttonMap.PrimaryThumbstickLeft    = RawButton.LThumbstickLeft;
			buttonMap.PrimaryThumbstickRight   = RawButton.LThumbstickRight;
			buttonMap.PrimaryTouchpad          = RawButton.None;
			buttonMap.SecondaryShoulder        = RawButton.RShoulder;
			buttonMap.SecondaryIndexTrigger    = RawButton.RIndexTrigger;
			buttonMap.SecondaryHandTrigger     = RawButton.None;
			buttonMap.SecondaryThumbstick      = RawButton.RThumbstick;
			buttonMap.SecondaryThumbstickUp    = RawButton.RThumbstickUp;
			buttonMap.SecondaryThumbstickDown  = RawButton.RThumbstickDown;
			buttonMap.SecondaryThumbstickLeft  = RawButton.RThumbstickLeft;
			buttonMap.SecondaryThumbstickRight = RawButton.RThumbstickRight;
			buttonMap.SecondaryTouchpad        = RawButton.None;
			buttonMap.DpadUp                   = RawButton.DpadUp;
			buttonMap.DpadDown                 = RawButton.DpadDown;
			buttonMap.DpadLeft                 = RawButton.DpadLeft;
			buttonMap.DpadRight                = RawButton.DpadRight;
			buttonMap.Up                       = RawButton.LThumbstickUp;
			buttonMap.Down                     = RawButton.LThumbstickDown;
			buttonMap.Left                     = RawButton.LThumbstickLeft;
			buttonMap.Right                    = RawButton.LThumbstickRight;
		}

		public override void ConfigureTouchMap()
		{
			touchMap.None                      = RawTouch.None;
			touchMap.One                       = RawTouch.None;
			touchMap.Two                       = RawTouch.None;
			touchMap.Three                     = RawTouch.None;
			touchMap.Four                      = RawTouch.None;
			touchMap.PrimaryIndexTrigger       = RawTouch.None;
			touchMap.PrimaryThumbstick         = RawTouch.None;
			touchMap.PrimaryThumbRest          = RawTouch.None;
			touchMap.PrimaryTouchpad           = RawTouch.None;
			touchMap.SecondaryIndexTrigger     = RawTouch.None;
			touchMap.SecondaryThumbstick       = RawTouch.None;
			touchMap.SecondaryThumbRest        = RawTouch.None;
			touchMap.SecondaryTouchpad         = RawTouch.None;
		}

		public override void ConfigureNearTouchMap()
		{
			nearTouchMap.None                      = RawNearTouch.None;
			nearTouchMap.PrimaryIndexTrigger       = RawNearTouch.None;
			nearTouchMap.PrimaryThumbButtons       = RawNearTouch.None;
			nearTouchMap.SecondaryIndexTrigger     = RawNearTouch.None;
			nearTouchMap.SecondaryThumbButtons     = RawNearTouch.None;
		}

		public override void ConfigureAxis1DMap()
		{
			axis1DMap.None                      = RawAxis1D.None;
			axis1DMap.PrimaryIndexTrigger       = RawAxis1D.LIndexTrigger;
			axis1DMap.PrimaryHandTrigger        = RawAxis1D.None;
			axis1DMap.SecondaryIndexTrigger     = RawAxis1D.RIndexTrigger;
			axis1DMap.SecondaryHandTrigger      = RawAxis1D.None;
		}

		public override void ConfigureAxis2DMap()
		{
			axis2DMap.None                      = RawAxis2D.None;
			axis2DMap.PrimaryThumbstick         = RawAxis2D.LThumbstick;
			axis2DMap.PrimaryTouchpad           = RawAxis2D.None;
			axis2DMap.SecondaryThumbstick       = RawAxis2D.RThumbstick;
			axis2DMap.SecondaryTouchpad         = RawAxis2D.None;
		}
	}
}