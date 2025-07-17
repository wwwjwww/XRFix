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
            m_AnchorManager.RemoveAnchor(anchor);
            Destroy(anchor.gameObject);
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

        var ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var hitPoint = hit.point;
            var plane = m_PlaneManager.GetPlane(hitPoint).Value;

            var hitTrackableId = new TrackableId();
            hitTrackableId.subId = plane.trackableId.subId;
            hitTrackableId.index = plane.trackableId.index;

            var anchorPose = new Pose(hitPoint, Quaternion.identity);
            var anchor = m_AnchorManager.AddAnchor(anchorPose, hitTrackableId);

            m_AnchorPoints.Add(anchor);
            Instantiate(m_AnchorPrefab, anchor.transform.position, Quaternion.identity);
        }
    }



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
