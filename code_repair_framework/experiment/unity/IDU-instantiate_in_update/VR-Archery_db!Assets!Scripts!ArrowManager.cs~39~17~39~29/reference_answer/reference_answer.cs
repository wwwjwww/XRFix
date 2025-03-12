    void Awake()
    {
            objectPool = new Queue<GameObject>();
            // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
            for (int i = 0; i < 10; i++)
            {
                var pooledObject = GameObject.Instantiate( arrowPrefab );
                pooledObject.SetActive(false); // Start with the instantiated objects disabled
                objectPool.Enqueue(pooledObject);
            }
    }

    public void AttachArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = objectPool.Dequeue();
            currentArrow.position = controler.transform;
            currentArrow.SetActive(true);
            currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            hasArrow = true;
        }
    }
