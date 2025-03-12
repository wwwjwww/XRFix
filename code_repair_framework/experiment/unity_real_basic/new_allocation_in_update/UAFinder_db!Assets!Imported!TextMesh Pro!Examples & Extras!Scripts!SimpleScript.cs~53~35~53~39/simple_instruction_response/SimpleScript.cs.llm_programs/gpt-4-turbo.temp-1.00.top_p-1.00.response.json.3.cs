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
        // Assuming m_textMeshPro is initialized somewhere before using it
    }



//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    private int m_frame;

    void Update()
    {
        m_textMeshPro.SetText("Frame: {0}", m_frame % 1000);
        m_frame += 1; // Increment in whole numbers since deltaTime is not needed for frame count
    }



    }
}
