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
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
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





    public GameObject pooledObject;

    public int pooledAmount;

    List<GameObject> pooledObjects;

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        // if there are no inactive objects in the pool, we instantiate a new one
        GameObject obj = Instantiate(pooledObject);
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

private void Throw()
{
    readyToThrow = false;

    GameObject thrownObject = pooler.GetPooledObject();
    Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
    rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
    thrownObject.transform.position = attackPoint.position;
    thrownObject.transform.rotation = attackPoint.rotation;
    thrownObject.SetActive(true); // activate the object before using it, to avoid activating it at frame 0

    throwCounter++;

    if (throwCounter == totalThrows)
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