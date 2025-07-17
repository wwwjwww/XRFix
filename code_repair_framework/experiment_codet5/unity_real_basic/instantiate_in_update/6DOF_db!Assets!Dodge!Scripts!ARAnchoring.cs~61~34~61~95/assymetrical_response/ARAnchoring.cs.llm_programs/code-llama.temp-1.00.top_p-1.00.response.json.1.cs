// Chnaged DetectedPlaneVisualizer so that planes aren't visable when VR turns on



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
        ﻿Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ﻿FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;
    }
    



    public void LateUpdate()
    {
        ﻿m_AllPlanes.Clear();
        ﻿ARCoreSession.GetTrackables<DetectedPlane>(m_AllPlanes);
        ﻿for (int i = 0; i < m_AllPlanes.Count; i++)
        ﻿{
        ﻿    DetectedPlane detectedPlane = m_AllPlanes[i];
        ﻿    Debug.Log("DetectedPlane Index: " + i + " Pose: " + detectedPlane.CenterPose);
        ﻿    GameObject planeObject = Instantiate(DetectedPlanePrefab, detectedPlane.CenterPose.position, detectedPlane.CenterPose.rotation);
        ﻿    planeObject.name = "ARPlane" + i.ToString();
        ﻿}
    }

    public void LateUpdate()
    {
        ﻿m_AllPlanes.Clear();
        ﻿ARCoreSession.GetTrackables<DetectedPlane>(m_AllPlanes);
        ﻿for (int i = 0; i < m_AllPlanes.Count; i++)
        ﻿{
        ﻿    DetectedPlane detectedPlane = m_AllPlanes[i];
        ﻿    Debug.Log("DetectedPlane Index: " + i + " Pose: " + detectedPlane.CenterPose);
        ﻿    GameObject planeObject = Instantiate(DetectedPlanePrefab, detectedPlane.CenterPose.position, detectedPlane.CenterPose.rotation);
        ﻿    planeObject.name = "ARPlane" + i.ToString();
        ﻿}
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