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
    protected GameObject a9;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb2;

    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }




// R key respawns the player
// Left mouse button controls the force applied to the player (force is based on distance between hands)
// If left mouse button is held down for longer than "handDeltaThreshold", the player is pushed forward by the difference in hand positions (the distance between the hands) * "swimForceMultiplier"
// If the magnitude of the boat's velocity is greater than "boatDistanceThreshold", the player is pushed away from the boat by the difference in the boat's velocity and "boatForceMultiplier"
// Animator boolean "Swimming" is set based on if the left mouse button is down
// Audio source "boatMotor" plays based on the magnitude of the boat's velocity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimControl : MonoBehaviour
{
    // public variables for settings
    public int swimForceMultiplier = 100; // how much force is applied to the player when the left mouse button is pressed
    public int boatForceMultiplier = 5; // how much force is applied to the boat when it pushes the player away
    public int boatDistanceThreshold = 5; // how close to the boat does the boat need to

    // Update is called once per frame
    void FixedUpdate()
    {
        var leftVelocity = (leftHand.localPosition - lastLeftPosition).magnitude / Time.deltaTime;
        lastLeftPosition = leftHand.localPosition;
        var rightVelocity = (rightHand.localPosition - lastRightPosition).magnitude / Time.deltaTime;
        lastRightPosition = rightHand.localPosition;
        var combined_velocity = Mathf.Clamp(leftVelocity + rightVelocity, 0, 2);
        speedReadout.text = string.Format("{0:0.00} m/s", combined_velocity);
        speedReadout2.text = string.Format("{0:0.00} m/s", combined_velocity);
        sfo._raiseObject = combined_velocity / 2 + 1f;
        rb.AddForce(Camera.main.transform.forward * combined_velocity * swimForceMultiplier);
        var leftDelta = leftHand.localPosition.y - Camera.main.transform.localPosition.y;
        var rightDelta = rightHand.localPosition.y - Camera.main.transform.localPosition.y;
        if (leftDelta > handDeltaThreshold || rightDelta > handDeltaThreshold)
        {
            Debug.Log("Hand is up");
            if ((Time.time - handUpTime) > 1)
            {
                Debug.Log("hand was up for more than threshold");
                handUp = true;
                boatMotor.Play();
            }
        } else
        {
            handUpTime = Time.time;
        }
        if (handUp)
        {
            var distance = Vector3.Distance(transform.position, boat.transform.position);
            if (distance > boatDistanceThreshold)
            {
                var target = new Vector3(transform.position.x, boat.transform.position.y, transform.position.z + 5);
                boat.transform.LookAt(target);
                boatRb.AddForce(boat.transform.forward * boatForceMultiplier, ForceMode.Acceleration);
            } else
            {
                lifeguardAnim.SetTrigger("side");
            }
        }
    }
}
