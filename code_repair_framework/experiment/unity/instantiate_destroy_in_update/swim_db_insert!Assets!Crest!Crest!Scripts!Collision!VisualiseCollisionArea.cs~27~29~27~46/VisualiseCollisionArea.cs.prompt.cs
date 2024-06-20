// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEngine;

namespace Crest
{
    /// <summary>
    /// Debug draw crosses in an area around the GameObject on the water surface.
    /// </summary>
    public class VisualiseCollisionArea : MonoBehaviour
    {
        [SerializeField]
        float _objectWidth = 0f;

        float[] _resultHeights = new float[s_steps * s_steps];

        static readonly float s_radius = 5f;
        static readonly int s_steps = 10;

        protected GameObject gobj8;
        Vector3[] _samplePositions = new Vector3[s_steps * s_steps];

        void Update()
        {
            // BUG: Instantiate/Destroy in Update() method
            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
            //             GameObject a8 = Instantiate(gobj8);
            //             var obj8 = a8.AddComponent<VisualiseRayTrace>();
            //             obj8.FreeObject();
            // 
            //             if (OceanRenderer.Instance == null || OceanRenderer.Instance.CollisionProvider == null)
            //             {
            //                 return;
            //             }
            // 
            //             var collProvider = OceanRenderer.Instance.CollisionProvider;
            // 
            //             for (int i = 0; i < s_steps; i++)
            //             {
            //                 for (int j = 0; j < s_steps; j++)
            //                 {
            //                     _samplePositions[j * s_steps + i] = new Vector3(((i + 0.5f) - s_steps / 2f) * s_radius, 0f, ((j + 0.5f) - s_steps / 2f) * s_radius);
            //                     _samplePositions[j * s_steps + i].x += transform.position.x;
            //                     _samplePositions[j * s_steps + i].z += transform.position.z;
            //                 }
            //             }
            // 
            //             if (collProvider.RetrieveSucceeded(collProvider.Query(GetHashCode(), _objectWidth, _samplePositions, _resultHeights, null, null)))
            //             {
            //                 for (int i = 0; i < s_steps; i++)
            //                 {
            //                     for (int j = 0; j < s_steps; j++)
            //                     {
            //                         var result = _samplePositions[j * s_steps + i];
            //                         result.y = _resultHeights[j * s_steps + i];
            // 
            //                         DebugDrawCross(result, 1f, Color.green);
            //                     }
            //                 }
            //             }
            //         }

            // FIXED VERSION:
