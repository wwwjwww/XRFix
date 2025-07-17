using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;







[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
public class AnchorCreator : MonoBehaviour
{

    [SerializeField]
    GameObject m_AnchorPrefab;

    public GameObject AnchorPrefab
    {
        get => m_AnchorPrefab;
        set => m_AnchorPrefab = value;
    }


    public void RemoveAllAnchors()
    {
        foreach (var anchor in m_AnchorPoints)
        {
            Destroy(anchor);
        }
        m_AnchorPoints.Clear();
        foreach (var obj in m_PooledObjects)
        {
            obj.SetActive(false);
        }
    }





    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();
        InitializeObjectPool();
    }

///     void Update()
//     {
// 
//         if (Input.touchCount == 0)
//             return;
// 
//         var touch = Input.GetTouch(0);
//         if (touch.phase != TouchPhase.Began)
//             return;
// 
//         if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
//         {
// 
// 
//             var hitPose = s_Hits[0].pose;
//             var hitTrackableId = s_Hits[0].trackableId;
//             var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);
// 
// 
// 
// 
// 
//             var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             Instantiate(m_AnchorPrefab, anchor.transform);
            // 
            //             if (anchor == null)
            //             {
            //                 Debug.Log("Error creating anchor.");
            //             }
            //             else
            //             {
            // 
            //                 m_AnchorPoints.Add(anchor);
            //             }
            //         }
            //     }

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private List<GameObject> m_PooledObjects;

    private int m_AmountToPool = 10;

    private List<ARAnchor> m_AnchorPoints;

    private ARRaycastManager m_RaycastManager;

    private ARAnchorManager m_AnchorManager;

    private ARPlaneManager m_PlaneManager;

    private void InitializeObjectPool()
    {
        m_PooledObjects = new List<GameObject>();
        for (int i = 0; i < m_AmountToPool; i++)
        {
            GameObject obj = Instantiate(m_AnchorPrefab);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (var obj in m_PooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        GameObject obj = Instantiate(m_AnchorPrefab);
        obj.SetActive(false);
        m_PooledObjects.Add(obj);
        return obj;
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return;

        if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;
            var hitTrackableId = s_Hits[0].trackableId;
            var hitPlane = m_PlaneManager.GetPlane(hitTrackableId);

            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
            if (anchor == null)
            {
                Debug.Log("Error creating anchor.");
            }
            else
            {
                m_AnchorPoints.Add(anchor);
                GameObject anchorObject = GetPooledObject();
                anchorObject.transform.position = anchor.transform.position;
                anchorObject.transform.rotation = anchor.transform.rotation;
                anchorObject.SetActive(true);
            }
        }
    }



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
