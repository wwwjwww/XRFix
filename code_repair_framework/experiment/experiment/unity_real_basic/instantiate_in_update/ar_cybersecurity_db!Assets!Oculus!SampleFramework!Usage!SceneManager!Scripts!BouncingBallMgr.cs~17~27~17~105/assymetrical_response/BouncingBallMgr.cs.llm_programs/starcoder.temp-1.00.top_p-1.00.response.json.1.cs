using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
	[SerializeField] private Transform trackingSpace;
	[SerializeField] private GameObject rightControllerPivot;
	[SerializeField] private OVRInput.RawButton actionBtn;
	[SerializeField] private GameObject rightControllerPivot;

    private GameObject currentBall;
    private bool ballGrabbed = false;




	private GameObject _currentBall;

	private bool _ballGrabbed;

	private void Awake()
	{
		_currentBall = ball;
		_ballGrabbed = false;
	}

	public void ResetBall()
	{
		ball.transform.position = trackingSpace.position;
		ball.transform.rotation = trackingSpace.rotation;
	}


}
