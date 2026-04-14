#nullable enable
using System;
using System.Threading.Tasks;

namespace BakaTest.Services.SceneManagement
{
    /// <summary>
    /// シーン管理サービスのインターフェース
    /// </summary>
    public interface ISceneManagementService
    {
        /// <summary>
        /// 現在のシーン名
        /// </summary>
        string CurrentSceneName { get; }

        /// <summary>
        /// シーンをロードします
        /// </summary>
        /// <param name="sceneName">ロードするシーン名</param>
        void LoadScene(string sceneName);

        /// <summary>
        /// シーンを非同期でロードします
        /// </summary>
        /// <param name="sceneName">ロードするシーン名</param>
        Task LoadSceneAsync(string sceneName);

        /// <summary>
        /// メインメニューシーンをロードします
        /// </summary>
        void LoadMainMenu();

        /// <summary>
        /// テスト選択シーンをロードします
        /// </summary>
        void LoadTestSelection();

        /// <summary>
        /// テスト受験シーンをロードします
        /// </summary>
        void LoadTestTaking();

        /// <summary>
        /// チャンピオン選択シーンをロードします
        /// </summary>
        void LoadChampionSelection();

        /// <summary>
        /// ポイント配分シーンをロードします
        /// </summary>
        void LoadPointAllocation();

        /// <summary>
        /// バトルシーンをロードします
        /// </summary>
        void LoadBattle();

        /// <summary>
        /// 結果シーンをロードします
        /// </summary>
        void LoadResults();

        /// <summary>
        /// シーンロード開始時に発火するイベント
        /// </summary>
        event Action<string>? SceneLoadStarted;

        /// <summary>
        /// シーンロード完了時に発火するイベント
        /// </summary>
        event Action<string>? SceneLoadCompleted;
    }
}
