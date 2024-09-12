using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FocusCheck : MonoBehaviour
{
    public TextMeshProUGUI text;

    protected Rigidbody rb10;

    protected GameObject gobj2;
    private GameObject a2;

    private float timeLimit = 5f;
    private float timer = 0f;

    private bool instantiate_gobj = false;

    void Start()
    {
    }



﻿    float timeLimit = 5f;
    bool instantiate_gobj = false;


    private void Update()
    {
        rb10.transform.Translate(4, 0, Time.deltaTime);
    
        timer += Time.deltaTime;
    
        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = Instantiate(gobj2);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a2);
            timer = 0;
            instantiate_gobj = false;
        }
    
        text.text = $"input: {OVRManager.hasInputFocus} - VR: {OVRManager.hasVrFocus}";
    }
}}

}
