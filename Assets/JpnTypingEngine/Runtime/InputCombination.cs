using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace JpnTypingEngine
{
    /// <summary>
    /// ひらがなの文章の入力組み合わせ
    /// </summary>
    public class InputCombination　: IDisposable
    {
        public string Hiragana { get; private set; }
        public List<HiraganaSection> HiraganaSections { get; private set; }
        private readonly Dictionary<int, List<HiraganaSection>> StartIndexToHiraganaSections = new();
        
        public InputCombination SetValue(string hiragana, List<HiraganaSection> hiraganaSections)
        {
            Hiragana = hiragana;
            HiraganaSections = hiraganaSections;
            
            //ListPoolでのリリース
            foreach (var variable in StartIndexToHiraganaSections.Values)
            {
                //release
                ListPool<HiraganaSection>.Release(variable);
            }
            
            //StartIndexToHiraganaSectionsを作り直す
            StartIndexToHiraganaSections.Clear();
            foreach (var hiraganaSection in hiraganaSections)
            {
                if (StartIndexToHiraganaSections.TryGetValue(hiraganaSection.StartIndex, value: out var section))
                {
                    section.Add(hiraganaSection);
                }
                else
                {
                    var sectionList = ListPool<HiraganaSection>.Get();
                    sectionList.Add(hiraganaSection);
                    StartIndexToHiraganaSections.Add(hiraganaSection.StartIndex,sectionList);
                }
            }
            //StartIndexToHiraganaSectionsをsectionが大きい順に
            foreach (var variable in StartIndexToHiraganaSections.Values)
            {
                variable.Sort((a, b) => b.Hiragana.Length - a.Hiragana.Length);
            }
            
            return this;
        }
        
        
        public List<HiraganaSection> GetHiraganaSections(int startIndex)
        {
            if (StartIndexToHiraganaSections.TryGetValue(startIndex, out var section))
            {
                return section;
            }
            return null;
        }

        public void Dispose()
        {
            foreach (var variable in StartIndexToHiraganaSections.Values)
            {
                //release
                ListPool<HiraganaSection>.Release(variable);
            }
        }
    }
}