    void FixedUpdate()
    {
        debug.text = controler.transform.rotation.x.ToString() +
                     "\n" + controler.transform.rotation.y.ToString() +
                     "\n" + controler.transform.rotation.z.ToString();
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            if (!hasArrow)
            {
                AttachArrow();
            } else if (isAttached) {
                Fire();
            }
        }
        else ThrowArrow();
    }
