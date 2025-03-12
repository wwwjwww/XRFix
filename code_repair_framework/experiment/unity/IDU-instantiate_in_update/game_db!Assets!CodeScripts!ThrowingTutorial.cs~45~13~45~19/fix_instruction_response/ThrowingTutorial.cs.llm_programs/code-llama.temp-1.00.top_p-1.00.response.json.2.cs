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
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.K;
    public float throwForce;

    bool readyToThrow;

    private int throwCounter = 0;
    private bool inCollider = false;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        pool = GetComponent<ObjectPool>(); // Initialize the object pool in Awake()
    }

    private void Start()
    {
        readyToThrow = true;
    }


// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//    private void Update()
//    {
//        if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
//        {
//            Throw();
//        }
//    }





    ObjectPool pool; // Add a field for the object pool

    private void Update()
    {
        if (inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            // Use the ObjectPool to instantiate the object:
            GameObject thrownObject = pool.Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation) as GameObject;
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
            throwCounter++;
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        if (throwCounter < totalThrows)
        {
            audioManager.PlaySFX(audioManager.axeSound);
            // Use the ObjectPool to instantiate the object:
            GameObject thrownObject = pool.Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation) as GameObject;
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
            throwCounter++;
        }
        else if (throwCounter == totalThrows)
        {
            audioManager.PlaySFX(audioManager.disappearSound);
            armorObject.SetActive(false);
            audioManager.PlaySFX(audioManager.appearSound);
            swordObject.SetActive(true);
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