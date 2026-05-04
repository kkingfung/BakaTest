#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;
using BakaTest.Tests.Mocks;
using BakaTest.Data.Champions;
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// ChampionSelectionViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class ChampionSelectionViewModelTests
    {
        private ChampionSelectionViewModel? _viewModel;
        private MockChampionService? _championService;
        private MockLocalizationService? _localizationService;
        private List<ChampionData> _testChampions = new();

        [SetUp]
        public void SetUp()
        {
            _championService = new MockChampionService();
            _localizationService = new MockLocalizationService();
            _viewModel = new ChampionSelectionViewModel(_championService, _localizationService);

            // Create test champions are created by MockChampionService
            _testChampions = new List<ChampionData>(_championService.OwnedChampions);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            foreach (var champion in _testChampions)
            {
                if (champion != null)
                {
                    Object.DestroyImmediate(champion);
                }
            }
            _testChampions.Clear();
        }

        [Test]
        public void Constructor_ShouldInitializeCommands()
        {
            // Assert
            Assert.IsNotNull(_viewModel!.FilterAllCommand);
            Assert.IsNotNull(_viewModel.FilterOwnedCommand);
            Assert.IsNotNull(_viewModel.FilterFreeCommand);
            Assert.IsNotNull(_viewModel.SelectChampionCommand);
            Assert.IsNotNull(_viewModel.ConfirmSelectionCommand);
            Assert.IsNotNull(_viewModel.BackCommand);
        }

        [Test]
        public void Constructor_ShouldLoadChampions()
        {
            // Assert
            Assert.Greater(_viewModel!.AllChampions.Count, 0);
            Assert.Greater(_viewModel.FilteredChampions.Count, 0);
        }

        [Test]
        public void Constructor_ShouldSelectFirstChampion()
        {
            // Assert
            Assert.IsNotNull(_viewModel!.SelectedChampion);
        }

        [Test]
        public void FilterAllCommand_ShouldShowAllChampions()
        {
            // Act
            _viewModel!.FilterAllCommand.Execute(null);

            // Assert
            Assert.AreEqual(ChampionSelectionViewModel.ChampionFilter.All, _viewModel.CurrentFilter);
            Assert.IsTrue(_viewModel.IsFilterAllSelected);
            Assert.AreEqual(_viewModel.AllChampions.Count, _viewModel.FilteredChampions.Count);
        }

        [Test]
        public void FilterOwnedCommand_ShouldShowOwnedChampions()
        {
            // Act
            _viewModel!.FilterOwnedCommand.Execute(null);

            // Assert
            Assert.AreEqual(ChampionSelectionViewModel.ChampionFilter.Owned, _viewModel.CurrentFilter);
            Assert.IsTrue(_viewModel.IsFilterOwnedSelected);
            Assert.AreEqual(_championService!.OwnedChampions.Count, _viewModel.FilteredChampions.Count);
        }

        [Test]
        public void FilterFreeCommand_ShouldShowFreeRotationChampions()
        {
            // Act
            _viewModel!.FilterFreeCommand.Execute(null);

            // Assert
            Assert.AreEqual(ChampionSelectionViewModel.ChampionFilter.FreeRotation, _viewModel.CurrentFilter);
            Assert.IsTrue(_viewModel.IsFilterFreeSelected);
            Assert.AreEqual(_championService!.FreeRotationChampions.Count, _viewModel.FilteredChampions.Count);
        }

        [Test]
        public void SelectChampionCommand_ShouldUpdateSelectedChampion()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];

            // Act
            _viewModel.SelectChampionCommand.Execute(champion);

            // Assert
            Assert.AreEqual(champion, _viewModel.SelectedChampion);
        }

        [Test]
        public void SelectedChampion_Changed_ShouldUpdateDisplayProperties()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];

            // Act
            _viewModel.SelectedChampion = champion;

            // Assert
            Assert.AreEqual(champion.GetChampionName(_localizationService!.CurrentLanguage), _viewModel.ChampionName);
            Assert.AreEqual(champion.role.ToString(), _viewModel.ChampionRole);
            Assert.AreEqual(champion.element.ToString(), _viewModel.ChampionElement);
        }

        [Test]
        public void SelectedChampion_Changed_ShouldUpdateStats()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];

            // Act
            _viewModel.SelectedChampion = champion;

            // Assert
            Assert.AreEqual((int)champion.baseStats.HP, _viewModel.HPValue);
            Assert.AreEqual((int)champion.baseStats.Attack, _viewModel.AttackValue);
            Assert.AreEqual((int)champion.baseStats.Defense, _viewModel.DefenseValue);
            Assert.AreEqual((int)champion.baseStats.Speed, _viewModel.SpeedValue);
        }

        [Test]
        public void SetAllocatedPoints_ShouldRecalculateStats()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];
            _viewModel.SelectedChampion = champion;
            int baseAttack = _viewModel.AttackValue;

            var points = new Dictionary<Subject, int>
            {
                { Subject.Math, 100 },
                { Subject.Science, 50 },
                { Subject.English, 75 },
                { Subject.History, 200 }
            };

            // Act
            _viewModel.SetAllocatedPoints(points);

            // Assert - Math points should increase attack
            Assert.Greater(_viewModel.AttackValue, baseAttack);
        }

        [Test]
        public void ConfirmSelectionCommand_WhenChampionSelected_CanExecute()
        {
            // Arrange
            _viewModel!.SelectedChampion = _viewModel.AllChampions[0];

            // Assert
            Assert.IsTrue(_viewModel.ConfirmSelectionCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmSelectionCommand_WhenNoChampion_CannotExecute()
        {
            // Arrange
            _viewModel!.SelectedChampion = null;

            // Assert
            Assert.IsFalse(_viewModel.ConfirmSelectionCommand.CanExecute(null));
        }

        [Test]
        public void ConfirmSelectionCommand_ShouldRaiseEvent()
        {
            // Arrange
            bool eventFired = false;
            _viewModel!.ChampionConfirmed += (sender, args) => eventFired = true;
            _viewModel.SelectedChampion = _viewModel.AllChampions[0];

            // Act
            _viewModel.ConfirmSelectionCommand.Execute(null);

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
        public void HPBarPercent_ShouldCalculateCorrectly()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];
            _viewModel.SelectedChampion = champion;

            // Assert - should be between 0 and 100
            Assert.GreaterOrEqual(_viewModel.HPBarPercent, 0f);
            Assert.LessOrEqual(_viewModel.HPBarPercent, 100f);
        }

        [Test]
        public void CritChance_ShouldConvertToPercentage()
        {
            // Arrange
            var champion = _viewModel!.AllChampions[0];

            // Act
            _viewModel.SelectedChampion = champion;

            // Assert - champion.criticalChance is 0.15, should display as 15%
            Assert.AreEqual(champion.criticalChance * 100f, _viewModel.CritChance);
        }

        [Test]
        public void IsChampionOwned_ShouldReturnCorrectValue()
        {
            // Arrange
            var ownedChampion = _championService!.OwnedChampions[0];

            // Act
            bool isOwned = _viewModel!.IsChampionOwned(ownedChampion);

            // Assert
            Assert.IsTrue(isOwned);
        }

        [Test]
        public void IsChampionFreeRotation_ShouldReturnCorrectValue()
        {
            // Arrange
            var freeChampion = _championService!.FreeRotationChampions[0];

            // Act
            bool isFree = _viewModel!.IsChampionFreeRotation(freeChampion);

            // Assert
            Assert.IsTrue(isFree);
        }
    }
}
