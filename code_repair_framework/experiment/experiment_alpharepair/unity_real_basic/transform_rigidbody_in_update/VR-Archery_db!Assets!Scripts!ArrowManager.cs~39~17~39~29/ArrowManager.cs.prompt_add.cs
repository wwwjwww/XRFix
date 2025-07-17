//    public void AttachArrow()
//    {
//        if (currentArrow == null)
//        {
//            currentArrow = Instantiate(arrowPrefab, controler.transform);
//            currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
//            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
//            hasArrow = true;
//        }
//    }

// FIXED CODE:

// Move this function in FixedUpdate() methods.