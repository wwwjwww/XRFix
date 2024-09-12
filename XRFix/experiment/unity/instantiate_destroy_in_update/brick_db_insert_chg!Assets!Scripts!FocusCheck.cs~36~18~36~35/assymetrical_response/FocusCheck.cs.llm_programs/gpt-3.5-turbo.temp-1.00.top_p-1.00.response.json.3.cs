/* Here're the buggy code lines from /Assets/Scripts/FocusCheck.cs:*/
﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;



    
    void Start()
    {
    }

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    
/* BUG: Instantiate/Destroy in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
*     void Update()
*     {
*         rb10.transform.Translate(4, 0, Time.deltaTime);
* 
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit){
*             a2 = Instantiate(gobj2);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit ){
*             Destroy(a2);
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
*         text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
*     }

* you can try to build an object pool before Update() method has been called.
* FIXED CODE:
*/

private void Awake()
    {
        rb10 = GetComponent<Rigidbody>(); // Initialize rb10 with the Rigidbody component of the GameObject
        gobj2 = new GameObject(); // Initialize gobj2 with a new GameObject
    }

    void Update()
    {
        rb10.transform.Translate(4 * Time.deltaTime, 0, 0); // Change vector arguments to a correct format
      
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2); // Instantiate gobj2
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2); // Destroy the instantiated object a2
            timer = 0;
            instantiate_gobj = false;
        }

        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }

}
