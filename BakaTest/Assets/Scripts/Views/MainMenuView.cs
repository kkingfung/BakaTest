#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Tests;
using BakaTest.Services.Save;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Localization;

namespace BakaTest.Views
{
    /// <summary>
    /// メインメニュー画面のView
    /// </summary>
    /// <remarks>
    /// プレイヤー情報、所持ポイント、統計情報を表示します。
    /// 各機能への画面遷移ボタンを提供します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : ViewBase<MainMenuViewModel>
    {
        // Player Info
        private Label? _playerNameLabel;
        private Label? _battleCoinsLabel;

        // Test Points
        private Label? _mathPointsLabel;
        private Label? _sciencePointsLabel;
        private Label? _englishPointsLabel;
        private Label? _historyPointsLabel;

        // Daily Tests
        private Label? _testsRemainingLabel;

        // Player Stats
        private Label? _playerLevelLabel;
        private Label? _totalTestsLabel;
        private Label? _totalBattlesLabel;
        private Label? _winRateLabel;
        private Label? _championsOwnedLabel;

        // Buttons
        private Button? _takeTestButton;
        private Button? _battleButton;
        private Button? _championsButton;
        private Button? _inventoryButton;
        private Button? _leaderboardButton;
        private Button? _settingsButton;
        private Button? _quitButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();
            var testService = ServiceLocator.Instance.Get<ITestService>();
            var saveService = ServiceLocator.Instance.Get<ISaveService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (playerDataService == null || championService == null || testService == null || saveService == null || localizationService == null)
            {
                Debug.LogError("[MainMenuView] Required services not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成
            var viewModel = new MainMenuViewModel(playerDataService, championService, testService, saveService, localizationService);
            SetViewModel(viewModel);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(MainMenuViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.NavigationRequested += OnNavigationRequested;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.NavigationRequested -= OnNavigationRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        private void OnEnable()
        {
            // 画面がアクティブになったらデータを再読み込み
            ViewModel?.RefreshData();
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Player Info
            _playerNameLabel = Root.Q<Label>("PlayerNameLabel");
            _battleCoinsLabel = Root.Q<Label>("BattleCoinsLabel");

            // Test Points
            _mathPointsLabel = Root.Q<Label>("MathPointsLabel");
            _sciencePointsLabel = Root.Q<Label>("SciencePointsLabel");
            _englishPointsLabel = Root.Q<Label>("EnglishPointsLabel");
            _historyPointsLabel = Root.Q<Label>("HistoryPointsLabel");

            // Daily Tests
            _testsRemainingLabel = Root.Q<Label>("TestsRemainingLabel");

            // Player Stats
            _playerLevelLabel = Root.Q<Label>("PlayerLevelLabel");
            _totalTestsLabel = Root.Q<Label>("TotalTestsLabel");
            _totalBattlesLabel = Root.Q<Label>("TotalBattlesLabel");
            _winRateLabel = Root.Q<Label>("WinRateLabel");
            _championsOwnedLabel = Root.Q<Label>("ChampionsOwnedLabel");

            // Buttons
            _takeTestButton = Root.Q<Button>("TakeTestButton");
            _battleButton = Root.Q<Button>("BattleButton");
            _championsButton = Root.Q<Button>("ChampionsButton");
            _inventoryButton = Root.Q<Button>("InventoryButton");
            _leaderboardButton = Root.Q<Button>("LeaderboardButton");
            _settingsButton = Root.Q<Button>("SettingsButton");
            _quitButton = Root.Q<Button>("QuitButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (_takeTestButton != null)
                _takeTestButton.clicked += OnTakeTestClicked;

            if (_battleButton != null)
                _battleButton.clicked += OnBattleClicked;

            if (_championsButton != null)
                _championsButton.clicked += OnChampionsClicked;

            if (_inventoryButton != null)
                _inventoryButton.clicked += OnInventoryClicked;

            if (_leaderboardButton != null)
                _leaderboardButton.clicked += OnLeaderboardClicked;

            if (_settingsButton != null)
                _settingsButton.clicked += OnSettingsClicked;

            if (_quitButton != null)
                _quitButton.clicked += OnQuitClicked;
        }

        /// <summary>
        /// イベントハンドラを解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (_takeTestButton != null)
                _takeTestButton.clicked -= OnTakeTestClicked;

            if (_battleButton != null)
                _battleButton.clicked -= OnBattleClicked;

            if (_championsButton != null)
                _championsButton.clicked -= OnChampionsClicked;

            if (_inventoryButton != null)
                _inventoryButton.clicked -= OnInventoryClicked;

            if (_leaderboardButton != null)
                _leaderboardButton.clicked -= OnLeaderboardClicked;

            if (_settingsButton != null)
                _settingsButton.clicked -= OnSettingsClicked;

            if (_quitButton != null)
                _quitButton.clicked -= OnQuitClicked;
        }

        /// <summary>
        /// ViewModelからUIを更新します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            // Player Info
            if (_playerNameLabel != null) _playerNameLabel.text = ViewModel.PlayerName;
            if (_battleCoinsLabel != null) _battleCoinsLabel.text = ViewModel.BattleCoins.ToString();

            // Test Points
            if (_mathPointsLabel != null) _mathPointsLabel.text = ViewModel.MathPoints.ToString();
            if (_sciencePointsLabel != null) _sciencePointsLabel.text = ViewModel.SciencePoints.ToString();
            if (_englishPointsLabel != null) _englishPointsLabel.text = ViewModel.EnglishPoints.ToString();
            if (_historyPointsLabel != null) _historyPointsLabel.text = ViewModel.HistoryPoints.ToString();

            // Daily Tests
            if (_testsRemainingLabel != null) _testsRemainingLabel.text = ViewModel.TestsRemainingText;

            // Player Stats
            if (_playerLevelLabel != null) _playerLevelLabel.text = ViewModel.PlayerLevel.ToString();
            if (_totalTestsLabel != null) _totalTestsLabel.text = ViewModel.TotalTests.ToString();
            if (_totalBattlesLabel != null) _totalBattlesLabel.text = ViewModel.TotalBattles.ToString();
            if (_winRateLabel != null) _winRateLabel.text = ViewModel.WinRateText;
            if (_championsOwnedLabel != null) _championsOwnedLabel.text = ViewModel.ChampionsOwnedText;
        }

        // ===== Event Handlers =====

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.PlayerName):
                    if (_playerNameLabel != null) _playerNameLabel.text = ViewModel.PlayerName;
                    break;

                case nameof(ViewModel.BattleCoins):
                    if (_battleCoinsLabel != null) _battleCoinsLabel.text = ViewModel.BattleCoins.ToString();
                    break;

                case nameof(ViewModel.MathPoints):
                    if (_mathPointsLabel != null) _mathPointsLabel.text = ViewModel.MathPoints.ToString();
                    break;

                case nameof(ViewModel.SciencePoints):
                    if (_sciencePointsLabel != null) _sciencePointsLabel.text = ViewModel.SciencePoints.ToString();
                    break;

                case nameof(ViewModel.EnglishPoints):
                    if (_englishPointsLabel != null) _englishPointsLabel.text = ViewModel.EnglishPoints.ToString();
                    break;

                case nameof(ViewModel.HistoryPoints):
                    if (_historyPointsLabel != null) _historyPointsLabel.text = ViewModel.HistoryPoints.ToString();
                    break;

                case nameof(ViewModel.TestsRemaining):
                    if (_testsRemainingLabel != null) _testsRemainingLabel.text = ViewModel.TestsRemainingText;
                    break;

                case nameof(ViewModel.TotalTests):
                    if (_totalTestsLabel != null) _totalTestsLabel.text = ViewModel.TotalTests.ToString();
                    break;

                case nameof(ViewModel.TotalBattles):
                    if (_totalBattlesLabel != null) _totalBattlesLabel.text = ViewModel.TotalBattles.ToString();
                    break;

                case nameof(ViewModel.WinRate):
                    if (_winRateLabel != null) _winRateLabel.text = ViewModel.WinRateText;
                    break;

                case nameof(ViewModel.ChampionsOwned):
                    if (_championsOwnedLabel != null) _championsOwnedLabel.text = ViewModel.ChampionsOwnedText;
                    break;
            }
        }

        private void OnNavigationRequested(object? sender, string destination)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[MainMenuView] ISceneManagementService not found.");
                return;
            }

            switch (destination)
            {
                case "TestSelection":
                    // sceneService.LoadTestSelection();
                    Debug.Log("[MainMenuView] Navigate to Test Selection (commented out)");
                    break;

                case "PointAllocation":
                    // sceneService.LoadPointAllocation();
                    Debug.Log("[MainMenuView] Navigate to Point Allocation (commented out)");
                    break;

                case "ChampionSelection":
                    // sceneService.LoadChampionSelection();
                    Debug.Log("[MainMenuView] Navigate to Champion Selection (commented out)");
                    break;

                default:
                    Debug.LogWarning($"[MainMenuView] Unknown navigation destination: {destination}");
                    break;
            }
        }

        private void OnTakeTestClicked()
        {
            ViewModel?.TakeTestCommand.Execute(null);
        }

        private void OnBattleClicked()
        {
            ViewModel?.BattleCommand.Execute(null);
        }

        private void OnChampionsClicked()
        {
            ViewModel?.ChampionsCommand.Execute(null);
        }

        private void OnInventoryClicked()
        {
            ViewModel?.InventoryCommand.Execute(null);
        }

        private void OnLeaderboardClicked()
        {
            ViewModel?.LeaderboardCommand.Execute(null);
        }

        private void OnSettingsClicked()
        {
            ViewModel?.SettingsCommand.Execute(null);
        }

        private void OnQuitClicked()
        {
            ViewModel?.QuitCommand.Execute(null);
        }
    }
}
