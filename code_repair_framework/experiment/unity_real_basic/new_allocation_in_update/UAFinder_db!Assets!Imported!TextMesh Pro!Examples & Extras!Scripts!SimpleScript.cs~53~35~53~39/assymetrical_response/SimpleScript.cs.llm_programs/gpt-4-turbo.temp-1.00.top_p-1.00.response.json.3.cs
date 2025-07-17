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
        m_textMeshPro = GetComponent<TextMeshPro>();
        label = "Frame: {0}";
        m_frame = 0;

        // Preallocate the string format here to avoid repeated allocations
        cachedString = label;
    }



/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }



    private string label;

    private int m_frame;

    private string cachedString;

    void Update()
    {
        // Use the cached string format
        m_textMeshPro.SetText(cachedString, m_frame % 1000);
        m_frame += (int)(1 * Time.deltaTime);
    }



    }
}
