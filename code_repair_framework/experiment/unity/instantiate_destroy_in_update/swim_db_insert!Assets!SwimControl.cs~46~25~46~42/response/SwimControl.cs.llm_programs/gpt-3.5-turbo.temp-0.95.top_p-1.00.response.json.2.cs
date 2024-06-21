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
        // Instantiate gobj9 if not already instantiated
        if (gobj9 == null)
        {
            gobj9 = Instantiate(gobj9);
            rb2 = gobj9.GetComponent<Rigidbody>();
        }

        // Clean up the object
        var component9 = gobj9.GetComponent<HandManager>();
        component9.CleanUpObject();

        // Rotate rb2
        rb2.transform.Rotate(0, 40, 0);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        // Calculate hand movement velocity
        float leftHandDelta = Vector3.Distance(leftHand.localPosition, lastLeftPosition) / Time.deltaTime;
        float rightHandDelta = Vector3.Distance(rightHand.localPosition, lastRightPosition) / Time.deltaTime;

        // Check if hand movement is above threshold
        if (leftHandDelta > handDeltaThreshold || rightHandDelta > handDeltaThreshold)
        {
            handUpTime += Time.deltaTime;
            if (!handUp)
            {
                handUp = true;
                lifeguardAnim.SetTrigger("HandUp");
            }
        }
        else
        {
            handUp = false;
            handUpTime = 0;
        }

        // Update last hand positions
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;

        // Apply swim force to the swimmer
        Vector3 swimForce = new Vector3(0, 0, (leftHandDelta + rightHandDelta) * swimForceMultiplier);
        rb.AddRelativeForce(swimForce);

        // Check if swimmer is close enough to the boat to apply force
        float distanceToBoat = Vector3.Distance(transform.position, boat.transform.position);
        if (distanceToBoat < boatDistanceThreshold)
        {
            Vector3 boatForce = (boat.transform.position - transform.position).normalized * boatForceMultiplier;
            boatRb.AddForce(boatForce);
            // Enable boat motor sound
            if (!boatMotor.isPlaying)
            {
                boatMotor.Play();
            }
        }
        else
        {
            // Disable boat motor sound
            if (boatMotor.isPlaying)
            {
                boatMotor.Stop();
            }
        }

        // Update speed readout text
        speedReadout.text = "Swim Speed: " + Mathf.Round(rb.velocity.magnitude * 100) / 100 + " m/s";
        speedReadout2.text = "Boat Speed: " + Mathf.Round(boatRb.velocity.magnitude * 100) / 100 + " m/s";
    }
}
