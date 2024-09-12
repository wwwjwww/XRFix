



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
                    c#
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

        #region MonoBehaviour
        private void Start()
        {
            gobj8 = Resources.Load<GameObject>("8-poly");
        }

        private void Update()
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
                    _samplePositions[i * s_steps + j] =
                        Camera.main.ViewportToWorldPoint(new Vector3((j + 0.5f) / s_steps, 1f - (i + 0.5f) / s_steps, 10f));
                    _samplePositions[i * s_steps + j].y = collProvider.SamplePointDepth(
                        _samplePositions[i * s_steps + j], out _resultHeights[i * s_steps + j]);
                }
            }

            // Draw the height map as a texture
            var heightMap = new Texture2D(s_steps, s_steps, TextureFormat.RFloat, false);
            heightMap.filterMode = FilterMode.Point;
            heightMap.wrapMode = TextureWrapMode.Clamp;
            heightMap.SetPixels(_resultHeights);
            heightMap.Apply();

            // Create a mesh to display the texture as a plane
            var verts = new Vector3[s_steps * s_steps + 1];
            var uvs = new Vector2[s_steps * s_steps + 1];
            var indices = new int[(s_steps - 1) * (s_steps - 1) * 6];
            var offset = 0;

            verts[0] = new Vector3(-0.5f * _objectWidth, -0.5f * _objectWidth, 0f);
            uvs[0] = new Vector2(0f, 0f);
            for (int i = 0; i < s_steps; i++)
            {
                for (int j = 0; j < s_steps; j++)
                {
                    verts[offset + 1] = new Vector3((j - 0.5f * (s_steps - 1)) * _objectWidth, (i - 0.5f * (s_steps - 1)) * _objectWidth, 0f);
                    uvs[offset + 1] = new Vector2(j / (float)(s_steps - 1), i / (float)(s_steps - 1));
                    offset++;
                }
            }

            for (int i = 0; i < (s_steps - 1); i++)
            {
                for (int j = 0; j < (s_steps - 1); j++)
                {
                    int i1 = i * s_steps + j;
                    int i2 = i1 + s_steps;
                    int i3 = (i + 1) * s_steps + j;
                    int i4 = i3 + s_steps;
                    indices[i * (s_steps - 1) * 6 + j * 6 + 0] = 0;
                    indices[i * (s_steps - 1) * 6 + j * 6 + 1] = i2;
                    indices[i * (s_steps - 1) * 6 + j * 6 + 2] = i3;
                    indices[i * (s_steps - 1) * 6 + j * 6 + 3] = i2;
                    indices[i * (s_steps - 1) * 6 + j * 6 + 4] = i4;
                    indices[i * (s_steps -
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
