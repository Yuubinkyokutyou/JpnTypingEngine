using System.Collections.Generic;

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// タイピングログ履歴を提供するインターフェース
    /// </summary>
    public interface ITypingLogHistoryProvider
    {
        /// <summary>
        /// 記録されたログのリスト（読み取り専用）
        /// </summary>
        IReadOnlyList<TypingLogEntry> LogHistory { get; }
    }
}
