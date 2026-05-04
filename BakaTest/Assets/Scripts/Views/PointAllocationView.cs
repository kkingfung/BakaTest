#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using BakaTest.Services.AI;
using BakaTest.Services.Battle;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;
using BakaTest.Services.Player;
using BakaTest.Services.SceneManagement;
using BakaTest.ViewModels;

namespace BakaTest.Views
{
    /// <summary>
    /// ポイント配分画面のView
    /// </summary>
    /// <remarks>
    /// PointAllocationViewModelとUI Toolkitを使用してポイント配分画面を表示します。
    /// スライダー操作、ステータスプレビュー、配分確定を担当します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class PointAllocationView : ViewBase<PointAllocationViewModel>
    {
        // Available points labels
        private Label? _mathAvailableLabel;
        private Label? _scienceAvailableLabel;
        private Label? _englishAvailableLabel;
        private Label? _historyAvailableLabel;

        // Allocation sliders
        private Slider? _mathSlider;
        private Slider? _scienceSlider;
        private Slider? _englishSlider;
        private Slider? _historySlider;

        // Allocated points labels
        private Label? _mathAllocatedLabel;
        private Label? _scienceAllocatedLabel;
        private Label? _englishAllocatedLabel;
        private Label? _historyAllocatedLabel;

        // Preview labels
        private Label? _mathPreviewLabel;
        private Label? _sciencePreviewLabel;
        private Label? _englishPreviewLabel;
        private Label? _historyPreviewLabel;

        // Total
        private Label? _totalAllocatedLabel;

        // Champion info
        private Label? _championNameLabel;
        private Label? _championRoleLabel;

        // Base stats
        private Label? _baseHPLabel;
        private Label? _baseAttackLabel;
        private Label? _baseDefenseLabel;
        private Label? _baseSpeedLabel;

        // Final stats
        private Label? _finalHPLabel;
        private Label? _finalAttackLabel;
        private Label? _finalDefenseLabel;
        private Label? _finalSpeedLabel;

        // Bonus stats
        private Label? _bonusHPLabel;
        private Label? _bonusAttackLabel;
        private Label? _bonusDefenseLabel;
        private Label? _bonusSpeedLabel;

        // Affinity labels
        private Label? _mathAffinityLabel;
        private Label? _scienceAffinityLabel;
        private Label? _englishAffinityLabel;
        private Label? _historyAffinityLabel;

        // Action buttons
        private Button? _resetButton;
        private Button? _confirmButton;
        private Button? _backButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービス取得
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (playerDataService == null)
            {
                Debug.LogError("[PointAllocationView] IPlayerDataService not found in ServiceLocator!");
                return;
            }

            if (championService == null)
            {
                Debug.LogError("[PointAllocationView] IChampionService not found in ServiceLocator!");
                return;
            }

            if (localizationService == null)
            {
                Debug.LogError("[PointAllocationView] ILocalizationService not found in ServiceLocator!");
                return;
            }

            // ViewModelを作成して設定
            SetViewModel(new PointAllocationViewModel(playerDataService, championService, localizationService));
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            // UI要素を取得
            GetUIElements();

            // ViewModelの値でUIを初期化
            UpdateUIFromViewModel();

            // イベントハンドラを登録
            RegisterEventHandlers();

            Debug.Log("[PointAllocationView] UI initialized successfully.");
        }

        protected override void BindViewModel(PointAllocationViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            // ViewModelのイベントを購読
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            viewModel.AllocationConfirmed += OnAllocationConfirmed;
            viewModel.BackRequested += OnBackRequested;

            Debug.Log("[PointAllocationView] ViewModel bound successfully.");
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.AllocationConfirmed -= OnAllocationConfirmed;
                ViewModel.BackRequested -= OnBackRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();

            Debug.Log("[PointAllocationView] ViewModel unbound.");
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Available points
            _mathAvailableLabel = Root.Q<Label>("MathAvailableLabel");
            _scienceAvailableLabel = Root.Q<Label>("ScienceAvailableLabel");
            _englishAvailableLabel = Root.Q<Label>("EnglishAvailableLabel");
            _historyAvailableLabel = Root.Q<Label>("HistoryAvailableLabel");

            // Sliders
            _mathSlider = Root.Q<Slider>("MathSlider");
            _scienceSlider = Root.Q<Slider>("ScienceSlider");
            _englishSlider = Root.Q<Slider>("EnglishSlider");
            _historySlider = Root.Q<Slider>("HistorySlider");

            // Allocated labels
            _mathAllocatedLabel = Root.Q<Label>("MathAllocatedLabel");
            _scienceAllocatedLabel = Root.Q<Label>("ScienceAllocatedLabel");
            _englishAllocatedLabel = Root.Q<Label>("EnglishAllocatedLabel");
            _historyAllocatedLabel = Root.Q<Label>("HistoryAllocatedLabel");

            // Preview labels
            _mathPreviewLabel = Root.Q<Label>("MathPreviewLabel");
            _sciencePreviewLabel = Root.Q<Label>("SciencePreviewLabel");
            _englishPreviewLabel = Root.Q<Label>("EnglishPreviewLabel");
            _historyPreviewLabel = Root.Q<Label>("HistoryPreviewLabel");

            // Total
            _totalAllocatedLabel = Root.Q<Label>("TotalAllocatedLabel");

            // Champion info
            _championNameLabel = Root.Q<Label>("ChampionNameLabel");
            _championRoleLabel = Root.Q<Label>("ChampionRoleLabel");

            // Base stats
            _baseHPLabel = Root.Q<Label>("BaseHPLabel");
            _baseAttackLabel = Root.Q<Label>("BaseAttackLabel");
            _baseDefenseLabel = Root.Q<Label>("BaseDefenseLabel");
            _baseSpeedLabel = Root.Q<Label>("BaseSpeedLabel");

            // Final stats
            _finalHPLabel = Root.Q<Label>("FinalHPLabel");
            _finalAttackLabel = Root.Q<Label>("FinalAttackLabel");
            _finalDefenseLabel = Root.Q<Label>("FinalDefenseLabel");
            _finalSpeedLabel = Root.Q<Label>("FinalSpeedLabel");

            // Bonus stats
            _bonusHPLabel = Root.Q<Label>("BonusHPLabel");
            _bonusAttackLabel = Root.Q<Label>("BonusAttackLabel");
            _bonusDefenseLabel = Root.Q<Label>("BonusDefenseLabel");
            _bonusSpeedLabel = Root.Q<Label>("BonusSpeedLabel");

            // Affinity
            _mathAffinityLabel = Root.Q<Label>("MathAffinityLabel");
            _scienceAffinityLabel = Root.Q<Label>("ScienceAffinityLabel");
            _englishAffinityLabel = Root.Q<Label>("EnglishAffinityLabel");
            _historyAffinityLabel = Root.Q<Label>("HistoryAffinityLabel");

            // Buttons
            _resetButton = Root.Q<Button>("ResetButton");
            _confirmButton = Root.Q<Button>("ConfirmButton");
            _backButton = Root.Q<Button>("BackButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Sliders
            if (_mathSlider != null)
                _mathSlider.RegisterValueChangedCallback(evt => OnMathSliderChanged(evt.newValue));
            if (_scienceSlider != null)
                _scienceSlider.RegisterValueChangedCallback(evt => OnScienceSliderChanged(evt.newValue));
            if (_englishSlider != null)
                _englishSlider.RegisterValueChangedCallback(evt => OnEnglishSliderChanged(evt.newValue));
            if (_historySlider != null)
                _historySlider.RegisterValueChangedCallback(evt => OnHistorySliderChanged(evt.newValue));

            // Buttons
            if (_resetButton != null)
                _resetButton.clicked += () => ViewModel?.ResetCommand.Execute(null);
            if (_confirmButton != null)
                _confirmButton.clicked += () => ViewModel?.ConfirmCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked += () => ViewModel?.BackCommand.Execute(null);
        }

        /// <summary>
        /// イベントハンドラの登録を解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            // Sliders
            if (_mathSlider != null)
                _mathSlider.UnregisterValueChangedCallback(evt => OnMathSliderChanged(evt.newValue));
            if (_scienceSlider != null)
                _scienceSlider.UnregisterValueChangedCallback(evt => OnScienceSliderChanged(evt.newValue));
            if (_englishSlider != null)
                _englishSlider.UnregisterValueChangedCallback(evt => OnEnglishSliderChanged(evt.newValue));
            if (_historySlider != null)
                _historySlider.UnregisterValueChangedCallback(evt => OnHistorySliderChanged(evt.newValue));

            // Buttons
            if (_resetButton != null)
                _resetButton.clicked -= () => ViewModel?.ResetCommand.Execute(null);
            if (_confirmButton != null)
                _confirmButton.clicked -= () => ViewModel?.ConfirmCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked -= () => ViewModel?.BackCommand.Execute(null);
        }

        /// <summary>
        /// ViewModelの値でUIを初期化します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            UpdateAvailablePoints();
            UpdateAllocatedPoints();
            UpdateChampionInfo();
            UpdateStats();
            UpdateAffinity();
        }

        /// <summary>
        /// ViewModelのプロパティ変更時の処理
        /// </summary>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.MathAvailable):
                case nameof(ViewModel.ScienceAvailable):
                case nameof(ViewModel.EnglishAvailable):
                case nameof(ViewModel.HistoryAvailable):
                    UpdateAvailablePoints();
                    break;

                case nameof(ViewModel.MathAllocated):
                case nameof(ViewModel.ScienceAllocated):
                case nameof(ViewModel.EnglishAllocated):
                case nameof(ViewModel.HistoryAllocated):
                case nameof(ViewModel.TotalAllocated):
                    UpdateAllocatedPoints();
                    break;

                case nameof(ViewModel.ChampionName):
                case nameof(ViewModel.ChampionRole):
                    UpdateChampionInfo();
                    break;

                case nameof(ViewModel.BaseHP):
                case nameof(ViewModel.BaseAttack):
                case nameof(ViewModel.BaseDefense):
                case nameof(ViewModel.BaseSpeed):
                case nameof(ViewModel.BonusHP):
                case nameof(ViewModel.BonusAttack):
                case nameof(ViewModel.BonusDefense):
                case nameof(ViewModel.BonusSpeed):
                case nameof(ViewModel.FinalHP):
                case nameof(ViewModel.FinalAttack):
                case nameof(ViewModel.FinalDefense):
                case nameof(ViewModel.FinalSpeed):
                case nameof(ViewModel.MathPreviewText):
                case nameof(ViewModel.SciencePreviewText):
                case nameof(ViewModel.EnglishPreviewText):
                case nameof(ViewModel.HistoryPreviewText):
                    UpdateStats();
                    break;

                case nameof(ViewModel.MathAffinityText):
                case nameof(ViewModel.ScienceAffinityText):
                case nameof(ViewModel.EnglishAffinityText):
                case nameof(ViewModel.HistoryAffinityText):
                    UpdateAffinity();
                    break;
            }
        }

        /// <summary>
        /// 利用可能ポイントを更新します
        /// </summary>
        private void UpdateAvailablePoints()
        {
            if (ViewModel == null) return;

            if (_mathAvailableLabel != null)
                _mathAvailableLabel.text = ViewModel.MathAvailable.ToString();
            if (_scienceAvailableLabel != null)
                _scienceAvailableLabel.text = ViewModel.ScienceAvailable.ToString();
            if (_englishAvailableLabel != null)
                _englishAvailableLabel.text = ViewModel.EnglishAvailable.ToString();
            if (_historyAvailableLabel != null)
                _historyAvailableLabel.text = ViewModel.HistoryAvailable.ToString();

            // スライダーの最大値を更新
            if (_mathSlider != null)
                _mathSlider.highValue = ViewModel.MathAvailable;
            if (_scienceSlider != null)
                _scienceSlider.highValue = ViewModel.ScienceAvailable;
            if (_englishSlider != null)
                _englishSlider.highValue = ViewModel.EnglishAvailable;
            if (_historySlider != null)
                _historySlider.highValue = ViewModel.HistoryAvailable;
        }

        /// <summary>
        /// 配分済みポイントを更新します
        /// </summary>
        private void UpdateAllocatedPoints()
        {
            if (ViewModel == null) return;

            if (_mathAllocatedLabel != null)
                _mathAllocatedLabel.text = ViewModel.MathAllocated.ToString();
            if (_scienceAllocatedLabel != null)
                _scienceAllocatedLabel.text = ViewModel.ScienceAllocated.ToString();
            if (_englishAllocatedLabel != null)
                _englishAllocatedLabel.text = ViewModel.EnglishAllocated.ToString();
            if (_historyAllocatedLabel != null)
                _historyAllocatedLabel.text = ViewModel.HistoryAllocated.ToString();

            if (_totalAllocatedLabel != null)
                _totalAllocatedLabel.text = ViewModel.TotalAllocatedText;

            // スライダーの値を更新（循環参照を避けるため、値が違う場合のみ）
            if (_mathSlider != null && Mathf.Abs(_mathSlider.value - ViewModel.MathAllocated) > 0.1f)
                _mathSlider.value = ViewModel.MathAllocated;
            if (_scienceSlider != null && Mathf.Abs(_scienceSlider.value - ViewModel.ScienceAllocated) > 0.1f)
                _scienceSlider.value = ViewModel.ScienceAllocated;
            if (_englishSlider != null && Mathf.Abs(_englishSlider.value - ViewModel.EnglishAllocated) > 0.1f)
                _englishSlider.value = ViewModel.EnglishAllocated;
            if (_historySlider != null && Mathf.Abs(_historySlider.value - ViewModel.HistoryAllocated) > 0.1f)
                _historySlider.value = ViewModel.HistoryAllocated;
        }

        /// <summary>
        /// チャンピオン情報を更新します
        /// </summary>
        private void UpdateChampionInfo()
        {
            if (ViewModel == null) return;

            if (_championNameLabel != null)
                _championNameLabel.text = ViewModel.ChampionName;
            if (_championRoleLabel != null)
                _championRoleLabel.text = ViewModel.ChampionRole;
        }

        /// <summary>
        /// ステータスを更新します
        /// </summary>
        private void UpdateStats()
        {
            if (ViewModel == null) return;

            // Base stats
            if (_baseHPLabel != null)
                _baseHPLabel.text = ViewModel.BaseHP.ToString();
            if (_baseAttackLabel != null)
                _baseAttackLabel.text = ViewModel.BaseAttack.ToString();
            if (_baseDefenseLabel != null)
                _baseDefenseLabel.text = ViewModel.BaseDefense.ToString();
            if (_baseSpeedLabel != null)
                _baseSpeedLabel.text = ViewModel.BaseSpeed.ToString();

            // Final stats
            if (_finalHPLabel != null)
                _finalHPLabel.text = ViewModel.FinalHP.ToString();
            if (_finalAttackLabel != null)
                _finalAttackLabel.text = ViewModel.FinalAttack.ToString();
            if (_finalDefenseLabel != null)
                _finalDefenseLabel.text = ViewModel.FinalDefense.ToString();
            if (_finalSpeedLabel != null)
                _finalSpeedLabel.text = ViewModel.FinalSpeed.ToString();

            // Bonus stats
            if (_bonusHPLabel != null)
                _bonusHPLabel.text = $"(+{ViewModel.BonusHP})";
            if (_bonusAttackLabel != null)
                _bonusAttackLabel.text = $"(+{ViewModel.BonusAttack})";
            if (_bonusDefenseLabel != null)
                _bonusDefenseLabel.text = $"(+{ViewModel.BonusDefense})";
            if (_bonusSpeedLabel != null)
                _bonusSpeedLabel.text = $"(+{ViewModel.BonusSpeed})";

            // Preview labels
            if (_mathPreviewLabel != null)
                _mathPreviewLabel.text = ViewModel.MathPreviewText;
            if (_sciencePreviewLabel != null)
                _sciencePreviewLabel.text = ViewModel.SciencePreviewText;
            if (_englishPreviewLabel != null)
                _englishPreviewLabel.text = ViewModel.EnglishPreviewText;
            if (_historyPreviewLabel != null)
                _historyPreviewLabel.text = ViewModel.HistoryPreviewText;
        }

        /// <summary>
        /// 親和性情報を更新します
        /// </summary>
        private void UpdateAffinity()
        {
            if (ViewModel == null) return;

            if (_mathAffinityLabel != null)
                _mathAffinityLabel.text = ViewModel.MathAffinityText;
            if (_scienceAffinityLabel != null)
                _scienceAffinityLabel.text = ViewModel.ScienceAffinityText;
            if (_englishAffinityLabel != null)
                _englishAffinityLabel.text = ViewModel.EnglishAffinityText;
            if (_historyAffinityLabel != null)
                _historyAffinityLabel.text = ViewModel.HistoryAffinityText;
        }

        /// <summary>
        /// Mathスライダー変更時
        /// </summary>
        private void OnMathSliderChanged(float newValue)
        {
            if (ViewModel != null)
                ViewModel.MathAllocated = (int)newValue;
        }

        /// <summary>
        /// Scienceスライダー変更時
        /// </summary>
        private void OnScienceSliderChanged(float newValue)
        {
            if (ViewModel != null)
                ViewModel.ScienceAllocated = (int)newValue;
        }

        /// <summary>
        /// Englishスライダー変更時
        /// </summary>
        private void OnEnglishSliderChanged(float newValue)
        {
            if (ViewModel != null)
                ViewModel.EnglishAllocated = (int)newValue;
        }

        /// <summary>
        /// Historyスライダー変更時
        /// </summary>
        private void OnHistorySliderChanged(float newValue)
        {
            if (ViewModel != null)
                ViewModel.HistoryAllocated = (int)newValue;
        }

        /// <summary>
        /// 配分確定時の処理
        /// </summary>
        private void OnAllocationConfirmed(object? sender, System.EventArgs e)
        {
            if (ViewModel == null || ViewModel.SelectedChampion == null)
            {
                Debug.LogError("[PointAllocationView] Cannot create battle setup: ViewModel or Champion is null.");
                return;
            }

            Debug.Log("[PointAllocationView] Allocation confirmed. Creating battle setup...");

            // サービス取得
            var battleService = ServiceLocator.Instance.Get<IBattleService>();
            var aiService = ServiceLocator.Instance.Get<IAIOpponentService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();

            if (battleService == null || aiService == null || sceneService == null)
            {
                Debug.LogError("[PointAllocationView] Required services not found.");
                return;
            }

            // プレイヤーの配分ポイントを取得
            var playerPoints = new Dictionary<Subject, int>
            {
                { Subject.Math, ViewModel.MathAllocated },
                { Subject.Science, ViewModel.ScienceAllocated },
                { Subject.English, ViewModel.EnglishAllocated },
                { Subject.History, ViewModel.HistoryAllocated }
            };

            // AIの難易度とチャンピオン選択
            var championService = ServiceLocator.Instance.Get<IChampionService>();
            var availableChampions = championService?.AvailableChampions ?? new List<ChampionData>();
            var aiChampion = aiService.SelectChampion(availableChampions, AIDifficulty.Medium);

            if (aiChampion == null && availableChampions.Count > 0)
            {
                // フォールバック: ランダムに選択
                aiChampion = availableChampions[UnityEngine.Random.Range(0, availableChampions.Count)];
            }

            // AI対戦相手のポイント配分を生成
            var aiPoints = aiService.AllocatePoints(AIDifficulty.Medium, AIPersonality.Tactical, aiChampion!);
            var aiName = aiService.GenerateAIName(AIDifficulty.Medium);

            // BattleSetupを作成
            var battleSetup = new BattleSetup(
                mode: BattleMode.Individual,
                player1Champion: ViewModel.SelectedChampion,
                player1Name: "Player",
                player1Points: playerPoints,
                player2Champion: aiChampion!,
                player2Name: aiName,
                player2Points: aiPoints
            );

            // BattleServiceに設定
            battleService.PendingBattleSetup = battleSetup;

            Debug.Log($"[PointAllocationView] Battle setup created. Player: {battleSetup.Player1Name} vs Opponent: {battleSetup.Player2Name}");

            // Battleシーンに遷移
            sceneService.LoadBattle();
        }

        /// <summary>
        /// 戻る要求時の処理
        /// </summary>
        private void OnBackRequested(object? sender, System.EventArgs e)
        {
            Debug.Log("[PointAllocationView] Back requested. Returning to menu...");

            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            sceneService?.LoadMainMenu();
        }
    }
}
