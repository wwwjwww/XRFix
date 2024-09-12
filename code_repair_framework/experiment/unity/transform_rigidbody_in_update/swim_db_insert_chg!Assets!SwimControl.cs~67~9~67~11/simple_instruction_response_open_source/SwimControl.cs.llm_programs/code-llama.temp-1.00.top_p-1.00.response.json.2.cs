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




using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float forwardInput = Input.GetAxis("Vertical");
        float mouseInput = Input.GetAxis("Mouse X");

        // Update the speed readout
        float speed = rb.velocity.magnitude;
        speedReadout.text = "Speed: " + speed.ToString("F2");

        // Swimming
        if (horizontalInput != 0 || forwardInput != 0)
        {
            // Calculate the swimming force
            Vector3 swimForce = new Vector3(horizontalInput, 0, forwardInput);
            swimForce = transform.TransformDirection(swimForce);
            swimForce *= swimForceMultiplier;

            // Apply the swimming force
            rb.AddForce(swimForce, ForceMode.VelocityChange);
        }

        // Updating Hand Position
        if (handUp)
        {
            Vector3 handPos = head.transform.position;
            handPos.y += 1.3f;
            leftHand.position = Vector3.Lerp(leftHand.position, handPos, handDeltaThreshold);
            rightHand.position = Vector3.Lerp(rightHand.position, handPos, handDeltaThreshold);
        }
        else
        {
            leftHand.position = Vector3.Lerp(leftHand.position, lastLeftPosition, handDeltaThreshold);
            rightHand.position = Vector3.Lerp(rightHand.position, lastRightPosition, handDeltaThreshold);
        }

        // Boat Control
        if (Vector3.Distance(leftHand.position, rightHand.position) > boatDistanceThreshold)
        {
            boat.transform.position = Vector3.Lerp(leftHand.position, rightHand.position, 0.5f);
        }
        else
        {
            boat.transform.position = Vector3.Lerp(leftHand.position, rightHand.position, 0.5f);
        }

        // Hand Animations
        if (handUp)
        {
            lifeguardAnim.SetBool("isSwimming", true);
            lifeguardAnim.SetBool("isWaiting", false);
        }
        else
        {
            lifeguardAnim.SetBool("isSwimming", false);
            lifeguardAnim.SetBool("isWaiting", true);
        }

        // Boat Animations
        if (Vector3.Distance(leftHand.position, rightHand.position) > boatDistanceThreshold)
        {
            lifeguardAnim.SetBool("isRowing", true);
            lifeguardAnim.SetBool("isOnBoat", false);
        }
        else
        {
            lifeguardAnim.SetBool("isRowing", false);
            lifeguardAnim.SetBool("isOnBoat", true);
        }

        // Boat Movement
        if (handUp && Input.GetKeyDown(KeyCode.Mouse0) && instantiate_gobj == false)
        {
            gobj9 = Instantiate(a9) as GameObject;
            instantiate_gobj = true;
        }
        if (handUp && Input.GetKeyDown(KeyCode.Mouse0) && instantiate_gobj)
        {
            rb2 = boat.GetComponent<Rigidbody>();
            Vector3 boatPos = gobj9.transform.position;
            boatPos.y += 1f;
            boat.transform.position = boatPos;
        }
        if (handUp && Input.GetKeyUp(KeyCode.Mouse0) && instantiate_gobj)
        {
            instantiate_gobj = false;
            rb2 = null;
        }
        

        // Boat Speed
        float boatSpeed = boatRb.velocity.magnitude;
        speedReadout2.text = "Boat Speed: " + boatSpeed.ToString("F2");
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
