using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    private Queue<GameObject> objectPool;

    public int poolSize = 10;

    void Start() {
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(launchObject);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    void Update() {
        if (Input.GetButtonDown(button)) {
            if (objectPool.Count > 0) {
                GameObject temp = objectPool.Dequeue();
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

    public void ReturnToPool(GameObject returnedObject) {
        returnedObject.SetActive(false);
        objectPool.Enqueue(returnedObject);
    }


}
