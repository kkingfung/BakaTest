#nullable enable
using NUnit.Framework;
using BakaTest.Services.Champions;
using BakaTest.Data.Champions;
using BakaTest.Tests.Mocks;
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Tests.Services
{
    /// <summary>
    /// ChampionServiceのユニットテスト
    /// </summary>
    [TestFixture]
    public class ChampionServiceTests
    {
        private ChampionService? _championService;
        private List<ChampionData> _testChampions = new List<ChampionData>();

        [SetUp]
        public void SetUp()
        {
            // Create test champions
            _testChampions = CreateTestChampions();

            // Initialize champion service
            _championService = new ChampionService(new MockSaveService(), new MockLocalizationService());

            // Manually inject test champions (normally loaded from Resources)
            typeof(ChampionService)
                .GetField("_allChampions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_championService, _testChampions);

            // Initialize owned champions
            typeof(ChampionService)
                .GetField("_ownedChampionIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_championService, new HashSet<string>());

            // Initialize free rotation
            var freeRotation = new List<string> { "test_tank", "test_dps" };
            typeof(ChampionService)
                .GetField("_freeRotationIds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_championService, freeRotation);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test champions
            foreach (var champion in _testChampions)
            {
                Object.DestroyImmediate(champion);
            }
            _testChampions.Clear();
        }

        private List<ChampionData> CreateTestChampions()
        {
            var champions = new List<ChampionData>();

            // Tank champion
            var tank = ScriptableObject.CreateInstance<ChampionData>();
            tank.championId = "test_tank";
            tank.SetChampionName(Data.Localization.Language.English, "Test Tank");
            tank.role = ChampionRole.Tank;
            tank.element = ElementType.Earth;
            tank.baseStats = new ChampionStats { HP = 1200, Attack = 80, Defense = 100, Speed = 60 };
            tank.unlockCost = 1000;
            champions.Add(tank);

            // DPS champion
            var dps = ScriptableObject.CreateInstance<ChampionData>();
            dps.championId = "test_dps";
            dps.SetChampionName(Data.Localization.Language.English, "Test DPS");
            dps.role = ChampionRole.DPS;
            dps.element = ElementType.Fire;
            dps.baseStats = new ChampionStats { HP = 900, Attack = 120, Defense = 70, Speed = 90 };
            dps.unlockCost = 1500;
            champions.Add(dps);

            // Support champion
            var support = ScriptableObject.CreateInstance<ChampionData>();
            support.championId = "test_support";
            support.SetChampionName(Data.Localization.Language.English, "Test Support");
            support.role = ChampionRole.Support;
            support.element = ElementType.Water;
            support.baseStats = new ChampionStats { HP = 1000, Attack = 70, Defense = 80, Speed = 100 };
            support.unlockCost = 1200;
            champions.Add(support);

            return champions;
        }

        [Test]
        public void AllChampions_ShouldReturnAllChampions()
        {
            // Act
            var allChampions = _championService!.AllChampions;

            // Assert
            Assert.AreEqual(3, allChampions.Count);
        }

        [Test]
        public void GetChampion_ValidId_ShouldReturnChampion()
        {
            // Act
            var champion = _championService!.GetChampion("test_tank");

            // Assert
            Assert.IsNotNull(champion);
            Assert.AreEqual("test_tank", champion!.championId);
            Assert.AreEqual(ChampionRole.Tank, champion.role);
        }

        [Test]
        public void GetChampion_InvalidId_ShouldReturnNull()
        {
            // Act
            var champion = _championService!.GetChampion("nonexistent_champion");

            // Assert
            Assert.IsNull(champion);
        }

        [Test]
        public void FreeRotationChampions_ShouldReturnRotationChampions()
        {
            // Act
            var rotation = _championService!.FreeRotationChampions;

            // Assert
            Assert.AreEqual(2, rotation.Count);
            Assert.IsTrue(rotation.Exists(c => c.championId == "test_tank"));
            Assert.IsTrue(rotation.Exists(c => c.championId == "test_dps"));
        }

        [Test]
        public void UnlockChampion_ValidId_ShouldReturnTrue()
        {
            // Act
            var result = _championService!.UnlockChampion("test_support");

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_championService.IsOwned("test_support"));
        }

        [Test]
        public void UnlockChampion_InvalidId_ShouldReturnFalse()
        {
            // Act
            var result = _championService!.UnlockChampion("nonexistent_champion");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void UnlockChampion_AlreadyOwned_ShouldReturnFalse()
        {
            // Arrange
            _championService!.UnlockChampion("test_tank");

            // Act
            var result = _championService.UnlockChampion("test_tank");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void UnlockChampion_ShouldRaiseEvent()
        {
            // Arrange
            ChampionData? unlockedChampion = null;
            _championService!.ChampionUnlocked += (champion) => unlockedChampion = champion;

            // Act
            _championService.UnlockChampion("test_dps");

            // Assert
            Assert.IsNotNull(unlockedChampion);
            Assert.AreEqual("test_dps", unlockedChampion!.championId);
        }

        [Test]
        public void IsOwned_OwnedChampion_ShouldReturnTrue()
        {
            // Arrange
            _championService!.UnlockChampion("test_tank");

            // Act
            var result = _championService.IsOwned("test_tank");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsOwned_NotOwnedChampion_ShouldReturnFalse()
        {
            // Act
            var result = _championService!.IsOwned("test_support");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void OwnedChampions_ShouldReturnOnlyOwnedChampions()
        {
            // Arrange
            _championService!.UnlockChampion("test_tank");
            _championService.UnlockChampion("test_support");

            // Act
            var owned = _championService.OwnedChampions;

            // Assert
            Assert.AreEqual(2, owned.Count);
            Assert.IsTrue(owned.Exists(c => c.championId == "test_tank"));
            Assert.IsTrue(owned.Exists(c => c.championId == "test_support"));
            Assert.IsFalse(owned.Exists(c => c.championId == "test_dps"));
        }

        [Test]
        public void AvailableChampions_ShouldIncludeOwnedAndFreeRotation()
        {
            // Arrange
            _championService!.UnlockChampion("test_support");

            // Act
            var available = _championService.AvailableChampions;

            // Assert
            Assert.AreEqual(3, available.Count); // tank (free), dps (free), support (owned)
            Assert.IsTrue(available.Exists(c => c.championId == "test_tank"));
            Assert.IsTrue(available.Exists(c => c.championId == "test_dps"));
            Assert.IsTrue(available.Exists(c => c.championId == "test_support"));
        }

        [Test]
        public void AvailableChampions_ShouldNotContainDuplicates()
        {
            // Arrange
            _championService!.UnlockChampion("test_tank"); // Already in free rotation

            // Act
            var available = _championService.AvailableChampions;

            // Assert
            var tankCount = available.FindAll(c => c.championId == "test_tank").Count;
            Assert.AreEqual(1, tankCount, "Duplicate champion found in available list");
        }

        [Test]
        public void GetChampionsByRole_ShouldFilterCorrectly()
        {
            // Act
            var tanks = _championService!.AllChampions.FindAll(c => c.role == ChampionRole.Tank);
            var dps = _championService.AllChampions.FindAll(c => c.role == ChampionRole.DPS);

            // Assert
            Assert.AreEqual(1, tanks.Count);
            Assert.AreEqual("test_tank", tanks[0].championId);
            Assert.AreEqual(1, dps.Count);
            Assert.AreEqual("test_dps", dps[0].championId);
        }

        [Test]
        public void GetChampionsByElement_ShouldFilterCorrectly()
        {
            // Act
            var fireChampions = _championService!.AllChampions.FindAll(c => c.element == ElementType.Fire);
            var waterChampions = _championService.AllChampions.FindAll(c => c.element == ElementType.Water);

            // Assert
            Assert.AreEqual(1, fireChampions.Count);
            Assert.AreEqual("test_dps", fireChampions[0].championId);
            Assert.AreEqual(1, waterChampions.Count);
            Assert.AreEqual("test_support", waterChampions[0].championId);
        }
    }
}
