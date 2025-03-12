using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    void Start()
    {
        currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        bool buttonDown = OVRInput.GetDown(actionBtn);
        if (buttonDown)
        {
            currentBall.GetComponent<Rigidbody>().isKinematic = false;
            ballGrabbed = true;
        }
        else if (OVRInput.GetUp(actionBtn))
        {
            currentBall.GetComponent<Rigidbody>().isKinematic = true;
            ballGrabbed = false;
        }

        if (ballGrabbed)
        {
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = rightControllerPivot.transform.rotation;
        }
    }


}
