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
    private void Start()
{
    _interactionManager = GetComponent<XRInteractionManager>();
    leftHand = _interactionManager.GetInteractorByHand(XRHandedness.Left);
    rightHand = _interactionManager.GetInteractorByHand(XRHandedness.Right);

    _leftHandTransform = leftHand.GetComponent<XRDirectInteractor>().attachedTo.transform;
    _rightHandTransform = rightHand.GetComponent<XRDirectInteractor>().attachedTo.transform;
}

public void Update()
{
    if (_interactionManager.IsReadyForBrickDestruction)
    {
        //Check right hand transform, if its valid, use that for checking interactables, else use left hand transform and same check, otherwise do nothing
        if (_rightHandTransform.gameObject.activeSelf)
        {
            Debug.Log("Right Hand: " + _interactionManager.BrickContainer.transform.GetChild(0), transform);
        }
        else
        {
            Debug.Log("Left Hand: " + _interactionManager.BrickContainer.transform.GetChild(0), transform);
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
