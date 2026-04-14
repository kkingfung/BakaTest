#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using BakaTest.Services.Player;

namespace BakaTest.Services.Tests
{
    /// <summary>
    /// テスト管理サービスの実装
    /// </summary>
    public class TestService : ITestService
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly TestConfig _config;
        private TestAttempt? _currentAttempt;

        /// <summary>
        /// 現在のテストセッション
        /// </summary>
        public TestAttempt? CurrentAttempt => _currentAttempt;

        /// <summary>
        /// テスト設定
        /// </summary>
        public TestConfig Config => _config;

        /// <summary>
        /// テストが受験中かどうか
        /// </summary>
        public bool IsTestInProgress => _currentAttempt != null && _currentAttempt.IsInProgress;

        /// <summary>
        /// 1日あたりの教科別ポイント上限
        /// </summary>
        public int DailyPointCap => _config.dailyPointCapPerSubject;

        /// <summary>
        /// テストが開始されたときに発火するイベント
        /// </summary>
        public event Action<TestAttempt>? TestStarted;

        /// <summary>
        /// テストが提出されたときに発火するイベント
        /// </summary>
        public event Action<TestResult>? TestSubmitted;

        /// <summary>
        /// ポイントが獲得されたときに発火するイベント
        /// </summary>
        public event Action<Subject, int>? PointsEarned;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TestService(IPlayerDataService playerDataService, TestConfig? config = null)
        {
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            
            // 設定をロード
            if (config == null)
            {
                config = Resources.Load<TestConfig>("Config/TestConfig");
                if (config == null)
                {
                    Debug.LogWarning("[TestService] TestConfig not found in Resources/Config/. Using default values.");
                    config = ScriptableObject.CreateInstance<TestConfig>();
                }
            }
            _config = config;

            Debug.Log($"[TestService] Initialized with config: {_config.questionsPerTest} questions, {_config.timeLimit}s time limit");
        }

        /// <summary>
        /// 今日のテストプールを取得します
        /// </summary>
        public List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty)
        {
            // TODO: 実際のAI生成システムまたは問題バンクから取得
            // 現在はダミーデータを生成
            return GenerateDummyQuestions(subject, difficulty, _config.questionsPerTest);
        }

        /// <summary>
        /// テストを開始します
        /// </summary>
        public TestAttempt StartTest(Subject subject, DifficultyLevel difficulty)
        {
            if (IsTestInProgress)
            {
                Debug.LogWarning("[TestService] Cannot start new test: test already in progress");
                throw new InvalidOperationException("Test already in progress");
            }

            // テストプールから問題を取得
            var questions = GetTodaysTestPool(subject, difficulty);

            // 新しいテストセッションを作成
            _currentAttempt = new TestAttempt
            {
                subject = subject,
                difficulty = difficulty,
                questions = questions,
                startTime = DateTime.Now
            };

            // 回答配列を初期化（すべて-1 = 未回答）
            _currentAttempt.playerAnswers = new List<int>(new int[questions.Count]);
            for (int i = 0; i < questions.Count; i++)
            {
                _currentAttempt.playerAnswers[i] = -1;
            }

            Debug.Log($"[TestService] Test started: {subject} {difficulty} with {questions.Count} questions");
            TestStarted?.Invoke(_currentAttempt);

            return _currentAttempt;
        }

        /// <summary>
        /// 回答を記録します
        /// </summary>
        public void RecordAnswer(int questionIndex, int answerIndex)
        {
            if (_currentAttempt == null)
            {
                Debug.LogWarning("[TestService] Cannot record answer: no test in progress");
                return;
            }

            _currentAttempt.RecordAnswer(questionIndex, answerIndex);
            Debug.Log($"[TestService] Answer recorded: Question {questionIndex} = Answer {answerIndex}");
        }

        /// <summary>
        /// テストを提出して採点します
        /// </summary>
        public TestResult SubmitTest()
        {
            if (_currentAttempt == null)
            {
                Debug.LogError("[TestService] Cannot submit test: no test in progress");
                throw new InvalidOperationException("No test in progress");
            }

            // テスト終了時刻を記録
            _currentAttempt.EndTest();

            // 正解数を計算
            int correctCount = 0;
            int totalPossiblePoints = 0;

            for (int i = 0; i < _currentAttempt.questions.Count; i++)
            {
                var question = _currentAttempt.questions[i];
                totalPossiblePoints += question.pointValue;

                if (i < _currentAttempt.playerAnswers.Count)
                {
                    if (question.IsCorrect(_currentAttempt.playerAnswers[i]))
                    {
                        correctCount++;
                    }
                }
            }

            // 獲得ポイントを計算（正解した問題のポイントの合計）
            int earnedPoints = 0;
            for (int i = 0; i < _currentAttempt.questions.Count && i < _currentAttempt.playerAnswers.Count; i++)
            {
                if (_currentAttempt.questions[i].IsCorrect(_currentAttempt.playerAnswers[i]))
                {
                    earnedPoints += _currentAttempt.questions[i].pointValue;
                }
            }

            // 1日上限チェック
            int todaysEarned = _playerDataService.GetTodaysEarnedPoints(_currentAttempt.subject);
            int dailyCap = _config.dailyPointCapPerSubject;
            int remaining = Math.Max(0, dailyCap - todaysEarned);

            int pointsToAward = Math.Min(earnedPoints, remaining);
            int pointsCapped = earnedPoints - pointsToAward;
            bool reachedCap = remaining <= 0 || pointsCapped > 0;

            // ポイントを付与
            if (pointsToAward > 0)
            {
                _playerDataService.AddPoints(_currentAttempt.subject, pointsToAward);
                PointsEarned?.Invoke(_currentAttempt.subject, pointsToAward);
            }

            // 結果を生成
            var result = TestResult.FromAttempt(_currentAttempt, pointsToAward, pointsCapped, reachedCap);

            Debug.Log($"[TestService] Test submitted: {correctCount}/{_currentAttempt.questions.Count} correct, {pointsToAward} points earned (capped: {pointsCapped})");

            TestSubmitted?.Invoke(result);

            // セッションをクリア
            _currentAttempt = null;

            return result;
        }

        /// <summary>
        /// 指定教科の今日の残り獲得可能ポイントを取得します
        /// </summary>
        public int GetRemainingDailyPoints(Subject subject)
        {
            int todaysEarned = _playerDataService.GetTodaysEarnedPoints(subject);
            int dailyCap = _config.dailyPointCapPerSubject;
            return Math.Max(0, dailyCap - todaysEarned);
        }

        /// <summary>
        /// ダミー問題を生成します（開発用）
        /// </summary>
        private List<TestQuestion> GenerateDummyQuestions(Subject subject, DifficultyLevel difficulty, int count)
        {
            var questions = new List<TestQuestion>();
            int pointsPerQuestion = _config.GetPointsForDifficulty(difficulty);

            for (int i = 0; i < count; i++)
            {
                var question = new TestQuestion
                {
                    questionId = $"q_{subject}_{difficulty}_{Guid.NewGuid()}",
                    subject = subject,
                    difficulty = difficulty,
                    questionText = $"[DUMMY] {subject} {difficulty} Question {i + 1}: What is the answer?",
                    choices = new string[]
                    {
                        "Choice A",
                        "Choice B",
                        "Choice C (Correct)",
                        "Choice D",
                        "Choice E"
                    },
                    correctAnswerIndex = 2, // Choice C
                    pointValue = pointsPerQuestion,
                    generatedDate = DateTime.Now,
                    isAIGenerated = false,
                    isReviewed = true
                };

                questions.Add(question);
            }

            Debug.Log($"[TestService] Generated {count} dummy questions for {subject} {difficulty}");
            return questions;
        }
    }
}
