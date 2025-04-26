namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピング入力のログエントリ
    /// </summary>
    public struct TypingLogEntry
    {
        /// <summary>
        ///     入力されたキー
        /// </summary>
        public char InputKey;

        /// <summary>
        ///     入力結果
        /// </summary>
        public TypingInputResult Result;

        /// <summary>
        ///     入力時のタイムスタンプ (ゲーム開始からの経過時間)
        /// </summary>
        public float Timestamp;

        // 必要に応じて、記録時の詳細な状態を追加できます
        // 例:
        // public string CurrentInputtedKeys; // その時点での入力済みローマ字
        // public string CurrentRemainingKeys; // その時点での残りローマ字
        // public string CurrentInputtedHiragana; // その時点での入力済みひらがな
    }
}