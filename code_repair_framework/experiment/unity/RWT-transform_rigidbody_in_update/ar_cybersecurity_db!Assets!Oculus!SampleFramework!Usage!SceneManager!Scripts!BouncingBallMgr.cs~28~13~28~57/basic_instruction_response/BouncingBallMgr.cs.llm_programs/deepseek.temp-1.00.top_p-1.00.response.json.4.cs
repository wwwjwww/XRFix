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
        var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
        Release(currentBall, ballPos, vel, angVel);
        ballGrabbed = false;
    }
}

public void Release(GameObject ball, Vector3 pos, Vector3 vel, Vector3 angVel)
{
    ball.GetComponent<Rigidbody>().isKinematic = false;
    ball.GetComponent<Rigidbody>().velocity = vel;
    ball.GetComponent<Rigidbody>().angularVelocity = angVel;
}


}
