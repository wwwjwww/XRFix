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

        foreach (var anchorObject in m_AnchorObjectsPool)
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




    private List<GameObject> m_AnchorObjectsPool = new List<GameObject>();

    void Start()
    {
        // Pre-build anchor objects in a pool
        for (int i = 0; i < 10; i++) // Adjust the pool size as necessary
        {
            GameObject anchorObject = Instantiate(m_AnchorPrefab);
            anchorObject.SetActive(false);
            m_AnchorObjectsPool.Add(anchorObject);
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
                GameObject anchorObject = GetPooledObject();
                if (anchorObject != null)
                {
                    anchorObject.transform.position = anchor.transform.position;
                    anchorObject.transform.rotation = anchor.transform.rotation;
                    anchorObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("No available anchor objects in the pool.");
                }
                m_AnchorPoints.Add(anchor);
            }
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (var anchorObject in m_AnchorObjectsPool)
        {
            if (!anchorObject.activeInHierarchy)
            {
                return anchorObject;
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
