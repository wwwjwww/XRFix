using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;


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



    private void Update()
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

    [SerializeField] private float TTL = 5.0f;

    [SerializeField] private AudioClip pop;

    [SerializeField] private AudioClip bounce;

    [SerializeField] private AudioClip loadball;

    [SerializeField] private Material visibleMat;

    [SerializeField] private Material hiddenMat;

    private AudioSource audioSource;

    private Transform centerEyeCamera;

    private bool isVisible = true;

    private float timer = 0f;

    private bool isReleased = false;

    private bool isReadyForDestroy = false;

    private void OnCollisionEnter() => audioSource.PlayOneShot(bounce);

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(loadball);
        centerEyeCamera = OVRManager.instance.GetComponentInChildren<OVRCameraRig>().centerEyeAnchor;
    }

    private void Update()
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

    private void UpdateVisibility()
    {
        Vector3 displacement = centerEyeCamera.position - this.transform.position;
        Ray ray = new Ray(this.transform.position, displacement);
        RaycastHit info;
        if (Physics.Raycast(ray, out info, displacement.magnitude))
        {
            if (info.collider.gameObject!= this.gameObject)
            {
                SetVisible(false);
            }
        }
        else
        {
            SetVisible(true);
        }
    }

    private void SetVisible(bool setVisible)
    {
        if (isVisible &&!setVisible)
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


}
