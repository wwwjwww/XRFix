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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private GameObject[] ballPool;

    private int poolSize = 10;

    private int currentIndex = 0;

    private void Start()
    {
        // Initialize the ball pool
        ballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ballPool[i] = Instantiate(ball, Vector3.zero, Quaternion.identity);
            ballPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            // Activate a ball from the pool instead of instantiating a new one
            currentBall = ballPool[currentIndex];
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = Quaternion.identity;
            currentBall.transform.parent = rightControllerPivot.transform;
            currentBall.SetActive(true);
            ballGrabbed = true;

            // Update the pool index for the next use
            currentIndex = (currentIndex + 1) % poolSize;
        }

        if (ballGrabbed && OVRInput.GetUp(actionBtn))
        {
            currentBall.transform.parent = null;
            var ballPos = currentBall.transform.position;
            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            currentBall = null;
            ballGrabbed = false;
        }
    }


}
