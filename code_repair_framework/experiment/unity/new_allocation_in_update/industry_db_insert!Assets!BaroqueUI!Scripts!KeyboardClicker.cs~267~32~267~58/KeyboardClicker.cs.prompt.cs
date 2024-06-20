using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;


namespace BaroqueUI
{
    [Serializable]
    public class KeyboardTypingEvent : UnityEvent<KeyboardClicker.EKeyState, string>
    {
        /* KeyboardClicker sends a KeyboardTypingEvent to report actions about the VR keyboard keys.

           Apart from the special keys (enter, esc, tab, backspace), all regular keys are sent in the
           following sequence:
           - Event(Preview);
           - optionally more Event(Preview), each replacing the current key;
           - one final Event(Confirm), always with the same key as the most recent Event(Preview).

           The special keys cannot occur between a Event(Preview) and a Event(Confirm).
           This script will always confirm a previewed key before that.  In one case this script can
           cancel a previewed key by emitting Event(Preview, "") and Event(Confirm, "").

           Dead keys are sent as, say, Event(Preview, "^").  Then it might be replaced with a
           Event(Preview, "â") followed by Event(Confirm, "â").  Or the dead key alone can be
           confirmed, e.g. if we press the space bar or a letter that can't combine with the dead key.

           There are two different ways to use this:

           * If you can easily accept corrections in output, e.g. if it is sent to an inputField,
             then write the Previewed key immediately but replace them with further inputs until
             you get the Confirm.  The Previewed key can be selected; then it is natural that they
             are replaced when different keys are Previewed, and gives some color feedback.

           * If you can't correct the output, then ignore Preview and only emit keys when you get
             the Confirm.
         */
    }


    public class KeyboardClicker : MonoBehaviour
    {
        public enum EKeyState { Preview, Confirm, Special_Backspace, Special_Tab, Special_Enter, Special_Esc };

        public KeyboardTypingEvent onKeyboardTyping;
        public bool enableTabKey = true;
        public bool enableEnterKey = true;
        public bool enableEscKey = false;
        public bool reactToTriggerToo = false;
        public float controllerPriority = 200;


#if !UNITY_ANDROID
        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode, byte[] keyboardState,
                [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
                System.Text.StringBuilder receivingBuffer, int bufferSize, uint flags);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        const uint MAPVK_VSC_TO_VK = 1;
#endif

        const int VK_SHIFT = 0x10;
        const int VK_CONTROL = 0x11;
        const int VK_MENU = 0x12;

        const int SCAN_UNKNOWN = -1;
        const int SCAN_ESC = 1;
        const int SCAN_BACKSPACE = 14;
        const int SCAN_TAB = 15;
        const int SCAN_ENTER = 28;
        const int SCAN_BACKSLASH = 43;
        const int SCAN_ALTGR = 56;
        const int SCAN_SPACE = 57;
        const int SCAN_EXTRA = 86;   /* many European keyboards have an extra key here; US keyboards report the same as SCAN_BACKSLASH */

        const string DEAD_KEY = "[DEAD_KEY]";


        internal static string GetCharsFromKeys(int scancode, bool shift, bool altGr, int next_scancode = 0, bool next_shift = false)
        {
#if !UNITY_ANDROID
            uint key = MapVirtualKey((uint)scancode, MAPVK_VSC_TO_VK);
            uint next_key = next_scancode > 0 ? MapVirtualKey((uint)next_scancode, MAPVK_VSC_TO_VK) : key;

            var buf = new System.Text.StringBuilder(128);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[VK_SHIFT] = 0xff;
            if (altGr)
            {
                keyboardState[VK_CONTROL] = 0xff;
                keyboardState[VK_MENU] = 0xff;
            }
            int result = ToUnicode(key, (uint)scancode, keyboardState, buf, 128, 0);
            if (result == -1)
            {
                /* dead keys seem to be stored inside Windows somewhere, so we need to clear
                 * it out in all cases.  That's why we send by default the dead key twice. */
                if (next_scancode == 0)
                {
                    ToUnicode(key, (uint)scancode, keyboardState, buf, 128, 0);
                    return DEAD_KEY + buf.ToString(0, 1);
                }
                else
                {
                    keyboardState[VK_SHIFT] = (byte)(next_shift ? 0xff : 0);
                    keyboardState[VK_CONTROL] = 0;
                    keyboardState[VK_MENU] = 0;
                    result = ToUnicode(next_key, (uint)next_scancode, keyboardState, buf, 128, 0);
                }
            }
            return buf.ToString(0, result);
#else
            /* Emulate the U.S. Keyboard layout */
            if (altGr)
                return "";
            if (!shift)
                switch (scancode)
                {
                    case 41: return "`";
                    case 2: return "1";
                    case 3: return "2";
                    case 4: return "3";
                    case 5: return "4";
                    case 6: return "5";
                    case 7: return "6";
                    case 8: return "7";
                    case 9: return "8";
                    case 10: return "9";
                    case 11: return "0";
                    case 12: return "-";
                    case 13: return "=";

                    case 16: return "q";
                    case 17: return "w";
                    case 18: return "e";
                    case 19: return "r";
                    case 20: return "t";
                    case 21: return "y";
                    case 22: return "u";
                    case 23: return "i";
                    case 24: return "o";
                    case 25: return "p";
                    case 26: return "[";
                    case 27: return "]";
                    case 43: return "\\";

                    case 30: return "a";
                    case 31: return "s";
                    case 32: return "d";
                    case 33: return "f";
                    case 34: return "g";
                    case 35: return "h";
                    case 36: return "j";
                    case 37: return "k";
                    case 38: return "l";
                    case 39: return ";";
                    case 40: return "'";

                    case 86: return "\\";
                    case 44: return "z";
                    case 45: return "x";
                    case 46: return "c";
                    case 47: return "v";
                    case 48: return "b";
                    case 49: return "n";
                    case 50: return "m";
                    case 51: return ",";
                    case 52: return ".";
                    case 53: return "/";

                    case 57: return " ";
                }
            else
                switch (scancode)
                {
                    case 41: return "~";
                    case 2: return "!";
                    case 3: return "@";
                    case 4: return "#";
                    case 5: return "$";
                    case 6: return "%";
                    case 7: return "^";
                    case 8: return "&";
                    case 9: return "*";
                    case 10: return "(";
                    case 11: return ")";
                    case 12: return "_";
                    case 13: return "+";

                    case 16: return "Q";
                    case 17: return "W";
                    case 18: return "E";
                    case 19: return "R";
                    case 20: return "T";
                    case 21: return "Y";
                    case 22: return "U";
                    case 23: return "I";
                    case 24: return "O";
                    case 25: return "P";
                    case 26: return "{";
                    case 27: return "}";
                    case 43: return "|";

                    case 30: return "A";
                    case 31: return "S";
                    case 32: return "D";
                    case 33: return "F";
                    case 34: return "G";
                    case 35: return "H";
                    case 36: return "J";
                    case 37: return "K";
                    case 38: return "L";
                    case 39: return ":";
                    case 40: return "\"";

                    case 86: return "|";
                    case 44: return "Z";
                    case 45: return "X";
                    case 46: return "C";
                    case 47: return "V";
                    case 48: return "B";
                    case 49: return "N";
                    case 50: return "M";
                    case 51: return "<";
                    case 52: return ">";
                    case 53: return "?";
                }
            return "";
#endif
        }


        class KeyInfo
        {
            /* these fields control the appearence of the key, not its behaviour */
            internal int scan_code;
            internal char scan_code_extra;
            internal string current_text;      /* what to display on the key */
            internal string[] texts;           /* array of 3 strings [normal, shift, altgr] */
            internal Image image;
            internal Text text;
            internal Timed.Time blink_end;

            internal const float TOTAL_KEY_TIME = 0.3f;

            internal bool Update(bool fallback = false)
            {
                        // BUG: Using New() allocation in Update() method.
                        // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
                        //                 string display_text = current_text.StartsWith(DEAD_KEY) ? current_text.Substring(DEAD_KEY.Length) : current_text;
                        //                 bool update = text.text != display_text;
                        //                 if (update)
                        //                     text.text = display_text;
                        // 
                        //                 update |= blink_end.seconds > 0;
                        //                 if (update)
                        //                 {
                        //                     float done_fraction = 1 - (blink_end - Timed.time) / TOTAL_KEY_TIME;
                        //                     Color col1 = Color.red, col2 = Color.white;
                        //                     if (current_text == "")
                        //                         col2 = new Color(0.9f, 0.9f, 0.9f);
                        //                     image.color = Color.Lerp(col1, col2, done_fraction);
                        //                     if (done_fraction >= 1)
                        //                     {
                        //                         blink_end.seconds = 0;
                        //                         if (fallback && current_text == texts[1])
                        //                             current_text = texts[0];    /* automatic fall back */
                        //                     }
                        //                 }
                        //                 return update;
                        //             }

                        // FIXED VERSION:
