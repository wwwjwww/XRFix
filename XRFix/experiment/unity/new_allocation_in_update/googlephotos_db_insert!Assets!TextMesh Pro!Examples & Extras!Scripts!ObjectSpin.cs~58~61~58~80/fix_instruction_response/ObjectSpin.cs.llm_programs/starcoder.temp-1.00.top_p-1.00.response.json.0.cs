using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class ObjectSpin : MonoBehaviour
    {

#pragma warning disable 0414

        public float SpinSpeed = 5;
        public int RotationRange = 15;
        private Transform m_transform;

        private float m_time;
        private Vector3 m_prevPOS;
        private Vector3 m_initial_Rotation;
        private Vector3 m_initial_Position;
        private Color32 m_lightColor;
        private int frames = 0;

        public enum MotionType { Rotation, BackAndForth, Translation };
        public MotionType Motion;

        void Awake()
        {
            m_transform = transform;
            m_initial_Rotation = m_transform.rotation.eulerAngles;
            m_initial_Position = m_transform.position;

            Light light = GetComponent<Light>();
            m_lightColor = light != null ? light.color : Color.black;
        }


        
        void Update()
        {
            if (Motion == MotionType.Rotation)
            {
                m_transform.Rotate(0, SpinSpeed * Time.deltaTime, 0);
            }
            else if (Motion == MotionType.BackAndForth)
            {
                m_time += SpinSpeed * Time.deltaTime;
                m_transform.rotation = Quaternion.Euler(m_initial_Rotation.x, Mathf.Sin(m_time) * RotationRange + m_initial_Rotation.y, m_initial_Rotation.z);
            }
            else
            {
                m_time += SpinSpeed * Time.deltaTime;

                float x = 15 * Mathf.Cos(m_time * .95f);
                float y = 10; 
                float z = 0f; 

                // BUG: Using New() allocation in Update() method.
                // MESSAGE: Update() method is called each frame. It's inefficient to allocate new resource using New() in Update() method.
                //                 m_transform.position = m_initial_Position + new Vector3(x, z, y);

                //Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
                // FIXED CODE:
                using UnityEngine;
using System.Collections;

public class ObjectSpin : MonoBehaviour
{
    #pragma warning disable 0649

    [SerializeField]
    private float m_SpinSpeed = 5;

    [SerializeField]
    private int m_RotationRange = 15;

    [SerializeField]
    private Color32 m_LightColor = Color.black;

    #pragma warning restore 0649

    private Transform m_Transform;

    private float m_Time;
    private Vector3 m_PrevPOS;
    private Vector3 m_Initial_Rotation;
    private Vector3 m_Initial_Position;

    private int m_Frames;

    public enum MotionType
    {
        Rotation,
        BackAndForth,
        Translation
    }

    public MotionType Motion { get; set; }

    private void Awake()
    {
        m_Transform = transform;
        m_Initial_Rotation = m_Transform.rotation.eulerAngles;
        m_Initial_Position = m_Transform.position;
    }

    private void Update()
    {
        if (Motion == MotionType.Rotation)
        {
            m_Transform.Rotate(Vector3.zero, m_SpinSpeed * Time.deltaTime);
        }
        else if (Motion == MotionType.BackAndForth)
        {
            m_Time += m_SpinSpeed * Time.deltaTime;
            m_Transform.rotation = Quaternion.Euler(m_Initial_Rotation.x, Mathf.Sin(m_Time) * m_RotationRange + m_Initial_Rotation.y, m_Initial_Rotation.z);
        }
        else
        {
            m_Time += m_SpinSpeed * Time.deltaTime;

            float x = 15 * Mathf.Cos(m_Time * 0.95f);
            float y = 10;
            float z = 0f;

            m_Transform.position = m_Initial_Position + new Vector3(x, z, y);
        }
    }
}

                // Drawing light patterns because they can be cool looking.
                //if (frames > 2)
                //    Debug.DrawLine(m_transform.position, m_prevPOS, m_lightColor, 100f);

                m_prevPOS = m_transform.position;
                frames += 1;
            }
        }
    }
}