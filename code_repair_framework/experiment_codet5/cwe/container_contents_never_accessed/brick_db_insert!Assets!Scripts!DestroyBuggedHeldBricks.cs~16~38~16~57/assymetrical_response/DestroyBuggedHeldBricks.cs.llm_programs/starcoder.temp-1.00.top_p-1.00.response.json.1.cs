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
    Here is a possible solution for the given code:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyBuggedHeldBricks : MonoBehaviour
{
    public XRInteractionManager interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform leftHandTransform;
    private Transform rightHandTransform;

    private void Awake()
    {
        leftHandTransform = leftHand.transform;
        rightHandTransform = rightHand.transform;
    }

    private void OnEnable()
    {
        interactionManager.selectEnter += OnSelectEnter;
        interactionManager.selectExit += OnSelectExit;
    }

    private void OnDisable()
    {
        interactionManager.selectEnter -= OnSelectEnter;
        interactionManager.selectExit -= OnSelectExit;
    }

    private void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (interactor == leftHand || interactor == rightHand)
        {
            foreach (XRBaseInteractable interactable in interactionManager.interactors)
            {
                if (interactable is XRBaseController)
                {
                    Destroy(interactable.gameObject);
                }
            }
        }
    }

    private void

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
