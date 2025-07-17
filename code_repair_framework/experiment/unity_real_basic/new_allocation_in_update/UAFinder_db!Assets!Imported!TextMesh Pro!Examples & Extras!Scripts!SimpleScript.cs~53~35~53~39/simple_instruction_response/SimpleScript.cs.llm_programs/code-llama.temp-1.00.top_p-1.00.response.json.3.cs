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

            m_textMeshPro = gameObject.AddComponent<TextMeshPro>();

            m_textMeshPro.autoSizeTextContainer = true;








            

            m_textMeshPro.fontSize = 48;

            m_textMeshPro.alignment = TextAlignmentOptions.Center;
            







            m_textMeshPro.enableWordWrapping = false; 


        }



//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



    public TMP_Textm_textMeshPro; // Initialize the text object as a property.

    public int m_frame;

    void Update()
    {
        m_textMeshPro.SetText(label, m_frame % 1000); // Send the label text to the text object.
        m_frame += 1 * Time.deltaTime; // Increment and display on the text object the current frame.
    }



    }
}
