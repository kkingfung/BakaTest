#nullable enable
using System;

namespace BakaTest.Data.Champions
{
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

        /// <summary>
        /// デフォルト値を持つコンストラクタ
        /// </summary>
        public SubjectAffinity(float math, float science, float english, float history)
        {
            MathToAttackRatio = math;
            ScienceToDefenseRatio = science;
            EnglishToSpeedRatio = english;
            HistoryToHPRatio = history;
        }

        /// <summary>
        /// すべての比率が1.0のデフォルト親和性
        /// </summary>
        public static SubjectAffinity Default => new SubjectAffinity(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// テストポイントをステータスに変換します
        /// </summary>
        public ChampionStats ConvertPointsToStats(float mathPoints, float sciencePoints, float englishPoints, float historyPoints)
        {
            return new ChampionStats(
                hp: historyPoints * HistoryToHPRatio,
                attack: mathPoints * MathToAttackRatio,
                defense: sciencePoints * ScienceToDefenseRatio,
                speed: englishPoints * EnglishToSpeedRatio
            );
        }

        public override string ToString()
        {
            return $"Math→ATK:{MathToAttackRatio:F2} Sci→DEF:{ScienceToDefenseRatio:F2} Eng→SPD:{EnglishToSpeedRatio:F2} His→HP:{HistoryToHPRatio:F2}";
        }
    }
}
