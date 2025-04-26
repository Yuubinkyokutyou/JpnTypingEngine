#region

using System;
using System.Text;
using UnityEngine;

#endregion


namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピングゲームの入力処理ロジックを担当するクラス
    /// </summary>
    public class TypingLogic
    {
        private readonly TypingState _state;
        private readonly HiraganaToInputConverter _converter;
        private InputCombination InputCombination => _converter.InputCombination;

        public TypingLogic(TypingState state, HiraganaToInputConverter converter)
        {
            _state = state;
            _converter = converter;
        }

        /// <summary>
        ///     キー入力を処理し、結果を返す
        /// </summary>
        /// <param name="key">入力されたキー</param>
        /// <returns>入力処理後の結果</returns>
        public TypingInputResult ProcessInput(char key)
        {
            var result = new TypingInputResult();

            // 入力されたキーが、現在のセクションの選択キーと一致するか
            if (_state.CurrentSectionSelectKey != null &&
                _state.CurrentSectionInputtedKey.Length < _state.CurrentSectionSelectKey.Length &&
                _state.CurrentSectionSelectKey[_state.CurrentSectionInputtedKey.Length] == key)
            {
                result = HandleCorrectInput(key);
            }
            else
            {
                result = HandleIncorrectOrAlternativeInput(key);
            }

            // セクションの入力が完了した場合
            if (_state.CurrentSectionSelectKey != null &&
                _state.CurrentSectionInputtedKey.Length == _state.CurrentSectionSelectKey.Length)
            {
                result = HandleSectionCompletion(result);
            }

            return result;
        }

        private TypingInputResult HandleCorrectInput(char key)
        {
            var result = new TypingInputResult { IsSuccess = true };
            _state.AppendToCurrentSectionInputtedKey(key);
            return result;
        }


        /// <summary>
        ///     入力されたキーが正しくない、または代替の入力候補を処理する
        /// </summary>
        /// <param name="key"></param>
        /// <returns>入力処理の結果</returns>
        private TypingInputResult HandleIncorrectOrAlternativeInput(char key)
        {
            var result = new TypingInputResult();
            _state.AppendToCurrentSectionInputtedKey(key);
            try
            {
                // 入力されたキーに基づき、新しい入力候補を探す
                SetSuggestSectionsBasedOnInput(_state.InputtedHiragana.Length, _state.CurrentSectionInputtedKey);
                result.InputKeyChange = true; // 入力候補が変わった
            }
            catch (MissTypeException e) // 候補が見つからなかった場合（タイプミス）
            {
                // 追加したキーがミスの原因なので削除
                _state.RemoveLastFromCurrentSectionInputtedKey();
                result.IsMiss = true;
#if UNITY_EDITOR
                Debug.LogError($"タイプミス: {e.Message}");
#endif
            }
            catch (Exception e) // その他の予期せぬエラー
            {
                // 追加したキーがミスの原因の可能性があるので削除
                _state.RemoveLastFromCurrentSectionInputtedKey();
                result.IsMiss = true; // ミスとして扱う
#if UNITY_EDITOR
                Debug.LogError($"予期せぬエラー: {e.Message}");
#endif
            }
            return result;
        }

        private TypingInputResult HandleSectionCompletion(TypingInputResult currentResult)
        {
            var result = currentResult;
            _state.AppendToInputtedSectionKeys(); // 入力済みキーに追加
            _state.ClearCurrentSectionInputtedKey(); // 現在セクションの入力キーをクリア
            if (_state.SuggestHiraganaSections.Count > 0)
            {
                _state.AppendToInputtedHiragana(_state.SuggestHiraganaSections[0].Hiragana); // 入力済みひらがなに追加

                // 文章を入力し終えた場合
                if (_state.InputtedHiragana.Length == InputCombination.Hiragana.Length)
                {
                    result.IsFinished = true;
                }
                else
                {
                    // 次のセクションへ
                    SetNextSection();
                }
            }
            else
            {
                // SuggestHiraganaSectionsが空の場合は予期せぬ状態
#if UNITY_EDITOR
                Debug.LogError("セクション完了時にSuggestHiraganaSectionsが空です。");
#endif
                result.IsFinished = true; // 終了として扱う
            }
            return result;
        }

        /// <summary>
        ///     問題を設定し、状態を初期化する
        /// </summary>
        /// <param name="hiragana">問題文（ひらがな）</param>
        public void SetQuestion(string hiragana)
        {
            _state.Reset();
            _converter.Convert(hiragana);
            SetInitialSuggestSections(0);
        }


        /// <summary>
        ///     指定されたひらがなインデックスから始まる最初の入力候補セクションを設定する
        /// </summary>
        /// <param name="startHiraganaIndex">開始するひらがなのインデックス</param>
        private void SetInitialSuggestSections(int startHiraganaIndex)
        {
            _state.ClearSuggestSections();
            SetRemainingSuggestSections(startHiraganaIndex);
            if (_state.SuggestHiraganaSections.Count > 0 && _state.SuggestHiraganaSections[0].InputPairs.Count > 0)
            {
                _state.CurrentSectionSelectKey = _state.SuggestHiraganaSections[0].InputPairs[0];
            }
            else
            {
                _state.CurrentSectionSelectKey = ""; // 候補がない場合は空文字
#if UNITY_EDITOR
                Debug.LogWarning($"開始インデックス {startHiraganaIndex} からの入力候補が見つかりません。");
#endif
            }
        }


        /// <summary>
        ///     現在の入力に基づいて予測セクションを更新する
        /// </summary>
        private void SetSuggestSectionsBasedOnInput(int inputtedHiraganaLength, StringBuilder currentSectionInputtedKey)
        {
            int suggestIndex = inputtedHiraganaLength;

            // 現在のセクションで何かしらの入力がある場合、組み合わせを検索する
            var result = FindMatchingSectionAndPair(currentSectionInputtedKey, inputtedHiraganaLength);

            // GetSectionByInputKeyでException（タイプミス）が発生した場合ClearしないためにここでClear
            _state.ClearSuggestSections();
            _state.AddSuggestSection(result.section);
            suggestIndex += result.section.Hiragana.Length;
            _state.CurrentSectionSelectKey = result.selectInputPair; // 新しい選択キーを設定
            SetRemainingSuggestSections(suggestIndex); // 残りのセクションを設定
        }

        /// <summary>
        ///     指定された開始インデックス以降の残りの予測セクションを設定する
        /// </summary>
        private void SetRemainingSuggestSections(int startHiraganaIndex)
        {
            while (startHiraganaIndex < InputCombination.Hiragana.Length)
            {
                var hiraganaSections = InputCombination.GetHiraganaSections(startHiraganaIndex);

                if (hiraganaSections.Count == 0)
                {
                    // ここでエラーにするか、警告を出すかは設計次第
#if UNITY_EDITOR
                    Debug.LogWarning($"インデックス {startHiraganaIndex} から始まる有効なひらがなセクションが見つかりません。");
#endif
                    // ループを抜けるか、例外を投げる
                    break; // または throw new Exception($"インデックス {startHiraganaIndex} から始まる有効なひらがなセクションが見つかりません。");
                }


                // 入力ひらがな文字が0文字のセクションは割り当てない（nの例外の時のため）
                HiraganaSection setSection = null;
                foreach (var section in hiraganaSections)
                {
                    if (section.Hiragana.Length > 0 && section.InputPairs.Count > 0) // ひらがながあり、入力ペアも存在するか確認
                    {
                        setSection = section;
                        break;
                    }
                }

                if (setSection != null)
                {
                    _state.AddSuggestSection(setSection);
                    startHiraganaIndex += setSection.Hiragana.Length;
                }
                else
                {
                    // 適切なセクションが見つからない場合の処理
#if UNITY_EDITOR
                    Debug.LogWarning($"インデックス {startHiraganaIndex} から始まる、ひらがなを持つ有効なセクションが見つかりません。");
#endif
                    break; // ループを抜ける
                }
            }
        }


        /// <summary>
        ///     次の入力セクションに進む
        /// </summary>
        private void SetNextSection()
        {
            _state.RemoveFirstSuggestSection(); // 現在のセクションを削除
            if (_state.SuggestHiraganaSections.Count > 0 && _state.SuggestHiraganaSections[0].InputPairs.Count > 0)
            {
                // 次のセクションの最初の入力ペアを選択キーとする
                _state.CurrentSectionSelectKey = _state.SuggestHiraganaSections[0].InputPairs[0];
            }
            else
            {
                // 次のセクションがない、または入力ペアがない場合
                _state.CurrentSectionSelectKey = ""; // 空文字に設定
#if UNITY_EDITOR
                if (_state.SuggestHiraganaSections.Count == 0)
                    Debug.LogWarning("次のセクションに進もうとしましたが、SuggestHiraganaSectionsが空です。");
                else
                    Debug.LogWarning("次のセクションに進もうとしましたが、InputPairsが空です。");
#endif
            }
        }

        /// <summary>
        ///     入力中のキーに一致するセクションと入力ペアを探す
        /// </summary>
        private (HiraganaSection section, string selectInputPair) FindMatchingSectionAndPair(
            StringBuilder currentSectionInputtedKey, int inputtedHiraganaLength)
        {
            var hiraganaSections = InputCombination.GetHiraganaSections(inputtedHiraganaLength);

            foreach (var hiraganaSection in hiraganaSections)
            {
                foreach (var inputPair in hiraganaSection.InputPairs)
                {
                    // 入力済みより短い場合除外
                    if (inputPair.Length < currentSectionInputtedKey.Length) continue;

                    // 入力されたキーが前方一致するかチェック
                    bool match = true;
                    for (int i = 0; i < currentSectionInputtedKey.Length; i++)
                    {
                        if (inputPair[i] != currentSectionInputtedKey[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        // 一致するペアが見つかった
                        return (hiraganaSection, inputPair);
                    }
                }
            }

            // 一致するペアが見つからなかった場合（タイプミス）
            throw new MissTypeException($"入力 '{currentSectionInputtedKey}' に一致する候補が見つかりません。");
        }
    }

    /// <summary>
    ///     タイプミスを表す例外クラス
    /// </summary>
    public class MissTypeException : Exception
    {
        public MissTypeException()
        {
        }

        public MissTypeException(string message) : base(message)
        {
        }

        public MissTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}