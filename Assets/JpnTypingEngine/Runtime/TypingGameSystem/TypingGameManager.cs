#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピングゲームシステム全体のファサード（窓口）クラス
    ///     状態管理、入力ロジック、表示データ生成を他のクラスに委譲する
    /// </summary>
    public class TypingGameManager : IDisposable
    {
        private readonly HiraganaToInputConverter _hiraganaToInputConverter;
        private readonly TypingState _state;
        private readonly TypingLogic _logic;
        private readonly TypingViewDataBuilder _viewBuilder;

        // HiraganaInputMappingの読み込みパス（外部から注入する方が望ましい）
        private const string HiraganaKeyPairListAssetPath = "HiraganaKeyPairList";

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="hiraganaInputMapping">ひらがなとローマ字のマッピング情報。nullの場合、Resourcesから読み込む</param>
        public TypingGameManager(HiraganaInputMapping hiraganaInputMapping = null)
        {
            // 依存性の注入: HiraganaInputMappingを外部から受け取る
            if (hiraganaInputMapping == null)
            {
                // フォールバックとしてResources.Loadを使用（テスト容易性のため推奨されない）
                hiraganaInputMapping = Resources.Load<HiraganaInputMapping>(HiraganaKeyPairListAssetPath);
                if (hiraganaInputMapping == null)
                {
                    throw new Exception(
                        $"Failed to load HiraganaInputMapping from Resources path: {HiraganaKeyPairListAssetPath}");
                }
            }

            _hiraganaToInputConverter = new HiraganaToInputConverter(hiraganaInputMapping);
            _state = new TypingState();
            _viewBuilder = new TypingViewDataBuilder();
            // TypingLogicにStateとConverterを渡す
            _logic = new TypingLogic(_state, _hiraganaToInputConverter);
        }

        /// <summary>
        ///     キー入力を処理し、結果を返す
        /// </summary>
        /// <param name="key">入力されたキー</param>
        /// <returns>入力結果</returns>
        public TypingInputResult InputKey(char key)
        {
            if (_hiraganaToInputConverter.InputCombination == null)
            {
                throw new InvalidOperationException("Question not set. Call SetQuestion first.");
            }

            // 問題が終了済みの場合
            if (_state.InputtedHiragana.Length == _hiraganaToInputConverter.InputCombination.Hiragana.Length)
            {
                throw new InvalidOperationException("Input received after the question is already completed.");
            }

            // 入力ロジックを実行
            var result = _logic.ProcessInput(key);
            // 表示用データを更新
            _viewBuilder.UpdateViewKeys(_state);

            // 結果オブジェクトに最新の状態を反映
            return result;
        }

        /// <summary>
        ///     新しい問題（ひらがな）を設定する
        /// </summary>
        /// <param name="hiragana">問題文（ひらがな）</param>
        /// <returns>初期状態の結果</returns>
        public TypingInputResult SetQuestion(string hiragana)
        {
            if (string.IsNullOrEmpty(hiragana))
            {
                throw new ArgumentException("Hiragana string cannot be null or empty.", nameof(hiragana));
            }

            // 状態とロジックをリセットし、新しい問題をセット
            _logic.SetQuestion(hiragana);

            // 表示用データを初期化・更新
            _viewBuilder.Clear(); // 先にクリア
            _viewBuilder.UpdateViewKeys(_state);

            // 初期状態の結果を生成して返す
            return new TypingInputResult();
        }

        /// <summary>
        ///     現在の表示用テキストデータを取得する
        /// </summary>
        /// <returns>表示用テキストデータ</returns>
        public TypingViewText GetTypingViewText()
        {
            return new TypingViewText
            {
                InputKeys = _viewBuilder.ViewInputKeys.ToString(),
                NotInputKeys = _viewBuilder.ViewNotInputKeys.ToString(),
                InputtedKeys = _viewBuilder.ViewInputtedKeys.ToString(),
                InputtedHiragana = _state.InputtedHiragana.ToString()
            };
        }

        /// <summary>
        ///     現在の問題に対する全ての可能な入力組み合わせを取得する
        ///     （このメソッドはロジッククラスやコンバータークラスに移譲することも検討可能）
        /// </summary>
        /// <returns>入力組み合わせのリスト</returns>
        public List<string> GetAllCombinations()
        {
            if (_hiraganaToInputConverter.InputCombination == null)
            {
                return new List<string>(); // 問題未設定時は空リスト
            }

            return _hiraganaToInputConverter.GetAllCombinationKeysUsesDanger();
        }

        public void Dispose()
        {
            _hiraganaToInputConverter?.Dispose();
        }
    }
}