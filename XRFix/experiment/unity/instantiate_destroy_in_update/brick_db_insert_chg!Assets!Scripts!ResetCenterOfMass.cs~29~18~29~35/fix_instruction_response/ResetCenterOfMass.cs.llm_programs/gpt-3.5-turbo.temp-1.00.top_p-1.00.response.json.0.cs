using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    public GameObject gobj6;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;
    private GameObject a6;

    void Start()
    {
        a6 = null; // Initialize a6 as null
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a6 = Instantiate(gobj6);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a6);
            timer = 0;
            instantiate_gobj = false;
        }
    }
}
