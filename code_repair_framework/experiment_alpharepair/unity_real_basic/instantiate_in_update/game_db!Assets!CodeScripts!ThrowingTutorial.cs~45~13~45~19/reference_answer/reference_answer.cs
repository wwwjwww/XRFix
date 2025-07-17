    void Awake()
     {
        objectPool = new Queue<GameObject>();
        // Pre-instantiate GameObjects (the pool size is arbitrary; choose a reasonable number based on expected use)
        for (int i = 0; i < 10; i++)
        {
            var pooledObject = Instantiate(objectToThrow);
            pooledObject.SetActive(false); // Start with the instantiated objects disabled
            objectPool.Enqueue(pooledObject);
        }
     }

    private void Throw()
    {
        readyToThrow = false;

        if (throwCounter < totalThrows)
        {
            audioManager.PlaySFX(audioManager.axeSound);
            var thrownObject = objectPool.Dequeue();
            thrownObject.position = attackPoint.position;
            thrownObject.rotation = attackPoint.rotation;
            thrownObject.SetActive(true);

            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
            throwCounter++;
        }
        else if (throwCounter == totalThrows)
        {
            audioManager.PlaySFX(audioManager.disappearSound);
            armorObject.SetActive(false); // Make the armor disappear
            audioManager.PlaySFX(audioManager.appearSound);
            swordObject.SetActive(true); // Make the sword appear
        }

        totalThrows--;


        Invoke(nameof(ResetThrow), throwCooldown);
    }
