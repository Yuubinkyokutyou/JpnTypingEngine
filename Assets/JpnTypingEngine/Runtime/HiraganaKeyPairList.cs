using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JpnTypingEngine
{
    [Serializable]
    public class HiraganaKeyPairList : ScriptableObject
    {
        public int MaxHiraganaLength;
        public HiraganaKeyPair[] HiraganaKeyPairs;
        
        
        public List<string> GetInputPair(string hiragana)
        {
            foreach (var hiraganaKeyPair in HiraganaKeyPairs)
            {
                if (hiraganaKeyPair.hiragana == hiragana)
                {
                    return hiraganaKeyPair.inputPairs;
                }
            }
            return null;
        }
        
        
        
        
        # if UNITY_EDITOR
        public void OnValidate()
        {
            int maxHiraganaLength = HiraganaKeyPairs
                .SelectMany(hiraganaKeyPair => hiraganaKeyPair.inputPairs)
                .Max(inputPair => inputPair.Length);

            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}