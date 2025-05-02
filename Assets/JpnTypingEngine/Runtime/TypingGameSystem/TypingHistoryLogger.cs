#region

using System;
using System.Collections.Generic;
using UniRx;

// using ディレクティブを追加

#endregion

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピング入力履歴を記録するクラス
    /// </summary>
    public class TypingHistoryLogger : IDisposable, ITypingLogHistoryProvider
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly Subject<TypingLogEntry> _onLogAdded = new Subject<TypingLogEntry>();

        /// <summary>
        ///     ログが追加された時に発行されるイベント
        /// </summary>
        public IObservable<TypingLogEntry> OnLogAdded => _onLogAdded;

        /// <summary>
        ///     記録されたログのリスト（読み取り専用）
        /// </summary>
        public IReadOnlyList<TypingLogEntry> LogHistory => _logHistory;

        private readonly List<TypingLogEntry> _logHistory = new();


        /// <summary>
        ///     タイピングログを追加します
        /// </summary>
        /// <param name="logEntry">追加するログエントリ</param>
        public void AddLog(TypingLogEntry logEntry)
        {
            _logHistory.Add(logEntry);
            _onLogAdded.OnNext(logEntry);
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