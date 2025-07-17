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
    Here is the fixed code based on your feedback:

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyBuggedHeldBricks : MonoBehaviour {

	public XRInteractionManager _interactionManager;

	public XRDirectInteractor leftHand;
	public XRDirectInteractor rightHand;

	private Transform _leftHandTransform;
	private Transform _rightHandTransform;

	public List<XRBaseInteractable> interactablesToCheck; // New line

	// Use this for initialization


	void Start () {
		_leftHandTransform = leftHand.transform;
		_rightHandTransform = rightHand.transform;

	}

	// Update is called once per frame


	void Update () {

		interactablesToCheck = new List<XRBaseInteractable>(); // New line

		// Check for left hand interaction

		XRBaseInteractable leftInteractable = _interactionManager.GetCurrentInteractable(_leftHandTransform);

		if (leftInteractable) {
			interactablesToCheck.Add(leftInteractable)
		}

		// Check for right hand interaction

		XRBaseInteractable rightInteractable = _interactionManager.GetCurrentInteractable(_rightHandTransform);

		if (rightInteractable) {
			interactablesToCheck.Add(rightInteractable)
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
