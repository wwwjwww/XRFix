using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private GameObject currentArrow;
    public static ArrowManager Instance;
    public OVRInput.Controller controller;
    public GameObject controler;
    public GameObject arrowPrefab;
    public GameObject LastArrow;
    public GameObject arrowStartPoint;
    public TextMesh debug;
    private bool hasArrow = false;
    private bool isAttached = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject); // Ensure only one instance of ArrowManager
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


//    void Update()
//    {
//        debug.text = controler.transform.rotation.x.ToString() +
//                     "\n" + controler.transform.rotation.y.ToString() +
//                     "\n" + controler.transform.rotation.z.ToString();
//        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
//        {
//            if (!hasArrow)
//            {
//                AttachArrow();
//            } else if (isAttached) {
//                Fire();
//            }
//        }
//        else ThrowArrow();
//    }

    
    public void AttachArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, controler.transform);
            currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            currentArrow.GetComponent<Collider>().isTrigger = true; // Ensure the collider is a trigger while attached
            hasArrow = true;
        }
    }
    
    private void Fire()
    {
        if (isAttached && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1f)
        { // changed threshold to account for accidentally going below zero
            float dist = (arrowStartPoint.transform.position - controler.transform.position).magnitude;
            currentArrow.transform.parent = null;
            Rigidbody r = currentArrow.GetComponent<Rigidbody>();
            r.velocity = currentArrow.transform.forward * 25f * dist;
            r.useGravity = true;
            currentArrow.GetComponent<Collider>().isTrigger = false;
            currentArrow = null;
            hasArrow = false;
            isAttached = false;
        }
    }
    



    void Update()
    {
        debug.text = controler.transform.rotation.x.ToString() +
                     "\n" + controler.transform.rotation.y.ToString() +
                     "\n" + controler.transform.rotation.z.ToString();
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.1f)
        { // changed threshold to avoid accidental firing
            if (!hasArrow)
            {
                AttachArrow();
            }
            else if (!isAttached)
            {
                isAttached = true; // Set isAttached to avoid multiple calls to Fire
            }
        }
        else if (isAttached)
        {
            Fire();
        }
        else
        {
            ThrowArrow();
        }
    }

    public void ThrowArrow()
    {
        if (currentArrow != null)
        {
            LastArrow = currentArrow;
            currentArrow = null;
            LastArrow.transform.parent = null;
            Rigidbody rb = LastArrow.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
            rb.velocity = OVRInput.GetLocalControllerVelocity(controller);
            hasArrow = false;
            isAttached = false;
        }
    }



    public void AttachBowToArrow()
    {
        currentArrow.transform.parent = controler.transform;
        currentArrow.transform.position = arrowStartPoint.transform.position;
        currentArrow.transform.rotation = arrowStartPoint.transform.rotation;

        isAttached = true;
    }
    
    public void playSound()
    {

    }
}
