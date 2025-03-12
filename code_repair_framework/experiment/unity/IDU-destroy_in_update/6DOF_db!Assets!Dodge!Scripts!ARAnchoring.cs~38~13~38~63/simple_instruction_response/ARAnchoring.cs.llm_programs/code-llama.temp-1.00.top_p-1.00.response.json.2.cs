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
    }
    

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



        private static int s_PlaneCount = 0;

        private readonly Color[] k_PlaneColors = new Color[]
        {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };

        public void Awake()
        {
            m_Mesh = GetComponent<MeshFilter>().mesh;
            m_MeshRenderer = GetComponent<UnityEngine.MeshRenderer>();
        }

        public void Initialize(DetectedPlane detectedPlane)
        {
            m_DetectedPlane = detectedPlane;
            m_PlaneCenter = m_DetectedPlane.CenterPose.position;

            // Added a check for the trackable is DetectedPlane, if it's not a DetectedPlane, skip the function call.
            if ((detectedPlane is DetectedPlane)){

              UpdateMeshVertices();
              UpdateMeshHeinds();
              UpdateMeshColors();
              m_Mesh.SetVertices(m_MeshVertices);
              m_Mesh.SetIndices(m_MeshIndices.ToArray(), MeshTopology.Triangles, 0);
              m_Mesh.SetColors(m_MeshColors);

              m_MeshRenderer.sharedMaterial = m_SharedMaterial;
              }

        }

        public void UpdateMeshVertices()
        {
            m_MeshVertices.Clear();

            float planeExtents = 0.006f;

            Vector3 normal = (Camera.main.transform.position - m_PlaneCenter).normalized;

            Vector3 right = Vector3.Cross(normal, Vector3.up);
            right.Normalize();

            Vector3 up = Vector3.Cross(normal, right);
            up.Normalize();

            // Set up "seeds" for the mesh vertices - these are the center points of the
            // axis-aligned bounding rectangle for the plane, stretched to fit the extents
            // of the plane.
            Vector3 leftBottom = m_PlaneCenter - right * planeExtents - up * planeExtents;
            Vector3 leftTop = m_PlaneCenter - right * planeExtents + up * planeExtents;
            Vector3 rightBottom = m_PlaneCenter + right * planeExtents - up * planeExtents;
            Vector3 rightTop = m_PlaneCenter + right * planeExtents + up * planeExtents;

            // These vertices will make up the mesh for the plane, arranged in thse order
            // to form quadrilaterals.
            m_MeshVertices.Add(leftBottom);
            m_MeshVertices.Add(leftTop);
            m_MeshVertices.Add(rightTop);
            m_MeshVertices.Add(rightBottom);
        }

        public void UpdateMeshHeinds()
        {
            m_MeshIndices.Clear();

            // The mesh for our plane will consist of two quadrilaterals.
            // The triangles are assembled in the same order as the vertices.
            m_MeshIndices.Add(0);
            m_MeshIndices.Add(1);
            m_MeshIndices.Add(2);
            m_MeshIndices.Add(0);
            m_MeshIndices.Add(2);
            m_MeshIndices.Add(3);
        }

        public void UpdateMeshColors()
        {
            if (s_PlaneCount < k_PlaneColors.Length)
            {
                m_MeshColors.Add(k_PlaneColors[s_PlaneCount % k_PlaneColors.Length]);
                ++s_PlaneCount;
            }
            else
            {
                m_MeshColors.Add(k_PlaneColors[0]);
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