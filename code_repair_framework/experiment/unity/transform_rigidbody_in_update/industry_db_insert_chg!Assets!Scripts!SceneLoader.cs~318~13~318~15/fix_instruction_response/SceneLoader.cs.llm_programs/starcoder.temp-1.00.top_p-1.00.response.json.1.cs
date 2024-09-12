using System;
using System.Collections;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    Rigidbody rb3;

    void Start()
    {
        rb3 = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb3.transform.Translate(Vector3.up * Time.deltaTime * 2);
    }

    public void OneShotEvent(Action ev)
    {
        StartCoroutine(RunOnce(ev));
    
    }
}
