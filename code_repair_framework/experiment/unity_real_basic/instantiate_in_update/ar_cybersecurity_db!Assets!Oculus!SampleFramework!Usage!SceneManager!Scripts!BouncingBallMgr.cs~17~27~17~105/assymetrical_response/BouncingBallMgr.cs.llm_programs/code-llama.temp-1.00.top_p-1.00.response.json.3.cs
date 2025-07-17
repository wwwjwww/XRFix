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
        if (OVRInput.GetDown(actionBtn) && ballGrabbed == false)
        {
            currentBall = Instantiate(ball, trackingspace.position, trackingspace.rotation) as GameObject;
            currentBall.transform.parent = rightControllerPivot.transform;
            ballGrabbed = true;
        }
        else if (ballGrabbed == true)
        {
            currentBall.transform.position = trackingspace.position;
        }
    }


}
