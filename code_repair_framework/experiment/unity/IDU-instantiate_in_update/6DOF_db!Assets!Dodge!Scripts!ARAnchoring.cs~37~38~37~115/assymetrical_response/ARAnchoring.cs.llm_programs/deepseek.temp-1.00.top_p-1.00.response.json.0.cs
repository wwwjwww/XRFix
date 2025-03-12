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
        isVR = XRSettings.loadedDeviceName == "VR"; 
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;
    }
    



    public void Update()
    {
        if (isVR)
        {
            Environment.SetActive(false);
        }
        else
        {
            Environment.SetActive(true);
       			Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
        }
    }

int userAge = Convert.ToInt32(Console.ReadLine());

int age = Convert.ToInt32(Console.ReadLine());

string message = (age >= 18) ? "You are eligible to vote" : "You are not eligible to vote";



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