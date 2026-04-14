#nullable enable
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BakaTest.Services.SceneManagement
{
    /// <summary>
    /// シーン管理サービスの実装
    /// </summary>
    public class SceneManagementService : ISceneManagementService
    {
        // シーン名定数
        private const string SCENE_MAIN_MENU = "MainMenu";
        private const string SCENE_TEST_SELECTION = "TestSelection";
        private const string SCENE_TEST_TAKING = "TestTaking";
        private const string SCENE_CHAMPION_SELECTION = "ChampionSelection";
        private const string SCENE_POINT_ALLOCATION = "PointAllocation";
        private const string SCENE_BATTLE = "Battle";
        private const string SCENE_RESULTS = "Results";

        /// <summary>
        /// 現在のシーン名
        /// </summary>
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        /// <summary>
        /// シーンロード開始時に発火するイベント
        /// </summary>
        public event Action<string>? SceneLoadStarted;

        /// <summary>
        /// シーンロード完了時に発火するイベント
        /// </summary>
        public event Action<string>? SceneLoadCompleted;

        /// <summary>
        /// シーンをロードします
        /// </summary>
        /// <param name="sceneName">ロードするシーン名</param>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneManagementService] Scene name is null or empty!");
                return;
            }

            Debug.Log($"[SceneManagementService] Loading scene: {sceneName}");
            SceneLoadStarted?.Invoke(sceneName);

            try
            {
                SceneManager.LoadScene(sceneName);
                SceneLoadCompleted?.Invoke(sceneName);
                Debug.Log($"[SceneManagementService] Scene loaded: {sceneName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneManagementService] Failed to load scene '{sceneName}': {ex.Message}");
            }
        }

        /// <summary>
        /// シーンを非同期でロードします
        /// </summary>
        /// <param name="sceneName">ロードするシーン名</param>
        public async Task LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneManagementService] Scene name is null or empty!");
                return;
            }

            Debug.Log($"[SceneManagementService] Loading scene async: {sceneName}");
            SceneLoadStarted?.Invoke(sceneName);

            try
            {
                var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                
                if (asyncOperation == null)
                {
                    Debug.LogError($"[SceneManagementService] Failed to start async load for scene '{sceneName}'");
                    return;
                }

                // ロード完了まで待つ
                while (!asyncOperation.isDone)
                {
                    await Task.Yield();
                }

                SceneLoadCompleted?.Invoke(sceneName);
                Debug.Log($"[SceneManagementService] Scene loaded async: {sceneName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneManagementService] Failed to load scene '{sceneName}' async: {ex.Message}");
            }
        }

        /// <summary>
        /// メインメニューシーンをロードします
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene(SCENE_MAIN_MENU);
        }

        /// <summary>
        /// テスト選択シーンをロードします
        /// </summary>
        public void LoadTestSelection()
        {
            LoadScene(SCENE_TEST_SELECTION);
        }

        /// <summary>
        /// テスト受験シーンをロードします
        /// </summary>
        public void LoadTestTaking()
        {
            LoadScene(SCENE_TEST_TAKING);
        }

        /// <summary>
        /// チャンピオン選択シーンをロードします
        /// </summary>
        public void LoadChampionSelection()
        {
            LoadScene(SCENE_CHAMPION_SELECTION);
        }

        /// <summary>
        /// ポイント配分シーンをロードします
        /// </summary>
        public void LoadPointAllocation()
        {
            LoadScene(SCENE_POINT_ALLOCATION);
        }

        /// <summary>
        /// バトルシーンをロードします
        /// </summary>
        public void LoadBattle()
        {
            LoadScene(SCENE_BATTLE);
        }

        /// <summary>
        /// 結果シーンをロードします
        /// </summary>
        public void LoadResults()
        {
            LoadScene(SCENE_RESULTS);
        }
    }
}
