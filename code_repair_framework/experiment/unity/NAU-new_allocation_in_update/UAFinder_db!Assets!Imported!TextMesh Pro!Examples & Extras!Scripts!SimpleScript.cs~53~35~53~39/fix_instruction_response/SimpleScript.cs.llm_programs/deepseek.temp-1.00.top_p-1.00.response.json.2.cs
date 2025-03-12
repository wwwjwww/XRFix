using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;


        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;


private void Start()
{
    m_builder = new StringBuilder(100); // Allocate enough space for the input
}



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



private StringBuilder m_builder;

void Update()
{
    m_builder.Length = 0; // Clear the StringBuilder every frame
    m_builder.Append(label);
    m_builder.Append(m_frame % 1000);
    m_textMeshPro.SetText(m_builder.ToString());
    m_frame += 1 * Time.deltaTime;
}



    }
}
