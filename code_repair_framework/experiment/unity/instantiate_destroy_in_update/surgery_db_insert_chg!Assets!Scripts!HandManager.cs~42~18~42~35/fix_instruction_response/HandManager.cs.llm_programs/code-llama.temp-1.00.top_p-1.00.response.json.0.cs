using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    public float handGrabDistance = 0.05f;
    public LayerMask grabLayers;
    public bool canGrab;

    void Start()
    {
        hand = GameObject.FindWithTag("Hand");
        grabLayers = LayerMask.GetMask("Default");
    }

    void Update()
    {
        if (canGrab)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 pos = hand.transform.position + (hand.transform.forward * handGrabDistance);
                RaycastHit hit;
                if (Physics.Raycast(pos, hand.transform.forward, out hit, handGrabDistance, grabLayers))
                {
                    GameObject grabbedObject = hit.transform.gameObject;
                    if (grabbedObject)
                    {
                        grabbedObject.transform.SetParent(hand.transform);
                        grabbedObject.transform.position = pos;
                        grabbedObject.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }
}
