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

private void Start()
{
    VRAreaAnchorEnvironment env = environment.GetComponent<VRANchorEnvironment>();
    env.isVR = true;
}
    



    protected void OnStart()
    {
        DestroyAllPlanes();
    }

    private void DestroyAllPlanes()
    {
        if (m_AllPlanes == null)
        {
            return;
        }
        for(int i = 0; i < m_AllPlanes.Count; i++)
        {
            Destroy(m_AllPlanes[i].gameObject);
        }
    }

    private void OnTrackableStatusChanged(ARPlane arPlane, ARTrackableState status)
    {
        if (arPlane == null)
        {
            return;
        }

        // Check if the plane was previously tracked and is no longer being tracked.
        if (m_AllPlanes.Contains(arPlane) && status == ARTrackableState.Stopped)
        {
            m_AllPlanes.Remove(arPlane);
            DetectPlaneAnchor(arPlane);
        }
        else if (status == ARTrackableState.Tracking)
        {
            m_AllPlanes.Add(arPlane);
            DetectPlaneAnchor(arPlane);
        }
    }

    private void DetectPlaneAnchor(ARPlane arPlane)
    {
        if (arPlane == null)
        {
            return;
        }

        // Create a GameObject containing an ARSessionOrigin component.
        var detectedPlaneObject = Instantiate(DetectedPlanePrefab);
        // Set the GameObject's parent to the ARSessionOrigin.
        detectedPlaneObject.transform.parent = FirstPersonCamera.transform;

        // Get the ARPlaneComponent from the GameObject.
        var plane = detectedPlaneObject.GetComponent<ARPlane>();

        // Construct hit test rays.
        var hits = new List<ARRaycastHit>();
        hits.Add(new ARRaycastHit());
        hits[0].pose = arPlane.CenterPose;

        // Create an anchor to determine the physical position and orientation of the plane in the world.
        var anchor = arPlane.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = DetectedPlanePrefab.AddComponent<ARAnchor>();
        }
        // Set the anchor's transform to the hit test ray's pose.
        anchor.transform.localPosition = hits[0].pose.position;
        anchor.transform.localRotation = hits[0].pose.rotation;

        // Save the newly detected plane.
        m_AllPlanes.Add(arPlane);
    }

    private void Update()
    {
        // Update detected planes position.
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            var plane = m_AllPlanes[i];
            if (plane.trackingState == ARPlane.TrackingState.Tracking;
                plane.alignment = ARPlaneAlignment.Vertical;
            }
        }

    public VRAnchorEnvironment environment;

    public FPAnchorEnvironment environment;



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