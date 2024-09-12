using System.Collections.Generic;
using UnityEngine;

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
        protected GameObject a8;

        private float timeLimit = 5f;
        private float timer  = 0f;
        private bool instantiate_gobj = false;

        Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        private Queue<GameObject> objectPool = new Queue<GameObject>();

        private void Awake() {
            for(int i = 0; i < 10; i++) { // Pool size of 10, you can adjust this number as needed
                GameObject pooledObj = Instantiate(gobj8);
                pooledObj.SetActive(false);
                objectPool.Enqueue(pooledObj);
            }
        }
        
        private GameObject GetPooledObject() {
            if(objectPool.Count > 0) {
                GameObject pooledObj = objectPool.Dequeue();
                pooledObj.SetActive(true);
                return pooledObj;
            } else {
                // Optionally handle the case where the pool is empty
                // by creating a new object or returning null
                return Instantiate(gobj8);
            }
        }

        private void ReturnPooledObject(GameObject obj) {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
        
        void Update()
        {
            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a8 = GetPooledObject();
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit )
            {
                a8.GetComponent<VisualiseRayTrace>().FreeObject();
                ReturnPooledObject(a8);
                timer = 0;
                instantiate_gobj = false;
            }

            if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
            {
                return;
            }
            
            var collProvider = OceanRenderer.Instance.CollisionProvider;
            
            // Calculate sample positions
            for (int i = 0; i < s_steps; i++)
            {
                for (int j = 0; j < s_steps; j++)
                {
                    var index = j * s_steps + i;
                    _samplePositions[index] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);
                    _samplePositions[index].x += transform.position.x;
                    _samplePositions[index].z += transform.position.z;
                }
            }

            // Handle collision data
            if (collProvider.RetrieveSucceeded(collProvider.Query(GetHashCode(), _objectWidth, _samplePositions, _resultHeights, null, null)))
            {
                for (int i = 0; i < s_steps; i++)
                {
                    for (int j = 0; j < s_steps; j++)
                    {
                        var index = j * s_steps + i;
                        var result = _samplePositions[index];
                        result.y = _resultHeights[index];

                        DebugDrawCross(result, 1f, Color.green);
                    }
                }
            }
        }

        private void DebugDrawCross(Vector3 position, float size, Color color) {
            Debug.DrawLine(position + Vector3.up * size, position - Vector3.up * size, color);
            Debug.DrawLine(position + Vector3.right * size, position - Vector3.right * size, color);
            Debug.DrawLine(position + Vector3.forward * size, position - Vector3.forward * size, color);
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
