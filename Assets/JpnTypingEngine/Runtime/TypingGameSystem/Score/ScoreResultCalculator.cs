using System.Collections.Generic;
using System.Linq;

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// タイピング履歴から最終的なスコア結果を計算するクラス
    /// </summary>
    public static class ScoreResultCalculator
    {
        /// <summary>
        /// 指定されたタイピングログ履歴からスコア結果を計算します。
        /// </summary>
        /// <param name="logHistory">計算対象のログ履歴</param>
        /// <returns>計算されたスコア結果</returns>
        public static ScoreResult CalculateResult(IReadOnlyList<TypingLogEntry> logHistory)
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

            // 最後のログエントリのタイムスタンプを取得して経過時間とする
            float elapsedTime = logHistory.LastOrDefault().Timestamp;
            float keysPerSecond = (elapsedTime > 0 && successCount > 0) ? successCount / elapsedTime : 0f;

            return new ScoreResult(successCount, missCount, accuracy, keysPerSecond);
        }

        /// <summary>
        /// TypingHistoryLoggerRx インスタンスからスコア結果を計算します。
        /// </summary>
        /// <param name="logger">タイピング履歴ロガー</param>
        /// <returns>計算されたスコア結果</returns>
        public static ScoreResult CalculateResult(TypingHistoryLoggerRx logger)
        {
            return CalculateResult(logger.LogHistory);
        }
    }
}
