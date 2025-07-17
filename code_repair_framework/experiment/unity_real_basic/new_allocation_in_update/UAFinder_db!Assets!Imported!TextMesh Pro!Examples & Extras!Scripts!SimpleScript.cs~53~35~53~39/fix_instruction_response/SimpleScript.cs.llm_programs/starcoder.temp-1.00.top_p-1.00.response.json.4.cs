// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }


Here's the fixed code:

// Related code from /Library/PackageCache/com.unity.textmeshpro@2.0.1/Scripts/Runtime/TMP_Text.cs:
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using

    }
}
