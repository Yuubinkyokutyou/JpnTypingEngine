using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JpnTypingEngine.InGame
{
    public class TypingGamePresenter : MonoBehaviour
    {
        [SerializeField] private QuestionDisplay questionDisplay;
        [SerializeField] InputKeyProvider inputKeyProvider;
        [SerializeField] List<string> hiraganaQuestionList;
        
        int QuestionIndex = 0;
        
        private HiraganaToInputConverter _hiraganaToInputConverter;
        readonly StringBuilder _viewInputKeys = new StringBuilder();
        private InputCombination _inputCombination;
        
        //予測の入力セクションを、入力順に格納
        readonly List<HiraganaSection> _suggestHiraganaSections = new List<HiraganaSection>();
        //現在のセクションであるHiraganaSection[0]のどのキーが選択されているか
        private string _currentSectionSelectKey;
        //現在のセクションの入力済みキー
        readonly StringBuilder _currentSectionInputtedKey = new StringBuilder();

        //入力されたひらがな
        readonly StringBuilder _inputtedHiragana = new StringBuilder();
        //入力済みのキー（セクションが完了したときにアップデート）
        readonly StringBuilder _inputtedKeys = new StringBuilder();
        
        private void Awake()
        {
            var hiraganaKeyPairList= Resources.Load<HiraganaKeyPairList>("HiraganaKeyPairList");
            _hiraganaToInputConverter = new HiraganaToInputConverter(hiraganaKeyPairList);
        }

        private void Start()
        {
            inputKeyProvider.OnInputKey += InputKey;
            
            SetQuestion(hiraganaQuestionList[0]);
        }
        
        private void OnDestroy()
        {
            inputKeyProvider.OnInputKey -= InputKey;
            
            _hiraganaToInputConverter?.Dispose();
        }

        private void InputKey(char key)
        {
            if (_inputCombination == null)
            {
                throw new Exception("値がセットされていません");
            }
            
            //入力されたキーが、現在のセクションの選択キーと一致するか
            if (_currentSectionSelectKey[_currentSectionInputtedKey.Length] == key)
            {
                _currentSectionInputtedKey.Append(key);
            }
            else
            {
                _currentSectionInputtedKey.Append(key);
                try
                {
                    SetSuggestSections(_inputtedHiragana.Length, _currentSectionInputtedKey);
                }
                catch(Exception e)
                {
                    //追加したキーが、ミスのため最後のキーを削除
                    _currentSectionInputtedKey.Remove(_currentSectionInputtedKey.Length - 1, 1);
                   
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
                _inputtedKeys.Append(_currentSectionInputtedKey);
                _currentSectionInputtedKey.Clear();
                _inputtedHiragana.Append(_suggestHiraganaSections[0].Hiragana);
                
                //文章を入力し終えた場合
                if (_inputtedHiragana.Length == _inputCombination.Hiragana.Length)
                {
                    QuestionIndex++;
                    SetQuestion(hiraganaQuestionList[QuestionIndex]);
                }
                else
                {
                    //次のセクションのための予測をセット
                    SetSuggestSections(_inputtedHiragana.Length);
                }
                
            }
            
            UpdateView();
        }

        private void UpdateView()
        {
            GetAndSetViewInputKeys();
            questionDisplay.UpdateInputKey(_viewInputKeys.ToString(), _inputtedKeys.Length + _currentSectionInputtedKey.Length,
                _inputCombination.Hiragana, _inputtedHiragana.Length);
        }

        private void SetQuestion(string hiragana)
        {
            _inputtedHiragana.Clear();
            _inputtedKeys.Clear();
            _currentSectionInputtedKey.Clear();
            _viewInputKeys.Clear();
            
            _inputCombination = _hiraganaToInputConverter.Convert(hiragana);
            
            SetSuggestSections(0);
            UpdateView();
        }
        
        
        
        /// <summary>
        /// _suggestHiraganaSectionsに予測のセクションを格納
        /// </summary>
        /// <param name="inputtedHiraganaLength"></param>
        /// <param name="newSectionInputtedKey"></param>
        private void SetSuggestSections(int inputtedHiraganaLength,StringBuilder newSectionInputtedKey = null)
        {
            
            int suggestIndex = inputtedHiraganaLength;

            if (newSectionInputtedKey != null)
            {
                //現在のセクションで何かしらの入力がある場合、組み合わせを検索する
                var result = GetSectionByInputKey(newSectionInputtedKey, inputtedHiraganaLength);
                //GetSectionByInputKeyでExceptionが発生した場合ClearしないためにここでClear
                _suggestHiraganaSections.Clear();
                _suggestHiraganaSections.Add(result.section);
                _currentSectionSelectKey = result.selectInputPair;
                suggestIndex += result.section.Hiragana.Length;
            }
            else
            {
                var hiraganaSections = _inputCombination.GetHiraganaSections(suggestIndex);
                
                _suggestHiraganaSections.Clear();
                _suggestHiraganaSections.Add(hiraganaSections[0]);
                suggestIndex += hiraganaSections[0].Hiragana.Length;
                _currentSectionSelectKey = hiraganaSections[0].InputPairs[0];

            }
            
            while (suggestIndex < _inputCombination.Hiragana.Length)
            {
                var hiraganaSections = _inputCombination.GetHiraganaSections(suggestIndex);
                _suggestHiraganaSections.Add(hiraganaSections[0]);
                suggestIndex += hiraganaSections[0].Hiragana.Length;
            }
            
        }

        //入力中のセクションで、入力されたキーに一致するセクションを取得
        private (HiraganaSection section,string selectInputPair) GetSectionByInputKey(StringBuilder newSectionInputtedKey,int inputtedHiraganaLength)
        {
            int suggestIndex = inputtedHiraganaLength;

            var hiraganaSections = _inputCombination.GetHiraganaSections(suggestIndex);
            
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
            _viewInputKeys.Append(_inputtedKeys);
            _viewInputKeys.Append(_currentSectionSelectKey);
            for(int i = 1; i < _suggestHiraganaSections.Count; i++)
            {
                _viewInputKeys.Append(_suggestHiraganaSections[i].InputPairs[0]);
            }

            return _viewInputKeys;
        }
        
    }

}