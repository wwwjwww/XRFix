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
            var temp = Instantiate(launchObject, transform.position, transform.rotation);
            temp.transform.SetParent(transform);
            temp.transform.localPosition = Vector3.zero;
            temp.transform.localRotation = Quaternion.identity;

            var rigidbody = temp.AddComponent<Rigidbody>();
            rigidbody.AddForce(transform.forward * force, ForceMode.Impulse);

            var launchable = temp.AddComponent<Launchable>();
            launchable.player = player;
            launchable.button = button;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown(button))
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.transform.SetParent(player.transform);
            rigidbody.transform.localPosition = Vector3.up * 1.5f;
            rigidbody.transform.localRotation = Quaternion.identity;
            rigidbody.isKinematic = false;

            var collider = GetComponent<Collider>();
            collider.enabled = false;

            var launch = GetComponent<Launch>();
            launch.enabled = true;
        }
    }

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        rb.isKinematic = false;
    }

    void OnDisable()
    {
        rb.isKinematic = true;
    }

    void Update()
    {
        if (rb.transform.position.y < -5)
        {
            Destroy(rb.gameObject);
        }
    }


}
