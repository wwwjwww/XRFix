
public class SwimControl : MonoBehaviour {
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
    private float timer = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb2;

    void Start() {
        rb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }

    void FixedUpdate() {
        timer += Time.deltaTime;
        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        // rb2.transform.Rotate(0, 40, 0);
    }

    void Update() {
        // Move this function in FixedUpdate() methods.
        // FIXED CODE:
        FixedUpdate();
    }
}

In the fixed code, the commented out code that was causing the error was moved to the FixedUpdate() method, which is called before Update() and has a fixed timestep of 1/50th of a second. This means that any code that needs to be affected by the physical laws of physics, such as moving an object in a realistic way, should be done in FixedUpdate().

Note that it's important to only call FixedUpdate() from within an update loop, not to call it as a free function

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
