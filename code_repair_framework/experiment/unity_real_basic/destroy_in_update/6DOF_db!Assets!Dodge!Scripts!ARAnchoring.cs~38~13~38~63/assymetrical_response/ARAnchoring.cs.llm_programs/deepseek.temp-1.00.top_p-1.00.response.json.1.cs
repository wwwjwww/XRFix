/* BUG: Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
*     public void Update()
*     {
*         if (isVR)
*         {
*             return;
*         }
* 
*         Session.GetTrackables<DetectedPlane>(m_AllPlanes);
*         for (int i = 0; i < m_AllPlanes.Count; i++)
*         {
*             GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
*             planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
*         }
* 
*         Touch touch;
*         if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
*         {
*             return;
*         }
* 
*         TrackableHit hit;
*         TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
* 
*         if (Frame.Raycast(Screen.width*0.5f, Screen.height*0.5f, raycastFilter, out hit))
*         {
* 
* 
*             hit.Trackable.GetType();
*             if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0 ))
*             {
*                 Debug.Log("Hit at back of the current DetectedPlane");
*             }
*             else
*             {
*                 var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);
*                 var anchor = hit.Trackable.CreateAnchor(hit.Pose);
*                 andyObject.transform.parent = anchor.transform;
*                 FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
*                 isVR = true;
* 
*             }
*         }
*     }


If you are destroying the GameObjects in Update() method, there are several ways to improve the efficiency of your code:

1. Use Object Pooling: You can pool game objects before entering Update(). When a plane is detected, check if there is an available object in the pool. If there isn't, you can instantiate a new one. This way, you avoid costly instantiation/destruction operations in Update().

2. Use Coroutines: Instead of running your code directly in Update(), you can use Coroutines. Coroutines allow you to pause the execution of a function. It can make your code more readable and easier to manage. Once the VR mode is activated, you can wait until the next frame to destroy the planes, instead of destroying them immediately. Here is a [link](https://docs.unity3d.com/Manual/Coroutines.html) to the Unity documentations about coroutines.

3. Modify Update(): Instead of destroying planes in Update(), you can move them to a different list or deactivate them. Then, in a separate method or at a different time, destroy them. This can be more efficient considering the cost of destroying GameObjects. The only inconvenience is that you access the objects during the update, so you need to ensure that this does not cause problems.

4. Use a GameObject Pool: Unity has a component called 'Object Pooler' which you can use to pool your game objects (planes in your case). It can be found in the unity asset store.

I would suggest using the first three methods because they are simpler and they should provide a noticeable performance improvement. It's always a good idea to test and profile your code under different conditions to see what works best in your specific scenario.

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