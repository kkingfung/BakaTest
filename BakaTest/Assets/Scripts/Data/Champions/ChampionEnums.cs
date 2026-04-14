#nullable enable

namespace BakaTest.Data.Champions
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
