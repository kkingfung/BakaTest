#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Player;
using BakaTest.Services.SceneManagement;
using BakaTest.Data.Tests;
using BakaTest.Data.Battle;

namespace BakaTest.Views
{
    /// <summary>
    /// 結果画面のView
    /// </summary>
    /// <remarks>
    /// ResultsViewModelとUI Toolkitを接続します。
    /// テスト結果とバトル結果の両方を表示できます。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class ResultsView : ViewBase<ResultsViewModel>
    {
        // 共通UI要素
        private Label? _titleLabel;
        private Label? _resultSummaryLabel;
        private VisualElement? _victoryIndicator;
        private VisualElement? _defeatIndicator;

        // テスト結果セクション
        private VisualElement? _testResultSection;
        private Label? _subjectLabel;
        private Label? _difficultyLabel;
        private Label? _scoreLabel;
        private Label? _accuracyLabel;
        private Label? _pointsEarnedLabel;
        private Label? _timeLabel;
        private Label? _dailyCapWarningLabel;

        // バトル結果セクション
        private VisualElement? _battleResultSection;
        private Label? _winnerLabel;
        private Label? _loserLabel;
        private Label? _turnCountLabel;
        private Label? _durationLabel;
        private Label? _coinsEarnedLabel;

        // ボタン
        private Button? _continueButton;
        private Button? _backToMenuButton;
        private Button? _playAgainButton;

        protected override void Awake()
        {
            base.Awake();

            // サービス取得
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();

            if (playerDataService == null)
            {
                Debug.LogError("[ResultsView] PlayerDataService not found!");
                return;
            }

            // ViewModelを作成して設定
            SetViewModel(new ResultsViewModel(playerDataService));
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            // UI要素を取得
            GetUIElements();

            // イベントハンドラを登録
            RegisterEventHandlers();

            // ViewModelの初期値でUIを更新
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(ResultsViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.ContinueRequested += OnContinueRequested;
                viewModel.BackToMenuRequested += OnBackToMenuRequested;
                viewModel.PlayAgainRequested += OnPlayAgainRequested;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.ContinueRequested -= OnContinueRequested;
                ViewModel.BackToMenuRequested -= OnBackToMenuRequested;
                ViewModel.PlayAgainRequested -= OnPlayAgainRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        private void GetUIElements()
        {
            if (Root == null) return;

            // 共通要素
            _titleLabel = Q<Label>("TitleLabel");
            _resultSummaryLabel = Q<Label>("ResultSummaryLabel");
            _victoryIndicator = Q<VisualElement>("VictoryIndicator");
            _defeatIndicator = Q<VisualElement>("DefeatIndicator");

            // テスト結果セクション
            _testResultSection = Q<VisualElement>("TestResultSection");
            _subjectLabel = Q<Label>("SubjectLabel");
            _difficultyLabel = Q<Label>("DifficultyLabel");
            _scoreLabel = Q<Label>("ScoreLabel");
            _accuracyLabel = Q<Label>("AccuracyLabel");
            _pointsEarnedLabel = Q<Label>("PointsEarnedLabel");
            _timeLabel = Q<Label>("TimeLabel");
            _dailyCapWarningLabel = Q<Label>("DailyCapWarningLabel");

            // バトル結果セクション
            _battleResultSection = Q<VisualElement>("BattleResultSection");
            _winnerLabel = Q<Label>("WinnerLabel");
            _loserLabel = Q<Label>("LoserLabel");
            _turnCountLabel = Q<Label>("TurnCountLabel");
            _durationLabel = Q<Label>("DurationLabel");
            _coinsEarnedLabel = Q<Label>("CoinsEarnedLabel");

            // ボタン
            _continueButton = Q<Button>("ContinueButton");
            _backToMenuButton = Q<Button>("BackToMenuButton");
            _playAgainButton = Q<Button>("PlayAgainButton");
        }

        private void RegisterEventHandlers()
        {
            if (_continueButton != null)
            {
                _continueButton.clicked += OnContinueClicked;
            }

            if (_backToMenuButton != null)
            {
                _backToMenuButton.clicked += OnBackToMenuClicked;
            }

            if (_playAgainButton != null)
            {
                _playAgainButton.clicked += OnPlayAgainClicked;
            }
        }

        private void UnregisterEventHandlers()
        {
            if (_continueButton != null)
            {
                _continueButton.clicked -= OnContinueClicked;
            }

            if (_backToMenuButton != null)
            {
                _backToMenuButton.clicked -= OnBackToMenuClicked;
            }

            if (_playAgainButton != null)
            {
                _playAgainButton.clicked -= OnPlayAgainClicked;
            }
        }

        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            UpdateCommonElements();
            UpdateResultTypeSection();
        }

        private void UpdateCommonElements()
        {
            if (ViewModel == null) return;

            if (_titleLabel != null)
                _titleLabel.text = ViewModel.TitleText;

            if (_resultSummaryLabel != null)
                _resultSummaryLabel.text = ViewModel.ResultSummary;

            // 勝敗インジケーター
            if (_victoryIndicator != null)
                _victoryIndicator.style.display = ViewModel.IsVictory ? DisplayStyle.Flex : DisplayStyle.None;

            if (_defeatIndicator != null)
                _defeatIndicator.style.display = ViewModel.IsVictory ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void UpdateResultTypeSection()
        {
            if (ViewModel == null) return;

            // 結果タイプに応じてセクションを表示/非表示
            if (ViewModel.ResultType == ResultType.Test)
            {
                ShowTestResultSection();
                HideBattleResultSection();
                UpdateTestResults();
            }
            else if (ViewModel.ResultType == ResultType.Battle)
            {
                HideTestResultSection();
                ShowBattleResultSection();
                UpdateBattleResults();
            }
        }

        private void ShowTestResultSection()
        {
            if (_testResultSection != null)
                _testResultSection.style.display = DisplayStyle.Flex;
        }

        private void HideTestResultSection()
        {
            if (_testResultSection != null)
                _testResultSection.style.display = DisplayStyle.None;
        }

        private void ShowBattleResultSection()
        {
            if (_battleResultSection != null)
                _battleResultSection.style.display = DisplayStyle.Flex;
        }

        private void HideBattleResultSection()
        {
            if (_battleResultSection != null)
                _battleResultSection.style.display = DisplayStyle.None;
        }

        private void UpdateTestResults()
        {
            if (ViewModel == null) return;

            if (_subjectLabel != null)
                _subjectLabel.text = ViewModel.SubjectText;

            if (_difficultyLabel != null)
                _difficultyLabel.text = ViewModel.DifficultyText;

            if (_scoreLabel != null)
                _scoreLabel.text = ViewModel.ScoreText;

            if (_accuracyLabel != null)
                _accuracyLabel.text = ViewModel.AccuracyText;

            if (_pointsEarnedLabel != null)
                _pointsEarnedLabel.text = ViewModel.PointsEarnedText;

            if (_timeLabel != null)
                _timeLabel.text = ViewModel.TimeText;

            if (_dailyCapWarningLabel != null)
            {
                _dailyCapWarningLabel.text = ViewModel.DailyCapWarning;
                _dailyCapWarningLabel.style.display = string.IsNullOrEmpty(ViewModel.DailyCapWarning)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
            }
        }

        private void UpdateBattleResults()
        {
            if (ViewModel == null) return;

            if (_winnerLabel != null)
                _winnerLabel.text = ViewModel.WinnerText;

            if (_loserLabel != null)
                _loserLabel.text = ViewModel.LoserText;

            if (_turnCountLabel != null)
                _turnCountLabel.text = ViewModel.TurnCountText;

            if (_durationLabel != null)
                _durationLabel.text = ViewModel.DurationText;

            if (_coinsEarnedLabel != null)
                _coinsEarnedLabel.text = ViewModel.CoinsEarnedText;
        }

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.TitleText):
                case nameof(ViewModel.ResultSummary):
                case nameof(ViewModel.IsVictory):
                    UpdateCommonElements();
                    break;

                case nameof(ViewModel.ResultType):
                case nameof(ViewModel.TestResult):
                case nameof(ViewModel.BattleResult):
                    UpdateResultTypeSection();
                    break;

                case nameof(ViewModel.SubjectText):
                case nameof(ViewModel.DifficultyText):
                case nameof(ViewModel.ScoreText):
                case nameof(ViewModel.AccuracyText):
                case nameof(ViewModel.PointsEarnedText):
                case nameof(ViewModel.TimeText):
                case nameof(ViewModel.DailyCapWarning):
                    UpdateTestResults();
                    break;

                case nameof(ViewModel.WinnerText):
                case nameof(ViewModel.LoserText):
                case nameof(ViewModel.TurnCountText):
                case nameof(ViewModel.DurationText):
                case nameof(ViewModel.CoinsEarnedText):
                    UpdateBattleResults();
                    break;
            }
        }

        private void OnContinueClicked()
        {
            ViewModel?.ContinueCommand.Execute(null);
        }

        private void OnBackToMenuClicked()
        {
            ViewModel?.BackToMenuCommand.Execute(null);
        }

        private void OnPlayAgainClicked()
        {
            ViewModel?.PlayAgainCommand.Execute(null);
        }

        private void OnContinueRequested(object? sender, System.EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[ResultsView] SceneManagementService not found!");
                return;
            }

            Debug.Log("[ResultsView] Continue to next screen...");

            // 結果タイプに応じて次の画面へ
            if (ViewModel?.ResultType == ResultType.Test)
            {
                // テスト結果 → ポイント配分画面へ
                Debug.Log("[ResultsView] Navigate to Point Allocation screen");
                sceneService.LoadPointAllocation();
            }
            else if (ViewModel?.ResultType == ResultType.Battle)
            {
                // バトル結果 → メインメニューへ
                Debug.Log("[ResultsView] Navigate to Main Menu");
                sceneService.LoadMainMenu();
            }
        }

        private void OnBackToMenuRequested(object? sender, System.EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[ResultsView] SceneManagementService not found!");
                return;
            }

            Debug.Log("[ResultsView] Back to main menu...");
            sceneService.LoadMainMenu();
        }

        private void OnPlayAgainRequested(object? sender, System.EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[ResultsView] SceneManagementService not found!");
                return;
            }

            Debug.Log("[ResultsView] Play again...");

            // 結果タイプに応じて戻る画面を変更
            if (ViewModel?.ResultType == ResultType.Test)
            {
                // テスト結果 → テスト選択画面へ
                Debug.Log("[ResultsView] Navigate to Test Selection screen");
                sceneService.LoadTestSelection();
            }
            else if (ViewModel?.ResultType == ResultType.Battle)
            {
                // バトル結果 → ポイント配分画面へ（再戦）
                Debug.Log("[ResultsView] Navigate to Point Allocation screen");
                sceneService.LoadPointAllocation();
            }
        }

        /// <summary>
        /// 外部からテスト結果を設定します
        /// </summary>
        public void SetTestResult(TestResult result)
        {
            ViewModel?.SetTestResult(result);
        }

        /// <summary>
        /// 外部からバトル結果を設定します
        /// </summary>
        public void SetBattleResult(BattleResult result)
        {
            ViewModel?.SetBattleResult(result);
        }
    }
}
