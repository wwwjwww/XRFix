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




using UnityEngine;
using UnityEngine.UI;

public class SwimControl : MonoBehaviour
{
    public int swimForceMultiplier = 100;
    private Rigidbody rb;
    public Crest.SimpleFloatingObject sfo;
    public GameObject head;
    private float handUpTime = 0;
    private float handDeltaThreshold =.2f;
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

    void Update()
    {
        // Read out the speed of the player
        Vector3 playerVelocity = rb.velocity;
        speedReadout.text = "Speed: " + playerVelocity.magnitude.ToString("F1");
        speedReadout2.text = "Speed: " + playerVelocity.magnitude.ToString("F1");

        // Set the handUp boolean
        handUp = (Vector3.Distance(leftHand.localPosition, lastLeftPosition) < handDeltaThreshold) &&
                 (Vector3.Distance(rightHand.localPosition, lastRightPosition) < handDeltaThreshold);
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;

        // Increase the handUpTime if handUp is true
        if (handUp) handUpTime += Time.deltaTime;
        else handUpTime = 0;

        // If handUpTime is greater than the handDeltaThreshold, set handUp to false
        if (handUpTime > handDeltaThreshold) handUp = false;

        // Apply force to the player based on whether they are holding onto something or not
        if (sfo == null && rb.velocity.magnitude <.1f)
        {
            rb.AddForce(Vector3.up * swimForceMultiplier, ForceMode.Acceleration);
        }
        else if (sfo!= null)
        {
            rb.AddForce(sfo.transform.up * swimForceMultiplier, ForceMode.Acceleration);
        }

        // Apply force to the boat if the player is near it and the boat is not moving
        if (boatDistanceThreshold > 0 && Vector3.Distance(transform.position, boat.transform.position) < boatDistanceThreshold && boatRb.velocity.magnitude <.1f)
        {
            boatRb.AddForce(Vector3.up * boatForceMultiplier, ForceMode.Acceleration);
            boatMotor.Play();
        }
        else if (boatDistanceThreshold <= 0)
        {
            boatMotor.Stop();
        }

        // Instantiate gobj9 if timeLimit is reached and gobj9 is not already instantiated
        if (!instantiate_gobj && Time.time > timer)
        {
            Instantiate(gobj9, new Vector3(15, 0, 0), Quaternion.identity);
            instantiate_gobj = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If the player enters the trigger collider of the lifeguard, trigger the lifeguard dialogue
        if (other.gameObject.CompareTag("Lifeguard"))
        {
            lifeguardAnim.SetTrigger("Enter");
        }
    }

    // FIXED CODE: Add the missing code below
    void OnTriggerStay(Collider other)
    {
        // If the player stays inside the trigger collider of the lifeguard, trigger the lifeguard dialogue
        if (other.gameObject.CompareTag("Lifeguard"))
        {
            lifeguardAnim.SetTrigger("Enter");
        }
    }
}


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
