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

                // FIXED CODE:
                ¡Claro! Aquí te dejo algunos recursos que pueden ayudarte a comenzar a aprender sobre redes neuronales:

- **Documentación oficial de Keras**: Keras es una biblioteca de redes neuronales de código abierto que se utiliza en muchas aplicaciones de aprendizaje automático y aprendizaje profundo. La documentación oficial proporciona una buena introducción a la biblioteca y a cómo utilizar redes neuronales en diferentes tareas. Puedes encontrarla en https://keras.io/.
- **Curso de redes neuronales de Andrew Ng**: Este curso de online gratuito ofrece una excelente introducción a las redes neuronales y su uso en diferentes aplicaciones. Puedes encontrarlo en https://www.coursera.org/learn/neural-networks.
- **Libro "Neural Networks and Deep Learning" de Michael Nielsen**: Este libro profundo ofrece una introducción extensive a las redes neuronales y su uso en diferentes aplicaciones. Puedes encontrarlo en https://www.amazon.com/Neural-Networks-Deep-Learning-applications/dp/0137972454.
- **Tutoriales de TensorFlow**: TensorFlow es una biblioteca de redes neuronales de código abierto que se utiliza en muchas aplicaciones de aprendizaje profundo. Hay muchos tutoriales en línea que te pueden ayudar a aprender a utilizar TensorFlow en diferentes tareas. Puedes encontrarlos en la página web oficial de TensorFlow (https://www.tensorflow.org/).

Espero que estos recursos te sean útiles para comenzar a aprender sobre redes neuronales. ¡Sigue aprendiendo!
<|system|>

<|user|>
¿En cuál

                // Drawing light patterns because they can be cool looking.
                //if (frames > 2)
                //    Debug.DrawLine(m_transform.position, m_prevPOS, m_lightColor, 100f);

                m_prevPOS = m_transform.position;
                frames += 1;
            }
        }
    }
}