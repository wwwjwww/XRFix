using UnityEngine;

namespace Crest
{
    public class VisualiseCollisionArea : MonoBehaviour
    {
        [SerializeField]
        float _objectWidth = 0f;

        float[] _resultHeights;

        static readonly float s_radius = 5f;
        static readonly int s_steps = 10;

        protected GameObject gobj8;
        protected GameObject a8;

        private float timeLimit = 5f;
        private float timer  = 0f;
        private bool instantiate_gobj = false;

        Vector3[] _samplePositions;

        // Object pooling
        private ObjectPool<VisualiseRayTrace> _rayTracePool;

        private void Awake()
        {
            _rayTracePool = new ObjectPool<VisualiseRayTrace>(() => 
            {
                var go = Instantiate(gobj8);
                return go.AddComponent<VisualiseRayTrace>();
            }, poolSize: 5); // Arbitrary pool size, adjust as necessary
            _samplePositions = new Vector3[s_steps * s_steps];
            _resultHeights = new float[s_steps * s_steps];
        }

        private void Update()
        {
            timer+=Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a8 = _rayTracePool.Get();
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit )
            {
                VisualiseRayTrace obj8 = a8.GetComponent<VisualiseRayTrace>();
                obj8.FreeObject();
                _rayTracePool.Release(obj8);
                timer = 0;
                instantiate_gobj = false;
            }

            if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
            {
                return;
            }

            var collProvider = OceanRenderer.Instance.CollisionProvider;

            for (int i = 0; i < s_steps; i++)
            {
                for (int j = 0; j < s_steps; j++)
                {
                    _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);
                    _samplePositions[j * s_steps + i].x += transform.position.x;
                    _samplePositions[j * s_steps + i].z += transform.position.z;
                }
            }

            if (collProvider.RetrieveSucceeded(collProvider.Query(GetHashCode(), _objectWidth, _samplePositions, _resultHeights, null, null)))
            {
                for (int i = 0; i < s_steps; i++)
                {
                    for (int j = 0; j < s_steps; j++)
                    {
                        var result = _samplePositions[j * s_steps + i];
                        result.y = _resultHeights[j * s_steps + i];

                        DebugDrawCross(result, 1f, Color.green);
                    }
                }
            }
        }
    }

    // Simple object pool implementation
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly Func<T> _objectGenerator;
        private readonly int _maxSize;

        public ObjectPool(Func<T> objectGenerator, int poolSize)
        {
            _objectGenerator = objectGenerator;
            _maxSize = poolSize;
            for (int i = 0; i < _maxSize; i++)
            {
                var newObj = _objectGenerator();
                newObj.gameObject.SetActive(false);
                _pool.Enqueue(newObj);
            }
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                var obj = _pool.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                var newObj = _objectGenerator();
                _pool.Enqueue(newObj); // Add the new object to the pool for later use
                newObj.gameObject.SetActive(true);
                return newObj;
            }
        }

        public void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

        public static void DebugDrawCross(Vector3 pos, float r, Color col, float duration = 0f)
        {
            Debug.DrawLine(pos - Vector3.up * r, pos + Vector3.up * r, col, duration);
            Debug.DrawLine(pos - Vector3.right * r, pos + Vector3.right * r, col, duration);
            Debug.DrawLine(pos - Vector3.forward * r, pos + Vector3.forward * r, col, duration);
        }
    }
}
