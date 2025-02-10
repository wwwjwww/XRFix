using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using UnityEngine.Rendering.Universal;

///     public override void Update(GameObject gameObject, Camera mainCamera,
//         OVRMixedRealityCaptureConfiguration configuration, OVRManager.TrackingOrigin trackingOrigin)
//     {
// #if OVR_ANDROID_MRC
//         if (skipFrame && OVRPlugin.Media.IsCastingToRemoteClient())
//         {
//             isFrameSkipped = !isFrameSkipped;
//             if (isFrameSkipped)
//             {
//                 return;
//             }
//         }
// #endif
// 
        //         RefreshCameraObjects(gameObject, mainCamera, configuration);
        // 
        //         
        //         OVRPlugin.SetHandNodePoseStateLatency(0.0);
        // 
        //         
        //         OVRPose stageToLocalPose =
        //             OVRPlugin.GetTrackingTransformRelativePose(OVRPlugin.TrackingOrigin.Stage).ToOVRPose();
        //         OVRPose localToStagePose = stageToLocalPose.Inverse();
        //         OVRPose head = localToStagePose * OVRPlugin.GetNodePose(OVRPlugin.Node.Head, OVRPlugin.Step.Render).ToOVRPose();
        //         OVRPose leftC = localToStagePose *
        //                         OVRPlugin.GetNodePose(OVRPlugin.Node.HandLeft, OVRPlugin.Step.Render).ToOVRPose();
        //         OVRPose rightC = localToStagePose *
        //                          OVRPlugin.GetNodePose(OVRPlugin.Node.HandRight, OVRPlugin.Step.Render).ToOVRPose();
        //         OVRPlugin.Media.SetMrcHeadsetControllerPose(head.ToPosef(), leftC.ToPosef(), rightC.ToPosef());
        // 
        // #if OVR_ANDROID_MRC
        //         RefreshAudioFilter(mainCamera);
        // 
        //         int drawTextureIndex = (frameIndex / 2) % 2;
        //         int castTextureIndex = 1 - drawTextureIndex;
        // 
        //         backgroundCamera.enabled = (frameIndex % 2) == 0;
        //         foregroundCamera.enabled = (frameIndex % 2) == 1;
        // 
        //         if (frameIndex % 2 == 0)
        //         {
        //             if (lastMrcEncodeFrameSyncId != -1)
        //             {
        //                 OVRPlugin.Media.SyncMrcFrame(lastMrcEncodeFrameSyncId);
        //                 lastMrcEncodeFrameSyncId = -1;
        //             }
        // 
        //             lastMrcEncodeFrameSyncId = CastMrcFrame(castTextureIndex);
        //             SetCameraTargetTexture(drawTextureIndex);
        //         }
        // 
        //         ++frameIndex;
        // #endif
        // 
        //         backgroundCamera.clearFlags = mainCamera.clearFlags;
        //         backgroundCamera.backgroundColor = mainCamera.backgroundColor;
        //         if (configuration.dynamicCullingMask)
        //         {
        //             backgroundCamera.cullingMask = (mainCamera.cullingMask & ~configuration.extraHiddenLayers) |
        //                                            configuration.extraVisibleLayers;
        //         }
        // 
        //         backgroundCamera.nearClipPlane = mainCamera.nearClipPlane;
        //         backgroundCamera.farClipPlane = mainCamera.farClipPlane;
        // 
        //         if (configuration.dynamicCullingMask)
        //         {
        //             foregroundCamera.cullingMask = (mainCamera.cullingMask & ~configuration.extraHiddenLayers) |
        //                                            configuration.extraVisibleLayers;
        //         }
        // 
        //         foregroundCamera.nearClipPlane = mainCamera.nearClipPlane;
        //         foregroundCamera.farClipPlane = mainCamera.farClipPlane;
        // 
        //         if (OVRMixedReality.useFakeExternalCamera || OVRPlugin.GetExternalCameraCount() == 0)
        //         {
        //             OVRPose worldSpacePose = new OVRPose();
        //             OVRPose trackingSpacePose = new OVRPose();
        //             trackingSpacePose.position = trackingOrigin == OVRManager.TrackingOrigin.EyeLevel
        //                 ? OVRMixedReality.fakeCameraEyeLevelPosition
        //                 : OVRMixedReality.fakeCameraFloorLevelPosition;
        //             trackingSpacePose.orientation = OVRMixedReality.fakeCameraRotation;
        //             worldSpacePose = OVRExtensions.ToWorldSpacePose(trackingSpacePose, mainCamera);
        // 
        //             backgroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
        //             backgroundCamera.aspect = OVRMixedReality.fakeCameraAspect;
        //             foregroundCamera.fieldOfView = OVRMixedReality.fakeCameraFov;
        //             foregroundCamera.aspect = OVRMixedReality.fakeCameraAspect;
        // 
        //             if (cameraInTrackingSpace)
        //             {
        //                 backgroundCamera.transform.FromOVRPose(trackingSpacePose, true);
        //                 foregroundCamera.transform.FromOVRPose(trackingSpacePose, true);
        //             }
        //             else
        //             {
        //                 backgroundCamera.transform.FromOVRPose(worldSpacePose);
        //                 foregroundCamera.transform.FromOVRPose(worldSpacePose);
        //             }
        //         }
        //         else
        //         {
        //             OVRPlugin.CameraExtrinsics extrinsics;
        //             OVRPlugin.CameraIntrinsics intrinsics;
        // 
        //             
        //             if (OVRPlugin.GetMixedRealityCameraInfo(0, out extrinsics, out intrinsics))
        //             {
        //                 float fovY = Mathf.Atan(intrinsics.FOVPort.UpTan) * Mathf.Rad2Deg * 2;
        //                 float aspect = intrinsics.FOVPort.LeftTan / intrinsics.FOVPort.UpTan;
        //                 backgroundCamera.fieldOfView = fovY;
        //                 backgroundCamera.aspect = aspect;
        //                 foregroundCamera.fieldOfView = fovY;
        //                 foregroundCamera.aspect = intrinsics.FOVPort.LeftTan / intrinsics.FOVPort.UpTan;
        // 
        //                 if (cameraInTrackingSpace)
        //                 {
        //                     OVRPose trackingSpacePose = ComputeCameraTrackingSpacePose(extrinsics);
        //                     backgroundCamera.transform.FromOVRPose(trackingSpacePose, true);
        //                     foregroundCamera.transform.FromOVRPose(trackingSpacePose, true);
        //                 }
        //                 else
        //                 {
        //                     OVRPose worldSpacePose = ComputeCameraWorldSpacePose(extrinsics, mainCamera);
        //                     backgroundCamera.transform.FromOVRPose(worldSpacePose);
        //                     foregroundCamera.transform.FromOVRPose(worldSpacePose);
        //                 }
        // #if OVR_ANDROID_MRC
        //                 cameraPoseTimeArray[drawTextureIndex] = extrinsics.LastChangedTimeSeconds;
        // #endif
        //             }
        //             else
        //             {
        //                 Debug.LogError("Failed to get external camera information");
        //                 return;
        //             }
        //         }
        // 
        //         Vector3 headToExternalCameraVec = mainCamera.transform.position - foregroundCamera.transform.position;
        //         float clipDistance = Vector3.Dot(headToExternalCameraVec, foregroundCamera.transform.forward);
        //         foregroundCamera.farClipPlane = Mathf.Max(foregroundCamera.nearClipPlane + 0.001f, clipDistance);
        //     }

        // FIXED CODE:
