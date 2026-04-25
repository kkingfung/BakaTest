#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Inventory;
using BakaTest.Services.Player;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Localization;

namespace BakaTest.Views
{
    /// <summary>
    /// インベントリ画面のView
    /// </summary>
    /// <remarks>
    /// 所有アイテムの閲覧、フィルタリング、売却機能を提供します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class InventoryView : ViewBase<InventoryViewModel>
    {
        // Player Info
        private Label? _playerCoinsLabel;
        private Label? _totalItemsLabel;

        // Item Details
        private Label? _itemNameLabel;
        private Label? _itemDescriptionLabel;
        private Label? _itemTypeLabel;
        private Label? _itemRarityLabel;
        private Label? _itemSellPriceLabel;

        // Filter Buttons
        private Button? _filterAllButton;
        private Button? _filterConsumableButton;
        private Button? _filterBuffButton;
        private Button? _filterDebuffButton;
        private Button? _filterSpecialButton;

        // Action Buttons
        private Button? _sellButton;
        private Button? _backButton;

        // Item List (placeholder - will be implemented with ListView or ScrollView)
        // private ListView? _itemList;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (inventoryService == null || playerDataService == null || localizationService == null)
            {
                Debug.LogError("[InventoryView] Required services not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成
            var viewModel = new InventoryViewModel(inventoryService, playerDataService, localizationService);
            SetViewModel(viewModel);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(InventoryViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.BackRequested += OnBackRequested;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.BackRequested -= OnBackRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        private void OnEnable()
        {
            // 画面がアクティブになったらデータを再読み込み
            ViewModel?.RefreshData();
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Player Info
            _playerCoinsLabel = Root.Q<Label>("PlayerCoinsLabel");
            _totalItemsLabel = Root.Q<Label>("TotalItemsLabel");

            // Item Details
            _itemNameLabel = Root.Q<Label>("ItemNameLabel");
            _itemDescriptionLabel = Root.Q<Label>("ItemDescriptionLabel");
            _itemTypeLabel = Root.Q<Label>("ItemTypeLabel");
            _itemRarityLabel = Root.Q<Label>("ItemRarityLabel");
            _itemSellPriceLabel = Root.Q<Label>("ItemSellPriceLabel");

            // Filter Buttons
            _filterAllButton = Root.Q<Button>("FilterAllButton");
            _filterConsumableButton = Root.Q<Button>("FilterConsumableButton");
            _filterBuffButton = Root.Q<Button>("FilterBuffButton");
            _filterDebuffButton = Root.Q<Button>("FilterDebuffButton");
            _filterSpecialButton = Root.Q<Button>("FilterSpecialButton");

            // Action Buttons
            _sellButton = Root.Q<Button>("SellButton");
            _backButton = Root.Q<Button>("BackButton");

            // Item List
            // _itemList = Root.Q<ListView>("ItemList");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (_filterAllButton != null)
                _filterAllButton.clicked += OnFilterAllClicked;

            if (_filterConsumableButton != null)
                _filterConsumableButton.clicked += OnFilterConsumableClicked;

            if (_filterBuffButton != null)
                _filterBuffButton.clicked += OnFilterBuffClicked;

            if (_filterDebuffButton != null)
                _filterDebuffButton.clicked += OnFilterDebuffClicked;

            if (_filterSpecialButton != null)
                _filterSpecialButton.clicked += OnFilterSpecialClicked;

            if (_sellButton != null)
                _sellButton.clicked += OnSellClicked;

            if (_backButton != null)
                _backButton.clicked += OnBackClicked;
        }

        /// <summary>
        /// イベントハンドラを解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            if (_filterAllButton != null)
                _filterAllButton.clicked -= OnFilterAllClicked;

            if (_filterConsumableButton != null)
                _filterConsumableButton.clicked -= OnFilterConsumableClicked;

            if (_filterBuffButton != null)
                _filterBuffButton.clicked -= OnFilterBuffClicked;

            if (_filterDebuffButton != null)
                _filterDebuffButton.clicked -= OnFilterDebuffClicked;

            if (_filterSpecialButton != null)
                _filterSpecialButton.clicked -= OnFilterSpecialClicked;

            if (_sellButton != null)
                _sellButton.clicked -= OnSellClicked;

            if (_backButton != null)
                _backButton.clicked -= OnBackClicked;
        }

        /// <summary>
        /// ViewModelからUIを更新します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            // Player Info
            if (_playerCoinsLabel != null) _playerCoinsLabel.text = ViewModel.PlayerCoinsText;
            if (_totalItemsLabel != null) _totalItemsLabel.text = $"Total Items: {ViewModel.TotalItems}";

            // Item Details
            if (_itemNameLabel != null) _itemNameLabel.text = ViewModel.ItemName;
            if (_itemDescriptionLabel != null) _itemDescriptionLabel.text = ViewModel.ItemDescription;
            if (_itemTypeLabel != null) _itemTypeLabel.text = ViewModel.ItemType;
            if (_itemRarityLabel != null) _itemRarityLabel.text = ViewModel.ItemRarity;
            if (_itemSellPriceLabel != null) _itemSellPriceLabel.text = ViewModel.ItemSellPriceText;

            // Update Sell Button State
            if (_sellButton != null)
            {
                _sellButton.SetEnabled(ViewModel.SelectedItem != null && ViewModel.SelectedItem.Quantity > 0);
            }

            // Update Filter Button States
            UpdateFilterButtonStates();
        }

        /// <summary>
        /// フィルターボタンの状態を更新します
        /// </summary>
        private void UpdateFilterButtonStates()
        {
            if (ViewModel == null) return;

            // フィルターボタンの選択状態を視覚的に表示（CSSクラスの追加/削除など）
            // TODO: USSで "selected" クラスを定義して適用
            // 例:
            // _filterAllButton?.EnableInClassList("selected", ViewModel.IsFilterAllSelected);
            // _filterConsumableButton?.EnableInClassList("selected", ViewModel.IsFilterConsumableSelected);
            // _filterBuffButton?.EnableInClassList("selected", ViewModel.IsFilterBuffSelected);
            // _filterDebuffButton?.EnableInClassList("selected", ViewModel.IsFilterDebuffSelected);
            // _filterSpecialButton?.EnableInClassList("selected", ViewModel.IsFilterSpecialSelected);
        }

        // ===== Event Handlers =====

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.PlayerCoins):
                    if (_playerCoinsLabel != null) _playerCoinsLabel.text = ViewModel.PlayerCoinsText;
                    break;

                case nameof(ViewModel.TotalItems):
                    if (_totalItemsLabel != null) _totalItemsLabel.text = $"Total Items: {ViewModel.TotalItems}";
                    break;

                case nameof(ViewModel.ItemName):
                    if (_itemNameLabel != null) _itemNameLabel.text = ViewModel.ItemName;
                    break;

                case nameof(ViewModel.ItemDescription):
                    if (_itemDescriptionLabel != null) _itemDescriptionLabel.text = ViewModel.ItemDescription;
                    break;

                case nameof(ViewModel.ItemType):
                    if (_itemTypeLabel != null) _itemTypeLabel.text = ViewModel.ItemType;
                    break;

                case nameof(ViewModel.ItemRarity):
                    if (_itemRarityLabel != null) _itemRarityLabel.text = ViewModel.ItemRarity;
                    break;

                case nameof(ViewModel.ItemSellPrice):
                    if (_itemSellPriceLabel != null) _itemSellPriceLabel.text = ViewModel.ItemSellPriceText;
                    break;

                case nameof(ViewModel.SelectedItem):
                    if (_sellButton != null)
                    {
                        _sellButton.SetEnabled(ViewModel.SelectedItem != null && ViewModel.SelectedItem.Quantity > 0);
                    }
                    break;

                case nameof(ViewModel.IsFilterAllSelected):
                case nameof(ViewModel.IsFilterConsumableSelected):
                case nameof(ViewModel.IsFilterBuffSelected):
                case nameof(ViewModel.IsFilterDebuffSelected):
                case nameof(ViewModel.IsFilterSpecialSelected):
                    UpdateFilterButtonStates();
                    break;
            }
        }

        private void OnBackRequested(object? sender, EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[InventoryView] ISceneManagementService not found.");
                return;
            }

            sceneService.LoadMainMenu();
        }

        private void OnFilterAllClicked()
        {
            ViewModel?.FilterAllCommand.Execute(null);
        }

        private void OnFilterConsumableClicked()
        {
            ViewModel?.FilterConsumableCommand.Execute(null);
        }

        private void OnFilterBuffClicked()
        {
            ViewModel?.FilterBuffCommand.Execute(null);
        }

        private void OnFilterDebuffClicked()
        {
            ViewModel?.FilterDebuffCommand.Execute(null);
        }

        private void OnFilterSpecialClicked()
        {
            ViewModel?.FilterSpecialCommand.Execute(null);
        }

        private void OnSellClicked()
        {
            ViewModel?.SellCommand.Execute(null);
        }

        private void OnBackClicked()
        {
            ViewModel?.BackCommand.Execute(null);
        }
    }
}
