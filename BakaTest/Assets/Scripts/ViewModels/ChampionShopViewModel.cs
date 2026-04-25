#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Champions;
using BakaTest.Services.Shop;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// チャンピオンショップ画面のViewModel
    /// </summary>
    /// <remarks>
    /// チャンピオンの閲覧、フィルタリング、購入を管理します。
    /// </remarks>
    public class ChampionShopViewModel : ViewModelBase
    {
        private readonly IShopService _shopService;
        private readonly IPlayerDataService _playerDataService;
        private readonly IChampionService _championService;
        private readonly ILocalizationService _localization;

        private List<ChampionData> _allChampions = new();
        private List<ChampionData> _filteredChampions = new();
        private ChampionData? _selectedChampion;
        private ChampionFilter _currentFilter = ChampionFilter.All;
        private ChampionRole? _roleFilter = null;
        private ElementType? _elementFilter = null;

        /// <summary>フィルター種別</summary>
        public enum ChampionFilter
        {
            All,          // すべて
            Owned,        // 所有済み
            Unowned,      // 未所有
            FreeRotation  // 無料ローテーション
        }

        // リスト
        public List<ChampionData> AllChampions
        {
            get => _allChampions;
            private set => SetProperty(ref _allChampions, value);
        }

        public List<ChampionData> FilteredChampions
        {
            get => _filteredChampions;
            private set => SetProperty(ref _filteredChampions, value);
        }

        // 選択されたチャンピオン
        public ChampionData? SelectedChampion
        {
            get => _selectedChampion;
            set
            {
                if (SetProperty(ref _selectedChampion, value))
                {
                    UpdateDisplayProperties();
                }
            }
        }

        // フィルター
        public ChampionFilter CurrentFilter
        {
            get => _currentFilter;
            private set
            {
                if (SetProperty(ref _currentFilter, value))
                {
                    ApplyFilters();
                    UpdateFilterStates();
                }
            }
        }

        public ChampionRole? RoleFilter
        {
            get => _roleFilter;
            set
            {
                if (SetProperty(ref _roleFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public ElementType? ElementFilter
        {
            get => _elementFilter;
            set
            {
                if (SetProperty(ref _elementFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        // フィルターボタン状態
        public bool IsFilterAllSelected => CurrentFilter == ChampionFilter.All;
        public bool IsFilterOwnedSelected => CurrentFilter == ChampionFilter.Owned;
        public bool IsFilterUnownedSelected => CurrentFilter == ChampionFilter.Unowned;
        public bool IsFilterFreeSelected => CurrentFilter == ChampionFilter.FreeRotation;

        // プレイヤー情報
        private int _playerCoins;
        public int PlayerCoins
        {
            get => _playerCoins;
            private set => SetProperty(ref _playerCoins, value);
        }

        public string PlayerCoinsText => $"{PlayerCoins} Coins";

        // チャンピオン詳細表示
        private string _championName = "No Champion Selected";
        private string _championDescription = string.Empty;
        private string _championRole = "-";
        private string _championElement = "-";
        private int _championPrice;
        private bool _isOwned;
        private bool _isFreeRotation;
        private bool _canPurchase;

        // ステータス表示
        private int _hpValue;
        private int _attackValue;
        private int _defenseValue;
        private int _speedValue;
        private float _critChance;
        private float _dodgeChance;

        public string ChampionName
        {
            get => _championName;
            private set => SetProperty(ref _championName, value);
        }

        public string ChampionDescription
        {
            get => _championDescription;
            private set => SetProperty(ref _championDescription, value);
        }

        public string ChampionRole
        {
            get => _championRole;
            private set => SetProperty(ref _championRole, value);
        }

        public string ChampionElement
        {
            get => _championElement;
            private set => SetProperty(ref _championElement, value);
        }

        public int ChampionPrice
        {
            get => _championPrice;
            private set => SetProperty(ref _championPrice, value);
        }

        public string ChampionPriceText => $"{ChampionPrice} Coins";

        public bool IsOwned
        {
            get => _isOwned;
            private set => SetProperty(ref _isOwned, value);
        }

        public bool IsFreeRotation
        {
            get => _isFreeRotation;
            private set => SetProperty(ref _isFreeRotation, value);
        }

        public bool CanPurchase
        {
            get => _canPurchase;
            private set => SetProperty(ref _canPurchase, value);
        }

        public int HPValue
        {
            get => _hpValue;
            private set => SetProperty(ref _hpValue, value);
        }

        public int AttackValue
        {
            get => _attackValue;
            private set => SetProperty(ref _attackValue, value);
        }

        public int DefenseValue
        {
            get => _defenseValue;
            private set => SetProperty(ref _defenseValue, value);
        }

        public int SpeedValue
        {
            get => _speedValue;
            private set => SetProperty(ref _speedValue, value);
        }

        public float CritChance
        {
            get => _critChance;
            private set => SetProperty(ref _critChance, value);
        }

        public float DodgeChance
        {
            get => _dodgeChance;
            private set => SetProperty(ref _dodgeChance, value);
        }

        public string CritChanceText => $"{CritChance * 100f:F1}%";
        public string DodgeChanceText => $"{DodgeChance * 100f:F1}%";

        // Commands
        public RelayCommand<object?> FilterAllCommand { get; }
        public RelayCommand<object?> FilterOwnedCommand { get; }
        public RelayCommand<object?> FilterUnownedCommand { get; }
        public RelayCommand<object?> FilterFreeCommand { get; }
        public RelayCommand<ChampionData?> SelectChampionCommand { get; }
        public RelayCommand<object?> PurchaseCommand { get; }
        public RelayCommand<object?> BackCommand { get; }

        // Events
        public event EventHandler? BackRequested;
        public event EventHandler<ChampionData>? ChampionPurchasedEvent;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChampionShopViewModel(
            IShopService shopService,
            IPlayerDataService playerDataService,
            IChampionService championService,
            ILocalizationService localization)
        {
            _shopService = shopService ?? throw new ArgumentNullException(nameof(shopService));
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            _championService = championService ?? throw new ArgumentNullException(nameof(championService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Commands
            FilterAllCommand = new RelayCommand<object?>(_ => ExecuteFilterAll());
            FilterOwnedCommand = new RelayCommand<object?>(_ => ExecuteFilterOwned());
            FilterUnownedCommand = new RelayCommand<object?>(_ => ExecuteFilterUnowned());
            FilterFreeCommand = new RelayCommand<object?>(_ => ExecuteFilterFree());
            SelectChampionCommand = new RelayCommand<ChampionData?>(ExecuteSelectChampion);
            PurchaseCommand = new RelayCommand<object?>(_ => ExecutePurchase(), _ => CanExecutePurchase());
            BackCommand = new RelayCommand<object?>(_ => ExecuteBack());

            // イベント購読
            _shopService.ChampionPurchased += OnChampionPurchased;
            _shopService.PurchaseFailed += OnPurchaseFailed;

            // データロード
            LoadChampions();
            LoadPlayerData();

            Debug.Log("[ChampionShopViewModel] Initialized.");
        }

        /// <summary>
        /// チャンピオンリストをロードします
        /// </summary>
        private void LoadChampions()
        {
            AllChampions = _shopService.GetAvailableChampions();
            ApplyFilters();

            // デフォルトで最初のチャンピオンを選択
            if (FilteredChampions.Count > 0)
            {
                SelectedChampion = FilteredChampions[0];
            }

            Debug.Log($"[ChampionShopViewModel] Loaded {AllChampions.Count} champions");
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
        private void ApplyFilters()
        {
            var filtered = new List<ChampionData>(AllChampions);

            // 所有状態フィルター
            switch (CurrentFilter)
            {
                case ChampionFilter.Owned:
                    var ownedIds = _championService.OwnedChampions.Select(c => c.championId).ToHashSet();
                    filtered = filtered.Where(c => ownedIds.Contains(c.championId)).ToList();
                    break;

                case ChampionFilter.Unowned:
                    var ownedIds2 = _championService.OwnedChampions.Select(c => c.championId).ToHashSet();
                    filtered = filtered.Where(c => !ownedIds2.Contains(c.championId)).ToList();
                    break;

                case ChampionFilter.FreeRotation:
                    var freeIds = _championService.FreeRotationChampions.Select(c => c.championId).ToHashSet();
                    filtered = filtered.Where(c => freeIds.Contains(c.championId)).ToList();
                    break;
            }

            // ロールフィルター
            if (RoleFilter.HasValue)
            {
                filtered = filtered.Where(c => c.role == RoleFilter.Value).ToList();
            }

            // 属性フィルター
            if (ElementFilter.HasValue)
            {
                filtered = filtered.Where(c => c.element == ElementFilter.Value).ToList();
            }

            FilteredChampions = filtered;

            // 選択されたチャンピオンがフィルター後のリストにない場合、最初のチャンピオンを選択
            if (SelectedChampion != null && !FilteredChampions.Contains(SelectedChampion))
            {
                SelectedChampion = FilteredChampions.Count > 0 ? FilteredChampions[0] : null;
            }

            Debug.Log($"[ChampionShopViewModel] Filtered to {FilteredChampions.Count} champions");
        }

        /// <summary>
        /// 表示プロパティを更新します
        /// </summary>
        private void UpdateDisplayProperties()
        {
            if (SelectedChampion == null)
            {
                ChampionName = "No Champion Selected";
                ChampionDescription = string.Empty;
                ChampionRole = "-";
                ChampionElement = "-";
                ChampionPrice = 0;
                IsOwned = false;
                IsFreeRotation = false;
                CanPurchase = false;
                HPValue = 0;
                AttackValue = 0;
                DefenseValue = 0;
                SpeedValue = 0;
                CritChance = 0;
                DodgeChance = 0;
                return;
            }

            ChampionName = SelectedChampion.GetChampionName(_localization.CurrentLanguage);
            ChampionDescription = SelectedChampion.GetDescription(_localization.CurrentLanguage);
            ChampionRole = SelectedChampion.role.ToString();
            ChampionElement = SelectedChampion.element.ToString();
            ChampionPrice = _shopService.GetChampionPrice(SelectedChampion.championId);

            // 所有状態
            IsOwned = _championService.OwnedChampions.Any(c => c.championId == SelectedChampion.championId);
            IsFreeRotation = _championService.FreeRotationChampions.Any(c => c.championId == SelectedChampion.championId);
            CanPurchase = !IsOwned && _shopService.CanBuyChampion(SelectedChampion.championId);

            // ステータス
            HPValue = (int)SelectedChampion.baseStats.HP;
            AttackValue = (int)SelectedChampion.baseStats.Attack;
            DefenseValue = (int)SelectedChampion.baseStats.Defense;
            SpeedValue = (int)SelectedChampion.baseStats.Speed;
            CritChance = SelectedChampion.criticalChance;
            DodgeChance = SelectedChampion.dodgeChance;

            // 表示テキスト更新
            OnPropertyChanged(nameof(ChampionPriceText));
            OnPropertyChanged(nameof(CritChanceText));
            OnPropertyChanged(nameof(DodgeChanceText));

            // Purchaseボタンの状態を更新
            PurchaseCommand.RaiseCanExecuteChanged();

            Debug.Log($"[ChampionShopViewModel] Selected: {ChampionName}, Price: {ChampionPrice}, Owned: {IsOwned}");
        }

        /// <summary>
        /// フィルター状態を更新します
        /// </summary>
        private void UpdateFilterStates()
        {
            OnPropertyChanged(nameof(IsFilterAllSelected));
            OnPropertyChanged(nameof(IsFilterOwnedSelected));
            OnPropertyChanged(nameof(IsFilterUnownedSelected));
            OnPropertyChanged(nameof(IsFilterFreeSelected));
        }

        // ===== Command Handlers =====

        private void ExecuteFilterAll()
        {
            CurrentFilter = ChampionFilter.All;
        }

        private void ExecuteFilterOwned()
        {
            CurrentFilter = ChampionFilter.Owned;
        }

        private void ExecuteFilterUnowned()
        {
            CurrentFilter = ChampionFilter.Unowned;
        }

        private void ExecuteFilterFree()
        {
            CurrentFilter = ChampionFilter.FreeRotation;
        }

        private void ExecuteSelectChampion(ChampionData? champion)
        {
            SelectedChampion = champion;
        }

        private void ExecutePurchase()
        {
            if (SelectedChampion == null || !CanExecutePurchase())
                return;

            Debug.Log($"[ChampionShopViewModel] Attempting to purchase: {ChampionName}");

            bool success = _shopService.BuyChampion(SelectedChampion.championId);

            if (success)
            {
                Debug.Log($"[ChampionShopViewModel] Successfully purchased: {ChampionName}");
            }
        }

        private bool CanExecutePurchase()
        {
            return SelectedChampion != null && CanPurchase;
        }

        private void ExecuteBack()
        {
            Debug.Log("[ChampionShopViewModel] Back button clicked");
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        // ===== Event Handlers =====

        private void OnChampionPurchased(ChampionData champion)
        {
            // プレイヤーコイン更新
            LoadPlayerData();

            // 所有状態更新
            UpdateDisplayProperties();

            // フィルター再適用（所有済みに移動する場合がある）
            ApplyFilters();

            ChampionPurchasedEvent?.Invoke(this, champion);

            Debug.Log($"[ChampionShopViewModel] Purchase completed: {champion.GetChampionName(_localization.CurrentLanguage)}");
        }

        private void OnPurchaseFailed(string reason)
        {
            Debug.LogWarning($"[ChampionShopViewModel] Purchase failed: {reason}");
            // TODO: UIに失敗メッセージを表示
        }

        /// <summary>
        /// データを再読み込みします
        /// </summary>
        public void RefreshData()
        {
            LoadPlayerData();
            ApplyFilters();
            UpdateDisplayProperties();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _shopService.ChampionPurchased -= OnChampionPurchased;
                _shopService.PurchaseFailed -= OnPurchaseFailed;
                Debug.Log("[ChampionShopViewModel] Disposed.");
            }
            base.Dispose(disposing);
        }
    }
}
