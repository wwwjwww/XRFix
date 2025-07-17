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



    public TextMeshProUGUI m_textMeshPro;

    public string label = "Label";

    public int m_frame = 0;

    void Update()
    {
        m_frame += 1 * Time.deltaTime;
        m_textMeshPro.SetText(label, m_frame % 1000);
    }

public class TMP_Text : TextMeshProUGUI
{
    public void SetText(string text, float arg0, float arg1, float arg2)
    {
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

        m_input_CharArray[index] = (char)0;
        m_charArray_Length = index; // Set the length to where this '0' termination is.

        #if UNITY_EDITOR

        m_text = new string(m_input_CharArray, 0, index);
        #endif

        m_inputSource = TextInputSources.SetText;
        m_isInputParsingRequired = true;
        m_havePropertiesChanged = true;
        m_isCalculateSizeRequired = true;

        SetVerticesDirty();
        SetLayoutDirty();
    }
}



    }
}
