using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    private Queue<GameObject> objectPool;

    private int poolSize = 10;

    void Start() {
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(launchObject);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void Update () {
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
            Rigidbody rb = temp.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.forward * force, ForceMode.Impulse);
            }
            Launchable launchable = temp.GetComponent<Launchable>();
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
