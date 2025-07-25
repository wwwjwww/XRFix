﻿// Chnaged DetectedPlaneVisualizer so that planes aren't visable when VR turns on



using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.XR;

public class ARAnchoring : MonoBehaviour
{
    public Camera FirstPersonCamera;
    public GameObject Environment;
    public GameObject DetectedPlanePrefab;
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
    public static bool isVR = false;

    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;

        // Build object pool
        for (int i = 0; i < 10; i++) // Adjust pool size as needed
        {
            GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            planeObject.SetActive(false);
            m_PooledPlanes.Add(planeObject);
        }
    }
    
///     public void Update()
//     {
//         if (isVR)
//         {
//             return;
//         }
// 
//         Session.GetTrackables<DetectedPlane>(m_AllPlanes);
//         for (int i = 0; i < m_AllPlanes.Count; i++)
//         {
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
            //             planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
            //         }
            // 
            //         Touch touch;
            //         if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            //         {
            //             return;
            //         }
            // 
            //         TrackableHit hit;
            //         TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
            // 
            //         if (Frame.Raycast(Screen.width*0.5f, Screen.height*0.5f, raycastFilter, out hit))
            //         {
            // 
            // 
            //             hit.Trackable.GetType();
            //             if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0 ))
            //             {
            //                 Debug.Log("Hit at back of the current DetectedPlane");
            //             }
            //             else
            //             {
            //                 var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);
            //                 var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            //                 andyObject.transform.parent = anchor.transform;
            //                 FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
            //                 isVR = true;
            // 
            //             }
            //         }
            //     }

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private List<GameObject> m_PooledPlanes = new List<GameObject>();

    public void Update()
    {
        if (isVR)
        {
            return;
        }

        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
        int planeCount = Mathf.Min(m_AllPlanes.Count, m_PooledPlanes.Count);
        for (int i = 0; i < planeCount; i++)
        {
            GameObject planeObject = m_PooledPlanes[i];
            if (!planeObject.activeInHierarchy)
            {
                planeObject.SetActive(true);
                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
            }
        }

        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit))
        {
            if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0))
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                andyObject.transform.parent = anchor.transform;
                FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
                isVR = true;
            }
        }
    }



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