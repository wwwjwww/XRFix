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
            else if (isAttached) {
                Fire();
            }
        }
        else ThrowArrow();
    }

    public void AttachArrow()
    {
        if (currentArrow == null)
        {
            currentArrow = ObjectPool.GetObject(arrowPrefab, controler.transform);
            currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            hasArrow = true;
        }
    }

public class ObjectPool
{
    //A list of active objects
    public static List<GameObject> activeObjects = new List<GameObject>();
	//The Prefab that will be pooled
	public static GameObject obj;
	//The amount of objects to be pooled
	public static int poolSize;
	//Constructor
	public ObjectPool (GameObject obj, int poolSize)
	{
		ObjectPool.obj = obj;
		ObjectPool.poolSize = poolSize;
		//Create the objects and add them to the active objects list
		for (int i = 0; i < poolSize; i++)
		{
			GameObject object = (GameObject)Instantiate(obj);
			activeObjects.Add(object);
		}
	}
	//Get an object from the pool. Return null if there are no more objects.
	public static GameObject GetObject (GameObject obj, Transform parent )
	{
		for (int i=0; i < activeObjects.Count; i++)
		{
			if (!activeObjects[i].gameObject.activeInHierarchy) {
                activeObjects[i].transform.parent = parent;
				return activeObjects[i];
			}
		}
		ObjectPool.obj = Instantiate(obj) ;
        ObjectPool.obj.transform.parent = parent;
		return ObjectPool.obj ;
	}
	//Destory an object. Return it to the pool.
	public static void DestroyObject (GameObject obj)
	{
		if (obj != null)
		{
			int index = activeObjects.FindIndex(x =>x.gameObject == obj);
			if (index >= 0)
			{
				activeObjects.RemoveAt(index);
				Destroy(obj);
			}
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
