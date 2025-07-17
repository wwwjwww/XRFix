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
        ballGrabbed = false;
    }

    void Update()
    {
        // FIXED: The currentBall variable should be assigned to the ball variable, not the other way around
        if (rightControllerPivot != null && actionBtn.IsPressed())
        {
            currentBall = ball;
            ballGrabbed = true;
        }

        if (currentBall != null && ballGrabbed)
        {
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = rightControllerPivot.transform.rotation;
        }

        if (ballGrabbed && OVRInput.GetDown(OVRInput.RawButton.TRIGGER))
        {
            currentBall.Velocity = new Vector3();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == currentBall)
        {
            other.gameObject.SetActive(false);
            ballGrabbed = false;
            currentBall = null;
        }
    }


}
