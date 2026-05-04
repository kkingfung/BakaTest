#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Data.Champions;
using BakaTest.Data.Localization;
using BakaTest.Services.Champions;
using UnityEngine;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// ChampionServiceのモック実装（テスト用）
    /// </summary>
    public class MockChampionService : IChampionService
    {
        private readonly List<ChampionData> _ownedChampions = new();
        private readonly List<ChampionData> _freeRotationChampions = new();
        private readonly List<ChampionData> _allChampions = new();

        public List<ChampionData> OwnedChampions => new List<ChampionData>(_ownedChampions);
        public List<ChampionData> FreeRotationChampions => new List<ChampionData>(_freeRotationChampions);
        public List<ChampionData> AllChampions => new List<ChampionData>(_allChampions);

        public List<ChampionData> AvailableChampions
        {
            get
            {
                var available = new HashSet<string>();
                var result = new List<ChampionData>();

                foreach (var champion in _ownedChampions)
                {
                    if (available.Add(champion.championId))
                    {
                        result.Add(champion);
                    }
                }

                foreach (var champion in _freeRotationChampions)
                {
                    if (available.Add(champion.championId))
                    {
                        result.Add(champion);
                    }
                }

                return result;
            }
        }

        public event Action<ChampionData>? ChampionUnlocked;
        public event Action? FreeRotationUpdated;

        public MockChampionService()
        {
            // Create test champions
            var tank = CreateTestChampion("test_tank", "Tank Champion", ChampionRole.Tank, ElementType.Earth);
            var dps = CreateTestChampion("test_dps", "DPS Champion", ChampionRole.DPS, ElementType.Fire);
            var support = CreateTestChampion("test_support", "Support Champion", ChampionRole.Support, ElementType.Water);
            var mage = CreateTestChampion("test_mage", "Mage Champion", ChampionRole.Mage, ElementType.Wind);
            var assassin = CreateTestChampion("test_assassin", "Assassin Champion", ChampionRole.Assassin, ElementType.Fire);

            _allChampions.Add(tank);
            _allChampions.Add(dps);
            _allChampions.Add(support);
            _allChampions.Add(mage);
            _allChampions.Add(assassin);

            // Add some owned champions
            _ownedChampions.Add(tank);
            _ownedChampions.Add(dps);
            _ownedChampions.Add(support);

            // Add some free rotation champions
            _freeRotationChampions.Add(mage);
            _freeRotationChampions.Add(assassin);
        }

        private ChampionData CreateTestChampion(string id, string name, ChampionRole role, ElementType element)
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();
            champion.championId = id;
            champion.SetChampionName(Language.English, name);
            champion.SetChampionName(Language.Japanese, name + "_JP");
            champion.role = role;
            champion.element = element;
            champion.baseStats = new ChampionStats
            {
                HP = 1000,
                Attack = 100,
                Defense = 80,
                Speed = 90
            };
            champion.criticalChance = 0.15f;
            champion.dodgeChance = 0.10f;
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.5f,
                ScienceToDefenseRatio = 0.5f,
                EnglishToSpeedRatio = 0.5f,
                HistoryToHPRatio = 2.0f
            };
            champion.unlockCost = 1000;
            return champion;
        }

        public bool UnlockChampion(string championId)
        {
            var champion = _allChampions.FirstOrDefault(c => c.championId == championId);
            if (champion == null || _ownedChampions.Contains(champion))
            {
                return false;
            }

            _ownedChampions.Add(champion);
            ChampionUnlocked?.Invoke(champion);
            return true;
        }

        public bool IsOwned(string championId)
        {
            return _ownedChampions.Any(c => c.championId == championId);
        }

        public bool IsFreeRotation(string championId)
        {
            return _freeRotationChampions.Any(c => c.championId == championId);
        }

        public bool IsAvailable(string championId)
        {
            return IsOwned(championId) || IsFreeRotation(championId);
        }

        public ChampionData? GetChampion(string championId)
        {
            return _allChampions.FirstOrDefault(c => c.championId == championId);
        }

        public void UpdateFreeRotation(List<string> championIds)
        {
            _freeRotationChampions.Clear();
            foreach (var id in championIds)
            {
                var champion = _allChampions.FirstOrDefault(c => c.championId == id);
                if (champion != null)
                {
                    _freeRotationChampions.Add(champion);
                }
            }
            FreeRotationUpdated?.Invoke();
        }

        /// <summary>
        /// テスト用: チャンピオンアンロックをシミュレート
        /// </summary>
        public void SimulateUnlock(ChampionData champion)
        {
            if (!_allChampions.Contains(champion))
            {
                _allChampions.Add(champion);
            }

            if (!_ownedChampions.Contains(champion))
            {
                _ownedChampions.Add(champion);
                ChampionUnlocked?.Invoke(champion);
            }
        }
    }
}
