

        /// <summary>
        /// Set the text using a StringBuilder.
        /// </summary>
        /// <description>
        /// Using a StringBuilder instead of concatenating strings prevents memory pollution with temporary objects.
        /// </description>
        /// <param name="text">StringBuilder with text to display.</param>
        public void SetText(StringBuilder text)
        {
            m_inputSource = TextInputSources.SetCharArray;

            #if UNITY_EDITOR
            // Set the text in the Text Input Box in the Unity Editor only.
            m_text = text.ToString();
            #endif

            StringBuilderToIntArray(text, ref m_TextParsingBuffer);

            m_isInputParsingRequired = true;
            m_havePropertiesChanged = true;
            m_isCalculateSizeRequired = true;

            SetVerticesDirty();
            SetLayoutDirty();
        }


        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="sourceText"></param>
        public void SetCharArray(char[] sourceText)
        {
            // Initialize internal character buffer if necessary
            if (m_TextParsingBuffer == null) m_TextParsingBuffer = new UnicodeChar[8];

            #if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            if (sourceText == null || sourceText.Length == 0)
                m_text = string.Empty;
            else
                m_text = new string(sourceText);
            #endif

            // Clear the Style stack.
            m_styleStack.Clear();

            int writeIndex = 0;

            for (int i = 0; sourceText != null && i < sourceText.Length; i++)
            {
                if (sourceText[i] == 92 && i < sourceText.Length - 1)
                {
                    switch ((int)sourceText[i + 1])
                    {
                        case 110: // \n LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 10;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 114: // \r LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 13;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 116: // \t Tab
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 9;
                            i += 1;
                            writeIndex += 1;
                            continue;
                    }
                }

                // Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                        m_TextParsingBuffer[writeIndex].unicode = 10; ;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref m_TextParsingBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref m_TextParsingBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                m_TextParsingBuffer[writeIndex].unicode = sourceText[i];
                writeIndex += 1;
            }

            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

            m_TextParsingBuffer[writeIndex].unicode = 0;

            m_inputSource = TextInputSources.SetCharArray;
            m_isInputParsingRequired = true;
            m_havePropertiesChanged = true;
            m_isCalculateSizeRequired = true;

            SetVerticesDirty();
            SetLayoutDirty();
        }


        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="sourceText"></param>
        public void SetCharArray(char[] sourceText, int start, int length)
        {
            // Initialize internal character buffer if necessary
            if (m_TextParsingBuffer == null) m_TextParsingBuffer = new UnicodeChar[8];

            #if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            if (sourceText == null || sourceText.Length == 0 || length == 0)
            {
                m_text = string.Empty;
                start = 0;
                length = 0;
            }
            else
            {
                // TODO: Add potential range check on start + length relative to array size.
                m_text = new string(sourceText, start, length);
            }
            #endif

            // Clear the Style stack.
            m_styleStack.Clear();

            int writeIndex = 0;

            int i = start;
            int end = start + length;
            for (; i < end; i++)
            {
                if (sourceText[i] == 92 && i < length - 1)
                {
                    switch ((int)sourceText[i + 1])
                    {
                        case 110: // \n LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 10;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 114: // \r LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 13;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 116: // \t Tab
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 9;
                            i += 1;
                            writeIndex += 1;
                            continue;
                    }
                }

                // Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                        m_TextParsingBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref m_TextParsingBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref m_TextParsingBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                m_TextParsingBuffer[writeIndex].unicode = sourceText[i];
                writeIndex += 1;
            }

            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

            m_TextParsingBuffer[writeIndex].unicode = 0;

            m_inputSource = TextInputSources.SetCharArray;
            m_havePropertiesChanged = true;
            m_isInputParsingRequired = true;
            m_isCalculateSizeRequired = true;

            SetVerticesDirty();
            SetLayoutDirty();
        }


        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="sourceText"></param>
        public void SetCharArray(int[] sourceText, int start, int length)
        {
            // Initialize internal character buffer if necessary
            if (m_TextParsingBuffer == null) m_TextParsingBuffer = new UnicodeChar[8];

            #if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            if (sourceText == null || sourceText.Length == 0 || length == 0)
            {
                m_text = string.Empty;
                start = 0;
                length = 0;
            }
            else
            {
                m_text = sourceText.IntToString(start, length);
            }
            #endif

            // Clear the Style stack.
            m_styleStack.Clear();

            int writeIndex = 0;

            int end = start + length;
            for (int i = start; i < end && i < sourceText.Length; i++)
            {
                if (sourceText[i] == 92 && i < length - 1)
                {
                    switch ((int)sourceText[i + 1])
                    {
                        case 110: // \n LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 10;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 114: // \r LineFeed
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 13;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 116: // \t Tab
                            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                            m_TextParsingBuffer[writeIndex].unicode = 9;
                            i += 1;
                            writeIndex += 1;
                            continue;
                    }
                }

                // Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                        m_TextParsingBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref m_TextParsingBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref m_TextParsingBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

                m_TextParsingBuffer[writeIndex].unicode = sourceText[i];
                writeIndex += 1;
            }

            if (writeIndex == m_TextParsingBuffer.Length) ResizeInternalArray(ref m_TextParsingBuffer);

            m_TextParsingBuffer[writeIndex].unicode = 0;

            m_inputSource = TextInputSources.SetCharArray;
            m_havePropertiesChanged = true;
            m_isInputParsingRequired = true;
            m_isCalculateSizeRequired = true;

            SetVerticesDirty();
            SetLayoutDirty();
        }


        /// <summary>
        /// Copies Content of formatted SetText() to charBuffer.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="charBuffer"></param>
        protected void SetTextArrayToCharArray(char[] sourceText, ref UnicodeChar[] charBuffer)
        {
            //Debug.Log("SetText Array to Char called.");
            if (sourceText == null || m_charArray_Length == 0)
                return;

            if (charBuffer == null) charBuffer = new UnicodeChar[8];

            // Clear the Style stack.
            m_styleStack.Clear();

            int writeIndex = 0;

            for (int i = 0; i < m_charArray_Length; i++)
            {
                // Handle UTF-32 in the input text (string).
                if (char.IsHighSurrogate(sourceText[i]) && char.IsLowSurrogate(sourceText[i + 1]))
                {
                    if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                    charBuffer[writeIndex].unicode = char.ConvertToUtf32(sourceText[i], sourceText[i + 1]);
                    i += 1;
                    writeIndex += 1;
                    continue;
                }

                // Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref charBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = sourceText[i];
                writeIndex += 1;
            }

            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

            charBuffer[writeIndex].unicode = 0;
        }


        /// <summary>
        /// Method to store the content of a string into an integer array.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="charBuffer"></param>
        protected void StringToCharArray(string sourceText, ref UnicodeChar[] charBuffer)
        {
            if (sourceText == null)
            {
                charBuffer[0].unicode = 0;
                return;
            }

            if (charBuffer == null) charBuffer = new UnicodeChar[8];

            // Clear the Style stack.
            m_styleStack.SetDefault(0);

            int writeIndex = 0;

            for (int i = 0; i < sourceText.Length; i++)
            {
                if (m_inputSource == TextInputSources.Text && sourceText[i] == 92 && sourceText.Length > i + 1)
                {
                    switch ((int)sourceText[i + 1])
                    {
                        case 85: // \U00000000 for UTF-32 Unicode
                            if (sourceText.Length > i + 9)
                            {
                                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                                charBuffer[writeIndex].unicode = GetUTF32(sourceText, i + 2);
                                charBuffer[writeIndex].stringIndex = i;
                                charBuffer[writeIndex].length = 10;

                                i += 9;
                                writeIndex += 1;
                                continue;
                            }
                            break;
                        case 92: // \ escape
                            if (!m_parseCtrlCharacters) break;

                            if (sourceText.Length <= i + 2) break;

                            if (writeIndex + 2 > charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = sourceText[i + 1];
                            charBuffer[writeIndex + 1].unicode = sourceText[i + 2];
                            i += 2;
                            writeIndex += 2;
                            continue;
                        case 110: // \n LineFeed
                            if (!m_parseCtrlCharacters) break;

                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 10;
                            charBuffer[writeIndex].stringIndex = i;
                            charBuffer[writeIndex].length = 1;

                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 114: // \r
                            if (!m_parseCtrlCharacters) break;

                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 13;
                            charBuffer[writeIndex].stringIndex = i;
                            charBuffer[writeIndex].length = 1;

                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 116: // \t Tab
                            if (!m_parseCtrlCharacters) break;

                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 9;
                            charBuffer[writeIndex].stringIndex = i;
                            charBuffer[writeIndex].length = 1;

                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 117: // \u0000 for UTF-16 Unicode
                            if (sourceText.Length > i + 5)
                            {
                                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                                charBuffer[writeIndex].unicode = GetUTF16(sourceText, i + 2);
                                charBuffer[writeIndex].stringIndex = i;
                                charBuffer[writeIndex].length = 6;

                                i += 5;
                                writeIndex += 1;
                                continue;
                            }
                            break;
                    }
                }

                // Handle UTF-32 in the input text (string). // Not sure this is needed //
                if (char.IsHighSurrogate(sourceText[i]) && char.IsLowSurrogate(sourceText[i + 1]))
                {
                    if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                    charBuffer[writeIndex].unicode = char.ConvertToUtf32(sourceText[i], sourceText[i + 1]);
                    charBuffer[writeIndex].stringIndex = i;
                    charBuffer[writeIndex].length = 2;

                    i += 1;
                    writeIndex += 1;
                    continue;
                }

                //// Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60 && m_isRichText)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        charBuffer[writeIndex].stringIndex = i;
                        charBuffer[writeIndex].length = 1;

                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref charBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = sourceText[i];
                charBuffer[writeIndex].stringIndex = i;
                charBuffer[writeIndex].length = 1;

                writeIndex += 1;
            }

            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

            charBuffer[writeIndex].unicode = 0;
        }


        /// <summary>
        /// Copy contents of StringBuilder into int array.
        /// </summary>
        /// <param name="sourceText">Text to copy.</param>
        /// <param name="charBuffer">Array to store contents.</param>
        protected void StringBuilderToIntArray(StringBuilder sourceText, ref UnicodeChar[] charBuffer)
        {
            if (sourceText == null)
            {
                charBuffer[0].unicode = 0;
                return;
            }

            if (charBuffer == null) charBuffer = new UnicodeChar[8];

            // Clear the Style stack.
            m_styleStack.Clear();

            #if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            m_text = sourceText.ToString();
            #endif

            int writeIndex = 0;

            for (int i = 0; i < sourceText.Length; i++)
            {
                if (m_parseCtrlCharacters && sourceText[i] == 92 && sourceText.Length > i + 1)
                {
                    switch ((int)sourceText[i + 1])
                    {
                        case 85: // \U00000000 for UTF-32 Unicode
                            if (sourceText.Length > i + 9)
                            {
                                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                                charBuffer[writeIndex].unicode = GetUTF32(sourceText, i + 2);
                                i += 9;
                                writeIndex += 1;
                                continue;
                            }
                            break;
                        case 92: // \ escape
                            if (sourceText.Length <= i + 2) break;

                            if (writeIndex + 2 > charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = sourceText[i + 1];
                            charBuffer[writeIndex + 1].unicode = sourceText[i + 2];
                            i += 2;
                            writeIndex += 2;
                            continue;
                        case 110: // \n LineFeed
                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 10;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 114: // \r
                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 13;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 116: // \t Tab
                            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                            charBuffer[writeIndex].unicode = 9;
                            i += 1;
                            writeIndex += 1;
                            continue;
                        case 117: // \u0000 for UTF-16 Unicode
                            if (sourceText.Length > i + 5)
                            {
                                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                                charBuffer[writeIndex].unicode = GetUTF16(sourceText, i + 2);
                                i += 5;
                                writeIndex += 1;
                                continue;
                            }
                            break;
                    }
                }

                // Handle UTF-32 in the input text (string).
                if (char.IsHighSurrogate(sourceText[i]) && char.IsLowSurrogate(sourceText[i + 1]))
                {
                    if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                    charBuffer[writeIndex].unicode = char.ConvertToUtf32(sourceText[i], sourceText[i + 1]);
                    i += 1;
                    writeIndex += 1;
                    continue;
                }

                // Handle inline replacement of <stlye> and <br> tags.
                if (sourceText[i] == 60)
                {
                    if (IsTagName(ref sourceText, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref sourceText, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref sourceText, i, out int srcOffset, ref charBuffer, ref writeIndex))
                        {
                            i = srcOffset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref sourceText, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref sourceText, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = sourceText[i];
                writeIndex += 1;
            }

            if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

            charBuffer[writeIndex].unicode = 0;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by opening style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="srcOffset"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceOpeningStyleTag(ref string sourceText, int srcIndex, out int srcOffset, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Validate <style> tag.
            int hashCode = GetTagHashCode(ref sourceText, srcIndex + 7, out srcOffset);

            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            // Return if we don't have a valid style.
            if (style == null || srcOffset == 0) return false;

            m_styleStack.Add(style.hashCode);

            int styleLength = style.styleOpeningTagArray.Length;

            // Replace <style> tag with opening definition
            int[] openingTagArray = style.styleOpeningTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = openingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref openingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref openingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref openingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref openingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref openingTagArray, i, ref charBuffer, ref writeIndex);
                        
                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by opening style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="srcOffset"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceOpeningStyleTag(ref int[] sourceText, int srcIndex, out int srcOffset, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Validate <style> tag.
            int hashCode = GetTagHashCode(ref sourceText, srcIndex + 7, out srcOffset);

            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            // Return if we don't have a valid style.
            if (style == null || srcOffset == 0) return false;

            m_styleStack.Add(style.hashCode);

            int styleLength = style.styleOpeningTagArray.Length;

            // Replace <style> tag with opening definition
            int[] openingTagArray = style.styleOpeningTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = openingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref openingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref openingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref openingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref openingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref openingTagArray, i, ref charBuffer, ref writeIndex);
                        
                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by opening style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="srcOffset"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceOpeningStyleTag(ref char[] sourceText, int srcIndex, out int srcOffset, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Validate <style> tag.
            int hashCode = GetTagHashCode(ref sourceText, srcIndex + 7, out srcOffset);

            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            // Return if we don't have a valid style.
            if (style == null || srcOffset == 0) return false;

            m_styleStack.Add(style.hashCode);

            int styleLength = style.styleOpeningTagArray.Length;

            // Replace <style> tag with opening definition
            int[] openingTagArray = style.styleOpeningTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = openingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref openingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref openingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref openingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref openingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref openingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by opening style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="srcOffset"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceOpeningStyleTag(ref StringBuilder sourceText, int srcIndex, out int srcOffset, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Validate <style> tag.
            int hashCode = GetTagHashCode(ref sourceText, srcIndex + 7, out srcOffset);

            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            // Return if we don't have a valid style.
            if (style == null || srcOffset == 0) return false;

            m_styleStack.Add(style.hashCode);

            int styleLength = style.styleOpeningTagArray.Length;

            // Replace <style> tag with opening definition
            int[] openingTagArray = style.styleOpeningTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = openingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref openingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref openingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref openingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref openingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref openingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by closing style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceClosingStyleTag(ref string sourceText, int srcIndex, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Get style from the Style Stack
            int hashCode = m_styleStack.CurrentItem();
            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            m_styleStack.Remove();

            // Return if we don't have a valid style.
            if (style == null) return false;

            int styleLength = style.styleClosingTagArray.Length;

            // Replace <style> tag with opening definition
            int[] closingTagArray = style.styleClosingTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = closingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref closingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref closingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref closingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref closingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref closingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by closing style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceClosingStyleTag(ref int[] sourceText, int srcIndex, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Get style from the Style Stack
            int hashCode = m_styleStack.CurrentItem();
            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            m_styleStack.Remove();

            // Return if we don't have a valid style.
            if (style == null) return false;

            int styleLength = style.styleClosingTagArray.Length;

            // Replace <style> tag with opening definition
            int[] closingTagArray = style.styleClosingTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = closingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref closingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref closingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref closingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref closingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref closingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to handle inline replacement of style tag by closing style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceClosingStyleTag(ref char[] sourceText, int srcIndex, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Get style from the Style Stack
            int hashCode = m_styleStack.CurrentItem();
            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            m_styleStack.Remove();

            // Return if we don't have a valid style.
            if (style == null) return false;

            int styleLength = style.styleClosingTagArray.Length;

            // Replace <style> tag with opening definition
            int[] closingTagArray = style.styleClosingTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = closingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref closingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref closingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref closingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref closingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref closingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }

        /// <summary>
        /// Method to handle inline replacement of style tag by closing style definition.
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="srcIndex"></param>
        /// <param name="charBuffer"></param>
        /// <param name="writeIndex"></param>
        /// <returns></returns>
        bool ReplaceClosingStyleTag(ref StringBuilder sourceText, int srcIndex, ref UnicodeChar[] charBuffer, ref int writeIndex)
        {
            // Get style from the Style Stack
            int hashCode = m_styleStack.CurrentItem();
            TMP_Style style = TMP_StyleSheet.GetStyle(hashCode);

            m_styleStack.Remove();

            // Return if we don't have a valid style.
            if (style == null) return false;

            int styleLength = style.styleClosingTagArray.Length;

            // Replace <style> tag with opening definition
            int[] closingTagArray = style.styleClosingTagArray;

            for (int i = 0; i < styleLength; i++)
            {
                int c = closingTagArray[i];

                if (c == 60)
                {
                    if (IsTagName(ref closingTagArray, "<BR>", i))
                    {
                        if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                        charBuffer[writeIndex].unicode = 10;
                        writeIndex += 1;
                        i += 3;

                        continue;
                    }
                    else if (IsTagName(ref closingTagArray, "<STYLE=", i))
                    {
                        if (ReplaceOpeningStyleTag(ref closingTagArray, i, out int offset, ref charBuffer, ref writeIndex))
                        {
                            i = offset;
                            continue;
                        }
                    }
                    else if (IsTagName(ref closingTagArray, "</STYLE>", i))
                    {
                        ReplaceClosingStyleTag(ref closingTagArray, i, ref charBuffer, ref writeIndex);

                        // Strip </style> even if style is invalid.
                        i += 7;
                        continue;
                    }
                }

                if (writeIndex == charBuffer.Length) ResizeInternalArray(ref charBuffer);

                charBuffer[writeIndex].unicode = c;
                writeIndex += 1;
            }

            return true;
        }


        /// <summary>
        /// Method to check for a matching rich text tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsTagName (ref string text, string tag, int index)
        {
            if (text.Length < index + tag.Length) return false;
            
            for (int i = 0; i < tag.Length; i++)
            {
                if (TMP_TextUtilities.ToUpperFast(text[index + i]) != tag[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Method to check for a matching rich text tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsTagName(ref char[] text, string tag, int index)
        {
            if (text.Length < index + tag.Length) return false;

            for (int i = 0; i < tag.Length; i++)
            {
                if (TMP_TextUtilities.ToUpperFast(text[index + i]) != tag[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Method to check for a matching rich text tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsTagName(ref int[] text, string tag, int index)
        {
            if (text.Length < index + tag.Length) return false;

            for (int i = 0; i < tag.Length; i++)
            {
                if (TMP_TextUtilities.ToUpperFast((char)text[index + i]) != tag[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Method to check for a matching rich text tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool IsTagName(ref StringBuilder text, string tag, int index)
        {
            if (text.Length < index + tag.Length) return false;

            for (int i = 0; i < tag.Length; i++)
            {
                if (TMP_TextUtilities.ToUpperFast(text[index + i]) != tag[i]) return false;
            }

            return true;
        }

        /// <summary>
        /// Get Hashcode for a given tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="closeIndex"></param>
        /// <returns></returns>
        int GetTagHashCode(ref string text, int index, out int closeIndex)
        {
            int hashCode = 0;
            closeIndex = 0;

            for (int i = index; i < text.Length; i++)
            {
                // Skip quote '"' character
                if (text[i] == 34) continue;

                // Break at '>'
                if (text[i] == 62) { closeIndex = i; break; }

                hashCode = (hashCode << 5) + hashCode ^ text[i];
            }

            return hashCode;
        }

        /// <summary>
        /// Get Hashcode for a given tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="closeIndex"></param>
        /// <returns></returns>
        int GetTagHashCode(ref char[] text, int index, out int closeIndex)
        {
            int hashCode = 0;
            closeIndex = 0;

            for (int i = index; i < text.Length; i++)
            {
                // Skip quote '"' character
                if (text[i] == 34) continue;

                // Break at '>'
                if (text[i] == 62) { closeIndex = i; break; }

                hashCode = (hashCode << 5) + hashCode ^ text[i];
            }

            return hashCode;
        }

        /// <summary>
        /// Get Hashcode for a given tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="closeIndex"></param>
        /// <returns></returns>
        int GetTagHashCode(ref int[] text, int index, out int closeIndex)
        {
            int hashCode = 0;
            closeIndex = 0;

            for (int i = index; i < text.Length; i++)
            {
                // Skip quote '"' character
                if (text[i] == 34) continue;

                // Break at '>'
                if (text[i] == 62) { closeIndex = i; break; }

                hashCode = (hashCode << 5) + hashCode ^ text[i];
            }

            return hashCode;
        }

        /// <summary>
        ///  Get Hashcode for a given tag.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <param name="closeIndex"></param>
        /// <returns></returns>
        int GetTagHashCode(ref StringBuilder text, int index, out int closeIndex)
        {
            int hashCode = 0;
            closeIndex = 0;

            for (int i = index; i < text.Length; i++)
            {
                // Skip quote '"' character
                if (text[i] == 34) continue;

                // Break at '>'
                if (text[i] == 62) { closeIndex = i; break; }

                hashCode = (hashCode << 5) + hashCode ^ text[i];
            }

            return hashCode;
        }

        /// <summary>
        /// 
        /// </summary>
        void ResizeInternalArray <T>(ref T[] array)
        {
            int size = Mathf.NextPowerOfTwo(array.Length + 1);

            System.Array.Resize(ref array, size);
        }


        private readonly float[] k_Power = { 5e-1f, 5e-2f, 5e-3f, 5e-4f, 5e-5f, 5e-6f, 5e-7f, 5e-8f, 5e-9f, 5e-10f }; // Used by FormatText to enable rounding and avoid using Mathf.Pow.

        /// <summary>
        /// Function used in conjunction with SetText()
        /// </summary>
        /// <param name="number"></param>
        /// <param name="index"></param>
        /// <param name="precision"></param>
        protected void AddFloatToCharArray(double number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            number += k_Power[Mathf.Min(9, precision)];

            double integer = Math.Truncate(number);

            AddIntToCharArray(integer, ref index, precision);

            if (precision > 0)
            {
                // Add the decimal point
                m_input_CharArray[index++] = '.';

                number -= integer;
                for (int p = 0; p < precision; p++)
                {
                    number *= 10;
                    long d = (long)(number);

                    m_input_CharArray[index++] = (char)(d + 48);
                    number -= d;
                }
            }
        }


        /// <summary>
        /// // Function used in conjunction with SetText()
        /// </summary>
        /// <param name="number"></param>
        /// <param name="index"></param>
        /// <param name="precision"></param>
        protected void AddIntToCharArray(double number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            int i = index;
            do
            {
                m_input_CharArray[i++] = (char)(number % 10 + 48);
                number /= 10;
            } while (number > 0.999d);

            int lastIndex = i;

            // Reverse string
            while (index + 1 < i)
            {
                i -= 1;
                char t = m_input_CharArray[index];
                m_input_CharArray[index] = m_input_CharArray[i];
                m_input_CharArray[i] = t;
                index += 1;
            }
            index = lastIndex;
        }


        /// <summary>
        /// Method used to determine the number of visible characters and required buffer allocations.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        protected virtual int SetArraySizes(UnicodeChar[] chars) { return 0; }


        /// <summary>
        /// Method which parses the text input, does the layout of the text as well as generating the geometry.
        /// </summary>
        protected virtual void GenerateTextMesh() { }


        /// <summary>
        /// Function to Calculate the Preferred Width and Height of the text object.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPreferredValues()
        {
            if (m_isInputParsingRequired || m_isTextTruncated)
            {
                m_isCalculatingPreferredValues = true;
                ParseInputText();
            }

            // CALCULATE PREFERRED WIDTH
            float preferredWidth = GetPreferredWidth();

            // CALCULATE PREFERRED HEIGHT
            float preferredHeight = GetPreferredHeight();

            return new Vector2(preferredWidth, preferredHeight);
        }


        /// <summary>
        /// Function to Calculate the Preferred Width and Height of the text object given the provided width and height.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPreferredValues(float width, float height)
        {
            if (m_isInputParsingRequired || m_isTextTruncated)
            {
                m_isCalculatingPreferredValues = true;
                ParseInputText();
            }

            Vector2 margin = new Vector2(width, height);

            // CALCULATE PREFERRED WIDTH
            float preferredWidth = GetPreferredWidth(margin);

            // CALCULATE PREFERRED HEIGHT
            float preferredHeight = GetPreferredHeight(margin);

            return new Vector2(preferredWidth, preferredHeight);
        }


        /// <summary>
        /// Function to Calculate the Preferred Width and Height of the text object given a certain string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Vector2 GetPreferredValues(string text)
        {
            m_isCalculatingPreferredValues = true;

            StringToCharArray(text, ref m_TextParsingBuffer);
            SetArraySizes(m_TextParsingBuffer);

            Vector2 margin = k_LargePositiveVector2;

            // CALCULATE PREFERRED WIDTH
            float preferredWidth = GetPreferredWidth(margin);

            // CALCULATE PREFERRED HEIGHT
            float preferredHeight = GetPreferredHeight(margin);

            return new Vector2(preferredWidth, preferredHeight);
        }


        /// <summary>
        ///  Function to Calculate the Preferred Width and Height of the text object given a certain string and size of text container.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Vector2 GetPreferredValues(string text, float width, float height)
        {
            m_isCalculatingPreferredValues = true;

            StringToCharArray(text, ref m_TextParsingBuffer);
            SetArraySizes(m_TextParsingBuffer);

            Vector2 margin = new Vector2(width, height);

            // CALCULATE PREFERRED WIDTH
            float preferredWidth = GetPreferredWidth(margin);

            // CALCULATE PREFERRED HEIGHT
            float preferredHeight = GetPreferredHeight(margin);

            return new Vector2(preferredWidth, preferredHeight);
        }


        /// <summary>
        /// Method to calculate the preferred width of a text object.
        /// </summary>
        /// <returns></returns>
        protected float GetPreferredWidth()
        {
            if (TMP_Settings.instance == null) return 0;

            float fontSize = m_enableAutoSizing ? m_fontSizeMax : m_fontSize;

            // Reset auto sizing point size bounds
            m_minFontSize = m_fontSizeMin;
            m_maxFontSize = m_fontSizeMax;
            m_charWidthAdjDelta = 0;

            // Set Margins to Infinity
            Vector2 margin = k_LargePositiveVector2;

            if (m_isInputParsingRequired || m_isTextTruncated)
            {
                m_isCalculatingPreferredValues = true;
                ParseInputText();
            }

            m_recursiveCount = 0;
            float preferredWidth = CalculatePreferredValues(fontSize, margin, true).x;

            m_isPreferredWidthDirty = false;

            //Debug.Log("GetPreferredWidth() Called at frame " + Time.frameCount + ". Returning width of " + preferredWidth);

            return preferredWidth;
        }


        /// <summary>
        /// Method to calculate the preferred width of a text object.
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        protected float GetPreferredWidth(Vector2 margin)
        {
            float fontSize = m_enableAutoSizing ? m_fontSizeMax : m_fontSize;

            // Reset auto sizing point size bounds
            m_minFontSize = m_fontSizeMin;
            m_maxFontSize = m_fontSizeMax;
            m_charWidthAdjDelta = 0;

            m_recursiveCount = 0;
            float preferredWidth = CalculatePreferredValues(fontSize, margin, true).x;

            //Debug.Log("GetPreferredWidth() Called. Returning width of " + preferredWidth);

            return preferredWidth;
        }


        /// <summary>
        /// Method to calculate the preferred height of a text object.
        /// </summary>
        /// <returns></returns>
        protected float GetPreferredHeight()
        {
            if (TMP_Settings.instance == null) return 0;

            float fontSize = m_enableAutoSizing ? m_fontSizeMax : m_fontSize;

            // Reset auto sizing point size bounds
            m_minFontSize = m_fontSizeMin;
            m_maxFontSize = m_fontSizeMax;
            m_charWidthAdjDelta = 0;

            Vector2 margin = new Vector2(m_marginWidth != 0 ? m_marginWidth : k_LargePositiveFloat, k_LargePositiveFloat);

            if (m_isInputParsingRequired || m_isTextTruncated)
            {
                m_isCalculatingPreferredValues = true;
                ParseInputText();
            }

            m_recursiveCount = 0;
            float preferredHeight = CalculatePreferredValues(fontSize, margin, !m_enableAutoSizing).y;

            m_isPreferredHeightDirty = false;

            //Debug.Log("GetPreferredHeight() Called. Returning height of " + preferredHeight);

            return preferredHeight;
        }


        /// <summary>
        /// Method to calculate the preferred height of a text object.
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        protected float GetPreferredHeight(Vector2 margin)
        {
            float fontSize = m_enableAutoSizing ? m_fontSizeMax : m_fontSize;

            // Reset auto sizing point size bounds
            m_minFontSize = m_fontSizeMin;
            m_maxFontSize = m_fontSizeMax;
            m_charWidthAdjDelta = 0;

            m_recursiveCount = 0;
            float preferredHeight = CalculatePreferredValues(fontSize, margin, true).y;

            //Debug.Log("GetPreferredHeight() Called. Returning height of " + preferredHeight);

            return preferredHeight;
        }


        /// <summary>
        /// Method returning the rendered width and height of the text object.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetRenderedValues()
        {
            return GetTextBounds().size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onlyVisibleCharacters">Should returned value only factor in visible characters and exclude those greater than maxVisibleCharacters for instance.</param>
        /// <returns></returns>
        public Vector2 GetRenderedValues(bool onlyVisibleCharacters)
        {
            return GetTextBounds(onlyVisibleCharacters).size;
        }


        /// <summary>
        /// Method returning the rendered width of the text object.
        /// </summary>
        /// <returns></returns>
        protected float GetRenderedWidth()
        {
            return GetRenderedValues().x;
        }

        /// <summary>
        /// Method returning the rendered width of the text object.
        /// </summary>
        /// <returns></returns>
        protected float GetRenderedWidth(bool onlyVisibleCharacters)
        {
            return GetRenderedValues(onlyVisibleCharacters).x;
        }

        /// <summary>
        /// Method returning the rendered height of the text object.
        /// </summary>
        /// <returns></returns>
        protected float GetRenderedHeight()
        {
            return GetRenderedValues().y;
        }

        /// <summary>
        /// Method returning the rendered height of the text object.
        /// </summary>
        /// <returns></returns>
        protected float GetRenderedHeight(bool onlyVisibleCharacters)
        {
            return GetRenderedValues(onlyVisibleCharacters).y;
        }


        /// <summary>
        /// Method to calculate the preferred width and height of the text object.
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize, bool ignoreTextAutoSizing)
        {
            //Debug.Log("*** CalculatePreferredValues() ***"); // ***** Frame: " + Time.frameCount);

            ////Profiler.BeginSample("TMP Generate Text - Phase I");

            // Early exit if no font asset was assigned. This should not be needed since LiberationSans SDF will be assigned by default.
            if (m_fontAsset == null || m_fontAsset.characterLookupTable == null)
            {
                Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + this.GetInstanceID());

                return Vector2.zero;
            }

            // Early exit if we don't have any Text to generate.
            if (m_TextParsingBuffer == null || m_TextParsingBuffer.Length == 0 || m_TextParsingBuffer[0].unicode == (char)0)
            {
                return Vector2.zero;
            }

            m_currentFontAsset = m_fontAsset;
            m_currentMaterial = m_sharedMaterial;
            m_currentMaterialIndex = 0;
            m_materialReferenceStack.SetDefault(new MaterialReference(0, m_currentFontAsset, null, m_currentMaterial, m_padding));

            // Total character count is computed when the text is parsed.
            int totalCharacterCount = m_totalCharacterCount; // m_VisibleCharacters.Count;

            if (m_internalCharacterInfo == null || totalCharacterCount > m_internalCharacterInfo.Length)
            {
                m_internalCharacterInfo = new TMP_CharacterInfo[totalCharacterCount > 1024 ? totalCharacterCount + 256 : Mathf.NextPowerOfTwo(totalCharacterCount)];
            }

            // Calculate the scale of the font based on selected font size and sampling point size.
            // baseScale is calculated using the font asset assigned to the text object.
            float baseScale = m_fontScale = (defaultFontSize / m_fontAsset.faceInfo.pointSize * m_fontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
            float currentElementScale = baseScale;
            m_fontScaleMultiplier = 1;

            m_currentFontSize = defaultFontSize;
            m_sizeStack.SetDefault(m_currentFontSize);
            float fontSizeDelta = 0;

            int charCode = 0; // Holds the character code of the currently being processed character.

            m_FontStyleInternal = m_fontStyle; // Set the default style.

            m_lineJustification = m_textAlignment; // Sets the line justification mode to match editor alignment.
            m_lineJustificationStack.SetDefault(m_lineJustification);

            float bold_xAdvance_multiplier = 1; // Used to increase spacing between character when style is bold.

            m_baselineOffset = 0; // Used by subscript characters.
            m_baselineOffsetStack.Clear();

            m_lineOffset = 0; // Amount of space between lines (font line spacing + m_linespacing).
            m_lineHeight = TMP_Math.FLOAT_UNSET;
            float lineGap = m_currentFontAsset.faceInfo.lineHeight - (m_currentFontAsset.faceInfo.ascentLine - m_currentFontAsset.faceInfo.descentLine);

            m_cSpacing = 0; // Amount of space added between characters as a result of the use of the <cspace> tag.
            m_monoSpacing = 0;
            float lineOffsetDelta = 0;
            m_xAdvance = 0; // Used to track the position of each character.
            float maxXAdvance = 0; // Used to determine Preferred Width.

            tag_LineIndent = 0; // Used for indentation of text.
            tag_Indent = 0;
            m_indentStack.SetDefault(0);
            tag_NoParsing = false;
            //m_isIgnoringAlignment = false;

            m_characterCount = 0; // Total characters in the char[]


            // Tracking of line information
            m_firstCharacterOfLine = 0;
            m_maxLineAscender = k_LargeNegativeFloat;
            m_maxLineDescender = k_LargePositiveFloat;
            m_lineNumber = 0;

            float marginWidth = marginSize.x;
            //float marginHeight = marginSize.y;
            m_marginLeft = 0;
            m_marginRight = 0;
            m_width = -1;

            // Used by Unity's Auto Layout system.
            float renderedWidth = 0;
            float renderedHeight = 0;
            float linebreakingWidth = 0;
            m_isCalculatingPreferredValues = true;

            // Tracking of the highest Ascender
            m_maxAscender = 0;
            m_maxDescender = 0;


            // Initialize struct to track states of word wrapping
            bool isFirstWord = true;
            bool isLastBreakingChar = false;
            WordWrapState savedLineState = new WordWrapState();
            SaveWordWrappingState(ref savedLineState, 0, 0);
            WordWrapState savedWordWrapState = new WordWrapState();
            int wrappingIndex = 0;

            // Counter to prevent recursive lockup when computing preferred values.
            m_recursiveCount += 1;

            // Parse through Character buffer to read HTML tags and begin creating mesh.
            for (int i = 0; m_TextParsingBuffer[i].unicode != 0; i++)
            {
                charCode = (int)m_TextParsingBuffer[i].unicode;

                // Parse Rich Text Tag
                #region Parse Rich Text Tag
                if (m_isRichText && charCode == 60)  // '<'
                {
                    m_isParsingText = true;
                    m_textElementType = TMP_TextElementType.Character;

                    // Check if Tag is valid. If valid, skip to the end of the validated tag.
                    if (ValidateHtmlTag(m_TextParsingBuffer, i + 1, out int endTagIndex))
                    {
                        i = endTagIndex;

                        // Continue to next character or handle the sprite element
                        if (m_textElementType == TMP_TextElementType.Character)
                            continue;
                    }
                }
                else
                {
                    m_textElementType = m_textInfo.characterInfo[m_characterCount].elementType;
                    m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;
                    m_currentFontAsset = m_textInfo.characterInfo[m_characterCount].fontAsset;
                }
                #endregion End Parse Rich Text Tag

                int prev_MaterialIndex = m_currentMaterialIndex;
                bool isUsingAltTypeface = m_textInfo.characterInfo[m_characterCount].isUsingAlternateTypeface;

                m_isParsingText = false;

                // Handle Font Styles like LowerCase, UpperCase and SmallCaps.
                #region Handling of LowerCase, UpperCase and SmallCaps Font Styles

                float smallCapsMultiplier = 1.0f;

                if (m_textElementType == TMP_TextElementType.Character)
                {
                    if (/*(m_fontStyle & FontStyles.UpperCase) == FontStyles.UpperCase ||*/ (m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
                    {
                        // If this character is lowercase, switch to uppercase.
                        if (char.IsLower((char)charCode))
                            charCode = char.ToUpper((char)charCode);

                    }
                    else if (/*(m_fontStyle & FontStyles.LowerCase) == FontStyles.LowerCase ||*/ (m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
                    {
                        // If this character is uppercase, switch to lowercase.
                        if (char.IsUpper((char)charCode))
                            charCode = char.ToLower((char)charCode);
                    }
                    else if (/*(m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps ||*/ (m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps)
                    {
                        if (char.IsLower((char)charCode))
                        {
                            smallCapsMultiplier = 0.8f;
                            charCode = char.ToUpper((char)charCode);
                        }
                    }
                }
                #endregion


                // Look up Character Data from Dictionary and cache it.
                #region Look up Character Data
                if (m_textElementType == TMP_TextElementType.Sprite)
                {
                    // If a sprite is used as a fallback then get a reference to it and set the color to white.
                    m_currentSpriteAsset = m_textInfo.characterInfo[m_characterCount].spriteAsset;
                    m_spriteIndex = m_textInfo.characterInfo[m_characterCount].spriteIndex;

                    TMP_SpriteCharacter sprite = m_currentSpriteAsset.spriteCharacterTable[m_spriteIndex];
                    if (sprite == null) continue;

                    // Sprites are assigned in the E000 Private Area + sprite Index
                    if (charCode == 60)
                        charCode = 57344 + m_spriteIndex;

                    m_currentFontAsset = m_fontAsset;

                    // The sprite scale calculations are based on the font asset assigned to the text object.
                    // Sprite scale is used to determine line height
                    // Current element scale represents a modified scale to normalize the sprite based on the font baseline to ascender.
                    float spriteScale = (m_currentFontSize / m_fontAsset.faceInfo.pointSize * m_fontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                    currentElementScale = m_fontAsset.faceInfo.ascentLine / sprite.glyph.metrics.height * sprite.scale * spriteScale;

                    m_cached_TextElement = sprite;

                    m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Sprite;
                    m_internalCharacterInfo[m_characterCount].scale = spriteScale;
                    //m_internalCharacterInfo[m_characterCount].spriteAsset = m_currentSpriteAsset;
                    //m_internalCharacterInfo[m_characterCount].fontAsset = m_currentFontAsset;
                    //m_internalCharacterInfo[m_characterCount].materialReferenceIndex = m_currentMaterialIndex;

                    m_currentMaterialIndex = prev_MaterialIndex;
                }
                else if (m_textElementType == TMP_TextElementType.Character)
                {
                    m_cached_TextElement = m_textInfo.characterInfo[m_characterCount].textElement;
                    if (m_cached_TextElement == null) continue;

                    //m_currentFontAsset = m_textInfo.characterInfo[m_characterCount].fontAsset;
                    //m_currentMaterial = m_textInfo.characterInfo[m_characterCount].material;
                    m_currentMaterialIndex = m_textInfo.characterInfo[m_characterCount].materialReferenceIndex;

                    // Re-calculate font scale as the font asset may have changed.
                    m_fontScale = m_currentFontSize * smallCapsMultiplier / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f);

                    currentElementScale = m_fontScale * m_fontScaleMultiplier * m_cached_TextElement.scale;

                    m_internalCharacterInfo[m_characterCount].elementType = TMP_TextElementType.Character;

                }
                #endregion


                // Handle Soft Hyphen
                #region Handle Soft Hyphen
                float old_scale = currentElementScale;
                if (charCode == 0xAD)
                {
                    currentElementScale = 0;
                }
                #endregion


                // Store some of the text object's information
                m_internalCharacterInfo[m_characterCount].character = (char)charCode;


                // Handle Kerning if Enabled.
                #region Handle Kerning
                TMP_GlyphValueRecord glyphAdjustments = new TMP_GlyphValueRecord();
                float characterSpacingAdjustment = m_characterSpacing;
                if (m_enableKerning)
                {
                    if (m_characterCount < totalCharacterCount - 1)
                    {
                        uint firstGlyphIndex = m_cached_TextElement.glyphIndex;
                        uint secondGlyphIndex = m_textInfo.characterInfo[m_characterCount + 1].textElement.glyphIndex;
                        long key = new GlyphPairKey(firstGlyphIndex, secondGlyphIndex).key;

                        if (m_currentFontAsset.fontFeatureTable.m_GlyphPairAdjustmentRecordLookupDictionary.TryGetValue(key, out TMP_GlyphPairAdjustmentRecord adjustmentPair))
                        {
                            glyphAdjustments = adjustmentPair.firstAdjustmentRecord.glyphValueRecord;
                            characterSpacingAdjustment = (adjustmentPair.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments ? 0 : characterSpacingAdjustment;
                        }
                    }

                    if (m_characterCount >= 1)
                    {
                        uint firstGlyphIndex = m_textInfo.characterInfo[m_characterCount - 1].textElement.glyphIndex;
                        uint secondGlyphIndex = m_cached_TextElement.glyphIndex;
                        long key = new GlyphPairKey(firstGlyphIndex, secondGlyphIndex).key;

                        if (m_currentFontAsset.fontFeatureTable.m_GlyphPairAdjustmentRecordLookupDictionary.TryGetValue(key, out TMP_GlyphPairAdjustmentRecord adjustmentPair))
                        {
                            glyphAdjustments += adjustmentPair.secondAdjustmentRecord.glyphValueRecord;
                            characterSpacingAdjustment = (adjustmentPair.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments ? 0 : characterSpacingAdjustment;
                        }
                    }
                }
                #endregion


                // Initial Implementation for RTL support.
                #region Handle Right-to-Left
                //if (m_isRightToLeft)
                //{
                //    m_xAdvance -= ((m_cached_TextElement.xAdvance * bold_xAdvance_multiplier + m_characterSpacing + m_wordSpacing + m_currentFontAsset.normalSpacingOffset) * currentElementScale + m_cSpacing) * (1 - m_charWidthAdjDelta);

                //    if (char.IsWhiteSpace((char)charCode) || charCode == 0x200B)
                //        m_xAdvance -= m_wordSpacing * currentElementScale;
                //}
                #endregion


                // Handle Mono Spacing
                #region Handle Mono Spacing
                float monoAdvance = 0;
                if (m_monoSpacing != 0)
                {
                    monoAdvance = (m_monoSpacing / 2 - (m_cached_TextElement.glyph.metrics.width / 2 + m_cached_TextElement.glyph.metrics.horizontalBearingX) * currentElementScale);
                    m_xAdvance += monoAdvance;
                }
                #endregion


                // Set Padding based on selected font style
                #region Handle Style Padding
                if (m_textElementType == TMP_TextElementType.Character && !isUsingAltTypeface && ((m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold)) // Checks for any combination of Bold Style.
                {
                    bold_xAdvance_multiplier = 1 + m_currentFontAsset.boldSpacing * 0.01f;
                }
                else
                {
                    //style_padding = m_currentFontAsset.normalStyle * 2;
                    bold_xAdvance_multiplier = 1.0f;
                }
                #endregion Handle Style Padding

                m_internalCharacterInfo[m_characterCount].baseLine = 0 - m_lineOffset + m_baselineOffset;


                // Compute and save text element Ascender and maximum line Ascender.
                float elementAscender = m_currentFontAsset.faceInfo.ascentLine * (m_textElementType == TMP_TextElementType.Character ? currentElementScale / smallCapsMultiplier : m_internalCharacterInfo[m_characterCount].scale) + m_baselineOffset;
                m_internalCharacterInfo[m_characterCount].ascender = elementAscender - m_lineOffset;
                m_maxLineAscender = elementAscender > m_maxLineAscender ? elementAscender : m_maxLineAscender;

                // Compute and save text element Descender and maximum line Descender.
                float elementDescender = m_currentFontAsset.faceInfo.descentLine * (m_textElementType == TMP_TextElementType.Character ? currentElementScale / smallCapsMultiplier: m_internalCharacterInfo[m_characterCount].scale) + m_baselineOffset;
                float elementDescenderII = m_internalCharacterInfo[m_characterCount].descender = elementDescender - m_lineOffset;
                m_maxLineDescender = elementDescender < m_maxLineDescender ? elementDescender : m_maxLineDescender;

                // Adjust maxLineAscender and maxLineDescender if style is superscript or subscript
                if ((m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript || (m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript)
                {
                    float baseAscender = (elementAscender - m_baselineOffset) / m_currentFontAsset.faceInfo.subscriptSize;
                    elementAscender = m_maxLineAscender;
                    m_maxLineAscender = baseAscender > m_maxLineAscender ? baseAscender : m_maxLineAscender;

                    float baseDescender = (elementDescender - m_baselineOffset) / m_currentFontAsset.faceInfo.subscriptSize;
                    elementDescender = m_maxLineDescender;
                    m_maxLineDescender = baseDescender < m_maxLineDescender ? baseDescender : m_maxLineDescender;
                }

                if (m_lineNumber == 0) m_maxAscender = m_maxAscender > elementAscender ? m_maxAscender : elementAscender;
                //if (m_lineOffset == 0) pageAscender = pageAscender > elementAscender ? pageAscender : elementAscender;

                // Setup Mesh for visible text elements. ie. not a SPACE / LINEFEED / CARRIAGE RETURN.
                #region Handle Visible Characters
                if (charCode == 9 || charCode == 0xA0 || charCode == 0x2007 || (!char.IsWhiteSpace((char)charCode) && charCode != 0x200B) || m_textElementType == TMP_TextElementType.Sprite)
                {
                    // Check if Character exceeds the width of the Text Container
                    #region Handle Line Breaking, Text Auto-Sizing and Horizontal Overflow
                    float width = m_width != -1 ? Mathf.Min(marginWidth + 0.0001f - m_marginLeft - m_marginRight, m_width) : marginWidth + 0.0001f - m_marginLeft - m_marginRight;

                    bool isJustifiedOrFlush = ((_HorizontalAlignmentOptions)m_lineJustification & _HorizontalAlignmentOptions.Flush) == _HorizontalAlignmentOptions.Flush || ((_HorizontalAlignmentOptions)m_lineJustification & _HorizontalAlignmentOptions.Justified) == _HorizontalAlignmentOptions.Justified;

                    // Calculate the line breaking width of the text.
                    linebreakingWidth = m_xAdvance + m_cached_TextElement.glyph.metrics.horizontalAdvance * (1 - m_charWidthAdjDelta) * (charCode != 0xAD ? currentElementScale : old_scale);

                    // Check if Character exceeds the width of the Text Container
                    if (linebreakingWidth > width * (isJustifiedOrFlush ? 1.05f : 1.0f))
                    {
                        // Word Wrapping
                        #region Handle Word Wrapping
                        if (enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
                        {
                            // Check if word wrapping is still possible
                            #region Line Breaking Check
                            if (wrappingIndex == savedWordWrapState.previous_WordBreak || isFirstWord)
                            {
                                // Word wrapping is no longer possible. Shrink size of text if auto-sizing is enabled.
                                #region Text Auto-Sizing
                                if (ignoreTextAutoSizing == false && m_currentFontSize > m_fontSizeMin)
                                {
                                    // Handle Character Width Adjustments
                                    #region Character Width Adjustments
                                    if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100)
                                    {
                                        m_recursiveCount = 0;
                                        m_charWidthAdjDelta += 0.01f;
                                        return CalculatePreferredValues(defaultFontSize, marginSize, false);
                                    }
                                    #endregion

                                    // Adjust Point Size
                                    m_maxFontSize = defaultFontSize;

                                    defaultFontSize -= Mathf.Max((defaultFontSize - m_minFontSize) / 2, 0.05f);
                                    defaultFontSize = (int)(Mathf.Max(defaultFontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                                    if (m_recursiveCount > 20) return new Vector2(renderedWidth, renderedHeight);
                                    return CalculatePreferredValues(defaultFontSize, marginSize, false);
                                }
                                #endregion

                                // Word wrapping is no longer possible, now breaking up individual words.
                                if (m_isCharacterWrappingEnabled == false)
                                {
                                    m_isCharacterWrappingEnabled = true;
                                }
                                else
                                    isLastBreakingChar = true;
                            }
                            #endregion

                            // Restore to previously stored state of last valid (space character or linefeed)
                            i = RestoreWordWrappingState(ref savedWordWrapState);
                            wrappingIndex = i;  // Used to detect when line length can no longer be reduced.

                            // Handling for Soft Hyphen
                            if (m_TextParsingBuffer[i].unicode == 0xAD) // && !m_isCharacterWrappingEnabled) // && ellipsisIndex != i && !m_isCharacterWrappingEnabled)
                            {
                                m_isTextTruncated = true;
                                m_TextParsingBuffer[i].unicode = 0x2D;
                                return CalculatePreferredValues(defaultFontSize, marginSize, true);
                            }

                            // Check if Line Spacing of previous line needs to be adjusted.
                            if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == TMP_Math.FLOAT_UNSET)
                            {
                                //Debug.Log("(1) Adjusting Line Spacing on line #" + m_lineNumber);
                                float offsetDelta = m_maxLineAscender - m_startOfLineAscender;
                                //AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, offsetDelta);
                                m_lineOffset += offsetDelta;
                                savedWordWrapState.lineOffset = m_lineOffset;
                                savedWordWrapState.previousLineAscender = m_maxLineAscender;

                                // TODO - Add check for character exceeding vertical bounds
                            }
                            //m_isNewPage = false;

                            // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                            float lineAscender = m_maxLineAscender - m_lineOffset;
                            float lineDescender = m_maxLineDescender - m_lineOffset;


                            // Update maxDescender and maxVisibleDescender
                            m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;


                            m_firstCharacterOfLine = m_characterCount; // Store first character of the next line.

                            // Compute Preferred Width & Height
                            renderedWidth += m_xAdvance;

                            if (m_enableWordWrapping)
                                renderedHeight = m_maxAscender - m_maxDescender;
                            else
                                renderedHeight = Mathf.Max(renderedHeight, lineAscender - lineDescender);


                            // Store the state of the line before starting on the new line.
                            SaveWordWrappingState(ref savedLineState, i, m_characterCount - 1);

                            m_lineNumber += 1;
                            //isStartOfNewLine = true;

                            // Check to make sure Array is large enough to hold a new line.
                            //if (m_lineNumber >= m_internalTextInfo.lineInfo.Length)
                            //    ResizeLineExtents(m_lineNumber);

                            // Apply Line Spacing based on scale of the last character of the line.
                            if (m_lineHeight == TMP_Math.FLOAT_UNSET)
                            {
                                float ascender = m_internalCharacterInfo[m_characterCount].ascender - m_internalCharacterInfo[m_characterCount].baseLine;
                                lineOffsetDelta = 0 - m_maxLineDescender + ascender + (lineGap + m_lineSpacing + m_lineSpacingDelta) * baseScale;
                                m_lineOffset += lineOffsetDelta;

                                m_startOfLineAscender = ascender;
                            }
                            else
                                m_lineOffset += m_lineHeight + m_lineSpacing * baseScale;

                            m_maxLineAscender = k_LargeNegativeFloat;
                            m_maxLineDescender = k_LargePositiveFloat;

                            m_xAdvance = 0 + tag_Indent;

                            continue;
                        }
                        #endregion End Word Wrapping

                        // Text Auto-Sizing (text exceeding Width of container. 
                        #region Handle Text Auto-Sizing
                        if (ignoreTextAutoSizing == false && defaultFontSize > m_fontSizeMin)
                        {
                            // Handle Character Width Adjustments
                            #region Character Width Adjustments
                            if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100)
                            {
                                m_recursiveCount = 0;
                                m_charWidthAdjDelta += 0.01f;
                                return CalculatePreferredValues(defaultFontSize, marginSize, false);
                            }
                            #endregion

                            // Adjust Point Size
                            m_maxFontSize = defaultFontSize;

                            defaultFontSize -= Mathf.Max((defaultFontSize - m_minFontSize) / 2, 0.05f);
                            defaultFontSize = (int)(Mathf.Max(defaultFontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                            if (m_recursiveCount > 20) return new Vector2(renderedWidth, renderedHeight);
                            return CalculatePreferredValues(defaultFontSize, marginSize, false);
                        }
                        #endregion End Text Auto-Sizing
                    }
                    #endregion End Check for Characters Exceeding Width of Text Container

                }
                #endregion Handle Visible Characters


                // Check if Line Spacing of previous line needs to be adjusted.
                #region Adjust Line Spacing
                if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == TMP_Math.FLOAT_UNSET && !m_isNewPage)
                {
                    //Debug.Log("Inline - Adjusting Line Spacing on line #" + m_lineNumber);
                    //float gap = 0; // Compute gap.

                    float offsetDelta = m_maxLineAscender - m_startOfLineAscender;
                    //AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, offsetDelta);
                    elementDescenderII -= offsetDelta;
                    m_lineOffset += offsetDelta;

                    m_startOfLineAscender += offsetDelta;
                    savedWordWrapState.lineOffset = m_lineOffset;
                    savedWordWrapState.previousLineAscender = m_startOfLineAscender;
                }
                #endregion


                // Check if text Exceeds the vertical bounds of the margin area.
                #region Check Vertical Bounds & Auto-Sizing
                /*
                if (m_maxAscender - elementDescenderII > marginHeight + 0.0001f)
                {
                    // Handle Line spacing adjustments
                    #region Line Spacing Adjustments
                    if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
                    {
                        //loopCountA = 0;

                        //m_lineSpacingDelta -= 1;
                        //GenerateTextMesh();
                        //return;
                    }
                    #endregion


                    // Handle Text Auto-sizing resulting from text exceeding vertical bounds.
                    #region Text Auto-Sizing (Text greater than vertical bounds)
                    if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                    {
                        m_maxFontSize = m_fontSize;

                        m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                        m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                        //m_recursiveCount = 0;
                        //if (loopCountA > 20) return; // Added to debug 
                        CalculatePreferredValues(m_fontSize, marginSize, false);
                        return Vector2.zero;
                    }
                    #endregion Text Auto-Sizing
                }
                */
                #endregion Check Vertical Bounds


                // Handle xAdvance & Tabulation Stops. Tab stops at every 25% of Font Size.
                #region XAdvance, Tabulation & Stops
                if (charCode == 9)
                {
                    float tabSize = m_currentFontAsset.faceInfo.tabWidth * currentElementScale;
                    float tabs = Mathf.Ceil(m_xAdvance / tabSize) * tabSize;
                    m_xAdvance = tabs > m_xAdvance ? tabs : m_xAdvance + tabSize;
                }
                else if (m_monoSpacing != 0)
                {
                    m_xAdvance += (m_monoSpacing - monoAdvance + ((characterSpacingAdjustment + m_currentFontAsset.normalSpacingOffset) * currentElementScale) + m_cSpacing) * (1 - m_charWidthAdjDelta);

                    if (char.IsWhiteSpace((char)charCode) || charCode == 0x200B)
                        m_xAdvance += m_wordSpacing * currentElementScale;
                }
                else
                {
                    m_xAdvance += ((m_cached_TextElement.glyph.metrics.horizontalAdvance * bold_xAdvance_multiplier + characterSpacingAdjustment + m_currentFontAsset.normalSpacingOffset + glyphAdjustments.xAdvance) * currentElementScale + m_cSpacing) * (1 - m_charWidthAdjDelta);

                    if (char.IsWhiteSpace((char)charCode) || charCode == 0x200B)
                        m_xAdvance += m_wordSpacing * currentElementScale;
                }


                #endregion Tabulation & Stops


                // Handle Carriage Return
                #region Carriage Return
                if (charCode == 13)
                {
                    maxXAdvance = Mathf.Max(maxXAdvance, renderedWidth + m_xAdvance);
                    renderedWidth = 0;
                    m_xAdvance = 0 + tag_Indent;
                }
                #endregion Carriage Return


                // Handle Line Spacing Adjustments + Word Wrapping & special case for last line.
                #region Check for Line Feed and Last Character
                if (charCode == 10 || m_characterCount == totalCharacterCount - 1)
                {
                    // Check if Line Spacing of previous line needs to be adjusted.
                    if (m_lineNumber > 0 && !TMP_Math.Approximately(m_maxLineAscender, m_startOfLineAscender) && m_lineHeight == TMP_Math.FLOAT_UNSET)
                    {
                        //Debug.Log("(2) Adjusting Line Spacing on line #" + m_lineNumber);
                        float offsetDelta = m_maxLineAscender - m_startOfLineAscender;
                        //AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, offsetDelta);
                        elementDescenderII -= offsetDelta;
                        m_lineOffset += offsetDelta;
                    }

                    // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                    //float lineAscender = m_maxLineAscender - m_lineOffset;
                    float lineDescender = m_maxLineDescender - m_lineOffset;

                    // Update maxDescender and maxVisibleDescender
                    m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;

                    m_firstCharacterOfLine = m_characterCount + 1;

                    // Store PreferredWidth paying attention to linefeed and last character of text.
                    if (charCode == 10 && m_characterCount != totalCharacterCount - 1)
                    {
                        maxXAdvance = Mathf.Max(maxXAdvance, renderedWidth + linebreakingWidth);
                        renderedWidth = 0;
                    }
                    else
                        renderedWidth = Mathf.Max(maxXAdvance, renderedWidth + linebreakingWidth);


                    renderedHeight = m_maxAscender - m_maxDescender;


                    // Add new line if not last lines or character.
                    if (charCode == 10)
                    {
                        // Store the state of the line before starting on the new line.
                        SaveWordWrappingState(ref savedLineState, i, m_characterCount);
                        // Store the state of the last Character before the new line.
                        SaveWordWrappingState(ref savedWordWrapState, i, m_characterCount);

                        m_lineNumber += 1;

                        // Apply Line Spacing
                        if (m_lineHeight == TMP_Math.FLOAT_UNSET)
                        {
                            lineOffsetDelta = 0 - m_maxLineDescender + elementAscender + (lineGap + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * baseScale;
                            m_lineOffset += lineOffsetDelta;
                        }
                        else
                            m_lineOffset += m_lineHeight + (m_lineSpacing + m_paragraphSpacing) * baseScale;

                        m_maxLineAscender = k_LargeNegativeFloat;
                        m_maxLineDescender = k_LargePositiveFloat;
                        m_startOfLineAscender = elementAscender;

                        m_xAdvance = 0 + tag_LineIndent + tag_Indent;

                        m_characterCount += 1;
                        continue;
                    }
                }
                #endregion Check for Linefeed or Last Character


                // Save State of Mesh Creation for handling of Word Wrapping
                #region Save Word Wrapping State
                if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
                {
                    if ((char.IsWhiteSpace((char)charCode) || charCode == 0x200B || charCode == 0x2D || charCode == 0xAD) && !m_isNonBreakingSpace && charCode != 0xA0 && charCode != 0x2011 && charCode != 0x202F && charCode != 0x2060)
                    {
                        // We store the state of numerous variables for the most recent Space, LineFeed or Carriage Return to enable them to be restored 
                        // for Word Wrapping.
                        SaveWordWrappingState(ref savedWordWrapState, i, m_characterCount);
                        m_isCharacterWrappingEnabled = false;
                        isFirstWord = false;
                    }
                    // Handling for East Asian languages
                    else if ((charCode > 0x1100 && charCode < 0x11ff || /* Hangul Jamo */
                                charCode > 0x2E80 && charCode < 0x9FFF || /* CJK */
                                charCode > 0xA960 && charCode < 0xA97F || /* Hangul Jame Extended-A */
                                charCode > 0xAC00 && charCode < 0xD7FF || /* Hangul Syllables */
                                charCode > 0xF900 && charCode < 0xFAFF || /* CJK Compatibility Ideographs */
                                charCode > 0xFE30 && charCode < 0xFE4F || /* CJK Compatibility Forms */
                                charCode > 0xFF00 && charCode < 0xFFEF)   /* CJK Halfwidth */
                                && !m_isNonBreakingSpace)
                    {
                        if (isFirstWord || isLastBreakingChar || TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(charCode) == false &&
                            (m_characterCount < totalCharacterCount - 1 &&
                            TMP_Settings.linebreakingRules.followingCharacters.ContainsKey(m_internalCharacterInfo[m_characterCount + 1].character) == false))
                        {
                            SaveWordWrappingState(ref savedWordWrapState, i, m_characterCount);
                            m_isCharacterWrappingEnabled = false;
                            isFirstWord = false;
                        }
                    }
                    else if ((isFirstWord || m_isCharacterWrappingEnabled == true || isLastBreakingChar))
                        SaveWordWrappingState(ref savedWordWrapState, i, m_characterCount);
                }
                #endregion Save Word Wrapping State

                m_characterCount += 1;
            }

            // Check Auto Sizing and increase font size to fill text container.
            #region Check Auto-Sizing (Upper Font Size Bounds)
            fontSizeDelta = m_maxFontSize - m_minFontSize;
            if (!m_isCharacterWrappingEnabled && ignoreTextAutoSizing == false && fontSizeDelta > 0.051f && defaultFontSize < m_fontSizeMax)
            {
                m_minFontSize = defaultFontSize;
                defaultFontSize += Mathf.Max((m_maxFontSize - defaultFontSize) / 2, 0.05f);
                defaultFontSize = (int)(Mathf.Min(defaultFontSize, m_fontSizeMax) * 20 + 0.5f) / 20f;

                if (m_recursiveCount > 20) return new Vector2(renderedWidth, renderedHeight);
                return CalculatePreferredValues(defaultFontSize, marginSize, false);
            }
            #endregion End Auto-sizing Check


            m_isCharacterWrappingEnabled = false;
            m_isCalculatingPreferredValues = false;

            // Adjust Preferred Width and Height to account for Margins.
            renderedWidth += m_margin.x > 0 ? m_margin.x : 0;
            renderedWidth += m_margin.z > 0 ? m_margin.z : 0;

            renderedHeight += m_margin.y > 0 ? m_margin.y : 0;
            renderedHeight += m_margin.w > 0 ? m_margin.w : 0;

            // Round Preferred Values to nearest 5/100.
            renderedWidth = (int)(renderedWidth * 100 + 1f) / 100f;
            renderedHeight = (int)(renderedHeight * 100 + 1f) / 100f;

            //Debug.Log("Preferred Values: (" + renderedWidth + ", " + renderedHeight + ") with Recursive count of " + m_recursiveCount);

            ////Profiler.EndSample();
            return new Vector2(renderedWidth, renderedHeight);
        }


        /// <summary>
        /// Method returning the compound bounds of the text object and child sub objects.
        /// </summary>
        /// <returns></returns>
        protected virtual Bounds GetCompoundBounds() { return new Bounds(); }


        /// <summary>
        /// Method which returns the bounds of the text object;
        /// </summary>
        /// <returns></returns>
        protected Bounds GetTextBounds()
        {
            if (m_textInfo == null || m_textInfo.characterCount > m_textInfo.characterInfo.Length) return new Bounds();

            Extents extent = new Extents(k_LargePositiveVector2, k_LargeNegativeVector2);

            for (int i = 0; i < m_textInfo.characterCount && i < m_textInfo.characterInfo.Length; i++)
            {
                if (!m_textInfo.characterInfo[i].isVisible) continue;

                extent.min.x = Mathf.Min(extent.min.x, m_textInfo.characterInfo[i].bottomLeft.x);
                extent.min.y = Mathf.Min(extent.min.y, m_textInfo.characterInfo[i].descender);

                extent.max.x = Mathf.Max(extent.max.x, m_textInfo.characterInfo[i].xAdvance);
                extent.max.y = Mathf.Max(extent.max.y, m_textInfo.characterInfo[i].ascender);
            }

            Vector2 size;
            size.x = extent.max.x - extent.min.x;
            size.y = extent.max.y - extent.min.y;

            Vector3 center = (extent.min + extent.max) / 2;

            return new Bounds(center, size);
        }


        /// <summary>
        /// Method which returns the bounds of the text object;
        /// </summary>
        /// <param name="onlyVisibleCharacters"></param>
        /// <returns></returns>
        protected Bounds GetTextBounds(bool onlyVisibleCharacters)
        {
            if (m_textInfo == null) return new Bounds();

            Extents extent = new Extents(k_LargePositiveVector2, k_LargeNegativeVector2);

            for (int i = 0; i < m_textInfo.characterCount; i++)
            {
                if ((i > maxVisibleCharacters || m_textInfo.characterInfo[i].lineNumber > m_maxVisibleLines) && onlyVisibleCharacters) break;

                if (onlyVisibleCharacters && !m_textInfo.characterInfo[i].isVisible) continue;

                extent.min.x = Mathf.Min(extent.min.x, m_textInfo.characterInfo[i].origin);
                extent.min.y = Mathf.Min(extent.min.y, m_textInfo.characterInfo[i].descender);

                extent.max.x = Mathf.Max(extent.max.x, m_textInfo.characterInfo[i].xAdvance);
                extent.max.y = Mathf.Max(extent.max.y, m_textInfo.characterInfo[i].ascender);
            }

            Vector2 size;
            size.x = extent.max.x - extent.min.x;
            size.y = extent.max.y - extent.min.y;

            Vector2 center = (extent.min + extent.max) / 2;

            return new Bounds(center, size);
        }


        /// <summary>
        /// Method to adjust line spacing as a result of using different fonts or font point size.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="offset"></param>
        protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset) { }


        /// <summary>
        /// Function to increase the size of the Line Extents Array.
        /// </summary>
        /// <param name="size"></param>
        protected void ResizeLineExtents(int size)
        {
            size = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size + 1);

            TMP_LineInfo[] temp_lineInfo = new TMP_LineInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < m_textInfo.lineInfo.Length)
                    temp_lineInfo[i] = m_textInfo.lineInfo[i];
                else
                {
                    temp_lineInfo[i].lineExtents.min = k_LargePositiveVector2;
                    temp_lineInfo[i].lineExtents.max = k_LargeNegativeVector2;

                    temp_lineInfo[i].ascender = k_LargeNegativeFloat;
                    temp_lineInfo[i].descender = k_LargePositiveFloat;
                }
            }

            m_textInfo.lineInfo = temp_lineInfo;
        }
        protected static Vector2 k_LargePositiveVector2 = new Vector2(TMP_Math.INT_MAX, TMP_Math.INT_MAX);
        protected static Vector2 k_LargeNegativeVector2 = new Vector2(TMP_Math.INT_MIN, TMP_Math.INT_MIN);
        protected static float k_LargePositiveFloat = TMP_Math.FLOAT_MAX;
        protected static float k_LargeNegativeFloat = TMP_Math.FLOAT_MIN;
        protected static int k_LargePositiveInt = TMP_Math.INT_MAX;
        protected static int k_LargeNegativeInt = TMP_Math.INT_MIN;

        /// <summary>
        /// Function used to evaluate the length of a text string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual TMP_TextInfo GetTextInfo(string text) { return null; }


        /// <summary>
        /// Function to force an update of the margin size.
        /// </summary>
        public virtual void ComputeMarginSize() { }


        /// <summary>
        /// Function used in conjunction with GetTextInfo to figure out Array allocations.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        //protected int GetArraySizes(int[] chars)
        //{
        //    //Debug.Log("Set Array Size called.");

        //    //int visibleCount = 0;
        //    //int totalCount = 0;
        //    int tagEnd = 0;

        //    m_totalCharacterCount = 0;
        //    m_isUsingBold = false;
        //    m_isParsingText = false;


        //    //m_VisibleCharacters.Clear();

        //    for (int i = 0; chars[i] != 0; i++)
        //    {
        //        int c = chars[i];

        //        if (m_isRichText && c == 60) // if Char '<'
        //        {
        //            // Check if Tag is Valid
        //            if (ValidateHtmlTag(chars, i + 1, out tagEnd))
        //            {
        //                i = tagEnd;
        //                //if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

        //                if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;

        //                continue;
        //            }
        //        }

        //        //if (!char.IsWhiteSpace((char)c) && c != 0x200B)
        //        //{
        //            //visibleCount += 1;
        //        //}

        //        //m_VisibleCharacters.Add((char)c);
        //        m_totalCharacterCount += 1;
        //    }

        //    return m_totalCharacterCount;
        //}


        /// <summary>
        /// Save the State of various variables used in the mesh creation loop in conjunction with Word Wrapping
        /// </summary>
        /// <param name="state"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
        {
            // Multi Font & Material support related
            state.currentFontAsset = m_currentFontAsset;
            state.currentSpriteAsset = m_currentSpriteAsset;
            state.currentMaterial = m_currentMaterial;
            state.currentMaterialIndex = m_currentMaterialIndex;

            state.previous_WordBreak = index;
            state.total_CharacterCount = count;
            state.visible_CharacterCount = m_lineVisibleCharacterCount;
            //state.visible_CharacterCount = m_visibleCharacterCount;
            //state.visible_SpriteCount = m_visibleSpriteCount;
            state.visible_LinkCount = m_textInfo.linkCount;

            state.firstCharacterIndex = m_firstCharacterOfLine;
            state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
            state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;

            state.fontStyle = m_FontStyleInternal;
            state.fontScale = m_fontScale;
            //state.maxFontScale = m_maxFontScale;
            state.fontScaleMultiplier = m_fontScaleMultiplier;
            state.currentFontSize = m_currentFontSize;

            state.xAdvance = m_xAdvance;
            state.maxCapHeight = m_maxCapHeight;
            state.maxAscender = m_maxAscender;
            state.maxDescender = m_maxDescender;
            state.maxLineAscender = m_maxLineAscender;
            state.maxLineDescender = m_maxLineDescender;
            state.previousLineAscender = m_startOfLineAscender;
            state.preferredWidth = m_preferredWidth;
            state.preferredHeight = m_preferredHeight;
            state.meshExtents = m_meshExtents;

            state.lineNumber = m_lineNumber;
            state.lineOffset = m_lineOffset;
            state.baselineOffset = m_baselineOffset;

            //state.alignment = m_lineJustification;
            state.vertexColor = m_htmlColor;
            state.underlineColor = m_underlineColor;
            state.strikethroughColor = m_strikethroughColor;
            state.highlightColor = m_highlightColor;

            state.isNonBreakingSpace = m_isNonBreakingSpace;
            state.tagNoParsing = tag_NoParsing;

            // XML Tag Stack
            state.basicStyleStack = m_fontStyleStack;
            state.colorStack = m_colorStack;
            state.underlineColorStack = m_underlineColorStack;
            state.strikethroughColorStack = m_strikethroughColorStack;
            state.highlightColorStack = m_highlightColorStack;
            state.colorGradientStack = m_colorGradientStack;
            state.sizeStack = m_sizeStack;
            state.indentStack = m_indentStack;
            state.fontWeightStack = m_FontWeightStack;
            state.styleStack = m_styleStack;
            state.baselineStack = m_baselineOffsetStack;
            state.actionStack = m_actionStack;
            state.materialReferenceStack = m_materialReferenceStack;
            state.lineJustificationStack = m_lineJustificationStack;
            //state.spriteAnimationStack = m_spriteAnimationStack;

            state.spriteAnimationID = m_spriteAnimationID;

            if (m_lineNumber < m_textInfo.lineInfo.Length)
                state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
        }


        /// <summary>
        /// Restore the State of various variables used in the mesh creation loop.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected int RestoreWordWrappingState(ref WordWrapState state)
        {
            int index = state.previous_WordBreak;

            // Multi Font & Material support related
            m_currentFontAsset = state.currentFontAsset;
            m_currentSpriteAsset = state.currentSpriteAsset;
            m_currentMaterial = state.currentMaterial;
            m_currentMaterialIndex = state.currentMaterialIndex;

            m_characterCount = state.total_CharacterCount + 1;
            m_lineVisibleCharacterCount = state.visible_CharacterCount;
            //m_visibleCharacterCount = state.visible_CharacterCount;
            //m_visibleSpriteCount = state.visible_SpriteCount;
            m_textInfo.linkCount = state.visible_LinkCount;

            m_firstCharacterOfLine = state.firstCharacterIndex;
            m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
            m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;

            m_FontStyleInternal = state.fontStyle;
            m_fontScale = state.fontScale;
            m_fontScaleMultiplier = state.fontScaleMultiplier;
            //m_maxFontScale = state.maxFontScale;
            m_currentFontSize = state.currentFontSize;

            m_xAdvance = state.xAdvance;
            m_maxCapHeight = state.maxCapHeight;
            m_maxAscender = state.maxAscender;
            m_maxDescender = state.maxDescender;
            m_maxLineAscender = state.maxLineAscender;
            m_maxLineDescender = state.maxLineDescender;
            m_startOfLineAscender = state.previousLineAscender;
            m_preferredWidth = state.preferredWidth;
            m_preferredHeight = state.preferredHeight;
            m_meshExtents = state.meshExtents;

            m_lineNumber = state.lineNumber;
            m_lineOffset = state.lineOffset;
            m_baselineOffset = state.baselineOffset;

            //m_lineJustification = state.alignment;
            m_htmlColor = state.vertexColor;
            m_underlineColor = state.underlineColor;
            m_strikethroughColor = state.strikethroughColor;
            m_highlightColor = state.highlightColor;

            m_isNonBreakingSpace = state.isNonBreakingSpace;
            tag_NoParsing = state.tagNoParsing;

            // XML Tag Stack
            m_fontStyleStack = state.basicStyleStack;
            m_colorStack = state.colorStack;
            m_underlineColorStack = state.underlineColorStack;
            m_strikethroughColorStack = state.strikethroughColorStack;
            m_highlightColorStack = state.highlightColorStack;
            m_colorGradientStack = state.colorGradientStack;
            m_sizeStack = state.sizeStack;
            m_indentStack = state.indentStack;
            m_FontWeightStack = state.fontWeightStack;
            m_styleStack = state.styleStack;
            m_baselineOffsetStack = state.baselineStack;
            m_actionStack = state.actionStack;
            m_materialReferenceStack = state.materialReferenceStack;
            m_lineJustificationStack = state.lineJustificationStack;
            //m_spriteAnimationStack = state.spriteAnimationStack;

            m_spriteAnimationID = state.spriteAnimationID;

            if (m_lineNumber < m_textInfo.lineInfo.Length)
                m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;

            return index;
        }


        /// <summary>
        /// Store vertex information for each character.
        /// </summary>
        /// <param name="style_padding">Style_padding.</param>
        /// <param name="vertexColor">Vertex color.</param>
        protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
        {
            // Save the Vertex Position for the Character
            #region Setup Mesh Vertices
            m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
            #endregion


            #region Setup Vertex Colors
            // Alpha is the lower of the vertex color or tag color alpha used.
            vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a) : (byte)(vertexColor.a);

            // Handle Vertex Colors & Vertex Color Gradient
            if (!m_enableVertexGradient)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
            }
            else
            {
                if (!m_overrideHtmlColors && m_colorStack.m_Index > 1)
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
                }
                else // Handle Vertex Color Gradient
                {
                    // Use Vertex Color Gradient Preset (if one is assigned)
                    if (m_fontColorGradientPreset != null)
                    {
                        m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradientPreset.bottomLeft * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradientPreset.topLeft * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradientPreset.topRight * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradientPreset.bottomRight * vertexColor;
                    }
                    else
                    {
                        m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradient.bottomLeft * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradient.topLeft * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradient.topRight * vertexColor;
                        m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradient.bottomRight * vertexColor;
                    }
                }
            }

            if (m_colorGradientPreset != null)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color *= m_colorGradientPreset.bottomLeft;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color *= m_colorGradientPreset.topLeft;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color *= m_colorGradientPreset.topRight;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color *= m_colorGradientPreset.bottomRight;
            }
            #endregion

            // Apply style_padding only if this is a SDF Shader.
            if (!m_isSDFShader)
                style_padding = 0f;


            // Setup UVs for the Character
            #region Setup UVs

            Vector2 uv0;
            uv0.x = (m_cached_TextElement.glyph.glyphRect.x - padding - style_padding) / m_currentFontAsset.atlasWidth;
            uv0.y = (m_cached_TextElement.glyph.glyphRect.y - padding - style_padding) / m_currentFontAsset.atlasHeight;

            Vector2 uv1;
            uv1.x = uv0.x;
            uv1.y = (m_cached_TextElement.glyph.glyphRect.y + padding + style_padding + m_cached_TextElement.glyph.glyphRect.height) / m_currentFontAsset.atlasHeight;

            Vector2 uv2;
            uv2.x = (m_cached_TextElement.glyph.glyphRect.x + padding + style_padding + m_cached_TextElement.glyph.glyphRect.width) / m_currentFontAsset.atlasWidth;
            uv2.y = uv1.y;

            Vector2 uv3;
            uv3.x = uv2.x;
            uv3.y = uv0.y;

            // Store UV Information
            m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv0;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv1;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv2;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv3;
            #endregion Setup UVs


            // Normal
            #region Setup Normals & Tangents
            //Vector3 normal = new Vector3(0, 0, -1);
            //m_textInfo.characterInfo[m_characterCount].vertex_BL.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_TL.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_TR.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_BR.normal = normal;

            // Tangents
            //Vector4 tangent = new Vector4(-1, 0, 0, 1);
            //m_textInfo.characterInfo[m_characterCount].vertex_BL.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_TL.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_TR.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_BR.tangent = tangent;
            #endregion end Normals & Tangents
        }


        /// <summary>
        /// Store vertex information for each sprite.
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="style_padding"></param>
        /// <param name="vertexColor"></param>
        protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
        {
            // Save the Vertex Position for the Character
            #region Setup Mesh Vertices
            m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
            #endregion

            // Vertex Color Alpha
            if (m_tintAllSprites) m_tintSprite = true;
            Color32 spriteColor = m_tintSprite ? m_spriteColor.Multiply(vertexColor) : m_spriteColor;
            spriteColor.a = spriteColor.a < m_fontColor32.a ? spriteColor.a = spriteColor.a < vertexColor.a ? spriteColor.a : vertexColor.a : m_fontColor32.a;

            Color32 c0 = spriteColor;
            Color32 c1 = spriteColor;
            Color32 c2 = spriteColor;
            Color32 c3 = spriteColor;

            if (m_enableVertexGradient)
            {
                if (m_fontColorGradientPreset != null)
                {
                    c0 = m_tintSprite ? c0.Multiply(m_fontColorGradientPreset.bottomLeft) : c0;
                    c1 = m_tintSprite ? c1.Multiply(m_fontColorGradientPreset.topLeft) : c1;
                    c2 = m_tintSprite ? c2.Multiply(m_fontColorGradientPreset.topRight) : c2;
                    c3 = m_tintSprite ? c3.Multiply(m_fontColorGradientPreset.bottomRight) : c3;
                }
                else
                {
                    c0 = m_tintSprite ? c0.Multiply(m_fontColorGradient.bottomLeft) : c0;
                    c1 = m_tintSprite ? c1.Multiply(m_fontColorGradient.topLeft) : c1;
                    c2 = m_tintSprite ? c2.Multiply(m_fontColorGradient.topRight) : c2;
                    c3 = m_tintSprite ? c3.Multiply(m_fontColorGradient.bottomRight) : c3;
                }
            }

            if (m_colorGradientPreset != null)
            {
                c0 = m_tintSprite ? c0.Multiply(m_colorGradientPreset.bottomLeft) : c0;
                c1 = m_tintSprite ? c1.Multiply(m_colorGradientPreset.topLeft) : c1;
                c2 = m_tintSprite ? c2.Multiply(m_colorGradientPreset.topRight) : c2;
                c3 = m_tintSprite ? c3.Multiply(m_colorGradientPreset.bottomRight) : c3;
            }

            m_textInfo.characterInfo[m_characterCount].vertex_BL.color = c0;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.color = c1;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.color = c2;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.color = c3;


            // Setup UVs for the Character
            #region Setup UVs
            Vector2 uv0 = new Vector2((float)m_cached_TextElement.glyph.glyphRect.x / m_currentSpriteAsset.spriteSheet.width, (float)m_cached_TextElement.glyph.glyphRect.y / m_currentSpriteAsset.spriteSheet.height);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, (float)(m_cached_TextElement.glyph.glyphRect.y + m_cached_TextElement.glyph.glyphRect.height) / m_currentSpriteAsset.spriteSheet.height);  // top left
            Vector2 uv2 = new Vector2((float)(m_cached_TextElement.glyph.glyphRect.x + m_cached_TextElement.glyph.glyphRect.width) / m_currentSpriteAsset.spriteSheet.width, uv1.y); // top right
            Vector2 uv3 = new Vector2(uv2.x, uv0.y); // bottom right

            // Store UV Information
            m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv0;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv1;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv2;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv3;
            #endregion Setup UVs


            // Normal
            #region Setup Normals & Tangents
            //Vector3 normal = new Vector3(0, 0, -1);
            //m_textInfo.characterInfo[m_characterCount].vertex_BL.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_TL.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_TR.normal = normal;
            //m_textInfo.characterInfo[m_characterCount].vertex_BR.normal = normal;

            // Tangents
            //Vector4 tangent = new Vector4(-1, 0, 0, 1);
            //m_textInfo.characterInfo[m_characterCount].vertex_BL.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_TL.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_TR.tangent = tangent;
            //m_textInfo.characterInfo[m_characterCount].vertex_BR.tangent = tangent;
            #endregion end Normals & Tangents

        }


        /// <summary>
        /// Store vertex attributes into the appropriate TMP_MeshInfo.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="index_X4"></param>
        protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            // Make sure buffers allocation are sufficient to hold the vertex data
            //if (m_textInfo.meshInfo[materialIndex].vertices.Length < index_X4 + 4)
            //    m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo(index_X4 + 4));


            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;


            // Setup UVS4
            //m_textInfo.meshInfo[0].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            //m_textInfo.meshInfo[0].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            //m_textInfo.meshInfo[0].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            //m_textInfo.meshInfo[0].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + 4;
        }


        protected virtual void FillCharacterVertexBuffers(int i, int index_X4, bool isVolumetric)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;

            if (isVolumetric)
            {
                Vector3 depth = new Vector3(0, 0, m_fontSize * m_fontScale);
                m_textInfo.meshInfo[materialIndex].vertices[4 + index_X4] = characterInfoArray[i].vertex_BL.position + depth;
                m_textInfo.meshInfo[materialIndex].vertices[5 + index_X4] = characterInfoArray[i].vertex_TL.position + depth;
                m_textInfo.meshInfo[materialIndex].vertices[6 + index_X4] = characterInfoArray[i].vertex_TR.position + depth;
                m_textInfo.meshInfo[materialIndex].vertices[7 + index_X4] = characterInfoArray[i].vertex_BR.position + depth;
            }

            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs0[4 + index_X4] = characterInfoArray[i].vertex_BL.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[5 + index_X4] = characterInfoArray[i].vertex_TL.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[6 + index_X4] = characterInfoArray[i].vertex_TR.uv;
                m_textInfo.meshInfo[materialIndex].uvs0[7 + index_X4] = characterInfoArray[i].vertex_BR.uv;
            }


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;

            if (isVolumetric)
            {
                m_textInfo.meshInfo[materialIndex].uvs2[4 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[5 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[6 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
                m_textInfo.meshInfo[materialIndex].uvs2[7 + index_X4] = characterInfoArray[i].vertex_BR.uv2;
            }


            // Setup UVS4
            //m_textInfo.meshInfo[0].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            //m_textInfo.meshInfo[0].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            //m_textInfo.meshInfo[0].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            //m_textInfo.meshInfo[0].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            if (isVolumetric)
            {
                Color32 backColor = new Color32(255, 255, 128, 255);
                m_textInfo.meshInfo[materialIndex].colors32[4 + index_X4] = backColor; //characterInfoArray[i].vertex_BL.color;
                m_textInfo.meshInfo[materialIndex].colors32[5 + index_X4] = backColor; //characterInfoArray[i].vertex_TL.color;
                m_textInfo.meshInfo[materialIndex].colors32[6 + index_X4] = backColor; //characterInfoArray[i].vertex_TR.color;
                m_textInfo.meshInfo[materialIndex].colors32[7 + index_X4] = backColor; //characterInfoArray[i].vertex_BR.color;
            }

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + (!isVolumetric ? 4 : 8);
        }


        /// <summary>
        /// Fill Vertex Buffers for Sprites
        /// </summary>
        /// <param name="i"></param>
        /// <param name="spriteIndex_X4"></param>
        protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
        {
            int materialIndex = m_textInfo.characterInfo[i].materialReferenceIndex;
            index_X4 = m_textInfo.meshInfo[materialIndex].vertexCount;

            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = index_X4;

            // Setup Vertices for Characters
            m_textInfo.meshInfo[materialIndex].vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo[materialIndex].vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo[materialIndex].vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo[materialIndex].vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo[materialIndex].uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo[materialIndex].uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo[materialIndex].uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo[materialIndex].uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;


            // Setup UVS4
            //m_textInfo.meshInfo[0].uvs4[0 + index_X4] = characterInfoArray[i].vertex_BL.uv4;
            //m_textInfo.meshInfo[0].uvs4[1 + index_X4] = characterInfoArray[i].vertex_TL.uv4;
            //m_textInfo.meshInfo[0].uvs4[2 + index_X4] = characterInfoArray[i].vertex_TR.uv4;
            //m_textInfo.meshInfo[0].uvs4[3 + index_X4] = characterInfoArray[i].vertex_BR.uv4;


            // setup Vertex Colors
            m_textInfo.meshInfo[materialIndex].colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo[materialIndex].colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo[materialIndex].colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo[materialIndex].colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;

            m_textInfo.meshInfo[materialIndex].vertexCount = index_X4 + 4;
        }


        /// <summary>
        /// Method to add the underline geometry.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startScale"></param>
        /// <param name="endScale"></param>
        /// <param name="maxScale"></param>
        /// <param name="underlineColor"></param>
        protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor)
        {
            if (m_cached_Underline_Character == null)
            {
                if (!TMP_Settings.warningsDisabled)
                    Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);

                return;
            }

            int verticesCount = index + 12;
            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (verticesCount > m_textInfo.meshInfo[0].vertices.Length)
            {
                // Resize Mesh Buffers
                m_textInfo.meshInfo[0].ResizeMeshInfo(verticesCount / 4);
            }

            // Adjust the position of the underline based on the lowest character. This matters for subscript character.
            start.y = Mathf.Min(start.y, end.y);
            end.y = Mathf.Min(start.y, end.y);

            float segmentWidth = m_cached_Underline_Character.glyph.metrics.width / 2 * maxScale;

            if (end.x - start.x < m_cached_Underline_Character.glyph.metrics.width * maxScale)
            {
                segmentWidth = (end.x - start.x) / 2f;
            }

            float startPadding = m_padding * startScale / maxScale;
            float endPadding = m_padding * endScale / maxScale;

            float underlineThickness = m_fontAsset.faceInfo.underlineThickness;

            // UNDERLINE VERTICES FOR (3) LINE SEGMENTS
            #region UNDERLINE VERTICES
            Vector3[] vertices = m_textInfo.meshInfo[0].vertices;

            // Front Part of the Underline
            vertices[index + 0] = start + new Vector3(0, 0 - (underlineThickness + m_padding) * maxScale, 0); // BL
            vertices[index + 1] = start + new Vector3(0, m_padding * maxScale, 0); // TL
            vertices[index + 2] = vertices[index + 1] + new Vector3(segmentWidth, 0, 0); // TR
            vertices[index + 3] = vertices[index + 0] + new Vector3(segmentWidth, 0, 0); // BR

            // Middle Part of the Underline
            vertices[index + 4] = vertices[index + 3]; // BL
            vertices[index + 5] = vertices[index + 2]; // TL
            vertices[index + 6] = end + new Vector3(-segmentWidth, m_padding * maxScale, 0);  // TR
            vertices[index + 7] = end + new Vector3(-segmentWidth, -(underlineThickness + m_padding) * maxScale, 0); // BR

            // End Part of the Underline
            vertices[index + 8] = vertices[index + 7]; // BL
            vertices[index + 9] = vertices[index + 6]; // TL
            vertices[index + 10] = end + new Vector3(0, m_padding * maxScale, 0); // TR
            vertices[index + 11] = end + new Vector3(0, -(underlineThickness + m_padding) * maxScale, 0); // BR
            #endregion

            // UNDERLINE UV0
            #region HANDLE UV0
            Vector2[] uvs0 = m_textInfo.meshInfo[0].uvs0;

            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2((m_cached_Underline_Character.glyph.glyphRect.x - startPadding) / m_fontAsset.atlasWidth, (m_cached_Underline_Character.glyph.glyphRect.y - m_padding) / m_fontAsset.atlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, (m_cached_Underline_Character.glyph.glyphRect.y + m_cached_Underline_Character.glyph.glyphRect.height + m_padding) / m_fontAsset.atlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_Underline_Character.glyph.glyphRect.x - startPadding + (float)m_cached_Underline_Character.glyph.glyphRect.width / 2) / m_fontAsset.atlasWidth, uv1.y); // Mid Top Left
            Vector2 uv3 = new Vector2(uv2.x, uv0.y); // Mid Bottom Left
            Vector2 uv4 = new Vector2((m_cached_Underline_Character.glyph.glyphRect.x + endPadding + (float)m_cached_Underline_Character.glyph.glyphRect.width / 2) / m_fontAsset.atlasWidth, uv1.y); // Mid Top Right
            Vector2 uv5 = new Vector2(uv4.x, uv0.y); // Mid Bottom right
            Vector2 uv6 = new Vector2((m_cached_Underline_Character.glyph.glyphRect.x + endPadding + m_cached_Underline_Character.glyph.glyphRect.width) / m_fontAsset.atlasWidth, uv1.y); // End Part - Bottom Right
            Vector2 uv7 = new Vector2(uv6.x, uv0.y); // End Part - Top Right

            // Left Part of the Underline
            uvs0[0 + index] = uv0; // BL
            uvs0[1 + index] = uv1; // TL
            uvs0[2 + index] = uv2; // TR
            uvs0[3 + index] = uv3; // BR

            // Middle Part of the Underline
            uvs0[4 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv0.y);
            uvs0[5 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv1.y);
            uvs0[6 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv1.y);
            uvs0[7 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv0.y);

            // Right Part of the Underline
            uvs0[8 + index] = uv5;
            uvs0[9 + index] = uv4;
            uvs0[10 + index] = uv6;
            uvs0[11 + index] = uv7;
            #endregion

            // UNDERLINE UV2
            #region HANDLE UV2 - SDF SCALE
            // UV1 contains Face / Border UV layout.
            float min_UvX = 0;
            float max_UvX = (vertices[index + 2].x - start.x) / (end.x - start.x);

            //Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            float xScale = Mathf.Abs(sdfScale);

            Vector2[] uvs2 = m_textInfo.meshInfo[0].uvs2;

            uvs2[0 + index] = PackUV(0, 0, xScale);
            uvs2[1 + index] = PackUV(0, 1, xScale);
            uvs2[2 + index] = PackUV(max_UvX, 1, xScale);
            uvs2[3 + index] = PackUV(max_UvX, 0, xScale);

            min_UvX = (vertices[index + 4].x - start.x) / (end.x - start.x);
            max_UvX = (vertices[index + 6].x - start.x) / (end.x - start.x);

            uvs2[4 + index] = PackUV(min_UvX, 0, xScale);
            uvs2[5 + index] = PackUV(min_UvX, 1, xScale);
            uvs2[6 + index] = PackUV(max_UvX, 1, xScale);
            uvs2[7 + index] = PackUV(max_UvX, 0, xScale);

            min_UvX = (vertices[index + 8].x - start.x) / (end.x - start.x);
            max_UvX = (vertices[index + 6].x - start.x) / (end.x - start.x);

            uvs2[8 + index] = PackUV(min_UvX, 0, xScale);
            uvs2[9 + index] = PackUV(min_UvX, 1, xScale);
            uvs2[10 + index] = PackUV(1, 1, xScale);
            uvs2[11 + index] = PackUV(1, 0, xScale);
            #endregion

            // UNDERLINE VERTEX COLORS
            #region
            // Alpha is the lower of the vertex color or tag color alpha used.
            underlineColor.a = m_fontColor32.a < underlineColor.a ? (byte)(m_fontColor32.a) : (byte)(underlineColor.a);

            Color32[] colors32 = m_textInfo.meshInfo[0].colors32;
            colors32[0 + index] = underlineColor;
            colors32[1 + index] = underlineColor;
            colors32[2 + index] = underlineColor;
            colors32[3 + index] = underlineColor;

            colors32[4 + index] = underlineColor;
            colors32[5 + index] = underlineColor;
            colors32[6 + index] = underlineColor;
            colors32[7 + index] = underlineColor;

            colors32[8 + index] = underlineColor;
            colors32[9 + index] = underlineColor;
            colors32[10 + index] = underlineColor;
            colors32[11 + index] = underlineColor;
            #endregion

            index += 12;
        }


        protected virtual void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
        {
            if (m_cached_Underline_Character == null)
            {
                if (!TMP_Settings.warningsDisabled) Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
                return;
            }

            int verticesCount = index + 4;
            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (verticesCount > m_textInfo.meshInfo[0].vertices.Length)
            {
                // Resize Mesh Buffers
                m_textInfo.meshInfo[0].ResizeMeshInfo(verticesCount / 4);
            }

            // UNDERLINE VERTICES FOR (3) LINE SEGMENTS
            #region HIGHLIGHT VERTICES
            Vector3[] vertices = m_textInfo.meshInfo[0].vertices;

            // Front Part of the Underline
            vertices[index + 0] = start; // BL
            vertices[index + 1] = new Vector3(start.x, end.y, 0); // TL
            vertices[index + 2] = end; // TR
            vertices[index + 3] = new Vector3(end.x, start.y, 0); // BR
            #endregion

            // UNDERLINE UV0
            #region HANDLE UV0
            Vector2[] uvs0 = m_textInfo.meshInfo[0].uvs0;

            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2(((float)m_cached_Underline_Character.glyph.glyphRect.x + m_cached_Underline_Character.glyph.glyphRect.width / 2) / m_fontAsset.atlasWidth, (m_cached_Underline_Character.glyph.glyphRect.y + (float)m_cached_Underline_Character.glyph.glyphRect.height / 2) / m_fontAsset.atlasHeight);  // bottom left
            //Vector2 uv1 = new Vector2(uv0.x, uv0.y);  // top left
            //Vector2 uv2 = new Vector2(uv0.x, uv0.y); // Top Right
            //Vector2 uv3 = new Vector2(uv2.x, uv0.y); // Bottom Right

            // Left Part of the Underline
            uvs0[0 + index] = uv0; // BL
            uvs0[1 + index] = uv0; // TL
            uvs0[2 + index] = uv0; // TR
            uvs0[3 + index] = uv0; // BR
            #endregion

            // UNDERLINE UV2
            #region HANDLE UV2 - SDF SCALE
            // UV1 contains Face / Border UV layout.
            //float min_UvX = 0;
            //float max_UvX = (vertices[index + 2].x - start.x) / (end.x - start.x);

            ////Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            //float xScale = 0; // Mathf.Abs(sdfScale);

            Vector2[] uvs2 = m_textInfo.meshInfo[0].uvs2;
            Vector2 customUV = new Vector2(0, 1);
            uvs2[0 + index] = customUV; // PackUV(-0.2f, -0.2f, xScale);
            uvs2[1 + index] = customUV; // PackUV(-0.2f, -0.1f, xScale);
            uvs2[2 + index] = customUV; // PackUV(-0.1f, -0.1f, xScale);
            uvs2[3 + index] = customUV; // PackUV(-0.1f, -0.2f, xScale);
            #endregion

            // HIGHLIGHT VERTEX COLORS
            #region
            // Alpha is the lower of the vertex color or tag color alpha used.
            highlightColor.a = m_fontColor32.a < highlightColor.a ? m_fontColor32.a : highlightColor.a;

            Color32[] colors32 = m_textInfo.meshInfo[0].colors32;
            colors32[0 + index] = highlightColor;
            colors32[1 + index] = highlightColor;
            colors32[2 + index] = highlightColor;
            colors32[3 + index] = highlightColor;
            #endregion

            index += 4;
        }


        /// <summary>
        /// Internal function used to load the default settings of text objects.
        /// </summary>
        protected void LoadDefaultSettings()
        {
            if (m_text == null || m_isWaitingOnResourceLoad)
            {
                if (TMP_Settings.autoSizeTextContainer)
                    autoSizeTextContainer = true;
                else
                {
                    m_rectTransform = this.rectTransform;

                    if (GetType() == typeof(TextMeshPro))
                        m_rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProTextContainerSize;
                    else
                        m_rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProUITextContainerSize;
                }

                m_enableWordWrapping = TMP_Settings.enableWordWrapping;
                m_enableKerning = TMP_Settings.enableKerning;
                m_enableExtraPadding = TMP_Settings.enableExtraPadding;
                m_tintAllSprites = TMP_Settings.enableTintAllSprites;
                m_parseCtrlCharacters = TMP_Settings.enableParseEscapeCharacters;
                m_fontSize = m_fontSizeBase = TMP_Settings.defaultFontSize;
                m_fontSizeMin = m_fontSize * TMP_Settings.defaultTextAutoSizingMinRatio;
                m_fontSizeMax = m_fontSize * TMP_Settings.defaultTextAutoSizingMaxRatio;
                m_isWaitingOnResourceLoad = false;
                raycastTarget = TMP_Settings.enableRaycastTarget;
            }
        }


        /// <summary>
        /// Method used to find and cache references to the Underline and Ellipsis characters.
        /// </summary>
        /// <param name=""></param>
        protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
        {
            // Check & Assign Underline Character for use with the Underline tag.
            if (!fontAsset.characterLookupTable.TryGetValue(95, out m_cached_Underline_Character))
            {
                m_cached_Underline_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(95,fontAsset, false, m_FontStyleInternal, (FontWeight)m_FontWeightInternal, out bool isUsingAlternativeTypeface, out TMP_FontAsset tempFontAsset);

                if (m_cached_Underline_Character == null)
            {
                    if (!TMP_Settings.warningsDisabled)
                        Debug.LogWarning("The character used for Underline and Strikethrough is not available in font asset [" + fontAsset.name + "].", this);
                }
            }

            // Check & Assign Underline Character for use with the Underline tag.
            if (!fontAsset.characterLookupTable.TryGetValue(8230, out m_cached_Ellipsis_Character)) //95
            {
                m_cached_Ellipsis_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(8230, fontAsset, false, m_FontStyleInternal, (FontWeight)m_FontWeightInternal, out bool isUsingAlternativeTypeface, out TMP_FontAsset tempFontAsset);

                if (m_cached_Ellipsis_Character == null)
            {
                    if (!TMP_Settings.warningsDisabled)
                        Debug.LogWarning("The character used for Ellipsis is not available in font asset [" + fontAsset.name + "].", this);
                }
            }
        }


        /// <summary>
        /// Replace a given number of characters (tag) in the array with a new character and shift subsequent characters in the array.
        /// </summary>
        /// <param name="chars">Array which contains the text.</param>
        /// <param name="insertionIndex">The index of where the new character will be inserted</param>
        /// <param name="tagLength">Length of the tag being replaced.</param>
        /// <param name="c">The replacement character.</param>
        protected void ReplaceTagWithCharacter(int[] chars, int insertionIndex, int tagLength, char c)
        {
            chars[insertionIndex] = c;

            for (int i = insertionIndex + tagLength; i < chars.Length; i++)
            {
                chars[i - 3] = chars[i];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //protected int GetMaterialReferenceForFontWeight()
        //{
        //    //bool isItalic = (m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic;

        //    m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentFontAsset.fontWeights[0].italicTypeface.material, m_currentFontAsset.fontWeights[0].italicTypeface, m_materialReferences, m_materialReferenceIndexLookup);

        //    return 0;
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
        {
            bool isItalic = (m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic;

            TMP_FontAsset fontAsset = null;

            int weightIndex = fontWeight / 100;

            if (isItalic)
                fontAsset = m_currentFontAsset.fontWeightTable[weightIndex].italicTypeface;
            else
                fontAsset = m_currentFontAsset.fontWeightTable[weightIndex].regularTypeface;

            return fontAsset;
        }


        /// <summary>
        /// Method to Enable or Disable child SubMesh objects.
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetActiveSubMeshes(bool state) { }


        /// <summary>
        /// Destroy Sub Mesh Objects.
        /// </summary>
        protected virtual void ClearSubMeshObjects() { }


        /// <summary>
        /// Function to clear the geometry of the Primary and Sub Text objects.
        /// </summary>
        public virtual void ClearMesh() { }


        /// <summary>
        /// Function to clear the geometry of the Primary and Sub Text objects.
        /// </summary>
        public virtual void ClearMesh(bool uploadGeometry) { }


        /// <summary>
        /// Function which returns the text after it has been parsed and rich text tags removed.
        /// </summary>
        /// <returns></returns>
        public virtual string GetParsedText()
        {
            if (m_textInfo == null)
                return string.Empty;

            int characterCount = m_textInfo.characterCount;

            // TODO - Could implement some static buffer pool shared by all instances of TMP objects.
            char[] buffer = new char[characterCount];

            for (int i = 0; i < characterCount && i < m_textInfo.characterInfo.Length; i++)
            {
                buffer[i] = m_textInfo.characterInfo[i].character;
            }

            return new string(buffer);
        }


        /// <summary>
        /// Function to pack scale information in the UV2 Channel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        //protected Vector2 PackUV(float x, float y, float scale)
        //{
        //    Vector2 output;

        //    output.x = Mathf.Floor(x * 4095);
        //    output.y = Mathf.Floor(y * 4095);

        //    output.x = (output.x * 4096) + output.y;
        //    output.y = scale;

        //    return output;
        //}

        /// <summary>
        /// Function to pack scale information in the UV2 Channel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        protected Vector2 PackUV(float x, float y, float scale)
        {
            Vector2 output;

            output.x = (int)(x * 511);
            output.y = (int)(y * 511);

            output.x = (output.x * 4096) + output.y;
            output.y = scale;

            return output;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected float PackUV(float x, float y)
        {
            double x0 = (int)(x * 511);
            double y0 = (int)(y * 511);

            return (float)((x0 * 4096) + y0);
        }


        /// <summary>
        /// Function used as a replacement for LateUpdate()
        /// </summary>
        internal virtual void InternalUpdate() { }


        /// <summary>
        /// Function to pack scale information in the UV2 Channel.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        //protected Vector2 PackUV(float x, float y, float scale)
        //{
        //    Vector2 output;

        //    output.x = Mathf.Floor(x * 4095);
        //    output.y = Mathf.Floor(y * 4095);

        //    return new Vector2((output.x * 4096) + output.y, scale);
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        //protected float PackUV(float x, float y)
        //{
        //    x = (x % 5) / 5;
        //    y = (y % 5) / 5;

        //    return Mathf.Round(x * 4096) + y;
        //}


        /// <summary>
        /// Method to convert Hex to Int
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        protected int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
            }
            return 15;
        }


        /// <summary>
        /// Convert UTF-16 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        protected int GetUTF16(string text, int i)
        {
            int unicode = 0;
            unicode += HexToInt(text[i]) << 12;
            unicode += HexToInt(text[i + 1]) << 8;
            unicode += HexToInt(text[i + 2]) << 4;
            unicode += HexToInt(text[i + 3]);
            return unicode;
        }

        /// <summary>
        /// Convert UTF-16 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        protected int GetUTF16(StringBuilder text, int i)
        {
            int unicode = 0;
            unicode += HexToInt(text[i]) << 12;
            unicode += HexToInt(text[i + 1]) << 8;
            unicode += HexToInt(text[i + 2]) << 4;
            unicode += HexToInt(text[i + 3]);
            return unicode;
        }


        /// <summary>
        /// Convert UTF-32 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        protected int GetUTF32(string text, int i)
        {
            int unicode = 0;
            unicode += HexToInt(text[i]) << 30;
            unicode += HexToInt(text[i + 1]) << 24;
            unicode += HexToInt(text[i + 2]) << 20;
            unicode += HexToInt(text[i + 3]) << 16;
            unicode += HexToInt(text[i + 4]) << 12;
            unicode += HexToInt(text[i + 5]) << 8;
            unicode += HexToInt(text[i + 6]) << 4;
            unicode += HexToInt(text[i + 7]);
            return unicode;
        }

        /// <summary>
        /// Convert UTF-32 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        protected int GetUTF32(StringBuilder text, int i)
        {
            int unicode = 0;
            unicode += HexToInt(text[i]) << 30;
            unicode += HexToInt(text[i + 1]) << 24;
            unicode += HexToInt(text[i + 2]) << 20;
            unicode += HexToInt(text[i + 3]) << 16;
            unicode += HexToInt(text[i + 4]) << 12;
            unicode += HexToInt(text[i + 5]) << 8;
            unicode += HexToInt(text[i + 6]) << 4;
            unicode += HexToInt(text[i + 7]);
            return unicode;
        }


        /// <summary>
        /// Method to convert Hex color values to Color32
        /// </summary>
        /// <param name="hexChars"></param>
        /// <param name="tagCount"></param>
        /// <returns></returns>
        protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
        {
            if (tagCount == 4)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[1]));
                byte g = (byte)(HexToInt(hexChars[2]) * 16 + HexToInt(hexChars[2]));
                byte b = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[3]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 5)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[1]));
                byte g = (byte)(HexToInt(hexChars[2]) * 16 + HexToInt(hexChars[2]));
                byte b = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[3]));
                byte a = (byte)(HexToInt(hexChars[4]) * 16 + HexToInt(hexChars[4]));

                return new Color32(r, g, b, a);
            }
            else if (tagCount == 7)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 9)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
                byte a = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));

                return new Color32(r, g, b, a);
            }
            else if (tagCount == 10)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[7]));
                byte g = (byte)(HexToInt(hexChars[8]) * 16 + HexToInt(hexChars[8]));
                byte b = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[9]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 11)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[7]));
                byte g = (byte)(HexToInt(hexChars[8]) * 16 + HexToInt(hexChars[8]));
                byte b = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[9]));
                byte a = (byte)(HexToInt(hexChars[10]) * 16 + HexToInt(hexChars[10]));

                return new Color32(r, g, b, a);
            }
            else if (tagCount == 13)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 15)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
                byte a = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));

                return new Color32(r, g, b, a);
            }

            return new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Method to convert Hex Color values to Color32
        /// </summary>
        /// <param name="hexChars"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
        {
            if (length == 7)
            {
                byte r = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
                byte g = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
                byte b = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));

                return new Color32(r, g, b, 255);
            }
            else if (length == 9)
            {
                byte r = (byte)(HexToInt(hexChars[startIndex + 1]) * 16 + HexToInt(hexChars[startIndex + 2]));
                byte g = (byte)(HexToInt(hexChars[startIndex + 3]) * 16 + HexToInt(hexChars[startIndex + 4]));
                byte b = (byte)(HexToInt(hexChars[startIndex + 5]) * 16 + HexToInt(hexChars[startIndex + 6]));
                byte a = (byte)(HexToInt(hexChars[startIndex + 7]) * 16 + HexToInt(hexChars[startIndex + 8]));

                return new Color32(r, g, b, a);
            }

            return s_colorWhite;
        }


        /// <summary>
        /// Method which returns the number of parameters used in a tag attribute and populates an array with such values.
        /// </summary>
        /// <param name="chars">Char[] containing the tag attribute and data</param>
        /// <param name="startIndex">The index of the first char of the data</param>
        /// <param name="length">The length of the data</param>
        /// <param name="parameters">The number of parameters contained in the Char[]</param>
        /// <returns></returns>
        int GetAttributeParameters(char[] chars, int startIndex, int length, ref float[] parameters)
        {
            int endIndex = startIndex;
            int attributeCount = 0;

            while (endIndex < startIndex + length)
            {
                parameters[attributeCount] = ConvertToFloat(chars, startIndex, length, out endIndex);

                length -= (endIndex - startIndex) + 1;
                startIndex = endIndex + 1;

                attributeCount += 1;
            }

            return attributeCount;
        }


        /// <summary>
        /// Extracts a float value from char[] assuming we know the position of the start, end and decimal point.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected float ConvertToFloat(char[] chars, int startIndex, int length)
        {
            return ConvertToFloat(chars, startIndex, length, out int lastIndex);
        }


        /// <summary>
        /// Extracts a float value from char[] given a start index and length. 
        /// </summary>
        /// <param name="chars"></param> The Char[] containing the numerical sequence.
        /// <param name="startIndex"></param> The index of the start of the numerical sequence.
        /// <param name="length"></param> The length of the numerical sequence.
        /// <param name="lastIndex"></param> Index of the last character in the validated sequence.
        /// <returns></returns>
        protected float ConvertToFloat(char[] chars, int startIndex, int length, out int lastIndex)
        {
            if (startIndex == 0) { lastIndex = 0; return -9999; }
            int endIndex = startIndex + length;

            bool isIntegerValue = true;
            float decimalPointMultiplier = 0;

            // Set value multiplier checking the first character to determine if we are using '+' or '-'
            int valueSignMultiplier = 1;
            if (chars[startIndex] == '+')
            {
                valueSignMultiplier = 1;
                startIndex += 1;
            }
            else if (chars[startIndex] == '-')
            {
                valueSignMultiplier = -1;
                startIndex += 1;
            }

            float value = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                uint c = chars[i];

                if (c >= '0' && c <= '9' || c == '.')
                {
                    if (c == '.')
                    {
                        isIntegerValue = false;
                        decimalPointMultiplier = 0.1f;
                        continue;
                    }

                    //Calculate integer and floating point value
                    if (isIntegerValue)
                        value = value * 10 + (c - 48) * valueSignMultiplier;
                    else
                {
                        value = value + (c - 48) * decimalPointMultiplier * valueSignMultiplier;
                        decimalPointMultiplier *= 0.1f;
                }

                    continue;
                }
                else if (c == ',')
                {
                    if (i + 1 < endIndex && chars[i + 1] == ' ')
                        lastIndex = i + 1;
                else
                        lastIndex = i;

                    return value;
                }
            }

            lastIndex = endIndex;
            return value;
        }


        /// <summary>
        /// Function to identify and validate the rich tag. Returns the position of the > if the tag was valid.
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        protected bool ValidateHtmlTag(UnicodeChar[] chars, int startIndex, out int endIndex)
        {
            int tagCharCount = 0;
            byte attributeFlag = 0;

            int attributeIndex = 0;
            m_xmlAttribute[attributeIndex].nameHashCode = 0;
            m_xmlAttribute[attributeIndex].valueHashCode = 0;
            m_xmlAttribute[attributeIndex].valueStartIndex = 0;
            m_xmlAttribute[attributeIndex].valueLength = 0;
            TagValueType tagValueType = m_xmlAttribute[attributeIndex].valueType = TagValueType.None;
            TagUnitType tagUnitType = m_xmlAttribute[attributeIndex].unitType = TagUnitType.Pixels;

            // Clear attribute name hash codes
            m_xmlAttribute[1].nameHashCode = 0;
            m_xmlAttribute[2].nameHashCode = 0;
            m_xmlAttribute[3].nameHashCode = 0;
            m_xmlAttribute[4].nameHashCode = 0;

            endIndex = startIndex;
            bool isTagSet = false;
            bool isValidHtmlTag = false;

            for (int i = startIndex; i < chars.Length && chars[i].unicode != 0 && tagCharCount < m_htmlTag.Length && chars[i].unicode != '<'; i++)
            {
                int unicode = chars[i].unicode;

                if (unicode == '>') // ASCII Code of End HTML tag '>'
                {
                    isValidHtmlTag = true;
                    endIndex = i;
                    m_htmlTag[tagCharCount] = (char)0;
                    break;
                }

                m_htmlTag[tagCharCount] = (char)unicode;
                tagCharCount += 1;

                if (attributeFlag == 1)
                {
                    if (tagValueType == TagValueType.None)
                    {
                        // Check for attribute type
                        if (unicode == '+' || unicode == '-' || unicode == '.' || (unicode >= '0' && unicode <= '9'))
                        {
                            tagUnitType = TagUnitType.Pixels;
                            tagValueType = m_xmlAttribute[attributeIndex].valueType = TagValueType.NumericalValue;
                            m_xmlAttribute[attributeIndex].valueStartIndex = tagCharCount - 1;
                            m_xmlAttribute[attributeIndex].valueLength += 1;
                        }
                        else if (unicode == '#')
                        {
                            tagUnitType = TagUnitType.Pixels;
                            tagValueType = m_xmlAttribute[attributeIndex].valueType = TagValueType.ColorValue;
                            m_xmlAttribute[attributeIndex].valueStartIndex = tagCharCount - 1;
                            m_xmlAttribute[attributeIndex].valueLength += 1;
                        }
                        else if (unicode == '"')
                        {
                            tagUnitType = TagUnitType.Pixels;
                            tagValueType = m_xmlAttribute[attributeIndex].valueType = TagValueType.StringValue;
                            m_xmlAttribute[attributeIndex].valueStartIndex = tagCharCount;
                        }
                        else
                        {
                            tagUnitType = TagUnitType.Pixels;
                            tagValueType = m_xmlAttribute[attributeIndex].valueType = TagValueType.StringValue;
                            m_xmlAttribute[attributeIndex].valueStartIndex = tagCharCount - 1;
                            m_xmlAttribute[attributeIndex].valueHashCode = (m_xmlAttribute[attributeIndex].valueHashCode << 5) + m_xmlAttribute[attributeIndex].valueHashCode ^ unicode;
                            m_xmlAttribute[attributeIndex].valueLength += 1;
                        }
                    }
                    else
                    {
                        if (tagValueType == TagValueType.NumericalValue)
                        {
                            // Check for termination of numerical value.
                            if (unicode == 'p' || unicode == 'e' || unicode == '%' || unicode == ' ')
                            {
                                attributeFlag = 2;
                                tagValueType = TagValueType.None;

                                switch (unicode)
                                {
                                    case 'e':
                                        m_xmlAttribute[attributeIndex].unitType = tagUnitType = TagUnitType.FontUnits;
                                        break;
                                    case '%':
                                        m_xmlAttribute[attributeIndex].unitType = tagUnitType = TagUnitType.Percentage;
                                        break;
                                    default:
                                        m_xmlAttribute[attributeIndex].unitType = tagUnitType = TagUnitType.Pixels;
                                        break;
                                }

                                attributeIndex += 1;
                                m_xmlAttribute[attributeIndex].nameHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueType = TagValueType.None;
                                m_xmlAttribute[attributeIndex].unitType = TagUnitType.Pixels;
                                m_xmlAttribute[attributeIndex].valueStartIndex = 0;
                                m_xmlAttribute[attributeIndex].valueLength = 0;

                            }
                            else if (attributeFlag != 2)
                            {
                                m_xmlAttribute[attributeIndex].valueLength += 1;
                            }
                        }
                        else if (tagValueType == TagValueType.ColorValue)
                        {
                            if (unicode != ' ')
                            {
                                m_xmlAttribute[attributeIndex].valueLength += 1;
                            }
                            else
                            {
                                attributeFlag = 2;
                                tagValueType = TagValueType.None;
                                tagUnitType = TagUnitType.Pixels;
                                attributeIndex += 1;
                                m_xmlAttribute[attributeIndex].nameHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueType = TagValueType.None;
                                m_xmlAttribute[attributeIndex].unitType = TagUnitType.Pixels;
                                m_xmlAttribute[attributeIndex].valueHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueStartIndex = 0;
                                m_xmlAttribute[attributeIndex].valueLength = 0;
                            }
                        }
                        else if (tagValueType == TagValueType.StringValue)
                        {
                            // Compute HashCode value for the named tag.
                            if (unicode != '"')
                            {
                                m_xmlAttribute[attributeIndex].valueHashCode = (m_xmlAttribute[attributeIndex].valueHashCode << 5) + m_xmlAttribute[attributeIndex].valueHashCode ^ unicode;
                                m_xmlAttribute[attributeIndex].valueLength += 1;
                            }
                            else
                            {
                                attributeFlag = 2;
                                tagValueType = TagValueType.None;
                                tagUnitType = TagUnitType.Pixels;
                                attributeIndex += 1;
                                m_xmlAttribute[attributeIndex].nameHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueType = TagValueType.None;
                                m_xmlAttribute[attributeIndex].unitType = TagUnitType.Pixels;
                                m_xmlAttribute[attributeIndex].valueHashCode = 0;
                                m_xmlAttribute[attributeIndex].valueStartIndex = 0;
                                m_xmlAttribute[attributeIndex].valueLength = 0;
                            }
                        }
                    }
                }


                if (unicode == '=') // '=' 
                    attributeFlag = 1;

                // Compute HashCode for the name of the attribute
                if (attributeFlag == 0 && unicode == ' ')
                {
                    if (isTagSet) return false;

                    isTagSet = true;
                    attributeFlag = 2;

                    tagValueType = TagValueType.None;
                    tagUnitType = TagUnitType.Pixels;
                    attributeIndex += 1;
                    m_xmlAttribute[attributeIndex].nameHashCode = 0;
                    m_xmlAttribute[attributeIndex].valueType = TagValueType.None;
                    m_xmlAttribute[attributeIndex].unitType = TagUnitType.Pixels;
                    m_xmlAttribute[attributeIndex].valueHashCode = 0;
                    m_xmlAttribute[attributeIndex].valueStartIndex = 0;
                    m_xmlAttribute[attributeIndex].valueLength = 0;
                }

                if (attributeFlag == 0)
                    m_xmlAttribute[attributeIndex].nameHashCode = (m_xmlAttribute[attributeIndex].nameHashCode << 3) - m_xmlAttribute[attributeIndex].nameHashCode + unicode;

                if (attributeFlag == 2 && unicode == ' ')
                    attributeFlag = 0;

            }

            if (!isValidHtmlTag)
            {
                return false;
            }

            //Debug.Log("Tag is [" + m_htmlTag.ArrayToString() + "].  Tag HashCode: " + m_xmlAttribute[0].nameHashCode + "  Tag Value HashCode: " + m_xmlAttribute[0].valueHashCode + "  Attribute 1 HashCode: " + m_xmlAttribute[1].nameHashCode + " Value HashCode: " + m_xmlAttribute[1].valueHashCode);
            //for (int i = 0; i < attributeIndex; i++)
            //    Debug.Log("Tag [" + i + "] with HashCode: " + m_xmlAttribute[i].nameHashCode + " has value of [" + new string(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength) + "] Numerical Value: " + ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength));

            #region Rich Text Tag Processing
            #if !RICH_TEXT_ENABLED
            // Special handling of the no parsing tag </noparse> </NOPARSE> tag
            if (tag_NoParsing && (m_xmlAttribute[0].nameHashCode != 53822163 && m_xmlAttribute[0].nameHashCode != 49429939))
                return false;
            else if (m_xmlAttribute[0].nameHashCode == 53822163 || m_xmlAttribute[0].nameHashCode == 49429939)
            {
                tag_NoParsing = false;
                return true;
            }

            // Color <#FFF> 3 Hex values (short form)
            if (m_htmlTag[0] == 35 && tagCharCount == 4)
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack.Add(m_htmlColor);
                return true;
            }
            // Color <#FFF7> 4 Hex values with alpha (short form)
            else if (m_htmlTag[0] == 35 && tagCharCount == 5)
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack.Add(m_htmlColor);
                return true;
            }
            // Color <#FF00FF>
            else if (m_htmlTag[0] == 35 && tagCharCount == 7) // if Tag begins with # and contains 7 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack.Add(m_htmlColor);
                return true;
            }
            // Color <#FF00FF00> with alpha
            else if (m_htmlTag[0] == 35 && tagCharCount == 9) // if Tag begins with # and contains 9 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack.Add(m_htmlColor);
                return true;
            }
            else
            {
                float value = 0;

                switch (m_xmlAttribute[0].nameHashCode)
                {
                    case 98: // <b>
                    case 66: // <B>
                        m_FontStyleInternal |= FontStyles.Bold;
                        m_fontStyleStack.Add(FontStyles.Bold);

                        m_FontWeightInternal = FontWeight.Bold;
                        return true;
                    case 427: // </b>
                    case 395: // </B>
                        if ((m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.Bold) == 0)
                            {
                                m_FontStyleInternal &= ~FontStyles.Bold;
                                m_FontWeightInternal = m_FontWeightStack.Peek();
                            }
                        }
                        return true;
                    case 105: // <i>
                    case 73: // <I>
                        m_FontStyleInternal |= FontStyles.Italic;
                        m_fontStyleStack.Add(FontStyles.Italic);
                        return true;
                    case 434: // </i>
                    case 402: // </I>
                        if ((m_fontStyle & FontStyles.Italic) != FontStyles.Italic)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.Italic) == 0)
                                m_FontStyleInternal &= ~FontStyles.Italic;
                        }
                        return true;
                    case 115: // <s>
                    case 83: // <S>
                        m_FontStyleInternal |= FontStyles.Strikethrough;
                        m_fontStyleStack.Add(FontStyles.Strikethrough);

                        if (m_xmlAttribute[1].nameHashCode == 281955 || m_xmlAttribute[1].nameHashCode == 192323)
                        {
                            m_strikethroughColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);
                            m_strikethroughColor.a = m_htmlColor.a < m_strikethroughColor.a ? (byte)(m_htmlColor.a) : (byte)(m_strikethroughColor .a);
                        }
                        else
                            m_strikethroughColor = m_htmlColor;

                        m_strikethroughColorStack.Add(m_strikethroughColor);

                        return true;
                    case 444: // </s>
                    case 412: // </S>
                        if ((m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.Strikethrough) == 0)
                                m_FontStyleInternal &= ~FontStyles.Strikethrough;
                        }
                        return true;
                    case 117: // <u>
                    case 85: // <U>
                        m_FontStyleInternal |= FontStyles.Underline;
                        m_fontStyleStack.Add(FontStyles.Underline);

                        if (m_xmlAttribute[1].nameHashCode == 281955 || m_xmlAttribute[1].nameHashCode == 192323)
                        {
                            m_underlineColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);
                            m_underlineColor.a = m_htmlColor.a < m_underlineColor.a ? (byte)(m_htmlColor.a) : (byte)(m_underlineColor.a);
                        }
                        else
                            m_underlineColor = m_htmlColor;

                        m_underlineColorStack.Add(m_underlineColor);

                        return true;
                    case 446: // </u>
                    case 414: // </U>
                        if ((m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
                        {
                            m_underlineColor = m_underlineColorStack.Remove();

                            if (m_fontStyleStack.Remove(FontStyles.Underline) == 0)
                                m_FontStyleInternal &= ~FontStyles.Underline;
                        }
                        return true;
                    case 43045: // <mark=#FF00FF80>
                    case 30245: // <MARK>
                        m_FontStyleInternal |= FontStyles.Highlight;
                        m_fontStyleStack.Add(FontStyles.Highlight);

                        m_highlightColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        m_highlightColor.a = m_htmlColor.a < m_highlightColor.a ? (byte)(m_htmlColor.a) : (byte)(m_highlightColor.a);
                        m_highlightColorStack.Add(m_highlightColor);

                        // Handle Mark Tag Attributes
                        for (int i = 0; i < m_xmlAttribute.Length && m_xmlAttribute[i].nameHashCode != 0; i++)
                        {
                            int nameHashCode = m_xmlAttribute[i].nameHashCode;

                            switch (nameHashCode)
                            {
                                case 281955: // color
                                    break;

                                case 15087385: // padding
                                    int paramCount = GetAttributeParameters(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength, ref m_attributeParameterValues);
                                    if (paramCount != 4) return false;

                                    m_highlightPadding = new Vector4(m_attributeParameterValues[0], m_attributeParameterValues[1], m_attributeParameterValues[2], m_attributeParameterValues[3]); 
                                    break;
                            }
                        }

                        return true;
                    case 155892: // </mark>
                    case 143092: // </MARK>
                        if ((m_fontStyle & FontStyles.Highlight) != FontStyles.Highlight)
                        {
                            m_highlightColor = m_highlightColorStack.Remove();

                            if (m_fontStyleStack.Remove(FontStyles.Highlight) == 0)
                                m_FontStyleInternal &= ~FontStyles.Highlight;
                        }
                        return true;
                    case 6552: // <sub>
                    case 4728: // <SUB>
                        m_fontScaleMultiplier *= m_currentFontAsset.faceInfo.subscriptSize > 0 ? m_currentFontAsset.faceInfo.subscriptSize : 1;
                        m_baselineOffsetStack.Push(m_baselineOffset);
                        m_baselineOffset += m_currentFontAsset.faceInfo.subscriptOffset * m_fontScale * m_fontScaleMultiplier;

                        m_fontStyleStack.Add(FontStyles.Subscript);
                        m_FontStyleInternal |= FontStyles.Subscript;
                        return true;
                    case 22673: // </sub>
                    case 20849: // </SUB>
                        if ((m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript)
                        {
                            if (m_fontScaleMultiplier < 1)
                            {
                                //m_baselineOffset -= m_currentFontAsset.fontInfo.SubscriptOffset * m_fontScale * m_fontScaleMultiplier;
                                m_baselineOffset = m_baselineOffsetStack.Pop();
                                m_fontScaleMultiplier /= m_currentFontAsset.faceInfo.subscriptSize > 0 ? m_currentFontAsset.faceInfo.subscriptSize : 1;
                            }

                            if (m_fontStyleStack.Remove(FontStyles.Subscript) == 0)
                                m_FontStyleInternal &= ~FontStyles.Subscript;
                        }
                        return true;
                    case 6566: // <sup>
                    case 4742: // <SUP>
                        m_fontScaleMultiplier *= m_currentFontAsset.faceInfo.superscriptSize > 0 ? m_currentFontAsset.faceInfo.superscriptSize : 1;
                        m_baselineOffsetStack.Push(m_baselineOffset);
                        m_baselineOffset += m_currentFontAsset.faceInfo.superscriptOffset * m_fontScale * m_fontScaleMultiplier;

                        m_fontStyleStack.Add(FontStyles.Superscript);
                        m_FontStyleInternal |= FontStyles.Superscript;
                        return true;
                    case 22687: // </sup>
                    case 20863: // </SUP>
                        if ((m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript)
                        {
                            if (m_fontScaleMultiplier < 1)
                            {
                                //m_baselineOffset -= m_currentFontAsset.fontInfo.SuperscriptOffset * m_fontScale * m_fontScaleMultiplier;
                                m_baselineOffset = m_baselineOffsetStack.Pop();
                                m_fontScaleMultiplier /= m_currentFontAsset.faceInfo.superscriptSize > 0 ? m_currentFontAsset.faceInfo.superscriptSize : 1;
                            }

                            if (m_fontStyleStack.Remove(FontStyles.Superscript) == 0)
                                m_FontStyleInternal &= ~FontStyles.Superscript;
                        }
                        return true;
                    case -330774850: // <font-weight>
                    case 2012149182: // <FONT-WEIGHT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);

                        //if (value == -9999) return false;

                        //if ((m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
                        //{
                        //    // Nothing happens since Bold is forced on the text.
                        //    //m_fontWeight = 700;
                        //    return true;
                        //}


                        //// Remove bold style
                        //m_style &= ~FontStyles.Bold;

                        switch ((int)value)
                        {
                            case 100:
                                m_FontWeightInternal = FontWeight.Thin;
                                break;
                            case 200:
                                m_FontWeightInternal = FontWeight.ExtraLight;
                                break;
                            case 300:
                                m_FontWeightInternal = FontWeight.Light;
                                break;
                            case 400:
                                m_FontWeightInternal = FontWeight.Regular;
                                break;
                            case 500:
                                m_FontWeightInternal = FontWeight.Medium;
                                break;
                            case 600:
                                m_FontWeightInternal = FontWeight.SemiBold;
                                break;
                            case 700:
                                m_FontWeightInternal = FontWeight.Bold;
                                break;
                            case 800:
                                m_FontWeightInternal = FontWeight.Heavy;
                                break;
                            case 900:
                                m_FontWeightInternal = FontWeight.Black;
                                break;
                        }

                        m_FontWeightStack.Add(m_FontWeightInternal);

                        return true;
                    case -1885698441: // </font-weight>
                    case 457225591: // </FONT-WEIGHT>
                        m_FontWeightStack.Remove();

                        if (m_FontStyleInternal == FontStyles.Bold)
                            m_FontWeightInternal = FontWeight.Bold;
                        else
                            m_FontWeightInternal = m_FontWeightStack.Peek();

                        return true;
                    case 6380: // <pos=000.00px> <pos=0em> <pos=50%>
                    case 4556: // <POS>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_xAdvance = value * (m_isOrthographic ? 1.0f : 0.1f);
                                //m_isIgnoringAlignment = true;
                                return true;
                            case TagUnitType.FontUnits:
                                m_xAdvance = value * m_currentFontSize * (m_isOrthographic ? 1.0f : 0.1f);
                                //m_isIgnoringAlignment = true;
                                return true;
                            case TagUnitType.Percentage:
                                m_xAdvance = m_marginWidth * value / 100;
                                //m_isIgnoringAlignment = true;
                                return true;
                        }
                        return false;
                    case 22501: // </pos>
                    case 20677: // </POS>
                        m_isIgnoringAlignment = false;
                        return true;
                    case 16034505: // <voffset>
                    case 11642281: // <VOFFSET>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_baselineOffset = value * (m_isOrthographic ? 1 : 0.1f);
                                return true;
                            case TagUnitType.FontUnits:
                                m_baselineOffset = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                return true;
                            case TagUnitType.Percentage:
                                //m_baselineOffset = m_marginHeight * val / 100;
                                return false;
                        }
                        return false;
                    case 54741026: // </voffset>
                    case 50348802: // </VOFFSET>
                        m_baselineOffset = 0;
                        return true;
                    case 43991: // <page>
                    case 31191: // <PAGE>
                        // This tag only works when Overflow - Page mode is used.
                        if (m_overflowMode == TextOverflowModes.Page)
                        {
                            m_xAdvance = 0 + tag_LineIndent + tag_Indent;
                            m_lineOffset = 0;
                            m_pageNumber += 1;
                            m_isNewPage = true;
                        }
                        return true;
                    // <BR> tag is now handled inline where it is replaced by a linefeed or \n.
                    //case 544: // <BR>
                    //case 800: // <br>
                    //    m_forceLineBreak = true;
                    //    return true;
                    case 43969: // <nobr>
                    case 31169: // <NOBR>
                        m_isNonBreakingSpace = true;
                        return true;
                    case 156816: // </nobr>
                    case 144016: // </NOBR>
                        m_isNonBreakingSpace = false;
                        return true;
                    case 45545: // <size=>
                    case 32745: // <SIZE>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                if (m_htmlTag[5] == 43) // <size=+00>
                                {
                                    m_currentFontSize = m_fontSize + value;
                                    m_sizeStack.Add(m_currentFontSize);
                                    m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                                    return true;
                                }
                                else if (m_htmlTag[5] == 45) // <size=-00>
                                {
                                    m_currentFontSize = m_fontSize + value;
                                    m_sizeStack.Add(m_currentFontSize);
                                    m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                                    return true;
                                }
                                else // <size=00.0>
                                {
                                    m_currentFontSize = value;
                                    m_sizeStack.Add(m_currentFontSize);
                                    m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                                    return true;
                                }
                            case TagUnitType.FontUnits:
                                m_currentFontSize = m_fontSize * value;
                                m_sizeStack.Add(m_currentFontSize);
                                m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                                return true;
                            case TagUnitType.Percentage:
                                m_currentFontSize = m_fontSize * value / 100;
                                m_sizeStack.Add(m_currentFontSize);
                                m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                                return true;
                        }
                        return false;
                    case 158392: // </size>
                    case 145592: // </SIZE>
                        m_currentFontSize = m_sizeStack.Remove();
                        m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));
                        return true;
                    case 41311: // <font=xx>
                    case 28511: // <FONT>
                        int fontHashCode = m_xmlAttribute[0].valueHashCode;
                        int materialAttributeHashCode = m_xmlAttribute[1].nameHashCode;
                        int materialHashCode = m_xmlAttribute[1].valueHashCode;

                        // Special handling for <font=default> or <font=Default>
                        if (fontHashCode == 764638571 || fontHashCode == 523367755)
                        {
                            m_currentFontAsset = m_materialReferences[0].fontAsset;
                            m_currentMaterial = m_materialReferences[0].material;
                            m_currentMaterialIndex = 0;
                            //Debug.Log("<font=Default> assigning Font Asset [" + m_currentFontAsset.name + "] with Material [" + m_currentMaterial.name + "].");

                            m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));

                            m_materialReferenceStack.Add(m_materialReferences[0]);

                            return true;
                        }

                        TMP_FontAsset tempFont;
                        Material tempMaterial;

                        // HANDLE NEW FONT ASSET
                        if (MaterialReferenceManager.TryGetFontAsset(fontHashCode, out tempFont))
                        {
                            //if (tempFont != m_currentFontAsset)
                            //{
                            //    //Debug.Log("Assigning Font Asset: " + tempFont.name);
                            //    m_currentFontAsset = tempFont;
                            //    m_fontScale = (m_currentFontSize / m_currentFontAsset.fontInfo.PointSize * m_currentFontAsset.fontInfo.Scale * (m_isOrthographic ? 1 : 0.1f));
                            //}
                        }
                        else
                        {
                            // Load Font Asset
                            tempFont = Resources.Load<TMP_FontAsset>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));

                            if (tempFont == null)
                                return false;

                            // Add new reference to the font asset as well as default material to the MaterialReferenceManager
                            MaterialReferenceManager.AddFontAsset(tempFont);
                        }


                        // HANDLE NEW MATERIAL
                        if (materialAttributeHashCode == 0 && materialHashCode == 0)
                        {
                            // No material specified then use default font asset material.
                            m_currentMaterial = tempFont.material;

                            m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, tempFont, m_materialReferences, m_materialReferenceIndexLookup);

                            m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
                        }
                        else if (materialAttributeHashCode == 103415287 || materialAttributeHashCode == 72669687) // using material attribute
                        {
                            if (MaterialReferenceManager.TryGetMaterial(materialHashCode, out tempMaterial))
                            {
                                m_currentMaterial = tempMaterial;

                                m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, tempFont, m_materialReferences, m_materialReferenceIndexLookup);

                                m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
                            }
                            else
                            {
                                // Load new material
                                tempMaterial = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength));

                                if (tempMaterial == null)
                                    return false;

                                // Add new reference to this material in the MaterialReferenceManager
                                MaterialReferenceManager.AddFontMaterial(materialHashCode, tempMaterial);

                                m_currentMaterial = tempMaterial;

                                m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, tempFont, m_materialReferences, m_materialReferenceIndexLookup);

                                m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
                            }
                        }
                        else
                            return false;

                        m_currentFontAsset = tempFont;
                        m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));

                        return true;
                    case 154158: // </font>
                    case 141358: // </FONT>
                        {
                            MaterialReference materialReference = m_materialReferenceStack.Remove();

                            m_currentFontAsset = materialReference.fontAsset;
                            m_currentMaterial = materialReference.material;
                            m_currentMaterialIndex = materialReference.index;

                            m_fontScale = (m_currentFontSize / m_currentFontAsset.faceInfo.pointSize * m_currentFontAsset.faceInfo.scale * (m_isOrthographic ? 1 : 0.1f));

                            return true;
                        }
                    case 103415287: // <material="material name">
                    case 72669687: // <MATERIAL>
                        materialHashCode = m_xmlAttribute[0].valueHashCode;

                        // Special handling for <material=default> or <material=Default>
                        if (materialHashCode == 764638571 || materialHashCode == 523367755)
                        {
                            // Check if material font atlas texture matches that of the current font asset.
                            //if (m_currentFontAsset.atlas.GetInstanceID() != m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID()) return false;

                            m_currentMaterial = m_materialReferences[0].material;
                            m_currentMaterialIndex = 0;

                            m_materialReferenceStack.Add(m_materialReferences[0]);

                            return true;
                        }


                        // Check if material 
                        if (MaterialReferenceManager.TryGetMaterial(materialHashCode, out tempMaterial))
                        {
                            // Check if material font atlas texture matches that of the current font asset.
                            //if (m_currentFontAsset.atlas.GetInstanceID() != tempMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID()) return false;

                            m_currentMaterial = tempMaterial;

                            m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset, m_materialReferences, m_materialReferenceIndexLookup);

                            m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
                        }
                        else
                        {
                            // Load new material
                            tempMaterial = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));

                            if (tempMaterial == null)
                                return false;

                            // Check if material font atlas texture matches that of the current font asset.
                            //if (m_currentFontAsset.atlas.GetInstanceID() != tempMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID()) return false;

                            // Add new reference to this material in the MaterialReferenceManager
                            MaterialReferenceManager.AddFontMaterial(materialHashCode, tempMaterial);

                            m_currentMaterial = tempMaterial;

                            m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentMaterial, m_currentFontAsset , m_materialReferences, m_materialReferenceIndexLookup);

                            m_materialReferenceStack.Add(m_materialReferences[m_currentMaterialIndex]);
                        }
                        return true;
                    case 374360934: // </material>
                    case 343615334: // </MATERIAL>
                        {
                            //if (m_currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != m_materialReferenceStack.PreviousItem().material.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
                            //    return false;

                            MaterialReference materialReference = m_materialReferenceStack.Remove();

                            m_currentMaterial = materialReference.material;
                            m_currentMaterialIndex = materialReference.index;

                            return true;
                        }
                    case 320078: // <space=000.00>
                    case 230446: // <SPACE>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_xAdvance += value * (m_isOrthographic ? 1 : 0.1f);
                                return true;
                            case TagUnitType.FontUnits:
                                m_xAdvance += value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                return true;
                            case TagUnitType.Percentage:
                                // Not applicable
                                return false;
                        }
                        return false;
                    case 276254: // <alpha=#FF>
                    case 186622: // <ALPHA>
                        if (m_xmlAttribute[0].valueLength != 3) return false;

                        m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
                        return true;

                    case 1750458: // <a name=" ">
                        return false;
                    case 426: // </a>
                        return true;
                    case 43066: // <link="name">
                    case 30266: // <LINK>
                        if (m_isParsingText && !m_isCalculatingPreferredValues)
                        {
                            int index = m_textInfo.linkCount;

                            if (index + 1 > m_textInfo.linkInfo.Length)
                                TMP_TextInfo.Resize(ref m_textInfo.linkInfo, index + 1);

                            m_textInfo.linkInfo[index].textComponent = this;
                            m_textInfo.linkInfo[index].hashCode = m_xmlAttribute[0].valueHashCode;
                            m_textInfo.linkInfo[index].linkTextfirstCharacterIndex = m_characterCount;

                            m_textInfo.linkInfo[index].linkIdFirstCharacterIndex = startIndex + m_xmlAttribute[0].valueStartIndex;
                            m_textInfo.linkInfo[index].linkIdLength = m_xmlAttribute[0].valueLength;
                            m_textInfo.linkInfo[index].SetLinkID(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        }
                        return true;
                    case 155913: // </link>
                    case 143113: // </LINK>
                        if (m_isParsingText && !m_isCalculatingPreferredValues)
                        {
                            if (m_textInfo.linkCount < m_textInfo.linkInfo.Length)
                            {
                                m_textInfo.linkInfo[m_textInfo.linkCount].linkTextLength = m_characterCount - m_textInfo.linkInfo[m_textInfo.linkCount].linkTextfirstCharacterIndex;

                                m_textInfo.linkCount += 1;
                            }
                        }
                        return true;
                    case 275917: // <align=>
                    case 186285: // <ALIGN>
                        switch (m_xmlAttribute[0].valueHashCode)
                        {
                            case 3774683: // <align=left>
                                m_lineJustification = TextAlignmentOptions.Left;
                                m_lineJustificationStack.Add(m_lineJustification);
                                return true;
                            case 136703040: // <align=right>
                                m_lineJustification = TextAlignmentOptions.Right;
                                m_lineJustificationStack.Add(m_lineJustification);
                                return true;
                            case -458210101: // <align=center>
                                m_lineJustification = TextAlignmentOptions.Center;
                                m_lineJustificationStack.Add(m_lineJustification);
                                return true;
                            case -523808257: // <align=justified>
                                m_lineJustification = TextAlignmentOptions.Justified;
                                m_lineJustificationStack.Add(m_lineJustification);
                                return true;
                            case 122383428: // <align=flush>
                                m_lineJustification = TextAlignmentOptions.Flush;
                                m_lineJustificationStack.Add(m_lineJustification);
                                return true;
                        }
                        return false;
                    case 1065846: // </align>
                    case 976214: // </ALIGN>
                        m_lineJustification = m_lineJustificationStack.Remove();
                        return true;
                    case 327550: // <width=xx>
                    case 237918: // <WIDTH>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_width = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                return false;
                            //break;
                            case TagUnitType.Percentage:
                                m_width = m_marginWidth * value / 100;
                                break;
                        }
                        return true;
                    case 1117479: // </width>
                    case 1027847: // </WIDTH>
                        m_width = -1;
                        return true;
                    // STYLE tag is now handled inline and replaced by its definition.
                    //case 322689: // <style="name">
                    //case 233057: // <STYLE>
                    //    TMP_Style style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);

                    //    if (style == null) return false;

                    //    m_styleStack.Add(style.hashCode);

                    //    // Parse Style Macro
                    //    for (int i = 0; i < style.styleOpeningTagArray.Length; i++)
                    //    {
                    //        if (style.styleOpeningTagArray[i] == 60)
                    //        {
                    //            if (ValidateHtmlTag(style.styleOpeningTagArray, i + 1, out i) == false) return false;
                    //        }
                    //    }
                    //    return true;
                    //case 1112618: // </style>
                    //case 1022986: // </STYLE>
                    //    style = TMP_StyleSheet.GetStyle(m_xmlAttribute[0].valueHashCode);

                    //    if (style == null)
                    //    {
                    //        // Get style from the Style Stack
                    //        int styleHashCode = m_styleStack.CurrentItem();
                    //        style = TMP_StyleSheet.GetStyle(styleHashCode);

                    //        m_styleStack.Remove();
                    //    }

                    //    if (style == null) return false;
                    //    //// Parse Style Macro
                    //    for (int i = 0; i < style.styleClosingTagArray.Length; i++)
                    //    {
                    //        if (style.styleClosingTagArray[i] == 60)
                    //            ValidateHtmlTag(style.styleClosingTagArray, i + 1, out i);
                    //    }
                    //    return true;
                    case 281955: // <color> <color=#FF00FF> or <color=#FF00FF00>
                    case 192323: // <COLOR=#FF00FF>
                        // <color=#FFF> 3 Hex (short hand)
                        if (m_htmlTag[6] == 35 && tagCharCount == 10)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack.Add(m_htmlColor);
                            return true;
                        }
                        // <color=#FFF7> 4 Hex (short hand)
                        else if (m_htmlTag[6] == 35 && tagCharCount == 11)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack.Add(m_htmlColor);
                            return true;
                        }
                        // <color=#FF00FF> 3 Hex pairs
                        if (m_htmlTag[6] == 35 && tagCharCount == 13)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack.Add(m_htmlColor);
                            return true;
                        }
                        // <color=#FF00FF00> 4 Hex pairs
                        else if (m_htmlTag[6] == 35 && tagCharCount == 15)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack.Add(m_htmlColor);
                            return true;
                        }

                        // <color=name>
                        switch (m_xmlAttribute[0].valueHashCode)
                        {
                            case 125395: // <color=red>
                                m_htmlColor = Color.red;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 3573310: // <color=blue>
                                m_htmlColor = Color.blue;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 117905991: // <color=black>
                                m_htmlColor = Color.black;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 121463835: // <color=green>
                                m_htmlColor = Color.green;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 140357351: // <color=white>
                                m_htmlColor = Color.white;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 26556144: // <color=orange>
                                m_htmlColor = new Color32(255, 128, 0, 255);
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case -36881330: // <color=purple>
                                m_htmlColor = new Color32(160, 32, 240, 255);
                                m_colorStack.Add(m_htmlColor);
                                return true;
                            case 554054276: // <color=yellow>
                                m_htmlColor = Color.yellow;
                                m_colorStack.Add(m_htmlColor);
                                return true;
                        }
                        return false;

                    case 100149144: //<gradient>
                    case 69403544:  // <GRADIENT>
                        int gradientPresetHashCode = m_xmlAttribute[0].valueHashCode;
                        TMP_ColorGradient tempColorGradientPreset;

                        // Check if Color Gradient Preset has already been loaded.
                        if (MaterialReferenceManager.TryGetColorGradientPreset(gradientPresetHashCode, out tempColorGradientPreset))
                        {
                            m_colorGradientPreset = tempColorGradientPreset;
                        }
                        else
                        {
                            // Load Color Gradient Preset
                            if (tempColorGradientPreset == null)
                            {
                                tempColorGradientPreset = Resources.Load<TMP_ColorGradient>(TMP_Settings.defaultColorGradientPresetsPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
                            }

                            if (tempColorGradientPreset == null)
                                return false;

                            MaterialReferenceManager.AddColorGradientPreset(gradientPresetHashCode, tempColorGradientPreset);
                            m_colorGradientPreset = tempColorGradientPreset;
                        }

                        m_colorGradientStack.Add(m_colorGradientPreset);

                        // TODO : Add support for defining preset in the tag itself

                        return true;

                    case 371094791: // </gradient>
                    case 340349191: // </GRADIENT>
                        m_colorGradientPreset = m_colorGradientStack.Remove();
                        return true;

                    case 1983971: // <cspace=xx.x>
                    case 1356515: // <CSPACE>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_cSpacing = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                m_cSpacing = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                return false;
                        }
                        return true;
                    case 7513474: // </cspace>
                    case 6886018: // </CSPACE>
                        if (!m_isParsingText) return true;

                        // Adjust xAdvance to remove extra space from last character.
                        if (m_characterCount > 0)
                        {
                            m_xAdvance -= m_cSpacing;
                            m_textInfo.characterInfo[m_characterCount - 1].xAdvance = m_xAdvance;
                        }
                        m_cSpacing = 0;
                        return true;
                    case 2152041: // <mspace=xx.x>
                    case 1524585: // <MSPACE>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_monoSpacing = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                m_monoSpacing = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                return false;
                        }
                        return true;
                    case 7681544: // </mspace>
                    case 7054088: // </MSPACE>
                        m_monoSpacing = 0;
                        return true;
                    case 280416: // <class="name">
                        return false;
                    case 1071884: // </color>
                    case 982252: // </COLOR>
                        m_htmlColor = m_colorStack.Remove();
                        return true;
                    case 2068980: // <indent=10px> <indent=10em> <indent=50%>
                    case 1441524: // <INDENT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                tag_Indent = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                tag_Indent = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                tag_Indent = m_marginWidth * value / 100;
                                break;
                        }
                        m_indentStack.Add(tag_Indent);

                        m_xAdvance = tag_Indent;
                        return true;
                    case 7598483: // </indent>
                    case 6971027: // </INDENT>
                        tag_Indent = m_indentStack.Remove();
                        //m_xAdvance = tag_Indent;
                        return true;
                    case 1109386397: // <line-indent>
                    case -842656867: // <LINE-INDENT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                tag_LineIndent = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                tag_LineIndent = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                tag_LineIndent = m_marginWidth * value / 100;
                                break;
                        }

                        m_xAdvance += tag_LineIndent;
                        return true;
                    case -445537194: // </line-indent>
                    case 1897386838: // </LINE-INDENT>
                        tag_LineIndent = 0;
                        return true;
                    case 2246877: // <sprite=x>
                    case 1619421: // <SPRITE>
                        int spriteAssetHashCode = m_xmlAttribute[0].valueHashCode;
                        TMP_SpriteAsset tempSpriteAsset;
                        m_spriteIndex = -1;

                        // CHECK TAG FORMAT
                        if (m_xmlAttribute[0].valueType == TagValueType.None || m_xmlAttribute[0].valueType == TagValueType.NumericalValue)
                        {
                            // No Sprite Asset is assigned to the text object
                            if (m_spriteAsset != null)
                            {
                                m_currentSpriteAsset = m_spriteAsset;
                            }
                            else if (m_defaultSpriteAsset != null)
                            {
                                m_currentSpriteAsset = m_defaultSpriteAsset;
                            }
                            else if (m_defaultSpriteAsset == null)
                            {
                                if (TMP_Settings.defaultSpriteAsset != null)
                                    m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
                                else
                                    m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");

                                m_currentSpriteAsset = m_defaultSpriteAsset;
                            }

                            // No valid sprite asset available
                            if (m_currentSpriteAsset == null)
                                return false;
                        }
                        else
                        {
                            // A Sprite Asset has been specified
                            if (MaterialReferenceManager.TryGetSpriteAsset(spriteAssetHashCode, out tempSpriteAsset))
                            {
                                m_currentSpriteAsset = tempSpriteAsset;
                            }
                            else
                            {
                                // Load Sprite Asset
                                if (tempSpriteAsset == null)
                                {
                                    tempSpriteAsset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + new string(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength));
                                }

                                if (tempSpriteAsset == null)
                                    return false;

                                //Debug.Log("Loading & assigning new Sprite Asset: " + tempSpriteAsset.name);
                                MaterialReferenceManager.AddSpriteAsset(spriteAssetHashCode, tempSpriteAsset);
                                m_currentSpriteAsset = tempSpriteAsset;
                            }
                        }

                        // Handling of <sprite=index> legacy tag format.
                        if (m_xmlAttribute[0].valueType == TagValueType.NumericalValue) // <sprite=index>
                        {
                            int index = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                            if (index == -9999) return false;

                            // Check to make sure sprite index is valid
                            if (index > m_currentSpriteAsset.spriteCharacterTable.Count - 1) return false;

                            m_spriteIndex = index;
                        }

                        m_spriteColor = s_colorWhite;
                        m_tintSprite = false;

                        // Handle Sprite Tag Attributes
                        for (int i = 0; i < m_xmlAttribute.Length && m_xmlAttribute[i].nameHashCode != 0; i++)
                        {
                            //Debug.Log("Attribute[" + i + "].nameHashCode=" + m_xmlAttribute[i].nameHashCode + "   Value:" + ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength));
                            int nameHashCode = m_xmlAttribute[i].nameHashCode;
                            int index = 0;

                            switch (nameHashCode)
                            {
                                case 43347: // <sprite name="">
                                case 30547: // <SPRITE NAME="">
                                    m_currentSpriteAsset = TMP_SpriteAsset.SearchForSpriteByHashCode(m_currentSpriteAsset, m_xmlAttribute[i].valueHashCode, true, out index);
                                    if (index == -1) return false;

                                    m_spriteIndex = index;
                                    break;
                                case 295562: // <sprite index=>
                                case 205930: // <SPRITE INDEX=>
                                    index = (int)ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);
                                    if (index == -9999) return false;

                                    // Check to make sure sprite index is valid
                                    if (index > m_currentSpriteAsset.spriteCharacterTable.Count - 1) return false;

                                    m_spriteIndex = index;
                                    break;
                                case 45819: // tint
                                case 33019: // TINT
                                    m_tintSprite = ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength) != 0;
                                    break;
                                case 281955: // color=#FF00FF80
                                case 192323: // COLOR
                                    m_spriteColor = HexCharsToColor(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength);
                                    break;
                                case 39505: // anim="0,16,12"  start, end, fps
                                case 26705: // ANIM
                                    //Debug.Log("Start: " + m_xmlAttribute[i].valueStartIndex + "  Length: " + m_xmlAttribute[i].valueLength);
                                    int paramCount = GetAttributeParameters(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength, ref m_attributeParameterValues);
                                    if (paramCount != 3) return false;

                                    m_spriteIndex = (int)m_attributeParameterValues[0];

                                    if (m_isParsingText)
                                    {
                                        // TODO : fix this!
                                        //if (m_attributeParameterValues[0] > m_currentSpriteAsset.spriteInfoList.Count - 1 || m_attributeParameterValues[1] > m_currentSpriteAsset.spriteInfoList.Count - 1)
                                        //    return false;

                                        spriteAnimator.DoSpriteAnimation(m_characterCount, m_currentSpriteAsset, m_spriteIndex, (int)m_attributeParameterValues[1], (int)m_attributeParameterValues[2]);
                                    }

                                    break;
                                //case 45545: // size
                                //case 32745: // SIZE

                                //    break;
                                default:
                                    if (nameHashCode != 2246877 && nameHashCode != 1619421)
                                        return false;
                                    break;
                            }
                        }

                        if (m_spriteIndex == -1) return false;

                        // Material HashCode for the Sprite Asset is the Sprite Asset Hash Code
                        m_currentMaterialIndex = MaterialReference.AddMaterialReference(m_currentSpriteAsset.material, m_currentSpriteAsset, m_materialReferences, m_materialReferenceIndexLookup);

                        m_textElementType = TMP_TextElementType.Sprite;
                        return true;
                    case 730022849: // <lowercase>
                    case 514803617: // <LOWERCASE>
                        m_FontStyleInternal |= FontStyles.LowerCase;
                        m_fontStyleStack.Add(FontStyles.LowerCase);
                        return true;
                    case -1668324918: // </lowercase>
                    case -1883544150: // </LOWERCASE>
                        if ((m_fontStyle & FontStyles.LowerCase) != FontStyles.LowerCase)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.LowerCase) == 0)
                                m_FontStyleInternal &= ~FontStyles.LowerCase;
                        }
                        return true;
                    case 13526026: // <allcaps>
                    case 9133802: // <ALLCAPS>
                    case 781906058: // <uppercase>
                    case 566686826: // <UPPERCASE>
                        m_FontStyleInternal |= FontStyles.UpperCase;
                        m_fontStyleStack.Add(FontStyles.UpperCase);
                        return true;
                    case 52232547: // </allcaps>
                    case 47840323: // </ALLCAPS>
                    case -1616441709: // </uppercase>
                    case -1831660941: // </UPPERCASE>
                        if ((m_fontStyle & FontStyles.UpperCase) != FontStyles.UpperCase)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.UpperCase) == 0)
                                m_FontStyleInternal &= ~FontStyles.UpperCase;
                        }
                        return true;
                    case 766244328: // <smallcaps>
                    case 551025096: // <SMALLCAPS>
                        m_FontStyleInternal |= FontStyles.SmallCaps;
                        m_fontStyleStack.Add(FontStyles.SmallCaps);
                        return true;
                    case -1632103439: // </smallcaps>
                    case -1847322671: // </SMALLCAPS>
                        if ((m_fontStyle & FontStyles.SmallCaps) != FontStyles.SmallCaps)
                        {
                            if (m_fontStyleStack.Remove(FontStyles.SmallCaps) == 0)
                                m_FontStyleInternal &= ~FontStyles.SmallCaps;
                        }
                        return true;
                    case 2109854: // <margin=00.0> <margin=00em> <margin=50%>
                    case 1482398: // <MARGIN>
                        // Check value type
                        switch (m_xmlAttribute[0].valueType)
                        {
                            case TagValueType.NumericalValue:
                                value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength); // px
                                if (value == -9999) return false;

                                // Determine tag unit type
                                switch (tagUnitType)
                                {
                                    case TagUnitType.Pixels:
                                        m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f);
                                        break;
                                    case TagUnitType.FontUnits:
                                        m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                        break;
                                    case TagUnitType.Percentage:
                                        m_marginLeft = (m_marginWidth - (m_width != -1 ? m_width : 0)) * value / 100;
                                        break;
                                }
                                m_marginLeft = m_marginLeft >= 0 ? m_marginLeft : 0;
                                m_marginRight = m_marginLeft;
                                return true;

                            case TagValueType.None:
                                for (int i = 1; i < m_xmlAttribute.Length && m_xmlAttribute[i].nameHashCode != 0; i++)
                                {
                                    // Get attribute name
                                    int nameHashCode = m_xmlAttribute[i].nameHashCode;

                                    switch (nameHashCode)
                                    {
                                        case 42823:  // <margin left=value>
                                            value = ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength); // px
                                            if (value == -9999) return false;

                                            switch (m_xmlAttribute[i].unitType)
                                            {
                                                case TagUnitType.Pixels:
                                                    m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f);
                                                    break;
                                                case TagUnitType.FontUnits:
                                                    m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                                    break;
                                                case TagUnitType.Percentage:
                                                    m_marginLeft = (m_marginWidth - (m_width != -1 ? m_width : 0)) * value / 100;
                                                    break;
                                            }
                                            m_marginLeft = m_marginLeft >= 0 ? m_marginLeft : 0;
                                            break;

                                        case 315620: // <margin right=value>
                                            value = ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength); // px
                                            if (value == -9999) return false;

                                            switch (m_xmlAttribute[i].unitType)
                                            {
                                                case TagUnitType.Pixels:
                                                    m_marginRight = value * (m_isOrthographic ? 1 : 0.1f);
                                                    break;
                                                case TagUnitType.FontUnits:
                                                    m_marginRight = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                                    break;
                                                case TagUnitType.Percentage:
                                                    m_marginRight = (m_marginWidth - (m_width != -1 ? m_width : 0)) * value / 100;
                                                    break;
                                            }
                                            m_marginRight = m_marginRight >= 0 ? m_marginRight : 0;
                                            break;
                                    }
                                }
                                return true;
                        }

                        return false;
                    case 7639357: // </margin>
                    case 7011901: // </MARGIN>
                        m_marginLeft = 0;
                        m_marginRight = 0;
                        return true;
                    case 1100728678: // <margin-left=xx.x>
                    case -855002522: // <MARGIN-LEFT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength); // px
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                m_marginLeft = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                m_marginLeft = (m_marginWidth - (m_width != -1 ? m_width : 0)) * value / 100;
                                break;
                        }
                        m_marginLeft = m_marginLeft >= 0 ? m_marginLeft : 0;
                        return true;
                    case -884817987: // <margin-right=xx.x>
                    case -1690034531: // <MARGIN-RIGHT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength); // px
                        if (value == -9999) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_marginRight = value * (m_isOrthographic ? 1 : 0.1f);
                                break;
                            case TagUnitType.FontUnits:
                                m_marginRight = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                m_marginRight = (m_marginWidth - (m_width != -1 ? m_width : 0)) * value / 100;
                                break;
                        }
                        m_marginRight = m_marginRight >= 0 ? m_marginRight : 0;
                        return true;
                    case 1109349752: // <line-height=xx.x>
                    case -842693512: // <LINE-HEIGHT>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnitType)
                        {
                            case TagUnitType.Pixels:
                                m_lineHeight = value * (m_isOrthographic ? 1 : 0.1f); 
                                break;
                            case TagUnitType.FontUnits:
                                m_lineHeight = value * (m_isOrthographic ? 1 : 0.1f) * m_currentFontSize;
                                break;
                            case TagUnitType.Percentage:
                                m_lineHeight = m_fontAsset.faceInfo.lineHeight * value / 100 * m_fontScale;
                                break;
                        }
                        return true;
                    case -445573839: // </line-height>
                    case 1897350193: // </LINE-HEIGHT>
                        m_lineHeight = TMP_Math.FLOAT_UNSET;
                        return true;
                    case 15115642: // <noparse>
                    case 10723418: // <NOPARSE>
                        tag_NoParsing = true;
                        return true;
                    case 1913798: // <action>
                    case 1286342: // <ACTION>
                        int actionID = m_xmlAttribute[0].valueHashCode;

                        if (m_isParsingText)
                        {
                            m_actionStack.Add(actionID);

                            Debug.Log("Action ID: [" + actionID + "] First character index: " + m_characterCount);


                        }
                        //if (m_isParsingText)
                        //{
                        // TMP_Action action = TMP_Action.GetAction(m_xmlAttribute[0].valueHashCode);
                        //}
                        return true;
                    case 7443301: // </action>
                    case 6815845: // </ACTION>
                        if (m_isParsingText)
                        {
                            Debug.Log("Action ID: [" + m_actionStack.CurrentItem() + "] Last character index: " + (m_characterCount - 1));
                        }

                        m_actionStack.Remove();
                        return true;
                    case 315682: // <scale=xx.x>
                    case 226050: // <SCALE=xx.x>
                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        m_FXMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(value, 1, 1));
                        m_isFXMatrixSet = true;

                        return true;
                    case 1105611: // </scale>
                    case 1015979: // </SCALE>
                        m_isFXMatrixSet = false;
                        return true;
                    case 2227963: // <rotate=xx.x>
                    case 1600507: // <ROTATE=xx.x>
                        // TODO: Add ability to use Random Rotation

                        value = ConvertToFloat(m_htmlTag, m_xmlAttribute[0].valueStartIndex, m_xmlAttribute[0].valueLength);
                        if (value == -9999) return false;

                        m_FXMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, value), Vector3.one);
                        m_isFXMatrixSet = true;

                        return true;
                    case 7757466: // </rotate>
                    case 7130010: // </ROTATE>
                        m_isFXMatrixSet = false;
                        return true;
                    case 317446: // <table>
                    case 227814: // <TABLE>
                        switch (m_xmlAttribute[1].nameHashCode)
                        {
                            case 327550: // width
                                float tableWidth = ConvertToFloat(m_htmlTag, m_xmlAttribute[1].valueStartIndex, m_xmlAttribute[1].valueLength);

                                switch (tagUnitType)
                                {
                                    case TagUnitType.Pixels:
                                        Debug.Log("Table width = " + tableWidth + "px.");
                                        break;
                                    case TagUnitType.FontUnits:
                                        Debug.Log("Table width = " + tableWidth + "em.");
                                        break;
                                    case TagUnitType.Percentage:
                                        Debug.Log("Table width = " + tableWidth + "%.");
                                        break;
                                }
                                break;
                        }
                        return true;
                    case 1107375: // </table>
                    case 1017743: // </TABLE>
                        return true;
                    case 926: // <tr>
                    case 670: // <TR>
                        return true;
                    case 3229: // </tr>
                    case 2973: // </TR>
                        return true;
                    case 916: // <th>
                    case 660: // <TH>
                        // Set style to bold and center alignment
                        return true;
                    case 3219: // </th>
                    case 2963: // </TH>
                        return true;
                    case 912: // <td>
                    case 656: // <TD>
                              // Style options
                        for (int i = 1; i < m_xmlAttribute.Length && m_xmlAttribute[i].nameHashCode != 0; i++)
                        {
                            switch (m_xmlAttribute[i].nameHashCode)
                            {
                                case 327550: // width
                                    float tableWidth = ConvertToFloat(m_htmlTag, m_xmlAttribute[i].valueStartIndex, m_xmlAttribute[i].valueLength);

                                    switch (tagUnitType)
                                    {
                                        case TagUnitType.Pixels:
                                            Debug.Log("Table width = " + tableWidth + "px.");
                                            break;
                                        case TagUnitType.FontUnits:
                                            Debug.Log("Table width = " + tableWidth + "em.");
                                            break;
                                        case TagUnitType.Percentage:
                                            Debug.Log("Table width = " + tableWidth + "%.");
                                            break;
                                    }
                                    break;
                                case 275917: // align
                                    switch (m_xmlAttribute[i].valueHashCode)
                                    {
                                        case 3774683: // left
                                            Debug.Log("TD align=\"left\".");
                                            break;
                                        case 136703040: // right
                                            Debug.Log("TD align=\"right\".");
                                            break;
                                        case -458210101: // center
                                            Debug.Log("TD align=\"center\".");
                                            break;
                                        case -523808257: // justified
                                            Debug.Log("TD align=\"justified\".");
                                            break;
                                    }
                                    break;
                            }
                        }

                        return true;
                    case 3215: // </td>
                    case 2959: // </TD>
                        return true;
                }
            }
            #endif
            #endregion

            return false;
        }
    }
}
