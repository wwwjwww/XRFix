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
        m_textMeshPro = GetComponent<TMPro.TMP_Text>();
    }



//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    private TMPro.TMP_Text m_textMeshPro;

    private string label = "Frame: {0:0.0}";

    void Update()
    {
        if (m_textMeshPro != null)
        {
            m_textMeshPro.SetText(label, m_frame % 1000);
            m_frame += 1 * Time.deltaTime;
        }
    }



    }
}
