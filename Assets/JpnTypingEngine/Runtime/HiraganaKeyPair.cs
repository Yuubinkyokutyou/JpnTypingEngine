using System.Collections.Generic;
using UnityEngine;

namespace JpnTypingEngine
{
    /// <summary>
    ///
    /// </summary>
    [System.Serializable]
    public struct HiraganaKeyPair
    {
        [SerializeField] string Hiragana;
        [SerializeField]  List<string> InputPairs;
        
        public string hiragana => Hiragana;
        public List<string> inputPairs => InputPairs;
        
        public HiraganaKeyPair(string hiragana, List<string> inputPairs)
        {
            Hiragana = hiragana;
            InputPairs = inputPairs;
        }
    }
}