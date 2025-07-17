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
            Destroy(gameObject); // Ensure there's only one instance of ArrowManager
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
            hasArrow = true;
        }
    }
    



    void Update()
    {
        debug.text = controler.transform.rotation.x.ToString() +
                     "\n" + controler.transform.rotation.y.ToString() +
                     "\n" + controler.transform.rotation.z.ToString();
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            if (!hasArrow && !isAttached) // ensure AttachArrow is only called once per arrow
            {
                AttachArrow();
            }
            else if (hasArrow && !isAttached)
            {
                isAttached = true; // flag that the arrow is now attached and ready to be fired
            }
        }
        else if (isAttached)
        {
            Fire(); // ensure Fire is called once when trigger is released
        }
    }

    private void Fire()
    {
        float dist = (arrowStartPoint.transform.position - controler.transform.position).magnitude;
        currentArrow.transform.parent = null;
        Rigidbody r = currentArrow.GetComponent<Rigidbody>();
        r.isKinematic = false;
        r.velocity = currentArrow.transform.forward * 25f * dist;
        r.useGravity = true;
        currentArrow.GetComponent<Collider>().isTrigger = false;
        LastArrow = currentArrow; // keep reference to the last arrow
        currentArrow = null;
        hasArrow = false;
        isAttached = false;
    }


    
    public void ThrowArrow()
    {
        if (currentArrow != null)
        {
            LastArrow = currentArrow;
            currentArrow = null;
            LastArrow.transform.parent = null;
            LastArrow.GetComponent<Rigidbody>().isKinematic = false;
            LastArrow.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(controller);
            LastArrow.GetComponent<Rigidbody>().velocity = OVRInput.GetLocalControllerVelocity(controller);
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
