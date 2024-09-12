using System.Collections;
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

    




Here is the fixed code:

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

    

void Update()
    {
        // Check if the user is focused on the game object
        if(EventSystem.current.IsPointerOverGameObject())
        {
            a2 = a2.transform.GetChild(0);
            instantiate_gobj = true;
        }
        else
        {
            instantiate_gobj = false;
        }

        // Check if the game object has been instantiated
        if (instantiate_gobj)
        {
            // Update the timer
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                // Instantiate the prefab
                gobj2 = Instantiate(a2, transform.position, Quaternion.identity);

                // Reset the timer
                timer = 0f;
            }
        }
    }
}
}
