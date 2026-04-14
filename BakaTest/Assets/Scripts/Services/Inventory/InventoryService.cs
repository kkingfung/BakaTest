#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Items;
using BakaTest.Services.Save;
using BakaTest.Services.Player;

namespace BakaTest.Services.Inventory
{
    /// <summary>
    /// インベントリ管理サービスの実装
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly ISaveService _saveService;
        private readonly IPlayerDataService _playerDataService;
        private readonly Dictionary<string, ItemData> _allItems = new();
        private readonly Dictionary<string, int> _ownedItems = new();

        /// <summary>所有しているアイテムのリスト</summary>
        public Dictionary<string, int> OwnedItems => new Dictionary<string, int>(_ownedItems);

        /// <summary>インベントリの最大容量（0 = 無制限）</summary>
        public int MaxCapacity { get; set; } = 0;

        /// <summary>現在のアイテム総数</summary>
        public int CurrentItemCount => _ownedItems.Values.Sum();

        /// <summary>アイテムが追加されたときに発火するイベント</summary>
        public event Action<string, int>? ItemAdded;

        /// <summary>アイテムが削除されたときに発火するイベント</summary>
        public event Action<string, int>? ItemRemoved;

        /// <summary>アイテムが購入されたときに発火するイベント</summary>
        public event Action<string, int>? ItemPurchased;

        /// <summary>アイテムが売却されたときに発火するイベント</summary>
        public event Action<string, int>? ItemSold;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InventoryService(ISaveService saveService, IPlayerDataService playerDataService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));

            Debug.Log("[InventoryService] Initialized.");
            LoadAllItems();
            LoadData();
        }

        /// <summary>
        /// すべてのアイテムデータをロードします
        /// </summary>
        private void LoadAllItems()
        {
            // Resources/Data/Itemsフォルダからすべてのアイテムをロード
            var items = Resources.LoadAll<ItemData>("Data/Items");

            foreach (var item in items)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemId))
                {
                    _allItems[item.itemId] = item;
                    Debug.Log($"[InventoryService] Loaded item: {item.itemName} ({item.itemId})");
                }
            }

            Debug.Log($"[InventoryService] Total items loaded: {_allItems.Count}");
        }

        /// <summary>
        /// セーブデータからロードします
        /// </summary>
        private void LoadData()
        {
            var saveData = _saveService.CurrentSaveData.inventoryData;

            _ownedItems.Clear();
            foreach (var kvp in saveData.ownedItems)
            {
                if (_allItems.ContainsKey(kvp.Key))
                {
                    _ownedItems[kvp.Key] = kvp.Value;
                    Debug.Log($"[InventoryService] Loaded item: {kvp.Key} x{kvp.Value}");
                }
            }

            Debug.Log($"[InventoryService] Loaded {_ownedItems.Count} item types, {CurrentItemCount} total items");
        }

        /// <summary>
        /// データをセーブデータに書き込みます
        /// </summary>
        private void SaveData()
        {
            var saveData = _saveService.CurrentSaveData.inventoryData;
            saveData.ownedItems = new Dictionary<string, int>(_ownedItems);

            _saveService.MarkDirty();
            Debug.Log("[InventoryService] Data saved to SaveService");
        }

        /// <summary>
        /// アイテムを追加します
        /// </summary>
        public bool AddItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("[InventoryService] Cannot add item: itemId is null or empty");
                return false;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning($"[InventoryService] Cannot add item: quantity must be positive ({quantity})");
                return false;
            }

            if (!_allItems.ContainsKey(itemId))
            {
                Debug.LogWarning($"[InventoryService] Cannot add item: item '{itemId}' not found");
                return false;
            }

            // 容量チェック
            if (MaxCapacity > 0 && CurrentItemCount + quantity > MaxCapacity)
            {
                Debug.LogWarning($"[InventoryService] Cannot add item: inventory full ({CurrentItemCount}/{MaxCapacity})");
                return false;
            }

            // アイテム追加
            if (!_ownedItems.ContainsKey(itemId))
            {
                _ownedItems[itemId] = 0;
            }
            _ownedItems[itemId] += quantity;

            var item = _allItems[itemId];
            Debug.Log($"[InventoryService] Added {quantity}x {item.itemName}. Total: {_ownedItems[itemId]}");

            ItemAdded?.Invoke(itemId, quantity);
            SaveData();

            return true;
        }

        /// <summary>
        /// アイテムを削除します
        /// </summary>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("[InventoryService] Cannot remove item: itemId is null or empty");
                return false;
            }

            if (quantity <= 0)
            {
                Debug.LogWarning($"[InventoryService] Cannot remove item: quantity must be positive ({quantity})");
                return false;
            }

            if (!_ownedItems.ContainsKey(itemId))
            {
                Debug.LogWarning($"[InventoryService] Cannot remove item: item '{itemId}' not owned");
                return false;
            }

            if (_ownedItems[itemId] < quantity)
            {
                Debug.LogWarning($"[InventoryService] Cannot remove item: insufficient quantity (have {_ownedItems[itemId]}, need {quantity})");
                return false;
            }

            // アイテム削除
            _ownedItems[itemId] -= quantity;

            // 0個になったら辞書から削除
            if (_ownedItems[itemId] <= 0)
            {
                _ownedItems.Remove(itemId);
            }

            var item = _allItems[itemId];
            Debug.Log($"[InventoryService] Removed {quantity}x {item.itemName}. Remaining: {_ownedItems.GetValueOrDefault(itemId, 0)}");

            ItemRemoved?.Invoke(itemId, quantity);
            SaveData();

            return true;
        }

        /// <summary>
        /// アイテムを所持しているか確認します
        /// </summary>
        public bool HasItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            return _ownedItems.GetValueOrDefault(itemId, 0) >= quantity;
        }

        /// <summary>
        /// アイテムの所持数を取得します
        /// </summary>
        public int GetItemCount(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return 0;

            return _ownedItems.GetValueOrDefault(itemId, 0);
        }

        /// <summary>
        /// アイテムを購入します
        /// </summary>
        public bool BuyItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("[InventoryService] Cannot buy item: itemId is null or empty");
                return false;
            }

            if (!_allItems.ContainsKey(itemId))
            {
                Debug.LogWarning($"[InventoryService] Cannot buy item: item '{itemId}' not found");
                return false;
            }

            var item = _allItems[itemId];
            int totalCost = item.buyPrice * quantity;

            // コイン残高チェック
            if (_playerDataService.BattleCoins < totalCost)
            {
                Debug.LogWarning($"[InventoryService] Cannot buy item: insufficient coins (have {_playerDataService.BattleCoins}, need {totalCost})");
                return false;
            }

            // コイン消費
            bool spent = _playerDataService.SpendCoins(totalCost);
            if (!spent)
            {
                Debug.LogError("[InventoryService] Failed to spend coins");
                return false;
            }

            // アイテム追加
            bool added = AddItem(itemId, quantity);
            if (!added)
            {
                // 失敗したらコインを返金
                _playerDataService.AddCoins(totalCost);
                Debug.LogError("[InventoryService] Failed to add item, coins refunded");
                return false;
            }

            Debug.Log($"[InventoryService] Purchased {quantity}x {item.itemName} for {totalCost} coins");
            ItemPurchased?.Invoke(itemId, quantity);

            return true;
        }

        /// <summary>
        /// アイテムを売却します
        /// </summary>
        public bool SellItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                Debug.LogWarning("[InventoryService] Cannot sell item: itemId is null or empty");
                return false;
            }

            if (!_allItems.ContainsKey(itemId))
            {
                Debug.LogWarning($"[InventoryService] Cannot sell item: item '{itemId}' not found");
                return false;
            }

            if (!HasItem(itemId, quantity))
            {
                Debug.LogWarning($"[InventoryService] Cannot sell item: insufficient quantity");
                return false;
            }

            var item = _allItems[itemId];
            int totalValue = item.sellPrice * quantity;

            // アイテム削除
            bool removed = RemoveItem(itemId, quantity);
            if (!removed)
            {
                Debug.LogError("[InventoryService] Failed to remove item");
                return false;
            }

            // コイン追加
            _playerDataService.AddCoins(totalValue);

            Debug.Log($"[InventoryService] Sold {quantity}x {item.itemName} for {totalValue} coins");
            ItemSold?.Invoke(itemId, quantity);

            return true;
        }

        /// <summary>
        /// インベントリをクリアします
        /// </summary>
        public void ClearInventory()
        {
            _ownedItems.Clear();
            SaveData();
            Debug.Log("[InventoryService] Inventory cleared");
        }

        /// <summary>
        /// アイテムデータを取得します
        /// </summary>
        public ItemData? GetItemData(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return null;

            return _allItems.GetValueOrDefault(itemId);
        }

        /// <summary>
        /// 全アイテムデータを取得します
        /// </summary>
        public List<ItemData> GetAllItemData()
        {
            return _allItems.Values.ToList();
        }
    }
}
