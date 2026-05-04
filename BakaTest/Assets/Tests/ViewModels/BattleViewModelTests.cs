#nullable enable
using NUnit.Framework;
using BakaTest.ViewModels;
using BakaTest.Services.Battle;
using BakaTest.Services.Localization;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;
using BakaTest.Tests.Mocks;
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Tests.ViewModels
{
    /// <summary>
    /// BattleViewModelのユニットテスト
    /// </summary>
    [TestFixture]
    public class BattleViewModelTests
    {
        private BattleViewModel? _viewModel;
        private IBattleService? _battleService;
        private ILocalizationService? _localizationService;
        private ChampionData? _testChampion1;
        private ChampionData? _testChampion2;

        [SetUp]
        public void SetUp()
        {
            _localizationService = new MockLocalizationService();
            _battleService = new BattleService(_localizationService);
            _viewModel = new BattleViewModel(_battleService, _localizationService, null);

            // Create test champions
            _testChampion1 = CreateTestChampion("test_player", "Player Champion", 1000, 100, 80, 90);
            _testChampion2 = CreateTestChampion("test_opponent", "Opponent Champion", 900, 110, 70, 95);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
            if (_testChampion1 != null) Object.DestroyImmediate(_testChampion1);
            if (_testChampion2 != null) Object.DestroyImmediate(_testChampion2);
        }

        private ChampionData CreateTestChampion(string id, string name, int hp, int attack, int defense, int speed)
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();
            champion.championId = id;
            champion.SetChampionName(Data.Localization.Language.English, name);
            champion.baseStats = new ChampionStats
            {
                HP = hp,
                Attack = attack,
                Defense = defense,
                Speed = speed
            };
            champion.criticalChance = 0.15f;
            champion.dodgeChance = 0.10f;
            champion.element = ElementType.Fire;
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.5f,
                ScienceToDefenseRatio = 0.5f,
                EnglishToSpeedRatio = 0.5f,
                HistoryToHPRatio = 2.0f
            };
            return champion;
        }

        private BattleSetup CreateTestBattleSetup()
        {
            return new BattleSetup(
                mode: BattleMode.Individual,
                player1Champion: _testChampion1!,
                player1Name: "Player1",
                player1Points: new Dictionary<Subject, int>
                {
                    { Subject.Math, 100 },
                    { Subject.Science, 80 },
                    { Subject.English, 60 },
                    { Subject.History, 120 }
                },
                player2Champion: _testChampion2!,
                player2Name: "Player2",
                player2Points: new Dictionary<Subject, int>
                {
                    { Subject.Math, 90 },
                    { Subject.Science, 70 },
                    { Subject.English, 85 },
                    { Subject.History, 110 }
                },
                battleSpeed: 1.0f
            );
        }

        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.IsNotNull(_viewModel);
            Assert.AreEqual(string.Empty, _viewModel!.PlayerName);
            Assert.AreEqual(string.Empty, _viewModel.OpponentName);
            Assert.AreEqual(0, _viewModel.PlayerCurrentHP);
            Assert.AreEqual(0, _viewModel.OpponentCurrentHP);
            Assert.IsFalse(_viewModel.IsResultVisible);
        }

        [Test]
        public void InitializeBattle_ShouldSetPlayerProperties()
        {
            // Arrange
            var setup = CreateTestBattleSetup();

            // Act
            _viewModel!.InitializeBattle(setup);

            // Assert
            Assert.AreEqual("Player1", _viewModel.PlayerName);
            Assert.Greater(_viewModel.PlayerMaxHP, 0);
            Assert.AreEqual(_viewModel.PlayerMaxHP, _viewModel.PlayerCurrentHP);
            Assert.AreEqual(1.0f, _viewModel.PlayerHealthPercent);
        }

        [Test]
        public void InitializeBattle_ShouldSetOpponentProperties()
        {
            // Arrange
            var setup = CreateTestBattleSetup();

            // Act
            _viewModel!.InitializeBattle(setup);

            // Assert
            Assert.AreEqual("Player2", _viewModel.OpponentName);
            Assert.Greater(_viewModel.OpponentMaxHP, 0);
            Assert.AreEqual(_viewModel.OpponentMaxHP, _viewModel.OpponentCurrentHP);
            Assert.AreEqual(1.0f, _viewModel.OpponentHealthPercent);
        }

        [Test]
        public void InitializeBattle_ShouldApplySubjectBonuses()
        {
            // Arrange
            var setup = CreateTestBattleSetup();

            // Act
            _viewModel!.InitializeBattle(setup);

            // Assert
            // Base HP: 1000, History points: 120, Ratio: 2.0 = +240 HP = 1240 total
            Assert.Greater(_viewModel.PlayerMaxHP, 1000);
        }

        [Test]
        public void PropertyChanged_ShouldFireForPlayerCurrentHP()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel!.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(BattleViewModel.PlayerCurrentHP))
                    propertyChanged = true;
            };
            var setup = CreateTestBattleSetup();

            // Act
            _viewModel.InitializeBattle(setup);

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [Test]
        public void PropertyChanged_ShouldFireForOpponentName()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel!.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(BattleViewModel.OpponentName))
                    propertyChanged = true;
            };
            var setup = CreateTestBattleSetup();

            // Act
            _viewModel.InitializeBattle(setup);

            // Assert
            Assert.IsTrue(propertyChanged);
        }

        [Test]
        public void IsResultVisible_ShouldBeFalseInitially()
        {
            // Assert
            Assert.IsFalse(_viewModel!.IsResultVisible);
        }

        [Test]
        public void Dispose_ShouldUnsubscribeFromBattleServiceEvents()
        {
            // Arrange
            var setup = CreateTestBattleSetup();
            _viewModel!.InitializeBattle(setup);

            // Act
            _viewModel.Dispose();

            // Verify disposal doesn't throw
            Assert.DoesNotThrow(() => _battleService!.AbortBattle());
        }
    }
}
