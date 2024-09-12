using System.Collections;
using System.Collections.Generic;
using System.Linq;
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



    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }

/*     void Update() {
*         if (ovrHand.IsTracked) {
*             hand.transform.GetChild(0).gameObject.SetActive(true);
*             controller.SetActive(false);
*         } else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch)) {
*             controller.SetActive(true);
*             hand.transform.GetChild(0).gameObject.SetActive(false);
*         }
        * BUG: Transform object of Rigidbody in Update() methods
        * MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        *         rb1.transform.Rotate(30, 0, 0);
        * 
        *         timer+=Time.deltaTime;
        * 
        *         if (!instantiate_gobj && timer >= timeLimit)
        *         {
        *             a2 = Instantiate(gobj2);
        *             timer = 0;
        *             instantiate_gobj = true;
        *         }
        *         if (instantiate_gobj && timer >= timeLimit )
        *         {
        *             var obj2 = a2.AddComponent<Slice>();
        *             obj2.DisposeObj();
        *             timer = 0;
        *             instantiate_gobj = false;
        *         }
        * 

        * Move this function in FixedUpdate() methods.
        * FIXED CODE:
        */

        
Here is the fixed code:

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
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



        void Start()
        {
            ovrHand = hand.GetComponent<OVRHand>();
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= timeLimit)
            {
                instantiate_gobj = true;
                timer = 0f;
            }
        }

        // This method is called once per frame
        void FixedUpdate()
        {
            if (instantiate_gobj)
            {
                // Instantiate the GO here
                GameObject gobj = new GameObject("MyGO");
                gobj.transform.position = new Vector3(0f, 2f, 0f);

                // Get a reference to the Rigidbody component of the GO
                Rigidbody rb = gobj.GetComponent<Rigidbody>();

                // Apply a force to the Rigidbody
                rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);

                // Set the GO as a child of the hand
                gobj.transform.parent = hand.transform;
            }
        }
    }
}
