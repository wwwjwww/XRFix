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

   [Header("References")]
   public Transform cam;
    public float throwCooldown; //after each throw

   [Header("References")]
   public Transform cam;
   public float throwForce;

   bool readyToThrow;

   private int throwCounter;  //Counts throws
   private bool inCollider; //Tracks if player in collider

   AudioManager audioManager;

   private void Awake()
   {
      audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
   }

   private void Start()
   {
      readyToThrow = true;
   }


//    private void Update()
//    {
//        if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0)
//        {
//            Throw();
//        }
//    }





   public float throwCoodown; // after each throw

   private void Update()
   {
      if(inCollider && Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0) {
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
         armorObject.SetActive(false); //Make armor disappear
         audioManager.PlaySFX(audioManager.appearSound);
         swordObject.SetActive(true);//Make sword appear
      }
  
      totalThrows--;
  
  
      Invoke(nameof(ResetThrow), throwCoodown);
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