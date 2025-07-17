using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowingTutorial : MonoBehaviour
{
    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.K;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public GameObject armorObject;
    public GameObject swordObject;
    public GameObject player;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.K;
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
        // Here's the buggy code:
        // audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        // Instead, you can try to build an object pool before Update() method has been called.
        // For example:
        // audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        // Here's the buggy code:
        // readyToThrow = true;
        // Instead, you can try to build an object pool before Update() method has been called.
        // For example:
        // readyToThrow = false;
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





    private void Update()
    {
        // Here's the buggy code:
        // if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        // {
        //     Throw();
        // }
        // Instead, you can try to build an object pool before Update() method has been called.
        // For example:
        if (inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        // Here's the buggy code:
        // readyToThrow = false;
        // Instead, you can try to build an object pool before Update() method has been called.
        // For example:
        readyToThrow = false;
        
        if (throwCounter < totalThrows)
        {
            audioManager.PlaySFX(audioManager.axeSound);

            // Here's the buggy code:
            // GameObject thrownObject = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
            // Instead, you can try to build an object pool before Update() method has been called.
            // For example:
            GameObject thrownObject = GameObject.Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
            throwCounter++;
        }
        else if (throwCounter == totalThrows)
        {
            audioManager.PlaySFX(audioManager.disappearSound);
            armorObject.SetActive(false); // Make the armor disappear
            audioManager.PlaySFX(audioManager.appearSound);
            swordObject.SetActive(true); // Make the sword appear
        }

        totalThrows--;

        Invoke(nameof(ResetThrow), throwCooldown);
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