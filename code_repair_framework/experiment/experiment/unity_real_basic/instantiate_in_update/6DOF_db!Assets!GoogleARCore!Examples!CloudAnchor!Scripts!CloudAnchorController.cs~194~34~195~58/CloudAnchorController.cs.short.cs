
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

                // FIXED CODE:
