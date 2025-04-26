using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JpnTypingEngine
{
    [Serializable]
    public class HiraganaInputMapping : ScriptableObject
    {
        [SerializeField] private int maxHiraganaLength;
        [SerializeField] private HiraganaKeyPair[] hiraganaKeyPairs;
        
        public int MaxHiraganaLength => maxHiraganaLength;
        public HiraganaKeyPair[] HiraganaKeyPairs => hiraganaKeyPairs;
        
        public List<string> GetInputPair(string hiragana)
        {
            foreach (var hiraganaKeyPair in hiraganaKeyPairs)
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
            maxHiraganaLength = hiraganaKeyPairs
                .SelectMany(hiraganaKeyPair => hiraganaKeyPair.inputPairs)
                .Max(inputPair => inputPair.Length);
            
            UnityEditor.EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// ひらがなと入力組み合わせのリストを設定する(エディタ拡張用)
        /// </summary>
        /// <param name="hiraganaKeyPairs"></param>
        public void SetHiraganaKeyPairs(HiraganaKeyPair[] hiraganaKeyPairs)
        {
            this.hiraganaKeyPairs = hiraganaKeyPairs;
            maxHiraganaLength = hiraganaKeyPairs
                .SelectMany(hiraganaKeyPair => hiraganaKeyPair.inputPairs)
                .Max(inputPair => inputPair.Length);

            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}