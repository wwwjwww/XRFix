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
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;
        // Initialize the detection of planes every frame
        StartCoroutine(DetectAndInstantiatePlanes());
    }
    



    void Update()
    {
        if(isVR)
        {
            // Set planes to deactivate if VR is enabled
            foreach (Transform t in DetectedPlanePrefab.transform)
            {
                t.gameObject.SetActive(false);
           
        }
    }

    private IEnumerator DetectAndInstantiatePlanes()
    {
        while (true)
        {
            if (Session.Status != SessionStatus.Tracking)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Get the updated plane list
            Session.GetAllPlanes(m_AllPlanes);
            // If we have multiple planes, find the one that is horizontal
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].Type == DetectedPlaneType.HorizontalUp)
                {
                    Vector3 pos = m_AllPlanes[i].CenterPose.position;
                    Quaternion rot = m_AllPlanes[i].CenterPose.rotation;
                    // Update the anchor transformation
                    DetectedPlane prefab = Instantiate(DetectedPlanePrefab, pos, rot);
                    prefab.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(5f);
            }
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