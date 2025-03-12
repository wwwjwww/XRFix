using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




private Vector3 lastBallPosition;

    void Start()
    {
        OVRInput.updateSource(); // You need to update the input source
        lastBallPosition = ball.transform.position;
    }

    void Update()
    {
        OVRInput.updateSource();

        if (OVRInput.get(actionBtn)) // check if the action button is pressed
        {
            if (!ballGrabbed)
            {
                currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity); // Instantiate a new ball
                currentBall.transform.parent = trackingspace; // set new ball as a child of tracking space

                ballGrabbed = true;
            }
        }
        else
        {
            if (ballGrabbed)
            {
                ballGrabbed = false;
            }
        }

        if (ballGrabbed)
        {
            if (Vector3.Distance(currentBall.transform.position, lastBallPosition) < 0.01f)
            {
                Debug.Log("Ball is at the same position, bounce it!");
                // Code to bounce the ball
            }
            else
            {
                lastBallPosition = currentBall.transform.position;
            }
        }
    }


}
