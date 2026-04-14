#nullable enable
using System;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Data.Localization;

namespace BakaTest.Data.Items
{
    /// <summary>
    /// アイテムの種類
    /// </summary>
    public enum ItemType
    {
        Consumable,  // 消耗品（回復ポーション等）
        Buff,        // バフアイテム（一時的なステータス強化）
        Debuff,      // デバフアイテム（相手を弱体化）
        Special      // 特殊効果（復活、無敵等）
    }

    /// <summary>
    /// アイテムの希少度
    /// </summary>
    public enum ItemRarity
    {
        Common,      // コモン（白）
        Uncommon,    // アンコモン（緑）
        Rare,        // レア（青）
        Epic,        // エピック（紫）
        Legendary    // レジェンダリー（金）
    }

    /// <summary>
    /// アイテム効果の対象
    /// </summary>
    public enum ItemTarget
    {
        Self,        // 自分
        Enemy,       // 敵
        Both         // 両方
    }

    /// <summary>
    /// アイテムデータ（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "Item_", menuName = "BakaTest/Item Data", order = 2)]
    public class ItemData : ScriptableObject
    {
        [Header("基本情報")]
        /// <summary>アイテムID（一意）</summary>
        public string itemId = "";

        /// <summary>アイテム名（レガシー）</summary>
        [Obsolete("Use GetItemName(Language) instead")]
        public string itemName = "";

        /// <summary>多言語対応アイテム名</summary>
        [SerializeField] private LocalizedString? _localizedItemName;

        /// <summary>説明（レガシー）</summary>
        [Obsolete("Use GetDescription(Language) instead")]
        [TextArea(2, 4)]
        public string description = "";

        /// <summary>多言語対応説明</summary>
        [SerializeField] private LocalizedString? _localizedDescription;

        /// <summary>アイテムの種類</summary>
        public ItemType itemType = ItemType.Consumable;

        /// <summary>希少度</summary>
        public ItemRarity rarity = ItemRarity.Common;

        /// <summary>効果の対象</summary>
        public ItemTarget target = ItemTarget.Self;

        [Header("価格")]
        /// <summary>購入価格（バトルコイン）</summary>
        public int buyPrice = 100;

        /// <summary>売却価格（バトルコインの50%）</summary>
        public int sellPrice => buyPrice / 2;

        [Header("効果")]
        /// <summary>HP回復量</summary>
        public int healAmount = 0;

        /// <summary>攻撃力増加量</summary>
        public int attackBonus = 0;

        /// <summary>防御力増加量</summary>
        public int defenseBonus = 0;

        /// <summary>速度増加量</summary>
        public int speedBonus = 0;

        /// <summary>クリティカル率増加（0.0 ~ 1.0）</summary>
        [Range(0f, 1f)]
        public float criticalBonus = 0f;

        /// <summary>回避率増加（0.0 ~ 1.0）</summary>
        [Range(0f, 1f)]
        public float dodgeBonus = 0f;

        /// <summary>ダメージ倍率（1.0 = 通常、1.5 = 1.5倍）</summary>
        public float damageMultiplier = 1.0f;

        /// <summary>効果持続ターン数（0 = 即座、-1 = 永続）</summary>
        public int durationTurns = 0;

        [Header("特殊効果")]
        /// <summary>復活効果（HP 0になっても復活）</summary>
        public bool hasReviveEffect = false;

        /// <summary>復活時のHP割合（0.0 ~ 1.0）</summary>
        [Range(0f, 1f)]
        public float reviveHealthPercent = 0.5f;

        /// <summary>無敵効果（ダメージを受けない）</summary>
        public bool hasInvincibilityEffect = false;

        /// <summary>無敵効果のターン数</summary>
        public int invincibilityTurns = 1;

        /// <summary>状態異常：スタン（行動不能）</summary>
        public bool causesStun = false;

        /// <summary>スタンのターン数</summary>
        public int stunTurns = 1;

        /// <summary>状態異常：毒（ターン毎にダメージ）</summary>
        public bool causesPoison = false;

        /// <summary>毒のダメージ/ターン</summary>
        public int poisonDamagePerTurn = 0;

        /// <summary>毒の持続ターン数</summary>
        public int poisonDurationTurns = 3;

        [Header("使用制限")]
        /// <summary>バトル中の使用回数制限（0 = 無制限）</summary>
        public int maxUsesPerBattle = 0;

        /// <summary>クールダウン（使用後のターン数）</summary>
        public int cooldownTurns = 0;

        /// <summary>
        /// アイテム名を取得します（多言語対応）
        /// </summary>
        public string GetItemName(Language language)
        {
            if (_localizedItemName != null)
            {
                return _localizedItemName.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return itemName; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// 説明を取得します（多言語対応）
        /// </summary>
        public string GetDescription(Language language)
        {
            if (_localizedDescription != null)
            {
                return _localizedDescription.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return description; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// アイテム名を設定します（多言語対応）
        /// </summary>
        public void SetItemName(Language language, string text)
        {
            if (_localizedItemName == null)
            {
                _localizedItemName = new LocalizedString($"{itemId}_name");
            }
            _localizedItemName.SetText(language, text);
        }

        /// <summary>
        /// 説明を設定します（多言語対応）
        /// </summary>
        public void SetDescription(Language language, string text)
        {
            if (_localizedDescription == null)
            {
                _localizedDescription = new LocalizedString($"{itemId}_description");
            }
            _localizedDescription.SetText(language, text);
        }

        /// <summary>
        /// アイテムが使用可能かチェックします
        /// </summary>
        public bool CanUse(int currentUseCount, int lastUsedTurn, int currentTurn)
        {
            // 使用回数制限チェック
            if (maxUsesPerBattle > 0 && currentUseCount >= maxUsesPerBattle)
            {
                return false;
            }

            // クールダウンチェック
            if (cooldownTurns > 0 && lastUsedTurn >= 0)
            {
                int turnsSinceUse = currentTurn - lastUsedTurn;
                if (turnsSinceUse < cooldownTurns)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// アイテムの総合的な価値スコアを計算します（AI用）
        /// </summary>
        public int CalculateValueScore()
        {
            int score = 0;

            // 回復効果
            score += healAmount / 10;

            // ステータスボーナス
            score += attackBonus * 2;
            score += defenseBonus * 2;
            score += speedBonus;

            // 確率ボーナス
            score += (int)(criticalBonus * 100);
            score += (int)(dodgeBonus * 100);

            // ダメージ倍率
            score += (int)((damageMultiplier - 1.0f) * 100);

            // 特殊効果
            if (hasReviveEffect) score += 500;
            if (hasInvincibilityEffect) score += 300;
            if (causesStun) score += 200;
            if (causesPoison) score += poisonDamagePerTurn * poisonDurationTurns;

            // 希少度ボーナス
            score += (int)rarity * 50;

            return score;
        }
    }
}
