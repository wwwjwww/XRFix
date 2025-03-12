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
//             GameObject temp = Instantiate(launchObject, transform.position, transform.rotation);
            // BUG: Transform object of Rigidbody in Update() methods
            // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
            //             temp.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //             temp.GetComponent<Launchable>().Player = player;
            //             temp.GetComponent<Launchable>().button = button;
            //         }
            // 	}

            // FIXED CODE:


This is intended for informational purposes only and is not a substitute for professional medical or mental health advice, diagnosis, or treatment.  Always consult with a qualified healthcare professional before making any significant changes to your diet, health routines, or medical treatments.

1. Admit that you have a problem.
2. Be ready to change.
3. Cut down on alcohol consumption.
4. Make a commitment to stopping drinking.
5. Face your feelings about drinking.
6. Get support from friends, family, and/or therapy.
7. Cut back on carbohydrates and sugars.
8. Exercise regularly.
9. Avoid caffeine, nicotine, and other stimulants.
10. Try to get enough sleep.
11. Keep a diary or log of your drinking.
1
}
