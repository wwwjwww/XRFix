using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform _trackingspace;
    [SerializeField] private GameObject _rightControllerPivot;
    [SerializeField] private OVRInput.RawButton _actionBtn;
    [SerializeField] private GameObject _rightControllerPivot;

    private GameObject currentBall;
    private bool ballGrabbed = false;

///     private void Update()
//     {
//         if (!ballGrabbed && OVRInput.GetDown(actionBtn))
//         {
            //             currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
            //             currentBall.transform.parent = rightControllerPivot.transform;
            //             ballGrabbed = true;
            //         }
            // 
            //         if (ballGrabbed && OVRInput.GetUp(actionBtn))
            //         {
            //             currentBall.transform.parent = null;
            //             var ballPos = currentBall.transform.position;
            //             var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            //             var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            //             currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            //             ballGrabbed = false;
            //         }
            //     }

            // FIXED CODE:


    private GameObject _currentBall;

    private bool _ballGrabbed = false;

    private void Update()
    {
        if (!_ballGrabbed && OVRInput.GetDown(_actionBtn))
        {
            _currentBall = Instantiate(_ball, _rightControllerPivot.transform.position, Quaternion.identity);
            _currentBall.transform.parent = _rightControllerPivot.transform;
            _ballGrabbed = true;
        }

        if (_ballGrabbed && OVRInput.GetUp(_actionBtn))
        {
            _currentBall.transform.parent = null;
            var ballPos = _currentBall.transform.position;
            var vel = _trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            _currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
            _ballGrabbed = false;
        }
    }


}
