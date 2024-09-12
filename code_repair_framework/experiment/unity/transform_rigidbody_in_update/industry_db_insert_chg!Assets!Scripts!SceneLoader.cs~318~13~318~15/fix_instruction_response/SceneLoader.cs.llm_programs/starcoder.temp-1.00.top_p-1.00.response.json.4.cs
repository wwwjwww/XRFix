using System.Collections;
using UnityEngine;

public class Example : MonoBehaviour
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

    void FixedUpdate()
    {
        // Do something that does not change over time.
        // E.g. simulate physics, move gam
    }
}
