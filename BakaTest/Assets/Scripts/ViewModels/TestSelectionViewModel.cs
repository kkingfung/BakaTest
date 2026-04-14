#nullable enable
using System;
using System.ComponentModel;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using BakaTest.Services.Tests;
using BakaTest.Services.Player;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// テスト選択画面のViewModel
    /// </summary>
    /// <remarks>
    /// ユーザーが教科と難易度を選択し、テストを開始します。
    /// デイリーポイント上限のチェックと表示を行います。
    /// </remarks>
    public class TestSelectionViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly IPlayerDataService _playerDataService;

        // 選択状態
        private Subject _selectedSubject = Subject.Math;
        private DifficultyLevel _selectedDifficulty = DifficultyLevel.Elementary;

        // 表示データ
        private int _questionsCount;
        private string _timeLimit = string.Empty;
        private int _pointsPerQuestion;
        private int _dailyPointsEarned;
        private int _dailyPointCap;
        private int _totalPotentialPoints;
        private bool _isDailyLimitReached;

        /// <summary>現在選択されている教科</summary>
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (SetProperty(ref _selectedSubject, value))
                {
                    UpdateTestInfo();
                    OnPropertyChanged(nameof(IsMathSelected));
                    OnPropertyChanged(nameof(IsScienceSelected));
                    OnPropertyChanged(nameof(IsEnglishSelected));
                    OnPropertyChanged(nameof(IsHistorySelected));
                }
            }
        }

        /// <summary>現在選択されている難易度</summary>
        public DifficultyLevel SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                if (SetProperty(ref _selectedDifficulty, value))
                {
                    UpdateTestInfo();
                    OnPropertyChanged(nameof(IsElementarySelected));
                    OnPropertyChanged(nameof(IsMiddleSchoolSelected));
                    OnPropertyChanged(nameof(IsHighSchoolSelected));
                    OnPropertyChanged(nameof(IsUniversitySelected));
                }
            }
        }

        // 選択状態プロパティ（UI binding用）
        public bool IsMathSelected => _selectedSubject == Subject.Math;
        public bool IsScienceSelected => _selectedSubject == Subject.Science;
        public bool IsEnglishSelected => _selectedSubject == Subject.English;
        public bool IsHistorySelected => _selectedSubject == Subject.History;

        public bool IsElementarySelected => _selectedDifficulty == DifficultyLevel.Elementary;
        public bool IsMiddleSchoolSelected => _selectedDifficulty == DifficultyLevel.MiddleSchool;
        public bool IsHighSchoolSelected => _selectedDifficulty == DifficultyLevel.HighSchool;
        public bool IsUniversitySelected => _selectedDifficulty == DifficultyLevel.University;

        /// <summary>問題数</summary>
        public int QuestionsCount
        {
            get => _questionsCount;
            private set => SetProperty(ref _questionsCount, value);
        }

        /// <summary>制限時間（表示用）</summary>
        public string TimeLimit
        {
            get => _timeLimit;
            private set => SetProperty(ref _timeLimit, value);
        }

        /// <summary>問題あたりのポイント</summary>
        public int PointsPerQuestion
        {
            get => _pointsPerQuestion;
            private set => SetProperty(ref _pointsPerQuestion, value);
        }

        /// <summary>今日獲得したポイント</summary>
        public int DailyPointsEarned
        {
            get => _dailyPointsEarned;
            private set => SetProperty(ref _dailyPointsEarned, value);
        }

        /// <summary>デイリー上限</summary>
        public int DailyPointCap
        {
            get => _dailyPointCap;
            private set => SetProperty(ref _dailyPointCap, value);
        }

        /// <summary>獲得可能な合計ポイント</summary>
        public int TotalPotentialPoints
        {
            get => _totalPotentialPoints;
            private set => SetProperty(ref _totalPotentialPoints, value);
        }

        /// <summary>デイリー上限に達しているか</summary>
        public bool IsDailyLimitReached
        {
            get => _isDailyLimitReached;
            private set => SetProperty(ref _isDailyLimitReached, value);
        }

        /// <summary>デイリー進捗表示</summary>
        public string DailyProgressText => $"{DailyPointsEarned} / {DailyPointCap}";

        // Commands
        public RelayCommand<object?> SelectMathCommand { get; }
        public RelayCommand<object?> SelectScienceCommand { get; }
        public RelayCommand<object?> SelectEnglishCommand { get; }
        public RelayCommand<object?> SelectHistoryCommand { get; }

        public RelayCommand<object?> SelectElementaryCommand { get; }
        public RelayCommand<object?> SelectMiddleSchoolCommand { get; }
        public RelayCommand<object?> SelectHighSchoolCommand { get; }
        public RelayCommand<object?> SelectUniversityCommand { get; }

        public RelayCommand<object?> StartTestCommand { get; }
        public RelayCommand<object?> BackToMenuCommand { get; }

        // Events
        public event EventHandler? TestStartRequested;
        public event EventHandler? BackToMenuRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TestSelectionViewModel(
            ITestService testService,
            IPlayerDataService playerDataService)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));

            // Commands
            SelectMathCommand = new RelayCommand<object?>(_ => SelectedSubject = Subject.Math);
            SelectScienceCommand = new RelayCommand<object?>(_ => SelectedSubject = Subject.Science);
            SelectEnglishCommand = new RelayCommand<object?>(_ => SelectedSubject = Subject.English);
            SelectHistoryCommand = new RelayCommand<object?>(_ => SelectedSubject = Subject.History);

            SelectElementaryCommand = new RelayCommand<object?>(_ => SelectedDifficulty = DifficultyLevel.Elementary);
            SelectMiddleSchoolCommand = new RelayCommand<object?>(_ => SelectedDifficulty = DifficultyLevel.MiddleSchool);
            SelectHighSchoolCommand = new RelayCommand<object?>(_ => SelectedDifficulty = DifficultyLevel.HighSchool);
            SelectUniversityCommand = new RelayCommand<object?>(_ => SelectedDifficulty = DifficultyLevel.University);

            StartTestCommand = new RelayCommand<object?>(ExecuteStartTest, CanExecuteStartTest);
            BackToMenuCommand = new RelayCommand<object?>(ExecuteBackToMenu);

            // 初期データ読み込み
            UpdateTestInfo();

            Debug.Log("[TestSelectionViewModel] Initialized.");
        }

        /// <summary>
        /// テスト情報を更新します
        /// </summary>
        private void UpdateTestInfo()
        {
            // 問題数と制限時間
            QuestionsCount = _testService.Config.questionsPerTest;
            int timeLimitSeconds = (int)_testService.Config.timeLimit;
            TimeLimit = $"{timeLimitSeconds / 60}:{(timeLimitSeconds % 60):D2}";

            // ポイント計算
            PointsPerQuestion = GetPointsPerQuestion(_selectedDifficulty);
            TotalPotentialPoints = QuestionsCount * PointsPerQuestion;

            // デイリー進捗
            DailyPointCap = _testService.Config.dailyPointCapPerSubject;
            DailyPointsEarned = _playerDataService.GetTodaysEarnedPoints(_selectedSubject);
            IsDailyLimitReached = DailyPointsEarned >= DailyPointCap;

            // StartTestCommandの実行可能状態を更新
            StartTestCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(DailyProgressText));

            Debug.Log($"[TestSelectionViewModel] Test info updated: {_selectedSubject} - {_selectedDifficulty}, Potential Points: {TotalPotentialPoints}");
        }

        /// <summary>
        /// 難易度に応じたポイント/問を取得
        /// </summary>
        private int GetPointsPerQuestion(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => _testService.Config.elementaryPointsPerQuestion,
                DifficultyLevel.MiddleSchool => _testService.Config.middleSchoolPointsPerQuestion,
                DifficultyLevel.HighSchool => _testService.Config.highSchoolPointsPerQuestion,
                DifficultyLevel.University => _testService.Config.universityPointsPerQuestion,
                _ => 10
            };
        }

        /// <summary>
        /// テスト開始を実行
        /// </summary>
        private void ExecuteStartTest(object? parameter)
        {
            Debug.Log($"[TestSelectionViewModel] Starting test: {SelectedSubject} - {SelectedDifficulty}");

            if (IsDailyLimitReached)
            {
                Debug.Log("[TestSelectionViewModel] Daily limit reached. Starting practice mode (no points).");
            }

            TestStartRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// テスト開始可能か
        /// </summary>
        private bool CanExecuteStartTest(object? parameter)
        {
            // テストが進行中でないことを確認
            // 将来的には問題数チェックも追加可能
            return !_testService.IsTestInProgress;
        }

        /// <summary>
        /// データを再読み込みします（View OnEnable時などに使用）
        /// </summary>
        public void RefreshData()
        {
            UpdateTestInfo();
            Debug.Log("[TestSelectionViewModel] Data refreshed.");
        }

        /// <summary>
        /// メニューに戻る
        /// </summary>
        private void ExecuteBackToMenu(object? parameter)
        {
            Debug.Log("[TestSelectionViewModel] Returning to menu.");
            BackToMenuRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.Log("[TestSelectionViewModel] Disposed.");
            }
            base.Dispose(disposing);
        }
    }
}
