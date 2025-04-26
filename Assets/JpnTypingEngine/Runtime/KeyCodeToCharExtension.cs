using UnityEngine;
using System.Collections.Generic; // Add this line

namespace JpnTypingEngine
{
    public static class KeyCodeToCharExtension
    {
        private static readonly Dictionary<KeyCode, char> KeyCodeCharMap = new Dictionary<KeyCode, char>();

        static KeyCodeToCharExtension()
        {
            // Initialize the map in the static constructor
            KeyCodeCharMap.Add(KeyCode.A, 'a');
            KeyCodeCharMap.Add(KeyCode.B, 'b');
            KeyCodeCharMap.Add(KeyCode.C, 'c');
            KeyCodeCharMap.Add(KeyCode.D, 'd');
            KeyCodeCharMap.Add(KeyCode.E, 'e');
            KeyCodeCharMap.Add(KeyCode.F, 'f');
            KeyCodeCharMap.Add(KeyCode.G, 'g');
            KeyCodeCharMap.Add(KeyCode.H, 'h');
            KeyCodeCharMap.Add(KeyCode.I, 'i');
            KeyCodeCharMap.Add(KeyCode.J, 'j');
            KeyCodeCharMap.Add(KeyCode.K, 'k');
            KeyCodeCharMap.Add(KeyCode.L, 'l');
            KeyCodeCharMap.Add(KeyCode.M, 'm');
            KeyCodeCharMap.Add(KeyCode.N, 'n');
            KeyCodeCharMap.Add(KeyCode.O, 'o');
            KeyCodeCharMap.Add(KeyCode.P, 'p');
            KeyCodeCharMap.Add(KeyCode.Q, 'q');
            KeyCodeCharMap.Add(KeyCode.R, 'r');
            KeyCodeCharMap.Add(KeyCode.S, 's');
            KeyCodeCharMap.Add(KeyCode.T, 't');
            KeyCodeCharMap.Add(KeyCode.U, 'u');
            KeyCodeCharMap.Add(KeyCode.V, 'v');
            KeyCodeCharMap.Add(KeyCode.W, 'w');
            KeyCodeCharMap.Add(KeyCode.X, 'x');
            KeyCodeCharMap.Add(KeyCode.Y, 'y');
            KeyCodeCharMap.Add(KeyCode.Z, 'z');
            KeyCodeCharMap.Add(KeyCode.Alpha0, '0');
            KeyCodeCharMap.Add(KeyCode.Alpha1, '1');
            KeyCodeCharMap.Add(KeyCode.Alpha2, '2');
            KeyCodeCharMap.Add(KeyCode.Alpha3, '3');
            KeyCodeCharMap.Add(KeyCode.Alpha4, '4');
            KeyCodeCharMap.Add(KeyCode.Alpha5, '5');
            KeyCodeCharMap.Add(KeyCode.Alpha6, '6');
            KeyCodeCharMap.Add(KeyCode.Alpha7, '7');
            KeyCodeCharMap.Add(KeyCode.Alpha8, '8');
            KeyCodeCharMap.Add(KeyCode.Alpha9, '9');
            KeyCodeCharMap.Add(KeyCode.Minus, '-');
            KeyCodeCharMap.Add(KeyCode.Caret, '^');
            KeyCodeCharMap.Add(KeyCode.Backslash, '\\');
            KeyCodeCharMap.Add(KeyCode.At, '@');
            KeyCodeCharMap.Add(KeyCode.LeftBracket, '[');
            KeyCodeCharMap.Add(KeyCode.Semicolon, ';');
            KeyCodeCharMap.Add(KeyCode.Colon, ':');
            KeyCodeCharMap.Add(KeyCode.RightBracket, ']');
            KeyCodeCharMap.Add(KeyCode.Comma, ',');
            KeyCodeCharMap.Add(KeyCode.Period, '.');
            KeyCodeCharMap.Add(KeyCode.Slash, '/');
            KeyCodeCharMap.Add(KeyCode.Underscore, '_');
            KeyCodeCharMap.Add(KeyCode.Backspace, '\b');
            KeyCodeCharMap.Add(KeyCode.Return, '\r');
            KeyCodeCharMap.Add(KeyCode.Space, ' ');
            // Add other mappings as needed
        }

        public static char ToChar(this KeyCode keyCode)
        {
             if (KeyCodeCharMap.TryGetValue(keyCode, out char result))
             {
                 return result;
             }
             return '\0'; // Return default char if not found
        }

    }
}