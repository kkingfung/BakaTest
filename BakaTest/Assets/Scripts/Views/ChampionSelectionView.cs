#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.Services.SceneManagement;
using BakaTest.Services.Champions;
using BakaTest.ViewModels;
using BakaTest.Data.Champions;
using System.ComponentModel;
using System.Collections.Generic;

namespace BakaTest.Views
{
    /// <summary>
    /// チャンピオン選択画面のView
    /// </summary>
    /// <remarks>
    /// ChampionSelectionViewModelとUI Toolkitを使用してチャンピオン選択画面を表示します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class ChampionSelectionView : ViewBase<ChampionSelectionViewModel>
    {
        // Filter buttons
        private Button? _filterAllButton;
        private Button? _filterOwnedButton;
        private Button? _filterFreeButton;

        // Champion list
        private ScrollView? _championListScroll;
        private Dictionary<string, VisualElement> _championCards = new();

        // Champion details
        private Label? _championNameLabel;
        private Label? _roleTagLabel;
        private Label? _elementTagLabel;

        // Stat bars
        private VisualElement? _hpBar;
        private VisualElement? _attackBar;
        private VisualElement? _defenseBar;
        private VisualElement? _speedBar;

        // Stat values
        private Label? _hpValueLabel;
        private Label? _attackValueLabel;
        private Label? _defenseValueLabel;
        private Label? _speedValueLabel;

        // Additional stats
        private Label? _critChanceLabel;
        private Label? _dodgeChanceLabel;

        // Abilities
        private ScrollView? _abilitiesScroll;
        private VisualElement? _abilityPlaceholder;

        // Action buttons
        private Button? _selectButton;
        private Button? _backButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービス取得
            var championService = ServiceLocator.Instance.Get<IChampionService>();

            if (championService == null)
            {
                Debug.LogError("[ChampionSelectionView] IChampionService not found in ServiceLocator!");
                return;
            }

            // ViewModelを作成して設定
            SetViewModel(new ChampionSelectionViewModel(championService));
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            // UI要素を取得
            GetUIElements();

            // ViewModelの値でUIを初期化
            UpdateUIFromViewModel();

            // イベントハンドラを登録
            RegisterEventHandlers();

            Debug.Log("[ChampionSelectionView] UI initialized successfully.");
        }

        protected override void BindViewModel(ChampionSelectionViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            // ViewModelのイベントを購読
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
            viewModel.ChampionConfirmed += OnChampionConfirmed;
            viewModel.BackRequested += OnBackRequested;

            Debug.Log("[ChampionSelectionView] ViewModel bound successfully.");
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.ChampionConfirmed -= OnChampionConfirmed;
                ViewModel.BackRequested -= OnBackRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();

            Debug.Log("[ChampionSelectionView] ViewModel unbound.");
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Filter buttons
            _filterAllButton = Root.Q<Button>("FilterAllButton");
            _filterOwnedButton = Root.Q<Button>("FilterOwnedButton");
            _filterFreeButton = Root.Q<Button>("FilterFreeButton");

            // Champion list
            _championListScroll = Root.Q<ScrollView>("ChampionListScroll");

            // Champion details
            _championNameLabel = Root.Q<Label>("ChampionNameLabel");
            _roleTagLabel = Root.Q<Label>("RoleTagLabel");
            _elementTagLabel = Root.Q<Label>("ElementTagLabel");

            // Stat bars
            _hpBar = Root.Q<VisualElement>("HPBar");
            _attackBar = Root.Q<VisualElement>("AttackBar");
            _defenseBar = Root.Q<VisualElement>("DefenseBar");
            _speedBar = Root.Q<VisualElement>("SpeedBar");

            // Stat values
            _hpValueLabel = Root.Q<Label>("HPValueLabel");
            _attackValueLabel = Root.Q<Label>("AttackValueLabel");
            _defenseValueLabel = Root.Q<Label>("DefenseValueLabel");
            _speedValueLabel = Root.Q<Label>("SpeedValueLabel");

            // Additional stats
            _critChanceLabel = Root.Q<Label>("CritChanceLabel");
            _dodgeChanceLabel = Root.Q<Label>("DodgeChanceLabel");

            // Abilities
            _abilitiesScroll = Root.Q<ScrollView>("AbilitiesScroll");
            _abilityPlaceholder = Root.Q<VisualElement>("AbilityPlaceholder");

            // Action buttons
            _selectButton = Root.Q<Button>("SelectButton");
            _backButton = Root.Q<Button>("BackButton");
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Filter buttons
            if (_filterAllButton != null)
                _filterAllButton.clicked += () => ViewModel?.FilterAllCommand.Execute(null);
            if (_filterOwnedButton != null)
                _filterOwnedButton.clicked += () => ViewModel?.FilterOwnedCommand.Execute(null);
            if (_filterFreeButton != null)
                _filterFreeButton.clicked += () => ViewModel?.FilterFreeCommand.Execute(null);

            // Action buttons
            if (_selectButton != null)
                _selectButton.clicked += () => ViewModel?.ConfirmSelectionCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked += () => ViewModel?.BackCommand.Execute(null);
        }

        /// <summary>
        /// イベントハンドラの登録を解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            // Filter buttons
            if (_filterAllButton != null)
                _filterAllButton.clicked -= () => ViewModel?.FilterAllCommand.Execute(null);
            if (_filterOwnedButton != null)
                _filterOwnedButton.clicked -= () => ViewModel?.FilterOwnedCommand.Execute(null);
            if (_filterFreeButton != null)
                _filterFreeButton.clicked -= () => ViewModel?.FilterFreeCommand.Execute(null);

            // Action buttons
            if (_selectButton != null)
                _selectButton.clicked -= () => ViewModel?.ConfirmSelectionCommand.Execute(null);
            if (_backButton != null)
                _backButton.clicked -= () => ViewModel?.BackCommand.Execute(null);
        }

        /// <summary>
        /// ViewModelの値でUIを初期化します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            RebuildChampionList();
            UpdateChampionDetails();
            UpdateFilterButtons();
        }

        /// <summary>
        /// ViewModelのプロパティ変更時の処理
        /// </summary>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.FilteredChampions):
                    RebuildChampionList();
                    break;

                case nameof(ViewModel.SelectedChampion):
                    UpdateChampionSelection();
                    UpdateChampionDetails();
                    break;

                case nameof(ViewModel.ChampionName):
                case nameof(ViewModel.ChampionRole):
                case nameof(ViewModel.ChampionElement):
                case nameof(ViewModel.HPValue):
                case nameof(ViewModel.AttackValue):
                case nameof(ViewModel.DefenseValue):
                case nameof(ViewModel.SpeedValue):
                case nameof(ViewModel.CritChance):
                case nameof(ViewModel.DodgeChance):
                case nameof(ViewModel.HPBarPercent):
                case nameof(ViewModel.AttackBarPercent):
                case nameof(ViewModel.DefenseBarPercent):
                case nameof(ViewModel.SpeedBarPercent):
                    UpdateChampionDetails();
                    break;

                case nameof(ViewModel.IsFilterAllSelected):
                case nameof(ViewModel.IsFilterOwnedSelected):
                case nameof(ViewModel.IsFilterFreeSelected):
                    UpdateFilterButtons();
                    break;
            }
        }

        /// <summary>
        /// チャンピオンリストを再構築します
        /// </summary>
        private void RebuildChampionList()
        {
            if (ViewModel == null || _championListScroll == null) return;

            // 既存のカードをクリア
            _championListScroll.Clear();
            _championCards.Clear();

            // 各チャンピオンのカードを作成
            foreach (var champion in ViewModel.FilteredChampions)
            {
                var card = CreateChampionCard(champion);
                _championListScroll.Add(card);
                _championCards[champion.championId] = card;
            }

            // 選択状態を更新
            UpdateChampionSelection();
        }

        /// <summary>
        /// チャンピオンカードを作成します
        /// </summary>
        private VisualElement CreateChampionCard(ChampionData champion)
        {
            var card = new VisualElement();
            card.AddToClassList("champion-card");

            // Portrait
            var portrait = new VisualElement();
            portrait.AddToClassList("champion-portrait");
            var portraitPlaceholder = new Label("?");
            portraitPlaceholder.AddToClassList("portrait-placeholder");
            portrait.Add(portraitPlaceholder);
            card.Add(portrait);

            // Name
            var nameLabel = new Label(champion.championName);
            nameLabel.AddToClassList("champion-card-name");
            card.Add(nameLabel);

            // Role
            var roleLabel = new Label(champion.role.ToString());
            roleLabel.AddToClassList("champion-card-role");
            card.Add(roleLabel);

            // Status (Owned/Free)
            bool isOwned = ViewModel?.IsChampionOwned(champion) ?? false;
            bool isFree = ViewModel?.IsChampionFreeRotation(champion) ?? false;

            if (isOwned)
            {
                var statusLabel = new Label("OWNED");
                statusLabel.AddToClassList("champion-card-status");
                statusLabel.AddToClassList("owned");
                card.Add(statusLabel);
            }
            else if (isFree)
            {
                var statusLabel = new Label("FREE");
                statusLabel.AddToClassList("champion-card-status");
                statusLabel.AddToClassList("free");
                card.Add(statusLabel);
            }

            // Click handler
            card.RegisterCallback<ClickEvent>(evt => ViewModel?.SelectChampionCommand.Execute(champion));

            return card;
        }

        /// <summary>
        /// チャンピオン選択状態を更新します
        /// </summary>
        private void UpdateChampionSelection()
        {
            if (ViewModel == null) return;

            // すべてのカードからselectedクラスを削除
            foreach (var card in _championCards.Values)
            {
                card.RemoveFromClassList("selected");
            }

            // 選択されたチャンピオンのカードにselectedクラスを追加
            if (ViewModel.SelectedChampion != null && _championCards.ContainsKey(ViewModel.SelectedChampion.championId))
            {
                _championCards[ViewModel.SelectedChampion.championId].AddToClassList("selected");
            }
        }

        /// <summary>
        /// チャンピオン詳細を更新します
        /// </summary>
        private void UpdateChampionDetails()
        {
            if (ViewModel == null) return;

            // Champion info
            if (_championNameLabel != null)
                _championNameLabel.text = ViewModel.ChampionName;
            if (_roleTagLabel != null)
                _roleTagLabel.text = ViewModel.ChampionRole;
            if (_elementTagLabel != null)
                _elementTagLabel.text = ViewModel.ChampionElement;

            // Stat values
            if (_hpValueLabel != null)
                _hpValueLabel.text = ViewModel.HPValue.ToString();
            if (_attackValueLabel != null)
                _attackValueLabel.text = ViewModel.AttackValue.ToString();
            if (_defenseValueLabel != null)
                _defenseValueLabel.text = ViewModel.DefenseValue.ToString();
            if (_speedValueLabel != null)
                _speedValueLabel.text = ViewModel.SpeedValue.ToString();

            // Stat bars
            if (_hpBar != null)
                _hpBar.style.width = Length.Percent(ViewModel.HPBarPercent);
            if (_attackBar != null)
                _attackBar.style.width = Length.Percent(ViewModel.AttackBarPercent);
            if (_defenseBar != null)
                _defenseBar.style.width = Length.Percent(ViewModel.DefenseBarPercent);
            if (_speedBar != null)
                _speedBar.style.width = Length.Percent(ViewModel.SpeedBarPercent);

            // Additional stats
            if (_critChanceLabel != null)
                _critChanceLabel.text = $"{ViewModel.CritChance:F0}%";
            if (_dodgeChanceLabel != null)
                _dodgeChanceLabel.text = $"{ViewModel.DodgeChance:F0}%";

            // Abilities placeholder visibility
            if (_abilityPlaceholder != null)
            {
                _abilityPlaceholder.style.display = ViewModel.SelectedChampion == null
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
        }

        /// <summary>
        /// フィルターボタンを更新します
        /// </summary>
        private void UpdateFilterButtons()
        {
            if (ViewModel == null) return;

            // Remove selected class from all
            _filterAllButton?.RemoveFromClassList("selected");
            _filterOwnedButton?.RemoveFromClassList("selected");
            _filterFreeButton?.RemoveFromClassList("selected");

            // Add selected class to active filter
            if (ViewModel.IsFilterAllSelected)
                _filterAllButton?.AddToClassList("selected");
            else if (ViewModel.IsFilterOwnedSelected)
                _filterOwnedButton?.AddToClassList("selected");
            else if (ViewModel.IsFilterFreeSelected)
                _filterFreeButton?.AddToClassList("selected");
        }

        /// <summary>
        /// チャンピオン確定時の処理
        /// </summary>
        private void OnChampionConfirmed(object? sender, System.EventArgs e)
        {
            Debug.Log("[ChampionSelectionView] Champion confirmed. Loading battle screen...");

            // バトル画面に遷移（実装予定）
            // var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            // sceneService?.LoadBattle();
        }

        /// <summary>
        /// 戻る要求時の処理
        /// </summary>
        private void OnBackRequested(object? sender, System.EventArgs e)
        {
            Debug.Log("[ChampionSelectionView] Back requested. Returning to point allocation...");

            // ポイント配分画面に戻る
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            sceneService?.LoadPointAllocation();
        }
    }
}
