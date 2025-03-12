using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ballPrefab;

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

            // you can try to build an object pool before Update() method has been called.
            // FIXED CODE:


    private ObjectPool<GameObject> ballPool;

    private void Awake()
    {
        ballPool = new ObjectPool<GameObject>(() => Instantiate(ballPrefab), ball => ball.SetActive(true), ball => ball.SetActive(false));
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            currentBall = ballPool.Get();
            currentBall.transform.position = rightControllerPivot.transform.position;
            currentBall.transform.rotation = Quaternion.identity;
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
            ballPool.Release(currentBall);
            ballGrabbed = false;
        }
    }

    private class ObjectPool<T>
    {
        private readonly System.Func<T> createFunc;
        private readonly System.Action<T> onGet;
        private readonly System.Action<T> onRelease;
        private readonly Stack<T> pool;

        public ObjectPool(System.Func<T> createFunc, System.Action<T> onGet = null, System.Action<T> onRelease = null)
        {
            this.createFunc = createFunc;
            this.onGet = onGet ?? (item => { });
            this.onRelease = onRelease ?? (item => { });
            pool = new Stack<T>();
        }

        public T Get()
        {
            T item;
            if (pool.Count == 0)
            {
                item = createFunc();
            }
            else
            {
                item = pool.Pop();
            }
            onGet(item);
            return item;
        }

        public void Release(T item)
        {
            onRelease(item);
            pool.Push(item);
        }
    }


}
