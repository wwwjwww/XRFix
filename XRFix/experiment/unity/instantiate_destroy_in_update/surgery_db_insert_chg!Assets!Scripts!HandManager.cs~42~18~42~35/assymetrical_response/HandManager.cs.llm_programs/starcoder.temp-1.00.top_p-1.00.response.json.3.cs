
/* Here're the buggy code lines from /Assets/Scripts/HandManager.cs:*/
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    protected GameObject gobj2;
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update() {
*         if (ovrHand.IsTracked) {
*             hand.transform.GetChild(0).gameObject.SetActive(true);
*             controller.SetActive(false);
*         } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
*             controller.SetActive(true);
*             hand.transform.GetChild(0).gameObject.SetActive(false);
*         }
*         rb1.transform.Rotate(30, 0, 0);
* 
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*             a2 = Instantiate(gobj2);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             var obj2 = a2.AddComponent<Slice>();
*             obj2.DisposeObj();
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
/* */
¡Claro! Aquí te presento una posible rutina de ejercicios para un cuerpo completo para los días laborables:

Martes:

8:00 a.m. - 9:00 a.m.: Caminar al menos 30 minutos o hacer una actividad física que te guste, como nadar, andar en bicicleta o andar en una máquina de ejercicio.

9:30 a.m. - 10:30 a.m.: Realizar un par de series de flexiones de brazos y tríceps (8-10 repeticiones cada serie).

10:30 a.m. - 11:00 a.m.: Descanso y técnica de refuerzo para los musculares empleados en la primera semana.

11:00 a.m. - 12:00 p.m.: Correr o caminar al menos 1 hora en una escala de velocidad media o lenta.

12:30 p.m. - 1:30 p.m.: Tomar un almuerzo saludable.

3:00 p.m. - 4:00 p.m.: Realizar un par de
    }
}
