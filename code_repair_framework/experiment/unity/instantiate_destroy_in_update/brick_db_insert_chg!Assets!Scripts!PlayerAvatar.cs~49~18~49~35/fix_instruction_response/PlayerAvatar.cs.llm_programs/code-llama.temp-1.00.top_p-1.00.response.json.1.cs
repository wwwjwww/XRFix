using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    public Transform Target;
    public float Distance = 1.0f;
    public float Speed = 1.0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 targetDirection = Target.position - transform.position;
        float targetDistance = targetDirection.magnitude;

        if (targetDistance < Distance)
        {
            rb.velocity = Vector3.zero;
        }
        else if (targetDistance > Distance)
        {
            targetDirection = Vector3.Normalize(targetDirection);
            rb.velocity = targetDirection * Speed * Time.deltaTime;
        }
    }
}

