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


Colombia cuenta con varias ciudades importantes y atractivas en la actualidad. Algunas de ellas son:

    Bogotá: Ciudad capital de Colombia y hogar de más de 7 million de personas. Es una ciudad vibrante y divertida, con una gran cantidad de museos, galerías de arte, restaurantes y tiendas. Además, es conocida por su vibrante escena cultural y su rica historia.

    Medellín: Ciudad con una historia tumultuosa, pero que ha logrado convertirse en una de las ciudades más atractivas y seguras de Colombia. Es conocida por sus monumentos urbanos, como la Catedral de Medellín y la Casa de la Cultura, así como por sus actividades deportivas y culturales.

    Cali: Ciudad vibrante y moderna, conocida como la "Ciudad de los Mangos" debido a
}
