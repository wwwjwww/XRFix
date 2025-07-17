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




private Queue<GameObject> anchorPool = new Queue<GameObject>();

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

        // Check if the anchor pool is not empty
        if (anchorPool.Count > 0)
        {
            var anchorObject = anchorPool.Dequeue();
            anchorObject.transform.position = hitPose.position;
            anchorObject.transform.rotation = hitPose.rotation;
            anchorObject.SetActive(true);
            m_AnchorPoints.Add(anchorObject.GetComponent<ARAnchor>());
        }
        else
        {
            var anchor = m_AnchorManager.AttachAnchor(hitPlane, hitPose);
            var newAnchorObject = Instantiate(m_AnchorPrefab, anchor.transform);
            newAnchorObject.SetActive(true);
            m_AnchorPoints.Add(anchor);
        }
    }
}



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
