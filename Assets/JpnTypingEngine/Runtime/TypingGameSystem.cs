using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JpnTypingEngine
{
    /// <summary>
    /// Converterの組み合わせをもとに、タイピングゲーム上での入力を受け取った結果を管理するクラス
    /// </summary>
    public class TypingGameSystem :IDisposable
    {
        public TypingInputResult TypingInputResult{ get; private set; }

        private HiraganaToInputConverter _hiraganaToInputConverter;
        private InputCombination InputCombination => _hiraganaToInputConverter.InputCombination;
        
        //予測の入力セクションを、入力順に格納
        readonly List<HiraganaSection> _suggestHiraganaSections = new List<HiraganaSection>();
        //現在のセクションであるHiraganaSection[0]のどのキーが選択されているか
        private string _currentSectionSelectKey;
        //現在のセクションの入力済みキー
        readonly StringBuilder _currentSectionInputtedKey = new StringBuilder();
        //入力されたひらがな
        readonly StringBuilder _inputtedHiragana = new StringBuilder();
        //入力済みのキー（セクションが完了したときにアップデート）
        readonly StringBuilder _inputtedSectionKeys = new StringBuilder();
        //表示key
        readonly StringBuilder _viewInputKeys = new StringBuilder();

        private const string HiraganaKeyPairListAssetPath = "HiraganaKeyPairList";

        public TypingGameSystem(HiraganaKeyPairList hiraganaKeyPairList=null)
        {
            if (hiraganaKeyPairList == null)
            {
                hiraganaKeyPairList = Resources.Load<HiraganaKeyPairList>(HiraganaKeyPairListAssetPath);
            }
            _hiraganaToInputConverter = new HiraganaToInputConverter(hiraganaKeyPairList);
        }
        
        
        public TypingInputResult InputKey(char key)
        {
            if (InputCombination == null)
            {
                throw new Exception("値がセットされていません");
            }

            TypingInputResult typingInputResult = new(
                _currentSectionInputtedKey,
                _inputtedHiragana,
                _inputtedSectionKeys,
                _viewInputKeys
                );
            
            //入力されたキーが、現在のセクションの選択キーと一致するか
            if (_currentSectionSelectKey[_currentSectionInputtedKey.Length] == key)
            {
                typingInputResult.IsSuccess = true;
                _currentSectionInputtedKey.Append(key);
            }
            else
            {
                _currentSectionInputtedKey.Append(key);
                try
                {
                    SetSuggestSections(_inputtedHiragana.Length, _currentSectionInputtedKey);
                    typingInputResult.InputKeyChange = true;
                }
                catch(Exception e)
                {
                    //追加したキーが、ミスのため最後のキーを削除
                    _currentSectionInputtedKey.Remove(_currentSectionInputtedKey.Length - 1, 1);
                    typingInputResult.IsMiss = true;
# if UNITY_EDITOR
                    //エラーが発生した場合、エラーの行数を表示
                    Debug.LogError(e.Message);
                    System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(e, true);
                    int lineNumber = stackTrace.GetFrame(0).GetFileLineNumber();
                    Debug.LogError("lineNumber:"+lineNumber);
#endif
                }
            }
            
            
            //セクションの入力が完了した場合
            if (_currentSectionInputtedKey.Length == _currentSectionSelectKey.Length)
            {
                _inputtedSectionKeys.Append(_currentSectionInputtedKey);
                _currentSectionInputtedKey.Clear();
                _inputtedHiragana.Append(_suggestHiraganaSections[0].Hiragana);
                
                //文章を入力し終えた場合
                if (_inputtedHiragana.Length == InputCombination.Hiragana.Length)
                {
                    typingInputResult.IsFinished = true;
                }
                else
                {
                    SetNextSection();
                }
                
            }


            GetAndSetViewInputKeys();
            return typingInputResult;
        }
        
        public TypingInputResult SetQuestion(string hiragana)
        {
            _inputtedHiragana.Clear();
            _inputtedSectionKeys.Clear();
            _currentSectionInputtedKey.Clear();
            _viewInputKeys.Clear();

            _hiraganaToInputConverter.Convert(hiragana);
            
            SetSuggestSections(0);
            
            GetAndSetViewInputKeys();
            
            return new TypingInputResult(
                _currentSectionInputtedKey,
                _inputtedHiragana,
                _inputtedSectionKeys,
                _viewInputKeys
            );
        }
        
        /// <summary>
        /// _suggestHiraganaSectionsに予測のセクションを格納
        /// _currentSectionSelectKeyに次のセクションの選択キーを格納
        /// </summary>
        private void SetSuggestSections(int inputtedHiraganaLength,StringBuilder newSectionInputtedKey)
        {
            int suggestIndex = inputtedHiraganaLength;

            //現在のセクションで何かしらの入力がある場合、組み合わせを検索する
            var result = GetSectionByInputKey(newSectionInputtedKey, inputtedHiraganaLength);
            //GetSectionByInputKeyでException（タイプミス）が発生した場合ClearしないためにここでClear
            _suggestHiraganaSections.Clear();
            _suggestHiraganaSections.Add(result.section);
            suggestIndex += result.section.Hiragana.Length;
            _currentSectionSelectKey = result.selectInputPair;
            SetRemainSections(suggestIndex);
        }

        private void SetSuggestSections(int inputtedHiraganaLength)
        {
            _suggestHiraganaSections.Clear();
            SetRemainSections(inputtedHiraganaLength);
            _currentSectionSelectKey = _suggestHiraganaSections[0].InputPairs[0];
        }

        
        void SetRemainSections(int startHiraganaIndex)
        {
            while (startHiraganaIndex < InputCombination.Hiragana.Length)
            {
                var hiraganaSections = InputCombination.GetHiraganaSections(startHiraganaIndex);
                
                if(hiraganaSections.Count == 0) throw new Exception("値が見つかりませんでした.");
                
                //入力ひらがな文字が0文字のセクションは割り当てない（nの例外の時のため）
                HiraganaSection setSection = null;
                foreach (var variable in hiraganaSections)
                {
                    if (variable.Hiragana.Length > 0)
                    {
                        setSection = variable;
                        break;
                    }
                }
                
                _suggestHiraganaSections.Add(setSection);
                startHiraganaIndex += setSection.Hiragana.Length;
            }
        }
        
        void SetNextSection()
        {
            _currentSectionSelectKey = _suggestHiraganaSections[1].InputPairs[0];
            _suggestHiraganaSections.RemoveAt(0);
        }

        //入力中のセクションで、入力されたキーに一致するセクションを取得
        private (HiraganaSection section,string selectInputPair) GetSectionByInputKey(StringBuilder newSectionInputtedKey,int inputtedHiraganaLength)
        {
            int suggestIndex = inputtedHiraganaLength;

            var hiraganaSections = InputCombination.GetHiraganaSections(suggestIndex);
            
            foreach (var hiraganaSection in hiraganaSections)
            {
                foreach (var inputPair in hiraganaSection.InputPairs)
                {                    
                    for (int i = 0; i < newSectionInputtedKey.Length; i++)
                    {
                        if (inputPair.Length <= i) continue;
                        
                        //一文字ずつチェックし、すべて一致していた場合追加
                        if (inputPair[i] == newSectionInputtedKey[i] && i == newSectionInputtedKey.Length - 1)
                        {
                            return (hiraganaSection,inputPair);
                        }
                    }
                }
            }

            throw new Exception("値が見つかりませんでした");
        }
        
        private StringBuilder GetAndSetViewInputKeys()
        {
            //１つ目は_currentSectionSelectKey
            _viewInputKeys.Clear();
            _viewInputKeys.Append(_inputtedSectionKeys);
            _viewInputKeys.Append(_currentSectionSelectKey);
            for(int i = 1; i < _suggestHiraganaSections.Count; i++)
            {
                _viewInputKeys.Append(_suggestHiraganaSections[i].InputPairs[0]);
            }

            return _viewInputKeys;
        }
        
        
        public void Dispose()
        {
            _hiraganaToInputConverter?.Dispose();
        }
    }
}