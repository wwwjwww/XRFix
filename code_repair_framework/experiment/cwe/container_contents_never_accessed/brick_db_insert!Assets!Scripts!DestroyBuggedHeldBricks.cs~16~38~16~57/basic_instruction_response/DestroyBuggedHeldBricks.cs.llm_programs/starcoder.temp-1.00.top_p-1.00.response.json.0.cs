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

    // FIXED CODE:
    public XRInteractionManager _interactionManager;

public XRDirectInteractor leftHand;
public XRDirectInteractor rightHand;

private Transform _leftHandTransform;
private Transform _rightHandTransform;

private readonly List<XRBaseInteractable> interactablesToCheck = new List<XRBaseInteractable>();

void Start()
{
    _leftHandTransform = leftHand.transform;
    _rightHandTransform = rightHand.transform;
}

void Update()
{
    if (leftHand.controller.gripPressed || rightHand.controller.gripPressed)
    {
        _interactionManager.GetValidInteractables(_leftHandTransform, interactablesToCheck);

        foreach (XRBaseInteractable interactable in interactablesToCheck)
        {
            if (interactable. TryGetTarget(out XRInteractiveObject target))
            {
                if (target.name.StartsWith("Brick"))
                {
                    target.transform.parent.parent.gameObject.SetActive(false);
                }
            }
        }
    }
}

// This code assumes that there is only one instance of each Brick game object and that they are children of a parent object that has the "Interactables" tag.

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
