using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Pool;

namespace JpnTypingEngine
{
    /// <summary>
    /// ひらがなから入力組み合わせに変換するクラス
    /// </summary>
    public class HiraganaToInputConverter : IDisposable
    {
        readonly HiraganaKeyPairList _hiraganaKeyPairList;
        private InputCombination _inputCombination = new InputCombination();
        private readonly List<HiraganaSection> _hiraganaSections = new List<HiraganaSection>();
        
        StringBuilder _sectionHiragana = new StringBuilder();
        
        public HiraganaToInputConverter(HiraganaKeyPairList hiraganaKeyPairList)
        {
            this._hiraganaKeyPairList = hiraganaKeyPairList;
        }
        
        
        public InputCombination Convert(string inputHiragana)
        {
            //ListPoolでのリリース
            foreach (var variable in _hiraganaSections)
            {
                //release
                ListPool<string>.Release(variable.InputPairs);
            }
            
            _hiraganaSections.Clear();
            
            
            //ひらがなの文章を、区切りと入力組み合わせに変換
            
            //_hiraganaKeyPairList
            for (var i = 0; i < inputHiragana.Length; i++)
            {
                for (var j = 0; j < _hiraganaKeyPairList.MaxHiraganaLength; j++)
                {
                    if (i + j + 1 > inputHiragana.Length) break;
                    _sectionHiragana.Clear();
                    _sectionHiragana.Append(inputHiragana, i, j + 1);
                    
                    var inputPairList = _hiraganaKeyPairList.GetInputPair(_sectionHiragana.ToString());
                    
                    if(inputPairList == null) continue;
                    
                    //ListPool使用
                    var inputPairs = ListPool<string>.Get();
                    foreach (var inputPair in inputPairList)
                    {
                        inputPairs.Add(inputPair);
                    }
                    
                    _hiraganaSections.Add(new HiraganaSection(_sectionHiragana.ToString(), i, inputPairs));
                }
            }
            

            return _inputCombination.SetValue(inputHiragana, _hiraganaSections);
        }

        public void Dispose()
        {
            _inputCombination?.Dispose();
        }
    }

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
    
    
    /// <summary>
    /// ひらがなの区切りとその入力組み合わせ
    /// </summary>
    public class HiraganaSection
    {
        public string Hiragana { get; private set; }

        /// <summary>
        /// 文章のひらがなの開始位置。「あいうえお」の「あ」だったら０
        /// </summary>
        public int StartIndex { get; private set; }
        
        /// <summary>
        ///　Hiraganaが「い」の場合、入力組み合わせは「i」「yi」
        /// </summary>
        public List<string> InputPairs { get; private set; }
        
        public HiraganaSection(string hiragana, int startIndex, List<string> inputPairs)
        {
            Hiragana = hiragana;
            StartIndex = startIndex;
            InputPairs = inputPairs;
        }
    }
}
    

