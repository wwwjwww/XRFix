using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject mainCamera;

    private GameObject selectedObject;
    private GameObject heldObject; 

    void Start()
    {
        rightHand = GameObject.Find("RightHand");
        leftHand = GameObject.Find("LeftHand");
        mainCamera = GameObject.Find("MainCamera");
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            // FIXED CODE: Add check for hit.collider != null
            if (hit.collider != null)
            {
                // FIXED CODE: Add check for hit.collider.tag == "Interactable"
                if (hit.collider.tag == "Interactable")
                {
                    selectedObject = hit.collider.gameObject;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // FIXED CODE: Add check for other.tag == "Interactable"
        if (other.tag == "Interactable")
        {
            heldObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // FIXED CODE: Add check for other.tag == "Interactable"
        if (other.tag == "Interactable")
        {
            heldObject = null;
        }
    }
}

