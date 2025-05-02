#region

using System;
using System.Collections.Generic;
using UniRx;
using JpnTypingEngine.TypingGameSystem; // using ディレクティブを追加

#endregion

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピング入力履歴を記録するクラス (UniRx版)
    /// </summary>
    public class TypingHistoryLoggerRx : IDisposable, ITypingLogHistoryProvider // ITypingLogHistoryProvider を実装
    {
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        ///     記録されたログのリスト（読み取り専用）
        /// </summary>
        public IReadOnlyList<TypingLogEntry> LogHistory => _logHistory;

        private readonly List<TypingLogEntry> _logHistory = new();

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="inputStream">購読する入力イベントストリーム</param>
        public TypingHistoryLoggerRx(IObservable<TypingLogEntry> inputStream)
        {
            // 入力イベントストリームを購読し、ログリストに追加する
            inputStream
                .Subscribe(entry => _logHistory.Add(entry))
                .AddTo(_disposables); // Dispose時に購読解除されるように登録
        }

        /// <summary>
        ///     ログ履歴をクリアします。
        /// </summary>
        public void Clear()
        {
            _logHistory.Clear();
        }

        /// <summary>
        ///     リソース（購読）を解放します。
        /// </summary>
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}