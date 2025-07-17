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
        // Initialize m_textMeshPro in Start or Awake method
        m_textMeshPro = GetComponent<TextMeshPro>();

        // If label text is dynamic but reuses placeholders, consider initializing or updating outside the Update loop where possible
        label = "Some label text {0}";
    }



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    private string label = "Some label text {0}";

    private int m_frame;

    void Update()
    {
        // Update m_textMeshPro without using new allocation each frame
        m_textMeshPro.SetText(label, m_frame % 1000);
        m_frame++;
    }



    }
}
