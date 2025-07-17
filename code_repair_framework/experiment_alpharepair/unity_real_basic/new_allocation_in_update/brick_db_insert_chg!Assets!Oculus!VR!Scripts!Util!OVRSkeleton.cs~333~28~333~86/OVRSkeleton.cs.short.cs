using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 	private void Update()
// 	{
// #if UNITY_EDITOR
// 		if (OVRInput.IsControllerConnected(OVRInput.Controller.Hands) && !IsInitialized)
// 		{
// 			if (_skeletonType != SkeletonType.None)
// 			{
// 				Initialize();
// 			}
// 		}
// 
// 		if (!ShouldUpdateBonePoses)
// 		{
// 			return;
// 		}
// #endif
// 
// 		if (!IsInitialized || _dataProvider == null)
// 		{
// 			IsDataValid = false;
// 			IsDataHighConfidence = false;
// 
// 			return;
// 		}
// 
// 		var data = _dataProvider.GetSkeletonPoseData();
// 
// 		IsDataValid = data.IsDataValid;
// 		if (data.IsDataValid)
// 		{
// 			IsDataHighConfidence = data.IsDataHighConfidence;
// 
// 			if (_updateRootPose)
// 			{
// 				transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
// 				transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
// 			}
// 
// 			if (_updateRootScale)
// 			{
				// BUG: Using New() allocation in Update() method.
				// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
				// 				transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
				// 			}
				// 
				// 			for (var i = 0; i < _bones.Count; ++i)
				// 			{
				// 				if (_bones[i].Transform != null)
				// 				{
				// 					_bones[i].Transform.localRotation = data.BoneRotations[i].FromFlippedXQuatf();
				// 					if (_bones[i].Id == BoneId.Hand_WristRoot)
				// 					{
				// 						_bones[i].Transform.localRotation *= wristFixupRotation;
				// 					}
				// 				}
				// 			}
				// 		}
				// 	}

				// Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
				// FIXED CODE:
