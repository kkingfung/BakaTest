#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Core.MVVM;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;
using BakaTest.Services.Battle;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// バトル画面のViewModel
    /// </summary>
    /// <remarks>
    /// オートバトルのシミュレーション、ダメージ計算、勝敗判定を行います。
    /// </remarks>
    public class BattleViewModel : ViewModelBase
    {
        private readonly IBattleService _battleService;
        private readonly System.Random _random;

        // バトル状態
        private BattleSetup? _battleSetup;
        private BattleUnit? _playerUnit;
        private BattleUnit? _opponentUnit;
        private List<BattleAction> _actionLog;
        private int _currentTurn;
        private float _battleTimer;
        private bool _isBattleActive;
        private DateTime _battleStartTime;

        // プレイヤーステータス
        private string _playerName = string.Empty;
        private int _playerCurrentHP;
        private int _playerMaxHP;
        private float _playerHealthPercent;

        // 対戦相手ステータス
        private string _opponentName = string.Empty;
        private int _opponentCurrentHP;
        private int _opponentMaxHP;
        private float _opponentHealthPercent;

        // バトル情報
        private string _battleTimerText = "00:00";
        private string _battleStatus = "Battle Starting...";
        private bool _isResultVisible;
        private string _resultTitle = string.Empty;
        private string _resultMessage = string.Empty;
        private int _rewardCoins;
        private int _rewardXP;

        // バトルログ
        private List<string> _battleLogMessages = new();

        /// <summary>バトル速度（秒/ターン）</summary>
        public float TurnDuration { get; set; } = 1.5f;

        // プロパティ
        public string PlayerName
        {
            get => _playerName;
            set => SetProperty(ref _playerName, value);
        }

        public int PlayerCurrentHP
        {
            get => _playerCurrentHP;
            set => SetProperty(ref _playerCurrentHP, value);
        }

        public int PlayerMaxHP
        {
            get => _playerMaxHP;
            set => SetProperty(ref _playerMaxHP, value);
        }

        public float PlayerHealthPercent
        {
            get => _playerHealthPercent;
            set => SetProperty(ref _playerHealthPercent, value);
        }

        public string OpponentName
        {
            get => _opponentName;
            set => SetProperty(ref _opponentName, value);
        }

        public int OpponentCurrentHP
        {
            get => _opponentCurrentHP;
            set => SetProperty(ref _opponentCurrentHP, value);
        }

        public int OpponentMaxHP
        {
            get => _opponentMaxHP;
            set => SetProperty(ref _opponentMaxHP, value);
        }

        public float OpponentHealthPercent
        {
            get => _opponentHealthPercent;
            set => SetProperty(ref _opponentHealthPercent, value);
        }

        public string BattleTimerText
        {
            get => _battleTimerText;
            set => SetProperty(ref _battleTimerText, value);
        }

        public string BattleStatus
        {
            get => _battleStatus;
            set => SetProperty(ref _battleStatus, value);
        }

        public bool IsResultVisible
        {
            get => _isResultVisible;
            set => SetProperty(ref _isResultVisible, value);
        }

        public string ResultTitle
        {
            get => _resultTitle;
            set => SetProperty(ref _resultTitle, value);
        }

        public string ResultMessage
        {
            get => _resultMessage;
            set => SetProperty(ref _resultMessage, value);
        }

        public int RewardCoins
        {
            get => _rewardCoins;
            set => SetProperty(ref _rewardCoins, value);
        }

        public int RewardXP
        {
            get => _rewardXP;
            set => SetProperty(ref _rewardXP, value);
        }

        public List<string> BattleLogMessages
        {
            get => _battleLogMessages;
            set => SetProperty(ref _battleLogMessages, value);
        }

        /// <summary>新しいバトルログメッセージが追加されたときに発火</summary>
        public event EventHandler<string>? BattleLogAdded;

        /// <summary>プレイヤーまたは対戦相手のHPが変化したときに発火</summary>
        public event EventHandler? HealthChanged;

        /// <summary>バトルが終了したときに発火</summary>
        public event EventHandler<BattleResult>? BattleCompleted;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleViewModel(IBattleService battleService)
        {
            _battleService = battleService ?? throw new ArgumentNullException(nameof(battleService));
            _random = new System.Random();
            _actionLog = new List<BattleAction>();
        }

        /// <summary>
        /// バトルを初期化して開始します
        /// </summary>
        public void InitializeBattle(BattleSetup setup)
        {
            _battleSetup = setup ?? throw new ArgumentNullException(nameof(setup));

            // BattleUnitを作成
            _playerUnit = setup.CreatePlayer1Unit();
            _opponentUnit = setup.CreatePlayer2Unit();

            // 初期ステータスを設定
            PlayerName = _playerUnit.PlayerName;
            PlayerMaxHP = _playerUnit.MaxHP;
            PlayerCurrentHP = _playerUnit.CurrentHP;
            PlayerHealthPercent = 1.0f;

            OpponentName = _opponentUnit.PlayerName;
            OpponentMaxHP = _opponentUnit.MaxHP;
            OpponentCurrentHP = _opponentUnit.CurrentHP;
            OpponentHealthPercent = 1.0f;

            // バトル状態をリセット
            _currentTurn = 0;
            _battleTimer = 0f;
            _isBattleActive = true;
            _battleStartTime = DateTime.Now;
            _actionLog.Clear();
            _battleLogMessages.Clear();

            BattleStatus = "Battle Start!";
            IsResultVisible = false;

            AddBattleLog($"⚔ Battle Started: {PlayerName} vs {OpponentName}");

            Debug.Log($"[BattleViewModel] Battle initialized: {PlayerName} vs {OpponentName}");
        }

        /// <summary>
        /// バトルを更新します（毎フレーム呼ばれる）
        /// </summary>
        public void UpdateBattle(float deltaTime)
        {
            if (!_isBattleActive || _playerUnit == null || _opponentUnit == null)
                return;

            // タイマー更新
            _battleTimer += deltaTime;
            UpdateBattleTimer();

            // ターン処理（TurnDuration秒ごと）
            if (_battleTimer >= (_currentTurn + 1) * TurnDuration)
            {
                ExecuteTurn();
            }
        }

        /// <summary>
        /// 1ターンを実行します
        /// </summary>
        private void ExecuteTurn()
        {
            _currentTurn++;

            if (_playerUnit == null || _opponentUnit == null)
                return;

            // 速度で行動順を決定
            bool playerActsFirst = _playerUnit.Speed >= _opponentUnit.Speed;

            if (playerActsFirst)
            {
                ExecuteAttack(_playerUnit, _opponentUnit);
                if (_opponentUnit.IsAlive)
                {
                    ExecuteAttack(_opponentUnit, _playerUnit);
                }
            }
            else
            {
                ExecuteAttack(_opponentUnit, _playerUnit);
                if (_playerUnit.IsAlive)
                {
                    ExecuteAttack(_playerUnit, _opponentUnit);
                }
            }

            // 勝敗判定
            CheckBattleEnd();
        }

        /// <summary>
        /// 攻撃を実行します
        /// </summary>
        private void ExecuteAttack(BattleUnit attacker, BattleUnit defender)
        {
            string attackerName = attacker.ChampionData.championName;
            string defenderName = defender.ChampionData.championName;

            // 回避判定
            float dodgeRoll = (float)_random.NextDouble();
            if (dodgeRoll < defender.DodgeChance)
            {
                string dodgeMessage = $"{defenderName} dodged {attackerName}'s attack!";
                AddBattleLog(dodgeMessage);

                var dodgeAction = new BattleAction(
                    BattleActionType.Dodge,
                    attackerName,
                    defenderName,
                    dodgeMessage,
                    _currentTurn
                );
                _actionLog.Add(dodgeAction);
                return;
            }

            // クリティカル判定
            float critRoll = (float)_random.NextDouble();
            bool isCritical = critRoll < attacker.CriticalChance;

            // ダメージ計算
            int baseDamage = attacker.Attack;
            float damageReduction = defender.Defense / (defender.Defense + 100f);
            int finalDamage = (int)(baseDamage * (1f - damageReduction));

            if (isCritical)
            {
                finalDamage = (int)(finalDamage * 1.5f);
            }

            // 最低1ダメージは保証
            finalDamage = Math.Max(1, finalDamage);

            // ダメージを適用
            defender.TakeDamage(finalDamage);

            // ログに追加
            string actionMessage;
            BattleActionType actionType;

            if (isCritical)
            {
                actionMessage = $"💥 CRITICAL! {attackerName} dealt {finalDamage} damage to {defenderName}!";
                actionType = BattleActionType.Critical;
            }
            else
            {
                actionMessage = $"{attackerName} attacked {defenderName} for {finalDamage} damage.";
                actionType = BattleActionType.Attack;
            }

            AddBattleLog(actionMessage);

            var action = new BattleAction(
                actionType,
                attackerName,
                defenderName,
                actionMessage,
                _currentTurn,
                damage: finalDamage
            );
            _actionLog.Add(action);

            // HPを更新
            UpdateHealthDisplay();
        }

        /// <summary>
        /// HP表示を更新します
        /// </summary>
        private void UpdateHealthDisplay()
        {
            if (_playerUnit != null)
            {
                PlayerCurrentHP = _playerUnit.CurrentHP;
                PlayerHealthPercent = _playerUnit.CurrentHP / (float)_playerUnit.MaxHP;
            }

            if (_opponentUnit != null)
            {
                OpponentCurrentHP = _opponentUnit.CurrentHP;
                OpponentHealthPercent = _opponentUnit.CurrentHP / (float)_opponentUnit.MaxHP;
            }

            HealthChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// バトル終了をチェックします
        /// </summary>
        private void CheckBattleEnd()
        {
            if (_playerUnit == null || _opponentUnit == null || _battleSetup == null)
                return;

            if (!_playerUnit.IsAlive || !_opponentUnit.IsAlive)
            {
                _isBattleActive = false;

                BattleUnit? winner = _playerUnit.IsAlive ? _playerUnit : _opponentUnit;
                BattleUnit? loser = _playerUnit.IsAlive ? _opponentUnit : _playerUnit;

                // BattleResultを作成
                var result = new BattleResult(
                    _battleSetup,
                    winner,
                    loser,
                    _actionLog,
                    _battleStartTime,
                    _currentTurn
                );

                // 勝敗メッセージ
                bool playerWon = _playerUnit.IsAlive;
                if (playerWon)
                {
                    ResultTitle = "VICTORY!";
                    ResultMessage = $"You defeated {OpponentName}!";
                    AddBattleLog($"🎉 {PlayerName} wins!");
                }
                else
                {
                    ResultTitle = "DEFEAT";
                    ResultMessage = $"{OpponentName} defeated you!";
                    AddBattleLog($"💀 {OpponentName} wins!");
                }

                // 報酬設定
                RewardCoins = result.CoinsEarned;
                RewardXP = playerWon ? 50 : 10; // 勝利時50XP、敗北時10XP

                // 結果パネルを表示
                IsResultVisible = true;

                // イベント発火
                BattleCompleted?.Invoke(this, result);

                Debug.Log($"[BattleViewModel] Battle ended. Winner: {winner.ChampionData.championName}");
            }
        }

        /// <summary>
        /// バトルタイマーを更新します
        /// </summary>
        private void UpdateBattleTimer()
        {
            TimeSpan elapsed = TimeSpan.FromSeconds(_battleTimer);
            BattleTimerText = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
        }

        /// <summary>
        /// バトルログにメッセージを追加します
        /// </summary>
        private void AddBattleLog(string message)
        {
            _battleLogMessages.Add(message);
            OnPropertyChanged(nameof(BattleLogMessages));
            BattleLogAdded?.Invoke(this, message);
        }

        /// <summary>
        /// アイテムを使用します（手動操作）
        /// </summary>
        public void UseItem(int itemSlotIndex)
        {
            // TODO: アイテムシステムが実装されたら使用処理を追加
            AddBattleLog($"[SYSTEM] Item slot {itemSlotIndex} is empty.");
            Debug.LogWarning($"[BattleViewModel] Item system not yet implemented.");
        }
    }
}
