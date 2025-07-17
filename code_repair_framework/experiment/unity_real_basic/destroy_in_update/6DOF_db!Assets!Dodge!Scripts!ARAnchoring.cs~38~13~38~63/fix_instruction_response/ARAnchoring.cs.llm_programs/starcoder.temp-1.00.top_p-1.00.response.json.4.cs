// BUG: Destroy in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
//    public void Update()
//    {
//        if (isVR)
//        {
//            return;
//        }
//
//        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
//        for (int i = 0; i < m_AllPlanes.Count; i++)
//        {
//            GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
//            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
//        }
//
//        Touch touch;
//        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
//        {
//            return;
//        }
//
//        TrackableHit hit;
//        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
//
//        if (Frame.Raycast(Screen.width*0.5f, Screen.height*0.5f, raycastFilter, out hit))
//        {
//
//
//            hit.Trackable.GetType();
//            if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0 ))
//            {
//                Debug.Log("Hit at back of the current DetectedPlane");
//            }
//            else
//            {
//                var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);
//                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
//                andyObject.transform.parent = anchor.transform;
//                FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
//                isVR = true;
//
//            }
//        }
//    }


La principal diferencia entre una impresora 3D DIY (Do It Yourself) y una impresora 3D profesional es el costo. Las impresoras 3D DIY suelen ser más asequibles y accesibles para aquellos que quieren experimentar con la impresión 3D. Además, los materiales de construcción de las impresoras 3D DIY suelen ser más fáciles de conseguir y la configuración de la impresora es más fácil de realizar. Por otro lado, las impresoras 3D profesionales suelen ser más complejas y requieren más tiempo y esfuerzo para su configuración y uso. Además,

    IEnumerator SwitchToVR()
    {
        string desiredDevice = "cardboard";
        if (string.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {
            _ShowAndroidToastMessage("LOL");
            XRSettings.LoadDeviceByName(desiredDevice);
            _ShowAndroidToastMessage(XRSettings.loadedDeviceName);
            yield return null;
        }
        XRSettings.enabled = true;
    }

    //Function for Debugging 
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

}