#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Data.Battle
{
    /// <summary>
    /// バトル中のチャンピオンユニット
    /// </summary>
    /// <remarks>
    /// 基本ステータスに教科ポイントからのボーナスを加算した最終ステータスを保持します。
    /// </remarks>
    public class BattleUnit
    {
        /// <summary>チャンピオンデータへの参照</summary>
        public ChampionData ChampionData { get; set; }

        /// <summary>プレイヤー名</summary>
        public string PlayerName { get; set; }

        /// <summary>配分されたポイント（教科別）</summary>
        public Dictionary<Subject, int> AllocatedPoints { get; set; }

        /// <summary>最終HP（基本値 + ボーナス）</summary>
        public int MaxHP { get; private set; }

        /// <summary>現在のHP</summary>
        public int CurrentHP { get; set; }

        /// <summary>最終攻撃力（基本値 + ボーナス）</summary>
        public int Attack { get; private set; }

        /// <summary>最終防御力（基本値 + ボーナス）</summary>
        public int Defense { get; private set; }

        /// <summary>最終速度（基本値 + ボーナス）</summary>
        public int Speed { get; private set; }

        /// <summary>クリティカル率</summary>
        public float CriticalChance { get; private set; }

        /// <summary>回避率</summary>
        public float DodgeChance { get; private set; }

        /// <summary>生存しているかどうか</summary>
        public bool IsAlive => CurrentHP > 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleUnit(ChampionData championData, string playerName, Dictionary<Subject, int> allocatedPoints)
        {
            ChampionData = championData ?? throw new ArgumentNullException(nameof(championData));
            PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
            AllocatedPoints = allocatedPoints ?? new Dictionary<Subject, int>();

            // ステータスを計算
            CalculateFinalStats();
        }

        /// <summary>
        /// 配分ポイントから最終ステータスを計算します
        /// </summary>
        private void CalculateFinalStats()
        {
            // 基本ステータスから開始
            int baseHP = (int)ChampionData.baseStats.HP;
            int baseAttack = (int)ChampionData.baseStats.Attack;
            int baseDefense = (int)ChampionData.baseStats.Defense;
            int baseSpeed = (int)ChampionData.baseStats.Speed;

            // 教科ポイントからボーナスを計算
            int mathPoints = AllocatedPoints.GetValueOrDefault(Subject.Math, 0);
            int sciencePoints = AllocatedPoints.GetValueOrDefault(Subject.Science, 0);
            int englishPoints = AllocatedPoints.GetValueOrDefault(Subject.English, 0);
            int historyPoints = AllocatedPoints.GetValueOrDefault(Subject.History, 0);

            // 親和性に基づいてボーナスを加算
            int attackBonus = (int)(mathPoints * ChampionData.subjectAffinity.MathToAttackRatio);
            int defenseBonus = (int)(sciencePoints * ChampionData.subjectAffinity.ScienceToDefenseRatio);
            int speedBonus = (int)(englishPoints * ChampionData.subjectAffinity.EnglishToSpeedRatio);
            int hpBonus = (int)(historyPoints * ChampionData.subjectAffinity.HistoryToHPRatio);

            // 最終ステータス
            MaxHP = baseHP + hpBonus;
            CurrentHP = MaxHP;
            Attack = baseAttack + attackBonus;
            Defense = baseDefense + defenseBonus;
            Speed = baseSpeed + speedBonus;
            CriticalChance = ChampionData.criticalChance;
            DodgeChance = ChampionData.dodgeChance;

            UnityEngine.Debug.Log($"[BattleUnit] {ChampionData.championName} stats calculated - HP: {MaxHP}, ATK: {Attack}, DEF: {Defense}, SPD: {Speed}");
        }

        /// <summary>
        /// ダメージを受けます
        /// </summary>
        public void TakeDamage(int damage)
        {
            CurrentHP = Math.Max(0, CurrentHP - damage);
            UnityEngine.Debug.Log($"[BattleUnit] {ChampionData.championName} took {damage} damage. HP: {CurrentHP}/{MaxHP}");
        }

        /// <summary>
        /// 回復します
        /// </summary>
        public void Heal(int amount)
        {
            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
            UnityEngine.Debug.Log($"[BattleUnit] {ChampionData.championName} healed {amount}. HP: {CurrentHP}/{MaxHP}");
        }
    }
}
