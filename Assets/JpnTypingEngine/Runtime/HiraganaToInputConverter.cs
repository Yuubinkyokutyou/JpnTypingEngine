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
        
        //Convertメソッドで使用するStringBuilder
        StringBuilder _sectionHiragana = new StringBuilder();
        
        //「ん」の後の文字がこれでない場合「n」の入力組み合わせを追加
        //母音、ナ行、ヤ行、ニャ行
        private string[] NBeforeHiraganas = new string[]
        {
            "あ", "い", "う", "え", "お",
            "な", "に", "ぬ", "ね", "の",
            "や", "ゆ", "よ",
            "にゃ", "にゅ", "にょ",
        };
        
        public HiraganaToInputConverter(HiraganaKeyPairList hiraganaKeyPairList)
        {
            this._hiraganaKeyPairList = hiraganaKeyPairList;
        }
        
        
        public InputCombination Convert(string inputHiragana)
        {
            ReleaseListPool();
            
            _hiraganaSections.Clear();
            
            
            //ひらがなの文章を、区切りと入力組み合わせに変換
            
            for (var i = 0; i < inputHiragana.Length; i++)
            {
                //_hiraganaKeyPairListから変換するため、セクションの最大文字数分繰り返す
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
                    
                    
                    //「ん」の文字の例外処理
                    //最後の文字の場合は追加しない
                    if (j == 0 &&
                        inputHiragana[i] == 'ん' && 
                        i != inputHiragana.Length - 1)
                    {
                        for (var n = 0; n < NBeforeHiraganas.Length; n++)
                        {
                            var nBeforeHiragana = NBeforeHiraganas[n];
                            //inputHiraganaのnBeforeHiragana.length文字後が存在するか
                            //inputHiraganaの後の文字がnBeforeHiraganaでないか
                            //TODO:Substringをキャッシュ出来そう
                            if (i + nBeforeHiragana.Length < inputHiragana.Length &&
                                inputHiragana.Substring(i + 1, nBeforeHiragana.Length) == nBeforeHiragana)
                            {
                                break;
                            }
                            
                            //すべて不一致だった場合、nを追加
                            if(n==NBeforeHiraganas.Length-1)
                            {
                                inputPairs.Insert(0, "n");
                            }
                        }
                    }
                    
                    _hiraganaSections.Add(new HiraganaSection(_sectionHiragana.ToString(), i, inputPairs));
                }
            }
            
            
            
            
            return _inputCombination.SetValue(inputHiragana, _hiraganaSections);
        }

        public void Dispose()
        {
            _inputCombination?.Dispose();
            
            ReleaseListPool();
        }
        
        
        //ListPoolでのリリース
        void ReleaseListPool()
        {
            foreach (var variable in _hiraganaSections)
            {
                //release
                ListPool<string>.Release(variable.InputPairs);
            }
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
    

