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


    private List<GameObject> tempObjects = new List<GameObject>();

    private int maxTempObjects = 10;

    void Start() 
    {
        for(int i = 0; i < maxTempObjects; i++)
        {
            GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            tempObjects.Add(temp);
            temp.GetComponent<Launchable>().Player = player;
            temp.GetComponent<Launchable>().button = button;
            temp.SetActive(false);
        }
    }

    void Update () 
    {
        if (Input.GetButtonDown(button))
        {
            foreach(var obj in tempObjects)
            {
                if(!obj.activeInHierarchy)
                {
                    obj.transform.position = transform.position;
                    obj.transform.rotation = transform.rotation;
                    obj.SetActive(true);
                    obj.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
                    break;
                }
            }
        }
    }


}
