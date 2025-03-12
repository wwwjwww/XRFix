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
        if (OVRInput.Get(actionBtn))
        {
            if (ballGrabbed)
            {
                currentBall.transform.parent = rightControllerPivot.transform;
                ballGrabbed = false;
            }
            else
            {
                currentBall.transform.parent = trackingspace;
                ballGrabbed = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ball)
        {
            currentBall = ball;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == ball)
        {
            currentBall = null;
        }
    }


}
