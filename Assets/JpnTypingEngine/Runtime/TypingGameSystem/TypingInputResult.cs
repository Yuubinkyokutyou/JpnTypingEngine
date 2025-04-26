namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     入力した結果
    /// </summary>
    public record TypingInputResult
    {
        //入力が完了したかどうか
        public bool IsFinished;

        public bool IsMiss;

        //入力成功
        public bool IsSuccess;

        //入力キーが変更されたかどうか
        public bool InputKeyChange;
    }
}