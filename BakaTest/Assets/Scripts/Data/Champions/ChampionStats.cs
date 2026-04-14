#nullable enable
using System;

namespace BakaTest.Data.Champions
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

        /// <summary>
        /// デフォルト値を持つコンストラクタ
        /// </summary>
        public ChampionStats(float hp, float attack, float defense, float speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            Speed = speed;
        }

        /// <summary>
        /// ステータスを加算します
        /// </summary>
        public static ChampionStats operator +(ChampionStats a, ChampionStats b)
        {
            return new ChampionStats(
                a.HP + b.HP,
                a.Attack + b.Attack,
                a.Defense + b.Defense,
                a.Speed + b.Speed
            );
        }

        /// <summary>
        /// ステータスを乗算します
        /// </summary>
        public static ChampionStats operator *(ChampionStats stats, float multiplier)
        {
            return new ChampionStats(
                stats.HP * multiplier,
                stats.Attack * multiplier,
                stats.Defense * multiplier,
                stats.Speed * multiplier
            );
        }

        public override string ToString()
        {
            return $"HP:{HP:F1} ATK:{Attack:F1} DEF:{Defense:F1} SPD:{Speed:F1}";
        }
    }
}
