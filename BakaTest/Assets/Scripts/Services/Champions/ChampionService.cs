#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Services.Save;

namespace BakaTest.Services.Champions
{
    /// <summary>
    /// チャンピオン管理サービスの実装
    /// </summary>
    public class ChampionService : IChampionService
    {
        private readonly ISaveService _saveService;
        private readonly Dictionary<string, ChampionData> _allChampions = new();
        private readonly HashSet<string> _ownedChampionIds = new();
        private readonly HashSet<string> _freeRotationChampionIds = new();

        /// <summary>
        /// 所有しているチャンピオンのリスト
        /// </summary>
        public List<ChampionData> OwnedChampions
        {
            get
            {
                return _ownedChampionIds
                    .Select(id => _allChampions.GetValueOrDefault(id))
                    .Where(c => c != null)
                    .Cast<ChampionData>()
                    .ToList();
            }
        }

        /// <summary>
        /// 現在の無料ローテーションチャンピオンのリスト
        /// </summary>
        public List<ChampionData> FreeRotationChampions
        {
            get
            {
                return _freeRotationChampionIds
                    .Select(id => _allChampions.GetValueOrDefault(id))
                    .Where(c => c != null)
                    .Cast<ChampionData>()
                    .ToList();
            }
        }

        /// <summary>
        /// 使用可能な全チャンピオン（所有+無料ローテーション）
        /// </summary>
        public List<ChampionData> AvailableChampions
        {
            get
            {
                var availableIds = new HashSet<string>(_ownedChampionIds);
                availableIds.UnionWith(_freeRotationChampionIds);

                return availableIds
                    .Select(id => _allChampions.GetValueOrDefault(id))
                    .Where(c => c != null)
                    .Cast<ChampionData>()
                    .ToList();
            }
        }

        /// <summary>
        /// 全チャンピオンのリスト（ロック済みを含む）
        /// </summary>
        public List<ChampionData> AllChampions
        {
            get
            {
                return _allChampions.Values.ToList();
            }
        }

        /// <summary>
        /// チャンピオンがアンロックされたときに発火するイベント
        /// </summary>
        public event Action<ChampionData>? ChampionUnlocked;

        /// <summary>
        /// 無料ローテーションが更新されたときに発火するイベント
        /// </summary>
        public event Action? FreeRotationUpdated;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChampionService(ISaveService saveService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            Debug.Log("[ChampionService] Initialized.");
            LoadAllChampions();
            LoadData();
        }

        /// <summary>
        /// すべてのチャンピオンデータをロードします
        /// </summary>
        private void LoadAllChampions()
        {
            // Resources/Data/Championsフォルダからすべてのチャンピオンをロード
            var champions = Resources.LoadAll<ChampionData>("Data/Champions");
            
            foreach (var champion in champions)
            {
                if (champion != null && !string.IsNullOrEmpty(champion.championId))
                {
                    _allChampions[champion.championId] = champion;
                    Debug.Log($"[ChampionService] Loaded champion: {champion.championName} ({champion.championId})");
                }
            }

            Debug.Log($"[ChampionService] Total champions loaded: {_allChampions.Count}");
        }

        /// <summary>
        /// セーブデータからロードします
        /// </summary>
        private void LoadData()
        {
            var saveData = _saveService.CurrentSaveData.championData;

            // 所有チャンピオンをロード
            _ownedChampionIds.Clear();
            foreach (var championId in saveData.ownedChampionIds)
            {
                if (_allChampions.ContainsKey(championId))
                {
                    _ownedChampionIds.Add(championId);
                    Debug.Log($"[ChampionService] Loaded owned: {championId}");
                }
            }

            // 無料ローテーションをロード
            _freeRotationChampionIds.Clear();
            foreach (var championId in saveData.freeRotationChampionIds)
            {
                if (_allChampions.ContainsKey(championId))
                {
                    _freeRotationChampionIds.Add(championId);
                    Debug.Log($"[ChampionService] Loaded free rotation: {championId}");
                }
            }

            // 無料ローテーションが空の場合、デフォルトで最初の4体を設定
            if (_freeRotationChampionIds.Count == 0 && _allChampions.Count >= 4)
            {
                var defaultRotation = _allChampions.Values.Take(4).ToList();
                foreach (var champion in defaultRotation)
                {
                    _freeRotationChampionIds.Add(champion.championId);
                    Debug.Log($"[ChampionService] Default free rotation: {champion.championName}");
                }
                SaveData(); // 初回のデフォルト設定を保存
            }

            Debug.Log($"[ChampionService] Loaded {_ownedChampionIds.Count} owned, {_freeRotationChampionIds.Count} free rotation");
        }

        /// <summary>
        /// データをセーブデータに書き込みます
        /// </summary>
        private void SaveData()
        {
            var saveData = _saveService.CurrentSaveData.championData;

            // 所有チャンピオンを保存
            saveData.ownedChampionIds = new List<string>(_ownedChampionIds);

            // 無料ローテーションを保存
            saveData.freeRotationChampionIds = new List<string>(_freeRotationChampionIds);

            // 最終更新日を保存
            saveData.lastFreeRotationUpdate = DateTime.Now.ToString("yyyy-MM-dd");

            // SaveServiceに変更を通知
            _saveService.MarkDirty();

            Debug.Log("[ChampionService] Data saved to SaveService");
        }

        /// <summary>
        /// チャンピオンIDからチャンピオンデータを取得します
        /// </summary>
        public ChampionData? GetChampion(string championId)
        {
            if (string.IsNullOrEmpty(championId))
            {
                return null;
            }

            return _allChampions.GetValueOrDefault(championId);
        }

        /// <summary>
        /// チャンピオンを所有しているか確認します
        /// </summary>
        public bool IsOwned(string championId)
        {
            return !string.IsNullOrEmpty(championId) && _ownedChampionIds.Contains(championId);
        }

        /// <summary>
        /// チャンピオンが無料ローテーションに含まれているか確認します
        /// </summary>
        public bool IsFreeRotation(string championId)
        {
            return !string.IsNullOrEmpty(championId) && _freeRotationChampionIds.Contains(championId);
        }

        /// <summary>
        /// チャンピオンが使用可能か確認します
        /// </summary>
        public bool IsAvailable(string championId)
        {
            return IsOwned(championId) || IsFreeRotation(championId);
        }

        /// <summary>
        /// チャンピオンをアンロックします
        /// </summary>
        public bool UnlockChampion(string championId)
        {
            if (string.IsNullOrEmpty(championId))
            {
                Debug.LogWarning("[ChampionService] Cannot unlock: championId is null or empty");
                return false;
            }

            if (!_allChampions.ContainsKey(championId))
            {
                Debug.LogWarning($"[ChampionService] Cannot unlock: champion '{championId}' not found");
                return false;
            }

            if (_ownedChampionIds.Contains(championId))
            {
                Debug.LogWarning($"[ChampionService] Champion '{championId}' is already owned");
                return false;
            }

            _ownedChampionIds.Add(championId);
            
            var champion = _allChampions[championId];
            Debug.Log($"[ChampionService] Champion unlocked: {champion.championName} ({championId})");
            
            ChampionUnlocked?.Invoke(champion);

            SaveData();

            return true;
        }

        /// <summary>
        /// 無料ローテーションを更新します
        /// </summary>
        public void UpdateFreeRotation(List<string> championIds)
        {
            _freeRotationChampionIds.Clear();

            foreach (var id in championIds)
            {
                if (_allChampions.ContainsKey(id))
                {
                    _freeRotationChampionIds.Add(id);
                }
            }

            Debug.Log($"[ChampionService] Free rotation updated: {_freeRotationChampionIds.Count} champions");
            FreeRotationUpdated?.Invoke();
            SaveData();
        }
    }
}
