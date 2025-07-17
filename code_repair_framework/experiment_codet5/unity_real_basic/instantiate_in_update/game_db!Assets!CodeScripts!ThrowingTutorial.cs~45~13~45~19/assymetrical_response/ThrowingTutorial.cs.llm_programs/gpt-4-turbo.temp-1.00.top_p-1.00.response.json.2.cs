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
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        InitializeObjectPool();
    }

    private void Start()
    {
        readyToThrow = true;
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





    private Queue<GameObject> objectPool;

    private void InitializeObjectPool()
    {
        objectPool = new Queue<GameObject>();

        for (int i = 0; i < totalThrows; i++)
        {
            GameObject pooledObject = Instantiate(objectToThrow);
            pooledObject.SetActive(false);
            objectPool.Enqueue(pooledObject);
        }
    }

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
            audioManager.PlaySFX(audioManager.axeSound);

            GameObject thrownObject = objectPool.Dequeue();
            thrownObject.transform.position = attackPoint.position;
            thrownObject.transform.rotation = attackPoint.rotation;
            thrownObject.SetActive(true);
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset velocity before applying force
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
            throwCounter++;

            objectPool.Enqueue(thrownObject); // Recycle object back to pool after use
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