#nullable enable
using System;

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
        public static BattleAction CreateAttack(BattleUnit attacker, BattleUnit defender, int damage)
        {
            string message = $"{attacker.ChampionData.championName} attacked {defender.ChampionData.championName} for {damage} damage!";

            var action = new BattleAction(
                BattleActionType.Attack,
                attacker.ChampionData.championName,
                defender.ChampionData.championName,
                message,
                turn: 0,
                damage: damage
            );

            defender.TakeDamage(damage);
            return action;
        }

        /// <summary>
        /// クリティカルヒットアクションを作成します
        /// </summary>
        public static BattleAction CreateCriticalHit(BattleUnit attacker, BattleUnit defender, int damage)
        {
            string message = $"💥 CRITICAL! {attacker.ChampionData.championName} dealt {damage} damage to {defender.ChampionData.championName}!";

            var action = new BattleAction(
                BattleActionType.Critical,
                attacker.ChampionData.championName,
                defender.ChampionData.championName,
                message,
                turn: 0,
                damage: damage
            );

            defender.TakeDamage(damage);
            return action;
        }

        /// <summary>
        /// 回避アクションを作成します
        /// </summary>
        public static BattleAction CreateMiss(BattleUnit attacker, BattleUnit defender)
        {
            string message = $"{defender.ChampionData.championName} dodged {attacker.ChampionData.championName}'s attack!";

            return new BattleAction(
                BattleActionType.Dodge,
                attacker.ChampionData.championName,
                defender.ChampionData.championName,
                message,
                turn: 0
            );
        }

        /// <summary>
        /// アイテム使用アクションを作成します
        /// </summary>
        public static BattleAction CreateItemUse(BattleUnit user, string itemId, int healAmount)
        {
            string message = $"{user.ChampionData.championName} used {itemId} and restored {healAmount} HP!";

            var action = new BattleAction(
                BattleActionType.Item,
                user.ChampionData.championName,
                user.ChampionData.championName,
                message,
                turn: 0,
                healing: healAmount
            );

            user.Heal(healAmount);
            return action;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
