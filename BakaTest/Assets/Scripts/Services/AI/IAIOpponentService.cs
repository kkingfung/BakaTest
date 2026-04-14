#nullable enable
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Battle;

namespace BakaTest.Services.AI
{
    /// <summary>
    /// AI難易度
    /// </summary>
    public enum AIDifficulty
    {
        Easy,       // 簡単：低いポイント総数、ランダム配分
        Medium,     // 普通：中程度のポイント、やや戦略的
        Hard        // 難しい：高いポイント、最適な配分
    }

    /// <summary>
    /// AI性格タイプ
    /// </summary>
    public enum AIPersonality
    {
        Balanced,   // バランス型：全ステータスに均等配分
        Aggressive, // 攻撃型：攻撃力重視（Math, Englishに多め）
        Defensive,  // 防御型：耐久力重視（Science, Historyに多め）
        Tactical,   // 戦術型：チャンピオンの親和性に最適化
        Random      // ランダム型：完全ランダム配分
    }

    /// <summary>
    /// AI対戦相手管理サービスのインターフェース
    /// </summary>
    public interface IAIOpponentService
    {
        /// <summary>
        /// AIのポイント配分を生成します
        /// </summary>
        /// <param name="difficulty">難易度</param>
        /// <param name="personality">性格タイプ</param>
        /// <param name="champion">使用するチャンピオン</param>
        /// <returns>教科別のポイント配分</returns>
        Dictionary<Subject, int> AllocatePoints(AIDifficulty difficulty, AIPersonality personality, ChampionData champion);

        /// <summary>
        /// 難易度に基づく総ポイント数を取得します
        /// </summary>
        /// <param name="difficulty">難易度</param>
        /// <returns>総ポイント数</returns>
        int GetTotalPointsForDifficulty(AIDifficulty difficulty);

        /// <summary>
        /// チャンピオンを選択します（使用可能なチャンピオンから）
        /// </summary>
        /// <param name="availableChampions">使用可能なチャンピオンリスト</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>選択されたチャンピオン</returns>
        ChampionData? SelectChampion(List<ChampionData> availableChampions, AIDifficulty difficulty);

        /// <summary>
        /// 難易度に基づくAI名を生成します
        /// </summary>
        /// <param name="difficulty">難易度</param>
        /// <returns>AI名</returns>
        string GenerateAIName(AIDifficulty difficulty);
    }
}
