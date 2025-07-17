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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private List<GameObject> objectPool;

    private int poolSize = 10; // Define the size of the pool

    void Start() {
        // Initialize the object pool
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(launchObject, transform.position, transform.rotation);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            GameObject temp = GetPooledObject();
            if (temp != null) {
                temp.transform.position = transform.position;
                temp.transform.rotation = transform.rotation;
                temp.SetActive(true);
                Rigidbody rb = temp.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.AddForce(transform.forward * force, ForceMode.Impulse);
                }
                Launchable launchable = temp.GetComponent<Launchable>();
                if (launchable != null) {
                    launchable.Player = player;
                    launchable.button = button;
                }
            }
        }
    }

    GameObject GetPooledObject() {
        foreach (GameObject obj in objectPool) {
            if (!obj.activeInHierarchy) {
                return obj;
            }
        }
        return null; // Return null if no inactive object is available
    }


}
