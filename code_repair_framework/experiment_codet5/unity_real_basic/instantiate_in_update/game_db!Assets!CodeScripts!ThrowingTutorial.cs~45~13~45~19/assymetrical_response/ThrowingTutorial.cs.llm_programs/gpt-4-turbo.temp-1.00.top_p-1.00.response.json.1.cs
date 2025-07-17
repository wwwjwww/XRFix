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
    public float throwCooldown; // after each throw

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





    private List<GameObject> objectPool = new List<GameObject>();

    private void InitializeObjectPool()
    {
        for (int i = 0; i < totalThrows; i++)
        {
            GameObject obj = Instantiate(objectToThrow);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    private GameObject GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        return null;
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

        if (throwCounter < totalThrows)
        {
            audioManager.PlaySFX(audioManager.axeSound);

            GameObject thrownObject = GetPooledObject();
            if (thrownObject != null)
            {
                thrownObject.transform.position = attackPoint.position;
                thrownObject.transform.rotation = attackPoint.rotation;
                thrownObject.SetActive(true);

                Rigidbody rb = thrownObject.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                rb.AddForce(cam.forward * throwForce, ForceMode.VelocityChange);

                throwCounter++;
            }
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