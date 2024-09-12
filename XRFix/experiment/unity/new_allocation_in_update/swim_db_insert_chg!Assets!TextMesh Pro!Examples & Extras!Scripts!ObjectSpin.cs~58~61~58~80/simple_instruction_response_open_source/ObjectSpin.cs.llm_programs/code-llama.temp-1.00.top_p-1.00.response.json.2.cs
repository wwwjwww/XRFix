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

                
                
using UnityEngine;
using UnityEngine.UI;

// FIXED CODE:
Transform m_transform = GetComponent<Transform>();
Quaternion m_initialRotation = m_transform.rotation;
Quaternion m_rotation;

// This function is not used, so it can be removed
void Awake()
{
    m_transform = transform;
    m_initial_Rotation = m_transform.rotation;
}

void Start()
{
    m_rotation = Quaternion.Euler(0, m_initial_Rotation.y, m_initial_Rotation.z);
}

void Update()
{
    m_time += SpinSpeed * Time.deltaTime;

    if (Motion == MotionType.Rotation)
    {
        m_transform.Rotate(0, SpinSpeed * Time.deltaTime, 0);
    }
    else if (Motion == MotionType.BackAndForth)
    {
        m_transform.rotation = Quaternion.Euler(m_initial_Rotation.x, Mathf.Sin(m_time) * RotationRange + m_initial_Rotation.y, m_initial_Rotation.z);
    }
    else
    {
        m_transform.rotation = m_rotation;
        m_rotation = Quaternion.Euler(
            x: 15 * Mathf.Cos(m_time * .95f),
            y: 10,
            z: 0f
        );
    }
}

// This variable is not used, so it can be removed
private float m_prevPOS;


                // Drawing light patterns because they can be cool looking.
                //if (frames > 2)
                //    Debug.DrawLine(m_transform.position, m_prevPOS, m_lightColor, 100f);

                m_prevPOS = m_transform.position;
                frames += 1;
            }
        }
    }
}