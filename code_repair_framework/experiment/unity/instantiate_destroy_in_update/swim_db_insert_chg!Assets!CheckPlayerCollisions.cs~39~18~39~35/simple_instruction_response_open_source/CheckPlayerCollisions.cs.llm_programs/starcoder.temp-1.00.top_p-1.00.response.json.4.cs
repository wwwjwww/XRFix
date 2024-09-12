//Here're the buggy code lines from /Assets/CheckPlayerCollisions.cs:
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCollisions : MonoBehaviour
{
    private Rigidbody rb;
    public float ripForce = 50f;
    public GameObject deathText;
    public GameObject winText;
    public GameObject statusText;

    protected GameObject gobj7;
    protected GameObject a7;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb4;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HideStatus()
    {
        statusText.SetActive(false);
    }

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     private void Update()
*     {
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a7 = Instantiate(gobj7);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             var component7 = a7.AddComponent<HandManager>();
*             component7.RemoveObject();
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         rb4.transform.Rotate(10, 0, 0);
* 
*         if (Input.GetKeyDown(KeyCode.DownArrow))
*         {
*             ripForce -= 10;
*             statusText.SetActive(true);
*             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
*             Invoke("HideStatus", 2);
*         }
*         else if (Input.GetKeyDown(KeyCode.UpArrow))
*         {
*             ripForce += 10;
*             statusText.SetActive(true);
*             statusText.GetComponent<TMPro.TextMeshProUGUI>().text = "rip force set to " + ripForce;
*             Invoke("HideStatus", 2);
*         }
*     }
//*/
Aprender un lenguaje de programación es una excelente forma de desarrollar habilidades valiosas y aprender nuevas habilidades. Para comenzar, te recomiendo elegir un lenguaje de programación que se ajuste a tus objetivos y preferencias personales. Algunas consideraciones para elegir un lenguaje de programación pueden ser:

1. Comodidad: Considera elegir un lenguaje de programación que se adapte a tu estilo de codificación y que no sea complicado de aprender.

2. Oportunidades de trabajo: Investiga cuáles son las oportunidades de empleo en el campo de programación con ese lenguaje específico.

3. Oferta de recursos: Investiga la disponibilidad de recursos, como tutoriales, documentación y comunidades, para ayudarte a aprender el lenguaje de programación.

4. Propósito: Determina el propósito de qué

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collision with " + collision.name);
        if (collision.name == "DeathZone")
        {
            deathText.SetActive(true);
            winText.SetActive(false);
        } else if (collision.name == "WinZone")
        {
            winText.SetActive(true);
            deathText.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "ForceField")
        {
            rb.AddForce(collision.transform.forward * ripForce);
        }
    }
}
