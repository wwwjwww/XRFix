-
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandManager : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject grabbedObject;

    private Hand leftHandController;
    private Hand rightHandController;
    private RaycastHit hitInfo;

    void Start()
    {
        leftHandController = leftHand.GetComponent<Hand>();
        rightHandController = rightHand.GetComponent<Hand>();
    }

    void Update()
    {
        if (leftHandController.TryGetPointerEventData(out var data))
        {
            foreach (var touch in data.touches)
            {
                if (touch.fingerId == 0)
                {
                    if (Physics.Raycast(touch.position, data.pointerCurrentRaycast.direction, out hitInfo, 100.0f, Physics.DefaultRaycastLayers))
                    {
                        if (hitInfo.collider.CompareTag("PickupTag"))
                        {
                            Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                            if (rigidbody)
                            {
                                rigidbody.isKinematic = true;
                            }
                            grabbedObject = hitInfo.collider.gameObject;
                            grabbedObject.transform.SetParent(leftHandController.transform);
                        }
                    }
                }
            }
        }

        if (rightHandController.TryGetPointerEventData(out var data))
        {
            foreach (var touch in data.touches)
            {
                if (touch.fingerId == 0)
                {
                    if (Physics.Raycast(touch.position, data.pointerCurrentRaycast.direction, out hitInfo, 100.0f, Physics.DefaultRaycastLayers))
                    {
                        if (hitInfo.collider.CompareTag("PickupTag"))
                        {
                            Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                            if (rigidbody)
                            {
                                rigidbody.isKinematic = true;
                            }
                            grabbedObject = hitInfo.collider.gameObject;
                            grabbedObject.transform.SetParent(rightHandController.transform);
                        }
                    }
                }
            }
        }
    }
  
}

