#region

using System.Collections.Generic;
using System.Linq;

#endregion

// using ディレクティブを追加

namespace JpnTypingEngine.TypingGameSystem.Score
{
    /// <summary>
    ///     タイピング履歴から最終的なスコア結果を計算するクラス
    /// </summary>
    public class ScoreResultCalculator
    {
        private readonly ITypingLogHistoryProvider _logHistoryProvider;

        /// <summary>
        ///     タイピングログ履歴プロバイダーを注入するコンストラクタ
        /// </summary>
        /// <param name="logHistoryProvider">タイピングログ履歴プロバイダー</param>
        public ScoreResultCalculator(ITypingLogHistoryProvider logHistoryProvider)
        {
            _logHistoryProvider = logHistoryProvider;
        }
        
        /// <summary>
        ///     指定されたタイピングログ履歴からスコア結果を計算します。
        /// </summary>
        /// <param name="logHistory">計算対象のログ履歴</param>
        /// <returns>計算されたスコア結果</returns>
        public ScoreResult CalculateResult(IReadOnlyList<TypingLogEntry> logHistory)
        {
            if (logHistory == null || logHistory.Count == 0)
            {
                // ログがない場合はデフォルト値を返す
                return new ScoreResult(0, 0, 0f, 0f);
            }

            int successCount = 0;
            int missCount = 0;

            foreach (var entry in logHistory)
            {
                if (entry.Result.IsSuccess)
                {
                    successCount++;
                }
                else if (entry.Result.IsMiss)
                {
                    missCount++;
                }
            }

            int totalTypes = successCount + missCount;
            float accuracy = (totalTypes > 0) ? (float)successCount / totalTypes : 0f;

            // 最初と最後のログエントリのタイムスタンプを取得して経過時間とする
            float elapsedTime =　logHistory.LastOrDefault().Timestamp - logHistory.FirstOrDefault().Timestamp;
            float keysPerSecond = (elapsedTime > 0 && successCount > 0) ? successCount / elapsedTime : 0f;

            return new ScoreResult(successCount, missCount, accuracy, keysPerSecond);
        }

        /// <summary>
        ///     注入されたITypingLogHistoryProviderインスタンスからスコア結果を計算します。
        /// </summary>
        /// <returns>計算されたスコア結果</returns>
        public ScoreResult CalculateResult()
        {
            return CalculateResult(_logHistoryProvider.LogHistory);
        }
    }
}