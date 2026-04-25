#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Tests;
using BakaTest.Services.Player;
using BakaTest.Services.Localization;
using BakaTest.ViewModels;
using BakaTest.Data.Tests;
using System.ComponentModel;

namespace BakaTest.Views
{
    /// <summary>
    /// テスト結果画面のView
    /// </summary>
    /// <remarks>
    /// TestResultsViewModelとUI Toolkitを使用してテスト結果画面を表示します。
    /// スコア、進捗、問題詳細の表示を担当します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class TestResultsView : ViewBase<TestResultsViewModel>
    {
        private ILocalizationService? _localizationService;

        // Header
        private Label? _titleLabel;
        private Label? _subjectLabel;

        // Score panel
        private Label? _scorePercentLabel;
        private Label? _gradeLabel;
        private Label? _correctAnswersLabel;
        private Label? _timeTakenLabel;
        private Label? _pointsEarnedLabel;

        // Daily progress
        private VisualElement? _progressBar;
        private VisualElement? _progressFill;
        private Label? _dailyProgressLabel;
        private Label? _remainingPointsLabel;

        // Breakdown
        private Button? _toggleBreakdownButton;
        private Label? _breakdownToggleLabel;
        private ScrollView? _questionBreakdownList;

        // Action buttons
        private Button? _continueButton;
        private Button? _retryButton;
        private Button? _backToMenuButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービス取得
            var testService = ServiceLocator.Instance.Get<ITestService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (testService == null)
            {
                Debug.LogError("[TestResultsView] ITestService not found in ServiceLocator!");
                return;
            }

            if (playerDataService == null)
            {
                Debug.LogError("[TestResultsView] IPlayerDataService not found in ServiceLocator!");
                return;
            }

            if (localizationService == null)
            {
                Debug.LogError("[TestResultsView] ILocalizationService not found in ServiceLocator!");
                return;
            }

            // LocalizationServiceを保存
            _localizationService = localizationService;

            // ViewModelを作成して設定
            SetViewModel(new TestResultsViewModel(testService, playerDataService));
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

            Debug.Log("[TestResultsView] UI initialized successfully.");
        }

        protected override void BindViewModel(TestResultsViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            // ViewModelのイベントを購読
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            viewModel.ContinueRequested += OnContinueRequested;
            viewModel.RetryTestRequested += OnRetryTestRequested;
            viewModel.BackToMenuRequested += OnBackToMenuRequested;

            Debug.Log("[TestResultsView] ViewModel bound successfully.");
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.ContinueRequested -= OnContinueRequested;
                ViewModel.RetryTestRequested -= OnRetryTestRequested;
                ViewModel.BackToMenuRequested -= OnBackToMenuRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();

            Debug.Log("[TestResultsView] ViewModel unbound.");
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Header
            _titleLabel = Root.Q<Label>("TitleLabel");
            _subjectLabel = Root.Q<Label>("SubjectLabel");

            // Score panel
            _scorePercentLabel = Root.Q<Label>("ScorePercentLabel");
            _gradeLabel = Root.Q<Label>("GradeLabel");
            _correctAnswersLabel = Root.Q<Label>("CorrectAnswersLabel");
            _timeTakenLabel = Root.Q<Label>("TimeTakenLabel");
            _pointsEarnedLabel = Root.Q<Label>("PointsEarnedLabel");

            // Daily progress
            _progressBar = Root.Q<VisualElement>("ProgressBar");
            _progressFill = Root.Q<VisualElement>("ProgressFill");
            _dailyProgressLabel = Root.Q<Label>("DailyProgressLabel");
            _remainingPointsLabel = Root.Q<Label>("RemainingPointsLabel");

            // Breakdown
            _toggleBreakdownButton = Root.Q<Button>("ToggleBreakdownButton");
            _breakdownToggleLabel = _toggleBreakdownButton?.Q<Label>("BreakdownToggleLabel");
            _questionBreakdownList = Root.Q<ScrollView>("QuestionBreakdownList");

            // Action buttons
            _continueButton = Root.Q<Button>("ContinueButton");
            _retryButton = Root.Q<Button>("RetryButton");
            _backToMenuButton = Root.Q<Button>("BackToMenuButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (_toggleBreakdownButton != null)
                _toggleBreakdownButton.clicked += () => ViewModel?.ToggleBreakdownCommand.Execute(null);

            if (_continueButton != null)
                _continueButton.clicked += () => ViewModel?.ContinueCommand.Execute(null);

            if (_retryButton != null)
                _retryButton.clicked += () => ViewModel?.RetryTestCommand.Execute(null);

            if (_backToMenuButton != null)
                _backToMenuButton.clicked += () => ViewModel?.BackToMenuCommand.Execute(null);
        }

        /// <summary>
        /// イベントハンドラの登録を解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (_toggleBreakdownButton != null)
                _toggleBreakdownButton.clicked -= () => ViewModel?.ToggleBreakdownCommand.Execute(null);

            if (_continueButton != null)
                _continueButton.clicked -= () => ViewModel?.ContinueCommand.Execute(null);

            if (_retryButton != null)
                _retryButton.clicked -= () => ViewModel?.RetryTestCommand.Execute(null);

            if (_backToMenuButton != null)
                _backToMenuButton.clicked -= () => ViewModel?.BackToMenuCommand.Execute(null);
        }

        /// <summary>
        /// ViewModelの値でUIを初期化します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            UpdateHeader();
            UpdateScorePanel();
            UpdateDailyProgress();
            UpdateBreakdownToggle();
        }

        /// <summary>
        /// ViewModelのプロパティ変更時の処理
        /// </summary>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.SubjectText):
                    UpdateHeader();
                    break;

                case nameof(ViewModel.ScorePercentText):
                case nameof(ViewModel.GradeText):
                case nameof(ViewModel.CorrectAnswersText):
                case nameof(ViewModel.TimeTakenText):
                case nameof(ViewModel.PointsEarnedText):
                case nameof(ViewModel.ScoreLevel):
                    UpdateScorePanel();
                    break;

                case nameof(ViewModel.DailyProgressText):
                case nameof(ViewModel.RemainingPointsText):
                case nameof(ViewModel.DailyProgressPercent):
                    UpdateDailyProgress();
                    break;

                case nameof(ViewModel.IsBreakdownExpanded):
                case nameof(ViewModel.BreakdownToggleText):
                    UpdateBreakdownToggle();
                    break;

                case nameof(ViewModel.QuestionResults):
                    RebuildQuestionBreakdown();
                    break;
            }
        }

        /// <summary>
        /// ヘッダー情報を更新します
        /// </summary>
        private void UpdateHeader()
        {
            if (ViewModel == null) return;

            if (_subjectLabel != null)
                _subjectLabel.text = ViewModel.SubjectText;
        }

        /// <summary>
        /// スコアパネルを更新します
        /// </summary>
        private void UpdateScorePanel()
        {
            if (ViewModel == null) return;

            if (_scorePercentLabel != null)
            {
                _scorePercentLabel.text = ViewModel.ScorePercentText;

                // CSSクラスを更新
                _scorePercentLabel.RemoveFromClassList("low");
                _scorePercentLabel.RemoveFromClassList("medium");
                _scorePercentLabel.RemoveFromClassList("high");
                _scorePercentLabel.AddToClassList(ViewModel.ScoreLevel);
            }

            if (_gradeLabel != null)
                _gradeLabel.text = ViewModel.GradeText;

            if (_correctAnswersLabel != null)
                _correctAnswersLabel.text = ViewModel.CorrectAnswersText;

            if (_timeTakenLabel != null)
                _timeTakenLabel.text = ViewModel.TimeTakenText;

            if (_pointsEarnedLabel != null)
                _pointsEarnedLabel.text = ViewModel.PointsEarnedText;
        }

        /// <summary>
        /// 本日の進捗を更新します
        /// </summary>
        private void UpdateDailyProgress()
        {
            if (ViewModel == null) return;

            if (_dailyProgressLabel != null)
                _dailyProgressLabel.text = ViewModel.DailyProgressText;

            if (_remainingPointsLabel != null)
                _remainingPointsLabel.text = ViewModel.RemainingPointsText;

            if (_progressFill != null)
                _progressFill.style.width = Length.Percent(ViewModel.DailyProgressPercent);
        }

        /// <summary>
        /// 詳細表示トグルを更新します
        /// </summary>
        private void UpdateBreakdownToggle()
        {
            if (ViewModel == null) return;

            if (_breakdownToggleLabel != null)
                _breakdownToggleLabel.text = ViewModel.BreakdownToggleText;

            if (_questionBreakdownList != null)
            {
                _questionBreakdownList.style.display = ViewModel.IsBreakdownExpanded
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }

            // 初回展開時に問題リストを生成
            if (ViewModel.IsBreakdownExpanded && _questionBreakdownList != null && _questionBreakdownList.childCount == 0)
            {
                RebuildQuestionBreakdown();
            }
        }

        /// <summary>
        /// 問題詳細リストを再構築します
        /// </summary>
        private void RebuildQuestionBreakdown()
        {
            if (ViewModel == null || _questionBreakdownList == null) return;

            // 既存のアイテムをクリア
            _questionBreakdownList.Clear();

            // 各問題の結果を追加
            foreach (var result in ViewModel.QuestionResults)
            {
                var questionItem = CreateQuestionItem(result);
                _questionBreakdownList.Add(questionItem);
            }
        }

        /// <summary>
        /// 問題アイテムのVisualElementを作成します
        /// </summary>
        private VisualElement CreateQuestionItem(QuestionResult result)
        {
            var container = new VisualElement();
            container.AddToClassList("question-item");
            container.AddToClassList(result.isCorrect ? "correct" : "incorrect");

            // 問題番号
            var numberLabel = new Label((result.questionIndex + 1).ToString());
            numberLabel.AddToClassList("question-number");
            container.Add(numberLabel);

            // ステータス（✓ or ✗）
            var statusLabel = new Label(result.isCorrect ? "✓" : "✗");
            statusLabel.AddToClassList("question-status");
            container.Add(statusLabel);

            // 問題文
            var questionTextStr = result.question != null && _localizationService != null
                ? result.question.GetQuestionText(_localizationService.CurrentLanguage)
                : "Unknown question";
            var questionText = new Label(questionTextStr);
            questionText.AddToClassList("question-text");
            container.Add(questionText);

            // 回答
            string answerText = result.wasSkipped
                ? "Skipped"
                : $"Your answer: {GetChoiceLetter(result.playerAnswer)}";
            var answerLabel = new Label(answerText);
            answerLabel.AddToClassList("question-answer");
            container.Add(answerLabel);

            return container;
        }

        /// <summary>
        /// 回答インデックスを文字に変換します
        /// </summary>
        private string GetChoiceLetter(int index)
        {
            return index switch
            {
                0 => "A",
                1 => "B",
                2 => "C",
                3 => "D",
                4 => "E",
                _ => "?"
            };
        }

        /// <summary>
        /// 続行要求時の処理
        /// </summary>
        private void OnContinueRequested(object? sender, System.EventArgs e)
        {
            Debug.Log("[TestResultsView] Continue requested. Loading next screen...");

            // ポイント配分画面に遷移（実装予定）
            // var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            // sceneService?.LoadPointAllocation();
        }

        /// <summary>
        /// 再受験要求時の処理
        /// </summary>
        private void OnRetryTestRequested(object? sender, System.EventArgs e)
        {
            Debug.Log("[TestResultsView] Retry requested. Loading test selection...");

            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            sceneService?.LoadTestSelection();
        }

        /// <summary>
        /// メニューに戻る要求時の処理
        /// </summary>
        private void OnBackToMenuRequested(object? sender, System.EventArgs e)
        {
            Debug.Log("[TestResultsView] Back to menu requested.");

            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            sceneService?.LoadMainMenu();
        }
    }
}
