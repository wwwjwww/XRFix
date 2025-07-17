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
        
        foreach (var anchorObject in m_AnchorObjectPool)
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
    }




    private List<GameObject> m_AnchorObjectPool = new List<GameObject>();

    private ARRaycastManager m_RaycastManager;

    private ARAnchorManager m_AnchorManager;

    private ARPlaneManager m_PlaneManager;

    private List<ARAnchor> m_AnchorPoints;

    void Start()
    {
        // Pre-instantiate a pool of anchor objects
        for (int i = 0; i < 10; i++)
        {
            var anchorObject = Instantiate(m_AnchorPrefab);
            anchorObject.SetActive(false);
            m_AnchorObjectPool.Add(anchorObject);
        }
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
                // Check the object pool for an inactive object
                GameObject anchorObject = m_AnchorObjectPool.Find(obj => !obj.activeSelf);
                if (anchorObject != null)
                {
                    anchorObject.transform.position = anchor.transform.position;
                    anchorObject.transform.rotation = anchor.transform.rotation;
                    anchorObject.SetActive(true);
                }
                else
                {
                    // If no inactive objects are available, instantiate a new one and add to the pool
                    anchorObject = Instantiate(m_AnchorPrefab, anchor.transform.position, anchor.transform.rotation);
                    m_AnchorObjectPool.Add(anchorObject);
                }

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
