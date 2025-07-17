using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    [SerializeField] private int poolSize = 10;

    private GameObject[] ballPool;

    private int ballPoolIndex = 0;

    private void Start()
    {
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
            int index = (ballPoolIndex + i) % poolSize;
            if (!ballPool[index].activeInHierarchy)
            {
                ballPoolIndex = index;
                return ballPool[index];
            }
        }
        return null;
    }


}
