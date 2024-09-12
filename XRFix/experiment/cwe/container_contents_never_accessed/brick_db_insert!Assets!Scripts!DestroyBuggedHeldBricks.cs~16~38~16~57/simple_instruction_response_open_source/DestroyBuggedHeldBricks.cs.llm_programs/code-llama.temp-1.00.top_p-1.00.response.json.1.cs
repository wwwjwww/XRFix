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

    
    
Here is the fixed code for the `DestroyBuggedHeldBricks` script:

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

    private void Start()
    {
        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;
    }

    private void Update()
    {
        if (_interactionManager.primarySelectedInteractor != null)
        {
            if (_interactionManager.primarySelectedInteractor.attachTransform.parent != _leftHandTransform)
            {
                _interactionManager.primarySelectedInteractor.attachTransform.parent = _leftHandTransform;
            }
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BricksDestroyer : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    private void Start()
    {
        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;
    }

    private void Update()
    {
        if (_interactionManager.primarySelectedInteractor != null)
        {
            if (_interactionManager.primarySelectedInteractor.attachTransform.parent != _rightHandTransform)
            {
                _interactionManager.primarySelectedInteractor.attachTransform.parent = _rightHandTransform;
            }
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandsDestroyer : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    private void Start()
    {
        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;
    }

    private void Update()
    {
        if (_interactionManager.primarySelectedInteractor != null)
        {
            if (_interactionManager.primarySelectedInteractor.attachTransform.parent != _leftHandTransform)
            {
                _interactionManager.primarySelectedInteractor.attachTransform.parent = _leftHandTransform;
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
