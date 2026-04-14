#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Services.Save;

namespace BakaTest.Services.Player
{
    /// <summary>
    /// プレイヤーデータ管理サービスの実装
    /// </summary>
    public class PlayerDataService : IPlayerDataService
    {
        private readonly ISaveService _saveService;
        private readonly Dictionary<Subject, int> _subjectPoints = new();
        private readonly Dictionary<Subject, int> _todaysEarnedPoints = new();
        private int _battleCoins;
        private DateTime _lastResetDate;

        /// <summary>
        /// バトルコインの残高
        /// </summary>
        public int BattleCoins => _battleCoins;

        /// <summary>
        /// ポイントが変更されたときに発火するイベント
        /// </summary>
        public event Action<Subject, int>? PointsChanged;

        /// <summary>
        /// コインが変更されたときに発火するイベント
        /// </summary>
        public event Action<int>? CoinsChanged;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlayerDataService(ISaveService saveService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            Debug.Log("[PlayerDataService] Initialized.");
            LoadData();
        }

        /// <summary>
        /// セーブデータからロードします
        /// </summary>
        private void LoadData()
        {
            var saveData = _saveService.CurrentSaveData.playerData;

            // ポイントをロード
            foreach (Subject subject in Enum.GetValues(typeof(Subject)))
            {
                _subjectPoints[subject] = saveData.points.GetValueOrDefault(subject, 0);
                _todaysEarnedPoints[subject] = saveData.todaysEarnedPoints.GetValueOrDefault(subject, 0);
            }

            // コインをロード
            _battleCoins = saveData.battleCoins;

            // 日付チェック
            if (DateTime.TryParse(saveData.currentDate, out DateTime savedDate))
            {
                _lastResetDate = savedDate;
            }
            else
            {
                _lastResetDate = DateTime.Today;
            }

            // デイリーリセットが必要かチェック
            CheckAndResetDaily();

            Debug.Log($"[PlayerDataService] Loaded from save. Total points: {GetTotalPoints()}, Coins: {_battleCoins}");
        }

        /// <summary>
        /// データをセーブデータに書き込みます
        /// </summary>
        private void SaveData()
        {
            var saveData = _saveService.CurrentSaveData.playerData;

            // ポイントを保存
            saveData.points = new Dictionary<Subject, int>(_subjectPoints);
            saveData.todaysEarnedPoints = new Dictionary<Subject, int>(_todaysEarnedPoints);

            // コインを保存
            saveData.battleCoins = _battleCoins;

            // 日付を保存
            saveData.currentDate = _lastResetDate.ToString("yyyy-MM-dd");

            // SaveServiceに変更を通知
            _saveService.MarkDirty();

            Debug.Log("[PlayerDataService] Data saved to SaveService");
        }

        /// <summary>
        /// 指定教科のテストポイントを取得します
        /// </summary>
        public int GetPoints(Subject subject)
        {
            return _subjectPoints.GetValueOrDefault(subject, 0);
        }

        /// <summary>
        /// すべての教科のポイントを取得します
        /// </summary>
        public Dictionary<Subject, int> GetAllPoints()
        {
            return new Dictionary<Subject, int>(_subjectPoints);
        }

        /// <summary>
        /// ポイントを追加します
        /// </summary>
        public void AddPoints(Subject subject, int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[PlayerDataService] Cannot add negative points: {amount}");
                return;
            }

            CheckAndResetDaily();

            if (!_subjectPoints.ContainsKey(subject))
            {
                _subjectPoints[subject] = 0;
            }

            _subjectPoints[subject] += amount;
            
            if (!_todaysEarnedPoints.ContainsKey(subject))
            {
                _todaysEarnedPoints[subject] = 0;
            }
            _todaysEarnedPoints[subject] += amount;

            Debug.Log($"[PlayerDataService] Added {amount} points to {subject}. New balance: {_subjectPoints[subject]}");

            PointsChanged?.Invoke(subject, _subjectPoints[subject]);

            // セーブデータに保存
            SaveData();
        }

        /// <summary>
        /// ポイントを消費します
        /// </summary>
        public bool SpendPoints(Dictionary<Subject, int> points)
        {
            // まず消費可能かチェック
            foreach (var kvp in points)
            {
                if (kvp.Value < 0)
                {
                    Debug.LogWarning($"[PlayerDataService] Cannot spend negative points: {kvp.Value}");
                    return false;
                }

                var currentPoints = GetPoints(kvp.Key);
                if (currentPoints < kvp.Value)
                {
                    Debug.LogWarning($"[PlayerDataService] Insufficient {kvp.Key} points. Required: {kvp.Value}, Available: {currentPoints}");
                    return false;
                }
            }

            // すべてチェックOKなら消費
            foreach (var kvp in points)
            {
                _subjectPoints[kvp.Key] -= kvp.Value;
                Debug.Log($"[PlayerDataService] Spent {kvp.Value} {kvp.Key} points. Remaining: {_subjectPoints[kvp.Key]}");
                PointsChanged?.Invoke(kvp.Key, _subjectPoints[kvp.Key]);
            }

            // セーブデータに保存
            SaveData();
            return true;
        }

        /// <summary>
        /// 指定教科の今日獲得したポイントを取得します
        /// </summary>
        public int GetTodaysEarnedPoints(Subject subject)
        {
            CheckAndResetDaily();
            return _todaysEarnedPoints.GetValueOrDefault(subject, 0);
        }

        /// <summary>
        /// 今日獲得したポイントをリセットします
        /// </summary>
        public void ResetDailyPoints()
        {
            _todaysEarnedPoints.Clear();
            foreach (Subject subject in Enum.GetValues(typeof(Subject)))
            {
                _todaysEarnedPoints[subject] = 0;
            }
            _lastResetDate = DateTime.Today;
            Debug.Log("[PlayerDataService] Daily points reset.");
        }

        /// <summary>
        /// 日付が変わっていたら自動的にリセットします
        /// </summary>
        private void CheckAndResetDaily()
        {
            if (DateTime.Today > _lastResetDate)
            {
                Debug.Log($"[PlayerDataService] New day detected. Resetting daily points. Last reset: {_lastResetDate}, Today: {DateTime.Today}");
                ResetDailyPoints();
            }
        }

        /// <summary>
        /// バトルコインを追加します
        /// </summary>
        public void AddCoins(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[PlayerDataService] Cannot add negative coins: {amount}");
                return;
            }

            _battleCoins += amount;
            Debug.Log($"[PlayerDataService] Added {amount} coins. New balance: {_battleCoins}");

            CoinsChanged?.Invoke(_battleCoins);

            // セーブデータに保存
            SaveData();
        }

        /// <summary>
        /// バトルコインを消費します
        /// </summary>
        public bool SpendCoins(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning($"[PlayerDataService] Cannot spend negative coins: {amount}");
                return false;
            }

            if (_battleCoins < amount)
            {
                Debug.LogWarning($"[PlayerDataService] Insufficient coins. Required: {amount}, Available: {_battleCoins}");
                return false;
            }

            _battleCoins -= amount;
            Debug.Log($"[PlayerDataService] Spent {amount} coins. Remaining: {_battleCoins}");

            CoinsChanged?.Invoke(_battleCoins);

            // セーブデータに保存
            SaveData();
            return true;
        }

        /// <summary>
        /// 全ポイントの合計を取得します（デバッグ用）
        /// </summary>
        private int GetTotalPoints()
        {
            int total = 0;
            foreach (var points in _subjectPoints.Values)
            {
                total += points;
            }
            return total;
        }
    }
}
