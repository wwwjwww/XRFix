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



// BUG: Using New() allocation in Update() method.
// MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
//        void Update()
//        {
//            m_textMeshPro.SetText(label, m_frame % 1000);
//            m_frame += 1 * Time.deltaTime;
//        }



void Update()
{
    // CHANGE: Allocate the TextMeshPro object once outside the Update() method to avoid unnecessary allocations.
    TextMeshPro m_textMeshPro = GetComponent<TextMeshPro>();

    // CHANGE: Use SetText() method instead of SetText() to avoid the bug.
    m_textMeshPro.SetText(label, m_frame % 1000);
    m_frame += 1 * Time.deltaTime;
}

public void SetText(string text, float arg0, float arg1, float arg2)
{
    // CHANGE: Use a local variable to store the decimal precision.
    int decimalPrecision = 0;
    int index = 0;

    for (int i = 0; i < text.Length; i++)
    {
        char c = text[i];

        if (c == 123) // '{'
        {

            if (text[i + 2] == 58) // ':'
            {
                decimalPrecision = text[i + 3] - 48;
            }

            switch (text[i + 1] - 48)
            {
                case 0: // 1st Arg
                    AddFloatToCharArray(arg0, ref index, decimalPrecision);
                    break;
                case 1: // 2nd Arg
                    AddFloatToCharArray(arg1, ref index, decimalPrecision);
                    break;
                case 2: // 3rd Arg
                    AddFloatToCharArray(arg2, ref index, decimalPrecision);
                    break;
            }

            if (text[i + 2] == 58)
                i += 4;
            else
                i += 2;

            continue;
        }
        m_input_CharArray[index] = c;
        index += 1;
    }

    // CHANGE: Use a local variable to store the length of the char array.
    int charArray_Length = index; // Set the length to where this '0' termination is.
     
    // CHANGE: Use a local variable to store the input string.
    string input_String = new string(m_input_CharArray, 0, index);
    
    // CHANGE: Use a local variable to store the text.
    string text_String = input_String;
    
    // CHANGE: Use a local variable to store the input source.
    TextInputSources inputSource = TextInputSources.SetText;
   
    // CHANGE: Use a local variable to store the is input parsing required.
    bool isInputParsingRequired = true;
    
    // CHANGE: Use a local variable to store the have properties changed.
    bool havePropertiesChanged = true;

    // CHANGE: Use a local variable to store the is calculate size required.
    bool isCalculateSizeRequired = true;

    // CHANGE: Use a local variable to store the char array length.
    m_charArray_Length = charArray_Length;

    // CHANGE: Use a local variable to store the text.
    m_text = text_String;
    
    // CHANGE: Use a local variable to store the input source.
    m_inputSource = inputSource;

    // CHANGE: Use a local variable to store the is input parsing required.
    m_isInputParsingRequired = isInputParsingRequired;

    // CHANGE: Use a local variable to store the have properties changed.
    m_havePropertiesChanged = havePropertiesChanged;

    // CHANGE: Use a local variable to store the is calculate size required.
    m_isCalculateSizeRequired = isCalculateSizeRequired;

    SetVerticesDirty();
    SetLayoutDirty();
}



    }
}
