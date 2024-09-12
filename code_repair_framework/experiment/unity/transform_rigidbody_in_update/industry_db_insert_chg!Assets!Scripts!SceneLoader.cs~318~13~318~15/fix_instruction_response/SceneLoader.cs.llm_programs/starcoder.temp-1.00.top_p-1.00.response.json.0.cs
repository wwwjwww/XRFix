using UnityEngine;

class SceneLoader : MonoBehaviour
{
    Rigidbody rb3;

    void Update()
    {
        rb3.transform.Translate(0, 0, Time.deltaTime * 2);
    }

    void FixedUpdate()
    {
        rb3.transform.Translate(0, 0, Time.fixedDeltaTime * 2);
    }

    void Awake()
    {
        rb3 = GetComponent<Rigidbody>();
    }

    }
}
