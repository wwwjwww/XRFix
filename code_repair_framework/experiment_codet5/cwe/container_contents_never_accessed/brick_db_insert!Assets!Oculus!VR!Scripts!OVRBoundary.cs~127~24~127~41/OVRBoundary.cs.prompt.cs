/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Master SDK License Version 1.0 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
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
