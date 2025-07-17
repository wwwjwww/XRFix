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


From your question, it seems like you are trying to optimize the Update() method in your ARAnchoring script. You want to eliminate usage of Destroy() inside the Update() method, which is inefficient because Destroy() causes a frame of delay. A solution to this problem is to use an object pool to manage the lifecycle of DetectedPlanePrefab instances. 

However, without the full context of your application (like what you're trying to achieve by using ARAnchoring and the actual issue you're facing), it's hard to provide a specific solution. But here's a general idea of how you can start optimizing your Update() method:

1. **Instantiate the plane object only once**: Instead of instantiating a new plane object in the Update() method every frame, you could instantiate it once when your application starts and keep reusing this object. This reduces the number of calls to Instantiate().

2. **Pull out expensive calculations into Start() or Awake()**: As you are setting the `Screen.sleepTimeout` in the Start() method, you might want to move this line to Awake() or the Start method of the GameObject (which is attached to the ARAnchoring script).

3. **Use the FixedUpdate() method instead of Update()**: FixedUpdate() has a fixed time between frames and is better for physics-related updates. If your application doesn't use any physics, you could consider using FixedUpdate().

4. **Object Pooling**: As you mentioned, you can use an object pool to manage the lifecycle of DetectedPlanePrefab instances. You create a pool of these objects that are reused every time a new plane is detected.

Remember, these are general tips and might not be applicable for your specific situation. Always analyze your application and decide which optimizations are necessary based on your specific needs.

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