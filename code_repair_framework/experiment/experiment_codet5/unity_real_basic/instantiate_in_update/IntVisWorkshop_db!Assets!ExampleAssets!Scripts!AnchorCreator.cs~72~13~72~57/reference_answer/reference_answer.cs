     void Awake()
     {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = Instantiate(m_AnchorPrefab);
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
     }

     void Update()
     {

         if (Input.touchCount == 0)
             return;

         var touch = Input.GetTouch(0);
         if (touch.phase != TouchPhase.Began)
             return;

         if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
         {


             var hitPose = s_Hits[0].pose;
             var hitTrackableId = s_Hits[0].trackableId;
             var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);




             var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
             var obj = objectPool.Dequeue();
             obj.position = anchor.transform;
             obj.SetActive(false);

             if (anchor == null)
             {
                 Debug.Log("Error creating anchor.");
             }
             else
             {

                 m_AnchorPoints.Add(anchor);
             }
         }
     }
