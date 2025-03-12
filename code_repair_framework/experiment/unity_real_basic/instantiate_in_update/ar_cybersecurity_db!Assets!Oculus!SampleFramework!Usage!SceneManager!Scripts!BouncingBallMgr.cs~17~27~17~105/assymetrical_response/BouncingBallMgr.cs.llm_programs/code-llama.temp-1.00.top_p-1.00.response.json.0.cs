using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    void Update()
    {
        if (OVRInput.GetDown(actionBtn))
        {
            ballGrabbed = true;
            currentBall = ball;
            ball.transform.SetParent(rightControllerPivot);
            ball.transform.localPosition = Vector3.zero;
        }
        else if (OVRInput.GetUp(actionBtn))
        {
            ballGrabbed = false;
            currentBall.transform.SetParent(null);
        }

        if (ballGrabbed && rightControllerPivot.transform.position.y < ball.transform.position.y)
        {
            ball.GetComponent<Rigidbody>().AddForce(0, -100f, 0);
        }
    }


}
