#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Core.Commands;
using BakaTest.Core.MVVM;
using BakaTest.Data.Champions;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Tests;
using BakaTest.Services.Save;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// メインメニュー画面のViewModel
    /// </summary>
    /// <remarks>
    /// プレイヤーのステータス、所持ポイント、統計情報を表示します。
    /// 各機能への画面遷移を管理します。
    /// </remarks>
    public class MainMenuViewModel : ViewModelBase
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly IChampionService _championService;
        private readonly ITestService _testService;
        private readonly ISaveService _saveService;

        // Player Info
        private string _playerName = "Player";
        private int _playerLevel = 1;
        private int _battleCoins;

        // Test Points
        private int _mathPoints;
        private int _sciencePoints;
        private int _englishPoints;
        private int _historyPoints;

        // Daily Tests
        private int _testsRemaining = 5;
        private int _maxTestsPerDay = 5;

        // Player Stats
        private int _totalTests;
        private int _totalBattles;
        private int _battlesWon;
        private float _winRate;
        private int _championsOwned;
        private int _totalChampions = 20;

        /// <summary>プレイヤー名</summary>
        public string PlayerName
        {
            get => _playerName;
            set => SetProperty(ref _playerName, value);
        }

        /// <summary>プレイヤーレベル</summary>
        public int PlayerLevel
        {
            get => _playerLevel;
            set => SetProperty(ref _playerLevel, value);
        }

        /// <summary>バトルコイン</summary>
        public int BattleCoins
        {
            get => _battleCoins;
            set => SetProperty(ref _battleCoins, value);
        }

        /// <summary>数学ポイント</summary>
        public int MathPoints
        {
            get => _mathPoints;
            set => SetProperty(ref _mathPoints, value);
        }

        /// <summary>理科ポイント</summary>
        public int SciencePoints
        {
            get => _sciencePoints;
            set => SetProperty(ref _sciencePoints, value);
        }

        /// <summary>英語ポイント</summary>
        public int EnglishPoints
        {
            get => _englishPoints;
            set => SetProperty(ref _englishPoints, value);
        }

        /// <summary>歴史ポイント</summary>
        public int HistoryPoints
        {
            get => _historyPoints;
            set => SetProperty(ref _historyPoints, value);
        }

        /// <summary>残りテスト回数</summary>
        public int TestsRemaining
        {
            get => _testsRemaining;
            set => SetProperty(ref _testsRemaining, value);
        }

        /// <summary>最大テスト回数</summary>
        public int MaxTestsPerDay
        {
            get => _maxTestsPerDay;
            set => SetProperty(ref _maxTestsPerDay, value);
        }

        /// <summary>テスト回数表示テキスト</summary>
        public string TestsRemainingText => $"{TestsRemaining}/{MaxTestsPerDay}";

        /// <summary>総テスト受験回数</summary>
        public int TotalTests
        {
            get => _totalTests;
            set => SetProperty(ref _totalTests, value);
        }

        /// <summary>総バトル回数</summary>
        public int TotalBattles
        {
            get => _totalBattles;
            set => SetProperty(ref _totalBattles, value);
        }

        /// <summary>バトル勝利数</summary>
        public int BattlesWon
        {
            get => _battlesWon;
            set
            {
                SetProperty(ref _battlesWon, value);
                UpdateWinRate();
            }
        }

        /// <summary>勝率</summary>
        public float WinRate
        {
            get => _winRate;
            set => SetProperty(ref _winRate, value);
        }

        /// <summary>勝率テキスト</summary>
        public string WinRateText => $"{WinRate:F1}%";

        /// <summary>所有チャンピオン数</summary>
        public int ChampionsOwned
        {
            get => _championsOwned;
            set => SetProperty(ref _championsOwned, value);
        }

        /// <summary>チャンピオン所有テキスト</summary>
        public string ChampionsOwnedText => $"{ChampionsOwned}/{_totalChampions}";

        // Commands
        public RelayCommand<object?> TakeTestCommand { get; }
        public RelayCommand<object?> BattleCommand { get; }
        public RelayCommand<object?> ChampionsCommand { get; }
        public RelayCommand<object?> InventoryCommand { get; }
        public RelayCommand<object?> LeaderboardCommand { get; }
        public RelayCommand<object?> SettingsCommand { get; }
        public RelayCommand<object?> QuitCommand { get; }

        /// <summary>画面遷移が要求されたときに発火</summary>
        public event EventHandler<string>? NavigationRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainMenuViewModel(
            IPlayerDataService playerDataService,
            IChampionService championService,
            ITestService testService,
            ISaveService saveService)
        {
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            _championService = championService ?? throw new ArgumentNullException(nameof(championService));
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            // Commands初期化
            TakeTestCommand = new RelayCommand<object?>(ExecuteTakeTest);
            BattleCommand = new RelayCommand<object?>(ExecuteBattle);
            ChampionsCommand = new RelayCommand<object?>(ExecuteChampions);
            InventoryCommand = new RelayCommand<object?>(ExecuteInventory);
            LeaderboardCommand = new RelayCommand<object?>(ExecuteLeaderboard);
            SettingsCommand = new RelayCommand<object?>(ExecuteSettings);
            QuitCommand = new RelayCommand<object?>(ExecuteQuit);

            // データをロード
            LoadPlayerData();

            // サービスイベントを購読
            SubscribeToServices();
        }

        /// <summary>
        /// プレイヤーデータをロードします
        /// </summary>
        private void LoadPlayerData()
        {
            // プレイヤー基本情報
            var saveData = _saveService.CurrentSaveData;
            if (saveData != null)
            {
                PlayerName = saveData.playerData.playerName;
                PlayerLevel = saveData.playerData.playerLevel;
                BattleCoins = _playerDataService.BattleCoins;
            }

            // テストポイント
            MathPoints = _playerDataService.GetPoints(Subject.Math);
            SciencePoints = _playerDataService.GetPoints(Subject.Science);
            EnglishPoints = _playerDataService.GetPoints(Subject.English);
            HistoryPoints = _playerDataService.GetPoints(Subject.History);

            // デイリーテスト残数（TODO: 実際のロジックを実装）
            TestsRemaining = MaxTestsPerDay;

            // 統計情報
            if (saveData != null)
            {
                TotalTests = saveData.playerData.totalTestsCompleted;
                TotalBattles = saveData.playerData.totalBattles;
                BattlesWon = saveData.playerData.battlesWon;
            }

            // チャンピオン所有数
            ChampionsOwned = _championService.OwnedChampions.Count;

            UpdateWinRate();

            Debug.Log($"[MainMenuViewModel] Player data loaded: {PlayerName}, Level {PlayerLevel}, {BattleCoins} coins");
        }

        /// <summary>
        /// 勝率を更新します
        /// </summary>
        private void UpdateWinRate()
        {
            if (TotalBattles > 0)
            {
                WinRate = (BattlesWon / (float)TotalBattles) * 100f;
            }
            else
            {
                WinRate = 0f;
            }

            OnPropertyChanged(nameof(WinRateText));
        }

        /// <summary>
        /// サービスイベントを購読します
        /// </summary>
        private void SubscribeToServices()
        {
            _playerDataService.PointsChanged += OnPointsChanged;
            _championService.ChampionUnlocked += OnChampionUnlocked;
        }

        /// <summary>
        /// ポイント変更イベントハンドラ
        /// </summary>
        private void OnPointsChanged(Subject subject, int newAmount)
        {
            switch (subject)
            {
                case Subject.Math:
                    MathPoints = newAmount;
                    break;
                case Subject.Science:
                    SciencePoints = newAmount;
                    break;
                case Subject.English:
                    EnglishPoints = newAmount;
                    break;
                case Subject.History:
                    HistoryPoints = newAmount;
                    break;
            }

            Debug.Log($"[MainMenuViewModel] {subject} points updated: {newAmount}");
        }

        /// <summary>
        /// チャンピオンアンロックイベントハンドラ
        /// </summary>
        private void OnChampionUnlocked(Data.Champions.ChampionData champion)
        {
            ChampionsOwned = _championService.OwnedChampions.Count;
            OnPropertyChanged(nameof(ChampionsOwnedText));
            Debug.Log($"[MainMenuViewModel] Champion unlocked: {champion.championName}");
        }

        /// <summary>
        /// データを再読み込みします（画面に戻ってきたときなど）
        /// </summary>
        public void RefreshData()
        {
            LoadPlayerData();
            OnPropertyChanged(nameof(TestsRemainingText));
            OnPropertyChanged(nameof(WinRateText));
            OnPropertyChanged(nameof(ChampionsOwnedText));
        }

        // ===== Command Handlers =====

        private void ExecuteTakeTest(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Take Test button clicked");
            NavigationRequested?.Invoke(this, "TestSelection");
        }

        private void ExecuteBattle(object? parameter)
        {
            // バトルには事前にチャンピオン選択とポイント配分が必要
            Debug.Log("[MainMenuViewModel] Battle button clicked");
            
            // ポイントがあるかチェック
            int totalPoints = MathPoints + SciencePoints + EnglishPoints + HistoryPoints;
            if (totalPoints == 0)
            {
                Debug.LogWarning("[MainMenuViewModel] No points available. Please take a test first.");
                return;
            }

            // チャンピオンを所有しているかチェック
            if (ChampionsOwned == 0)
            {
                Debug.LogWarning("[MainMenuViewModel] No champions owned. Starting with free rotation.");
            }

            NavigationRequested?.Invoke(this, "PointAllocation");
        }

        private void ExecuteChampions(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Champions button clicked");
            NavigationRequested?.Invoke(this, "ChampionSelection");
        }

        private void ExecuteInventory(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Inventory button clicked (not yet implemented)");
            // TODO: インベントリ画面に遷移
        }

        private void ExecuteLeaderboard(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Leaderboard button clicked (not yet implemented)");
            // TODO: リーダーボード画面に遷移
        }

        private void ExecuteSettings(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Settings button clicked (not yet implemented)");
            // TODO: 設定画面に遷移
        }

        private void ExecuteQuit(object? parameter)
        {
            Debug.Log("[MainMenuViewModel] Quit button clicked");
            
            // セーブしてからアプリケーション終了
            _saveService.Save();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _playerDataService.PointsChanged -= OnPointsChanged;
                _championService.ChampionUnlocked -= OnChampionUnlocked;
            }

            base.Dispose(disposing);
        }
    }
}
