#nullable enable
using System;
using System.ComponentModel;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using BakaTest.Services.Tests;
using BakaTest.Services.Localization;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// テスト受験画面のViewModel
    /// </summary>
    /// <remarks>
    /// テストの進行、回答の管理、タイマーの制御を行います。
    /// </remarks>
    public class TestTakingViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly ILocalizationService _localization;
        private TestAttempt? _currentAttempt;

        // 現在の状態
        private int _currentQuestionIndex;
        private float _timeRemaining;
        private bool _isSubmitPanelVisible;

        // 表示データ
        private string _subjectText = string.Empty;
        private string _questionNumberText = string.Empty;
        private string _timerText = string.Empty;
        private float _progressPercent;
        private int _unansweredCount;
        private string _questionText = string.Empty;
        private string[] _answerChoices = new string[5];
        private int _selectedAnswerIndex = -1;

        /// <summary>現在のテストセッション</summary>
        public TestAttempt? CurrentAttempt
        {
            get => _currentAttempt;
            private set => SetProperty(ref _currentAttempt, value);
        }

        /// <summary>現在の問題インデックス (0-based)</summary>
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            private set
            {
                if (SetProperty(ref _currentQuestionIndex, value))
                {
                    UpdateCurrentQuestion();
                }
            }
        }

        /// <summary>残り時間（秒）</summary>
        public float TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (SetProperty(ref _timeRemaining, value))
                {
                    UpdateTimerDisplay();
                    
                    // タイムアップ
                    if (_timeRemaining <= 0)
                    {
                        OnTimeUp();
                    }
                }
            }
        }

        /// <summary>提出確認パネルの表示状態</summary>
        public bool IsSubmitPanelVisible
        {
            get => _isSubmitPanelVisible;
            set => SetProperty(ref _isSubmitPanelVisible, value);
        }

        // 表示プロパティ
        public string SubjectText { get => _subjectText; private set => SetProperty(ref _subjectText, value); }
        public string QuestionNumberText { get => _questionNumberText; private set => SetProperty(ref _questionNumberText, value); }
        public string TimerText { get => _timerText; private set => SetProperty(ref _timerText, value); }
        public float ProgressPercent { get => _progressPercent; private set => SetProperty(ref _progressPercent, value); }
        public int UnansweredCount { get => _unansweredCount; private set => SetProperty(ref _unansweredCount, value); }
        public string QuestionText { get => _questionText; private set => SetProperty(ref _questionText, value); }
        public string[] AnswerChoices => _answerChoices;
        public string AnswerAText => _answerChoices[0];
        public string AnswerBText => _answerChoices[1];
        public string AnswerCText => _answerChoices[2];
        public string AnswerDText => _answerChoices[3];
        public string AnswerEText => _answerChoices[4];

        public int SelectedAnswerIndex
        {
            get => _selectedAnswerIndex;
            private set => SetProperty(ref _selectedAnswerIndex, value);
        }

        // 選択状態（View用）
        public bool IsAnswerASelected => _selectedAnswerIndex == 0;
        public bool IsAnswerBSelected => _selectedAnswerIndex == 1;
        public bool IsAnswerCSelected => _selectedAnswerIndex == 2;
        public bool IsAnswerDSelected => _selectedAnswerIndex == 3;
        public bool IsAnswerESelected => _selectedAnswerIndex == 4;

        // ナビゲーション状態
        public bool CanGoPrevious => _currentQuestionIndex > 0;
        public bool CanGoNext => _currentAttempt != null && _currentQuestionIndex < _currentAttempt.questions.Count - 1;
        public bool IsOnLastQuestion => _currentAttempt != null && _currentQuestionIndex == _currentAttempt.questions.Count - 1;

        // Commands
        public RelayCommand<object?> SelectAnswerACommand { get; }
        public RelayCommand<object?> SelectAnswerBCommand { get; }
        public RelayCommand<object?> SelectAnswerCCommand { get; }
        public RelayCommand<object?> SelectAnswerDCommand { get; }
        public RelayCommand<object?> SelectAnswerECommand { get; }
        public RelayCommand<object?> PreviousQuestionCommand { get; }
        public RelayCommand<object?> NextQuestionCommand { get; }
        public RelayCommand<object?> SkipQuestionCommand { get; }
        public RelayCommand<object?> ShowSubmitPanelCommand { get; }
        public RelayCommand<object?> HideSubmitPanelCommand { get; }
        public RelayCommand<object?> SubmitTestCommand { get; }

        // Events
        public event EventHandler? TestSubmitted;

        #pragma warning disable CS0067 // Event is declared but never used
        public event EventHandler? TestAborted;
        #pragma warning restore CS0067

        public TestTakingViewModel(ITestService testService, ILocalizationService localization)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // 初期化
            for (int i = 0; i < _answerChoices.Length; i++)
            {
                _answerChoices[i] = string.Empty;
            }

            // Commands
            SelectAnswerACommand = new RelayCommand<object?>(_ => SelectAnswer(0));
            SelectAnswerBCommand = new RelayCommand<object?>(_ => SelectAnswer(1));
            SelectAnswerCCommand = new RelayCommand<object?>(_ => SelectAnswer(2));
            SelectAnswerDCommand = new RelayCommand<object?>(_ => SelectAnswer(3));
            SelectAnswerECommand = new RelayCommand<object?>(_ => SelectAnswer(4));

            PreviousQuestionCommand = new RelayCommand<object?>(ExecutePreviousQuestion, _ => CanGoPrevious);
            NextQuestionCommand = new RelayCommand<object?>(ExecuteNextQuestion, _ => CanGoNext);
            SkipQuestionCommand = new RelayCommand<object?>(ExecuteSkipQuestion);

            ShowSubmitPanelCommand = new RelayCommand<object?>(ExecuteShowSubmitPanel);
            HideSubmitPanelCommand = new RelayCommand<object?>(ExecuteHideSubmitPanel);
            SubmitTestCommand = new RelayCommand<object?>(ExecuteSubmitTest);

            // 現在のテストを取得
            LoadCurrentTest();

            Debug.Log("[TestTakingViewModel] Initialized.");
        }

        /// <summary>
        /// 現在進行中のテストをロードします
        /// </summary>
        private void LoadCurrentTest()
        {
            CurrentAttempt = _testService.CurrentAttempt;

            if (CurrentAttempt == null)
            {
                Debug.LogError("[TestTakingViewModel] No test in progress!");
                return;
            }

            // 残り時間を計算
            float totalTime = _testService.Config.timeLimit;
            float elapsed = (float)(DateTime.Now - CurrentAttempt.startTime).TotalSeconds;
            TimeRemaining = Mathf.Max(0, totalTime - elapsed);

            // 最初の問題を表示
            CurrentQuestionIndex = 0;

            Debug.Log($"[TestTakingViewModel] Loaded test: {CurrentAttempt.subject} - {CurrentAttempt.difficulty}, {CurrentAttempt.questions.Count} questions, {TimeRemaining:F0}s remaining");
        }

        /// <summary>
        /// 現在の問題を更新します
        /// </summary>
        private void UpdateCurrentQuestion()
        {
            if (CurrentAttempt == null || CurrentQuestionIndex < 0 || CurrentQuestionIndex >= CurrentAttempt.questions.Count)
            {
                return;
            }

            var question = CurrentAttempt.questions[CurrentQuestionIndex];

            // 表示テキスト更新
            SubjectText = $"{CurrentAttempt.subject} - {CurrentAttempt.difficulty}";
            QuestionNumberText = $"Question {CurrentQuestionIndex + 1}/{CurrentAttempt.questions.Count}";
            QuestionText = question.GetQuestionText(_localization.CurrentLanguage);

            // 選択肢を更新
            var choices = question.GetChoices(_localization.CurrentLanguage);
            for (int i = 0; i < 5; i++)
            {
                _answerChoices[i] = choices[i];
            }
            OnPropertyChanged(nameof(AnswerAText));
            OnPropertyChanged(nameof(AnswerBText));
            OnPropertyChanged(nameof(AnswerCText));
            OnPropertyChanged(nameof(AnswerDText));
            OnPropertyChanged(nameof(AnswerEText));

            // 保存済みの回答を復元
            SelectedAnswerIndex = CurrentAttempt.playerAnswers[CurrentQuestionIndex];

            // 進捗を更新
            UpdateProgress();

            // ナビゲーションボタンの状態を更新
            PreviousQuestionCommand.RaiseCanExecuteChanged();
            NextQuestionCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(IsOnLastQuestion));
        }

        /// <summary>
        /// 進捗を更新します
        /// </summary>
        private void UpdateProgress()
        {
            if (CurrentAttempt == null) return;

            int total = CurrentAttempt.questions.Count;
            int answered = 0;

            foreach (var answer in CurrentAttempt.playerAnswers)
            {
                if (answer >= 0) answered++;
            }

            UnansweredCount = total - answered;
            ProgressPercent = (float)(CurrentQuestionIndex + 1) / total * 100f;

            Debug.Log($"[TestTakingViewModel] Progress: {answered}/{total} answered, {ProgressPercent:F1}%");
        }

        /// <summary>
        /// タイマー表示を更新します
        /// </summary>
        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            TimerText = $"{minutes}:{seconds:D2}";
        }

        /// <summary>
        /// 回答を選択します
        /// </summary>
        private void SelectAnswer(int answerIndex)
        {
            if (CurrentAttempt == null) return;

            SelectedAnswerIndex = answerIndex;

            // サービスに回答を記録
            _testService.RecordAnswer(CurrentQuestionIndex, answerIndex);

            // 選択状態を通知
            OnPropertyChanged(nameof(IsAnswerASelected));
            OnPropertyChanged(nameof(IsAnswerBSelected));
            OnPropertyChanged(nameof(IsAnswerCSelected));
            OnPropertyChanged(nameof(IsAnswerDSelected));
            OnPropertyChanged(nameof(IsAnswerESelected));

            // 進捗を更新
            UpdateProgress();

            Debug.Log($"[TestTakingViewModel] Answer selected: Question {CurrentQuestionIndex + 1}, Answer {(char)('A' + answerIndex)}");
        }

        /// <summary>
        /// 前の問題に移動します
        /// </summary>
        private void ExecutePreviousQuestion(object? parameter)
        {
            if (CanGoPrevious)
            {
                CurrentQuestionIndex--;
            }
        }

        /// <summary>
        /// 次の問題に移動します
        /// </summary>
        private void ExecuteNextQuestion(object? parameter)
        {
            if (CanGoNext)
            {
                CurrentQuestionIndex++;
            }
            else if (IsOnLastQuestion)
            {
                // 最後の問題の場合、提出パネルを表示
                ExecuteShowSubmitPanel(null);
            }
        }

        /// <summary>
        /// 問題をスキップ（未回答）します
        /// </summary>
        private void ExecuteSkipQuestion(object? parameter)
        {
            // 回答をクリア
            if (CurrentAttempt != null)
            {
                _testService.RecordAnswer(CurrentQuestionIndex, -1);
                SelectedAnswerIndex = -1;

                OnPropertyChanged(nameof(IsAnswerASelected));
                OnPropertyChanged(nameof(IsAnswerBSelected));
                OnPropertyChanged(nameof(IsAnswerCSelected));
                OnPropertyChanged(nameof(IsAnswerDSelected));
                OnPropertyChanged(nameof(IsAnswerESelected));

                UpdateProgress();
            }

            // 次の問題へ
            if (CanGoNext)
            {
                CurrentQuestionIndex++;
            }
        }

        /// <summary>
        /// 提出確認パネルを表示します
        /// </summary>
        private void ExecuteShowSubmitPanel(object? parameter)
        {
            IsSubmitPanelVisible = true;
            Debug.Log("[TestTakingViewModel] Submit panel shown.");
        }

        /// <summary>
        /// 提出確認パネルを非表示にします
        /// </summary>
        private void ExecuteHideSubmitPanel(object? parameter)
        {
            IsSubmitPanelVisible = false;
        }

        /// <summary>
        /// テストを提出します
        /// </summary>
        private void ExecuteSubmitTest(object? parameter)
        {
            if (CurrentAttempt == null) return;

            Debug.Log($"[TestTakingViewModel] Submitting test: {UnansweredCount} unanswered questions.");

            // テストを提出
            var result = _testService.SubmitTest();

            Debug.Log($"[TestTakingViewModel] Test submitted. Score: {result.correctAnswers}/{result.totalQuestions}");

            // イベント発火
            TestSubmitted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// タイムアップ時の処理
        /// </summary>
        private void OnTimeUp()
        {
            Debug.Log("[TestTakingViewModel] Time's up! Auto-submitting test.");

            // 自動提出
            ExecuteSubmitTest(null);
        }

        /// <summary>
        /// Viewからの時間更新を受け取ります
        /// </summary>
        public void UpdateTimer(float deltaTime)
        {
            TimeRemaining -= deltaTime;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.Log("[TestTakingViewModel] Disposed.");
            }
            base.Dispose(disposing);
        }
    }
}
