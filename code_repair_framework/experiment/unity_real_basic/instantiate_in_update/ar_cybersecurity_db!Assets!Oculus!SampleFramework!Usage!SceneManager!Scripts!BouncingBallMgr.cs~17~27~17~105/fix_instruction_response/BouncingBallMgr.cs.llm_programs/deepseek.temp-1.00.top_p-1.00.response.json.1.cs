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


    [SerializeField] private int poolSize; // The amount of balls in the pool

    private GameObject[] ballPool; // The pool of balls

    private int currentBallIndex; // The current ball index

    private void Awake()
    {
        ballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ballPool[i] = Instantiate(ball);
            ballPool[i].SetActive(false); // Deactivate the balls initially to save memory
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(actionBtn))
        {
            // Get the next inactive ball in the pool
            for (int i = 0; i < poolSize; i++)
            {
                // Use a circular rotation for the index
                currentBallIndex = (currentBallIndex + 1) % poolSize;
                if (!ballPool[currentBallIndex].activeInHierarchy)
                {
                    // Set the position and rotation before activating it
                    ballPool[currentBallIndex].transform.position = rightControllerPivot.transform.position;
                    ballPool[currentBallIndex].transform.rotation = Quaternion.identity;
                    ballPool[currentBallIndex].SetActive(true);
                    break;
                }
            }
        }

        // Check released balls
        if (OVRInput.GetUp(actionBtn))
        {
            // Get the current ball
            GameObject currentBall = ballPool[currentBallIndex];
            if (currentBall.activeInHierarchy)
            {
                currentBall.SetActive(false); // Deactivate the ball
                Vector3 ballPos = currentBall.transform.position;
                Quaternion rotation = currentBall.transform.rotation;
                // You can then call Release method on the ball object here based on how your code is set up
            }
        }
    }


}
