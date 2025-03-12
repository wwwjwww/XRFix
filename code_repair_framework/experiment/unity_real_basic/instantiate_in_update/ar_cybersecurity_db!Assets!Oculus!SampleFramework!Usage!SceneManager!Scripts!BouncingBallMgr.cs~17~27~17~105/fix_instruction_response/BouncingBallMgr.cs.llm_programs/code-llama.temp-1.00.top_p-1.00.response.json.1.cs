using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;

///     private void Update()
//     {
//         if (!ballGrabbed && OVRInput.GetDown(actionBtn))
//         {
            // BUG: Instantiate in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
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


    private void Update()
    {
        // You can try to build an object pool before Update() method has been called.
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
            //     currentBall = Instantiate(ball, rightControllerPivot.transform.position, Quaternion.identity);
            //     currentBall.transform.parent = rightControllerPivot.transform;
            //     ballGrabbed = true;
        }

        // FIXED CODE:
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            // Use a pool to pre-instantiate balls before Update() method has been called.
            if (ballPool == null)
            {
                ballPool = new ObjectPool<GameObject>(ball, 3);
            }
            currentBall = ballPool.Get();
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

public class ObjectPool<T> where T : class
{
    private readonly List<T> _objectPool = new List<T>();

    public ObjectPool(T prefab, int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            _objectPool.Add(Instantiate(prefab));
        }
    }

    public T Get()
    {
        if (_objectPool.Count == 0)
        {
            return null;
        }
        return _objectPool[0];
    }

    public void Return(T obj)
    {
        if (_objectPool.Contains(obj))
        {
            _objectPool.Remove(obj);
        }
        _objectPool.Add(obj);
    }
}


}
