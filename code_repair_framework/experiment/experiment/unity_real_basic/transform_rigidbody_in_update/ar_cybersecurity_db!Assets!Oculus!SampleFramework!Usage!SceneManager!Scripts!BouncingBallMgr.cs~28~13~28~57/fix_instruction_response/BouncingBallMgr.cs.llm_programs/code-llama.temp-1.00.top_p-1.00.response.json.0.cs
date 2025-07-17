using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject rightControllerPivot;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;


// BUG: Transform object of Rigidbody in Update() methods
// MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
//    private void Update()
//    {
//        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
//        {
//            currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
//            currentBall.transform.parent = rightControllerPivot.transform;
//            ballGrabbed = true;
//        }
//
//        if (ballGrabbed && OVRInput.GetUp(actionBtn))
//        {
//            currentBall.transform.parent = null;
//            var ballPos = currentBall.transform.position;
//            var vel = trackingspace.rotation * OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
//            var angVel = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
//            currentBall.GetComponent<BouncingBallLogic>().Release(ballPos, vel, angVel);
//            ballGrabbed = false;
//        }
//    }



void FixedUpdate()
  {
    if (!ballGrabbed && OVRInput.GetDown(actionBtn))
    {
      currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
      currentBall.transform.parent = rightControllerPivot.transform;
      ballGrabbed = true;
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

  void FixedUpdate()
  {
    if (!isReleased) return;
    UpdateVisibility();
    timer += Time.deltaTime;
    if (!isReadyForDestroy && timer >= TTL)
    {
      isReadyForDestroy = true;
      float clipLength = pop.length;
      audioSource.PlayOneShot(pop);
      StartCoroutine(PlayPopCallback(clipLength));
    }
  }

  void UpdateVisibility()
  {
    Vector3 displacement = centerEyeCamera.position - this.transform.position;
    Ray ray = new Ray(this.transform.position, displacement);
    RaycastHit info;
    if (Physics.Raycast(ray, out info, displacement.magnitude))
    {
      if (info.collider.gameObject != this.gameObject)
      {
        SetVisible(false);
      }
    }
    else
    {
      SetVisible(true);
    }
  }

  void SetVisible(bool setVisible)
  {
    if (isVisible && !setVisible)
    {
      GetComponent<MeshRenderer>().material = hiddenMat;
      isVisible = false;
    }

    if (!isVisible && setVisible)
    {
      GetComponent<MeshRenderer>().material = visibleMat;
      isVisible = true;
    }
  }

  public void Release(Vector3 pos, Vector3 vel, Vector3 angVel)
  {
    isReleased = true;
    transform.position = pos;
    GetComponent<Rigidbody>().isKinematic = false;
    GetComponent<Rigidbody>().velocity = vel;
    GetComponent<Rigidbody>().angularVelocity = angVel;
  }


}
