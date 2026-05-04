#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Tutorial;
using BakaTest.Services.Localization;
using BakaTest.Data.Tutorial;
using BakaTest.Tests.Mocks;
using System;
using UnityEngine;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// TutorialViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class TutorialViewModelTests
    {
        private TutorialViewModel? _viewModel;
        private MockTutorialService? _tutorialService;
        private MockLocalizationService? _localizationService;

        [SetUp]
        public void SetUp()
        {
            _localizationService = new MockLocalizationService();
            _tutorialService = new MockTutorialService();
            _viewModel = new TutorialViewModel(_tutorialService, _localizationService);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.IsNotNull(_viewModel!.NextCommand);
            Assert.IsNotNull(_viewModel.SkipCommand);
            Assert.IsNotNull(_viewModel.BackCommand);
            Assert.IsNotNull(_viewModel.StepTitle);
            Assert.IsNotNull(_viewModel.StepDescription);
            Assert.IsNotNull(_viewModel.ProgressText);
        }

        [Test]
        public void NextCommand_WhenTutorialNotStarted_ShouldStartTutorial()
        {
            // Arrange
            bool tutorialStarted = false;
            _tutorialService!.TutorialStartedEvent += () => tutorialStarted = true;

            // Act
            _viewModel!.NextCommand.Execute(null);

            // Assert
            Assert.IsTrue(tutorialStarted);
        }

        [Test]
        public void NextCommand_WhenTutorialInProgress_ShouldMoveToNextStep()
        {
            // Arrange
            _tutorialService!.StartTutorial();
            int stepChangeCount = 0;
            _tutorialService.StepChangedEvent += (step) => stepChangeCount++;

            // Act
            _viewModel!.NextCommand.Execute(null);

            // Assert
            Assert.AreEqual(1, stepChangeCount);
        }

        [Test]
        public void SkipCommand_ShouldSkipTutorial()
        {
            // Arrange
            _tutorialService!.StartTutorial();
            bool tutorialSkipped = false;
            _viewModel!.TutorialSkipped += (sender, args) => tutorialSkipped = true;

            // Act
            _viewModel.SkipCommand.Execute(null);

            // Assert
            Assert.IsTrue(tutorialSkipped);
            Assert.IsFalse(_tutorialService.IsInTutorial);
        }

        [Test]
        public void TutorialCompleted_Event_ShouldFireWhenTutorialCompletes()
        {
            // Arrange
            bool completedEventFired = false;
            _viewModel!.TutorialCompleted += (sender, args) => completedEventFired = true;

            // Act
            _tutorialService!.CompleteTutorial();

            // Assert
            Assert.IsTrue(completedEventFired);
        }

        [Test]
        public void PropertyChanged_ShouldFireForStepTitle()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel!.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TutorialViewModel.StepTitle))
                    propertyChanged = true;
            };

            // Act
            _tutorialService!.StartTutorial();

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [Test]
        public void PropertyChanged_ShouldFireForProgressText()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel!.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TutorialViewModel.ProgressText))
                    propertyChanged = true;
            };

            // Act
            _tutorialService!.StartTutorial();

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [Test]
        public void ShowNextButton_ShouldBeTrueWhenTutorialInProgress()
        {
            // Arrange & Act
            _tutorialService!.StartTutorial();

            // Assert
            Assert.IsTrue(_viewModel!.ShowNextButton);
        }

        [Test]
        public void ShowSkipButton_ShouldBeFalseWhenTutorialNotStarted()
        {
            // Assert
            Assert.IsFalse(_viewModel!.ShowSkipButton);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeFromEvents()
        {
            // Arrange
            _tutorialService!.StartTutorial();

            // Act
            _viewModel!.Dispose();
            bool eventFired = false;
            _viewModel.PropertyChanged += (sender, args) => eventFired = true;
            _tutorialService.CompleteCurrentStep();

            // Assert
            Assert.IsFalse(eventFired);
        }
    }
}
