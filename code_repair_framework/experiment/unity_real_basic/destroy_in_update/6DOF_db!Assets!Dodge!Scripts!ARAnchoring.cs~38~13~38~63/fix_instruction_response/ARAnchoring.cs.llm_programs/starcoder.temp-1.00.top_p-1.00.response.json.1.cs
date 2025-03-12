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



    private static ObjectPool m_Instance;

    [System.Serializable]
    public class ObjectInfo
    {
        public string Name;
        public Transform Prefab;
        public int MaxAmount;
    }

    [SerializeField]
    private List<ObjectInfo> m_ObjectsToPool = new List<ObjectInfo>();

    private Dictionary<string, Queue<Transform>> m_PoolDictionary = new Dictionary<string, Queue<Transform>>();

    private void Awake()
    {
        foreach (ObjectInfo objInfo in m_ObjectsToPool)
        {
            CreateObjectPool(objInfo);
        }
    }

    private void CreateObjectPool(ObjectInfo objInfo)
    {
        Queue<Transform> objectQueue = new Queue<Transform>();

        for (int i = 0; i < objInfo.MaxAmount; i++)
        {
            Transform newObject = Instantiate(objInfo.Prefab);
            newObject.SetActive(false);
            objectQueue.Enqueue(newObject);
        }

        m_PoolDictionary.Add(objInfo.Name, objectQueue);
    }

    public Transform GetObject(string objectName)
    {
        if (!m_PoolDictionary.ContainsKey(objectName))
        {
            Debug.LogError("Object " + objectName + " is not in the object pool.");
            return null;
        }

        Queue<Transform> objectQueue = m_PoolDictionary[objectName];

        if (objectQueue.Count == 0)
        {
            Debug.LogWarning("No objects available in object pool for " + objectName + ". Creating new object.");
            Transform newObject = Instantiate(m_ObjectsToPool.Find(obj => obj.Name == objectName).Prefab);
            newObject.gameObject.SetActive(true);
            return newObject;
        }

        Transform objectToReturn = objectQueue.Dequeue();
        objectToReturn.SetActive(true);
        return objectToReturn;
    }

    public void ReturnObject(Transform objectToReturn, string objectName)
    {
        if (!m_PoolDictionary.ContainsKey(objectName))
        {
            Debug.LogError("Object " + objectName + " is not in the object pool.");
            return;
        }

        objectToReturn.gameObject.SetActive(false);
        m_PoolDictionary[objectName].Enqueue(objectToReturn);
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