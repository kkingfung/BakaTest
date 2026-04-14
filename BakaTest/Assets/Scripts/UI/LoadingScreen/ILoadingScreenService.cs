#nullable enable
using System;

namespace BakaTest.UI.LoadingScreen
{
    /// <summary>
    /// ローディング画面管理サービスのインターフェース
    /// </summary>
    public interface ILoadingScreenService
    {
        /// <summary>
        /// ローディング画面を表示します
        /// </summary>
        /// <param name="message">表示するメッセージ（オプション）</param>
        void Show(string? message = null);

        /// <summary>
        /// ローディング画面を非表示にします
        /// </summary>
        /// <param name="fadeOut">フェードアウトするか</param>
        void Hide(bool fadeOut = true);

        /// <summary>
        /// 進行状況を更新します（0.0 ~ 1.0）
        /// </summary>
        /// <param name="progress">進行状況</param>
        void SetProgress(float progress);

        /// <summary>
        /// メッセージを更新します
        /// </summary>
        /// <param name="message">表示するメッセージ</param>
        void SetMessage(string message);

        /// <summary>
        /// ローディング画面が表示中か
        /// </summary>
        bool IsShowing { get; }

        /// <summary>ローディング画面が表示された時に発火</summary>
        event Action? LoadingScreenShown;

        /// <summary>ローディング画面が非表示になった時に発火</summary>
        event Action? LoadingScreenHidden;
    }
}
