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
    m_textMeshPro = new TMP_Text();
}



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



private TMP_Text m_textMeshPro;

void Update()
{
    m_textMeshPro.SetText(label, m_frame % 1000);
    mm_frame += 1 * Time.deltaTime;
}



    }
}
