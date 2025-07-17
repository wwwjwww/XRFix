using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




private void Start()
    {
        // Instantiate ball object at the start of the scene
        currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
        currentBall.transform.parent = rightControllerPivot.transform;
        currentBall.SetActive(false); // Deactivate the ball initially
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            // When action button is pressed, activate the ball
            currentBall.SetActive(true);
            ballGrabbed = true;
        }

        if (ballGrabbed && OVRInput.GetUp(actionBtn))
        {
            currentBall.transform.parent = null;
            var ballPos = currentBall.transform.position;
            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            currentBall.SetActive(false); // Deactivate the ball when released
            ballGrabbed = false;
        }
    }


}
