using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;


        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;


    void Start()
    {
        // Get the TextMeshPro component
        m_textMeshPro = GetComponent<TextMeshPro>();
    }



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    string label;

    int m_frame;

    TextMeshPro m_textMeshPro;

    void Update()
    {
        // Set the text property using SetText() method with the parameters
        m_textMeshPro.SetText(label, m_frame % 1000);
        m_frame += 1 * Time.deltaTime;
    }



    }
}
