#nullable enable
using UnityEngine;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Tests;
using BakaTest.Services.Battle;
using BakaTest.Services.Save;
using BakaTest.Services.AI;
using BakaTest.Services.Inventory;
using BakaTest.Services.Settings;
using BakaTest.Services.Questions;
using BakaTest.Services.Audio;
using BakaTest.UI.LoadingScreen;
using BakaTest.Data.Tests;

namespace BakaTest.Core.Services
{
    /// <summary>
    /// ゲーム起動時の初期化を行うクラス
    /// </summary>
    /// <remarks>
    /// Startupシーンに配置し、すべてのサービスをServiceLocatorに登録します。
    /// </remarks>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private TestConfig? _testConfig;

        [Header("Debug")]
        [SerializeField] private bool _showRegisteredServices = true;

        private SaveService? _saveService;

        private void Awake()
        {
            Debug.Log("[GameBootstrap] Initializing game services...");
            
            // DontDestroyOnLoad設定
            DontDestroyOnLoad(gameObject);
            
            // サービス登録
            RegisterServices();
            
            // 登録確認（デバッグ）
            if (_showRegisteredServices)
            {
                LogRegisteredServices();
            }
            
            Debug.Log("[GameBootstrap] Game services initialized successfully.");
        }

        private void Update()
        {
            // SaveServiceの自動保存更新
            _saveService?.Update(Time.deltaTime);
        }

        /// <summary>
        /// すべてのサービスをServiceLocatorに登録します
        /// </summary>
        private void RegisterServices()
        {
            // セーブサービス（最優先：他のサービスが依存）
            _saveService = new SaveService();
            ServiceLocator.Instance.Register<ISaveService>(_saveService);

            // セーブデータをロードまたは新規作成
            if (_saveService.SaveExists)
            {
                bool loaded = _saveService.Load();
                if (!loaded)
                {
                    Debug.LogWarning("[GameBootstrap] Failed to load save data. Creating new save.");
                    _saveService.CreateNew();
                }
            }
            else
            {
                Debug.Log("[GameBootstrap] No save data found. Creating new save.");
                _saveService.CreateNew();
            }

            // 自動保存を有効化
            _saveService.EnableAutoSave();

            // シーン管理サービス
            var sceneService = new SceneManagementService();
            ServiceLocator.Instance.Register<ISceneManagementService>(sceneService);

            // プレイヤーデータサービス（SaveServiceに依存）
            var playerDataService = new PlayerDataService(_saveService);
            ServiceLocator.Instance.Register<IPlayerDataService>(playerDataService);

            // チャンピオンサービス（SaveServiceに依存）
            var championService = new ChampionService(_saveService);
            ServiceLocator.Instance.Register<IChampionService>(championService);

            // テストサービス（PlayerDataServiceに依存）
            var testService = new TestService(playerDataService);
            ServiceLocator.Instance.Register<ITestService>(testService);

            // バトルサービス
            var battleService = new BattleService();
            ServiceLocator.Instance.Register<IBattleService>(battleService);

            // AI対戦相手サービス
            var aiOpponentService = new AIOpponentService();
            ServiceLocator.Instance.Register<IAIOpponentService>(aiOpponentService);

            // インベントリサービス（SaveServiceとPlayerDataServiceに依存）
            var inventoryService = new InventoryService(_saveService, playerDataService);
            ServiceLocator.Instance.Register<IInventoryService>(inventoryService);

            // 設定サービス（SaveServiceに依存）
            var settingsService = new SettingsService(_saveService);
            ServiceLocator.Instance.Register<ISettingsService>(settingsService);

            // オーディオサービス（SettingsServiceに依存）
            var audioService = new AudioService(settingsService);
            audioService.LoadAllAudioFromResources();
            ServiceLocator.Instance.Register<IAudioService>(audioService);

            // ローディング画面サービス
            var loadingScreenService = new LoadingScreenService();
            ServiceLocator.Instance.Register<ILoadingScreenService>(loadingScreenService);

            // 問題バンクサービス（TestConfigに依存）
            if (_testConfig != null)
            {
                var questionBankService = new QuestionBankService(_testConfig);
                questionBankService.LoadAllQuestionBanks();
                ServiceLocator.Instance.Register<IQuestionBankService>(questionBankService);
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] TestConfig is not assigned. QuestionBankService will not be registered.");
            }
        }

        /// <summary>
        /// 登録されているサービスをログ出力します（デバッグ用）
        /// </summary>
        private void LogRegisteredServices()
        {
            var services = ServiceLocator.Instance.GetRegisteredServiceTypes();
            var count = 0;
            
            Debug.Log("[GameBootstrap] === Registered Services ===");
            foreach (var serviceType in services)
            {
                Debug.Log($"[GameBootstrap]   - {serviceType.Name}");
                count++;
            }
            Debug.Log($"[GameBootstrap] Total: {count} services registered");
        }

        private void OnApplicationQuit()
        {
            // アプリケーション終了時に保存
            if (_saveService != null)
            {
                _saveService.Save();
                Debug.Log("[GameBootstrap] Game saved on application quit.");
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // モバイル対応：バックグラウンドに移行時に保存
            if (pauseStatus && _saveService != null)
            {
                _saveService.Save();
                Debug.Log("[GameBootstrap] Game saved on application pause.");
            }
        }

        private void OnDestroy()
        {
            Debug.Log("[GameBootstrap] Cleaning up services...");
            ServiceLocator.Instance.Clear();
        }
    }
}
