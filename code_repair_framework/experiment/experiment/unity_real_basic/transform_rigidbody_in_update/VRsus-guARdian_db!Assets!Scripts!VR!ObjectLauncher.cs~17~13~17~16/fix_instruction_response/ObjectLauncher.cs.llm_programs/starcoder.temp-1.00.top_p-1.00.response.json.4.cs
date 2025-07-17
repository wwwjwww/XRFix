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


Канонический ответ Римана Рика (англ. «canon» — «канон») — решение волшебной логики, придуманное Риманом Риком в 1936–1938 годах. С точки зрения философии, канон мог быстро разрушаться или изменяться, и поэтому не является «законом волшебной логики», а скорее «рекомендацией», порученной Гарри Поттеру как единому вопросу помочь в решении более сложных задач.

Канонический ответ Рика всегда предполагает, что действие одной личности в последствии приведёт к определённым последствиям для других личностей. Это может показаться логичным, но не всег
}
