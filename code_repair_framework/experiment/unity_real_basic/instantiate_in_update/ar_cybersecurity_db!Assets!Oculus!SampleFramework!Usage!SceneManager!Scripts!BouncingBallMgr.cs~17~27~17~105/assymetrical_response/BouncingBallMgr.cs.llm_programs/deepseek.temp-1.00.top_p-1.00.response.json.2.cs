using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ballPrefab; // Assign a ball prefab to this field in the Inspector
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ballPrefab; // Assign a ball prefab to this field in the Inspector

    private GameObject currentBall;
    private bool ballGrabbed = false;




    void Update()
    {
        if (OVRInput.GetDown(actionBtn) && !ballGrabbed) // Create a new ball when the action button is pressed and a ball is not already being grabbed
        {
            currentBall = Instantiate(ballPrefab, rightControllerPivot.transform.position, rightControllerPivot.transform.rotation);
            ballGrabbed = true;
        }

        if (OVRInput.GetUp(actionBtn) && ballGrabbed) // When the action button is released, the ball is released
        {
            ballGrabbed = false;
        }

        if (ballGrabbed)  // While the ball is being grabbed, keep updating its position to the right controller's position
        {
            currentBall.transform.position = rightControllerPivot.transform.position;
        }
    }


}
