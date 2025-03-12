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
        // Create a pool of projectiles
        projectilePool = new GameObject[totalThrows];
        for (int i = 0; i < totalThrows; i++)
        {
            GameObject obj = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
            obj.SetActive(false); // Deactivate them all initially
            projectilePool[i] = obj;
        }

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





    private GameObject[] projectilePool;

    private int currentPoolIndex = 0;

    private void Update()
    {
        if (inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        if (throwCounter < totalThrows)
        {
            //...

            // Enable next available pooled object
            projectilePool[currentPoolIndex].SetActive(true);
            //...

            // Reset the position and rotation of the pooled object
            projectilePool[currentPoolIndex].transform.position = attackPoint.position;
            projectilePool[currentPoolIndex].transform.rotation = attackPoint.rotation;

            // Get next index
            currentPoolIndex = (currentPoolIndex + 1) % totalThrows;

            //...

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