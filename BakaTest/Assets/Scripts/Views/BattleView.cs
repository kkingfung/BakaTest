#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Battle;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Localization;
using BakaTest.Services.Inventory;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;

namespace BakaTest.Views
{
    /// <summary>
    /// バトル画面のView
    /// </summary>
    /// <remarks>
    /// バトルのビジュアル表示、HP更新、バトルログ表示を行います。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class BattleView : ViewBase<BattleViewModel>
    {
        // Header
        private Label? _battleTimerLabel;

        // Player Champion
        private Label? _playerNameLabel;
        private VisualElement? _playerHealthBar;
        private Label? _playerHealthLabel;

        // Opponent Champion
        private Label? _opponentNameLabel;
        private VisualElement? _opponentHealthBar;
        private Label? _opponentHealthLabel;

        // Battle Status
        private Label? _battleStatusLabel;

        // Battle Log
        private ScrollView? _battleLogScroll;

        // Item Slots
        private Button? _itemSlot0;
        private Button? _itemSlot1;
        private Button? _itemSlot2;
        private Button? _itemSlot3;

        // Result Overlay
        private VisualElement? _resultOverlay;
        private Label? _resultTitleLabel;
        private Label? _resultMessageLabel;
        private Label? _rewardCoinsLabel;
        private Label? _rewardXPLabel;
        private Button? _viewDetailsButton;
        private Button? _continueButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var battleService = ServiceLocator.Instance.Get<IBattleService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (battleService == null)
            {
                Debug.LogError("[BattleView] IBattleService not found in ServiceLocator.");
                return;
            }

            if (localizationService == null)
            {
                Debug.LogError("[BattleView] ILocalizationService not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成（インベントリサービスも渡す）
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            var viewModel = new BattleViewModel(battleService, localizationService, inventoryService);
            SetViewModel(viewModel);

            // バトルセットアップを取得（前画面から渡される）
            BattleSetup? setup = battleService.PendingBattleSetup;

            // セットアップがない場合はダミーデータを使用（デバッグ用）
            if (setup == null)
            {
                Debug.LogWarning("[BattleView] No PendingBattleSetup found. Using dummy data for testing.");
                setup = CreateDummyBattleSetup();
            }
            else
            {
                // セットアップを使用したらクリア
                battleService.PendingBattleSetup = null;
            }

            viewModel.InitializeBattle(setup);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(BattleViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.BattleLogAdded += OnBattleLogAdded;
                viewModel.HealthChanged += OnHealthChanged;
                viewModel.BattleCompleted += OnBattleCompleted;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.BattleLogAdded -= OnBattleLogAdded;
                ViewModel.HealthChanged -= OnHealthChanged;
                ViewModel.BattleCompleted -= OnBattleCompleted;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        private void Update()
        {
            // バトルを毎フレーム更新
            ViewModel?.UpdateBattle(Time.deltaTime);
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Header
            _battleTimerLabel = Root.Q<Label>("BattleTimerLabel");

            // Player Champion
            _playerNameLabel = Root.Q<Label>("PlayerNameLabel");
            _playerHealthBar = Root.Q<VisualElement>("PlayerHealthBar");
            _playerHealthLabel = Root.Q<Label>("PlayerHealthLabel");

            // Opponent Champion
            _opponentNameLabel = Root.Q<Label>("OpponentNameLabel");
            _opponentHealthBar = Root.Q<VisualElement>("OpponentHealthBar");
            _opponentHealthLabel = Root.Q<Label>("OpponentHealthLabel");

            // Battle Status
            _battleStatusLabel = Root.Q<Label>("BattleStatusLabel");

            // Battle Log
            _battleLogScroll = Root.Q<ScrollView>("BattleLogScroll");

            // Item Slots
            _itemSlot0 = Root.Q<Button>("ItemSlot0");
            _itemSlot1 = Root.Q<Button>("ItemSlot1");
            _itemSlot2 = Root.Q<Button>("ItemSlot2");
            _itemSlot3 = Root.Q<Button>("ItemSlot3");

            // Result Overlay
            _resultOverlay = Root.Q<VisualElement>("ResultOverlay");
            _resultTitleLabel = Root.Q<Label>("ResultTitleLabel");
            _resultMessageLabel = Root.Q<Label>("ResultMessageLabel");
            _rewardCoinsLabel = Root.Q<Label>("RewardCoinsLabel");
            _rewardXPLabel = Root.Q<Label>("RewardXPLabel");
            _viewDetailsButton = Root.Q<Button>("ViewDetailsButton");
            _continueButton = Root.Q<Button>("ContinueButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Item Slots
            if (_itemSlot0 != null) _itemSlot0.clicked += () => OnItemSlotClicked(0);
            if (_itemSlot1 != null) _itemSlot1.clicked += () => OnItemSlotClicked(1);
            if (_itemSlot2 != null) _itemSlot2.clicked += () => OnItemSlotClicked(2);
            if (_itemSlot3 != null) _itemSlot3.clicked += () => OnItemSlotClicked(3);

            // Result Buttons
            if (_viewDetailsButton != null) _viewDetailsButton.clicked += OnViewDetailsClicked;
            if (_continueButton != null) _continueButton.clicked += OnContinueClicked;
        }

        /// <summary>
        /// イベントハンドラを解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (_itemSlot0 != null) _itemSlot0.clicked -= () => OnItemSlotClicked(0);
            if (_itemSlot1 != null) _itemSlot1.clicked -= () => OnItemSlotClicked(1);
            if (_itemSlot2 != null) _itemSlot2.clicked -= () => OnItemSlotClicked(2);
            if (_itemSlot3 != null) _itemSlot3.clicked -= () => OnItemSlotClicked(3);

            if (_viewDetailsButton != null) _viewDetailsButton.clicked -= OnViewDetailsClicked;
            if (_continueButton != null) _continueButton.clicked -= OnContinueClicked;
        }

        /// <summary>
        /// ViewModelからUIを更新します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            // Player
            if (_playerNameLabel != null) _playerNameLabel.text = ViewModel.PlayerName;
            if (_playerHealthLabel != null) _playerHealthLabel.text = $"{ViewModel.PlayerCurrentHP}/{ViewModel.PlayerMaxHP}";
            UpdateHealthBar(_playerHealthBar, ViewModel.PlayerHealthPercent);

            // Opponent
            if (_opponentNameLabel != null) _opponentNameLabel.text = ViewModel.OpponentName;
            if (_opponentHealthLabel != null) _opponentHealthLabel.text = $"{ViewModel.OpponentCurrentHP}/{ViewModel.OpponentMaxHP}";
            UpdateHealthBar(_opponentHealthBar, ViewModel.OpponentHealthPercent);

            // Battle Timer
            if (_battleTimerLabel != null) _battleTimerLabel.text = ViewModel.BattleTimerText;

            // Battle Status
            if (_battleStatusLabel != null) _battleStatusLabel.text = ViewModel.BattleStatus;

            // Result Overlay
            UpdateResultOverlay();
        }

        /// <summary>
        /// HP バーを更新します
        /// </summary>
        private void UpdateHealthBar(VisualElement? healthBar, float percent)
        {
            if (healthBar == null) return;

            // width を パーセンテージで設定
            healthBar.style.width = Length.Percent(percent * 100f);
        }

        /// <summary>
        /// 結果オーバーレイを更新します
        /// </summary>
        private void UpdateResultOverlay()
        {
            if (_resultOverlay == null || ViewModel == null) return;

            _resultOverlay.style.display = ViewModel.IsResultVisible ? DisplayStyle.Flex : DisplayStyle.None;

            if (_resultTitleLabel != null)
            {
                _resultTitleLabel.text = ViewModel.ResultTitle;

                // 勝敗でクラスを変更
                _resultTitleLabel.RemoveFromClassList("victory");
                _resultTitleLabel.RemoveFromClassList("defeat");
                _resultTitleLabel.AddToClassList(ViewModel.ResultTitle == "VICTORY!" ? "victory" : "defeat");
            }

            if (_resultMessageLabel != null) _resultMessageLabel.text = ViewModel.ResultMessage;
            if (_rewardCoinsLabel != null) _rewardCoinsLabel.text = $"+{ViewModel.RewardCoins} Battle Coins";
            if (_rewardXPLabel != null) _rewardXPLabel.text = $"+{ViewModel.RewardXP} XP";
        }

        /// <summary>
        /// バトルログメッセージを追加します
        /// </summary>
        private void AddBattleLogMessage(string message)
        {
            if (_battleLogScroll == null) return;

            var messageLabel = new Label(message);
            messageLabel.AddToClassList("battle-log-message");

            // メッセージの種類に応じてクラスを追加
            if (message.Contains("CRITICAL") || message.Contains("💥"))
            {
                messageLabel.AddToClassList("critical");
            }
            else if (message.Contains("damage") || message.Contains("attacked"))
            {
                messageLabel.AddToClassList("damage");
            }
            else if (message.Contains("healed") || message.Contains("restored"))
            {
                messageLabel.AddToClassList("heal");
            }
            else if (message.Contains("[SYSTEM]") || message.Contains("⚔") || message.Contains("🎉") || message.Contains("💀"))
            {
                messageLabel.AddToClassList("system");
            }

            _battleLogScroll.Add(messageLabel);

            // 自動スクロール（最新メッセージへ）
            _battleLogScroll.ScrollTo(messageLabel);
        }

        // ===== イベントハンドラ =====

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.PlayerCurrentHP):
                case nameof(ViewModel.PlayerHealthPercent):
                    if (_playerHealthLabel != null)
                        _playerHealthLabel.text = $"{ViewModel.PlayerCurrentHP}/{ViewModel.PlayerMaxHP}";
                    UpdateHealthBar(_playerHealthBar, ViewModel.PlayerHealthPercent);
                    break;

                case nameof(ViewModel.OpponentCurrentHP):
                case nameof(ViewModel.OpponentHealthPercent):
                    if (_opponentHealthLabel != null)
                        _opponentHealthLabel.text = $"{ViewModel.OpponentCurrentHP}/{ViewModel.OpponentMaxHP}";
                    UpdateHealthBar(_opponentHealthBar, ViewModel.OpponentHealthPercent);
                    break;

                case nameof(ViewModel.BattleTimerText):
                    if (_battleTimerLabel != null)
                        _battleTimerLabel.text = ViewModel.BattleTimerText;
                    break;

                case nameof(ViewModel.BattleStatus):
                    if (_battleStatusLabel != null)
                        _battleStatusLabel.text = ViewModel.BattleStatus;
                    break;

                case nameof(ViewModel.IsResultVisible):
                    UpdateResultOverlay();
                    break;
            }
        }

        private void OnBattleLogAdded(object? sender, string message)
        {
            AddBattleLogMessage(message);
        }

        private void OnHealthChanged(object? sender, EventArgs e)
        {
            // HP変更時の追加処理（アニメーションなど）があれば実装
        }

        private void OnBattleCompleted(object? sender, BattleResult result)
        {
            var localization = ServiceLocator.Instance.Get<ILocalizationService>();
            var language = localization?.CurrentLanguage ?? Data.Localization.Language.Japanese;
            Debug.Log($"[BattleView] Battle completed: {result.GetSummary(language)}");
        }

        private void OnItemSlotClicked(int slotIndex)
        {
            ViewModel?.UseItem(slotIndex);
        }

        private void OnViewDetailsClicked()
        {
            // 詳細結果画面に遷移（現在は未実装なのでResultsシーンを使用）
            // TODO: 専用の詳細結果画面が実装されたらそちらに遷移
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService != null)
            {
                Debug.Log("[BattleView] Navigating to Results screen for detailed view.");
                sceneService.LoadResults();
            }
            else
            {
                Debug.LogWarning("[BattleView] SceneManagementService not found.");
            }
        }

        private void OnContinueClicked()
        {
            // バトル終了後、メインメニューに戻る
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService != null)
            {
                Debug.Log("[BattleView] Returning to Main Menu.");
                sceneService.LoadMainMenu();
            }
            else
            {
                Debug.LogWarning("[BattleView] SceneManagementService not found.");
            }
        }

        /// <summary>
        /// ダミーのバトルセットアップを作成します（テスト用）
        /// </summary>
        /// <remarks>
        /// NOTE: この関数はデバッグ/テスト用です。
        /// 実際の運用では、PointAllocationViewがBattleService.PendingBattleSetupに
        /// セットアップを設定してからBattleシーンに遷移します。
        /// </remarks>
        private BattleSetup CreateDummyBattleSetup()
        {
            // ダミーデータでバトルセットアップを作成

            // ダミーチャンピオンデータを作成（実際にはChampionServiceから取得）
            var playerChampion = ScriptableObject.CreateInstance<ChampionData>();
            playerChampion.SetChampionName(Data.Localization.Language.English, "Hero");
            playerChampion.baseStats = new ChampionStats { HP = 1000, Attack = 100, Defense = 50, Speed = 80 };
            playerChampion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.5f,
                ScienceToDefenseRatio = 0.4f,
                EnglishToSpeedRatio = 0.3f,
                HistoryToHPRatio = 2.0f
            };
            playerChampion.criticalChance = 0.15f;
            playerChampion.dodgeChance = 0.1f;

            var opponentChampion = ScriptableObject.CreateInstance<ChampionData>();
            opponentChampion.SetChampionName(Data.Localization.Language.English, "Rival");
            opponentChampion.baseStats = new ChampionStats { HP = 950, Attack = 110, Defense = 45, Speed = 75 };
            opponentChampion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.6f,
                ScienceToDefenseRatio = 0.3f,
                EnglishToSpeedRatio = 0.4f,
                HistoryToHPRatio = 1.8f
            };
            opponentChampion.criticalChance = 0.12f;
            opponentChampion.dodgeChance = 0.08f;

            var playerPoints = new Dictionary<Subject, int>
            {
                { Subject.Math, 100 },
                { Subject.Science, 80 },
                { Subject.English, 60 },
                { Subject.History, 120 }
            };

            var opponentPoints = new Dictionary<Subject, int>
            {
                { Subject.Math, 90 },
                { Subject.Science, 70 },
                { Subject.English, 85 },
                { Subject.History, 110 }
            };

            var setup = new BattleSetup(
                BattleMode.Individual,
                playerChampion,
                "You",
                playerPoints,
                opponentChampion,
                "AI Opponent",
                opponentPoints,
                battleSpeed: 1.0f
            );

            return setup;
        }
    }
}
