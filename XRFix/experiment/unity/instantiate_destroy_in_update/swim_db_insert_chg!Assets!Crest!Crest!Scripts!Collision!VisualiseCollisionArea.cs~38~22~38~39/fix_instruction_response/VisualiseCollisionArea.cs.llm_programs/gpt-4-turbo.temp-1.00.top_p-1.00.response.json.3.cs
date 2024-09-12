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

        Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        public Queue<GameObject> ObjectPool = new Queue<GameObject>();

        private void Awake()
        {
            // Pre-load some objects into the object pool
            for (int i = 0; i < 10; i++) // assuming you want a pool of 10 objects
            {
                GameObject pooledObj = Instantiate(gobj8);
                pooledObj.SetActive(false);
                ObjectPool.Enqueue(pooledObj);
            }
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (timer >= timeLimit)
            {
                if (!instantiate_gobj)
                {
                    if (ObjectPool.Count > 0)
                    {
                        a8 = ObjectPool.Dequeue();
                        a8.SetActive(true);
                        instantiate_gobj = true;
                    }
                }
                else
                {
                    var obj8 = a8.GetComponent<VisualiseRayTrace>() ??
                        a8.AddComponent<VisualiseRayTrace>();
                    ObjectPool.Enqueue(a8);
                    a8.SetActive(false);
                    instantiate_gobj = false;
                }

                timer = 0;
            }

            // Remaining code in Update() method (perform raycast etc.)
        }
    }
}

namespace Crest
{
    public class VisualiseRayTrace : MonoBehaviour
    {
        RayTraceHelper _rayTrace = new RayTraceHelper(50f, 2f);
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
