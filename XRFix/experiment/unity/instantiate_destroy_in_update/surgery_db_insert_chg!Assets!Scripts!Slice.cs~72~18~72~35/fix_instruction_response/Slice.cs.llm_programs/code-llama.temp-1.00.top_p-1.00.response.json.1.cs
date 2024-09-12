using UnityEngine;

public class Slice : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 10.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * speed, ForceMode.Impulse);
        }
    }
}
