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

    // Build object pool
    for (int i = 0; i < totalThrows; i++)
    {
        GameObject obj = Instantiate(objectToThrow, attackPoint.position, attackPoint.rotation);
        obj.SetActive(false);
        objectPool.Add(obj);
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





private List<GameObject> objectPool = new List<GameObject>();

private void Update()
{
    if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
    {
        ThrowFromPool();
    }
} 

private void ThrowFromPool()
{
    readyToThrow = false;
    GameObject objToThrow = GetPooledObject();
    
    if (objToThrow != null)
    {
        audioManager.PlaySFX(audioManager.axeSound);
        objToThrow.transform.position = attackPoint.position;
        objToThrow.transform.rotation = attackPoint.rotation;
        objToThrow.SetActive(true);

        Rigidbody rb = objToThrow.GetComponent<Rigidbody>();
        rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);
        
        throwCounter++;
    }

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

private GameObject GetPooledObject()
{
    for (int i = 0; i < objectPool.Count; i++)
    {
        if (!objectPool[i].activeInHierarchy)
        {
            return objectPool[i];
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