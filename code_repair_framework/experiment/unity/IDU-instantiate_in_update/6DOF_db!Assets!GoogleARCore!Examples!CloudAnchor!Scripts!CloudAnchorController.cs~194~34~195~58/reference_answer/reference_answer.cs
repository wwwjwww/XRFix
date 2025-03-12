void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        var pooledObject = Instantiate(_GetAndyPrefab());
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

         public void Update()
         {
             _UpdateApplicationLifecycle();



             if (m_CurrentMode != ApplicationMode.Hosting || m_LastPlacedAnchor != null)
             {
                 return;
             }


             Touch touch;
             if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
             {
                 return;
             }


             if (Application.platform != RuntimePlatform.IPhonePlayer)
             {
                 TrackableHit hit;
                 if (Frame.Raycast(touch.position.x, touch.position.y,
                         TrackableHitFlags.PlaneWithinPolygon, out hit))
                 {
                     m_LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                 }
             }
             else
             {
                 Pose hitPose;
                 if (m_ARKit.RaycastPlane(ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                 {
                     m_LastPlacedAnchor = m_ARKit.CreateAnchor(hitPose);
                 }
             }

             if (m_LastPlacedAnchor != null)
             {
                 var andyObject = objectPool.Dequeue();
                 andyObject.position = m_LastPlacedAnchor.transform.position;
                 andyObject.rotation = m_LastPlacedAnchor.transform.rotation;
                 andyObject.SetActive(true);

                 andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                 andyObject.transform.parent = m_LastPlacedAnchor.transform;

                 _HostLastPlacedAnchor();
             }
         }
