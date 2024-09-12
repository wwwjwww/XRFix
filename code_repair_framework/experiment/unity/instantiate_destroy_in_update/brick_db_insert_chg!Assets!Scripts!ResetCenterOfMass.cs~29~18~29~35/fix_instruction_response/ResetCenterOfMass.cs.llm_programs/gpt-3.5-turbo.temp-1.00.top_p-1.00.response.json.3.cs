//Here's the fixed code for /Assets/Scripts/ResetCenterOfMass.cs:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    protected GameObject gobj6;
    protected GameObject a6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    private void Awake()
    {
        gobj6 = new GameObject(); // Initialize gobj6
    }

    void Start()
    {
        timer = timeLimit; // Start timer at timeLimit value
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = Instantiate(gobj6);
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a6);
            instantiate_gobj = false;
        }
    }
}
