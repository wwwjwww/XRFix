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
    public class DestroyBuggedHeldBricks : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    // Bug fix:
    // - Removed the unused interactablesToCheck field
    // - Added a field to keep track of the current bricks that are being held
    private List<XRBaseInteractable> _bricksBeingHeld;

    void Start()
    {
        // Initialize the list of bricks being held
        _bricksBeingHeld = new List<XRBaseInteractable>();
    }

    void Update()
    {
        // Check if a brick is being held by the left hand
        if (leftHand.attachTransform != null)
        {
            // Get the brick being held and add it to the list of bricks being held
            XRBaseInteractable brick = leftHand.attachTransform.GetComponent<XRBaseInteractable>();
            if (brick != null)
            {
                _bricksBeingHeld.Add(brick);
            }
        }

        // Check if a brick is being held by the right hand
        if (rightHand.attachTransform != null)
        {
            // Get the brick being held and add it to the list of bricks being held
            XRBaseInteractable brick = rightHand.attachTransform.GetComponent<XRBaseInteractable>();
            if (brick != null)
            {
                _bricksBeingHeld.Add(brick);
            }
        }

        // Destroy any bricks that are being held and are not in the list of bricks being held
        foreach (XRBaseInteractable brick in _interactionManager.interactables)
        {
            if (brick.isHeld)
            {
                if (!_bricksBeingHeld.Contains(brick))
                {
                    Destroy(brick);
                }
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
