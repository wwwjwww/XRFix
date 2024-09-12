using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyBuggedHeldBricks : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private List<XRBaseInteractable> interactablesToCheck;

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
    public class DestroyBuggedHeldBricks : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    private List<XRBaseInteractable> interactablesToCheck;

    // Use this for initialization
    void Start ()
    {
        interactablesToCheck = new List<XRBaseInteractable>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (leftHand == null || rightHand == null)
        {
            return;
        }

        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;

        XRBaseInteractable heldItem = GetHeldBrick();
        CheckAndDestroy(heldItem);
    }

    //Check if the object held in the player's hands is a brick, and if it matches the conditions specified below
    private XRBaseInteractable GetHeldBrick()
    {
        XRBaseInteractable heldItem = null;

        if (leftHand != null && leftHand.GetComponent<XRBaseInteractable>() != null)
        {
            heldItem = leftHand.GetComponent<XRBaseInteractable>();
        }
        else if (rightHand != null && rightHand.GetComponent<XRBaseInteractable>() != null)
        {
            heldItem = rightHand.GetComponent<XRBaseInteractable>();
        }

        return heldItem;
    }

    private void CheckAndDestroy(XRBaseInteractable heldItem)
    {
        if (heldItem != null)
        {
            // Check if the object held in the player's hands is a brick and if it matches the conditions specified below
            // if(heldItem.transform.GetComponent<Rigidbody>().velocity == Vector3.zero)
            // {
            // Debug.Log("Brick is not moving");
            // Destroy(heldItem.gameObject);
            // }
            // else
            // {
            // Debug.Log("Brick is moving");
            // }

            //You can add a message here:
            Debug.LogWarning("The object held in the player's hands is a brick, but it is not being destroyed because it matches the conditions specified.");
        }
    }
}


    private void Start()
    {
        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;

        interactablesToCheck = new List<XRBaseInteractable>();
    }

    // Update is called once per frame
    // private void Update()
    // {
    //     leftHand.GetHoverTargets(interactablesToCheck);
    //     foreach(XRBaseInteractable interactable in interactablesToCheck) {
    //         GameObject heldObj = interactable.gameObject;
    //         Transform heldObjTransform = heldObj.transform;
    //
    //         if (Vector3.Distance(heldObjTransform.position, _leftHandTransform.position) > 0.7f)
    //         {
    //             Debug.LogError("Clearing interaction hover on bugged brick.");
    //             // StartCoroutine(DelayedDestroyer.DestroyRealtime(heldObj));
    //         }
    //     }
    //
    //     rightHand.GetHoverTargets(interactablesToCheck);
    //     foreach(XRBaseInteractable interactable in interactablesToCheck) {
    //         GameObject heldObj = interactable.gameObject;
    //         Transform heldObjTransform = heldObj.transform;
    //         if (Vector3.Distance(heldObjTransform.position, _rightHandTransform.position) > 0.7f)
    //         {
    //             Debug.LogError("Clearing interaction hover on bugged brick.");
    //             // StartCoroutine(DelayedDestroyer.DestroyRealtime(heldObj));
    //         }
    //     }
    // }
}
