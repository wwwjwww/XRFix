using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ballPrefab;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    private Queue<GameObject> ballPool = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject ball = Instantiate(ballPrefab);
            ball.SetActive(false);
            ballPool.Enqueue(ball);
        }
    }

    private GameObject GetBallFromPool()
    {
        if (ballPool.Count > 0)
        {
            GameObject ball = ballPool.Dequeue();
            ball.SetActive(true);
            return ball;
        }
        else
        {
            GameObject newBall = Instantiate(ballPrefab);
            return newBall;
        }
    }

    private void ReturnBallToPool(GameObject ball)
    {
        ball.SetActive(false);
        ballPool.Enqueue(ball);
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            currentBall = GetBallFromPool();
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = Quaternion.identity;
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
            ReturnBallToPool(currentBall);
        }
    }


}
