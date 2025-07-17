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

    /* BUG: Container contents are never accessed
    * MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    *     private List<XRBaseInteractable> interactablesToCheck;

    * Remove or Commented-out the collection if it is no longer needed
    * FIXED CODE:
    */
    FIXED CODE:
﻿using System.Collections;
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

    // Use this for initialization
    void Start()
    {
        leftHand = _interactionManager.GetDevice(InputDeviceCharacteristics.Left);
        rightHand = _interactionManager.GetDevice(InputDeviceCharacteristics.Right);

        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHand.SelectPressed || rightHand.SelectPressed)
        {
            Debug.Log("Select Pressed");
        }

        if (leftHand.GrabPressed || rightHand.GrabPressed)
        {
            Debug.Log("Grab Pressed");
        }

        if (leftHand.UngrabPressed || rightHand.UngrabPressed)
        {
            Debug.Log("Ungrab Pressed");
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
