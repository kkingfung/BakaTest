#nullable enable
using NUnit.Framework;
using BakaTest.Services.Tests;
using BakaTest.Data.Tests;
using BakaTest.Data.Champions;
using BakaTest.Tests.Mocks;
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Tests.Services
{
    /// <summary>
    /// TestServiceのユニットテスト
    /// </summary>
    [TestFixture]
    public class TestServiceTests
    {
        private TestService? _testService;
        private TestConfig? _testConfig;

        [SetUp]
        public void SetUp()
        {
            // Create test configuration
            _testConfig = ScriptableObject.CreateInstance<TestConfig>();
            _testConfig.questionsPerTest = 10;
            _testConfig.timeLimit = 300f; // 5 minutes
            _testConfig.dailyPointCapPerSubject = 1000;

            // Initialize test service
            _testService = new TestService(new MockPlayerDataService(), _testConfig);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            if (_testConfig != null)
            {
                Object.DestroyImmediate(_testConfig);
            }
        }

        [Test]
        public void StartTest_ShouldCreateTestAttempt()
        {
            // Arrange
            var subject = Subject.Math;
            var difficulty = DifficultyLevel.Elementary;

            // Act
            var attempt = _testService!.StartTest(subject, difficulty);

            // Assert
            Assert.IsNotNull(attempt);
            Assert.AreEqual(subject, attempt!.subject);
            Assert.AreEqual(difficulty, attempt.difficulty);
            Assert.AreEqual(10, attempt.questions.Count);
            Assert.AreEqual(10, attempt.playerAnswers.Count);
            Assert.IsTrue(attempt.startTime <= System.DateTime.Now);
        }

        [Test]
        public void StartTest_ShouldSetCurrentAttempt()
        {
            // Arrange
            var subject = Subject.Science;
            var difficulty = DifficultyLevel.MiddleSchool;

            // Act
            _testService!.StartTest(subject, difficulty);

            // Assert
            Assert.IsNotNull(_testService.CurrentAttempt);
            Assert.AreEqual(subject, _testService.CurrentAttempt!.subject);
        }

        [Test]
        public void RecordAnswer_ShouldStorePlayerAnswer()
        {
            // Arrange
            _testService!.StartTest(Subject.Math, DifficultyLevel.Elementary);
            var questionIndex = 0;
            var answerIndex = 2;

            // Act
            _testService.RecordAnswer(questionIndex, answerIndex);

            // Assert
            Assert.AreEqual(answerIndex, _testService.CurrentAttempt!.playerAnswers[questionIndex]);
        }

        [Test]
        public void SubmitTest_ShouldCalculateCorrectScore()
        {
            // Arrange
            _testService!.StartTest(Subject.Math, DifficultyLevel.Elementary);

            // Simulate answering questions (we know correct answers from test data)
            for (int i = 0; i < _testService.CurrentAttempt!.questions.Count; i++)
            {
                var question = _testService.CurrentAttempt.questions[i];
                _testService.RecordAnswer(i, question.correctAnswerIndex);
            }

            // Act
            var result = _testService.SubmitTest();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.totalQuestions);
            Assert.AreEqual(10, result.correctAnswers);
            Assert.AreEqual(100f, result.scorePercent);
        }

        [Test]
        public void SubmitTest_WithPartialCorrect_ShouldCalculateCorrectScore()
        {
            // Arrange
            _testService!.StartTest(Subject.English, DifficultyLevel.HighSchool);

            // Answer half correctly, half incorrectly
            for (int i = 0; i < _testService.CurrentAttempt!.questions.Count; i++)
            {
                var question = _testService.CurrentAttempt.questions[i];
                if (i < 5)
                {
                    // Correct answer
                    _testService.RecordAnswer(i, question.correctAnswerIndex);
                }
                else
                {
                    // Wrong answer
                    int wrongAnswer = (question.correctAnswerIndex + 1) % 5;
                    _testService.RecordAnswer(i, wrongAnswer);
                }
            }

            // Act
            var result = _testService.SubmitTest();

            // Assert
            Assert.AreEqual(10, result.totalQuestions);
            Assert.AreEqual(5, result.correctAnswers);
            Assert.AreEqual(50f, result.scorePercent);
        }

        [Test]
        public void GetRemainingDailyPoints_InitialValue_ShouldReturnFullCap()
        {
            // Act
            var remaining = _testService!.GetRemainingDailyPoints(Subject.Math);

            // Assert
            Assert.AreEqual(1000, remaining);
        }

        [Test]
        public void SubmitTest_ShouldDecreaseDailyPoints()
        {
            // Arrange
            _testService!.StartTest(Subject.Math, DifficultyLevel.HighSchool);

            // Answer all correctly
            for (int i = 0; i < _testService.CurrentAttempt!.questions.Count; i++)
            {
                var question = _testService.CurrentAttempt.questions[i];
                _testService.RecordAnswer(i, question.correctAnswerIndex);
            }

            var remainingBefore = _testService.GetRemainingDailyPoints(Subject.Math);

            // Act
            var result = _testService.SubmitTest();
            var remainingAfter = _testService.GetRemainingDailyPoints(Subject.Math);

            // Assert
            Assert.Less(remainingAfter, remainingBefore);
            Assert.AreEqual(remainingBefore - result.pointsEarned, remainingAfter);
        }

        [Test]
        public void GetTodaysTestPool_ShouldReturnUniqueQuestions()
        {
            // Arrange
            var subject = Subject.History;
            var difficulty = DifficultyLevel.University;

            // Act
            var pool = _testService!.GetTodaysTestPool(subject, difficulty);

            // Assert
            Assert.IsNotNull(pool);
            Assert.GreaterOrEqual(pool.Count, 10); // At least enough for one test

            // Check uniqueness
            var uniqueIds = new HashSet<string>();
            foreach (var question in pool)
            {
                Assert.IsTrue(uniqueIds.Add(question.questionId), "Duplicate question found in pool");
            }
        }

        [Test]
        public void StartTest_WhenNoCurrentAttempt_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _testService!.StartTest(Subject.Math, DifficultyLevel.Elementary));
        }

        [Test]
        public void RecordAnswer_InvalidIndex_ShouldNotThrow()
        {
            // Arrange
            _testService!.StartTest(Subject.Math, DifficultyLevel.Elementary);

            // Act & Assert (should handle gracefully)
            Assert.DoesNotThrow(() => _testService.RecordAnswer(-1, 0));
            Assert.DoesNotThrow(() => _testService.RecordAnswer(999, 0));
        }

        [Test]
        public void SubmitTest_WithoutStarting_ShouldReturnNull()
        {
            // Act
            var result = _testService!.SubmitTest();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Config_ShouldReturnCorrectValues()
        {
            // Assert
            Assert.AreEqual(10, _testService!.Config.questionsPerTest);
            Assert.AreEqual(300f, _testService.Config.timeLimit);
            Assert.AreEqual(1000, _testService.Config.dailyPointCapPerSubject);
        }
    }
}
