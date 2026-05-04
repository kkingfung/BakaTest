#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Services.Player;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// PlayerDataServiceのモック実装（テスト用）
    /// </summary>
    public class MockPlayerDataService : IPlayerDataService
    {
        private readonly Dictionary<Subject, int> _points = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        private readonly Dictionary<Subject, int> _todaysEarnedPoints = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        private int _battleCoins = 0;

        public int BattleCoins => _battleCoins;

        public event Action<Subject, int>? PointsChanged;
        public event Action<int>? CoinsChanged;

        public int GetPoints(Subject subject)
        {
            return _points.GetValueOrDefault(subject, 0);
        }

        public Dictionary<Subject, int> GetAllPoints()
        {
            return new Dictionary<Subject, int>(_points);
        }

        public void AddPoints(Subject subject, int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));

            _points[subject] = _points.GetValueOrDefault(subject, 0) + amount;
            _todaysEarnedPoints[subject] = _todaysEarnedPoints.GetValueOrDefault(subject, 0) + amount;
            PointsChanged?.Invoke(subject, _points[subject]);
        }

        public bool SpendPoints(Dictionary<Subject, int> points)
        {
            // Check if player has enough points
            foreach (var kvp in points)
            {
                if (GetPoints(kvp.Key) < kvp.Value)
                {
                    return false;
                }
            }

            // Spend points
            foreach (var kvp in points)
            {
                _points[kvp.Key] -= kvp.Value;
                PointsChanged?.Invoke(kvp.Key, _points[kvp.Key]);
            }

            return true;
        }

        public int GetTodaysEarnedPoints(Subject subject)
        {
            return _todaysEarnedPoints.GetValueOrDefault(subject, 0);
        }

        public void ResetDailyPoints()
        {
            _todaysEarnedPoints[Subject.Math] = 0;
            _todaysEarnedPoints[Subject.Science] = 0;
            _todaysEarnedPoints[Subject.English] = 0;
            _todaysEarnedPoints[Subject.History] = 0;
        }

        public void AddCoins(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));

            _battleCoins += amount;
            CoinsChanged?.Invoke(_battleCoins);
        }

        public bool SpendCoins(int amount)
        {
            if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));
            if (_battleCoins < amount) return false;

            _battleCoins -= amount;
            CoinsChanged?.Invoke(_battleCoins);
            return true;
        }
    }
}
