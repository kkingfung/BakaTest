#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Shop;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Localization;

namespace BakaTest.Views
{
    /// <summary>
    /// チャンピオンショップ画面のView
    /// </summary>
    /// <remarks>
    /// チャンピオンの閲覧、フィルタリング、購入機能を提供します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class ChampionShopView : ViewBase<ChampionShopViewModel>
    {
        // Player Info
        private Label? _playerCoinsLabel;

        // Champion Details
        private Label? _championNameLabel;
        private Label? _championDescriptionLabel;
        private Label? _championRoleLabel;
        private Label? _championElementLabel;
        private Label? _championPriceLabel;
        private Label? _isOwnedLabel;
        private Label? _isFreeRotationLabel;

        // Champion Stats
        private Label? _hpValueLabel;
        private Label? _attackValueLabel;
        private Label? _defenseValueLabel;
        private Label? _speedValueLabel;
        private Label? _critChanceLabel;
        private Label? _dodgeChanceLabel;

        // Filter Buttons
        private Button? _filterAllButton;
        private Button? _filterOwnedButton;
        private Button? _filterUnownedButton;
        private Button? _filterFreeButton;

        // Action Buttons
        private Button? _purchaseButton;
        private Button? _backButton;

        // Champion List (placeholder - will be implemented with ListView or ScrollView)
        // private ListView? _championList;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var shopService = ServiceLocator.Instance.Get<IShopService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();

            if (shopService == null || playerDataService == null || championService == null || localizationService == null)
            {
                Debug.LogError("[ChampionShopView] Required services not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成
            var viewModel = new ChampionShopViewModel(shopService, playerDataService, championService, localizationService);
            SetViewModel(viewModel);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(ChampionShopViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.BackRequested += OnBackRequested;
                viewModel.ChampionPurchasedEvent += OnChampionPurchased;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.BackRequested -= OnBackRequested;
                ViewModel.ChampionPurchasedEvent -= OnChampionPurchased;
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

            // Champion Details
            _championNameLabel = Root.Q<Label>("ChampionNameLabel");
            _championDescriptionLabel = Root.Q<Label>("ChampionDescriptionLabel");
            _championRoleLabel = Root.Q<Label>("ChampionRoleLabel");
            _championElementLabel = Root.Q<Label>("ChampionElementLabel");
            _championPriceLabel = Root.Q<Label>("ChampionPriceLabel");
            _isOwnedLabel = Root.Q<Label>("IsOwnedLabel");
            _isFreeRotationLabel = Root.Q<Label>("IsFreeRotationLabel");

            // Champion Stats
            _hpValueLabel = Root.Q<Label>("HPValueLabel");
            _attackValueLabel = Root.Q<Label>("AttackValueLabel");
            _defenseValueLabel = Root.Q<Label>("DefenseValueLabel");
            _speedValueLabel = Root.Q<Label>("SpeedValueLabel");
            _critChanceLabel = Root.Q<Label>("CritChanceLabel");
            _dodgeChanceLabel = Root.Q<Label>("DodgeChanceLabel");

            // Filter Buttons
            _filterAllButton = Root.Q<Button>("FilterAllButton");
            _filterOwnedButton = Root.Q<Button>("FilterOwnedButton");
            _filterUnownedButton = Root.Q<Button>("FilterUnownedButton");
            _filterFreeButton = Root.Q<Button>("FilterFreeButton");

            // Action Buttons
            _purchaseButton = Root.Q<Button>("PurchaseButton");
            _backButton = Root.Q<Button>("BackButton");

            // Champion List
            // _championList = Root.Q<ListView>("ChampionList");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            if (_filterAllButton != null)
                _filterAllButton.clicked += OnFilterAllClicked;

            if (_filterOwnedButton != null)
                _filterOwnedButton.clicked += OnFilterOwnedClicked;

            if (_filterUnownedButton != null)
                _filterUnownedButton.clicked += OnFilterUnownedClicked;

            if (_filterFreeButton != null)
                _filterFreeButton.clicked += OnFilterFreeClicked;

            if (_purchaseButton != null)
                _purchaseButton.clicked += OnPurchaseClicked;

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

            if (_filterOwnedButton != null)
                _filterOwnedButton.clicked -= OnFilterOwnedClicked;

            if (_filterUnownedButton != null)
                _filterUnownedButton.clicked -= OnFilterUnownedClicked;

            if (_filterFreeButton != null)
                _filterFreeButton.clicked -= OnFilterFreeClicked;

            if (_purchaseButton != null)
                _purchaseButton.clicked -= OnPurchaseClicked;

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

            // Champion Details
            if (_championNameLabel != null) _championNameLabel.text = ViewModel.ChampionName;
            if (_championDescriptionLabel != null) _championDescriptionLabel.text = ViewModel.ChampionDescription;
            if (_championRoleLabel != null) _championRoleLabel.text = ViewModel.ChampionRole;
            if (_championElementLabel != null) _championElementLabel.text = ViewModel.ChampionElement;
            if (_championPriceLabel != null) _championPriceLabel.text = ViewModel.ChampionPriceText;
            if (_isOwnedLabel != null) _isOwnedLabel.text = ViewModel.IsOwned ? "Owned" : "Not Owned";
            if (_isFreeRotationLabel != null) _isFreeRotationLabel.text = ViewModel.IsFreeRotation ? "Free Rotation" : "";

            // Champion Stats
            if (_hpValueLabel != null) _hpValueLabel.text = ViewModel.HPValue.ToString();
            if (_attackValueLabel != null) _attackValueLabel.text = ViewModel.AttackValue.ToString();
            if (_defenseValueLabel != null) _defenseValueLabel.text = ViewModel.DefenseValue.ToString();
            if (_speedValueLabel != null) _speedValueLabel.text = ViewModel.SpeedValue.ToString();
            if (_critChanceLabel != null) _critChanceLabel.text = ViewModel.CritChanceText;
            if (_dodgeChanceLabel != null) _dodgeChanceLabel.text = ViewModel.DodgeChanceText;

            // Update Purchase Button State
            if (_purchaseButton != null)
            {
                _purchaseButton.SetEnabled(ViewModel.CanPurchase);
            }

            // Update Filter Button States (visual indication of selected filter)
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
            // _filterOwnedButton?.EnableInClassList("selected", ViewModel.IsFilterOwnedSelected);
            // _filterUnownedButton?.EnableInClassList("selected", ViewModel.IsFilterUnownedSelected);
            // _filterFreeButton?.EnableInClassList("selected", ViewModel.IsFilterFreeSelected);
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

                case nameof(ViewModel.ChampionName):
                    if (_championNameLabel != null) _championNameLabel.text = ViewModel.ChampionName;
                    break;

                case nameof(ViewModel.ChampionDescription):
                    if (_championDescriptionLabel != null) _championDescriptionLabel.text = ViewModel.ChampionDescription;
                    break;

                case nameof(ViewModel.ChampionRole):
                    if (_championRoleLabel != null) _championRoleLabel.text = ViewModel.ChampionRole;
                    break;

                case nameof(ViewModel.ChampionElement):
                    if (_championElementLabel != null) _championElementLabel.text = ViewModel.ChampionElement;
                    break;

                case nameof(ViewModel.ChampionPrice):
                    if (_championPriceLabel != null) _championPriceLabel.text = ViewModel.ChampionPriceText;
                    break;

                case nameof(ViewModel.IsOwned):
                    if (_isOwnedLabel != null) _isOwnedLabel.text = ViewModel.IsOwned ? "Owned" : "Not Owned";
                    break;

                case nameof(ViewModel.IsFreeRotation):
                    if (_isFreeRotationLabel != null) _isFreeRotationLabel.text = ViewModel.IsFreeRotation ? "Free Rotation" : "";
                    break;

                case nameof(ViewModel.CanPurchase):
                    if (_purchaseButton != null) _purchaseButton.SetEnabled(ViewModel.CanPurchase);
                    break;

                case nameof(ViewModel.HPValue):
                    if (_hpValueLabel != null) _hpValueLabel.text = ViewModel.HPValue.ToString();
                    break;

                case nameof(ViewModel.AttackValue):
                    if (_attackValueLabel != null) _attackValueLabel.text = ViewModel.AttackValue.ToString();
                    break;

                case nameof(ViewModel.DefenseValue):
                    if (_defenseValueLabel != null) _defenseValueLabel.text = ViewModel.DefenseValue.ToString();
                    break;

                case nameof(ViewModel.SpeedValue):
                    if (_speedValueLabel != null) _speedValueLabel.text = ViewModel.SpeedValue.ToString();
                    break;

                case nameof(ViewModel.CritChance):
                    if (_critChanceLabel != null) _critChanceLabel.text = ViewModel.CritChanceText;
                    break;

                case nameof(ViewModel.DodgeChance):
                    if (_dodgeChanceLabel != null) _dodgeChanceLabel.text = ViewModel.DodgeChanceText;
                    break;

                case nameof(ViewModel.IsFilterAllSelected):
                case nameof(ViewModel.IsFilterOwnedSelected):
                case nameof(ViewModel.IsFilterUnownedSelected):
                case nameof(ViewModel.IsFilterFreeSelected):
                    UpdateFilterButtonStates();
                    break;
            }
        }

        private void OnBackRequested(object? sender, EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[ChampionShopView] ISceneManagementService not found.");
                return;
            }

            sceneService.LoadMainMenu();
        }

        private void OnChampionPurchased(object? sender, BakaTest.Data.Champions.ChampionData champion)
        {
            Debug.Log($"[ChampionShopView] Champion purchased: {champion.championId}");
            // TODO: 購入成功時のフィードバック（アニメーション、サウンドなど）
        }

        private void OnFilterAllClicked()
        {
            ViewModel?.FilterAllCommand.Execute(null);
        }

        private void OnFilterOwnedClicked()
        {
            ViewModel?.FilterOwnedCommand.Execute(null);
        }

        private void OnFilterUnownedClicked()
        {
            ViewModel?.FilterUnownedCommand.Execute(null);
        }

        private void OnFilterFreeClicked()
        {
            ViewModel?.FilterFreeCommand.Execute(null);
        }

        private void OnPurchaseClicked()
        {
            ViewModel?.PurchaseCommand.Execute(null);
        }

        private void OnBackClicked()
        {
            ViewModel?.BackCommand.Execute(null);
        }
    }
}
