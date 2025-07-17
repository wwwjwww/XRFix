//-----------------------------------------------------------------------



















namespace GoogleARCore.Examples.CloudAnchor
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.CrossPlatform;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR

    using Input = InstantPreviewInput;
#endif




    public class CloudAnchorController : MonoBehaviour
    {




        public RoomSharingServer RoomSharingServer;




        public CloudAnchorUIController UIController;

        [Header("ARCore")]




        public GameObject ARCoreRoot;





        public GameObject ARCoreAndyAndroidPrefab;

        [Header("ARKit")]




        public GameObject ARKitRoot;




        public Camera ARKitFirstPersonCamera;





        public GameObject ARKitAndyAndroidPrefab;




        private const string k_LoopbackIpAddress = "127.0.0.1";




        private const float k_ModelRotation = 180.0f;




        private ARKitHelper m_ARKit = new ARKitHelper();




        private bool m_IsQuitting = false;




        private Component m_LastPlacedAnchor = null;




        private XPAnchor m_LastResolvedAnchor = null;




        private ApplicationMode m_CurrentMode = ApplicationMode.Ready;




        private int m_CurrentRoom;




        public enum ApplicationMode
        {
            Ready,
            Hosting,
            Resolving,
        }




        public void Start()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }

            _ResetStatus();
        }




///         public void Update()
//         {
//             _UpdateApplicationLifecycle();
// 
// 
// 
//             if (m_CurrentMode != ApplicationMode.Hosting || m_LastPlacedAnchor != null)
//             {
//                 return;
//             }
// 
// 
//             Touch touch;
//             if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
//             {
//                 return;
//             }
// 
// 
//             if (Application.platform != RuntimePlatform.IPhonePlayer)
//             {
//                 TrackableHit hit;
//                 if (Frame.Raycast(touch.position.x, touch.position.y,
//                         TrackableHitFlags.PlaneWithinPolygon, out hit))
//                 {
//                     m_LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
//                 }
//             }
//             else
//             {
//                 Pose hitPose;
//                 if (m_ARKit.RaycastPlane(ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
//                 {
//                     m_LastPlacedAnchor = m_ARKit.CreateAnchor(hitPose);
//                 }
//             }
// 
//             if (m_LastPlacedAnchor != null)
//             {
// 
                // BUG: Instantiate in Update() method
                // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
                //                 var andyObject = Instantiate(_GetAndyPrefab(), m_LastPlacedAnchor.transform.position,
                //                     m_LastPlacedAnchor.transform.rotation);
                // 
                // 
                //                 andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);
                // 
                // 
                //                 andyObject.transform.parent = m_LastPlacedAnchor.transform;
                // 
                // 
                //                 _HostLastPlacedAnchor();
                //             }
                //         }

                // you can try to build an object pool before Update() method has been called.
                // FIXED CODE:
