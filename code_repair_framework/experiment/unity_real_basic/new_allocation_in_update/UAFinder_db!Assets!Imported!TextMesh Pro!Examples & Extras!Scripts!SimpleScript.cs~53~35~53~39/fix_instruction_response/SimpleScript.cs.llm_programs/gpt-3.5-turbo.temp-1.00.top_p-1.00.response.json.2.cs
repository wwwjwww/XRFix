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



private string m_text;

            private char[] m_input_CharArray = new char[8];

            private int m_charArray_Length = 8;

            private TextInputSources m_inputSource;

            private bool m_isInputParsingRequired;

            private bool m_havePropertiesChanged;

            private bool m_isCalculateSizeRequired;

            void Update()
            {
                SetText(label, m_frame % 1000, 0, 0);
                m_frame += 1 * Time.deltaTime;
            }

            public void SetText(string text, float arg0, float arg1, float arg2)
            {
                int decimalPrecision = 0;
                int index = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (c == 123)
                    {

                        if (text[i + 2] == 58)
                        {
                            decimalPrecision = text[i + 3] - 48;
                        }

                        switch (text[i + 1] - 48)
                        {
                            case 0:
                                AddFloatToCharArray(arg0, ref index, decimalPrecision);
                                break;
                            case 1:
                                AddFloatToCharArray(arg1, ref index, decimalPrecision);
                                break;
                            case 2:
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
                m_charArray_Length = index;

                m_text = new string(m_input_CharArray, 0, index);

                m_inputSource = TextInputSources.SetText;
                m_isInputParsingRequired = true;
                m_havePropertiesChanged = true;
                m_isCalculateSizeRequired = true;

                SetVerticesDirty();
                SetLayoutDirty();
            }


    }
}
