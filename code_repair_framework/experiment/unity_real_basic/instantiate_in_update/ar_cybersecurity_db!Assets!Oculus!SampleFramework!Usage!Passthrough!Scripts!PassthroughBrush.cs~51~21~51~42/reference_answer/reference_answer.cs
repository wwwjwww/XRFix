    void Awake()
    {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Instantiate(lineSegmentPrefab);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
    }

    void StartLine(Vector3 inkPos)
    {
        var newLine = objectPool.Dequeue();
        newLine.position = inkPos;
        newLine.rotation = Quaternion.identity;
        newLine.SetActive(true);
        currentLineSegment = newLine.GetComponent<LineRenderer>();
        currentLineSegment.positionCount = 1;
        currentLineSegment.SetPosition(0, inkPos);
        strokeWidth = currentLineSegment.startWidth;
        strokeLength = 0.0f;
        inkPositions.Clear();
        inkPositions.Add(inkPos);
        newLine.transform.parent = lineContainer.transform;
    }
