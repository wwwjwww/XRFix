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

    void Awake()
    {
        leftHandTransform = leftHand.transform;
        rightHandTransform = rightHand.transform;
    }

    void Update()
    {
        // Check if both hands have picked up a brick
        if (leftHand. TryGetHandObject (out GameObject leftHandObject) &&
            rightHand.TryGetHandObject(out GameObject rightHandObject) &&
            leftHandObject.CompareTag("Brick") &&
            rightHandObject.CompareTag("Brick"))
        {
            // Get the center point of the two hands
            Vector3 centerPoint = (leftHandTransform.position + rightHandTransform.position) / 2f;

            // Create an empty game object at the center point
            GameObject spawnedObject = new GameObject();
            spawnedObject.transform.position = centerPoint;

            // Destroy the two held bricks
            Destroy(leftHandObject);

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
