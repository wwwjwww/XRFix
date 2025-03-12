using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private GameObject currentArrow;
    private static ArrowManager _instance;
    public OVRInput.Controller controller;
    public GameObject controler;
    public GameObject arrowPrefab;
    public GameObject LastArrow;
    public GameObject arrowStartPoint;
    public TextMesh debug;
    private bool hasArrow = false;
    private bool isAttached = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance!= this)
        {
            Destroy(gameObject);
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

    



    [SerializeField] private GameObject _arrowPrefab;

    [SerializeField] private Transform _arrowParent;

    [SerializeField] private OVRInput.Controller _controller = OVRInput.Controller.LTouch;

    private GameObject _currentArrow;

    private bool _hasArrow;

    private bool _isAttached;

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, _controller) > 0f)
        {
            if (!_hasArrow)
            {
                AttachArrow();
            }
            else if (_isAttached)
            {
                Fire();
            }
        }
        else
        {
            ThrowArrow();
        }
    }

    public void AttachArrow()
    {
        if (_currentArrow == null)
        {
            _currentArrow = Instantiate(_arrowPrefab, _arrowParent);
            _currentArrow.transform.localPosition = new Vector3(0.1f, 0f, 0.3f);
            _currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            _hasArrow = true;
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
