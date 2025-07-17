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


Aquí hay algunos de los sitios de YouTube más populares para aprender desarrollo web:

1. Codecademy: ofrece cursos gratuitos de programación en varios lenguajes de programación, incluyendo HTML, CSS y JavaScript.

2. Wes Bos: ofrece una gran cantidad de cursos de desarrollo web gratuitos, incluyendo Responsive Web Design, React y JavaScript.

3. FreeCodeCamp: una plataforma de aprendizaje gratuita que ofrece cursos en varias tecnologías de desarrollo web, incluyendo HTML, CSS, JavaScript y
}
