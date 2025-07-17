using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	

/// 	void Update () {
// 		if (Input.GetButtonDown(button))
//         {
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


    private Queue<GameObject> objectPool;

    void Start() {
        objectPool = new Queue<GameObject>();
        
        // Pre-instantiate a pool of objects
        for (int i = 0; i < 10; i++) {
            GameObject obj = Instantiate(launchObject);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            LaunchObject();
        }
    }

    void LaunchObject() {
        GameObject temp;

        if (objectPool.Count > 0) {
            temp = objectPool.Dequeue();
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.SetActive(true);
        } else {
            temp = Instantiate(launchObject, transform.position, transform.rotation);
            objectPool.Enqueue(temp);
        }
        
        Rigidbody rb = temp.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = Vector3.zero; // Reset velocity
            rb.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        Launchable launchable = temp.GetComponent<Launchable>();
        if (launchable != null) {
            launchable.Player = player;
            launchable.button = button;
        }

        StartCoroutine(DisableAfterTime(temp, 5f)); // Automatically disable after 5 seconds
    }

    IEnumerator DisableAfterTime(GameObject obj, float time) {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }


}
