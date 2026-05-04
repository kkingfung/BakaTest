#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using BakaTest.Services.Tests;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// TestServiceのモック実装（テスト用）
    /// </summary>
    public class MockTestService : ITestService
    {
        private readonly TestConfig _config;
        private TestAttempt? _currentAttempt;
        private readonly Dictionary<Subject, int> _todaysEarnedPoints = new();

        public TestAttempt? CurrentAttempt => _currentAttempt;
        public TestConfig Config => _config;
        public bool IsTestInProgress => _currentAttempt != null;
        public int DailyPointCap => _config.dailyPointCapPerSubject;

        public event Action<TestAttempt>? TestStarted;
        public event Action<TestResult>? TestSubmitted;
        public event Action<Subject, int>? PointsEarned;

        public MockTestService()
        {
            // Create default test config
            _config = new TestConfig
            {
                questionsPerTest = 10,
                timeLimit = 300, // 5 minutes
                elementaryPointsPerQuestion = 10,
                middleSchoolPointsPerQuestion = 20,
                highSchoolPointsPerQuestion = 50,
                universityPointsPerQuestion = 100,
                dailyPointCapPerSubject = 1000
            };

            // Initialize today's earned points
            _todaysEarnedPoints[Subject.Math] = 0;
            _todaysEarnedPoints[Subject.Science] = 0;
            _todaysEarnedPoints[Subject.English] = 0;
            _todaysEarnedPoints[Subject.History] = 0;
        }

        public List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty)
        {
            return GenerateTestQuestions(subject, difficulty);
        }

        public TestAttempt StartTest(Subject subject, DifficultyLevel difficulty)
        {
            var questions = GenerateTestQuestions(subject, difficulty);
            _currentAttempt = new TestAttempt
            {
                subject = subject,
                difficulty = difficulty,
                questions = questions,
                playerAnswers = new List<int>(new int[questions.Count]),
                startTime = DateTime.Now
            };

            // Initialize all answers to -1 (unanswered)
            for (int i = 0; i < _currentAttempt.playerAnswers.Count; i++)
            {
                _currentAttempt.playerAnswers[i] = -1;
            }

            TestStarted?.Invoke(_currentAttempt);
            return _currentAttempt;
        }

        public void RecordAnswer(int questionIndex, int answerIndex)
        {
            if (_currentAttempt == null)
            {
                throw new InvalidOperationException("No test in progress");
            }

            _currentAttempt.RecordAnswer(questionIndex, answerIndex);
        }

        public TestResult SubmitTest()
        {
            if (_currentAttempt == null)
            {
                throw new InvalidOperationException("No test in progress");
            }

            // Calculate result
            int correctAnswers = 0;
            int totalAnswered = 0;

            for (int i = 0; i < _currentAttempt.playerAnswers.Count; i++)
            {
                int selectedAnswer = _currentAttempt.playerAnswers[i];
                if (selectedAnswer >= 0) // -1 means unanswered
                {
                    totalAnswered++;
                    if (i < _currentAttempt.questions.Count)
                    {
                        if (_currentAttempt.questions[i].correctAnswerIndex == selectedAnswer)
                        {
                            correctAnswers++;
                        }
                    }
                }
            }

            int pointsPerQuestion = GetPointsPerQuestion(_currentAttempt.difficulty);
            int pointsEarned = correctAnswers * pointsPerQuestion;

            var result = new TestResult
            {
                subject = _currentAttempt.subject,
                difficulty = _currentAttempt.difficulty,
                totalQuestions = _currentAttempt.questions.Count,
                correctAnswers = correctAnswers,
                pointsEarned = pointsEarned,
                timeTaken = DateTime.Now - _currentAttempt.startTime
            };

            // Update today's earned points
            if (_todaysEarnedPoints.ContainsKey(_currentAttempt.subject))
            {
                _todaysEarnedPoints[_currentAttempt.subject] += pointsEarned;
            }

            // End the test
            _currentAttempt.EndTest();

            // Fire events
            TestSubmitted?.Invoke(result);
            PointsEarned?.Invoke(result.subject, pointsEarned);

            // Clear current attempt
            _currentAttempt = null;

            return result;
        }

        public int GetTodaysEarnedPoints(Subject subject)
        {
            return _todaysEarnedPoints.GetValueOrDefault(subject, 0);
        }

        public int GetRemainingDailyPoints(Subject subject)
        {
            int earned = GetTodaysEarnedPoints(subject);
            return Math.Max(0, _config.dailyPointCapPerSubject - earned);
        }

        /// <summary>
        /// テスト用問題生成（ダミー）
        /// </summary>
        private List<TestQuestion> GenerateTestQuestions(Subject subject, DifficultyLevel difficulty)
        {
            var questions = new List<TestQuestion>();
            for (int i = 0; i < _config.questionsPerTest; i++)
            {
                var question = new TestQuestion
                {
                    questionId = $"{subject}_{difficulty}_{i}",
                    subject = subject,
                    difficulty = difficulty,
                    correctAnswerIndex = 0
                };

                // Set localized question text
                question.SetQuestionText(Data.Localization.Language.English, $"Test question {i + 1}");

                // Set localized choices
                question.SetChoice(Data.Localization.Language.English, 0, "A");
                question.SetChoice(Data.Localization.Language.English, 1, "B");
                question.SetChoice(Data.Localization.Language.English, 2, "C");
                question.SetChoice(Data.Localization.Language.English, 3, "D");
                question.SetChoice(Data.Localization.Language.English, 4, "E");

                // Set localized explanation
                question.SetExplanation(Data.Localization.Language.English, "Test explanation");

                questions.Add(question);
            }
            return questions;
        }

        private int GetPointsPerQuestion(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => _config.elementaryPointsPerQuestion,
                DifficultyLevel.MiddleSchool => _config.middleSchoolPointsPerQuestion,
                DifficultyLevel.HighSchool => _config.highSchoolPointsPerQuestion,
                DifficultyLevel.University => _config.universityPointsPerQuestion,
                _ => 10
            };
        }

        /// <summary>
        /// テスト用: 今日獲得したポイントをリセット
        /// </summary>
        public void ResetTodaysPoints()
        {
            _todaysEarnedPoints[Subject.Math] = 0;
            _todaysEarnedPoints[Subject.Science] = 0;
            _todaysEarnedPoints[Subject.English] = 0;
            _todaysEarnedPoints[Subject.History] = 0;
        }
    }
}
