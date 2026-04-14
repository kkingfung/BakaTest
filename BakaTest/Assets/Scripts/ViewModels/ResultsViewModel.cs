#nullable enable
using System;
using UnityEngine;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Services.Player;
using BakaTest.Data.Tests;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// 結果表示画面のViewModel
    /// </summary>
    /// <remarks>
    /// テスト結果とバトル結果の両方を表示できる統合画面です。
    /// </remarks>
    public class ResultsViewModel : ViewModelBase
    {
        private readonly IPlayerDataService _playerDataService;

        /// <summary>続けるコマンド</summary>
        public RelayCommand<object?> ContinueCommand { get; }

        /// <summary>メニューに戻るコマンド</summary>
        public RelayCommand<object?> BackToMenuCommand { get; }

        /// <summary>もう一度プレイするコマンド</summary>
        public RelayCommand<object?> PlayAgainCommand { get; }

        private ResultType _resultType;
        /// <summary>結果の種類</summary>
        public ResultType ResultType
        {
            get => _resultType;
            set => SetProperty(ref _resultType, value);
        }

        private TestResult? _testResult;
        /// <summary>テスト結果</summary>
        public TestResult? TestResult
        {
            get => _testResult;
            set
            {
                if (SetProperty(ref _testResult, value))
                {
                    UpdateDisplayValues();
                }
            }
        }

        private BattleResult? _battleResult;
        /// <summary>バトル結果</summary>
        public BattleResult? BattleResult
        {
            get => _battleResult;
            set
            {
                if (SetProperty(ref _battleResult, value))
                {
                    UpdateDisplayValues();
                }
            }
        }

        // 表示用プロパティ
        private string _titleText = string.Empty;
        public string TitleText
        {
            get => _titleText;
            set => SetProperty(ref _titleText, value);
        }

        private string _resultSummary = string.Empty;
        public string ResultSummary
        {
            get => _resultSummary;
            set => SetProperty(ref _resultSummary, value);
        }

        private bool _isVictory;
        public bool IsVictory
        {
            get => _isVictory;
            set => SetProperty(ref _isVictory, value);
        }

        // テスト結果用プロパティ
        private string _subjectText = string.Empty;
        public string SubjectText
        {
            get => _subjectText;
            set => SetProperty(ref _subjectText, value);
        }

        private string _difficultyText = string.Empty;
        public string DifficultyText
        {
            get => _difficultyText;
            set => SetProperty(ref _difficultyText, value);
        }

        private string _scoreText = string.Empty;
        public string ScoreText
        {
            get => _scoreText;
            set => SetProperty(ref _scoreText, value);
        }

        private string _accuracyText = string.Empty;
        public string AccuracyText
        {
            get => _accuracyText;
            set => SetProperty(ref _accuracyText, value);
        }

        private string _pointsEarnedText = string.Empty;
        public string PointsEarnedText
        {
            get => _pointsEarnedText;
            set => SetProperty(ref _pointsEarnedText, value);
        }

        private string _timeText = string.Empty;
        public string TimeText
        {
            get => _timeText;
            set => SetProperty(ref _timeText, value);
        }

        private string _dailyCapWarning = string.Empty;
        public string DailyCapWarning
        {
            get => _dailyCapWarning;
            set => SetProperty(ref _dailyCapWarning, value);
        }

        // バトル結果用プロパティ
        private string _winnerText = string.Empty;
        public string WinnerText
        {
            get => _winnerText;
            set => SetProperty(ref _winnerText, value);
        }

        private string _loserText = string.Empty;
        public string LoserText
        {
            get => _loserText;
            set => SetProperty(ref _loserText, value);
        }

        private string _turnCountText = string.Empty;
        public string TurnCountText
        {
            get => _turnCountText;
            set => SetProperty(ref _turnCountText, value);
        }

        private string _durationText = string.Empty;
        public string DurationText
        {
            get => _durationText;
            set => SetProperty(ref _durationText, value);
        }

        private string _coinsEarnedText = string.Empty;
        public string CoinsEarnedText
        {
            get => _coinsEarnedText;
            set => SetProperty(ref _coinsEarnedText, value);
        }

        /// <summary>続けるボタン要求イベント</summary>
        public event EventHandler? ContinueRequested;

        /// <summary>メニューに戻る要求イベント</summary>
        public event EventHandler? BackToMenuRequested;

        /// <summary>もう一度プレイ要求イベント</summary>
        public event EventHandler? PlayAgainRequested;

        public ResultsViewModel(IPlayerDataService playerDataService)
        {
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));

            // コマンド初期化
            ContinueCommand = new RelayCommand<object?>(ExecuteContinue);
            BackToMenuCommand = new RelayCommand<object?>(ExecuteBackToMenu);
            PlayAgainCommand = new RelayCommand<object?>(ExecutePlayAgain);
        }

        /// <summary>
        /// テスト結果を設定します
        /// </summary>
        public void SetTestResult(TestResult result)
        {
            ResultType = ResultType.Test;
            TestResult = result;
            BattleResult = null;

            Debug.Log($"[ResultsViewModel] Test result set: {result.subject} - {result.correctAnswers}/{result.totalQuestions}");
        }

        /// <summary>
        /// バトル結果を設定します
        /// </summary>
        public void SetBattleResult(BattleResult result)
        {
            ResultType = ResultType.Battle;
            BattleResult = result;
            TestResult = null;

            Debug.Log($"[ResultsViewModel] Battle result set: Winner={result.Winner?.ChampionData.championName}");
        }

        /// <summary>
        /// 表示用の値を更新します
        /// </summary>
        private void UpdateDisplayValues()
        {
            if (ResultType == ResultType.Test && TestResult != null)
            {
                UpdateTestDisplayValues();
            }
            else if (ResultType == ResultType.Battle && BattleResult != null)
            {
                UpdateBattleDisplayValues();
            }
        }

        /// <summary>
        /// テスト結果の表示値を更新します
        /// </summary>
        private void UpdateTestDisplayValues()
        {
            if (TestResult == null) return;

            TitleText = "テスト結果";

            // 教科と難易度
            SubjectText = GetSubjectDisplayName(TestResult.subject);
            DifficultyText = GetDifficultyDisplayName(TestResult.difficulty);

            // スコア
            ScoreText = $"{TestResult.correctAnswers} / {TestResult.totalQuestions}";
            AccuracyText = $"{TestResult.scorePercent:F1}%";

            // ポイント
            PointsEarnedText = $"+{TestResult.pointsEarned}";

            // 時間
            TimeText = $"{TestResult.timeTaken.TotalMinutes:F1} 分";

            // 上限警告
            if (TestResult.reachedDailyCap)
            {
                DailyCapWarning = $"※1日上限に達しました（制限: {TestResult.pointsCapped}ポイント）";
            }
            else
            {
                DailyCapWarning = string.Empty;
            }

            // 結果サマリー
            if (TestResult.scorePercent >= 80)
            {
                ResultSummary = "素晴らしい結果です！";
                IsVictory = true;
            }
            else if (TestResult.scorePercent >= 60)
            {
                ResultSummary = "よくできました！";
                IsVictory = true;
            }
            else if (TestResult.scorePercent >= 40)
            {
                ResultSummary = "もう少し頑張りましょう。";
                IsVictory = false;
            }
            else
            {
                ResultSummary = "復習が必要です。";
                IsVictory = false;
            }
        }

        /// <summary>
        /// バトル結果の表示値を更新します
        /// </summary>
        private void UpdateBattleDisplayValues()
        {
            if (BattleResult == null) return;

            // 勝敗判定
            bool playerWon = BattleResult.Player1Won;
            IsVictory = playerWon;

            TitleText = playerWon ? "勝利！" : "敗北...";
            ResultSummary = BattleResult.GetSummary();

            // 勝者と敗者
            if (BattleResult.Winner != null)
            {
                WinnerText = $"勝者: {BattleResult.Winner.ChampionData.championName}";
            }

            if (BattleResult.Loser != null)
            {
                LoserText = $"敗者: {BattleResult.Loser.ChampionData.championName}";
            }

            // バトル統計
            TurnCountText = $"ターン数: {BattleResult.TurnCount}";
            DurationText = $"所要時間: {BattleResult.Duration.TotalSeconds:F1} 秒";
            CoinsEarnedText = playerWon ? $"獲得コイン: +{BattleResult.CoinsEarned}" : "獲得コイン: 0";
        }

        /// <summary>
        /// 教科の表示名を取得します
        /// </summary>
        private string GetSubjectDisplayName(Subject subject)
        {
            return subject switch
            {
                Subject.Math => "数学",
                Subject.Science => "理科",
                Subject.English => "英語",
                Subject.History => "歴史",
                _ => subject.ToString()
            };
        }

        /// <summary>
        /// 難易度の表示名を取得します
        /// </summary>
        private string GetDifficultyDisplayName(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => "小学校",
                DifficultyLevel.MiddleSchool => "中学校",
                DifficultyLevel.HighSchool => "高校",
                DifficultyLevel.University => "大学",
                _ => difficulty.ToString()
            };
        }

        private void ExecuteContinue(object? parameter)
        {
            Debug.Log("[ResultsViewModel] Continue clicked");
            ContinueRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteBackToMenu(object? parameter)
        {
            Debug.Log("[ResultsViewModel] Back to menu clicked");
            BackToMenuRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ExecutePlayAgain(object? parameter)
        {
            Debug.Log("[ResultsViewModel] Play again clicked");
            PlayAgainRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 結果の種類
    /// </summary>
    public enum ResultType
    {
        /// <summary>テスト結果</summary>
        Test,
        /// <summary>バトル結果</summary>
        Battle
    }
}
