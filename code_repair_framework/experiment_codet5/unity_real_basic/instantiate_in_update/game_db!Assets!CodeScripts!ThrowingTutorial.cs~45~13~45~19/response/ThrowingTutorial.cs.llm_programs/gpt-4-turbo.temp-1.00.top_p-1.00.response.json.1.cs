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
    }

    private void Start()
    {
        readyToThrow = true;
        InitializeObjectPool();
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





    private Queue<GameObject> objectPool; // Object pool for objects to throw

    public int poolSize = 10; // Adjustable pool size

    private void InitializeObjectPool()
    {
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToThrow);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
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

            GameObject thrownObject = GetObjectFromPool();
            thrownObject.transform.position = attackPoint.position;
            thrownObject.transform.rotation = attackPoint.rotation;
            thrownObject.SetActive(true);

            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // Reset velocity
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

    private GameObject GetObjectFromPool()
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            objectPool.Enqueue(obj); // Put it back to the pool so it's reusable later
            return obj;
        }
        else
        {
            // If the pool is empty, instantiate a new one and add it to the pool
            GameObject obj = Instantiate(objectToThrow);
            objectPool.Enqueue(obj);
            return obj;
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