



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

        void Update()
        {
            timer+=Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a8 = Instantiate(gobj8);
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit )
            {
                var obj8 = a8.AddComponent<VisualiseRayTrace>();
                obj8.FreeObject();
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
                    /* BUG: Using New() allocation in Update() method.
                    * MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                    *                     _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);

                    * Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
                    * FIXED CODE:
                    */
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

        void Update()
        {
            timer+=Time.deltaTime;

            if (!instantiate_gobj && timer >= timeLimit)
            {
                a8 = Instantiate(gobj8);
                timer = 0;
                instantiate_gobj = true;
            }
            if (instantiate_gobj && timer >= timeLimit )
            {
                var obj8 = a8.AddComponent<VisualiseRayTrace>();
                obj8.FreeObject();
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
                    var samplePosition = new Vector3((j * _objectWidth) - (s_steps * _objectWidth / 2), 0, (i * _objectWidth) - (s_steps * _objectWidth / 2));
                    var sampleHeight = collProvider.SampleHeight(samplePosition, 0f);
                    _resultHeights[(i * s_steps) + j] = sampleHeight;
                }
            }

            var mesh = a8.GetComponent<MeshFilter>().mesh;

            if (mesh.vertices.Length!= _resultHeights.Length)
            {
                mesh.vertices = new Vector3[_resultHeights.Length];
                mesh.triangles = new int[(s_steps - 1) * (s_steps - 1) * 6];
            }

            for (int i = 0; i < s_steps; i++)
            {
                for (int j = 0; j < s_steps; j++)
                {
                    int vertIndex = (i * s_steps) + j;
                    var samplePosition = _samplePositions[vertIndex] = new Vector3((j * _objectWidth) - (s_steps * _objectWidth / 2), _resultHeights[vertIndex], (i * _objectWidth) - (s_steps * _objectWidth / 2));
                    mesh.vertices[vertIndex] = samplePosition;
                }
            }

            var triangleIndex = 0;
            for (int i = 0; i < (s_steps - 1); i++)
            {
                for (int j = 0; j < (s_steps - 1); j++)
                {
                    mesh.triangles[triangleIndex] = (i * s_steps) + j;
                    mesh.triangles[triangleIndex + 1] = (i * s_steps) + j + 1;
                    mesh.triangles[triangleIndex + 2] = ((i + 1) * s_steps) + j;

                    mesh.triangles[triangleIndex + 3] = ((i + 1) * s_steps) + j;
                    mesh.triangles[triangleIndex + 4] = (i * s_steps) + j + 1;
                    mesh.triangles[triangleIndex + 5] = ((i + 1) * s_steps) + j + 1;
                    triangleIndex += 6;
                }
            }

            a8.GetComponent<MeshRenderer>().material.SetFloat("_ObjectWidth", _objectWidth);
        }

        void OnDrawGizmosSelected()
        {
            if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
            {
                return;
            }

            var collProvider = OceanRenderer.Instance.CollisionProvider;

            for (int i = 0; i < s_steps; i++)
            {
                for (int j = 0; j < s_steps; j++)
                {
                    var samplePosition = new Vector3((j * _objectWidth) - (s_steps * _objectWidth / 2), 0, (i * _objectWidth) - (s_steps * _objectWidth / 2));
                    var sampleHeight = collProvider.SampleHeight(samplePosition, 0f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(samplePosition, new Vector3(_objectWidth, sampleHeight, _objectWidth));
                }
            }
        }
    }
}

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

        public static void DebugDrawCross(Vector3 pos, float r, Color col, float duration = 0f)
        {
            Debug.DrawLine(pos - Vector3.up * r, pos + Vector3.up * r, col, duration);
            Debug.DrawLine(pos - Vector3.right * r, pos + Vector3.right * r, col, duration);
            Debug.DrawLine(pos - Vector3.forward * r, pos + Vector3.forward * r, col, duration);
        }
    }
}
