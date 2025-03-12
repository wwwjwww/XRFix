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
            currentArrow.GetComponent<Collider>().isTrigger = true;
            hasArrow = true;
        }
    }
    
    private void Fire()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) <= 0)
        {
            float dist = (arrowStartPoint.transform.position - controler.transform.position).magnitude;
            currentArrow.transform.parent = null;
            Rigidbody r = currentArrow.GetComponent<Rigidbody>();
            r.velocity = arrowStartPoint.transform.forward * 25f * dist;
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
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            if (!hasArrow)
            {
                AttachArrow();
            }
            else if (hasArrow && !isAttached)
            {
                isAttached = true;
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
            Rigidbody r = LastArrow.GetComponent<Rigidbody>();
            r.isKinematic = false;
            r.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
            r.velocity = OVRInput.GetLocalControllerVelocity(controller);
            LastArrow.GetComponent<Collider>().isTrigger = false;
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
