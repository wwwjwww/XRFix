using UnityEngine;

namespace Crest
{
    public class VisualiseCollisionArea : MonoBehaviour
    {
        [SerializeField]
        private float _objectWidth = 0f;

        private float[] _resultHeights = new float[s_steps * s_steps];

        private static readonly float s_radius = 5f;
        private static readonly int s_steps = 10;

        protected GameObject gobj8;
        protected GameObject a8;

        private float timeLimit = 5f;
        private float timer = 0f;
        private bool instantiate_gobj = false;

        private Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        // Object pool for efficient instantiation
        private GameObjectPool gameObjectPool;

        // Start is called before the first frame update
        void Start()
        {
            gameObjectPool = new GameObjectPool(gobj8, 1); // Initialize with desired prefab and pool size
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                if (!instantiate_gobj)
                {
                    a8 = gameObjectPool.GetPooledObject();
                    instantiate_gobj = true;
                }
                else
                {
                    a8.GetComponent<VisualiseRayTrace>().FreeObject();
                    gameObjectPool.ReturnToPool(a8);
                    instantiate_gobj = false;
                }

                timer = 0f;
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

        private void DebugDrawCross(Vector3 position, float size, Color color)
        {
            Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color);
            Debug.DrawLine(position - Vector3.left * size, position + Vector3.left * size, color);
            Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color);
        }
    }

    // Simple object pool implementation
    class GameObjectPool
    {
        private GameObject prefab;
        private Queue<GameObject> pooledObjects;

        public GameObjectPool(GameObject prefab, int initialSize)
        {
            this.prefab = prefab;
            pooledObjects = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                AddObjectToPool();
            }
        }

        private void AddObjectToPool()
        {
            var newObject = GameObject.Instantiate(prefab);
            newObject.SetActive(false);
            pooledObjects.Enqueue(newObject);
        }

        public GameObject GetPooledObject()
        {
            if (pooledObjects.Count == 0)
            {
                AddObjectToPool();
            }

            var obj = pooledObjects.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void ReturnToPool(GameObject objectToReturn)
        {
            objectToReturn.SetActive(false);
            pooledObjects.Enqueue(objectToReturn);
        }
    }

    // Modified VisualiseRayTrace component
    // Assuming the script still resides at /Assets/Crest/Crest/Scripts/Collision/VisualiseRayTrace.cs

    public class VisualiseRayTrace : MonoBehaviour
    {
        RayTraceHelper _rayTrace = new RayTraceHelper(50f, 2f);

        public void FreeObject()
        {
            // This function will now be used to signal that the object can be returned to the pool
            // Pool manager or another component needs to handle actual return to the pool
            gameObject.SetActive(false);
        }
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
