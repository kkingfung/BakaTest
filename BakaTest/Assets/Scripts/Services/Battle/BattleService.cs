#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;

namespace BakaTest.Services.Battle
{
    /// <summary>
    /// バトル管理サービスの実装
    /// </summary>
    public class BattleService : IBattleService
    {
        private BattleResult? _currentBattle;
        private BattleSetup? _currentSetup;
        private BattleUnit? _player1Unit;
        private BattleUnit? _player2Unit;
        private List<BattleAction> _actionLog = new List<BattleAction>();
        private DateTime _battleStartTime;
        private int _turnCount;

        private readonly System.Random _random = new System.Random();

        /// <summary>
        /// 現在進行中のバトル結果
        /// </summary>
        public BattleResult? CurrentBattle => _currentBattle;

        /// <summary>
        /// バトルが進行中かどうか
        /// </summary>
        public bool IsBattleInProgress => _currentBattle != null && _player1Unit != null && _player2Unit != null;

        /// <summary>
        /// プレイヤー1のバトルユニット（現在のHP含む）
        /// </summary>
        public BattleUnit? Player1Unit => _player1Unit;

        /// <summary>
        /// プレイヤー2のバトルユニット（現在のHP含む）
        /// </summary>
        public BattleUnit? Player2Unit => _player2Unit;

        /// <summary>
        /// 現在のターン数
        /// </summary>
        public int CurrentTurn => _turnCount;

        /// <summary>
        /// バトルが開始されたときに発火するイベント
        /// </summary>
        public event Action<BattleSetup>? BattleStarted;

        /// <summary>
        /// バトルアクションが発生したときに発火するイベント
        /// </summary>
        public event Action<BattleAction>? ActionPerformed;

        /// <summary>
        /// バトルが終了したときに発火するイベント
        /// </summary>
        public event Action<BattleResult>? BattleEnded;

        /// <summary>
        /// バトルを開始します
        /// </summary>
        public void StartBattle(BattleSetup setup)
        {
            if (IsBattleInProgress)
            {
                Debug.LogWarning("[BattleService] Battle already in progress!");
                return;
            }

            _currentSetup = setup ?? throw new ArgumentNullException(nameof(setup));
            _player1Unit = setup.CreatePlayer1Unit();
            _player2Unit = setup.CreatePlayer2Unit();
            _actionLog = new List<BattleAction>();
            _battleStartTime = DateTime.Now;
            _turnCount = 0;

            Debug.Log($"[BattleService] Battle started: {_player1Unit.ChampionData.championName} vs {_player2Unit.ChampionData.championName}");
            BattleStarted?.Invoke(setup);
        }

        /// <summary>
        /// バトルをシミュレーション実行します
        /// </summary>
        public BattleResult? SimulateBattle()
        {
            if (!IsBattleInProgress || _player1Unit == null || _player2Unit == null || _currentSetup == null)
            {
                Debug.LogError("[BattleService] Cannot simulate battle: no battle in progress!");
                return null;
            }

            // 速度で先攻を決定
            BattleUnit faster = _player1Unit.Speed >= _player2Unit.Speed ? _player1Unit : _player2Unit;
            BattleUnit slower = _player1Unit.Speed >= _player2Unit.Speed ? _player2Unit : _player1Unit;

            Debug.Log($"[BattleService] {faster.ChampionData.championName} (SPD: {faster.Speed}) attacks first!");

            // 両者が生存している限りループ
            int maxTurns = 100; // 無限ループ防止
            while (_player1Unit.IsAlive && _player2Unit.IsAlive && _turnCount < maxTurns)
            {
                _turnCount++;

                // 先攻ユニットの攻撃
                PerformAttack(faster, slower);
                if (!slower.IsAlive)
                    break;

                // 後攻ユニットの反撃
                PerformAttack(slower, faster);
                if (!faster.IsAlive)
                    break;
            }

            // 勝者と敗者を決定
            BattleUnit? winner = _player1Unit.IsAlive ? _player1Unit : (_player2Unit.IsAlive ? _player2Unit : null);
            BattleUnit? loser = winner == _player1Unit ? _player2Unit : (_player1Unit.IsAlive ? null : _player1Unit);

            // 結果を生成
            _currentBattle = new BattleResult(_currentSetup, winner, loser, _actionLog, _battleStartTime, _turnCount);

            Debug.Log($"[BattleService] Battle ended! Winner: {winner?.ChampionData.championName ?? "None"}");
            BattleEnded?.Invoke(_currentBattle);

            return _currentBattle;
        }

        /// <summary>
        /// 1ターンだけ処理を進めます（ターンベース用）
        /// </summary>
        /// <returns>バトルが終了した場合はBattleResult、継続中はnull</returns>
        public BattleResult? ProcessSingleTurn()
        {
            if (!IsBattleInProgress || _player1Unit == null || _player2Unit == null || _currentSetup == null)
            {
                Debug.LogError("[BattleService] Cannot process turn: no battle in progress!");
                return null;
            }

            // 既に決着済み
            if (!_player1Unit.IsAlive || !_player2Unit.IsAlive)
            {
                return FinalizeBattle();
            }

            // 速度で先攻を決定（初回のみログ）
            BattleUnit faster = _player1Unit.Speed >= _player2Unit.Speed ? _player1Unit : _player2Unit;
            BattleUnit slower = _player1Unit.Speed >= _player2Unit.Speed ? _player2Unit : _player1Unit;

            if (_turnCount == 0)
            {
                Debug.Log($"[BattleService] {faster.ChampionData.championName} (SPD: {faster.Speed}) attacks first!");
            }

            _turnCount++;

            // 先攻ユニットの攻撃
            PerformAttack(faster, slower);
            if (!slower.IsAlive)
            {
                return FinalizeBattle();
            }

            // 後攻ユニットの反撃
            PerformAttack(slower, faster);
            if (!faster.IsAlive)
            {
                return FinalizeBattle();
            }

            // ターン終了、バトル継続
            Debug.Log($"[BattleService] Turn {_turnCount} complete. P1 HP: {_player1Unit.CurrentHP}/{_player1Unit.MaxHP}, P2 HP: {_player2Unit.CurrentHP}/{_player2Unit.MaxHP}");
            return null;
        }

        /// <summary>
        /// バトルを終了し、結果を生成します
        /// </summary>
        private BattleResult? FinalizeBattle()
        {
            if (_player1Unit == null || _player2Unit == null || _currentSetup == null)
                return null;

            // 勝者と敗者を決定
            BattleUnit? winner = _player1Unit.IsAlive ? _player1Unit : (_player2Unit.IsAlive ? _player2Unit : null);
            BattleUnit? loser = winner == _player1Unit ? _player2Unit : (_player1Unit.IsAlive ? null : _player1Unit);

            // 結果を生成
            _currentBattle = new BattleResult(_currentSetup, winner, loser, _actionLog, _battleStartTime, _turnCount);

            Debug.Log($"[BattleService] Battle ended! Winner: {winner?.ChampionData.championName ?? "None"}");
            BattleEnded?.Invoke(_currentBattle);

            return _currentBattle;
        }

        /// <summary>
        /// 攻撃を実行します
        /// </summary>
        private void PerformAttack(BattleUnit attacker, BattleUnit defender)
        {
            // 回避判定
            if (RollDodge(defender.DodgeChance))
            {
                BattleAction missAction = BattleAction.CreateMiss(attacker, defender);
                _actionLog.Add(missAction);
                ActionPerformed?.Invoke(missAction);
                Debug.Log($"[BattleService] {missAction.Description}");
                return;
            }

            // ダメージ計算
            int baseDamage = CalculateDamage(attacker, defender);

            // クリティカル判定
            bool isCritical = RollCritical(attacker.CriticalChance);
            int finalDamage = isCritical ? (int)(baseDamage * 1.5f) : baseDamage;

            // アクション作成
            BattleAction action = isCritical
                ? BattleAction.CreateCriticalHit(attacker, defender, finalDamage)
                : BattleAction.CreateAttack(attacker, defender, finalDamage);

            _actionLog.Add(action);
            ActionPerformed?.Invoke(action);
            Debug.Log($"[BattleService] {action.Description}");
        }

        /// <summary>
        /// ダメージを計算します
        /// </summary>
        private int CalculateDamage(BattleUnit attacker, BattleUnit defender)
        {
            // 基本ダメージ = 攻撃力 - 防御力/2
            int baseDamage = Math.Max(1, attacker.Attack - defender.Defense / 2);

            // 属性相性ボーナス
            float elementMultiplier = GetElementAdvantage(attacker.ChampionData.element, defender.ChampionData.element);

            // 最終ダメージ
            int finalDamage = (int)(baseDamage * elementMultiplier);

            // ランダム幅 (±10%)
            float variance = 0.9f + (float)_random.NextDouble() * 0.2f;
            finalDamage = (int)(finalDamage * variance);

            return Math.Max(1, finalDamage);
        }

        /// <summary>
        /// 属性相性の倍率を取得します
        /// </summary>
        /// <remarks>
        /// Fire > Earth > Wind > Water > Fire の三すくみ
        /// </remarks>
        private float GetElementAdvantage(ElementType attacker, ElementType defender)
        {
            if (attacker == defender)
                return 1.0f;

            bool hasAdvantage = (attacker, defender) switch
            {
                (ElementType.Fire, ElementType.Earth) => true,
                (ElementType.Earth, ElementType.Wind) => true,
                (ElementType.Wind, ElementType.Water) => true,
                (ElementType.Water, ElementType.Fire) => true,
                _ => false
            };

            bool hasDisadvantage = (attacker, defender) switch
            {
                (ElementType.Fire, ElementType.Water) => true,
                (ElementType.Water, ElementType.Wind) => true,
                (ElementType.Wind, ElementType.Earth) => true,
                (ElementType.Earth, ElementType.Fire) => true,
                _ => false
            };

            if (hasAdvantage)
                return 1.5f; // 有利: 1.5倍
            else if (hasDisadvantage)
                return 0.75f; // 不利: 0.75倍
            else
                return 1.0f; // 等倍
        }

        /// <summary>
        /// クリティカル判定を行います
        /// </summary>
        private bool RollCritical(float criticalChance)
        {
            return _random.NextDouble() < criticalChance;
        }

        /// <summary>
        /// 回避判定を行います
        /// </summary>
        private bool RollDodge(float dodgeChance)
        {
            return _random.NextDouble() < dodgeChance;
        }

        /// <summary>
        /// アイテムを使用します
        /// </summary>
        public void UseItem(string itemId)
        {
            if (!IsBattleInProgress || _player1Unit == null)
            {
                Debug.LogWarning("[BattleService] Cannot use item: no battle in progress!");
                return;
            }

            // TODO: アイテムシステム実装
            // 現在はダミー実装（回復ポーション）
            int healAmount = 100;
            BattleAction itemAction = BattleAction.CreateItemUse(_player1Unit, itemId, healAmount);
            _actionLog.Add(itemAction);
            ActionPerformed?.Invoke(itemAction);

            Debug.Log($"[BattleService] {itemAction.Description}");
        }

        /// <summary>
        /// バトルを中断します
        /// </summary>
        public void AbortBattle()
        {
            if (!IsBattleInProgress)
            {
                Debug.LogWarning("[BattleService] No battle to abort!");
                return;
            }

            Debug.Log("[BattleService] Battle aborted!");
            _currentBattle = null;
            _currentSetup = null;
            _player1Unit = null;
            _player2Unit = null;
            _actionLog.Clear();
            _turnCount = 0;
        }
    }
}
