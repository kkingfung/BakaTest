#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;
using BakaTest.Tests.Mocks;
using BakaTest.Data.Champions;
using UnityEngine;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// PointAllocationViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class PointAllocationViewModelTests
    {
        private PointAllocationViewModel? _viewModel;
        private MockPlayerDataService? _playerDataService;
        private MockChampionService? _championService;
        private MockLocalizationService? _localizationService;

        [SetUp]
        public void SetUp()
        {
            _playerDataService = new MockPlayerDataService();
            _championService = new MockChampionService();
            _localizationService = new MockLocalizationService();

            // Add some test points
            _playerDataService.AddPoints(Subject.Math, 200);
            _playerDataService.AddPoints(Subject.Science, 150);
            _playerDataService.AddPoints(Subject.English, 100);
            _playerDataService.AddPoints(Subject.History, 250);

            _viewModel = new PointAllocationViewModel(
                _playerDataService,
                _championService,
                _localizationService);
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
            Assert.IsNotNull(_viewModel!.ResetCommand);
            Assert.IsNotNull(_viewModel.ConfirmCommand);
            Assert.IsNotNull(_viewModel.BackCommand);
        }

        [Test]
        public void Constructor_ShouldLoadAvailablePoints()
        {
            // Assert
            Assert.AreEqual(200, _viewModel!.MathAvailable);
            Assert.AreEqual(150, _viewModel.ScienceAvailable);
            Assert.AreEqual(100, _viewModel.EnglishAvailable);
            Assert.AreEqual(250, _viewModel.HistoryAvailable);
        }

        [Test]
        public void Constructor_ShouldLoadDefaultChampion()
        {
            // Assert
            Assert.IsNotNull(_viewModel!.SelectedChampion);
        }

        [Test]
        public void TotalAvailable_ShouldCalculateCorrectly()
        {
            // Assert
            Assert.AreEqual(700, _viewModel!.TotalAvailable); // 200+150+100+250
        }

        [Test]
        public void TotalAllocated_ShouldCalculateCorrectly()
        {
            // Arrange
            _viewModel!.MathAllocated = 50;
            _viewModel.ScienceAllocated = 30;

            // Assert
            Assert.AreEqual(80, _viewModel.TotalAllocated);
        }

        [Test]
        public void MathAllocated_Changed_ShouldRecalculateStats()
        {
            // Arrange
            int baseAttack = _viewModel!.BaseAttack;

            // Act
            _viewModel.MathAllocated = 100;

            // Assert - bonus attack should be calculated
            Assert.Greater(_viewModel.BonusAttack, 0);
            Assert.Greater(_viewModel.FinalAttack, baseAttack);
        }

        [Test]
        public void HistoryAllocated_Changed_ShouldIncreaseHP()
        {
            // Arrange
            int baseHP = _viewModel!.BaseHP;

            // Act
            _viewModel.HistoryAllocated = 100;

            // Assert
            Assert.Greater(_viewModel.BonusHP, 0);
            Assert.Greater(_viewModel.FinalHP, baseHP);
        }

        [Test]
        public void SelectedChampion_Changed_ShouldLoadStats()
        {
            // Arrange
            var newChampion = _championService!.OwnedChampions[0];

            // Act
            _viewModel!.SelectedChampion = newChampion;

            // Assert
            Assert.AreEqual((int)newChampion.baseStats.HP, _viewModel.BaseHP);
            Assert.AreEqual((int)newChampion.baseStats.Attack, _viewModel.BaseAttack);
            Assert.AreEqual((int)newChampion.baseStats.Defense, _viewModel.BaseDefense);
            Assert.AreEqual((int)newChampion.baseStats.Speed, _viewModel.BaseSpeed);
        }

        [Test]
        public void ChampionName_ShouldDisplayCorrectly()
        {
            // Arrange
            var champion = _viewModel!.SelectedChampion;

            // Assert
            if (champion != null)
            {
                Assert.AreEqual(champion.GetChampionName(_localizationService!.CurrentLanguage), _viewModel.ChampionName);
            }
        }

        [Test]
        public void ResetCommand_ShouldClearAllAllocations()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;
            _viewModel.ScienceAllocated = 50;
            _viewModel.EnglishAllocated = 75;
            _viewModel.HistoryAllocated = 125;

            // Act
            _viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.AreEqual(0, _viewModel.MathAllocated);
            Assert.AreEqual(0, _viewModel.ScienceAllocated);
            Assert.AreEqual(0, _viewModel.EnglishAllocated);
            Assert.AreEqual(0, _viewModel.HistoryAllocated);
        }

        [Test]
        public void ConfirmCommand_WhenValid_CanExecute()
        {
            // Arrange
            _viewModel!.MathAllocated = 50;

            // Assert
            Assert.IsTrue(_viewModel.ConfirmCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmCommand_WhenNoAllocation_CannotExecute()
        {
            // Arrange - no points allocated

            // Assert
            Assert.IsFalse(_viewModel!.ConfirmCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmCommand_WhenExceedsAvailable_CannotExecute()
        {
            // Arrange
            _viewModel!.MathAllocated = 999; // More than available (200)

            // Assert
            Assert.IsFalse(_viewModel.ConfirmCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmCommand_ShouldSpendPoints()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;
            _viewModel.ScienceAllocated = 50;

            // Act
            _viewModel.ConfirmCommand.Execute(null);

            // Assert - points should be spent
            Assert.AreEqual(100, _playerDataService!.GetPoints(Subject.Math)); // 200 - 100
            Assert.AreEqual(100, _playerDataService.GetPoints(Subject.Science)); // 150 - 50
        }

        [Test]
        public void ConfirmCommand_ShouldRaiseEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel!.AllocationConfirmed += (sender, args) => eventFired = true;
            _viewModel.MathAllocated = 50;

            // Act
            _viewModel.ConfirmCommand.Execute(null);

            // Assert
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void BackCommand_ShouldRaiseEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel!.BackRequested += (sender, args) => eventFired = true;

            // Act
            _viewModel.BackCommand.Execute(null);

            // Assert
            Assert.IsTrue(eventFired);
        }

        [Test]
        public void GetAllocatedPoints_ShouldReturnDictionary()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;
            _viewModel.ScienceAllocated = 50;

            // Act
            var points = _viewModel.GetAllocatedPoints();

            // Assert
            Assert.AreEqual(100, points[Subject.Math]);
            Assert.AreEqual(50, points[Subject.Science]);
            Assert.AreEqual(0, points[Subject.English]);
            Assert.AreEqual(0, points[Subject.History]);
        }

        [Test]
        public void SetChampion_ShouldUpdateSelectedChampion()
        {
            // Arrange
            var newChampion = _championService!.OwnedChampions[0];

            // Act
            _viewModel!.SetChampion(newChampion);

            // Assert
            Assert.AreEqual(newChampion, _viewModel.SelectedChampion);
        }

        [Test]
        public void TotalAllocatedText_ShouldFormatCorrectly()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;

            // Assert
            Assert.AreEqual("100 / 700", _viewModel.TotalAllocatedText);
        }

        [Test]
        public void MathPreviewText_ShouldShowBonusAttack()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;

            // Assert
            Assert.IsTrue(_viewModel.MathPreviewText.Contains("Attack"));
        }

        [Test]
        public void AffinityTexts_ShouldDisplayCorrectly()
        {
            // Assert
            Assert.IsTrue(_viewModel!.MathAffinityText.Contains("Math"));
            Assert.IsTrue(_viewModel.ScienceAffinityText.Contains("Science"));
            Assert.IsTrue(_viewModel.EnglishAffinityText.Contains("English"));
            Assert.IsTrue(_viewModel.HistoryAffinityText.Contains("History"));
        }

        [Test]
        public void FinalStats_ShouldEqualBasesPlusBonuses()
        {
            // Arrange
            _viewModel!.MathAllocated = 100;

            // Assert
            Assert.AreEqual(_viewModel.BaseAttack + _viewModel.BonusAttack, _viewModel.FinalAttack);
        }
    }
}
