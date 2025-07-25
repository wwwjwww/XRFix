﻿using UnityEngine;
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
                // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                //                 m_transform.position = m_initial_Position + new Vector3(x, z, y);

                //Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
                // FIXED CODE:
                using System.Collections;
public class ObjectSpin : MonoBehaviour
{
    void Update()
    {
        // BUG: Using New() allocation in Update() method.
        // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
        //                 m_transform.position = m_initial_Position + new Vector3(x, z, y);

        // Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.
        Vector3 newPosition = m_initial_Position + new Vector3(x, z, y);
        m_transform.position = newPosition;
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