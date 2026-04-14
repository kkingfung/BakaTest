#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Tests;
using BakaTest.Data.Champions;
using BakaTest.Services.Tests;
using BakaTest.Services.Player;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// テスト結果画面のViewModel
    /// </summary>
    /// <remarks>
    /// ITestServiceとIPlayerDataServiceを使用してテスト結果の表示と管理を行います。
    /// </remarks>
    public class TestResultsViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly IPlayerDataService _playerDataService;
        private TestResult? _currentResult;
        private bool _isBreakdownExpanded;

        // Display properties
        private string _subjectText = string.Empty;
        private string _scorePercentText = string.Empty;
        private string _gradeText = string.Empty;
        private string _correctAnswersText = string.Empty;
        private string _timeTakenText = string.Empty;
        private string _pointsEarnedText = string.Empty;
        private string _dailyProgressText = string.Empty;
        private string _remainingPointsText = string.Empty;
        private float _dailyProgressPercent;
        private string _breakdownToggleText = "▼ View Question Breakdown";

        /// <summary>現在の結果</summary>
        public TestResult? CurrentResult
        {
            get => _currentResult;
            private set
            {
                if (SetProperty(ref _currentResult, value))
                {
                    UpdateDisplayProperties();
                }
            }
        }

        /// <summary>教科と難易度表示</summary>
        public string SubjectText
        {
            get => _subjectText;
            private set => SetProperty(ref _subjectText, value);
        }

        /// <summary>スコアパーセント表示</summary>
        public string ScorePercentText
        {
            get => _scorePercentText;
            private set => SetProperty(ref _scorePercentText, value);
        }

        /// <summary>評価テキスト</summary>
        public string GradeText
        {
            get => _gradeText;
            private set => SetProperty(ref _gradeText, value);
        }

        /// <summary>正解数表示</summary>
        public string CorrectAnswersText
        {
            get => _correctAnswersText;
            private set => SetProperty(ref _correctAnswersText, value);
        }

        /// <summary>所要時間表示</summary>
        public string TimeTakenText
        {
            get => _timeTakenText;
            private set => SetProperty(ref _timeTakenText, value);
        }

        /// <summary>獲得ポイント表示</summary>
        public string PointsEarnedText
        {
            get => _pointsEarnedText;
            private set => SetProperty(ref _pointsEarnedText, value);
        }

        /// <summary>本日の進捗表示</summary>
        public string DailyProgressText
        {
            get => _dailyProgressText;
            private set => SetProperty(ref _dailyProgressText, value);
        }

        /// <summary>残りポイント表示</summary>
        public string RemainingPointsText
        {
            get => _remainingPointsText;
            private set => SetProperty(ref _remainingPointsText, value);
        }

        /// <summary>本日の進捗（パーセント）</summary>
        public float DailyProgressPercent
        {
            get => _dailyProgressPercent;
            private set => SetProperty(ref _dailyProgressPercent, value);
        }

        /// <summary>詳細表示の展開状態</summary>
        public bool IsBreakdownExpanded
        {
            get => _isBreakdownExpanded;
            private set
            {
                if (SetProperty(ref _isBreakdownExpanded, value))
                {
                    BreakdownToggleText = value
                        ? "▲ Hide Question Breakdown"
                        : "▼ View Question Breakdown";
                }
            }
        }

        /// <summary>詳細トグルボタンテキスト</summary>
        public string BreakdownToggleText
        {
            get => _breakdownToggleText;
            private set => SetProperty(ref _breakdownToggleText, value);
        }

        /// <summary>問題結果リスト</summary>
        public List<QuestionResult> QuestionResults => CurrentResult?.questionResults ?? new List<QuestionResult>();

        /// <summary>スコアレベル（CSSクラス用）</summary>
        public string ScoreLevel
        {
            get
            {
                if (CurrentResult == null) return "medium";
                if (CurrentResult.scorePercent >= 70f) return "high";
                if (CurrentResult.scorePercent >= 50f) return "medium";
                return "low";
            }
        }

        // Commands
        public RelayCommand<object?> ToggleBreakdownCommand { get; }
        public RelayCommand<object?> ContinueCommand { get; }
        public RelayCommand<object?> RetryTestCommand { get; }
        public RelayCommand<object?> BackToMenuCommand { get; }

        // Events
        public event EventHandler? ContinueRequested;
        public event EventHandler? RetryTestRequested;
        public event EventHandler? BackToMenuRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TestResultsViewModel(ITestService testService, IPlayerDataService playerDataService)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));

            // Commands
            ToggleBreakdownCommand = new RelayCommand<object?>(_ => ExecuteToggleBreakdown());
            ContinueCommand = new RelayCommand<object?>(_ => ExecuteContinue());
            RetryTestCommand = new RelayCommand<object?>(_ => ExecuteRetryTest());
            BackToMenuCommand = new RelayCommand<object?>(_ => ExecuteBackToMenu());

            // 最新の結果を読み込み
            LoadLatestResult();
        }

        /// <summary>
        /// 最新のテスト結果を読み込みます
        /// </summary>
        private void LoadLatestResult()
        {
            // テストサービスから最新の結果を取得
            // （実装は ITestService.GetLatestResult() などが必要）
            // 現時点では CurrentAttempt から生成すると仮定
            
            if (_testService.CurrentAttempt != null && !_testService.CurrentAttempt.IsInProgress)
            {
                // CurrentAttempt から TestResult を生成
                CurrentResult = GenerateResultFromAttempt(_testService.CurrentAttempt);
            }
        }

        /// <summary>
        /// TestAttempt から TestResult を生成します
        /// </summary>
        private TestResult GenerateResultFromAttempt(TestAttempt attempt)
        {
            var result = new TestResult
            {
                subject = attempt.subject,
                difficulty = attempt.difficulty,
                totalQuestions = attempt.questions.Count,
                timeTaken = attempt.TimeTaken,
                attempt = attempt
            };

            // 各問題の結果を計算
            int correctCount = 0;
            int unansweredCount = 0;

            for (int i = 0; i < attempt.questions.Count; i++)
            {
                var question = attempt.questions[i];
                int playerAnswer = attempt.playerAnswers[i];
                bool isCorrect = playerAnswer != -1 && question.IsCorrect(playerAnswer);

                if (playerAnswer == -1)
                {
                    unansweredCount++;
                }

                if (isCorrect)
                {
                    correctCount++;
                }

                var questionResult = new QuestionResult(i, question, playerAnswer, isCorrect);
                result.questionResults.Add(questionResult);
            }

            result.correctAnswers = correctCount;
            result.unansweredQuestions = unansweredCount;

            // ポイント計算
            result.pointsEarned = correctCount * GetPointsPerQuestion(attempt.difficulty);

            return result;
        }

        /// <summary>
        /// 難易度ごとのポイント/問を取得します
        /// </summary>
        private int GetPointsPerQuestion(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => 10,
                DifficultyLevel.MiddleSchool => 20,
                DifficultyLevel.HighSchool => 30,
                DifficultyLevel.University => 50,
                _ => 10
            };
        }

        /// <summary>
        /// 表示プロパティを更新します
        /// </summary>
        private void UpdateDisplayProperties()
        {
            if (CurrentResult == null)
            {
                SubjectText = "No Result";
                ScorePercentText = "0%";
                GradeText = "-";
                CorrectAnswersText = "0 / 0";
                TimeTakenText = "0:00";
                PointsEarnedText = "+0 pts";
                return;
            }

            // 基本情報
            SubjectText = $"{CurrentResult.subject} - {CurrentResult.difficulty}";
            ScorePercentText = $"{CurrentResult.scorePercent:F0}%";
            GradeText = CurrentResult.GetGrade();
            CorrectAnswersText = $"{CurrentResult.correctAnswers} / {CurrentResult.totalQuestions}";

            // 所要時間
            int minutes = (int)CurrentResult.timeTaken.TotalMinutes;
            int seconds = CurrentResult.timeTaken.Seconds;
            TimeTakenText = $"{minutes}:{seconds:D2}";

            // 獲得ポイント
            PointsEarnedText = $"+{CurrentResult.pointsEarned} pts";

            // 本日の進捗
            UpdateDailyProgress();

            // プロパティ変更通知（CSSクラス用）
            OnPropertyChanged(nameof(ScoreLevel));
            OnPropertyChanged(nameof(QuestionResults));
        }

        /// <summary>
        /// 本日の進捗情報を更新します
        /// </summary>
        private void UpdateDailyProgress()
        {
            if (CurrentResult == null) return;

            int todaysPoints = _playerDataService.GetTodaysEarnedPoints(CurrentResult.subject);
            int dailyCap = _testService.Config.dailyPointCapPerSubject;
            int remaining = Math.Max(0, dailyCap - todaysPoints);

            DailyProgressText = $"{todaysPoints} / {dailyCap} points today";
            RemainingPointsText = $"{remaining} points remaining for today";
            DailyProgressPercent = dailyCap > 0 ? (todaysPoints / (float)dailyCap) * 100f : 0f;
        }

        /// <summary>
        /// 詳細表示の切り替え
        /// </summary>
        private void ExecuteToggleBreakdown()
        {
            IsBreakdownExpanded = !IsBreakdownExpanded;
        }

        /// <summary>
        /// 続行ボタン
        /// </summary>
        private void ExecuteContinue()
        {
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 再受験ボタン
        /// </summary>
        private void ExecuteRetryTest()
        {
            RetryTestRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// メニューに戻るボタン
        /// </summary>
        private void ExecuteBackToMenu()
        {
            BackToMenuRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 外部から結果を設定します
        /// </summary>
        public void SetResult(TestResult result)
        {
            CurrentResult = result;
        }
    }
}
