using UnityEngine;
using System.Collections.Generic;

namespace Crest
{
    public class VisualiseCollisionArea : MonoBehaviour
    {
        [SerializeField]
        float _objectWidth = 0f;

        float[] _resultHeights = new float[s_steps * s_steps];

        static readonly float s_radius = 5f;
        static readonly int s_steps = 10;

        protected GameObject gobj8;

        private float timeLimit = 5f;
        private float timer  = 0f;
        
        private Queue<GameObject> _objectPool = new Queue<GameObject>();
        
        Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        private void Start()
        {
            CreateObjectPool();
        }

        private void CreateObjectPool()
        {
            for (int i = 0; i < 10; i++) // Arbitrary pool size
            {
                var pooledObject = Instantiate(gobj8);
                pooledObject.SetActive(false);
                _objectPool.Enqueue(pooledObject);
            }
        }

        private GameObject GetPooledObject()
        {
            if (_objectPool.Count > 0)
            {
                var pooledObject = _objectPool.Dequeue();
                pooledObject.SetActive(true);
                return pooledObject;
            }
            else
            {
                var newObj = Instantiate(gobj8);
                return newObj;
            }
        }

        private void ReturnPooledObject(GameObject obj)
        {
            obj.SetActive(false);
            _objectPool.Enqueue(obj);
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                if (_objectPool.Count > 0)
                {
                    var obj8 = GetPooledObject();
                    var visualiseRayTrace = obj8.AddComponent<VisualiseRayTrace>();
                    visualiseRayTrace.FreeObject = () => ReturnPooledObject(obj8);
                }
                timer = 0;
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
            Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color);
            Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color);
        }

        public static void DebugDrawCross(Vector3 pos, float r, Color col, float duration = 0f)
        {
            Debug.DrawLine(pos - Vector3.up * r, pos + Vector3.up * r, col, duration);
            Debug.DrawLine(pos - Vector3.right * r, pos + Vector3.right * r, col, duration);
            Debug.DrawLine(pos - Vector3.forward * r, pos + Vector3.forward * r, col, duration);
        }
    }
}
