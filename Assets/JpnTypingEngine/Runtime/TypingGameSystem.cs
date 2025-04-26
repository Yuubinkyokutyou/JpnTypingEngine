#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#endregion

namespace JpnTypingEngine
{
    /// <summary>
    ///     Converterの組み合わせをもとに、タイピングゲーム上での入力を受け取った結果を管理するクラス
    /// </summary>
    public class TypingGameSystem : IDisposable
    {
        public TypingInputResult TypingInputResult;

        private readonly HiraganaToInputConverter _hiraganaToInputConverter;
        private InputCombination InputCombination => _hiraganaToInputConverter.InputCombination;

        //予測の入力セクションを、入力順に格納
        private readonly List<HiraganaSection> _suggestHiraganaSections = new();

        //現在のセクションであるHiraganaSection[0]のどのキーが選択されているか
        private string _currentSectionSelectKey;

        //現在のセクションの入力済みキー
        private readonly StringBuilder _currentSectionInputtedKey = new();

        //入力されたひらがな
        private readonly StringBuilder _inputtedHiragana = new();

        //入力済みのキー（セクションが完了したときにアップデート）
        private readonly StringBuilder _inputtedSectionKeys = new();

        //表示key
        private readonly StringBuilder _viewInputKeys = new();

        //表示、未入力のキー
        private readonly StringBuilder _viewNotInputKeys = new();

        //表示、入力済みのキー
        private readonly StringBuilder _viewInputtedKeys = new();


        private const string HiraganaKeyPairListAssetPath = "HiraganaKeyPairList";

        public TypingGameSystem(HiraganaInputMapping hiraganaInputMapping = null)
        {
            if (hiraganaInputMapping == null)
            {
                hiraganaInputMapping = Resources.Load<HiraganaInputMapping>(HiraganaKeyPairListAssetPath);
            }

            _hiraganaToInputConverter = new HiraganaToInputConverter(hiraganaInputMapping);
        }

        public TypingInputResult InputKey(char key)
        {
            if (InputCombination == null)
            {
                throw new Exception("値がセットされていません");
            }

            //問題が終了済みの場合
            if (_inputtedHiragana.Length == InputCombination.Hiragana.Length)
            {
                throw new Exception("入力完了している状態で、入力されています。");
            }


            TypingInputResult = new TypingInputResult(
                _currentSectionInputtedKey,
                _inputtedHiragana,
                _inputtedSectionKeys,
                _viewInputKeys,
                _viewInputtedKeys,
                _viewNotInputKeys
            );

            //入力されたキーが、現在のセクションの選択キーと一致するか
            if (_currentSectionSelectKey[_currentSectionInputtedKey.Length] == key)
            {
                TypingInputResult.IsSuccess = true;
                _currentSectionInputtedKey.Append(key);
            }
            else
            {
                _currentSectionInputtedKey.Append(key);
                try
                {
                    SetSuggestSections(_inputtedHiragana.Length, _currentSectionInputtedKey);
                    TypingInputResult.InputKeyChange = true;
                }
                catch (Exception e)
                {
                    //追加したキーが、ミスのため最後のキーを削除
                    _currentSectionInputtedKey.Remove(_currentSectionInputtedKey.Length - 1, 1);
                    TypingInputResult.IsMiss = true;
# if UNITY_EDITOR
                    Debug.LogError(e.Message);
#endif
                }
//                 catch (Exception e)
//                 {
// # if UNITY_EDITOR
//                     // エラーが発生した場合、エラーの行数を表示
//                     Debug.LogError(e.Message);
//                     System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(e, true);
//                     int lineNumber = stackTrace.GetFrame(0).GetFileLineNumber();
//                     Debug.LogError("lineNumber:"+lineNumber);
// #endif
//                 }
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
                    TypingInputResult.IsFinished = true;
                }
                else
                {
                    SetNextSection();
                }
            }


            SetViewKeys();
            return TypingInputResult;
        }

        public TypingInputResult SetQuestion(string hiragana)
        {
            _inputtedHiragana.Clear();
            _inputtedSectionKeys.Clear();
            _currentSectionInputtedKey.Clear();
            _viewInputKeys.Clear();

            _hiraganaToInputConverter.Convert(hiragana);

            SetSuggestSections(0);

            SetViewKeys();

            TypingInputResult = new TypingInputResult(
                _currentSectionInputtedKey,
                _inputtedHiragana,
                _inputtedSectionKeys,
                _viewInputKeys,
                _viewInputtedKeys,
                _viewNotInputKeys
            );

            return TypingInputResult;
        }

        /// <summary>
        ///     _suggestHiraganaSectionsに予測のセクションを格納
        ///     _currentSectionSelectKeyに次のセクションの選択キーを格納
        /// </summary>
        private void SetSuggestSections(int inputtedHiraganaLength, StringBuilder newSectionInputtedKey)
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


        private void SetRemainSections(int startHiraganaIndex)
        {
            while (startHiraganaIndex < InputCombination.Hiragana.Length)
            {
                var hiraganaSections = InputCombination.GetHiraganaSections(startHiraganaIndex);

                if (hiraganaSections.Count == 0) throw new Exception("値が見つかりませんでした.");

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

        private void SetNextSection()
        {
            _currentSectionSelectKey = _suggestHiraganaSections[1].InputPairs[0];
            _suggestHiraganaSections.RemoveAt(0);
        }

        //入力中のセクションで、入力されたキーに一致するセクションを取得
        private (HiraganaSection section, string selectInputPair) GetSectionByInputKey(
            StringBuilder newSectionInputtedKey, int inputtedHiraganaLength)
        {
            int suggestIndex = inputtedHiraganaLength;

            var hiraganaSections = InputCombination.GetHiraganaSections(suggestIndex);

            foreach (var hiraganaSection in hiraganaSections)
            {
                foreach (var inputPair in hiraganaSection.InputPairs)
                {
                    //入力済みより短い場合除外
                    if (inputPair.Length < newSectionInputtedKey.Length) continue;

                    //一文字ずつチェックし、すべて一致していた場合追加
                    for (int i = 0; i < newSectionInputtedKey.Length; i++)
                    {
                        if (inputPair[i] != newSectionInputtedKey[i]) break;
                        if (i == newSectionInputtedKey.Length - 1)
                        {
                            return (hiraganaSection, inputPair);
                        }
                    }
                }
            }

            throw new MissTypeException("タイプミスが発生しました。");
        }

        private void SetViewKeys()
        {
            //１つ目は_currentSectionSelectKey
            _viewInputKeys.Clear();
            _viewInputKeys.Append(_inputtedSectionKeys);
            _viewInputKeys.Append(_currentSectionSelectKey);
            for (int i = 1; i < _suggestHiraganaSections.Count; i++)
            {
                _viewInputKeys.Append(_suggestHiraganaSections[i].InputPairs[0]);
            }

            _viewInputtedKeys.Clear();
            _viewInputtedKeys.Append(_inputtedSectionKeys);
            _viewInputtedKeys.Append(_currentSectionInputtedKey);

            _viewNotInputKeys.Clear();
            _viewNotInputKeys.Append(_viewInputKeys);
            _viewNotInputKeys.Remove(0, _viewInputtedKeys.Length);
        }

        public List<string> GetAllCombinations()
        {
            // StartIndexに基づいてHiraganaSectionをソート
            var sortedSections = _suggestHiraganaSections.OrderBy(section => section.StartIndex).ToList();

            // 初期化: 空のリストのリストから開始
            var results = new List<List<string>> { new() };

            // SortedSectionsごとにループ
            foreach (var section in sortedSections)
            {
                var newResults = new List<List<string>>();

                // 現在の結果セットに対して、新しい入力ペアを追加したリストを生成
                foreach (var result in results)
                {
                    foreach (var input in section.InputPairs)
                    {
                        var newResult = new List<string>(result) { input };
                        newResults.Add(newResult);
                    }
                }

                // 結果を更新
                results = newResults;
            }

            // 最終的な組み合わせを文字列に結合して返す
            return results.Select(resultList => string.Join("", resultList)).ToList();
        }


        public void Dispose()
        {
            _hiraganaToInputConverter?.Dispose();
        }
    }


    public class MissTypeException : Exception
    {
        public MissTypeException(string message)
            : base(message)
        {
        }
    }
}