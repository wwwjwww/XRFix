using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    protected GameObject gobj2;
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    // FIXED CODE:
    private ObjectPool objectPool; // Declare a new ObjectPool variable

    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
        objectPool = new ObjectPool(gobj2, 10); // Create a new ObjectPool and initialize it with the game object to be pooled (gobj2) and the max object count (10)
    }

    void Update()
    {
        if (ovrHand.IsTracked)
        {
            hand.transform.GetChild(0).gameObject.SetActive(true);
            controller.SetActive(false);
        }
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch))
        {
            controller.SetActive(true);
            hand.transform.GetChild(0).gameObject.SetActive(false);
        }

        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a2 = objectPool.GetObject(); // Get an object from the pool
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            var obj2 = a2.AddComponent<Slice>();
            obj2.DisposeObj();
            timer = 0;
            instantiate_gobj = false;
        }
    }

}
