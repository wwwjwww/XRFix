



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

