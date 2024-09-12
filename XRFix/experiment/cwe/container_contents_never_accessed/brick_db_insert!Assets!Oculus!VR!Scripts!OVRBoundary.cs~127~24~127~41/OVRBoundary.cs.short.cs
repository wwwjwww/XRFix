using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Boundary = UnityEngine.Experimental.XR.Boundary;

	// BUG: Container contents are never accessed
	// MESSAGE: A collection or map whose contents are never queried or accessed is useless.
	// 	private List<Vector3> cachedGeometryList = new List<Vector3>();

	// FIXED CODE:
