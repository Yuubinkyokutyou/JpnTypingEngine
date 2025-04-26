namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// リアルタイムでスコアを計算するクラス
    /// 未使用（AI生成）
    /// </summary>
    /*public class ScoreCalculatorRx : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        ///     現在のスコア（読み取り専用、変更通知付き）
        /// </summary>
        public IReadOnlyReactiveProperty<float> CurrentScore => _currentScore;

        private readonly ReactiveProperty<float> _currentScore = new(0f);

        // スコア計算ロジックの例
        private const float SuccessScore = 10f;
        private const float MissScorePenalty = -5f;
        // TODO: 時間経過による減点、コンボボーナスなどのロジックを追加

        /// <summary>
        ///     コンストラクタ
        /// </summary>
        /// <param name="inputStream">購読する入力イベントストリーム</param>
        public ScoreCalculatorRx(IObservable<TypingLogEntry> inputStream)
        {
            // 入力イベントストリームを購読し、スコアを更新する
            inputStream
                // .ObserveOn(Scheduler.ThreadPool) // 計算が重い場合は別スレッドで実行
                .Subscribe(entry => { CalculateScore(entry); })
                .AddTo(_disposables); // Dispose時に購読解除されるように登録
        }

        /// <summary>
        ///     個々のログエントリに基づいてスコアを計算・更新します。
        /// </summary>
        /// <param name="entry">ログエントリ</param>
        private void CalculateScore(TypingLogEntry entry)
        {
            float scoreDelta = 0f;

            if (entry.Result.IsSuccess)
            {
                scoreDelta += SuccessScore;
                // TODO: コンボ判定・加算など
            }
            else if (entry.Result.IsMiss)
            {
                scoreDelta += MissScorePenalty;
                // TODO: コンボ中断など
            }

            // TODO: 入力速度に応じたボーナスなど

            _currentScore.Value += scoreDelta;
        }

        /// <summary>
        ///     スコアをリセットします。
        /// </summary>
        public void Reset()
        {
            _currentScore.Value = 0f;
            // TODO: コンボカウントなどもリセット
        }

        /// <summary>
        ///     リソース（購読）を解放します。
        /// </summary>
        public void Dispose()
        {
            _disposables?.Dispose();
            _currentScore?.Dispose();
        }
    }*/
}