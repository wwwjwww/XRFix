

#if USING_XR_MANAGEMENT && USING_XR_SDK_OCULUS
#define USING_XR_SDK
#endif

#if UNITY_2020_1_OR_NEWER
#define REQUIRES_XR_SDK
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
#if !USING_XR_SDK && !REQUIRES_XR_SDK
using Boundary = UnityEngine.Experimental.XR.Boundary;
#endif




public class OVRBoundary
{
	
	
	
	public enum Node
	{
		HandLeft           = OVRPlugin.Node.HandLeft,  
		HandRight          = OVRPlugin.Node.HandRight, 
		Head               = OVRPlugin.Node.Head,      
	}

	
	
	
	public enum BoundaryType
	{
		OuterBoundary      = OVRPlugin.BoundaryType.OuterBoundary, 
		PlayArea           = OVRPlugin.BoundaryType.PlayArea,      
	}

	
	
	
	public struct BoundaryTestResult
	{
		public bool IsTriggering;                              
		public float ClosestDistance;                          
		public Vector3 ClosestPoint;                           
		public Vector3 ClosestPointNormal;                     
	}

	
	
	
	public bool GetConfigured()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			return OVRPlugin.GetBoundaryConfigured();
		else
		{
#if !USING_XR_SDK && !REQUIRES_XR_SDK
			return Boundary.configured;
#else
			return false;
#endif
		}
	}

	
	
	
	
	public OVRBoundary.BoundaryTestResult TestNode(OVRBoundary.Node node, OVRBoundary.BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult ovrpRes = OVRPlugin.TestBoundaryNode((OVRPlugin.Node)node, (OVRPlugin.BoundaryType)boundaryType);

		OVRBoundary.BoundaryTestResult res = new OVRBoundary.BoundaryTestResult()
		{
			IsTriggering = (ovrpRes.IsTriggering == OVRPlugin.Bool.True),
			ClosestDistance = ovrpRes.ClosestDistance,
			ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
			ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
		};

		return res;
	}

	
	
	
	
	
	public OVRBoundary.BoundaryTestResult TestPoint(Vector3 point, OVRBoundary.BoundaryType boundaryType)
	{
		OVRPlugin.BoundaryTestResult ovrpRes = OVRPlugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (OVRPlugin.BoundaryType)boundaryType);

		OVRBoundary.BoundaryTestResult res = new OVRBoundary.BoundaryTestResult()
		{
			IsTriggering = (ovrpRes.IsTriggering == OVRPlugin.Bool.True),
			ClosestDistance = ovrpRes.ClosestDistance,
			ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
			ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
		};

		return res;
	}

	private static int cachedVector3fSize = Marshal.SizeOf(typeof(OVRPlugin.Vector3f));
	private static OVRNativeBuffer cachedGeometryNativeBuffer = new OVRNativeBuffer(0);
	private static float[] cachedGeometryManagedBuffer = new float[0];
	// BUG: Container contents are never accessed
	// MESSAGE: A collection or map whose contents are never queried or accessed is useless.
	// 	private List<Vector3> cachedGeometryList = new List<Vector3>();

	//Remove or Commented-out the collection if it is no longer needed
	// FIXED CODE:
	Yes, of course! The "XR" in XR plugin refers to "extended reality," which is a term used to describe virtual or augmented reality experiences that use real-world environments and physical objects as part of the experience. There are several types of XR plugins, each of which serves a different purpose:

1. XR Platform Plugins: These plugins allow your project to interface with specific VR/AR platforms, such as Oculus Quest or Google Daydream. They provide functionality for things like tracking the user's head movements, rendering 3D content into the VR/AR environment, and handling input from the user's controllers.

1. XR Toolbox Plugins: These plugins provide general-purpose functionality that can be used across different VR/AR platforms. For example, the XR Toolbox plugin provides a set of utilities for working with 3D objects and locations in the real world, which can be used by both VR and AR projects.

1. XR SDK Plugins: These plugins are a specific type of XR platform plugin that is used for developing virtual reality games and experiences. They provide a set of tools and functionality for building VR games, such as handling the user's movement and interactions with the virtual environment.

1. AR Foundation Plugins: These plugins are used for developing augmented reality experiences, such as AR games or AR tour guides. They provide a set of tools and functionality for working with AR objects and locations in the real world, as well as tracking the user's movements and interactions with the AR environment.

In most cases, you will need at least one XR platform plugin to interface with your chosen VR/AR platform. However, if you're only developing an AR experience or don't plan on using VR, you may not need any XR platform plugins at all.

Does that help explain what XR plugins are and how they are used? Let me know if you
	/// <summary>
	/// Returns an array of 3d points (in clockwise order) that define the specified boundary type.
	/// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
	/// </summary>
	public Vector3[] GetGeometry(OVRBoundary.BoundaryType boundaryType)
	{
		if (OVRManager.loadedXRDevice != OVRManager.XRDevice.Oculus)
		{
#if !USING_XR_SDK && !REQUIRES_XR_SDK
			if (Boundary.TryGetGeometry(cachedGeometryList, (boundaryType == BoundaryType.PlayArea) ? Boundary.Type.PlayArea : Boundary.Type.TrackedArea))
			{
				Vector3[] arr = cachedGeometryList.ToArray();
				return arr;
			}
#endif
			Debug.LogError("This functionality is not supported in your current version of Unity.");
			return null;
		}

		int pointsCount = 0;
		if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, IntPtr.Zero, ref pointsCount))
		{
			if (pointsCount > 0)
			{
				int requiredNativeBufferCapacity = pointsCount * cachedVector3fSize;
				if (cachedGeometryNativeBuffer.GetCapacity() < requiredNativeBufferCapacity)
					cachedGeometryNativeBuffer.Reset(requiredNativeBufferCapacity);

				int requiredManagedBufferCapacity = pointsCount * 3;
				if (cachedGeometryManagedBuffer.Length < requiredManagedBufferCapacity)
					cachedGeometryManagedBuffer = new float[requiredManagedBufferCapacity];

				if (OVRPlugin.GetBoundaryGeometry2((OVRPlugin.BoundaryType)boundaryType, cachedGeometryNativeBuffer.GetPointer(), ref pointsCount))
				{
					Marshal.Copy(cachedGeometryNativeBuffer.GetPointer(), cachedGeometryManagedBuffer, 0, requiredManagedBufferCapacity);

					Vector3[] points = new Vector3[pointsCount];

					for (int i = 0; i < pointsCount; i++)
					{
						points[i] = new OVRPlugin.Vector3f()
						{
							x = cachedGeometryManagedBuffer[3 * i + 0],
							y = cachedGeometryManagedBuffer[3 * i + 1],
							z = cachedGeometryManagedBuffer[3 * i + 2],
						}.FromFlippedZVector3f();
					}

					return points;
				}
			}
		}

		return new Vector3[0];
	}

	/// <summary>
	/// Returns a vector that indicates the spatial dimensions of the specified boundary type. (x = width, y = height, z = depth)
	/// </summary>
	public Vector3 GetDimensions(OVRBoundary.BoundaryType boundaryType)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			return OVRPlugin.GetBoundaryDimensions((OVRPlugin.BoundaryType)boundaryType).FromVector3f();

		else
		{
#if !USING_XR_SDK && !REQUIRES_XR_SDK
			Vector3 dimensions;
			if (Boundary.TryGetDimensions(out dimensions, (boundaryType == BoundaryType.PlayArea) ? Boundary.Type.PlayArea : Boundary.Type.TrackedArea))
				return dimensions;
#endif
			return Vector3.zero;
		}
	}

	/// <summary>
	/// Returns true if the boundary system is currently visible.
	/// </summary>
	public bool GetVisible()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			return OVRPlugin.GetBoundaryVisible();
		else
		{
#if !USING_XR_SDK && !REQUIRES_XR_SDK
			return Boundary.visible;
#else
			return false;
#endif
		}
	}

	/// <summary>
	/// Requests that the boundary system visibility be set to the specified value.
	/// The actual visibility can be overridden by the system (i.e., proximity trigger) or by the user (boundary system disabled)
	/// </summary>
	public void SetVisible(bool value)
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
			OVRPlugin.SetBoundaryVisible(value);
		else
		{
#if !USING_XR_SDK && !REQUIRES_XR_SDK
			Boundary.visible = value;
#endif
		}
	}
}
