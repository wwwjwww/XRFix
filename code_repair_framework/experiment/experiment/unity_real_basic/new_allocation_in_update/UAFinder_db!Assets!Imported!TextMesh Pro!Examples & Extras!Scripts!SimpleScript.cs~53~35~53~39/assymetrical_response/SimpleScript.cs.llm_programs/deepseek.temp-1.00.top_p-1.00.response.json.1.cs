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



/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }



void Setup()
{
    // Create the string once and store it in a variable
    label = "Your label goes here";

    // Create the TextMeshPro instance
    m_textMeshPro = new TMP_Text();
}

void Update()
{
    if (m_frame % 1000 == 0)
    {
        // Do not create a new string every frame, reuse the same one
        m_textMeshPro.SetText(label, m_frame % 1000, m_frame, Time.time);
    }

    m_frame += 1 * Time.deltaTime;
}



    }
}
