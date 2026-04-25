#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Tests;
using BakaTest.Services.Localization;
using BakaTest.ViewModels;
using System.ComponentModel;

namespace BakaTest.Views
{
    /// <summary>
    /// テスト受験画面のView
    /// </summary>
    /// <remarks>
    /// TestTakingViewModelとUI Toolkitを使用してテスト受験画面を表示します。
    /// タイマー更新、問題表示、回答選択、進行管理を担当します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class TestTakingView : ViewBase<TestTakingViewModel>
    {
        // Header elements
        private Label? _subjectLabel;
        private Label? _timerLabel;
        private Label? _questionNumberLabel;
        private VisualElement? _progressBar;
        private VisualElement? _progressFill;

        // Question panel
        private Label? _questionText;

        // Answer buttons
        private Button? _answerAButton;
        private Button? _answerBButton;
        private Button? _answerCButton;
        private Button? _answerDButton;
        private Button? _answerEButton;

        // Answer text labels
        private Label? _answerAText;
        private Label? _answerBText;
        private Label? _answerCText;
        private Label? _answerDText;
        private Label? _answerEText;

        // Navigation buttons
        private Button? _previousButton;
        private Button? _skipButton;
        private Button? _nextButton;

        // Submit panel
        private VisualElement? _submitPanel;
        private Label? _unansweredCountLabel;
        private Button? _reviewButton;
        private Button? _submitButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービス取得
            var testService = ServiceLocator.Instance.Get<ITestService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (testService == null)
            {
                Debug.LogError("[TestTakingView] ITestService not found in ServiceLocator!");
                return;
            }

            if (localizationService == null)
            {
                Debug.LogError("[TestTakingView] ILocalizationService not found in ServiceLocator!");
                return;
            }

            // ViewModelを作成して設定
            SetViewModel(new TestTakingViewModel(testService, localizationService));
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

            Debug.Log("[TestTakingView] UI initialized successfully.");
        }

        protected override void BindViewModel(TestTakingViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            // ViewModelのイベントを購読
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            viewModel.TestSubmitted += OnTestSubmitted;
            viewModel.TestAborted += OnTestAborted;

            Debug.Log("[TestTakingView] ViewModel bound successfully.");
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.TestSubmitted -= OnTestSubmitted;
                ViewModel.TestAborted -= OnTestAborted;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();

            Debug.Log("[TestTakingView] ViewModel unbound.");
        }

        private void Update()
        {
            // タイマーを更新
            if (ViewModel != null && !ViewModel.IsSubmitPanelVisible)
            {
                ViewModel.UpdateTimer(Time.deltaTime);
            }
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Header
            _subjectLabel = Root.Q<Label>("SubjectLabel");
            _timerLabel = Root.Q<Label>("TimerLabel");
            _questionNumberLabel = Root.Q<Label>("QuestionNumberLabel");
            _progressBar = Root.Q<VisualElement>("ProgressBar");
            _progressFill = Root.Q<VisualElement>("ProgressFill");

            // Question
            _questionText = Root.Q<Label>("QuestionText");

            // Answer buttons
            _answerAButton = Root.Q<Button>("AnswerA");
            _answerBButton = Root.Q<Button>("AnswerB");
            _answerCButton = Root.Q<Button>("AnswerC");
            _answerDButton = Root.Q<Button>("AnswerD");
            _answerEButton = Root.Q<Button>("AnswerE");

            // Answer text labels (inside buttons)
            _answerAText = _answerAButton?.Q<Label>("answer-text");
            _answerBText = _answerBButton?.Q<Label>("answer-text");
            _answerCText = _answerCButton?.Q<Label>("answer-text");
            _answerDText = _answerDButton?.Q<Label>("answer-text");
            _answerEText = _answerEButton?.Q<Label>("answer-text");

            // Navigation
            _previousButton = Root.Q<Button>("PreviousButton");
            _skipButton = Root.Q<Button>("SkipButton");
            _nextButton = Root.Q<Button>("NextButton");

            // Submit panel
            _submitPanel = Root.Q<VisualElement>("SubmitPanel");
            _unansweredCountLabel = Root.Q<Label>("UnansweredCountLabel");
            _reviewButton = Root.Q<Button>("ReviewButton");
            _submitButton = Root.Q<Button>("SubmitButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Answer selection
            if (_answerAButton != null)
                _answerAButton.clicked += () => ViewModel?.SelectAnswerACommand.Execute(null);
            if (_answerBButton != null)
                _answerBButton.clicked += () => ViewModel?.SelectAnswerBCommand.Execute(null);
            if (_answerCButton != null)
                _answerCButton.clicked += () => ViewModel?.SelectAnswerCCommand.Execute(null);
            if (_answerDButton != null)
                _answerDButton.clicked += () => ViewModel?.SelectAnswerDCommand.Execute(null);
            if (_answerEButton != null)
                _answerEButton.clicked += () => ViewModel?.SelectAnswerECommand.Execute(null);

            // Navigation
            if (_previousButton != null)
                _previousButton.clicked += () => ViewModel?.PreviousQuestionCommand.Execute(null);
            if (_skipButton != null)
                _skipButton.clicked += () => ViewModel?.SkipQuestionCommand.Execute(null);
            if (_nextButton != null)
                _nextButton.clicked += () => ViewModel?.NextQuestionCommand.Execute(null);

            // Submit panel
            // TODO: Implement ReviewAnswersCommand in TestTakingViewModel
            // if (_reviewButton != null)
            //     _reviewButton.clicked += () => ViewModel?.ReviewAnswersCommand.Execute(null);
            if (_submitButton != null)
                _submitButton.clicked += () => ViewModel?.SubmitTestCommand.Execute(null);
        }

        /// <summary>
        /// イベントハンドラの登録を解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            // Answer buttons
            if (_answerAButton != null)
                _answerAButton.clicked -= () => ViewModel?.SelectAnswerACommand.Execute(null);
            if (_answerBButton != null)
                _answerBButton.clicked -= () => ViewModel?.SelectAnswerBCommand.Execute(null);
            if (_answerCButton != null)
                _answerCButton.clicked -= () => ViewModel?.SelectAnswerCCommand.Execute(null);
            if (_answerDButton != null)
                _answerDButton.clicked -= () => ViewModel?.SelectAnswerDCommand.Execute(null);
            if (_answerEButton != null)
                _answerEButton.clicked -= () => ViewModel?.SelectAnswerECommand.Execute(null);

            // Navigation
            if (_previousButton != null)
                _previousButton.clicked -= () => ViewModel?.PreviousQuestionCommand.Execute(null);
            if (_skipButton != null)
                _skipButton.clicked -= () => ViewModel?.SkipQuestionCommand.Execute(null);
            if (_nextButton != null)
                _nextButton.clicked -= () => ViewModel?.NextQuestionCommand.Execute(null);

            // Submit panel
            // TODO: Implement ReviewAnswersCommand in TestTakingViewModel
            // if (_reviewButton != null)
            //     _reviewButton.clicked -= () => ViewModel?.ReviewAnswersCommand.Execute(null);
            if (_submitButton != null)
                _submitButton.clicked -= () => ViewModel?.SubmitTestCommand.Execute(null);
        }

        /// <summary>
        /// ViewModelの値でUIを初期化します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            UpdateHeaderInfo();
            UpdateQuestionContent();
            UpdateAnswerButtons();
            UpdateNavigationButtons();
            UpdateSubmitPanel();
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
                case nameof(ViewModel.QuestionNumberText):
                    UpdateHeaderInfo();
                    break;

                case nameof(ViewModel.TimerText):
                case nameof(ViewModel.TimeRemaining):
                    UpdateTimer();
                    break;

                case nameof(ViewModel.ProgressPercent):
                    UpdateProgressBar();
                    break;

                case nameof(ViewModel.QuestionText):
                    UpdateQuestionContent();
                    break;

                case nameof(ViewModel.IsAnswerASelected):
                case nameof(ViewModel.IsAnswerBSelected):
                case nameof(ViewModel.IsAnswerCSelected):
                case nameof(ViewModel.IsAnswerDSelected):
                case nameof(ViewModel.IsAnswerESelected):
                    UpdateAnswerButtons();
                    break;

                case nameof(ViewModel.CanGoPrevious):
                case nameof(ViewModel.CanGoNext):
                    UpdateNavigationButtons();
                    break;

                case nameof(ViewModel.IsSubmitPanelVisible):
                case nameof(ViewModel.UnansweredCount):
                    UpdateSubmitPanel();
                    break;
            }
        }

        /// <summary>
        /// ヘッダー情報を更新します
        /// </summary>
        private void UpdateHeaderInfo()
        {
            if (ViewModel == null) return;

            if (_subjectLabel != null)
                _subjectLabel.text = ViewModel.SubjectText;

            if (_questionNumberLabel != null)
                _questionNumberLabel.text = ViewModel.QuestionNumberText;
        }

        /// <summary>
        /// タイマー表示を更新します
        /// </summary>
        private void UpdateTimer()
        {
            if (ViewModel == null || _timerLabel == null) return;

            _timerLabel.text = ViewModel.TimerText;

            // タイマーの色を変更（残り時間に応じて）
            _timerLabel.RemoveFromClassList("warning");
            _timerLabel.RemoveFromClassList("danger");

            if (ViewModel.TimeRemaining <= 60f) // 1分以下
            {
                _timerLabel.AddToClassList("danger");
            }
            else if (ViewModel.TimeRemaining <= 120f) // 2分以下
            {
                _timerLabel.AddToClassList("warning");
            }
        }

        /// <summary>
        /// プログレスバーを更新します
        /// </summary>
        private void UpdateProgressBar()
        {
            if (ViewModel == null || _progressFill == null) return;

            _progressFill.style.width = Length.Percent(ViewModel.ProgressPercent);
        }

        /// <summary>
        /// 問題内容を更新します
        /// </summary>
        private void UpdateQuestionContent()
        {
            if (ViewModel == null) return;

            if (_questionText != null)
                _questionText.text = ViewModel.QuestionText;

            // 回答選択肢のテキストを更新
            var choices = ViewModel.AnswerChoices;
            if (_answerAText != null && choices.Length > 0)
                _answerAText.text = choices[0];
            if (_answerBText != null && choices.Length > 1)
                _answerBText.text = choices[1];
            if (_answerCText != null && choices.Length > 2)
                _answerCText.text = choices[2];
            if (_answerDText != null && choices.Length > 3)
                _answerDText.text = choices[3];
            if (_answerEText != null && choices.Length > 4)
                _answerEText.text = choices[4];
        }

        /// <summary>
        /// 回答ボタンの状態を更新します
        /// </summary>
        private void UpdateAnswerButtons()
        {
            if (ViewModel == null) return;

            // すべてのselectedクラスを削除
            _answerAButton?.RemoveFromClassList("selected");
            _answerBButton?.RemoveFromClassList("selected");
            _answerCButton?.RemoveFromClassList("selected");
            _answerDButton?.RemoveFromClassList("selected");
            _answerEButton?.RemoveFromClassList("selected");

            // 選択されているボタンにselectedクラスを追加
            if (ViewModel.IsAnswerASelected)
                _answerAButton?.AddToClassList("selected");
            if (ViewModel.IsAnswerBSelected)
                _answerBButton?.AddToClassList("selected");
            if (ViewModel.IsAnswerCSelected)
                _answerCButton?.AddToClassList("selected");
            if (ViewModel.IsAnswerDSelected)
                _answerDButton?.AddToClassList("selected");
            if (ViewModel.IsAnswerESelected)
                _answerEButton?.AddToClassList("selected");
        }

        /// <summary>
        /// ナビゲーションボタンの状態を更新します
        /// </summary>
        private void UpdateNavigationButtons()
        {
            if (ViewModel == null) return;

            if (_previousButton != null)
                _previousButton.SetEnabled(ViewModel.CanGoPrevious);

            if (_nextButton != null)
                _nextButton.SetEnabled(ViewModel.CanGoNext);
        }

        /// <summary>
        /// 提出パネルの表示を更新します
        /// </summary>
        private void UpdateSubmitPanel()
        {
            if (ViewModel == null || _submitPanel == null) return;

            _submitPanel.style.display = ViewModel.IsSubmitPanelVisible
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (_unansweredCountLabel != null && ViewModel.IsSubmitPanelVisible)
            {
                _unansweredCountLabel.text = ViewModel.UnansweredCount > 0
                    ? $"⚠ {ViewModel.UnansweredCount} unanswered questions"
                    : "✓ All questions answered";
            }
        }

        /// <summary>
        /// テスト提出時の処理
        /// </summary>
        private void OnTestSubmitted(object? sender, System.EventArgs e)
        {
            Debug.Log("[TestTakingView] Test submitted. Loading results screen...");

            // 結果画面に遷移（実装予定）
            // var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            // sceneService?.LoadResults();
        }

        /// <summary>
        /// テスト中断時の処理
        /// </summary>
        private void OnTestAborted(object? sender, System.EventArgs e)
        {
            Debug.Log("[TestTakingView] Test aborted. Returning to menu...");

            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            sceneService?.LoadMainMenu();
        }
    }
}
