#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Champions;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// チャンピオン選択画面のViewModel
    /// </summary>
    /// <remarks>
    /// IChampionServiceを使用してチャンピオンの選択と管理を行います。
    /// </remarks>
    public class ChampionSelectionViewModel : ViewModelBase
    {
        private readonly IChampionService _championService;
        private readonly ILocalizationService _localization;

        private List<ChampionData> _allChampions = new();
        private List<ChampionData> _filteredChampions = new();
        private ChampionData? _selectedChampion;
        private ChampionFilter _currentFilter = ChampionFilter.All;
        
        // Allocated points (from previous screen)
        private Dictionary<Subject, int> _allocatedPoints = new();

        /// <summary>フィルター種別</summary>
        public enum ChampionFilter
        {
            All,
            Owned,
            FreeRotation
        }

        /// <summary>全チャンピオンリスト</summary>
        public List<ChampionData> AllChampions
        {
            get => _allChampions;
            private set => SetProperty(ref _allChampions, value);
        }

        /// <summary>フィルター済みチャンピオンリスト</summary>
        public List<ChampionData> FilteredChampions
        {
            get => _filteredChampions;
            private set => SetProperty(ref _filteredChampions, value);
        }

        /// <summary>選択されたチャンピオン</summary>
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

        /// <summary>現在のフィルター</summary>
        public ChampionFilter CurrentFilter
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

        // Filter button states
        public bool IsFilterAllSelected => CurrentFilter == ChampionFilter.All;
        public bool IsFilterOwnedSelected => CurrentFilter == ChampionFilter.Owned;
        public bool IsFilterFreeSelected => CurrentFilter == ChampionFilter.FreeRotation;

        // Display Properties
        private string _championName = "No Champion Selected";
        private string _championRole = "-";
        private string _championElement = "-";
        private int _hpValue;
        private int _attackValue;
        private int _defenseValue;
        private int _speedValue;
        private float _critChance;
        private float _dodgeChance;
        private float _hpBarPercent;
        private float _attackBarPercent;
        private float _defenseBarPercent;
        private float _speedBarPercent;

        public string ChampionName
        {
            get => _championName;
            private set => SetProperty(ref _championName, value);
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

        // Stat bar percentages (0-100)
        public float HPBarPercent
        {
            get => _hpBarPercent;
            private set => SetProperty(ref _hpBarPercent, value);
        }

        public float AttackBarPercent
        {
            get => _attackBarPercent;
            private set => SetProperty(ref _attackBarPercent, value);
        }

        public float DefenseBarPercent
        {
            get => _defenseBarPercent;
            private set => SetProperty(ref _defenseBarPercent, value);
        }

        public float SpeedBarPercent
        {
            get => _speedBarPercent;
            private set => SetProperty(ref _speedBarPercent, value);
        }

        // Commands
        public RelayCommand<object?> FilterAllCommand { get; }
        public RelayCommand<object?> FilterOwnedCommand { get; }
        public RelayCommand<object?> FilterFreeCommand { get; }
        public RelayCommand<ChampionData?> SelectChampionCommand { get; }
        public RelayCommand<object?> ConfirmSelectionCommand { get; }
        public RelayCommand<object?> BackCommand { get; }

        // Events
        public event EventHandler? ChampionConfirmed;
        public event EventHandler? BackRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChampionSelectionViewModel(IChampionService championService, ILocalizationService localization)
        {
            _championService = championService ?? throw new ArgumentNullException(nameof(championService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Commands
            FilterAllCommand = new RelayCommand<object?>(_ => ExecuteFilterAll());
            FilterOwnedCommand = new RelayCommand<object?>(_ => ExecuteFilterOwned());
            FilterFreeCommand = new RelayCommand<object?>(_ => ExecuteFilterFree());
            SelectChampionCommand = new RelayCommand<ChampionData?>(ExecuteSelectChampion);
            ConfirmSelectionCommand = new RelayCommand<object?>(_ => ExecuteConfirmSelection(), _ => CanConfirmSelection());
            BackCommand = new RelayCommand<object?>(_ => ExecuteBack());

            // チャンピオンリストを読み込み
            LoadChampions();
        }

        /// <summary>
        /// チャンピオンリストを読み込みます
        /// </summary>
        private void LoadChampions()
        {
            var owned = _championService.OwnedChampions;
            var free = _championService.FreeRotationChampions;

            // 全チャンピオンリストを作成（重複排除）
            var allSet = new HashSet<string>();
            var allList = new List<ChampionData>();

            foreach (var champion in owned)
            {
                if (allSet.Add(champion.championId))
                {
                    allList.Add(champion);
                }
            }

            foreach (var champion in free)
            {
                if (allSet.Add(champion.championId))
                {
                    allList.Add(champion);
                }
            }

            AllChampions = allList;

            // フィルターを適用
            ApplyFilter();

            // デフォルトで最初のチャンピオンを選択
            if (FilteredChampions.Count > 0)
            {
                SelectedChampion = FilteredChampions[0];
            }
        }

        /// <summary>
        /// フィルターを適用します
        /// </summary>
        private void ApplyFilter()
        {
            switch (CurrentFilter)
            {
                case ChampionFilter.All:
                    FilteredChampions = new List<ChampionData>(AllChampions);
                    break;

                case ChampionFilter.Owned:
                    var ownedIds = _championService.OwnedChampions.Select(c => c.championId).ToHashSet();
                    FilteredChampions = AllChampions.Where(c => ownedIds.Contains(c.championId)).ToList();
                    break;

                case ChampionFilter.FreeRotation:
                    var freeIds = _championService.FreeRotationChampions.Select(c => c.championId).ToHashSet();
                    FilteredChampions = AllChampions.Where(c => freeIds.Contains(c.championId)).ToList();
                    break;
            }

            // 選択されたチャンピオンがフィルター後のリストにない場合、最初のチャンピオンを選択
            if (SelectedChampion != null && !FilteredChampions.Contains(SelectedChampion))
            {
                SelectedChampion = FilteredChampions.Count > 0 ? FilteredChampions[0] : null;
            }
        }

        /// <summary>
        /// 表示プロパティを更新します
        /// </summary>
        private void UpdateDisplayProperties()
        {
            if (SelectedChampion == null)
            {
                ChampionName = "No Champion Selected";
                ChampionRole = "-";
                ChampionElement = "-";
                HPValue = 0;
                AttackValue = 0;
                DefenseValue = 0;
                SpeedValue = 0;
                CritChance = 0;
                DodgeChance = 0;
                HPBarPercent = 0;
                AttackBarPercent = 0;
                DefenseBarPercent = 0;
                SpeedBarPercent = 0;
                return;
            }

            ChampionName = SelectedChampion.GetChampionName(_localization.CurrentLanguage);
            ChampionRole = SelectedChampion.role.ToString();
            ChampionElement = SelectedChampion.element.ToString();

            // 配分されたポイントでステータスを計算
            CalculateStatsWithAllocatedPoints();

            // クリティカル・回避確率
            CritChance = SelectedChampion.criticalChance * 100f;
            DodgeChance = SelectedChampion.dodgeChance * 100f;

            // ConfirmSelectionCommandの状態を更新
            ConfirmSelectionCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 配分されたポイントでステータスを計算します
        /// </summary>
        private void CalculateStatsWithAllocatedPoints()
        {
            if (SelectedChampion == null)
                return;

            // Base stats
            int baseHP = (int)SelectedChampion.baseStats.HP;
            int baseAttack = (int)SelectedChampion.baseStats.Attack;
            int baseDefense = (int)SelectedChampion.baseStats.Defense;
            int baseSpeed = (int)SelectedChampion.baseStats.Speed;

            // ボーナス計算（配分されたポイント × 親和性比率）
            int mathPoints = _allocatedPoints.ContainsKey(Subject.Math) ? _allocatedPoints[Subject.Math] : 0;
            int sciencePoints = _allocatedPoints.ContainsKey(Subject.Science) ? _allocatedPoints[Subject.Science] : 0;
            int englishPoints = _allocatedPoints.ContainsKey(Subject.English) ? _allocatedPoints[Subject.English] : 0;
            int historyPoints = _allocatedPoints.ContainsKey(Subject.History) ? _allocatedPoints[Subject.History] : 0;

            int bonusAttack = (int)(mathPoints * SelectedChampion.subjectAffinity.MathToAttackRatio);
            int bonusDefense = (int)(sciencePoints * SelectedChampion.subjectAffinity.ScienceToDefenseRatio);
            int bonusSpeed = (int)(englishPoints * SelectedChampion.subjectAffinity.EnglishToSpeedRatio);
            int bonusHP = (int)(historyPoints * SelectedChampion.subjectAffinity.HistoryToHPRatio);

            // Final stats
            HPValue = baseHP + bonusHP;
            AttackValue = baseAttack + bonusAttack;
            DefenseValue = baseDefense + bonusDefense;
            SpeedValue = baseSpeed + bonusSpeed;

            // Calculate bar percentages (relative to max expected values)
            HPBarPercent = CalculateBarPercent(HPValue, 5000); // Max HP: 5000
            AttackBarPercent = CalculateBarPercent(AttackValue, 500); // Max Attack: 500
            DefenseBarPercent = CalculateBarPercent(DefenseValue, 500); // Max Defense: 500
            SpeedBarPercent = CalculateBarPercent(SpeedValue, 200); // Max Speed: 200
        }

        /// <summary>
        /// バーのパーセンテージを計算します
        /// </summary>
        private float CalculateBarPercent(int value, int maxValue)
        {
            return Math.Min(100f, (value / (float)maxValue) * 100f);
        }

        /// <summary>
        /// フィルター状態を更新します
        /// </summary>
        private void UpdateFilterStates()
        {
            OnPropertyChanged(nameof(IsFilterAllSelected));
            OnPropertyChanged(nameof(IsFilterOwnedSelected));
            OnPropertyChanged(nameof(IsFilterFreeSelected));
        }

        /// <summary>
        /// Allフィルター実行
        /// </summary>
        private void ExecuteFilterAll()
        {
            CurrentFilter = ChampionFilter.All;
        }

        /// <summary>
        /// Ownedフィルター実行
        /// </summary>
        private void ExecuteFilterOwned()
        {
            CurrentFilter = ChampionFilter.Owned;
        }

        /// <summary>
        /// FreeRotationフィルター実行
        /// </summary>
        private void ExecuteFilterFree()
        {
            CurrentFilter = ChampionFilter.FreeRotation;
        }

        /// <summary>
        /// チャンピオン選択実行
        /// </summary>
        private void ExecuteSelectChampion(ChampionData? champion)
        {
            SelectedChampion = champion;
        }

        /// <summary>
        /// 選択確定実行
        /// </summary>
        private void ExecuteConfirmSelection()
        {
            if (!CanConfirmSelection())
                return;

            ChampionConfirmed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 選択確定可能かどうか
        /// </summary>
        private bool CanConfirmSelection()
        {
            return SelectedChampion != null;
        }

        /// <summary>
        /// 戻る実行
        /// </summary>
        private void ExecuteBack()
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 配分されたポイントを設定します（外部から）
        /// </summary>
        public void SetAllocatedPoints(Dictionary<Subject, int> allocatedPoints)
        {
            _allocatedPoints = new Dictionary<Subject, int>(allocatedPoints);
            
            // ステータスを再計算
            if (SelectedChampion != null)
            {
                CalculateStatsWithAllocatedPoints();
            }
        }

        /// <summary>
        /// チャンピオンが所有されているかどうか
        /// </summary>
        public bool IsChampionOwned(ChampionData champion)
        {
            var ownedIds = _championService.OwnedChampions.Select(c => c.championId).ToHashSet();
            return ownedIds.Contains(champion.championId);
        }

        /// <summary>
        /// チャンピオンが無料ローテーションかどうか
        /// </summary>
        public bool IsChampionFreeRotation(ChampionData champion)
        {
            var freeIds = _championService.FreeRotationChampions.Select(c => c.championId).ToHashSet();
            return freeIds.Contains(champion.championId);
        }
    }
}
