/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https:

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

#if USING_XR_MANAGEMENT && USING_XR_SDK_OCULUS
#define USING_XR_SDK
#endif

#if UNITY_2020_1_OR_NEWER
#define REQUIRES_XR_SDK
#endif

#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || (UNITY_ANDROID && !UNITY_EDITOR))
#define OVRPLUGIN_UNSUPPORTED_PLATFORM
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
#define OVRPLUGIN_INCLUDE_MRC_ANDROID
#endif

using System;
using System.Runtime.InteropServices;
using UnityEngine;



public static class OVRPlugin
{
#if OVRPLUGIN_UNSUPPORTED_PLATFORM
	public const bool isSupportedPlatform = false;
#else
	public const bool isSupportedPlatform = true;
#endif

#if OVRPLUGIN_UNSUPPORTED_PLATFORM
	public static readonly System.Version wrapperVersion = _versionZero;
#else
	public static readonly System.Version wrapperVersion = OVRP_1_55_0.version;
#endif

#if !OVRPLUGIN_UNSUPPORTED_PLATFORM
	private static System.Version _version;
#endif
	public static System.Version version
	{
		get {
#if OVRPLUGIN_UNSUPPORTED_PLATFORM
			Debug.LogWarning("Platform is not currently supported by OVRPlugin");
			return _versionZero;
#else
			if (_version == null)
			{
				try
				{
					string pluginVersion = OVRP_1_1_0.ovrp_GetVersion();

					if (pluginVersion != null)
					{
						
						pluginVersion = pluginVersion.Split('-')[0];
						_version = new System.Version(pluginVersion);
					}
					else
					{
						_version = _versionZero;
					}
				}
				catch
				{
					_version = _versionZero;
				}

				
				if (_version == OVRP_0_5_0.version)
					_version = OVRP_0_1_0.version;

				if (_version > _versionZero && _version < OVRP_1_3_0.version)
					throw new PlatformNotSupportedException("Oculus Utilities version " + wrapperVersion + " is too new for OVRPlugin version " + _version.ToString() + ". Update to the latest version of Unity.");
			}

			return _version;
#endif
		}
	}

#if !OVRPLUGIN_UNSUPPORTED_PLATFORM
	private static System.Version _nativeSDKVersion;
#endif
	public static System.Version nativeSDKVersion
	{
		get {
#if OVRPLUGIN_UNSUPPORTED_PLATFORM
			return _versionZero;
#else
			if (_nativeSDKVersion == null)
			{
				try
				{
					string sdkVersion = string.Empty;

					if (version >= OVRP_1_1_0.version)
						sdkVersion = OVRP_1_1_0.ovrp_GetNativeSDKVersion();
					else
						sdkVersion = _versionZero.ToString();

					if (sdkVersion != null)
					{
						
						sdkVersion = sdkVersion.Split('-')[0];
						_nativeSDKVersion = new System.Version(sdkVersion);
					}
					else
					{
						_nativeSDKVersion = _versionZero;
					}
				}
				catch
				{
					_nativeSDKVersion = _versionZero;
				}
			}

			return _nativeSDKVersion;
#endif
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private class GUID
	{
		public int a;
		public short b;
		public short c;
		public byte d0;
		public byte d1;
		public byte d2;
		public byte d3;
		public byte d4;
		public byte d5;
		public byte d6;
		public byte d7;
	}

	public enum Bool
	{
		False = 0,
		True
	}

	public enum Result
	{
		
		Success = 0,

		
		Failure = -1000,
		Failure_InvalidParameter = -1001,
		Failure_NotInitialized = -1002,
		Failure_InvalidOperation = -1003,
		Failure_Unsupported = -1004,
		Failure_NotYetImplemented = -1005,
		Failure_OperationFailed = -1006,
		Failure_InsufficientSize = -1007,
		Failure_DataIsInvalid = -1008,
		Failure_DeprecatedOperation = -1009
	}

	public enum CameraStatus
	{
		CameraStatus_None,
		CameraStatus_Connected,
		CameraStatus_Calibrating,
		CameraStatus_CalibrationFailed,
		CameraStatus_Calibrated,
		CameraStatus_ThirdPerson,
		CameraStatus_EnumSize = 0x7fffffff
	}

	public enum CameraAnchorType
	{
		CameraAnchorType_PreDefined = 0,
		CameraAnchorType_Custom = 1,
		CameraAnchorType_Count,
		CameraAnchorType_EnumSize = 0x7fffffff
	}

	public enum XrApi
	{
		Unknown = 0,
		CAPI = 1,
		VRAPI = 2,
		OpenXR = 3,
		EnumSize = 0x7fffffff
	}

	public enum Eye
	{
		None = -1,
		Left = 0,
		Right = 1,
		Count = 2
	}

	public enum Tracker
	{
		None = -1,
		Zero = 0,
		One = 1,
		Two = 2,
		Three = 3,
		Count,
	}

	public enum Node
	{
		None = -1,
		EyeLeft = 0,
		EyeRight = 1,
		EyeCenter = 2,
		HandLeft = 3,
		HandRight = 4,
		TrackerZero = 5,
		TrackerOne = 6,
		TrackerTwo = 7,
		TrackerThree = 8,
		Head = 9,
		DeviceObjectZero = 10,
		Count,
	}

	public enum Controller
	{
		None = 0,
		LTouch = 0x00000001,
		RTouch = 0x00000002,
		Touch = LTouch | RTouch,
		Remote = 0x00000004,
		Gamepad = 0x00000010,
		LHand = 0x00000020,
		RHand = 0x00000040,
		Hands = LHand | RHand,
		Active = unchecked((int)0x80000000),
		All = ~None,
	}

	public enum Handedness
	{
		Unsupported = 0,
		LeftHanded = 1,
		RightHanded = 2,
	}

	public enum TrackingOrigin
	{
		EyeLevel = 0,
		FloorLevel = 1,
		Stage = 2,
		Count,
	}

	public enum RecenterFlags
	{
		Default = 0,
		IgnoreAll = unchecked((int)0x80000000),
		Count,
	}

	public enum BatteryStatus
	{
		Charging = 0,
		Discharging,
		Full,
		NotCharging,
		Unknown,
	}

	public enum EyeTextureFormat
	{
		Default = 0,
		R8G8B8A8_sRGB = 0,
		R8G8B8A8 = 1,
		R16G16B16A16_FP = 2,
		R11G11B10_FP = 3,
		B8G8R8A8_sRGB = 4,
		B8G8R8A8 = 5,
		R5G6B5 = 11,
		EnumSize = 0x7fffffff
	}

	public enum PlatformUI
	{
		None = -1,
		ConfirmQuit = 1,
		GlobalMenuTutorial, 
	}

	public enum SystemRegion
	{
		Unspecified = 0,
		Japan,
		China,
	}

	public enum SystemHeadset
	{
		None = 0,

		
		Oculus_Quest = 8,
		Oculus_Quest_2 = 9,
		Placeholder_10,
		Placeholder_11,
		Placeholder_12,
		Placeholder_13,
		Placeholder_14,

		
		Rift_DK1 = 0x1000,
		Rift_DK2,
		Rift_CV1,
		Rift_CB,
		Rift_S,
		Oculus_Link_Quest,
		Oculus_Link_Quest_2,
		PC_Placeholder_4103,
		PC_Placeholder_4104,
		PC_Placeholder_4105,
		PC_Placeholder_4106,
		PC_Placeholder_4107
	}

	public enum OverlayShape
	{
		Quad = 0,
		Cylinder = 1,
		Cubemap = 2,
		OffcenterCubemap = 4,
		Equirect = 5,
	}

	public enum Step
	{
		Render = -1,
		Physics = 0, 
	}

	public enum CameraDevice
	{
		None = 0,
		WebCamera0 = 100,
		WebCamera1 = 101,
		ZEDCamera = 300,
	}

	public enum CameraDeviceDepthSensingMode
	{
		Standard = 0,
		Fill = 1,
	}

	public enum CameraDeviceDepthQuality
	{
		Low = 0,
		Medium = 1,
		High = 2,
	}

	public enum FixedFoveatedRenderingLevel
	{
		Off = 0,
		Low = 1,
		Medium = 2,
		High = 3,
		
		HighTop = 4,
		EnumSize = 0x7FFFFFFF
	}

	[Obsolete("Please use FixedFoveatedRenderingLevel instead", false)]
	public enum TiledMultiResLevel
	{
		Off = 0,
		LMSLow = FixedFoveatedRenderingLevel.Low,
		LMSMedium = FixedFoveatedRenderingLevel.Medium,
		LMSHigh = FixedFoveatedRenderingLevel.High,
		
		LMSHighTop = FixedFoveatedRenderingLevel.HighTop,
		EnumSize = 0x7FFFFFFF
	}

	public enum PerfMetrics
	{
		App_CpuTime_Float = 0,
		App_GpuTime_Float = 1,

		Compositor_CpuTime_Float = 3,
		Compositor_GpuTime_Float = 4,
		Compositor_DroppedFrameCount_Int = 5,

		System_GpuUtilPercentage_Float = 7,
		System_CpuUtilAveragePercentage_Float = 8,
		System_CpuUtilWorstPercentage_Float = 9,

		
		Device_CpuClockFrequencyInMHz_Float = 10,
		Device_GpuClockFrequencyInMHz_Float = 11,
		Device_CpuClockLevel_Int = 12,
		Device_GpuClockLevel_Int = 13,

		Count,
		EnumSize = 0x7FFFFFFF
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CameraDeviceIntrinsicsParameters
	{
		float fx; /* Focal length in pixels along x axis. */
		float fy; /* Focal length in pixels along y axis. */
		float cx; /* Optical center along x axis, defined in pixels (usually close to width/2). */
		float cy; /* Optical center along y axis, defined in pixels (usually close to height/2). */
		double disto0; /* Distortion factor : [ k1, k2, p1, p2, k3 ]. Radial (k1,k2,k3) and Tangential (p1,p2) distortion.*/
		double disto1;
		double disto2;
		double disto3;
		double disto4;
		float v_fov; /* Vertical field of view after stereo rectification, in degrees. */
		float h_fov; /* Horizontal field of view after stereo rectification, in degrees.*/
		float d_fov; /* Diagonal field of view after stereo rectification, in degrees.*/
		int w; /* Resolution width */
		int h; /* Resolution height */
	}

	private const int OverlayShapeFlagShift = 4;
	private enum OverlayFlag
	{
		None = unchecked((int)0x00000000),
		OnTop = unchecked((int)0x00000001),
		HeadLocked = unchecked((int)0x00000002),
		NoDepth = unchecked((int)0x00000004),
		ExpensiveSuperSample = unchecked((int)0x00000008),

		
		ShapeFlag_Quad = unchecked((int)OverlayShape.Quad << OverlayShapeFlagShift),
		ShapeFlag_Cylinder = unchecked((int)OverlayShape.Cylinder << OverlayShapeFlagShift),
		ShapeFlag_Cubemap = unchecked((int)OverlayShape.Cubemap << OverlayShapeFlagShift),
		ShapeFlag_OffcenterCubemap = unchecked((int)OverlayShape.OffcenterCubemap << OverlayShapeFlagShift),
		ShapeFlagRangeMask = unchecked((int)0xF << OverlayShapeFlagShift),

		Hidden = unchecked((int)0x000000200),
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2f
	{
		public float x;
		public float y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3f
	{
		public float x;
		public float y;
		public float z;
		public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}", x, y, z);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4f
	{
		public float x;
		public float y;
		public float z;
		public float w;
		public static readonly Vector4f zero = new Vector4f { x = 0.0f, y = 0.0f, z = 0.0f, w = 0.0f };
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4s
	{
		public short x;
		public short y;
		public short z;
		public short w;
		public static readonly Vector4s zero = new Vector4s { x = 0, y = 0, z = 0, w = 0 };
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Quatf
	{
		public float x;
		public float y;
		public float z;
		public float w;
		public static readonly Quatf identity = new Quatf { x = 0.0f, y = 0.0f, z = 0.0f, w = 1.0f };
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Posef
	{
		public Quatf Orientation;
		public Vector3f Position;
		public static readonly Posef identity = new Posef { Orientation = Quatf.identity, Position = Vector3f.zero };
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Position ({0}), Orientation({1})", Position, Orientation);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TextureRectMatrixf
	{
		public Rect leftRect;
		public Rect rightRect;
		public Vector4 leftScaleBias;
		public Vector4 rightScaleBias;
		public static readonly TextureRectMatrixf zero = new TextureRectMatrixf { leftRect = new Rect(0, 0, 1, 1), rightRect = new Rect(0, 0, 1, 1), leftScaleBias = new Vector4(1, 1, 0, 0), rightScaleBias = new Vector4(1, 1, 0, 0) };

		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Rect Left ({0}), Rect Right({1}), Scale Bias Left ({2}), Scale Bias Right({3})", leftRect, rightRect, leftScaleBias, rightScaleBias);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PoseStatef
	{
		public Posef Pose;
		public Vector3f Velocity;
		public Vector3f Acceleration;
		public Vector3f AngularVelocity;
		public Vector3f AngularAcceleration;
		public double Time;

		public static readonly PoseStatef identity = new PoseStatef
		{
			Pose = Posef.identity,
			Velocity = Vector3f.zero,
			Acceleration = Vector3f.zero,
			AngularVelocity = Vector3f.zero,
			AngularAcceleration = Vector3f.zero
		};
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ControllerState4
	{
		public uint ConnectedControllers;
		public uint Buttons;
		public uint Touches;
		public uint NearTouches;
		public float LIndexTrigger;
		public float RIndexTrigger;
		public float LHandTrigger;
		public float RHandTrigger;
		public Vector2f LThumbstick;
		public Vector2f RThumbstick;
		public Vector2f LTouchpad;
		public Vector2f RTouchpad;
		public byte LBatteryPercentRemaining;
		public byte RBatteryPercentRemaining;
		public byte LRecenterCount;
		public byte RRecenterCount;
		public byte Reserved_27;
		public byte Reserved_26;
		public byte Reserved_25;
		public byte Reserved_24;
		public byte Reserved_23;
		public byte Reserved_22;
		public byte Reserved_21;
		public byte Reserved_20;
		public byte Reserved_19;
		public byte Reserved_18;
		public byte Reserved_17;
		public byte Reserved_16;
		public byte Reserved_15;
		public byte Reserved_14;
		public byte Reserved_13;
		public byte Reserved_12;
		public byte Reserved_11;
		public byte Reserved_10;
		public byte Reserved_09;
		public byte Reserved_08;
		public byte Reserved_07;
		public byte Reserved_06;
		public byte Reserved_05;
		public byte Reserved_04;
		public byte Reserved_03;
		public byte Reserved_02;
		public byte Reserved_01;
		public byte Reserved_00;

		public ControllerState4(ControllerState2 cs)
		{
			ConnectedControllers = cs.ConnectedControllers;
			Buttons = cs.Buttons;
			Touches = cs.Touches;
			NearTouches = cs.NearTouches;
			LIndexTrigger = cs.LIndexTrigger;
			RIndexTrigger = cs.RIndexTrigger;
			LHandTrigger = cs.LHandTrigger;
			RHandTrigger = cs.RHandTrigger;
			LThumbstick = cs.LThumbstick;
			RThumbstick = cs.RThumbstick;
			LTouchpad = cs.LTouchpad;
			RTouchpad = cs.RTouchpad;
			LBatteryPercentRemaining = 0;
			RBatteryPercentRemaining = 0;
			LRecenterCount = 0;
			RRecenterCount = 0;
			Reserved_27 = 0;
			Reserved_26 = 0;
			Reserved_25 = 0;
			Reserved_24 = 0;
			Reserved_23 = 0;
			Reserved_22 = 0;
			Reserved_21 = 0;
			Reserved_20 = 0;
			Reserved_19 = 0;
			Reserved_18 = 0;
			Reserved_17 = 0;
			Reserved_16 = 0;
			Reserved_15 = 0;
			Reserved_14 = 0;
			Reserved_13 = 0;
			Reserved_12 = 0;
			Reserved_11 = 0;
			Reserved_10 = 0;
			Reserved_09 = 0;
			Reserved_08 = 0;
			Reserved_07 = 0;
			Reserved_06 = 0;
			Reserved_05 = 0;
			Reserved_04 = 0;
			Reserved_03 = 0;
			Reserved_02 = 0;
			Reserved_01 = 0;
			Reserved_00 = 0;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ControllerState2
	{
		public uint ConnectedControllers;
		public uint Buttons;
		public uint Touches;
		public uint NearTouches;
		public float LIndexTrigger;
		public float RIndexTrigger;
		public float LHandTrigger;
		public float RHandTrigger;
		public Vector2f LThumbstick;
		public Vector2f RThumbstick;
		public Vector2f LTouchpad;
		public Vector2f RTouchpad;

		public ControllerState2(ControllerState cs)
		{
			ConnectedControllers = cs.ConnectedControllers;
			Buttons = cs.Buttons;
			Touches = cs.Touches;
			NearTouches = cs.NearTouches;
			LIndexTrigger = cs.LIndexTrigger;
			RIndexTrigger = cs.RIndexTrigger;
			LHandTrigger = cs.LHandTrigger;
			RHandTrigger = cs.RHandTrigger;
			LThumbstick = cs.LThumbstick;
			RThumbstick = cs.RThumbstick;
			LTouchpad = new Vector2f() { x = 0.0f, y = 0.0f };
			RTouchpad = new Vector2f() { x = 0.0f, y = 0.0f };
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ControllerState
	{
		public uint ConnectedControllers;
		public uint Buttons;
		public uint Touches;
		public uint NearTouches;
		public float LIndexTrigger;
		public float RIndexTrigger;
		public float LHandTrigger;
		public float RHandTrigger;
		public Vector2f LThumbstick;
		public Vector2f RThumbstick;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HapticsBuffer
	{
		public IntPtr Samples;
		public int SamplesCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HapticsState
	{
		public int SamplesAvailable;
		public int SamplesQueued;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HapticsDesc
	{
		public int SampleRateHz;
		public int SampleSizeInBytes;
		public int MinimumSafeSamplesQueued;
		public int MinimumBufferSamplesCount;
		public int OptimalBufferSamplesCount;
		public int MaximumBufferSamplesCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct AppPerfFrameStats
	{
		public int HmdVsyncIndex;
		public int AppFrameIndex;
		public int AppDroppedFrameCount;
		public float AppMotionToPhotonLatency;
		public float AppQueueAheadTime;
		public float AppCpuElapsedTime;
		public float AppGpuElapsedTime;
		public int CompositorFrameIndex;
		public int CompositorDroppedFrameCount;
		public float CompositorLatency;
		public float CompositorCpuElapsedTime;
		public float CompositorGpuElapsedTime;
		public float CompositorCpuStartToGpuEndElapsedTime;
		public float CompositorGpuEndToVsyncElapsedTime;
	}

	public const int AppPerfFrameStatsMaxCount = 5;

	[StructLayout(LayoutKind.Sequential)]
	public struct AppPerfStats
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = AppPerfFrameStatsMaxCount)]
		public AppPerfFrameStats[] FrameStats;
		public int FrameStatsCount;
		public Bool AnyFrameStatsDropped;
		public float AdaptiveGpuPerformanceScale;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Sizei
	{
		public int w;
		public int h;

		public static readonly Sizei zero = new Sizei { w = 0, h = 0 };
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Sizef
	{
		public float w;
		public float h;

		public static readonly Sizef zero = new Sizef { w = 0, h = 0 };
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2i
	{
		public int x;
		public int y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Recti {
		Vector2i Pos;
		Sizei Size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Rectf {
		Vector2f Pos;
		Sizef Size;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Frustumf
	{
		public float zNear;
		public float zFar;
		public float fovX;
		public float fovY;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Frustumf2
	{
		public float zNear;
		public float zFar;
		public Fovf Fov;
	}

	public enum BoundaryType
	{
		OuterBoundary = 0x0001,
		PlayArea = 0x0100,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BoundaryTestResult
	{
		public Bool IsTriggering;
		public float ClosestDistance;
		public Vector3f ClosestPoint;
		public Vector3f ClosestPointNormal;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BoundaryGeometry
	{
		public BoundaryType BoundaryType;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public Vector3f[] Points;
		public int PointsCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Colorf
	{
		public float r;
		public float g;
		public float b;
		public float a;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Fovf
	{
		public float UpTan;
		public float DownTan;
		public float LeftTan;
		public float RightTan;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CameraIntrinsics
	{
		public Bool IsValid;
		public double LastChangedTimeSeconds;
		public Fovf FOVPort;
		public float VirtualNearPlaneDistanceMeters;
		public float VirtualFarPlaneDistanceMeters;
		public Sizei ImageSensorPixelResolution;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct CameraExtrinsics
	{
		public Bool IsValid;
		public double LastChangedTimeSeconds;
		public CameraStatus CameraStatusData;
		public Node AttachedToNode;
		public Posef RelativePose;
	}

	public enum LayerLayout
	{
		Stereo = 0,
		Mono = 1,
		DoubleWide = 2,
		Array = 3,
		EnumSize = 0xF
	}

	public enum LayerFlags
	{
		Static = (1 << 0),
		LoadingScreen = (1 << 1),
		SymmetricFov = (1 << 2),
		TextureOriginAtBottomLeft = (1 << 3),
		ChromaticAberrationCorrection = (1 << 4),
		NoAllocation = (1 << 5),
		ProtectedContent = (1 << 6),
		AndroidSurfaceSwapChain = (1 << 7),
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LayerDesc
	{
		public OverlayShape Shape;
		public LayerLayout Layout;
		public Sizei TextureSize;
		public int MipLevels;
		public int SampleCount;
		public EyeTextureFormat Format;
		public int LayerFlags;

		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Fovf[] Fov;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		public Rectf[] VisibleRect;
		public Sizei MaxViewportSize;
		EyeTextureFormat DepthFormat;

		public override string ToString()
		{
			string delim = ", ";
			return Shape.ToString()
				+ delim + Layout.ToString()
				+ delim + TextureSize.w.ToString() + "x" + TextureSize.h.ToString()
				+ delim + MipLevels.ToString()
				+ delim + SampleCount.ToString()
				+ delim + Format.ToString()
				+ delim + LayerFlags.ToString();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LayerSubmit
	{
		int LayerId;
		int TextureStage;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		Recti[] ViewportRect;
		Posef Pose;
		int LayerSubmitFlags;
	}

	public enum TrackingConfidence
	{
		Low = 0,
		High = 0x3f800000,
	}

	public enum Hand
	{
		None = -1,
		HandLeft = 0,
		HandRight = 1,
	}

	[Flags]
	public enum HandStatus
	{
		HandTracked = (1 << 0), 
		InputStateValid = (1 << 1), 
		SystemGestureInProgress = (1 << 6), 
		DominantHand = (1 << 7), 
		MenuPressed = (1 << 8) 
	}

	public enum BoneId
	{
		Invalid                 = -1,

		
		Hand_Start              = 0,
		Hand_WristRoot          = Hand_Start + 0, 
		Hand_ForearmStub        = Hand_Start + 1, 
		Hand_Thumb0             = Hand_Start + 2, 
		Hand_Thumb1             = Hand_Start + 3, 
		Hand_Thumb2             = Hand_Start + 4, 
		Hand_Thumb3             = Hand_Start + 5, 
		Hand_Index1             = Hand_Start + 6, 
		Hand_Index2             = Hand_Start + 7, 
		Hand_Index3             = Hand_Start + 8, 
		Hand_Middle1            = Hand_Start + 9, 
		Hand_Middle2            = Hand_Start + 10, 
		Hand_Middle3            = Hand_Start + 11, 
		Hand_Ring1              = Hand_Start + 12, 
		Hand_Ring2              = Hand_Start + 13, 
		Hand_Ring3              = Hand_Start + 14, 
		Hand_Pinky0             = Hand_Start + 15, 
		Hand_Pinky1             = Hand_Start + 16, 
		Hand_Pinky2             = Hand_Start + 17, 
		Hand_Pinky3             = Hand_Start + 18, 
		Hand_MaxSkinnable       = Hand_Start + 19,
		
		
		Hand_ThumbTip           = Hand_MaxSkinnable + 0, 
		Hand_IndexTip           = Hand_MaxSkinnable + 1, 
		Hand_MiddleTip          = Hand_MaxSkinnable + 2, 
		Hand_RingTip            = Hand_MaxSkinnable + 3, 
		Hand_PinkyTip           = Hand_MaxSkinnable + 4, 
		Hand_End                = Hand_MaxSkinnable + 5,


		

		// BUG: Constant condition
		// MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
		// 		Max = ((int)Hand_End > 50) ? (int)Hand_End : 50,

		// FIXED CODE:
