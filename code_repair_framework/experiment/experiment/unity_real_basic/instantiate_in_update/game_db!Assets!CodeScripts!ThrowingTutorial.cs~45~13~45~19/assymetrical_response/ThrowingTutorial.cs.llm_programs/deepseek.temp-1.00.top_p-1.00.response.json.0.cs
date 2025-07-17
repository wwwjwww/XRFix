using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowingTutorial : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public GameObject armorObject;
    public GameObject swordObject;
    public GameObject player;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown; //after each throw

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.K;
    public float throwForce;

    bool readyToThrow;

    private int throwCounter = 0; // counts throws
    private bool inCollider = false; // tracks if player is in collider

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        // initialize object pool
        throwPool = new GameObject[totalThrows];
        for (int i = 0; i < totalThrows; i++)
        {
            throwPool[i] = Instantiate(objectToThrow);
            throwPool[i].SetActive(false);
        }
    }


/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*     private void Update()
*     {
*         if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
*         {
*             Throw();
*         }
*     }





    private GameObject[] throwPool;   // pool of objects

    private void Update()
    {
        if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        if (throwCounter < totalThrows)
        {
            //...existing code for playing sounds...

            // get the first inactive object from the pool
            GameObject thrownObject = null;
            for(int i = 0; i < throwPool.Length; i++)
            {
                if(!throwPool[i].activeInHierarchy)
                {
                    thrownObject = throwPool[i];
                    break;
                }
           
	    	thrownObject.SetActive(true);
	    	thrownObject.transform.position = attackPoint.position;
	    	thrownObject.transform.rotation = attackPoint.rotation;
	    	Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
	    	rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
	    	throwCounter++;
	    	}

	    	Invoke(nameof(ResetThrow), throwCooldown);
        }
    }



    private void ResetThrow()
    {
        readyToThrow = true;
    }

    // This method is called when the player enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) 
        {
            inCollider = true;
        }
    }

    // This method is called when the player leaves the collider
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) 
        {
            inCollider = false;
        }
    }
}