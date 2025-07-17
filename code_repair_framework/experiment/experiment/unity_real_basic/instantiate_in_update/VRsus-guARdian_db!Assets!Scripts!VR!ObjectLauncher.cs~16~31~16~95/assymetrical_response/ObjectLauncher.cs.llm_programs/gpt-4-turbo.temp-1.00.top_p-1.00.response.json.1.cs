using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLauncher : MonoBehaviour {

    public GameObject player;
    public GameObject launchObject;
    public float force = 5;
    public string button = "Sprint";
	




    private Queue<GameObject> objectPool = new Queue<GameObject>();

    public int poolSize = 10;

    void Start() {
        for (int i = 0; i < poolSize; i++) {
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
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
            GameObject temp = objectPool.Dequeue();
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
            temp.SetActive(true);
            temp.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
            StartCoroutine(DeactivateAndEnqueue(temp));
        }
    }

    IEnumerator DeactivateAndEnqueue(GameObject launchedObject) {
        yield return new WaitForSeconds(3); // Adjust the duration as needed
        launchedObject.SetActive(false);
        objectPool.Enqueue(launchedObject);
    }


}
