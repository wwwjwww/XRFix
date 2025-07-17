// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }


using UnityEngine;
using System.Collections;

//Do not use New() allocation in Update() method. The following is an alternative implementation.
// void Update()
// {
//     m_textMeshPro.SetText(label, m_frame % 1000);
//     m_frame += Time.deltaTime;
// }

// Here's the definition of the function call in another component.

using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Text

    }
}
