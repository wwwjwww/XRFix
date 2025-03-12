using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;


        private const string label = "The <#0050FF>count is: </color>{0:2}";
    private float m_frame = 0;


    void Start()
    {
        
    }



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    void Update()
    {
        // Here's the fixed code using 'Text' function.
        m_textMeshPro.SetText(label, m_frame % 1000);
        m_frame += 1 * Time.deltaTime;
    }

    public TextMeshProUGUI m_textMeshPro;

    public string label;



    }
}
