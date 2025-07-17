using UnityEngine;

public class BouncingBallMgr : MonoBehaviour
{
    [SerializeField] private Transform trackingspace;
    [SerializeField] private GameObject ball;
    [SerializeField] private OVRInput.RawButton actionBtn;
    [SerializeField] private GameObject ball;

    private GameObject currentBall;
    private bool ballGrabbed = false;




    private ObjectPool<GameObject> ballPool;

    private void Start()
    {
        ballPool = new ObjectPool<GameObject>(() => Instantiate(ball), 10);
    }

    private void Update()
    {
        if (!ballGrabbed && OVRInput.GetDown(actionBtn))
        {
            currentBall = ballPool.GetObject();
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
            ballGrabbed = false;

            ballPool.ReturnObject(currentBall);
            currentBall = null;
        }
    }

    private class ObjectPool<T>
    {
        private readonly Queue<T> objects = new Queue<T>();
        private readonly Func<T> createFunc;
        private readonly int initialCount;

        public ObjectPool(Func<T> createFunc, int initialCount)
        {
            this.createFunc = createFunc;
            this.initialCount = initialCount;

            for (int i = 0; i < initialCount; i++)
            {
                objects.Enqueue(createFunc());
            }
        }

        public T GetObject()
        {
            return objects.Count > 0 ? objects.Dequeue() : createFunc();
        }

        public void ReturnObject(T obj)
        {
            objects.Enqueue(obj);
        }
    }


}
