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

/// <summary>
/// Provides access to the Oculus boundary system.
/// </summary>
public class OVRBoundary
{
	/// <summary>
	/// Specifies a tracked node that can be queried through the boundary system.
	/// </summary>
	public enum Node
	{
		HandLeft           = OVRPlugin.Node.HandLeft,  ///< Tracks the left hand node.
		HandRight          = OVRPlugin.Node.HandRight, ///< Tracks the right hand node.
		Head               = OVRPlugin.Node.Head,      ///< Tracks the head node.
	}

	/// <summary>
	/// Specifies a boundary type surface.
	/// </summary>
	public enum BoundaryType
	{
		OuterBoundary      = OVRPlugin.BoundaryType.OuterBoundary, ///< Outer boundary that closely matches the user's configured walls.
		PlayArea           = OVRPlugin.BoundaryType.PlayArea,      ///< Smaller convex area inset within the outer boundary.
	}

	/// <summary>
	/// Provides test results of boundary system queries.
	/// </summary>
	public struct BoundaryTestResult
	{
		public bool IsTriggering;                              ///< Returns true if the queried test would violate and/or trigger the tested boundary types.
		public float ClosestDistance;                          ///< Returns the distance between the queried test object and the closest tested boundary type.
		public Vector3 ClosestPoint;                           ///< Returns the closest point to the queried test object.
		public Vector3 ClosestPointNormal;                     ///< Returns the normal of the closest point to the queried test object.
	}

	/// <summary>
	/// Returns true if the boundary system is currently configured with valid boundary data.
	/// </summary>
	public bool GetConfigured()
	{
	// BUG: Container contents are never accessed
	// MESSAGE: A collection or map whose contents are never queried or accessed is useless.
	// 		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus)
	// 			return OVRPlugin.GetBoundaryConfigured();
	// 		else
	// 		{
	// #if !USING_XR_SDK && !REQUIRES_XR_SDK
	// 			return Boundary.configured;
	// #else
	// 			return false;
	// #endif
	// 		}
	// 	}
	// 
	// 	/// <summary>
	// 	/// Returns the results of testing a tracked node against the specified boundary type.
	// 	/// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
	// 	/// </summary>
	// 	public OVRBoundary.BoundaryTestResult TestNode(OVRBoundary.Node node, OVRBoundary.BoundaryType boundaryType)
	// 	{
	// 		OVRPlugin.BoundaryTestResult ovrpRes = OVRPlugin.TestBoundaryNode((OVRPlugin.Node)node, (OVRPlugin.BoundaryType)boundaryType);
	// 
	// 		OVRBoundary.BoundaryTestResult res = new OVRBoundary.BoundaryTestResult()
	// 		{
	// 			IsTriggering = (ovrpRes.IsTriggering == OVRPlugin.Bool.True),
	// 			ClosestDistance = ovrpRes.ClosestDistance,
	// 			ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
	// 			ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
	// 		};
	// 
	// 		return res;
	// 	}
	// 
	// 	/// <summary>
	// 	/// Returns the results of testing a 3d point against the specified boundary type.
	// 	/// The test point is expected in local tracking space.
	// 	/// All points are returned in local tracking space shared by tracked nodes and accessible through OVRCameraRig's trackingSpace anchor.
	// 	/// </summary>
	// 	public OVRBoundary.BoundaryTestResult TestPoint(Vector3 point, OVRBoundary.BoundaryType boundaryType)
	// 	{
	// 		OVRPlugin.BoundaryTestResult ovrpRes = OVRPlugin.TestBoundaryPoint(point.ToFlippedZVector3f(), (OVRPlugin.BoundaryType)boundaryType);
	// 
	// 		OVRBoundary.BoundaryTestResult res = new OVRBoundary.BoundaryTestResult()
	// 		{
	// 			IsTriggering = (ovrpRes.IsTriggering == OVRPlugin.Bool.True),
	// 			ClosestDistance = ovrpRes.ClosestDistance,
	// 			ClosestPoint = ovrpRes.ClosestPoint.FromFlippedZVector3f(),
	// 			ClosestPointNormal = ovrpRes.ClosestPointNormal.FromFlippedZVector3f(),
	// 		};
	// 
	// 		return res;
	// 	}
	// 
	// 	private static int cachedVector3fSize = Marshal.SizeOf(typeof(OVRPlugin.Vector3f));
	// 	private static OVRNativeBuffer cachedGeometryNativeBuffer = new OVRNativeBuffer(0);
	// 	private static float[] cachedGeometryManagedBuffer = new float[0];

	// FIXED VERSION:
