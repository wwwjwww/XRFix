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

    private void Start()
    {
        readyToThrow = true;
    }


// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    private void Update()
//    {
//        if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
//        {
//            Throw();
//        }
//    }





private void FixedUpdate()
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
        audioManager.PlaySFX(audioManager.axeSound);

        GameObject thrownObject = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
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