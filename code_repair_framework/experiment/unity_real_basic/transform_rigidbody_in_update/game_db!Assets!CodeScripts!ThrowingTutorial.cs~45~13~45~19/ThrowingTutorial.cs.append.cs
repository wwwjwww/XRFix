
    private void Throw()
    {
        readyToThrow = false;

        if (throwCounter < totalThrows)
        {
            audioManager.PlaySFX(audioManager.axeSound);
            // instantiate object to throw
            GameObject thrownObject = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
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

        // implement throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    // This method is called when the player enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) 
        {
            inCollider = true;
        }
    }

    // This method is called when the player leaves the collider
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) 
        {
            inCollider = false;
        }
    }
}