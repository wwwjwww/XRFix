// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }


//Here're the buggy code lines from /Assets/Imported/TextMesh Pro/Examples & Extras/Scripts/SimpleScript.cs:
using UnityEngine;
using System.Collections;

//Using New() allocation in Update() method.
void Update()
{
    //Move allocation outside of Update()
    m_textMeshPro.SetText(

    }
}
