#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using BakaTest.Data.Champions;
using BakaTest.Data.Items;
using BakaTest.Data.Localization;

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

        /// <summary>基本HP（教科ボーナス適用済み）</summary>
        private int _baseMaxHP;

        /// <summary>最終HP（基本値 + アイテムボーナス）</summary>
        public int MaxHP => _baseMaxHP;

        /// <summary>現在のHP</summary>
        public int CurrentHP { get; set; }

        /// <summary>基本攻撃力（教科ボーナス適用済み）</summary>
        private int _baseAttack;

        /// <summary>最終攻撃力（基本値 + アイテムボーナス）</summary>
        public int Attack => _baseAttack + ActiveStatusEffects.Sum(e => e.AttackBonus);

        /// <summary>基本防御力（教科ボーナス適用済み）</summary>
        private int _baseDefense;

        /// <summary>最終防御力（基本値 + アイテムボーナス）</summary>
        public int Defense => _baseDefense + ActiveStatusEffects.Sum(e => e.DefenseBonus);

        /// <summary>基本速度（教科ボーナス適用済み）</summary>
        private int _baseSpeed;

        /// <summary>最終速度（基本値 + アイテムボーナス）</summary>
        public int Speed => _baseSpeed + ActiveStatusEffects.Sum(e => e.SpeedBonus);

        /// <summary>基本クリティカル率</summary>
        private float _baseCriticalChance;

        /// <summary>最終クリティカル率（基本値 + アイテムボーナス）</summary>
        public float CriticalChance => Math.Min(1.0f, _baseCriticalChance + ActiveStatusEffects.Sum(e => e.CriticalBonus));

        /// <summary>基本回避率</summary>
        private float _baseDodgeChance;

        /// <summary>最終回避率（基本値 + アイテムボーナス）</summary>
        public float DodgeChance => Math.Min(1.0f, _baseDodgeChance + ActiveStatusEffects.Sum(e => e.DodgeBonus));

        /// <summary>アクティブなステータス効果</summary>
        public List<StatusEffect> ActiveStatusEffects { get; private set; } = new List<StatusEffect>();

        /// <summary>生存しているかどうか</summary>
        public bool IsAlive => CurrentHP > 0;

        /// <summary>無敵状態かどうか</summary>
        public bool IsInvincible => ActiveStatusEffects.Any(e => e.HasInvincibility && e.IsActive);

        /// <summary>復活効果を持っているかどうか</summary>
        public bool HasReviveEffect => ActiveStatusEffects.Any(e => e.HasRevive && e.IsActive);

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

            // 基本ステータス（教科ボーナス含む）を設定
            _baseMaxHP = baseHP + hpBonus;
            CurrentHP = _baseMaxHP;
            _baseAttack = baseAttack + attackBonus;
            _baseDefense = baseDefense + defenseBonus;
            _baseSpeed = baseSpeed + speedBonus;
            _baseCriticalChance = ChampionData.criticalChance;
            _baseDodgeChance = ChampionData.dodgeChance;

            // デバッグログ（英語名を使用）
            string championName = ChampionData.GetChampionName(Language.English);
            UnityEngine.Debug.Log($"[BattleUnit] {championName} stats calculated - HP: {MaxHP}, ATK: {Attack}, DEF: {Defense}, SPD: {Speed}");
        }

        /// <summary>
        /// ダメージを受けます
        /// </summary>
        public void TakeDamage(int damage, Language language)
        {
            // 無敵状態の場合はダメージを受けない
            if (IsInvincible)
            {
                string championName = ChampionData.GetChampionName(language);
                UnityEngine.Debug.Log($"[BattleUnit] {championName} is invincible! No damage taken.");
                return;
            }

            CurrentHP = Math.Max(0, CurrentHP - damage);

            string championName2 = ChampionData.GetChampionName(language);
            UnityEngine.Debug.Log($"[BattleUnit] {championName2} took {damage} damage. HP: {CurrentHP}/{MaxHP}");
        }

        /// <summary>
        /// 回復します
        /// </summary>
        public void Heal(int amount, Language language)
        {
            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);

            string championName = ChampionData.GetChampionName(language);
            UnityEngine.Debug.Log($"[BattleUnit] {championName} healed {amount}. HP: {CurrentHP}/{MaxHP}");
        }

        /// <summary>
        /// ステータス効果を適用します
        /// </summary>
        public void ApplyStatusEffect(ItemData itemData, Language language)
        {
            var effect = new StatusEffect(itemData);
            ActiveStatusEffects.Add(effect);

            string championName = ChampionData.GetChampionName(language);
            string itemName = itemData.GetItemName(language);
            UnityEngine.Debug.Log($"[BattleUnit] {championName} received status effect from {itemName}");
        }

        /// <summary>
        /// ターン終了時のステータス効果処理
        /// </summary>
        public void ProcessEndOfTurnEffects(Language language)
        {
            string championName = ChampionData.GetChampionName(language);

            // 効果の持続時間を減らす
            foreach (var effect in ActiveStatusEffects)
            {
                effect.DecrementTurn();
            }

            // 期限切れの効果を削除
            int removedCount = ActiveStatusEffects.RemoveAll(e => !e.IsActive);
            if (removedCount > 0)
            {
                UnityEngine.Debug.Log($"[BattleUnit] {championName} - {removedCount} status effect(s) expired");
            }
        }

        /// <summary>
        /// 復活を試みます
        /// </summary>
        /// <returns>復活に成功した場合はtrue</returns>
        public bool TryRevive(Language language)
        {
            if (!HasReviveEffect)
            {
                return false;
            }

            // 復活効果を探す
            var reviveEffect = ActiveStatusEffects.FirstOrDefault(e => e.HasRevive && e.IsActive);
            if (reviveEffect == null)
            {
                return false;
            }

            // 復活処理
            int reviveHP = (int)(MaxHP * reviveEffect.ReviveHealthPercent);
            CurrentHP = reviveHP;

            // 復活効果は1回のみなので削除
            ActiveStatusEffects.Remove(reviveEffect);

            string championName = ChampionData.GetChampionName(language);
            UnityEngine.Debug.Log($"[BattleUnit] {championName} revived with {reviveHP} HP!");
            return true;
        }
    }
}
