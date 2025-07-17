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


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     void Update()
*     {
*         debug.text = controler.transform.rotation.x.ToString() +
*                      "\n" + controler.transform.rotation.y.ToString() +
*                      "\n" + controler.transform.rotation.z.ToString();
*         if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
*         {
*             if (!hasArrow)
*             {
*                 AttachArrow();
*             } else if (isAttached) {
*                 Fire();
*             }
*         }
*         else ThrowArrow();
*     }

    



List<GameObject> arrowPool = new List<GameObject>();

int poolSize = 5;

void Start()
{
    for (int i = 0; i < poolSize; i++)
    {
        GameObject newArrow = Instantiate(arrowPrefab);
        newArrow.SetActive(false);
        arrowPool.Add(newArrow);
    }
}

void Update()
{
    // Your existing code in Update() method...
}

public void AttachArrow()
{
    foreach (GameObject arrow in arrowPool)
    {
        if (!arrow.activeInHierarchy)
        {
            currentArrow = arrow;
            currentArrow.transform.position = controler.transform.position;
            currentArrow.transform.rotation = controler.transform.rotation;
            currentArrow.SetActive(true);
            hasArrow = true;
            break;
        }
    }
}


    
    private void Fire()
    {
        if (isAttached && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0)
        {
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
