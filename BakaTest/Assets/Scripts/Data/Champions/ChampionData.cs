#nullable enable
using System;
using UnityEngine;
using BakaTest.Data.Localization;

namespace BakaTest.Data.Champions
{
    /// <summary>
    /// チャンピオンデータのScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "NewChampion", menuName = "BakaTest/Champion Data", order = 1)]
    public class ChampionData : ScriptableObject
    {
        [Header("基本情報")]
        /// <summary>チャンピオンID（ユニークな識別子）</summary>
        [Tooltip("ユニークな識別子（例: champion_warrior_001）")]
        public string championId = string.Empty;

        /// <summary>チャンピオン名（レガシー）</summary>
        [Obsolete("Use GetChampionName(Language) instead")]
        [Tooltip("表示名")]
        public string championName = string.Empty;

        /// <summary>多言語対応チャンピオン名</summary>
        [SerializeField] private LocalizedString? _localizedChampionName;

        /// <summary>説明文（レガシー）</summary>
        [Obsolete("Use GetDescription(Language) instead")]
        [TextArea(3, 5)]
        [Tooltip("チャンピオンの説明")]
        public string description = string.Empty;

        /// <summary>多言語対応説明</summary>
        [SerializeField] private LocalizedString? _localizedDescription;
        
        /// <summary>アイコン（UI表示用）</summary>
        [Tooltip("チャンピオン選択画面などで表示する2Dアイコン")]
        public Sprite? icon;
        
        /// <summary>3Dモデルプレハブ（バトル用）</summary>
        [Tooltip("バトル画面で使用する3Dモデル")]
        public GameObject? modelPrefab;

        [Header("分類")]
        /// <summary>ロール</summary>
        [Tooltip("チャンピオンの役割")]
        public ChampionRole role = ChampionRole.DPS;
        
        /// <summary>属性</summary>
        [Tooltip("元素属性（相性に影響）")]
        public ElementType element = ElementType.Fire;

        [Header("基礎ステータス")]
        /// <summary>基礎ステータス</summary>
        [Tooltip("ポイント配分前のベースステータス")]
        public ChampionStats baseStats = new ChampionStats(100f, 10f, 5f, 5f);

        [Header("教科親和性")]
        /// <summary>教科ボーナス比率</summary>
        [Tooltip("各教科のポイントがステータスに変換される比率")]
        public SubjectAffinity subjectAffinity = SubjectAffinity.Default;

        [Header("アビリティ")]
        /// <summary>パッシブアビリティ名（レガシー）</summary>
        [Obsolete("Use GetPassiveName(Language) instead")]
        [Tooltip("パッシブアビリティの名前")]
        public string passiveName = "Passive Ability";

        /// <summary>多言語対応パッシブアビリティ名</summary>
        [SerializeField] private LocalizedString? _localizedPassiveName;

        /// <summary>パッシブアビリティの説明（レガシー）</summary>
        [Obsolete("Use GetPassiveDescription(Language) instead")]
        [TextArea(2, 3)]
        [Tooltip("パッシブアビリティの効果説明")]
        public string passiveDescription = string.Empty;

        /// <summary>多言語対応パッシブアビリティ説明</summary>
        [SerializeField] private LocalizedString? _localizedPassiveDescription;

        /// <summary>アクティブスキル名（レガシー）</summary>
        [Obsolete("Use GetActiveSkillName(Language) instead")]
        [Tooltip("アクティブスキルの名前")]
        public string activeSkillName = "Active Skill";

        /// <summary>多言語対応アクティブスキル名</summary>
        [SerializeField] private LocalizedString? _localizedActiveSkillName;

        /// <summary>アクティブスキルの説明（レガシー）</summary>
        [Obsolete("Use GetActiveSkillDescription(Language) instead")]
        [TextArea(2, 3)]
        [Tooltip("アクティブスキルの効果説明")]
        public string activeSkillDescription = string.Empty;

        /// <summary>多言語対応アクティブスキル説明</summary>
        [SerializeField] private LocalizedString? _localizedActiveSkillDescription;
        
        /// <summary>スキルクールダウン（秒）</summary>
        [Tooltip("アクティブスキルの再使用時間")]
        public float skillCooldown = 10f;

        [Header("確率効果")]
        /// <summary>クリティカル率（0-1）</summary>
        [Range(0f, 1f)]
        [Tooltip("クリティカルヒット発生確率")]
        public float criticalChance = 0.1f;

        /// <summary>回避率（0-1）</summary>
        [Range(0f, 1f)]
        [Tooltip("攻撃回避確率")]
        public float dodgeChance = 0.05f;

        [Header("購入設定")]
        /// <summary>アンロックコスト（バトルコイン）</summary>
        [Tooltip("このチャンピオンを購入するために必要なバトルコイン")]
        public int unlockCost = 1000;

        /// <summary>
        /// チャンピオン名を取得します（多言語対応）
        /// </summary>
        public string GetChampionName(Language language)
        {
            if (_localizedChampionName != null)
            {
                return _localizedChampionName.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return championName; // フォールバック
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
        /// パッシブアビリティ名を取得します（多言語対応）
        /// </summary>
        public string GetPassiveName(Language language)
        {
            if (_localizedPassiveName != null)
            {
                return _localizedPassiveName.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return passiveName; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// パッシブアビリティ説明を取得します（多言語対応）
        /// </summary>
        public string GetPassiveDescription(Language language)
        {
            if (_localizedPassiveDescription != null)
            {
                return _localizedPassiveDescription.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return passiveDescription; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// アクティブスキル名を取得します（多言語対応）
        /// </summary>
        public string GetActiveSkillName(Language language)
        {
            if (_localizedActiveSkillName != null)
            {
                return _localizedActiveSkillName.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return activeSkillName; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// アクティブスキル説明を取得します（多言語対応）
        /// </summary>
        public string GetActiveSkillDescription(Language language)
        {
            if (_localizedActiveSkillDescription != null)
            {
                return _localizedActiveSkillDescription.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return activeSkillDescription; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// チャンピオン名を設定します（多言語対応）
        /// </summary>
        public void SetChampionName(Language language, string text)
        {
            if (_localizedChampionName == null)
            {
                _localizedChampionName = new LocalizedString($"{championId}_name");
            }
            _localizedChampionName.SetText(language, text);
        }

        /// <summary>
        /// 説明を設定します（多言語対応）
        /// </summary>
        public void SetDescription(Language language, string text)
        {
            if (_localizedDescription == null)
            {
                _localizedDescription = new LocalizedString($"{championId}_description");
            }
            _localizedDescription.SetText(language, text);
        }

        /// <summary>
        /// パッシブアビリティ名を設定します（多言語対応）
        /// </summary>
        public void SetPassiveName(Language language, string text)
        {
            if (_localizedPassiveName == null)
            {
                _localizedPassiveName = new LocalizedString($"{championId}_passive_name");
            }
            _localizedPassiveName.SetText(language, text);
        }

        /// <summary>
        /// パッシブアビリティ説明を設定します（多言語対応）
        /// </summary>
        public void SetPassiveDescription(Language language, string text)
        {
            if (_localizedPassiveDescription == null)
            {
                _localizedPassiveDescription = new LocalizedString($"{championId}_passive_description");
            }
            _localizedPassiveDescription.SetText(language, text);
        }

        /// <summary>
        /// アクティブスキル名を設定します（多言語対応）
        /// </summary>
        public void SetActiveSkillName(Language language, string text)
        {
            if (_localizedActiveSkillName == null)
            {
                _localizedActiveSkillName = new LocalizedString($"{championId}_active_name");
            }
            _localizedActiveSkillName.SetText(language, text);
        }

        /// <summary>
        /// アクティブスキル説明を設定します（多言語対応）
        /// </summary>
        public void SetActiveSkillDescription(Language language, string text)
        {
            if (_localizedActiveSkillDescription == null)
            {
                _localizedActiveSkillDescription = new LocalizedString($"{championId}_active_description");
            }
            _localizedActiveSkillDescription.SetText(language, text);
        }

        /// <summary>
        /// テストポイントを基に実際のバトルステータスを計算します
        /// </summary>
        /// <param name="mathPoints">数学ポイント</param>
        /// <param name="sciencePoints">理科ポイント</param>
        /// <param name="englishPoints">英語ポイント</param>
        /// <param name="historyPoints">歴史ポイント</param>
        /// <returns>計算されたバトルステータス</returns>
        public ChampionStats CalculateBattleStats(float mathPoints, float sciencePoints, float englishPoints, float historyPoints)
        {
            // ポイントからボーナスステータスを計算
            var bonusStats = subjectAffinity.ConvertPointsToStats(mathPoints, sciencePoints, englishPoints, historyPoints);
            
            // ベースステータスにボーナスを加算
            return baseStats + bonusStats;
        }

        /// <summary>
        /// 属性相性によるダメージ倍率を取得します
        /// </summary>
        /// <param name="targetElement">対戦相手の属性</param>
        /// <returns>ダメージ倍率（0.5〜2.0）</returns>
        public float GetElementAdvantage(ElementType targetElement)
        {
            // 相性システム: Fire > Earth > Wind > Water > Fire
            if (element == ElementType.Fire && targetElement == ElementType.Earth) return 2.0f;
            if (element == ElementType.Earth && targetElement == ElementType.Wind) return 2.0f;
            if (element == ElementType.Wind && targetElement == ElementType.Water) return 2.0f;
            if (element == ElementType.Water && targetElement == ElementType.Fire) return 2.0f;

            // 逆相性は0.5倍
            if (element == ElementType.Earth && targetElement == ElementType.Fire) return 0.5f;
            if (element == ElementType.Wind && targetElement == ElementType.Earth) return 0.5f;
            if (element == ElementType.Water && targetElement == ElementType.Wind) return 0.5f;
            if (element == ElementType.Fire && targetElement == ElementType.Water) return 0.5f;

            // 同属性または無関係は1.0倍
            return 1.0f;
        }

        /// <summary>
        /// デバッグ用の文字列表現
        /// </summary>
        public override string ToString()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return $"{championName} [{role}] [{element}] - Base Stats: {baseStats}";
#pragma warning restore CS0618
        }

        #region Editor Validation

        private void OnValidate()
        {
            // IDが空の場合、アセット名から自動生成
            if (string.IsNullOrEmpty(championId))
            {
                championId = $"champion_{name.ToLower().Replace(" ", "_")}";
            }

#pragma warning disable CS0618 // Type or member is obsolete
            // 名前が空の場合、アセット名を使用
            if (string.IsNullOrEmpty(championName))
            {
                championName = name;
            }
#pragma warning restore CS0618
        }

        #endregion
    }
}
