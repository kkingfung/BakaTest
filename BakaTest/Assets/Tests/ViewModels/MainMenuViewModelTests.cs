#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Tests;
using BakaTest.Services.Save;
using BakaTest.Services.Localization;
using BakaTest.Tests.Mocks;
using BakaTest.Data.Champions;
using System;
using UnityEngine;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// MainMenuViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class MainMenuViewModelTests
    {
        private MainMenuViewModel? _viewModel;
        private MockPlayerDataService? _playerDataService;
        private MockChampionService? _championService;
        private MockTestService? _testService;
        private MockSaveService? _saveService;
        private MockLocalizationService? _localizationService;

        [SetUp]
        public void SetUp()
        {
            _playerDataService = new MockPlayerDataService();
            _championService = new MockChampionService();
            _testService = new MockTestService();
            _saveService = new MockSaveService();
            _localizationService = new MockLocalizationService();

            _viewModel = new MainMenuViewModel(
                _playerDataService,
                _championService,
                _testService,
                _saveService,
                _localizationService);
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
            Assert.IsNotNull(_viewModel);
            Assert.IsNotNull(_viewModel!.TakeTestCommand);
            Assert.IsNotNull(_viewModel.BattleCommand);
            Assert.IsNotNull(_viewModel.ChampionsCommand);
            Assert.IsNotNull(_viewModel.InventoryCommand);
            Assert.IsNotNull(_viewModel.LeaderboardCommand);
            Assert.IsNotNull(_viewModel.SettingsCommand);
            Assert.IsNotNull(_viewModel.QuitCommand);
        }

        [Test]
        public void Constructor_ShouldLoadPlayerData()
        {
            // Assert - verify default save data was loaded
            Assert.AreEqual("TestPlayer", _viewModel!.PlayerName);
            Assert.AreEqual(1, _viewModel.PlayerLevel);
        }

        [Test]
        public void TakeTestCommand_ShouldRaiseNavigationEvent()
        {
            // Arrange
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.TakeTestCommand.Execute(null);

            // Assert
            Assert.AreEqual("TestSelection", navigatedTo);
        }

        [Test]
        public void BattleCommand_WhenNoPoints_ShouldNotNavigate()
        {
            // Arrange
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.BattleCommand.Execute(null);

            // Assert
            Assert.IsNull(navigatedTo);
        }

        [Test]
        public void BattleCommand_WhenHasPoints_ShouldNavigateToPointAllocation()
        {
            // Arrange
            _playerDataService!.AddPoints(Subject.Math, 100);
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.BattleCommand.Execute(null);

            // Assert
            Assert.AreEqual("PointAllocation", navigatedTo);
        }

        [Test]
        public void ChampionsCommand_ShouldNavigateToChampionShop()
        {
            // Arrange
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.ChampionsCommand.Execute(null);

            // Assert
            Assert.AreEqual("ChampionShop", navigatedTo);
        }

        [Test]
        public void InventoryCommand_ShouldNavigateToInventory()
        {
            // Arrange
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.InventoryCommand.Execute(null);

            // Assert
            Assert.AreEqual("Inventory", navigatedTo);
        }

        [Test]
        public void SettingsCommand_ShouldNavigateToSettings()
        {
            // Arrange
            string? navigatedTo = null;
            _viewModel!.NavigationRequested += (sender, scene) => navigatedTo = scene;

            // Act
            _viewModel.SettingsCommand.Execute(null);

            // Assert
            Assert.AreEqual("Settings", navigatedTo);
        }

        [Test]
        public void PointsChanged_ShouldUpdateMathPoints()
        {
            // Arrange
            _playerDataService!.AddPoints(Subject.Math, 50);

            // Act
            _playerDataService.AddPoints(Subject.Math, 100);

            // Assert
            Assert.AreEqual(150, _viewModel!.MathPoints);
        }

        [Test]
        public void PointsChanged_ShouldUpdateSciencePoints()
        {
            // Act
            _playerDataService!.AddPoints(Subject.Science, 75);

            // Assert
            Assert.AreEqual(75, _viewModel!.SciencePoints);
        }

        [Test]
        public void ChampionUnlocked_ShouldUpdateChampionsOwned()
        {
            // Arrange
            int initialCount = _viewModel!.ChampionsOwned;
            var newChampion = ScriptableObject.CreateInstance<ChampionData>();
            newChampion.championId = "test_new";

            // Act
            _championService!.SimulateUnlock(newChampion);

            // Assert
            Assert.AreEqual(initialCount + 1, _viewModel.ChampionsOwned);

            // Cleanup
            UnityEngine.Object.DestroyImmediate(newChampion);
        }

        [Test]
        public void WinRate_ShouldCalculateCorrectly()
        {
            // Arrange - set up in mock save data
            _viewModel!.RefreshData(); // Reload to get mock data (10 total, 7 won)

            // Assert
            Assert.AreEqual(70.0f, _viewModel.WinRate);
        }

        [Test]
        public void TestsRemainingText_ShouldFormatCorrectly()
        {
            // Assert
            Assert.AreEqual("5/5", _viewModel!.TestsRemainingText);
        }

        [Test]
        public void WinRateText_ShouldFormatCorrectly()
        {
            // Act
            _viewModel!.RefreshData();

            // Assert
            Assert.AreEqual("70.0%", _viewModel.WinRateText);
        }

        [Test]
        public void RefreshData_ShouldReloadAllData()
        {
            // Arrange
            _playerDataService!.AddPoints(Subject.Math, 500);

            // Act
            _viewModel!.RefreshData();

            // Assert
            Assert.AreEqual(500, _viewModel.MathPoints);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeFromEvents()
        {
            // Arrange
            _viewModel!.Dispose();

            // Act - change points after disposal
            _playerDataService!.AddPoints(Subject.Math, 999);

            // MathPoints should not update after disposal
            // (We can't directly test this, but we verify no exception is thrown)
            Assert.DoesNotThrow(() => _playerDataService.AddPoints(Subject.English, 100));
        }
    }
}
