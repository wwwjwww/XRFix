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
        if (OVRInput.Get(OVRInput.Button.Two) &&!ballGrabbed)
        {
            GrabBall();
        }
        else if (OVRInput.GetUp(OVRInput.Button.Two) && ballGrabbed)
        {
            ReleaseBall();
        }
    }

    private void GrabBall()
    {
        currentBall = Instantiate(ball, rightControllerPivot.transform.position, rightControllerPivot.transform.rotation);
        currentBall.transform.SetParent(rightControllerPivot.transform);
        ballGrabbed = true;
    }

    private void ReleaseBall()
    {
        currentBall.transform.SetParent(trackingspace);
        Destroy(currentBall);
        ballGrabbed = false;
    }


}
