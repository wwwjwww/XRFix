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
/*     void Update()
*     {
*         timer+=Time.deltaTime;
* 
*         if (!instantiate_gobj && timer >= timeLimit)
*         {
*            a9 = Instantiate(gobj9);
*             timer = 0;
*             instantiate_gobj = true;
*         }
*         if (instantiate_gobj && timer >= timeLimit )
*         {
*             var component9 = a9.AddComponent<HandManager>();
*             component9.CleanUpObject();
*             timer = 0;
*             instantiate_gobj = false;
*         }
* 
        * BUG: Transform object of Rigidbody in Update() methods
        * MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        *         rb2.transform.Rotate(0, 40, 0);
        *     }

        * Move this function in FixedUpdate() methods.
        * FIXED CODE:
        */

        
using UnityEngine;

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

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb2;
    private float startTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }

    void Update()
    {
        if (timer >= timeLimit)
        {
            timer -= timeLimit;
            if (!instantiate_gobj)
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/gobj9"));
                instantiate_gobj = true;
            }
            if (a9 == null)
            {
                a9 = GameObject.Find("gobj9 (1)");
            }
            gobj9 = a9.transform.Find("Cube").gameObject;
            rb2 = gobj9.GetComponent<Rigidbody>();
            startTime = Time.time;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                lifeguardAnim.SetTrigger("Die");
                Instantiate(Resources.Load<GameObject>("Prefabs/a9"), transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 acceleration = Vector3.zero;
        acceleration += CalculateHandDrag();
        acceleration += CalculateBoatDrag();
        rb.AddForce(acceleration * Time.deltaTime, ForceMode.VelocityChange);
        rb2.AddForce(acceleration * Time.deltaTime, ForceMode.VelocityChange);
        speedReadout.text = rb.velocity.ToString("F2");
        speedReadout2.text = rb2.velocity.ToString("F2");
    }

    Vector3 CalculateHandDrag()
    {
        Vector3 acceleration = Vector3.zero;
        Vector3 currentLeftPosition = leftHand.localPosition;
        Vector3 currentRightPosition = rightHand.localPosition;

        float deltaLeft = (currentLeftPosition - lastLeftPosition).magnitude;
        float deltaRight = (currentRightPosition - lastRightPosition).magnitude;

        acceleration += (currentLeftPosition - lastLeftPosition).normalized * (1 - handUpTime) * Mathf.Min(deltaLeft, handDeltaThreshold) * swimForceMultiplier;
        acceleration += (currentRightPosition - lastRightPosition).normalized * (1 - handUpTime) * Mathf.Min(deltaRight, handDeltaThreshold) * swimForceMultiplier;

        lastLeftPosition = currentLeftPosition;
        lastRightPosition = currentRightPosition;

        handUpTime = Mathf.Min(handUpTime + Time.deltaTime, 1);

        if (handUpTime >= 1)
        {
            handUp = false;
        }

        return acceleration;
    }

    Vector3 CalculateBoatDrag()
    {
        Vector3 acceleration = Vector3.zero;
        Vector3 boatPosition = boatRb.position;
        float distanceToBoat = Vector3.Distance(transform.position, boatPosition);

        if (distanceToBoat < boatDistanceThreshold)
        {
            acceleration += (transform.position - boatPosition).normalized * boatForceMultiplier;
            boatMotor.Play

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
