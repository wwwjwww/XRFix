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
    Invoke(nameof(BuildObjectPool), 0.1f);
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





private void BuildObjectPool()
{
    for (int i = 0; i < totalThrows; i++)
    {
        GameObject thrownObject = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
        thrownObject.SetActive(false);
    }
}

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

    GameObject pooledObject = GetPooledObject();
    if (pooledObject != null)
    {
        audioManager.PlaySFX(audioManager.axeSound);
        
        pooledObject.transform.position = attackPoint.position;
        pooledObject.transform.rotation = attackPoint.rotation;
        pooledObject.SetActive(true);

        Rigidbody rb = pooledObject.GetComponent<Rigidbody>();
        rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
    }
    
    totalThrows--;
    Invoke(nameof(ResetThrow), throwCooldown);
}

private GameObject GetPooledObject()
{
    for (int i = 0; i < totalThrows; i++)
    {
        if (!transform.GetChild(i).gameObject.activeInHierarchy)
        {
            return transform.GetChild(i).gameObject;
        }
    }
    return null;
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