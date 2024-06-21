using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwimControl : MonoBehaviour
{
    public int swimForceMultiplier = 100;
    private Rigidbody rb;
    public Crest.SimpleFloatingObject sfo;
    public GameObject head;
    private float handUpTime = 0;
    private float handDeltaThreshold = .2f;
    public bool handUp = false;
    public GameObject boat;
    private Rigidbody boatRb;
    public int boatForceMultiplier = 5;
    public int boatDistanceThreshold = 5;
    public Animator lifeguardAnim;

    public Transform leftHand;
    public Transform rightHand;
    public TextMeshPro speedReadout;
    public TextMeshPro speedReadout2;

    private Vector3 lastLeftPosition;
    private Vector3 lastRightPosition;

    public AudioSource boatMotor;

    protected GameObject gobj9;

    protected Rigidbody rb2;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }
    void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a9 = Instantiate(gobj9);
        //         var component9 = a9.AddComponent<HandManager>();
        //         component9.CleanUpObject();
        //
        //         rb2.transform.Rotate(0, 40, 0);
        //     }

        // FIXED VERSION:
        // Instantiate/Destroy should be done in appropriate methods like Start() or OnTriggerEnter()

        // Example Fixed Version:
        // void Start()
        // {
        //     GameObject a9 = Instantiate(gobj9);
        //     var component9 = a9.AddComponent<HandManager>();
        //     component9.CleanUpObject();

        //     rb2.transform.Rotate(0, 40, 0);
        // }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed = rb.velocity.magnitude;
        speedReadout.text = "Speed: " + speed.ToString("F2") + " m/s";
        speedReadout2.text = "Speed: " + speed.ToString("F2") + " m/s";

        if (Vector3.Distance(transform.position, boat.transform.position) <= boatDistanceThreshold)
        {
            Vector3 direction = boat.transform.position - transform.position;
            boatRb.AddForce(direction.normalized * boatForceMultiplier);
            lifeguardAnim.SetBool("HandUp", false);
            handUpTime = 0;
            handUp = false;
        }
        else
        {
            sfo.AddForce(head.transform.up * Input.GetAxis("Mouse Y") * Time.deltaTime * swimForceMultiplier);

            if (Input.GetMouseButton(0))
            {
                if (!handUp)
                {
                    handUp = true;
                    handUpTime = Time.time;
                }

                if (Time.time - handUpTime > handDeltaThreshold)
                {
                    lifeguardAnim.SetBool("HandUp", true);
                    sfo.AddForce(head.transform.up * 10);
                }
            }
            else
            {
                lifeguardAnim.SetBool("HandUp", false);
                handUpTime = 0;
                handUp = false;
            }
        }

        Vector3 leftHandDelta = leftHand.localPosition - lastLeftPosition;
        Vector3 rightHandDelta = rightHand.localPosition - lastRightPosition;

        Vector3 avgHandDelta = (leftHandDelta + rightHandDelta) / 2;
        Vector3 torqueVector = new Vector3(avgHandDelta.y, -avgHandDelta.x, 0);

        rb.AddTorque(torqueVector * 10);

        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }
}
