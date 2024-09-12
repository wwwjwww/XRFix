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

    void Start()
    {
        InvokeRepeating("InstantiateAndDestroyObject", 0, timeLimit);
    }

    void InstantiateAndDestroyObject()
    {
        if (!instantiate_gobj)
        {
            a6 = Instantiate(gobj6);
            instantiate_gobj = true;
        }
        else
        {
            Destroy(a6);
            instantiate_gobj = false;
        }
    }
}