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


    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> ballPool;

    private void Awake()
    {
        ballPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ball);
            obj.SetActive(false);
            ballPool.Enqueue(obj);
        }
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn) && ballPool.Count > 0)
        {
            currentBall = ballPool.Dequeue();
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = Quaternion.identity;
            currentBall.SetActive(true);
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
            currentBall.SetActive(false);
            ballPool.Enqueue(currentBall);
            ballGrabbed = false;
            currentBall = null;
        }
    }


}
