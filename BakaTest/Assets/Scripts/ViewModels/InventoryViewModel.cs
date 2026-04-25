#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Items;
using BakaTest.Services.Inventory;
using BakaTest.Services.Player;
using BakaTest.Services.Localization;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// インベントリ画面のViewModel
    /// </summary>
    /// <remarks>
    /// 所有アイテムの閲覧、使用、売却を管理します。
    /// </remarks>
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerDataService _playerDataService;
        private readonly ILocalizationService _localization;

        private List<ItemEntry> _items = new();
        private List<ItemEntry> _filteredItems = new();
        private ItemEntry? _selectedItem;
        private ItemTypeFilter _currentFilter = ItemTypeFilter.All;

        /// <summary>アイテムエントリ（表示用）</summary>
        public class ItemEntry
        {
            public ItemData Data { get; set; } = null!;
            public int Quantity { get; set; }
            public string DisplayName { get; set; } = string.Empty;
            public string DisplayQuantity => $"x{Quantity}";
        }

        /// <summary>フィルター種別</summary>
        public enum ItemTypeFilter
        {
            All,
            Consumable,
            Buff,
            Debuff,
            Special
        }

        public List<ItemEntry> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        public List<ItemEntry> FilteredItems
        {
            get => _filteredItems;
            private set => SetProperty(ref _filteredItems, value);
        }

        public ItemEntry? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    UpdateDisplayProperties();
                }
            }
        }

        public ItemTypeFilter CurrentFilter
        {
            get => _currentFilter;
            private set
            {
                if (SetProperty(ref _currentFilter, value))
                {
                    ApplyFilter();
                    UpdateFilterStates();
                }
            }
        }

        // フィルターボタン状態
        public bool IsFilterAllSelected => CurrentFilter == ItemTypeFilter.All;
        public bool IsFilterConsumableSelected => CurrentFilter == ItemTypeFilter.Consumable;
        public bool IsFilterBuffSelected => CurrentFilter == ItemTypeFilter.Buff;
        public bool IsFilterDebuffSelected => CurrentFilter == ItemTypeFilter.Debuff;
        public bool IsFilterSpecialSelected => CurrentFilter == ItemTypeFilter.Special;

        // プレイヤー情報
        private int _playerCoins;
        public int PlayerCoins
        {
            get => _playerCoins;
            private set => SetProperty(ref _playerCoins, value);
        }

        public string PlayerCoinsText => $"{PlayerCoins} Coins";

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            private set => SetProperty(ref _totalItems, value);
        }

        // アイテム詳細表示
        private string _itemName = "No Item Selected";
        private string _itemDescription = string.Empty;
        private string _itemType = "-";
        private string _itemRarity = "-";
        private int _itemSellPrice;

        public string ItemName
        {
            get => _itemName;
            private set => SetProperty(ref _itemName, value);
        }

        public string ItemDescription
        {
            get => _itemDescription;
            private set => SetProperty(ref _itemDescription, value);
        }

        public string ItemType
        {
            get => _itemType;
            private set => SetProperty(ref _itemType, value);
        }

        public string ItemRarity
        {
            get => _itemRarity;
            private set => SetProperty(ref _itemRarity, value);
        }

        public int ItemSellPrice
        {
            get => _itemSellPrice;
            private set => SetProperty(ref _itemSellPrice, value);
        }

        public string ItemSellPriceText => $"{ItemSellPrice} Coins";

        // Commands
        public RelayCommand<object?> FilterAllCommand { get; }
        public RelayCommand<object?> FilterConsumableCommand { get; }
        public RelayCommand<object?> FilterBuffCommand { get; }
        public RelayCommand<object?> FilterDebuffCommand { get; }
        public RelayCommand<object?> FilterSpecialCommand { get; }
        public RelayCommand<ItemEntry?> SelectItemCommand { get; }
        public RelayCommand<object?> SellCommand { get; }
        public RelayCommand<object?> BackCommand { get; }

        // Events
        public event EventHandler? BackRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InventoryViewModel(
            IInventoryService inventoryService,
            IPlayerDataService playerDataService,
            ILocalizationService localization)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Commands
            FilterAllCommand = new RelayCommand<object?>(_ => ExecuteFilterAll());
            FilterConsumableCommand = new RelayCommand<object?>(_ => ExecuteFilterConsumable());
            FilterBuffCommand = new RelayCommand<object?>(_ => ExecuteFilterBuff());
            FilterDebuffCommand = new RelayCommand<object?>(_ => ExecuteFilterDebuff());
            FilterSpecialCommand = new RelayCommand<object?>(_ => ExecuteFilterSpecial());
            SelectItemCommand = new RelayCommand<ItemEntry?>(ExecuteSelectItem);
            SellCommand = new RelayCommand<object?>(_ => ExecuteSell(), _ => CanExecuteSell());
            BackCommand = new RelayCommand<object?>(_ => ExecuteBack());

            // イベント購読
            _inventoryService.ItemRemoved += OnItemRemoved;
            _inventoryService.ItemSold += OnItemSold;

            // データロード
            LoadItems();
            LoadPlayerData();

            Debug.Log("[InventoryViewModel] Initialized.");
        }

        /// <summary>
        /// アイテムリストをロードします
        /// </summary>
        private void LoadItems()
        {
            var ownedItems = _inventoryService.OwnedItems;
            var itemEntries = new List<ItemEntry>();

            foreach (var kvp in ownedItems)
            {
                var itemData = _inventoryService.GetItemData(kvp.Key);
                if (itemData != null)
                {
                    itemEntries.Add(new ItemEntry
                    {
                        Data = itemData,
                        Quantity = kvp.Value,
                        DisplayName = itemData.GetItemName(_localization.CurrentLanguage)
                    });
                }
            }

            Items = itemEntries;
            TotalItems = ownedItems.Values.Sum();

            ApplyFilter();

            // デフォルトで最初のアイテムを選択
            if (FilteredItems.Count > 0)
            {
                SelectedItem = FilteredItems[0];
            }

            Debug.Log($"[InventoryViewModel] Loaded {Items.Count} item types, {TotalItems} total items");
        }

        /// <summary>
        /// プレイヤーデータをロードします
        /// </summary>
        private void LoadPlayerData()
        {
            PlayerCoins = _playerDataService.BattleCoins;
            OnPropertyChanged(nameof(PlayerCoinsText));
        }

        /// <summary>
        /// フィルターを適用します
        /// </summary>
        private void ApplyFilter()
        {
            var filtered = new List<ItemEntry>(Items);

            switch (CurrentFilter)
            {
                case ItemTypeFilter.Consumable:
                    filtered = filtered.Where(e => e.Data.itemType == Data.Items.ItemType.Consumable).ToList();
                    break;
                case ItemTypeFilter.Buff:
                    filtered = filtered.Where(e => e.Data.itemType == Data.Items.ItemType.Buff).ToList();
                    break;
                case ItemTypeFilter.Debuff:
                    filtered = filtered.Where(e => e.Data.itemType == Data.Items.ItemType.Debuff).ToList();
                    break;
                case ItemTypeFilter.Special:
                    filtered = filtered.Where(e => e.Data.itemType == Data.Items.ItemType.Special).ToList();
                    break;
            }

            FilteredItems = filtered;

            // 選択されたアイテムがフィルター後のリストにない場合、最初のアイテムを選択
            if (SelectedItem != null && !FilteredItems.Contains(SelectedItem))
            {
                SelectedItem = FilteredItems.Count > 0 ? FilteredItems[0] : null;
            }

            Debug.Log($"[InventoryViewModel] Filtered to {FilteredItems.Count} items");
        }

        /// <summary>
        /// 表示プロパティを更新します
        /// </summary>
        private void UpdateDisplayProperties()
        {
            if (SelectedItem == null)
            {
                ItemName = "No Item Selected";
                ItemDescription = string.Empty;
                ItemType = "-";
                ItemRarity = "-";
                ItemSellPrice = 0;
                return;
            }

            ItemName = SelectedItem.Data.GetItemName(_localization.CurrentLanguage);
            ItemDescription = SelectedItem.Data.GetDescription(_localization.CurrentLanguage);
            ItemType = SelectedItem.Data.itemType.ToString();
            ItemRarity = SelectedItem.Data.rarity.ToString();
            ItemSellPrice = SelectedItem.Data.sellPrice;

            OnPropertyChanged(nameof(ItemSellPriceText));
            SellCommand.RaiseCanExecuteChanged();

            Debug.Log($"[InventoryViewModel] Selected: {ItemName}, Quantity: {SelectedItem.Quantity}, Sell Price: {ItemSellPrice}");
        }

        /// <summary>
        /// フィルター状態を更新します
        /// </summary>
        private void UpdateFilterStates()
        {
            OnPropertyChanged(nameof(IsFilterAllSelected));
            OnPropertyChanged(nameof(IsFilterConsumableSelected));
            OnPropertyChanged(nameof(IsFilterBuffSelected));
            OnPropertyChanged(nameof(IsFilterDebuffSelected));
            OnPropertyChanged(nameof(IsFilterSpecialSelected));
        }

        // ===== Command Handlers =====

        private void ExecuteFilterAll() => CurrentFilter = ItemTypeFilter.All;
        private void ExecuteFilterConsumable() => CurrentFilter = ItemTypeFilter.Consumable;
        private void ExecuteFilterBuff() => CurrentFilter = ItemTypeFilter.Buff;
        private void ExecuteFilterDebuff() => CurrentFilter = ItemTypeFilter.Debuff;
        private void ExecuteFilterSpecial() => CurrentFilter = ItemTypeFilter.Special;

        private void ExecuteSelectItem(ItemEntry? item)
        {
            SelectedItem = item;
        }

        private void ExecuteSell()
        {
            if (SelectedItem == null || !CanExecuteSell())
                return;

            Debug.Log($"[InventoryViewModel] Attempting to sell: {ItemName}");

            bool success = _inventoryService.SellItem(SelectedItem.Data.itemId, 1);

            if (success)
            {
                Debug.Log($"[InventoryViewModel] Successfully sold: {ItemName}");
            }
        }

        private bool CanExecuteSell()
        {
            return SelectedItem != null && SelectedItem.Quantity > 0;
        }

        private void ExecuteBack()
        {
            Debug.Log("[InventoryViewModel] Back button clicked");
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        // ===== Event Handlers =====

        private void OnItemRemoved(string itemId, int quantity)
        {
            LoadItems();
            LoadPlayerData();
        }

        private void OnItemSold(string itemId, int quantity)
        {
            LoadItems();
            LoadPlayerData();
        }

        /// <summary>
        /// データを再読み込みします
        /// </summary>
        public void RefreshData()
        {
            LoadItems();
            LoadPlayerData();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inventoryService.ItemRemoved -= OnItemRemoved;
                _inventoryService.ItemSold -= OnItemSold;
                Debug.Log("[InventoryViewModel] Disposed.");
            }
            base.Dispose(disposing);
        }
    }
}
