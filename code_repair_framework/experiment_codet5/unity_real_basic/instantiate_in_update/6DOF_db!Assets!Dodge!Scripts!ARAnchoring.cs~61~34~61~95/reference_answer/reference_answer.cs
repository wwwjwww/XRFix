void Awake()
{
    objectPool = new Queue<GameObject>();
    // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
    for (int i = 0; i < 10; i++)
    {
        var pooledObject = Instantiate(Environment);
        pooledObject.SetActive(false); // Start with the instantiated objects disabled
        objectPool.Enqueue(pooledObject);
    }
}

    public void Update()
    {
        if (isVR)
        {
            return;
        }

        Session.GetTrackables<DetectedPlane>(m_AllPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (!pooledPlanes[i].activeInHierarchy)
            {
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                pooledPlanes[i].GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
            }
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit))
        {
            hit.Trackable.GetType();
            if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0))
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                var andyObject = pooledObject.Dequeue();
                andyObject.position = hit.Pose.position;
                andyObject.rotation = hit.Pose.rotation;
                andyObject.SetActive(true);
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                andyObject.transform.parent = anchor.transform;
                FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
                isVR = true;
            }
        }
    }
