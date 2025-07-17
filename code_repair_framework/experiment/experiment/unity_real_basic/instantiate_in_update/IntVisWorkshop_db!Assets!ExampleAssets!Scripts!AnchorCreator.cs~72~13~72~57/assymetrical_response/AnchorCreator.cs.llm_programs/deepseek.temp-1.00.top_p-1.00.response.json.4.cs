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




void Update()
{
    // Check if there are any touches on the screen
    if (Input.touchCount > 0)
    {
        // Get the touch position
        Vector2 touchPosition = Input.GetTouch(0).position;

        // Create a list to hold the hits
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Try to raycast into the scene
        if (m_RaycastManager.Raycast(touchPosition, hits, TrackableType.All))
        {
            // If we hit something, create a new anchor
            Pose hitPose = hits[0].pose;
            ARAnchor newAnchor = m_AnchorManager.AddAnchor(hitPose);
            m_AnchorPoints.Add(newAnchor);
        }
    }
}



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
