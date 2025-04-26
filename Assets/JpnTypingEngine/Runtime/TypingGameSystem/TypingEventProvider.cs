#region

using System;
using UniRx;
using UnityEngine;

#endregion

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピング入力イベントを発行するクラス
    /// </summary>
    public class TypingEventProvider : IDisposable
    {
        // OnInput サブジェクト: TypingLogEntry イベントを発行
        private readonly Subject<TypingLogEntry> _onInput = new();
        public IObservable<TypingLogEntry> OnInput => _onInput;

        /// <summary>
        ///     入力イベントを発行します。
        /// </summary>
        /// <param name="key">入力されたキー</param>
        /// <param name="result">入力結果</param>
        /// <param name="state">（任意）記録したい場合の状態オブジェクト</param>
        public void Publish(char key, TypingInputResult result, TypingState state = null)
        {
            var entry = new TypingLogEntry
            {
                InputKey = key,
                Result = result,
                Timestamp = Time.time
                // 必要であれば state から詳細情報を取得して entry に設定
                // CurrentInputtedKeys = state?.InputtedSectionKeys.ToString() + state?.CurrentSectionInputtedKey.ToString(),
                // CurrentInputtedHiragana = state?.InputtedHiragana.ToString()
            };
            _onInput.OnNext(entry);
        }

        /// <summary>
        ///     リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            _onInput?.Dispose();
        }
    }
}