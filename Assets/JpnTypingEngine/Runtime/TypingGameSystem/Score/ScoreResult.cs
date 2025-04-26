using System;

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    /// タイピングゲームのスコア結果を格納するクラス
    /// </summary>
    public class ScoreResult
    {
        /// <summary>
        /// 成功したキータイプ数
        /// </summary>
        public int SuccessKeyTypes { get; }

        /// <summary>
        /// 失敗したキータイプ数
        /// </summary>
        public int MissKeyTypes { get; }

        /// <summary>
        /// 成功キータイプ確率（正確性） (0.0 ~ 1.0)
        /// </summary>
        public float Accuracy { get; }

        /// <summary>
        /// 1秒あたりの成功キータイプ数 (Keys Per Second)
        /// </summary>
        public float KeysPerSecond { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="successKeyTypes">成功キータイプ数</param>
        /// <param name="missKeyTypes">失敗キータイプ数</param>
        /// <param name="accuracy">成功キータイプ確率</param>
        /// <param name="keysPerSecond">1秒あたりの成功キータイプ数</param>
        public ScoreResult(int successKeyTypes, int missKeyTypes, float accuracy, float keysPerSecond)
        {
            SuccessKeyTypes = successKeyTypes;
            MissKeyTypes = missKeyTypes;
            Accuracy = accuracy;
            KeysPerSecond = keysPerSecond;
        }
    }
}
