using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button;
	

/// 	void Update () {
// 		if (Input.GetButtonDown(button))
//         {
//             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


    void FixedUpdate()
    {
        if (Input.GetButtonDown(button))
        {
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
        }
    }

    public GameObject Player;

    void OnMouseDown()
    {
        if (Player.GetComponent<PlayerController>().GetButtonDown(button))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Player.transform.forward * 20, ForceMode.Impulse);
        }
    }

    public string[] buttons = new string[] { "Jump", "Attack", "Sprint", "Interact" };

    public bool GetButtonDown(string btn)
    {
        return Input.GetButtonDown(btn);
    }


}
