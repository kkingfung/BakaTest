# Create Champion

新しいチャンピオンのScriptableObjectとデータ構造を作成してください。

## 要件

このコマンドは、ゲーム企画書に基づいたチャンピオンシステムを実装します：
- パッシブアビリティ + アクティブスキル1つ
- ロールシステム（Tank, DPS, Support, Mage, Assassin）
- 属性タイプ（Fire, Water, Earth, Wind）
- 教科親和性システム（各教科でステータスボーナス）

## 作成するファイル

1. **ChampionData.cs** - ScriptableObject定義
2. **ChampionAbility.cs** - アビリティ基底クラス
3. **ChampionStats.cs** - ステータス構造体
4. **Enums.cs** - ロール、属性、教科の列挙型

## テンプレート

### ChampionStats.cs
```csharp
#nullable enable
using System;

namespace CavalryFight.Champions
{
    /// <summary>
    /// チャンピオンの基礎ステータス
    /// </summary>
    [Serializable]
    public struct ChampionStats
    {
        /// <summary>HP（ヒットポイント）</summary>
        public float HP;
        
        /// <summary>攻撃力</summary>
        public float Attack;
        
        /// <summary>防御力</summary>
        public float Defense;
        
        /// <summary>速度</summary>
        public float Speed;
    }

    /// <summary>
    /// 教科親和性（各教科のステータスボーナス比率）
    /// </summary>
    [Serializable]
    public struct SubjectAffinity
    {
        /// <summary>数学→攻撃力変換比率</summary>
        public float MathToAttackRatio;
        
        /// <summary>理科→防御力変換比率</summary>
        public float ScienceToDefenseRatio;
        
        /// <summary>英語→速度変換比率</summary>
        public float EnglishToSpeedRatio;
        
        /// <summary>歴史→HP変換比率</summary>
        public float HistoryToHPRatio;
    }
}
```

### ChampionEnums.cs
```csharp
#nullable enable

namespace CavalryFight.Champions
{
    /// <summary>
    /// チャンピオンのロール
    /// </summary>
    public enum ChampionRole
    {
        Tank,      // タンク：高HP、高防御
        DPS,       // DPS：高攻撃力
        Support,   // サポート：味方強化
        Mage,      // メイジ：魔法攻撃
        Assassin   // アサシン：高速、高クリティカル
    }

    /// <summary>
    /// チャンピオンの属性タイプ
    /// </summary>
    public enum ElementType
    {
        Fire,   // 火 → 土に強い
        Water,  // 水 → 火に強い
        Earth,  // 土 → 風に強い
        Wind    // 風 → 水に強い
    }

    /// <summary>
    /// 教科
    /// </summary>
    public enum Subject
    {
        Math,     // 数学
        Science,  // 理科
        English,  // 英語
        History   // 歴史
    }
}
```

### ChampionData.cs
```csharp
#nullable enable
using UnityEngine;

namespace CavalryFight.Champions
{
    /// <summary>
    /// チャンピオンデータのScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "NewChampion", menuName = "BakaTest/Champion Data")]
    public class ChampionData : ScriptableObject
    {
        [Header("基本情報")]
        /// <summary>チャンピオンID</summary>
        public string championId = string.Empty;
        
        /// <summary>チャンピオン名</summary>
        public string championName = string.Empty;
        
        /// <summary>説明文</summary>
        [TextArea(3, 5)]
        public string description = string.Empty;
        
        /// <summary>アイコン</summary>
        public Sprite? icon;
        
        /// <summary>3Dモデルプレハブ</summary>
        public GameObject? modelPrefab;

        [Header("分類")]
        /// <summary>ロール</summary>
        public ChampionRole role = ChampionRole.DPS;
        
        /// <summary>属性</summary>
        public ElementType element = ElementType.Fire;

        [Header("ステータス")]
        /// <summary>基礎ステータス</summary>
        public ChampionStats baseStats = new ChampionStats
        {
            HP = 100f,
            Attack = 10f,
            Defense = 5f,
            Speed = 5f
        };

        [Header("教科親和性")]
        /// <summary>教科ボーナス比率</summary>
        public SubjectAffinity subjectAffinity = new SubjectAffinity
        {
            MathToAttackRatio = 1.0f,
            ScienceToDefenseRatio = 1.0f,
            EnglishToSpeedRatio = 1.0f,
            HistoryToHPRatio = 1.0f
        };

        [Header("アビリティ")]
        /// <summary>パッシブアビリティの説明</summary>
        [TextArea(2, 3)]
        public string passiveDescription = string.Empty;
        
        /// <summary>アクティブスキル名</summary>
        public string activeSkillName = string.Empty;
        
        /// <summary>アクティブスキルの説明</summary>
        [TextArea(2, 3)]
        public string activeSkillDescription = string.Empty;
        
        /// <summary>スキルクールダウン（秒）</summary>
        public float skillCooldown = 10f;

        [Header("確率効果")]
        /// <summary>クリティカル率（0-1）</summary>
        [Range(0f, 1f)]
        public float criticalChance = 0.1f;
        
        /// <summary>回避率（0-1）</summary>
        [Range(0f, 1f)]
        public float dodgeChance = 0.05f;

        /// <summary>
        /// テストポイントを基に実際のバトルステータスを計算
        /// </summary>
        public ChampionStats CalculateBattleStats(float mathPoints, float sciencePoints, float englishPoints, float historyPoints)
        {
            return new ChampionStats
            {
                HP = baseStats.HP + (historyPoints * subjectAffinity.HistoryToHPRatio),
                Attack = baseStats.Attack + (mathPoints * subjectAffinity.MathToAttackRatio),
                Defense = baseStats.Defense + (sciencePoints * subjectAffinity.ScienceToDefenseRatio),
                Speed = baseStats.Speed + (englishPoints * subjectAffinity.EnglishToSpeedRatio)
            };
        }
    }
}
```

ユーザーに:
1. チャンピオン名
2. ロールと属性
3. 得意教科（どの教科で特にボーナスが大きいか）
4. パッシブとアクティブスキルのコンセプト

を聞いてから、適切なScriptableObjectアセットを作成してください。
