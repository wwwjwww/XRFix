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
    }





    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();
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

            // FIXED CODE:


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

        if (anchor != null)
        {
            var instantiatedObject = Instantiate(m_AnchorPrefab, anchor.transform.position, anchor.transform.rotation);
            instantiatedObject.transform.parent = anchor.transform;
            m_AnchorPoints.Add(anchor);
        }
        else
        {
            Debug.Log("Error creating anchor.");
        }
    }
}

In the fixed version, the anchor is created at the right time (after checking anchor is not null) and the prefab is instantiated at the anchor's position and rotation. We also set the instantiated object as a child of the anchor to make sure it gets destroyed when the anchor is destroyed.


    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
