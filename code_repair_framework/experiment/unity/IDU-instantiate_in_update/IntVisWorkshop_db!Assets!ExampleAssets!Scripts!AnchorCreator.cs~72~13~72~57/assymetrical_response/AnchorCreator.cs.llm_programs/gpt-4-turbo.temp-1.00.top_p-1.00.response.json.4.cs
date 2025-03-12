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
            Destroy(anchor.gameObject);
        }
        m_AnchorPoints.Clear();

        foreach (var anchorObject in m_AnchorPool)
        {
            anchorObject.SetActive(false);
        }
    }





    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_AnchorManager = GetComponent<ARAnchorManager>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_AnchorPoints = new List<ARAnchor>();
        m_AnchorPool = new List<GameObject>();

        for (int i = 0; i < 10; i++)  // Adjust the pool size based on expected usage
        {
            var anchorObj = Instantiate(m_AnchorPrefab);
            anchorObj.SetActive(false);
            m_AnchorPool.Add(anchorObj);
        }
    }




    List<GameObject> m_AnchorPool;

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

                foreach (var anchorObj in m_AnchorPool)
                {
                    if (!anchorObj.activeInHierarchy)
                    {  
                        // Reuse an inactive object from the pool
                        anchorObj.transform.position = anchor.transform.position;
                        anchorObj.transform.rotation = anchor.transform.rotation;
                        anchorObj.SetActive(true);
                        break;
                    }
                }
            }
        }
    }



    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    List<ARAnchor> m_AnchorPoints;

    ARRaycastManager m_RaycastManager;

    ARAnchorManager m_AnchorManager;

    ARPlaneManager m_PlaneManager;
}
