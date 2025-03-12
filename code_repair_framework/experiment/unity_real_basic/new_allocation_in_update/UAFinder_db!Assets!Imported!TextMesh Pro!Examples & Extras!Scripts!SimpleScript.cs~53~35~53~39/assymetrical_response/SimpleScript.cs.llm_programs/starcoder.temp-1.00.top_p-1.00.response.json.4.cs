using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

    private TextMeshPro m_textMeshPro;  //!< Reference to the Text Mesh Pro text component.


        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;


    void Start()
    {
        // Find the Text Mesh Pro text component on the object.
        m_textMeshPro = GetComponent<TextMeshPro>();
    }



/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }



    private int m_frame = 0;              //!< Frame count used to increment the text.



    }
}
