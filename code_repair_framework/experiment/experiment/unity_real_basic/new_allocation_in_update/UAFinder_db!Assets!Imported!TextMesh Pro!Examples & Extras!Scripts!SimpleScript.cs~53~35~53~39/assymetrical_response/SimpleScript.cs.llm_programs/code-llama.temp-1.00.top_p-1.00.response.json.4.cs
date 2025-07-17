/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }


Sure, here's the fixed code snippet:

//Here's the fixed code snippet:
using UnityEngine;
using System.Collections;

/* BUG: Using New() allocation in Update() method.
* MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
*         void Update()
*         {
*             m_textMeshPro.SetText(label, m_frame % 1000);
*             m_frame += 1 * Time.deltaTime;
*         }
//Here's the definition of function call in another component.
//Related code from /Library/PackageCache/com.unity.textmeshpro@2.0.1/Scripts/Runtime/TMP_Text.cs:
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

*         public void SetText(string text, float arg0, float arg1, float arg2)
*         {
*             int decimalPrecision = 0;
*             int index = 0;
* 
*             for (int i = 0; i < text.Length; i++)
*             {
*                 char c = text[i];
* 
*                 if (c == 123) // '{'
*                 {
* 
*                     if (text[i + 2] == 58) // ':'
*                     {
*                         decimalPrecision = text[i + 3] - 48;
*                     }
* 
*                     switch (text[i + 1] - 48)
*                     {
*                         case 0: // 1st Arg
*                             AddFloatToCharArray(arg0, ref index, decimalPrecision);
*                             break;
*                         case 1: // 2nd Arg
*                             AddFloatToCharArray(arg1, ref index, decimalPrecision);
*                             break;
*                         case 2: // 3rd Arg
*                             AddFloatToCharArray(arg2, ref index, decimalPrecision);
*                             break;
*                     }
* 
*                     if (text[i + 2] == 58)
*                         i += 4;
*                     else
*                         i += 2;
* 
*                     continue;
*                 }
*                 m_input_CharArray[index] = c;
*                 index += 1;
*             }
* 
*             m_input_CharArray[index] = (char)0;
*             m_charArray_Length = index; // Set the length to where this '0' termination is.
* 
*             #if UNITY_EDITOR
* 
*             m_text = new string(m_input_CharArray, 0, index);
*             #endif
* 
*             m_inputSource = TextInputSources.SetText;
*             m_isInputParsingRequired = true;
*             m_havePropertiesChanged = true;
*             m_isCalculateSizeRequired = true;

*             if(m_charArray_Length <= 0)
*             {
*                 m_charArray_Length = 1000;
*                 m_input_CharArray = new char[m_charArray_Length];
*             }
* 
*             SetVerticesDirty();
*             SetLayoutDirty();
*         }

* FIXED CODE:
*/

* Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.

void Start()
{
    m_input_CharArray = new char[1000];
}

// Here's the fix in the Update() method
void Update()
{
    m_textMeshPro.SetText(label, m_frame % 1000);
    m_frame += 1 * Time.deltaTime;
}

    }
}
