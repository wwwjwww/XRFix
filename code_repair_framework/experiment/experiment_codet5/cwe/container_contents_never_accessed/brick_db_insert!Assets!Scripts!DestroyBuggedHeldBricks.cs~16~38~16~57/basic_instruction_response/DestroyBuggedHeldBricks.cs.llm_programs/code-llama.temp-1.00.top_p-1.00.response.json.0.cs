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

    // A collection or map whose contents are never queried or accessed is useless.
    // Fix: Introduce a new attribute of type List<XRBaseInteractable> to store the interactables to check.
    [HideInInspector]
    public List<XRBaseInteractable> interactablesToCheck = new List<XRBaseInteractable>();

    private void Start()
    {
        _leftHandTransform = leftHand.attachedTo;
        _rightHandTransform = rightHand.attachedTo;
    }

    private void Update()
    {
        if (CanDestroyBrick())
        {
            destroyBrick();
        }
    }

    private bool CanDestroyBrick()
    {
        foreach (XRBaseInteractable interactable in interactablesToCheck)
        {
            if (!isBricks(interactable.gameObject))
            {
                continue;
            }

            if (interactable.IsBeingHeld)
            {
                return true;
            }
        }

        return false;
    }

    private void destroyBrick()
    {
        Destroy(_leftHandTransform.gameObject);
        Destroy(_rightHandTransform.gameObject);

        ClearInteractablesToCheck();
    }

    private void ClearInteractablesToCheck()
    {
        interactablesToCheck.Clear();
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
