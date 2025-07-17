using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;


// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    private void Update()
//    {
//        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
//        {
//            currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
//            currentBall.transform.parent = rightControllerPivot.transform;
//            ballGrabbed = true;
//        }
//
//        if (ballGrabbed && OVRInput.GetUp(actionBtn))
//        {
//            currentBall.transform.parent = null;
//            var ballPos = currentBall.transform.position;
//            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
//            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
//            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
//            ballGrabbed = false;
//        }
//    }



    [SerializeField] private float TTL = 5.0f;

    [SerializeField] private AudioClip pop;

    [SerializeField] private AudioClip bounce;

    [SerializeField] private AudioClip loadball;

    [SerializeField] private Material visibleMat;

    [SerializeField] private Material hiddenMat;

    private AudioSource audioSource;

    private Transform centerEyeCamera;

    private bool isVisible = true;

    private float timer = 0f;

    private bool isReleased = false;

    private bool isReadyForDestroy = false;

    private void OnCollisionEnter() => audioSource.PlayOneShot(bounce);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(loadball);
        centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
            currentBall.transform.parent = rightControllerPivot.transform;
            ballGrabbed = true;
        }

        if (ballGrabbed && OVRInput.GetUp(actionBtn))
        {
            currentBall.transform.parent = null;
            var ballPos = currentBall.transform.position;
            //Bug fix: Transform object of Rigidbody in Update() methods
            //Instead of using Update(), use FixedUpdate() to simulate real-world movement
            //var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            //var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            var vel = rigidbody.velocity;
            var angVel = rigidbody.angularVelocity;
            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            ballGrabbed = false;
        }
    }

    private void FixedUpdate()
    {
        if (ballGrabbed)
        {
        //Bug fix: Transform object of Rigidbody in FixedUpdate() methods
        //Instead of using FixedUpdate(), use Update() to simulate real-world movement
        //var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        //var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
        //currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
        //BallGrabbed = false;
        }
    }


}
