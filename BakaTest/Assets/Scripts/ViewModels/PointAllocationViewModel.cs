#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Champions;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Localization;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// ポイント配分画面のViewModel
    /// </summary>
    /// <remarks>
    /// IPlayerDataServiceとIChampionServiceを使用してポイントの配分とチャンピオンステータスのプレビューを管理します。
    /// </remarks>
    public class PointAllocationViewModel : ViewModelBase
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly IChampionService _championService;
        private readonly ILocalizationService _localization;

        private ChampionData? _selectedChampion;
        
        // Available points
        private int _mathAvailable;
        private int _scienceAvailable;
        private int _englishAvailable;
        private int _historyAvailable;
        
        // Allocated points
        private int _mathAllocated;
        private int _scienceAllocated;
        private int _englishAllocated;
        private int _historyAllocated;
        
        // Base stats
        private int _baseHP;
        private int _baseAttack;
        private int _baseDefense;
        private int _baseSpeed;
        
        // Bonus stats (from allocated points)
        private int _bonusHP;
        private int _bonusAttack;
        private int _bonusDefense;
        private int _bonusSpeed;

        /// <summary>選択されたチャンピオン</summary>
        public ChampionData? SelectedChampion
        {
            get => _selectedChampion;
            set
            {
                if (SetProperty(ref _selectedChampion, value))
                {
                    LoadChampionStats();
                    RecalculateStats();
                }
            }
        }

        // Available Points
        public int MathAvailable
        {
            get => _mathAvailable;
            private set => SetProperty(ref _mathAvailable, value);
        }

        public int ScienceAvailable
        {
            get => _scienceAvailable;
            private set => SetProperty(ref _scienceAvailable, value);
        }

        public int EnglishAvailable
        {
            get => _englishAvailable;
            private set => SetProperty(ref _englishAvailable, value);
        }

        public int HistoryAvailable
        {
            get => _historyAvailable;
            private set => SetProperty(ref _historyAvailable, value);
        }

        // Allocated Points
        public int MathAllocated
        {
            get => _mathAllocated;
            set
            {
                if (SetProperty(ref _mathAllocated, value))
                {
                    RecalculateStats();
                    OnPropertyChanged(nameof(TotalAllocated));
                }
            }
        }

        public int ScienceAllocated
        {
            get => _scienceAllocated;
            set
            {
                if (SetProperty(ref _scienceAllocated, value))
                {
                    RecalculateStats();
                    OnPropertyChanged(nameof(TotalAllocated));
                }
            }
        }

        public int EnglishAllocated
        {
            get => _englishAllocated;
            set
            {
                if (SetProperty(ref _englishAllocated, value))
                {
                    RecalculateStats();
                    OnPropertyChanged(nameof(TotalAllocated));
                }
            }
        }

        public int HistoryAllocated
        {
            get => _historyAllocated;
            set
            {
                if (SetProperty(ref _historyAllocated, value))
                {
                    RecalculateStats();
                    OnPropertyChanged(nameof(TotalAllocated));
                }
            }
        }

        /// <summary>合計配分ポイント</summary>
        public int TotalAllocated => MathAllocated + ScienceAllocated + EnglishAllocated + HistoryAllocated;

        /// <summary>合計利用可能ポイント</summary>
        public int TotalAvailable => MathAvailable + ScienceAvailable + EnglishAvailable + HistoryAvailable;

        // Base Stats
        public int BaseHP
        {
            get => _baseHP;
            private set => SetProperty(ref _baseHP, value);
        }

        public int BaseAttack
        {
            get => _baseAttack;
            private set => SetProperty(ref _baseAttack, value);
        }

        public int BaseDefense
        {
            get => _baseDefense;
            private set => SetProperty(ref _baseDefense, value);
        }

        public int BaseSpeed
        {
            get => _baseSpeed;
            private set => SetProperty(ref _baseSpeed, value);
        }

        // Bonus Stats
        public int BonusHP
        {
            get => _bonusHP;
            private set => SetProperty(ref _bonusHP, value);
        }

        public int BonusAttack
        {
            get => _bonusAttack;
            private set => SetProperty(ref _bonusAttack, value);
        }

        public int BonusDefense
        {
            get => _bonusDefense;
            private set => SetProperty(ref _bonusDefense, value);
        }

        public int BonusSpeed
        {
            get => _bonusSpeed;
            private set => SetProperty(ref _bonusSpeed, value);
        }

        // Final Stats
        public int FinalHP => BaseHP + BonusHP;
        public int FinalAttack => BaseAttack + BonusAttack;
        public int FinalDefense => BaseDefense + BonusDefense;
        public int FinalSpeed => BaseSpeed + BonusSpeed;

        // Display Properties
        public string ChampionName => SelectedChampion?.GetChampionName(_localization.CurrentLanguage) ?? "No Champion Selected";
        public string ChampionRole => SelectedChampion?.role.ToString() ?? "-";
        
        public string TotalAllocatedText => $"{TotalAllocated} / {TotalAvailable}";
        
        public string MathPreviewText => $"→ +{BonusAttack} Attack";
        public string SciencePreviewText => $"→ +{BonusDefense} Defense";
        public string EnglishPreviewText => $"→ +{BonusSpeed} Speed";
        public string HistoryPreviewText => $"→ +{BonusHP} HP";
        
        public string MathAffinityText => $"Math → Attack: {SelectedChampion?.subjectAffinity.MathToAttackRatio ?? 0:F1}x";
        public string ScienceAffinityText => $"Science → Defense: {SelectedChampion?.subjectAffinity.ScienceToDefenseRatio ?? 0:F1}x";
        public string EnglishAffinityText => $"English → Speed: {SelectedChampion?.subjectAffinity.EnglishToSpeedRatio ?? 0:F1}x";
        public string HistoryAffinityText => $"History → HP: {SelectedChampion?.subjectAffinity.HistoryToHPRatio ?? 0:F1}x";

        // Commands
        public RelayCommand<object?> ResetCommand { get; }
        public RelayCommand<object?> ConfirmCommand { get; }
        public RelayCommand<object?> BackCommand { get; }

        // Events
        public event EventHandler? AllocationConfirmed;
        public event EventHandler? BackRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PointAllocationViewModel(IPlayerDataService playerDataService, IChampionService championService, ILocalizationService localization)
        {
            _playerDataService = playerDataService ?? throw new ArgumentNullException(nameof(playerDataService));
            _championService = championService ?? throw new ArgumentNullException(nameof(championService));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Commands
            ResetCommand = new RelayCommand<object?>(_ => ExecuteReset());
            ConfirmCommand = new RelayCommand<object?>(_ => ExecuteConfirm(), _ => CanConfirm());
            BackCommand = new RelayCommand<object?>(_ => ExecuteBack());

            // 利用可能ポイントを読み込み
            LoadAvailablePoints();

            // デフォルトチャンピオンを読み込み（最初の所有チャンピオン）
            LoadDefaultChampion();
        }

        /// <summary>
        /// 利用可能ポイントを読み込みます
        /// </summary>
        private void LoadAvailablePoints()
        {
            MathAvailable = _playerDataService.GetPoints(Subject.Math);
            ScienceAvailable = _playerDataService.GetPoints(Subject.Science);
            EnglishAvailable = _playerDataService.GetPoints(Subject.English);
            HistoryAvailable = _playerDataService.GetPoints(Subject.History);

            OnPropertyChanged(nameof(TotalAvailable));
        }

        /// <summary>
        /// デフォルトチャンピオンを読み込みます
        /// </summary>
        private void LoadDefaultChampion()
        {
            var ownedChampions = _championService.OwnedChampions;
            if (ownedChampions.Count > 0)
            {
                SelectedChampion = ownedChampions[0];
            }
        }

        /// <summary>
        /// チャンピオンの基礎ステータスを読み込みます
        /// </summary>
        private void LoadChampionStats()
        {
            if (SelectedChampion == null)
            {
                BaseHP = 0;
                BaseAttack = 0;
                BaseDefense = 0;
                BaseSpeed = 0;
                return;
            }

            BaseHP = (int)SelectedChampion.baseStats.HP;
            BaseAttack = (int)SelectedChampion.baseStats.Attack;
            BaseDefense = (int)SelectedChampion.baseStats.Defense;
            BaseSpeed = (int)SelectedChampion.baseStats.Speed;

            // Display properties更新
            OnPropertyChanged(nameof(ChampionName));
            OnPropertyChanged(nameof(ChampionRole));
            OnPropertyChanged(nameof(MathAffinityText));
            OnPropertyChanged(nameof(ScienceAffinityText));
            OnPropertyChanged(nameof(EnglishAffinityText));
            OnPropertyChanged(nameof(HistoryAffinityText));
        }

        /// <summary>
        /// ボーナスステータスを再計算します
        /// </summary>
        private void RecalculateStats()
        {
            if (SelectedChampion == null)
            {
                BonusHP = 0;
                BonusAttack = 0;
                BonusDefense = 0;
                BonusSpeed = 0;
                return;
            }

            // Subject affinity を使用してポイントをステータスに変換
            BonusAttack = (int)(MathAllocated * SelectedChampion.subjectAffinity.MathToAttackRatio);
            BonusDefense = (int)(ScienceAllocated * SelectedChampion.subjectAffinity.ScienceToDefenseRatio);
            BonusSpeed = (int)(EnglishAllocated * SelectedChampion.subjectAffinity.EnglishToSpeedRatio);
            BonusHP = (int)(HistoryAllocated * SelectedChampion.subjectAffinity.HistoryToHPRatio);

            // Final stats更新
            OnPropertyChanged(nameof(FinalHP));
            OnPropertyChanged(nameof(FinalAttack));
            OnPropertyChanged(nameof(FinalDefense));
            OnPropertyChanged(nameof(FinalSpeed));

            // Preview text更新
            OnPropertyChanged(nameof(MathPreviewText));
            OnPropertyChanged(nameof(SciencePreviewText));
            OnPropertyChanged(nameof(EnglishPreviewText));
            OnPropertyChanged(nameof(HistoryPreviewText));

            // Confirmボタンの状態を更新
            ConfirmCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// リセット実行
        /// </summary>
        private void ExecuteReset()
        {
            MathAllocated = 0;
            ScienceAllocated = 0;
            EnglishAllocated = 0;
            HistoryAllocated = 0;
        }

        /// <summary>
        /// 確認実行
        /// </summary>
        private void ExecuteConfirm()
        {
            if (!CanConfirm())
                return;

            // ポイントをコミット（PlayerDataServiceから消費）
            var pointsToSpend = new Dictionary<Subject, int>
            {
                { Subject.Math, MathAllocated },
                { Subject.Science, ScienceAllocated },
                { Subject.English, EnglishAllocated },
                { Subject.History, HistoryAllocated }
            };

            _playerDataService.SpendPoints(pointsToSpend);

            // イベント発火
            AllocationConfirmed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 確認可能かどうか
        /// </summary>
        private bool CanConfirm()
        {
            // チャンピオンが選択されている
            if (SelectedChampion == null)
                return false;

            // 少なくとも何らかのポイントが配分されている
            if (TotalAllocated == 0)
                return false;

            // すべての配分が範囲内
            if (MathAllocated > MathAvailable)
                return false;
            if (ScienceAllocated > ScienceAvailable)
                return false;
            if (EnglishAllocated > EnglishAvailable)
                return false;
            if (HistoryAllocated > HistoryAvailable)
                return false;

            return true;
        }

        /// <summary>
        /// 戻る実行
        /// </summary>
        private void ExecuteBack()
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// チャンピオンを設定します（外部から）
        /// </summary>
        public void SetChampion(ChampionData champion)
        {
            SelectedChampion = champion;
        }

        /// <summary>
        /// 配分されたポイントを取得します
        /// </summary>
        public Dictionary<Subject, int> GetAllocatedPoints()
        {
            return new Dictionary<Subject, int>
            {
                { Subject.Math, MathAllocated },
                { Subject.Science, ScienceAllocated },
                { Subject.English, EnglishAllocated },
                { Subject.History, HistoryAllocated }
            };
        }
    }
}
