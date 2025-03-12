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
    // Initialize the anchor pool
    m_AnchorPool = new List<GameObject>();
    for (int i = 0; i < m_AnchorPoolSize; i++)
    {
        GameObject obj = Instantiate(m_AnchorPrefab, transform);
        obj.SetActive(false);
        m_AnchorPool.Add(obj);
    }
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


public GameObject m_AnchorPrefab;

private List<GameObject> m_AnchorPool;

private int m_AnchorPoolSize = 5;

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

        // Get an inactive anchor from the pool
        GameObject anchorObj = GetInactiveAnchor();
        if (anchorObj != null)
        {
            // Set the anchor position and rotation
            anchorObj.transform.position = hitPose.position;
            anchorObj.transform.rotation = hitPose.rotation;

            // Set the anchor to active
            anchorObj.SetActive(true);
        }
        else
        {
            Debug.Log("Error creating anchor.");
        }
    }
}

private GameObject GetInactiveAnchor()
{
    for (int i = 0; i < m_AnchorPool.Count; i++)
    {
        if (!m_AnchorPool[i].activeSelf)
        {
            return m_AnchorPool[i];
        }
    }
    return null;
}



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
