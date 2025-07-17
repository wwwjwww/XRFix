using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;


    private string label;
    private float m_frame;


    private void Start()
    {
        m_frame = 0;
        m_textMeshPro = GetComponent<TextMeshProUGUI>();
    }



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    private TextMeshProUGUI m_textMeshPro;

    void Update()
    {
        m_textMeshPro.SetText(label, m_frame % 1000);
        m_frame += 1 * Time.deltaTime;
    }



    }
}
