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


    private List<XRBaseInteractable> interactablesToCheck = new List<XRBaseInteractable>();

    void Start()
    {
        UpdateHandTransforms();
    }

    void UpdateHandTransforms()
    {
        if (leftHand != null)
        {
            _leftHandTransform = leftHand.transform;
        }

        if (rightHand != null)
        {
            _rightHandTransform = rightHand.transform;
        }
    }

    void Update()
    {
        if (_interactionManager != null)
        {
            // Assuming there's logic needed to manage or use the interactables
            _interactionManager.GetRegisteredInteractables(interactablesToCheck);
            foreach (var interactable in interactablesToCheck)
            {
                // Example logic: Perform actions on interactables
                if (interactable.transform.IsChildOf(_leftHandTransform) || interactable.transform.IsChildOf(_rightHandTransform))
                {
                    // Logic to handle bugged interactables
                }
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
