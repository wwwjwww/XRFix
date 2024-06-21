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
        // Check if hands are above head
        CheckHandsAboveHead();

        // Control speed of boat
        ControlBoatSpeed();

        // Update speed readouts
        UpdateSpeedReadouts();
    }

    void FixedUpdate()
    {
        // Swim forward based on hand movement
        SwimForward();
    }

    void SwimForward()
    {
        // Calculate hand movement delta
        float leftHandDelta = Vector3.Distance(leftHand.localPosition, lastLeftPosition);
        float rightHandDelta = Vector3.Distance(rightHand.localPosition, lastRightPosition);

        // Calculate total hand movement delta
        float totalHandDelta = leftHandDelta + rightHandDelta;

        // Apply swim force to move character forward
        rb.AddForce(head.transform.forward * totalHandDelta * swimForceMultiplier);

        // Save current hand positions for next frame
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }

    void CheckHandsAboveHead()
    {
        // Check if hands are above head
        if (leftHand.localPosition.y > head.transform.localPosition.y && rightHand.localPosition.y > head.transform.localPosition.y)
        {
            if (!handUp)
            {
                handUp = true;
                lifeguardAnim.SetBool("HandUp", true);
                handUpTime = Time.time;
            }
            else
            {
                if (Time.time - handUpTime > handDeltaThreshold)
                {
                    // Implement action when hands are up for a certain time
                    // For example, signal lifeguard for help
                }
            }
        }
        else
        {
            handUp = false;
            lifeguardAnim.SetBool("HandUp", false);
        }
    }

    void ControlBoatSpeed()
    {
        // Calculate distance between player and boat
        float distanceToBoat = Vector3.Distance(transform.position, boat.transform.position);

        // Adjust boat speed based on distance
        if (distanceToBoat < boatDistanceThreshold)
        {
            float speedMultiplier = 1 - (distanceToBoat / boatDistanceThreshold);
            boatRb.AddForce(boat.transform.forward * boatForceMultiplier * speedMultiplier);
            if (!boatMotor.isPlaying)
            {
                boatMotor.Play();
            }
        }
        else
        {
            boatMotor.Stop();
        }
    }

    void UpdateSpeedReadouts()
    {
        // Update speed readouts with boat speed
        speedReadout.text = "Boat Speed: " + boatRb.velocity.magnitude.ToString("0.0") + " m/s";
        speedReadout2.text = "Boat Speed: " + boatRb.velocity.magnitude.ToString("0.0") + " m/s";
    }
}
