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
        ballGrabbed = false;

        // Move the following lines of code to FixedUpdate to handle physics-based transformations
        currentBall.GetComponent<BouncingBallLogic>().PrepareForRelease(
            trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch),
            OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch)
        );
    }
}

private void FixedUpdate()
{
    if (currentBall != null && currentBall.transform.parent == null)
    {
        Vector3 ballPos = currentBall.transform.position;
        currentBall.GetComponent<BouncingBallLogic>().Release(
            ballPos,
            currentBall.GetComponent<BouncingBallLogic>().GetPreparedVelocity(),
            currentBall.GetComponent<BouncingBallLogic>().GetPreparedAngularVelocity()
        );
    }
}

    private Vector3 preparedVelocity;

    private Vector3 preparedAngularVelocity;

    public void PrepareForRelease(Vector3 vel, Vector3 angVel)
    {
        preparedVelocity = vel;
        preparedAngularVelocity = angVel;
    }

    public Vector3 GetPreparedVelocity()
    {
        return preparedVelocity;
    }

    public Vector3 GetPreparedAngularVelocity()
    {
        return preparedAngularVelocity;
    }

    public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
    {
        isReleased = true;
        transform.position = pos; // set the origin to match target
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = vel;
        rb.angularVelocity = angVel;
    }


}
