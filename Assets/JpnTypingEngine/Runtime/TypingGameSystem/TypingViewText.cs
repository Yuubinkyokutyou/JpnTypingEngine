namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     表示用のテキストデータ構造体
    /// </summary>
    public struct TypingViewText
    {
        /// <summary>
        ///     表示テキスト
        /// </summary>
        public string InputKeys;

        /// <summary>
        ///     表示、未入力のテキスト
        /// </summary>
        public string NotInputKeys;

        /// <summary>
        ///     表示、入力済みのテキスト
        /// </summary>
        public string InputtedKeys;

        /// <summary>
        ///     入力済みひらがな
        /// </summary>
        public string InputtedHiragana;
    }
}