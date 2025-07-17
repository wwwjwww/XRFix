using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5f;
    public string button = "Sprint";
	

/// 	void Update () {
// 		if (Input.GetButtonDown(button))
//         {
//             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate () {
        if (Input.GetButtonDown(button))
        {
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb)
            {
                // The launched object will inherit the momentum of the player
                rb.AddForce(playerRb.velocity * force, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("LaunchObject doesn't have a Rigidbody.");
            }
        }
    }


}
