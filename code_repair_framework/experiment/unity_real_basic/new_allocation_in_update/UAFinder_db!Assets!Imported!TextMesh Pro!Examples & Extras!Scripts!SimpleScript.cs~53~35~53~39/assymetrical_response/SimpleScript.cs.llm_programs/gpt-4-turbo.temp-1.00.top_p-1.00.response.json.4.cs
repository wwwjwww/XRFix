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
        m_frame = 0;
    }



/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }



    void Update()
    {
        m_textMeshPro.SetText(label, m_frame % 1000); // Reusing label string
        m_frame += 1 * Time.deltaTime;
    }



    }
}
