#nullable enable
using BakaTest.Data.Items;

namespace BakaTest.Data.Battle
{
    /// <summary>
    /// バトル中のステータス効果（バフ/デバフ）
    /// </summary>
    /// <remarks>
    /// アイテムやアビリティによって付与される一時的な効果を表します。
    /// </remarks>
    public class StatusEffect
    {
        /// <summary>効果の元となったアイテムID</summary>
        public string SourceItemId { get; set; }

        /// <summary>攻撃力ボーナス</summary>
        public int AttackBonus { get; set; }

        /// <summary>防御力ボーナス</summary>
        public int DefenseBonus { get; set; }

        /// <summary>速度ボーナス</summary>
        public int SpeedBonus { get; set; }

        /// <summary>クリティカル率ボーナス</summary>
        public float CriticalBonus { get; set; }

        /// <summary>回避率ボーナス</summary>
        public float DodgeBonus { get; set; }

        /// <summary>ダメージ倍率</summary>
        public float DamageMultiplier { get; set; }

        /// <summary>残り継続ターン数（-1 = 永続、0 = 期限切れ）</summary>
        public int RemainingTurns { get; set; }

        /// <summary>無敵効果</summary>
        public bool HasInvincibility { get; set; }

        /// <summary>復活効果</summary>
        public bool HasRevive { get; set; }

        /// <summary>復活時のHP割合</summary>
        public float ReviveHealthPercent { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public StatusEffect(ItemData itemData)
        {
            SourceItemId = itemData.itemId;
            AttackBonus = itemData.attackBonus;
            DefenseBonus = itemData.defenseBonus;
            SpeedBonus = itemData.speedBonus;
            CriticalBonus = itemData.criticalBonus;
            DodgeBonus = itemData.dodgeBonus;
            DamageMultiplier = itemData.damageMultiplier;
            RemainingTurns = itemData.durationTurns;
            HasInvincibility = itemData.hasInvincibilityEffect;
            HasRevive = itemData.hasReviveEffect;
            ReviveHealthPercent = itemData.reviveHealthPercent;
        }

        /// <summary>
        /// 効果が有効かどうか
        /// </summary>
        public bool IsActive => RemainingTurns != 0;

        /// <summary>
        /// ターン経過処理
        /// </summary>
        public void DecrementTurn()
        {
            if (RemainingTurns > 0)
            {
                RemainingTurns--;
            }
            // RemainingTurns == -1 の場合は永続効果なので減らさない
        }
    }
}
