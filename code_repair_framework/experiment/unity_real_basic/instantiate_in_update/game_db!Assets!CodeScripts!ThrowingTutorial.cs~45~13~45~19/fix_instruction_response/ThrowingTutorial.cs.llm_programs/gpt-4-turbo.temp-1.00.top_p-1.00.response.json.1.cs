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
        
        // Initialize the object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectToThrow);
            obj.SetActive(false);
            throwObjectPool.Enqueue(obj);
        }
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





    private Queue<GameObject> throwObjectPool = new Queue<GameObject>();

    private int poolSize = 10; // adjust pool size as needed

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
            
            GameObject thrownObject = throwObjectPool.Dequeue();
            thrownObject.SetActive(true);
            thrownObject.transform.position = attackPoint.position;
            thrownObject.transform.rotation = attackPoint.rotation;
            
            Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero; // reset velocity
            rb.angularVelocity = Vector3.zero; // reset angular velocity
            rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);

            throwObjectPool.Enqueue(thrownObject); // add the object back into the pool

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