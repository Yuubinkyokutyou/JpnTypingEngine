using System;
using UnityEngine;

namespace JpnTypingEngine
{
    public class InputKeyProvider :Singleton<InputKeyProvider>
    {
        public event Action<char> OnInputKey;
        
        private void OnGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                var c = Event.current.keyCode.ToChar();
                if (c != '\0')
                {
                    OnInputKey?.Invoke(c);
                }
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();
            OnInputKey = null;
        }
    }
}