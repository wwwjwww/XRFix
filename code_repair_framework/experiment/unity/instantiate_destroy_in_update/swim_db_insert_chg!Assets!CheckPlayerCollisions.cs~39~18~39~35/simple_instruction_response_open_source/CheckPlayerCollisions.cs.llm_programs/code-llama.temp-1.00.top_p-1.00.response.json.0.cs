using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;

    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

    void Update() {
        if (ovrHand.IsTracked) {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
*     public void RemoveObject(){
*         Destroy(gameObject);
*     }

}

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.name);
        if (collision.name == "DeathZone")
        {
            deathText.SetActive(true);
            winText.SetActive(false);
        } else if (collision.name == "WinZone")
        {
            winText.SetActive(true);
            deathText.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "ForceField")
        {
            rb.AddForce(collision.transform.forward * ripForce);
        }
    }
}
