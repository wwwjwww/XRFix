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

        // You'll want to replace this with the actual number of arrows you plan on pooling.
        // For a small number of arrows, we can use a Queue for speed. For larger numbers use LinkedList or an ObjectPool.
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(arrowPrefab);
            obj.SetActive(false);
            arrowPool.Enqueue(obj);
        }
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

    



    private Queue<GameObject> arrowPool = new Queue<GameObject>();

    public GameObject GetArrowFromPool()
    {
        if (arrowPool.Count == 0)
        {
            GameObject obj = Instantiate(arrowPrefab);
            return obj;
        }
        return arrowPool.Dequeue();
    }

    public void ReturnArrowToPool(GameObject arrow)
    {
        arrow.SetActive(false);
        arrowPool.Enqueue(arrow);
    }

    void Update()
    {
        debug.text = controler.transform.rotation.x.ToString() +
                      "\n" + controler.transformtransform.rotation.y.ToString() +
                      "\n" + controler.transform.rotation.z.ToString();
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0)
        {
            if (!hasArrow)
            {
                AttachArrow();
            } else if (isAttached) {
                Fire();
            }
        }
        else ThrowArrow();
    }

    public void AttachArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = GetArrowFromPool();
            currentArrow.transform.SetParent(controler.transform);
            currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            hasArrow = true;
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
