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


    private Queue<GameObject> objectPool = new Queue<GameObject>();

    public int poolSize = 10;

    void Start() {
        for (int i = 0; i < poolSize; i++) {
            GameObject temp = Instantiate(launchObject);
            temp.SetActive(false);
            objectPool.Enqueue(temp);
        }
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            LaunchObject();
        }
    }

    void LaunchObject() {
        if (objectPool.Count > 0) {
            GameObject obj = objectPool.Dequeue();
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.SetActive(true);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(transform.forward * force, ForceMode.Impulse);
            }

            Launchable launchable = obj.GetComponent<Launchable>();
            if (launchable != null) {
                launchable.Player = player;
                launchable.button = button;
            }           
        }
    }

    public void ReturnToPool(GameObject obj) {
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }


}
