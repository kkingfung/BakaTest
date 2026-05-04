#nullable enable
using System;
using BakaTest.Data.Items;
using BakaTest.Data.Localization;

namespace BakaTest.Data.Battle
{
    /// <summary>
    /// バトルアクションの種類
    /// </summary>
    public enum BattleActionType
    {
        Attack,      // 通常攻撃
        Critical,    // クリティカルヒット
        Dodge,       // 回避
        Ability,     // アビリティ使用
        Item,        // アイテム使用
        StatusEffect // ステータス効果
    }

    /// <summary>
    /// バトル中の1アクション
    /// </summary>
    /// <remarks>
    /// ダメージ、回復、回避などの1回の行動を表します。
    /// BattleResultのActionLogに記録されます。
    /// </remarks>
    public class BattleAction
    {
        /// <summary>アクションの種類</summary>
        public BattleActionType Type { get; set; }

        /// <summary>行動者の名前</summary>
        public string ActorName { get; set; }

        /// <summary>対象の名前</summary>
        public string TargetName { get; set; }

        /// <summary>ダメージ量（攻撃の場合）</summary>
        public int Damage { get; set; }

        /// <summary>回復量（回復の場合）</summary>
        public int Healing { get; set; }

        /// <summary>アクションメッセージ</summary>
        public string Message { get; set; }

        /// <summary>ターン番号</summary>
        public int Turn { get; set; }

        /// <summary>タイムスタンプ</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleAction(
            BattleActionType type,
            string actorName,
            string targetName,
            string message,
            int turn,
            int damage = 0,
            int healing = 0)
        {
            Type = type;
            ActorName = actorName ?? "Unknown";
            TargetName = targetName ?? "Unknown";
            Message = message ?? string.Empty;
            Turn = turn;
            Damage = damage;
            Healing = healing;
            Timestamp = DateTime.Now;
        }

        /// <summary>アクション説明</summary>
        public string Description => Message;

        /// <summary>
        /// 通常攻撃アクションを作成します
        /// </summary>
        public static BattleAction CreateAttack(BattleUnit attacker, BattleUnit defender, int damage, Language language)
        {
            string attackerName = attacker.ChampionData.GetChampionName(language);
            string defenderName = defender.ChampionData.GetChampionName(language);
            string message = $"{attackerName} attacked {defenderName} for {damage} damage!";

            var action = new BattleAction(
                BattleActionType.Attack,
                attackerName,
                defenderName,
                message,
                turn: 0,
                damage: damage
            );

            defender.TakeDamage(damage, language);
            return action;
        }

        /// <summary>
        /// クリティカルヒットアクションを作成します
        /// </summary>
        public static BattleAction CreateCriticalHit(BattleUnit attacker, BattleUnit defender, int damage, Language language)
        {
            string attackerName = attacker.ChampionData.GetChampionName(language);
            string defenderName = defender.ChampionData.GetChampionName(language);
            string message = $"💥 CRITICAL! {attackerName} dealt {damage} damage to {defenderName}!";

            var action = new BattleAction(
                BattleActionType.Critical,
                attackerName,
                defenderName,
                message,
                turn: 0,
                damage: damage
            );

            defender.TakeDamage(damage, language);
            return action;
        }

        /// <summary>
        /// 回避アクションを作成します
        /// </summary>
        public static BattleAction CreateMiss(BattleUnit attacker, BattleUnit defender, Language language)
        {
            string attackerName = attacker.ChampionData.GetChampionName(language);
            string defenderName = defender.ChampionData.GetChampionName(language);
            string message = $"{defenderName} dodged {attackerName}'s attack!";

            return new BattleAction(
                BattleActionType.Dodge,
                attackerName,
                defenderName,
                message,
                turn: 0
            );
        }

        /// <summary>
        /// アイテム使用アクションを作成します（レガシー）
        /// </summary>
        [Obsolete("Use CreateItemUse(BattleUnit, BattleUnit?, ItemData, Language) instead")]
        public static BattleAction CreateItemUse(BattleUnit user, string itemId, int healAmount, Language language)
        {
            string userName = user.ChampionData.GetChampionName(language);
            string message = $"{userName} used {itemId} and restored {healAmount} HP!";

            var action = new BattleAction(
                BattleActionType.Item,
                userName,
                userName,
                message,
                turn: 0,
                healing: healAmount
            );

            user.Heal(healAmount, language);
            return action;
        }

        /// <summary>
        /// アイテム使用アクションを作成します（完全版）
        /// </summary>
        /// <param name="user">使用者</param>
        /// <param name="target">対象（null = 自分）</param>
        /// <param name="itemData">使用するアイテム</param>
        /// <param name="language">言語</param>
        public static BattleAction CreateItemUse(BattleUnit user, BattleUnit? target, ItemData itemData, Language language)
        {
            string userName = user.ChampionData.GetChampionName(language);
            string itemName = itemData.GetItemName(language);
            BattleUnit actualTarget = target ?? user;
            string targetName = actualTarget.ChampionData.GetChampionName(language);

            // アイテム効果を適用
            string effectDescription = "";

            // 回復効果
            if (itemData.healAmount > 0)
            {
                actualTarget.Heal(itemData.healAmount, language);
                effectDescription = $"restored {itemData.healAmount} HP";
            }

            // ステータス効果（バフ/デバフ）
            if (itemData.attackBonus != 0 || itemData.defenseBonus != 0 || itemData.speedBonus != 0 ||
                itemData.criticalBonus > 0 || itemData.dodgeBonus > 0 || itemData.damageMultiplier != 1.0f ||
                itemData.hasInvincibilityEffect || itemData.hasReviveEffect)
            {
                actualTarget.ApplyStatusEffect(itemData, language);
                if (!string.IsNullOrEmpty(effectDescription))
                {
                    effectDescription += " and applied status effect";
                }
                else
                {
                    effectDescription = "applied status effect";
                }
            }

            string message = $"{userName} used {itemName} on {targetName} and {effectDescription}!";

            return new BattleAction(
                BattleActionType.Item,
                userName,
                targetName,
                message,
                turn: 0,
                healing: itemData.healAmount
            );
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
