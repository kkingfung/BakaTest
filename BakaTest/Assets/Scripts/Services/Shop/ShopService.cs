#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Data.Items;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Inventory;
using BakaTest.Services.Localization;

namespace BakaTest.Services.Shop
{
    /// <summary>
    /// ショップ管理サービスの実装
    /// </summary>
    public class ShopService : IShopService
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly IChampionService _championService;
        private readonly IInventoryService _inventoryService;
        private readonly ILocalizationService _localization;

        private readonly Dictionary<string, ChampionData> _allChampions = new();
        private readonly Dictionary<string, int> _championPrices = new();

        /// <summary>チャンピオン購入時に発火するイベント</summary>
        public event Action<ChampionData>? ChampionPurchased;

        /// <summary>アイテム購入時に発火するイベント</summary>
        public event Action<string, int>? ItemPurchased;

        /// <summary>購入失敗時に発火するイベント</summary>
        public event Action<string>? PurchaseFailed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShopService(
            IPlayerDataService playerDataService,
            IChampionService championService,
            IInventoryService inventoryService,
            ILocalizationService localization)
        {
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            _championService = championService ?? throw new ArgumentNullException(nameof(championService));
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            Debug.Log("[ShopService] Initialized.");
            LoadAllChampions();
            SetupChampionPrices();
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
                    Debug.Log($"[ShopService] Loaded champion: {champion.GetChampionName(_localization.CurrentLanguage)} ({champion.championId})");
                }
            }

            Debug.Log($"[ShopService] Total champions loaded: {_allChampions.Count}");
        }

        /// <summary>
        /// チャンピオンの価格を設定します
        /// </summary>
        private void SetupChampionPrices()
        {
            // ロール別の基本価格
            var basePrices = new Dictionary<ChampionRole, int>
            {
                { ChampionRole.Tank, 500 },
                { ChampionRole.DPS, 600 },
                { ChampionRole.Support, 450 },
                { ChampionRole.Mage, 550 },
                { ChampionRole.Assassin, 700 }
            };

            foreach (var champion in _allChampions.Values)
            {
                int basePrice = basePrices.GetValueOrDefault(champion.role, 500);
                _championPrices[champion.championId] = basePrice;
            }

            Debug.Log($"[ShopService] Champion prices set for {_championPrices.Count} champions");
        }

        /// <summary>
        /// 購入可能なすべてのチャンピオンを取得します
        /// </summary>
        public List<ChampionData> GetAvailableChampions()
        {
            return _allChampions.Values.ToList();
        }

        /// <summary>
        /// 購入可能なすべてのアイテムを取得します
        /// </summary>
        public List<ItemData> GetAvailableItems()
        {
            // InventoryServiceから全アイテムを取得
            return _inventoryService.GetAllItemData();
        }

        /// <summary>
        /// チャンピオンを購入します
        /// </summary>
        public bool BuyChampion(string championId)
        {
            if (string.IsNullOrEmpty(championId))
            {
                Debug.LogWarning("[ShopService] Cannot buy champion: championId is null or empty");
                PurchaseFailed?.Invoke("Invalid champion ID");
                return false;
            }

            if (!_allChampions.ContainsKey(championId))
            {
                Debug.LogWarning($"[ShopService] Cannot buy champion: champion '{championId}' not found");
                PurchaseFailed?.Invoke("Champion not found");
                return false;
            }

            var champion = _allChampions[championId];

            // すでに所有しているかチェック
            if (_championService.OwnedChampions.Any(c => c.championId == championId))
            {
                Debug.LogWarning($"[ShopService] Cannot buy champion: already owned '{championId}'");
                PurchaseFailed?.Invoke("Champion already owned");
                return false;
            }

            int price = GetChampionPrice(championId);

            // コイン残高チェック
            if (_playerDataService.BattleCoins < price)
            {
                Debug.LogWarning($"[ShopService] Cannot buy champion: insufficient coins (have {_playerDataService.BattleCoins}, need {price})");
                PurchaseFailed?.Invoke($"Insufficient coins (need {price})");
                return false;
            }

            // コイン消費
            bool spent = _playerDataService.SpendCoins(price);
            if (!spent)
            {
                Debug.LogError("[ShopService] Failed to spend coins");
                PurchaseFailed?.Invoke("Failed to process payment");
                return false;
            }

            // チャンピオンをアンロック
            bool unlocked = _championService.UnlockChampion(championId);
            if (!unlocked)
            {
                // 失敗したらコインを返金
                _playerDataService.AddCoins(price);
                Debug.LogError("[ShopService] Failed to unlock champion, coins refunded");
                PurchaseFailed?.Invoke("Failed to unlock champion");
                return false;
            }

            Debug.Log($"[ShopService] Purchased champion: {champion.GetChampionName(_localization.CurrentLanguage)} for {price} coins");
            ChampionPurchased?.Invoke(champion);

            return true;
        }

        /// <summary>
        /// アイテムを購入します
        /// </summary>
        public bool BuyItem(string itemId, int quantity = 1)
        {
            // InventoryServiceに委譲
            bool success = _inventoryService.BuyItem(itemId, quantity);

            if (success)
            {
                ItemPurchased?.Invoke(itemId, quantity);
            }
            else
            {
                PurchaseFailed?.Invoke("Failed to purchase item");
            }

            return success;
        }

        /// <summary>
        /// チャンピオンの価格を取得します
        /// </summary>
        public int GetChampionPrice(string championId)
        {
            return _championPrices.GetValueOrDefault(championId, 500);
        }

        /// <summary>
        /// チャンピオンが購入可能かチェックします
        /// </summary>
        public bool CanBuyChampion(string championId)
        {
            if (string.IsNullOrEmpty(championId))
                return false;

            if (!_allChampions.ContainsKey(championId))
                return false;

            // すでに所有している場合は購入不可
            if (_championService.OwnedChampions.Any(c => c.championId == championId))
                return false;

            int price = GetChampionPrice(championId);
            return _playerDataService.BattleCoins >= price;
        }

        /// <summary>
        /// アイテムが購入可能かチェックします
        /// </summary>
        public bool CanBuyItem(string itemId, int quantity = 1)
        {
            var itemData = _inventoryService.GetItemData(itemId);
            if (itemData == null)
                return false;

            int totalCost = itemData.buyPrice * quantity;
            return _playerDataService.BattleCoins >= totalCost;
        }
    }
}
