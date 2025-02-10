//        public void SetText(string text, float arg0, float arg1, float arg2)
//        {
//            int decimalPrecision = 0;
//            int index = 0;
//
//            for (int i = 0; i < text.Length; i++)
//            {
//                char c = text[i];
//
//                if (c == 123) 
//                {
//                    
//                    if (text[i + 2] == 58) 
//                    {
//                        decimalPrecision = text[i + 3] - 48;
//                    }
//
//                    switch (text[i + 1] - 48)
//                    {
//                        case 0: 
//                            AddFloatToCharArray(arg0, ref index, decimalPrecision);
//                            break;
//                        case 1: 
//                            AddFloatToCharArray(arg1, ref index, decimalPrecision);
//                            break;
//                        case 2: 
//                            AddFloatToCharArray(arg2, ref index, decimalPrecision);
//                            break;
//                    }
//
//                    if (text[i + 2] == 58)
//                        i += 4;
//                    else
//                        i += 2;
//
//                    continue;
//                }
//                m_input_CharArray[index] = c;
//                index += 1;
//            }
//
//            m_input_CharArray[index] = (char)0;
//            m_charArray_Length = index; 
//
//            #if UNITY_EDITOR
//            
//            m_text = new string(m_input_CharArray, 0, index);
//            #endif
//
//            m_inputSource = TextInputSources.SetText;
//            m_isInputParsingRequired = true;
//            m_havePropertiesChanged = true;
//            m_isCalculateSizeRequired = true;
//
//            SetVerticesDirty();
//            SetLayoutDirty();
//        }

// FIXED CODE:
