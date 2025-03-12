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

    private int poolSize = 10;

    void Start() {
        // Initialize the object pool
        for (int i = 0; i < poolSize; i++) {
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
        if (objectPool.Count > 0) {
            GameObject temp = objectPool.Dequeue();
            temp.SetActive(true);
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;

            // Optional: Enqueue back to pool after some time
            StartCoroutine(RecycleObject(temp));
        }
    }

    IEnumerator RecycleObject(GameObject obj) {
        yield return new WaitForSeconds(5); // Adjust time as needed
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }


}
