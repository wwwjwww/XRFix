//        public void SetText(string text, float arg0, float arg1, float arg2)
//        {
//            int decimalPrecision = 0;
//            int index = 0;
//
//            for (int i = 0; i < text.Length; i++)
//            {
//                char c = text[i];
//
//                if (c == 123) // '{'
//                {
//
//                    if (text[i + 2] == 58) // ':'
//                    {
//                        decimalPrecision = text[i + 3] - 48;
//                    }
//
//                    switch (text[i + 1] - 48)
//                    {
//                        case 0: // 1st Arg
//                            AddFloatToCharArray(arg0, ref index, decimalPrecision);
//                            break;
//                        case 1: // 2nd Arg
//                            AddFloatToCharArray(arg1, ref index, decimalPrecision);
//                            break;
//                        case 2: // 3rd Arg
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
//            m_charArray_Length = index; // Set the length to where this '0' termination is.
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

// Please move this allocation before Update() method has been called or reuse existing heap allocation if possible.