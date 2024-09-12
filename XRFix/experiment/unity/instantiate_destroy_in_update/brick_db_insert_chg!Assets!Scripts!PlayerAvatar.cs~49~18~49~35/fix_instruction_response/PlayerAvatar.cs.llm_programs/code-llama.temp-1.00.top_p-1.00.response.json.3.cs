using UnityEngine;
using UnityEngine.AI;

public class PlayerAvatar : MonoBehaviour
{
    public GameObject target;
    public float speed = 5.0f;

    private Transform targetTransform;
    private NavMeshAgent agent;

    void Start()
    {
        targetTransform = target.GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null && agent != null)
        {
            Vector3 targetPos = targetTransform.position + new Vector3(0, 0, -1);
            agent.SetDestination(targetPos);
        }
    }
}

