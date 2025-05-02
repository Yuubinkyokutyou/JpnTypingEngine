#region

using System;
using UniRx;

#endregion

namespace JpnTypingEngine.TypingGameSystem
{
    /// <summary>
    ///     タイピングログイベントを中継するPresenterクラス
    ///     タイピングゲームマネージャーが発行するイベントをログ記録クラスなどに中継します
    /// </summary>
    public class TypingLogPresenter : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        /// <summary>
        ///     タイピングゲームマネージャーに接続する
        /// </summary>
        /// <param name="manager">接続するタイピングゲームマネージャー</param>
        /// <param name="typingHistoryLogger"></param>
        public void ConnectToManager(TypingGameManager manager, TypingHistoryLogger typingHistoryLogger)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            manager.TypingLogObservable.Subscribe(e => typingHistoryLogger.AddLog(e)).AddTo(_disposables);
        }

        /// <summary>
        ///     リソースを解放する
        /// </summary>
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}