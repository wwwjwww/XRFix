using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;

///     private void Update()
//     {
//         if (!ballGrabbed && OVRInput.GetDown(actionBtn))
//         {
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //             currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
            //             currentBall.transform.parent = rightControllerPivot.transform;
            //             ballGrabbed = true;
            //         }
            // 
            //         if (ballGrabbed && OVRInput.GetUp(actionBtn))
            //         {
            //             currentBall.transform.parent = null;
            //             var ballPos = currentBall.transform.position;
            //             var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            //             var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            //             currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            //             ballGrabbed = false;
            //         }
            //     }

            // FIXED CODE:


    [SerializeField] private int poolSize = 10; // set the initial size of the pool

    private GameObject[] ballPool; // for object pooling

    private int currentBallIndex = 0; // index for the next ball to be instantiated

    private void Start()
    {
        // Initialize the object pool
        ballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ballPool[i] = Instantiate(ball);
            ballPool[i].SetActive(false); // deactivate all balls in the pool initially
        }
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            // Grab the ball from the pool
            currentBallIndex = (currentBallIndex + 1) % poolSize; // cycle through the pool
            GameObject currentBall = ballPool[currentBallIndex];
            currentBall.SetActive(true);
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.parent = rightControllerPivot.transform;
            ballGrabbed = true;
        }

        if (ballGrabbed && OVRInput.GetUp(actionBtn))
        {
            currentBall.transform.parent = null;
            var ballPos = currentBall.transform.position;
            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            ballGrabbed = false;
        }
    }


}
