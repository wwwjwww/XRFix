using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    private void Update()
    {
        if (OVRInput.GetDown(actionBtn, OVRInput.Controller.LTouch))
        {
            // Check if the ball is being grabbed and if the controller is being tracked by the gaze system
            if (ballGrabbed && trackingspace != null)
            {
                // Calculate the velocity of the ball based on the speed of the controller
                Vector3 velocity = trackingspace.transform.forward * 10f;

                // Apply the calculated velocity to the ball
                currentBall.GetComponent<Rigidbody>().velocity = velocity;
            }
        }
    }

    private void GrabBall()
    {
        // Check if the ball is already grabbed
        if (!ballGrabbed)
        {
            // Set the ball as grabbed
            ballGrabbed = true;
            // Get the current ball object
            currentBall = ball;
            // Set the ball as kinematic to prevent it from colliding with other objects
            ball.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void ReleaseBall()
    {
        // Set the ball as not grabbed
        ballGrabbed = false;
        // Set the ball as dynamic to allow it to collide with other objects
        ball.GetComponent<Rigidbody>().isKinematic = false;
    }


}
