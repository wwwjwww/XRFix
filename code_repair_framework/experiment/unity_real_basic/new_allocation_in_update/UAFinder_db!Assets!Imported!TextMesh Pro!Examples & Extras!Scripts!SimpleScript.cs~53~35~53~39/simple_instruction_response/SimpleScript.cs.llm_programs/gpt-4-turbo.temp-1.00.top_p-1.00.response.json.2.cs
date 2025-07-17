using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

    private TextMeshPro m_textMeshPro;


    private const string label = "Frame: {0}";
    private float m_frame;


    void Start()
    {
        m_textMeshPro = GetComponent<TextMeshPro>();
    }



//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    void Update()
    {
        m_textMeshPro.SetText(label, m_frame % 1000);
        m_frame += 1 * Time.deltaTime;
    }



    }
}
