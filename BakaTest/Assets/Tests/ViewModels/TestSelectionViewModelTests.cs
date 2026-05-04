#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Tests;
using BakaTest.Services.Player;
using BakaTest.Tests.Mocks;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using System;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// TestSelectionViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class TestSelectionViewModelTests
    {
        private TestSelectionViewModel? _viewModel;
        private MockTestService? _testService;
        private MockPlayerDataService? _playerDataService;

        [SetUp]
        public void SetUp()
        {
            _testService = new MockTestService();
            _playerDataService = new MockPlayerDataService();
            _viewModel = new TestSelectionViewModel(_testService, _playerDataService);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        [Test]
        public void Constructor_ShouldInitializeCommands()
        {
            // Assert
            Assert.IsNotNull(_viewModel!.SelectMathCommand);
            Assert.IsNotNull(_viewModel.SelectScienceCommand);
            Assert.IsNotNull(_viewModel.SelectEnglishCommand);
            Assert.IsNotNull(_viewModel.SelectHistoryCommand);
            Assert.IsNotNull(_viewModel.SelectElementaryCommand);
            Assert.IsNotNull(_viewModel.SelectMiddleSchoolCommand);
            Assert.IsNotNull(_viewModel.SelectHighSchoolCommand);
            Assert.IsNotNull(_viewModel.SelectUniversityCommand);
            Assert.IsNotNull(_viewModel.StartTestCommand);
            Assert.IsNotNull(_viewModel.BackToMenuCommand);
        }

        [Test]
        public void Constructor_ShouldSetDefaultSelection()
        {
            // Assert
            Assert.AreEqual(Subject.Math, _viewModel!.SelectedSubject);
            Assert.AreEqual(DifficultyLevel.Elementary, _viewModel.SelectedDifficulty);
        }

        [Test]
        public void SelectMathCommand_ShouldSetMathSubject()
        {
            // Arrange
            _viewModel!.SelectedSubject = Subject.Science;

            // Act
            _viewModel.SelectMathCommand.Execute(null);

            // Assert
            Assert.AreEqual(Subject.Math, _viewModel.SelectedSubject);
            Assert.IsTrue(_viewModel.IsMathSelected);
        }

        [Test]
        public void SelectScienceCommand_ShouldSetScienceSubject()
        {
            // Act
            _viewModel!.SelectScienceCommand.Execute(null);

            // Assert
            Assert.AreEqual(Subject.Science, _viewModel.SelectedSubject);
            Assert.IsTrue(_viewModel.IsScienceSelected);
        }

        [Test]
        public void SelectElementaryCommand_ShouldSetElementaryDifficulty()
        {
            // Arrange
            _viewModel!.SelectedDifficulty = DifficultyLevel.University;

            // Act
            _viewModel.SelectElementaryCommand.Execute(null);

            // Assert
            Assert.AreEqual(DifficultyLevel.Elementary, _viewModel.SelectedDifficulty);
            Assert.IsTrue(_viewModel.IsElementarySelected);
        }

        [Test]
        public void SelectHighSchoolCommand_ShouldSetHighSchoolDifficulty()
        {
            // Act
            _viewModel!.SelectHighSchoolCommand.Execute(null);

            // Assert
            Assert.AreEqual(DifficultyLevel.HighSchool, _viewModel.SelectedDifficulty);
            Assert.IsTrue(_viewModel.IsHighSchoolSelected);
        }

        [Test]
        public void SelectedSubject_Changed_ShouldUpdateTestInfo()
        {
            // Arrange
            int initialPoints = _viewModel!.TotalPotentialPoints;

            // Act
            _viewModel.SelectedDifficulty = DifficultyLevel.University;

            // Assert - university has higher points per question
            Assert.Greater(_viewModel.TotalPotentialPoints, initialPoints);
        }

        [Test]
        public void SelectedDifficulty_Changed_ShouldUpdatePointsPerQuestion()
        {
            // Act
            _viewModel!.SelectedDifficulty = DifficultyLevel.University;

            // Assert
            Assert.AreEqual(100, _viewModel.PointsPerQuestion); // University: 100 points/question
        }

        [Test]
        public void QuestionsCount_ShouldMatchConfig()
        {
            // Assert
            Assert.AreEqual(10, _viewModel!.QuestionsCount);
        }

        [Test]
        public void TimeLimit_ShouldFormatCorrectly()
        {
            // Assert - 300 seconds = 5:00
            Assert.AreEqual("5:00", _viewModel!.TimeLimit);
        }

        [Test]
        public void DailyProgressText_ShouldFormatCorrectly()
        {
            // Assert
            Assert.AreEqual("0 / 1000", _viewModel!.DailyProgressText);
        }

        [Test]
        public void IsDailyLimitReached_WhenUnderCap_ShouldBeFalse()
        {
            // Assert
            Assert.IsFalse(_viewModel!.IsDailyLimitReached);
        }

        [Test]
        public void IsDailyLimitReached_WhenAtCap_ShouldBeTrue()
        {
            // Arrange - earn 1000 points to hit the cap
            _playerDataService!.AddPoints(Subject.Math, 1000);

            // Act
            _viewModel!.RefreshData();

            // Assert
            Assert.IsTrue(_viewModel.IsDailyLimitReached);
        }

        [Test]
        public void StartTestCommand_ShouldRaiseTestStartEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel!.TestStartRequested += (sender, args) => eventFired = true;

            // Act
            _viewModel.StartTestCommand.Execute(null);

            // Assert
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void StartTestCommand_CanExecute_WhenNoTestInProgress()
        {
            // Assert
            Assert.IsTrue(_viewModel!.StartTestCommand.CanExecute(null));
        }

        [Test]
        public void StartTestCommand_CannotExecute_WhenTestInProgress()
        {
            // Arrange
            _testService!.StartTest(Subject.Math, DifficultyLevel.Elementary);

            // Act
            _viewModel!.RefreshData();

            // Assert
            Assert.IsFalse(_viewModel.StartTestCommand.CanExecute(null));
        }

        [Test]
        public void BackToMenuCommand_ShouldRaiseBackEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel!.BackToMenuRequested += (sender, args) => eventFired = true;

            // Act
            _viewModel.BackToMenuCommand.Execute(null);

            // Assert
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void RefreshData_ShouldUpdateDailyProgress()
        {
            // Arrange
            _playerDataService!.AddPoints(Subject.Math, 500);

            // Act
            _viewModel!.RefreshData();

            // Assert
            Assert.AreEqual(500, _viewModel.DailyPointsEarned);
        }

        [Test]
        public void TotalPotentialPoints_ShouldCalculateCorrectly()
        {
            // Arrange - Elementary: 10 points/question × 10 questions = 100
            _viewModel!.SelectedDifficulty = DifficultyLevel.Elementary;

            // Assert
            Assert.AreEqual(100, _viewModel.TotalPotentialPoints);

            // Arrange - HighSchool: 50 points/question × 10 questions = 500
            _viewModel.SelectedDifficulty = DifficultyLevel.HighSchool;

            // Assert
            Assert.AreEqual(500, _viewModel.TotalPotentialPoints);
        }
    }
}
