#nullable enable
using UnityEditor;
using UnityEngine;
using BakaTest.Data.Items;
using System.IO;

namespace BakaTest.Editor
{
    /// <summary>
    /// ItemDataのScriptableObjectを生成するユーティリティ
    /// </summary>
    public static class ItemDataGenerator
    {
        private const string OutputFolder = "Assets/Resources/Data/Items";

        [MenuItem("BakaTest/Generate/Create Sample Battle Items")]
        public static void GenerateSampleItems()
        {
            // フォルダが存在しない場合は作成
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
                AssetDatabase.Refresh();
            }

            Debug.Log("[ItemDataGenerator] Generating sample battle items...");

            // Consumables (回復アイテム)
            CreateHealthPotion();
            CreateGreaterHealthPotion();
            CreatePhoenixDown();

            // Buffs (自己強化)
            CreateStrengthElixir();
            CreateIronSkinPotion();
            CreateHastePotion();
            CreateBattleFury();
            CreateFocusCharm();

            // Debuffs (敵弱体化)
            CreatePoisonVial();
            CreateSmokeBomb();
            CreateParalyzingDart();
            CreateWeakeningCurse();

            // Special (特殊効果)
            CreateGuardianAngel();
            CreateUltimatePower();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[ItemDataGenerator] 14 battle items created in {OutputFolder}");
        }

        #region Consumables

        /// <summary>
        /// 体力回復ポーション（コモン）
        /// </summary>
        private static void CreateHealthPotion()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_health_potion";
            item.itemName = "体力回復ポーション";
            item.description = "HPを200回復する基本的な回復アイテム。";
            item.itemType = ItemType.Consumable;
            item.rarity = ItemRarity.Common;
            item.target = ItemTarget.Self;

            item.buyPrice = 100;
            item.healAmount = 200;
            item.durationTurns = 0; // 即座に効果

            item.maxUsesPerBattle = 3;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_HealthPotion.asset");
        }

        /// <summary>
        /// 上級体力回復ポーション（レア）
        /// </summary>
        private static void CreateGreaterHealthPotion()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_greater_health_potion";
            item.itemName = "上級体力回復ポーション";
            item.description = "HPを500回復する強力な回復アイテム。";
            item.itemType = ItemType.Consumable;
            item.rarity = ItemRarity.Rare;
            item.target = ItemTarget.Self;

            item.buyPrice = 300;
            item.healAmount = 500;
            item.durationTurns = 0;

            item.maxUsesPerBattle = 2;
            item.cooldownTurns = 2; // 2ターンクールダウン

            CreateAsset(item, "Item_GreaterHealthPotion.asset");
        }

        /// <summary>
        /// 不死鳥の羽根（レジェンダリー）
        /// </summary>
        private static void CreatePhoenixDown()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_phoenix_down";
            item.itemName = "不死鳥の羽根";
            item.description = "戦闘不能になったとき、自動的に50%のHPで復活する。";
            item.itemType = ItemType.Special;
            item.rarity = ItemRarity.Legendary;
            item.target = ItemTarget.Self;

            item.buyPrice = 1000;
            item.hasReviveEffect = true;
            item.reviveHealthPercent = 0.5f;
            item.durationTurns = -1; // 永続（使用されるまで）

            item.maxUsesPerBattle = 1;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_PhoenixDown.asset");
        }

        #endregion

        #region Buffs

        /// <summary>
        /// 力の秘薬（アンコモン）
        /// </summary>
        private static void CreateStrengthElixir()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_strength_elixir";
            item.itemName = "力の秘薬";
            item.description = "攻撃力を50増加させる（3ターン持続）。";
            item.itemType = ItemType.Buff;
            item.rarity = ItemRarity.Uncommon;
            item.target = ItemTarget.Self;

            item.buyPrice = 150;
            item.attackBonus = 50;
            item.durationTurns = 3;

            item.maxUsesPerBattle = 0; // 無制限
            item.cooldownTurns = 4;

            CreateAsset(item, "Item_StrengthElixir.asset");
        }

        /// <summary>
        /// 鉄の皮膚ポーション（アンコモン）
        /// </summary>
        private static void CreateIronSkinPotion()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_iron_skin_potion";
            item.itemName = "鉄の皮膚ポーション";
            item.description = "防御力を60増加させる（4ターン持続）。";
            item.itemType = ItemType.Buff;
            item.rarity = ItemRarity.Uncommon;
            item.target = ItemTarget.Self;

            item.buyPrice = 180;
            item.defenseBonus = 60;
            item.durationTurns = 4;

            item.maxUsesPerBattle = 0;
            item.cooldownTurns = 5;

            CreateAsset(item, "Item_IronSkinPotion.asset");
        }

        /// <summary>
        /// 加速ポーション（レア）
        /// </summary>
        private static void CreateHastePotion()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_haste_potion";
            item.itemName = "加速ポーション";
            item.description = "速度を40増加させる（3ターン持続）。";
            item.itemType = ItemType.Buff;
            item.rarity = ItemRarity.Rare;
            item.target = ItemTarget.Self;

            item.buyPrice = 250;
            item.speedBonus = 40;
            item.durationTurns = 3;

            item.maxUsesPerBattle = 0;
            item.cooldownTurns = 4;

            CreateAsset(item, "Item_HastePotion.asset");
        }

        /// <summary>
        /// 戦闘の狂気（エピック）
        /// </summary>
        private static void CreateBattleFury()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_battle_fury";
            item.itemName = "戦闘の狂気";
            item.description = "クリティカル率+20%、ダメージ1.5倍（2ターン持続）。";
            item.itemType = ItemType.Buff;
            item.rarity = ItemRarity.Epic;
            item.target = ItemTarget.Self;

            item.buyPrice = 500;
            item.criticalBonus = 0.20f;
            item.damageMultiplier = 1.5f;
            item.durationTurns = 2;

            item.maxUsesPerBattle = 1;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_BattleFury.asset");
        }

        /// <summary>
        /// 集中のお守り（レア）
        /// </summary>
        private static void CreateFocusCharm()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_focus_charm";
            item.itemName = "集中のお守り";
            item.description = "クリティカル率+15%、回避率+10%（3ターン持続）。";
            item.itemType = ItemType.Buff;
            item.rarity = ItemRarity.Rare;
            item.target = ItemTarget.Self;

            item.buyPrice = 350;
            item.criticalBonus = 0.15f;
            item.dodgeBonus = 0.10f;
            item.durationTurns = 3;

            item.maxUsesPerBattle = 0;
            item.cooldownTurns = 5;

            CreateAsset(item, "Item_FocusCharm.asset");
        }

        #endregion

        #region Debuffs

        /// <summary>
        /// 毒の小瓶（コモン）
        /// </summary>
        private static void CreatePoisonVial()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_poison_vial";
            item.itemName = "毒の小瓶";
            item.description = "敵に毒を付与し、3ターンの間、毎ターン50ダメージを与える。";
            item.itemType = ItemType.Debuff;
            item.rarity = ItemRarity.Common;
            item.target = ItemTarget.Enemy;

            item.buyPrice = 120;
            item.causesPoison = true;
            item.poisonDamagePerTurn = 50;
            item.poisonDurationTurns = 3;
            item.durationTurns = 0;

            item.maxUsesPerBattle = 0;
            item.cooldownTurns = 3;

            CreateAsset(item, "Item_PoisonVial.asset");
        }

        /// <summary>
        /// 煙幕弾（アンコモン）
        /// </summary>
        private static void CreateSmokeBomb()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_smoke_bomb";
            item.itemName = "煙幕弾";
            item.description = "敵の命中率を下げ、防御力を30減少させる（2ターン持続）。";
            item.itemType = ItemType.Debuff;
            item.rarity = ItemRarity.Uncommon;
            item.target = ItemTarget.Enemy;

            item.buyPrice = 200;
            item.defenseBonus = -30; // マイナス効果
            item.dodgeBonus = -0.15f; // 敵の回避率減少
            item.durationTurns = 2;

            item.maxUsesPerBattle = 0;
            item.cooldownTurns = 4;

            CreateAsset(item, "Item_SmokeBomb.asset");
        }

        /// <summary>
        /// 麻痺の矢（レア）
        /// </summary>
        private static void CreateParalyzingDart()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_paralyzing_dart";
            item.itemName = "麻痺の矢";
            item.description = "敵を1ターンの間スタン状態にする（行動不能）。";
            item.itemType = ItemType.Debuff;
            item.rarity = ItemRarity.Rare;
            item.target = ItemTarget.Enemy;

            item.buyPrice = 400;
            item.causesStun = true;
            item.stunTurns = 1;
            item.durationTurns = 0;

            item.maxUsesPerBattle = 2;
            item.cooldownTurns = 5;

            CreateAsset(item, "Item_ParalyzingDart.asset");
        }

        /// <summary>
        /// 弱体化の呪い（エピック）
        /// </summary>
        private static void CreateWeakeningCurse()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_weakening_curse";
            item.itemName = "弱体化の呪い";
            item.description = "敵の攻撃力-40、防御力-40、速度-30（3ターン持続）。";
            item.itemType = ItemType.Debuff;
            item.rarity = ItemRarity.Epic;
            item.target = ItemTarget.Enemy;

            item.buyPrice = 600;
            item.attackBonus = -40;
            item.defenseBonus = -40;
            item.speedBonus = -30;
            item.durationTurns = 3;

            item.maxUsesPerBattle = 1;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_WeakeningCurse.asset");
        }

        #endregion

        #region Special

        /// <summary>
        /// 守護天使（レジェンダリー）
        /// </summary>
        private static void CreateGuardianAngel()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_guardian_angel";
            item.itemName = "守護天使";
            item.description = "2ターンの間、無敵状態になる（ダメージを受けない）。";
            item.itemType = ItemType.Special;
            item.rarity = ItemRarity.Legendary;
            item.target = ItemTarget.Self;

            item.buyPrice = 1500;
            item.hasInvincibilityEffect = true;
            item.invincibilityTurns = 2;
            item.durationTurns = 0;

            item.maxUsesPerBattle = 1;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_GuardianAngel.asset");
        }

        /// <summary>
        /// 究極の力（エピック）
        /// </summary>
        private static void CreateUltimatePower()
        {
            var item = ScriptableObject.CreateInstance<ItemData>();

            item.itemId = "item_ultimate_power";
            item.itemName = "究極の力";
            item.description = "全ステータス+50、クリティカル率+25%、ダメージ2倍（1ターン持続）。";
            item.itemType = ItemType.Special;
            item.rarity = ItemRarity.Epic;
            item.target = ItemTarget.Self;

            item.buyPrice = 800;
            item.attackBonus = 50;
            item.defenseBonus = 50;
            item.speedBonus = 50;
            item.criticalBonus = 0.25f;
            item.damageMultiplier = 2.0f;
            item.durationTurns = 1;

            item.maxUsesPerBattle = 1;
            item.cooldownTurns = 0;

            CreateAsset(item, "Item_UltimatePower.asset");
        }

        #endregion

        /// <summary>
        /// アセットを作成して保存します
        /// </summary>
        private static void CreateAsset(ItemData item, string fileName)
        {
            string path = Path.Combine(OutputFolder, fileName);

            // 既存のアセットがある場合は上書き確認
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog(
                    "Overwrite Confirmation",
                    $"Item '{item.itemName}' already exists. Overwrite?",
                    "Yes",
                    "No"))
                {
                    Debug.Log($"[ItemDataGenerator] Skipped {item.itemName}");
                    return;
                }
            }

            AssetDatabase.CreateAsset(item, path);
            Debug.Log($"[ItemDataGenerator] Created {item.itemName} at {path}");
        }
    }
}
