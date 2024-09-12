// FIXED CODE:

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

        private float timeLimit = 5f;
        private float timer = 0f;
        private bool instantiate_gobj = false;

        Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        private GameObject _instantiatedObject;

        void Start()
        {
            _instantiatedObject = new GameObject("VisualiseRayTraceObject");
            _instantiatedObject.AddComponent<VisualiseRayTrace>();
            _instantiatedObject.SetActive(false);
        }

        void Update()
        {
            timer += Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                _instantiatedObject.SetActive(true);
                _instantiatedObject.transform.position = transform.position;
                timer = 0;
                instantiate_gobj = true;
            }
            else if (instantiate_gobj && timer >= timeLimit)
            {
                _instantiatedObject.SetActive(false);
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
                    _samplePositions[j * s_steps + i] += transform.position;
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


        public static void DebugDrawCross(Vector3 pos, float r, Color col, float duration = 0f)
        {
            Debug.DrawLine(pos - Vector3.up * r, pos + Vector3.up * r, col, duration);
            Debug.DrawLine(pos - Vector3.right * r, pos + Vector3.right * r, col, duration);
            Debug.DrawLine(pos - Vector3.forward * r, pos + Vector3.forward * r, col, duration);
        }
    }
}


