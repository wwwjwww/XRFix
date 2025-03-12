using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    private GameObject[] ballPool;

    private int poolSize = 10;

    private int poolIndex = 0;

    private void Awake()
    {
        // Initialize the object pool
        ballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            ballPool[i] = Instantiate(ball);
            ballPool[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            currentBall = GetPooledBall();
            if (currentBall != null)
            {
                currentBall.transform.position = rightControllerPivot.transform.position;
                currentBall.transform.rotation = Quaternion.identity;
                currentBall.transform.parent = rightControllerPivot.transform;
                currentBall.SetActive(true);
                ballGrabbed = true;
            }
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

    private GameObject GetPooledBall()
    {
        for (int i = 0; i < poolSize; i++)
        {
            poolIndex = (poolIndex + 1) % poolSize;
            if (!ballPool[poolIndex].activeInHierarchy)
            {
                return ballPool[poolIndex];
            }
        }
        return null;
    }


}
