#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Services.Champions
{
    /// <summary>
    /// チャンピオン管理サービスのインターフェース
    /// </summary>
    public interface IChampionService
    {
        /// <summary>
        /// 所有しているチャンピオンのリスト
        /// </summary>
        List<ChampionData> OwnedChampions { get; }

        /// <summary>
        /// 現在の無料ローテーションチャンピオンのリスト
        /// </summary>
        List<ChampionData> FreeRotationChampions { get; }

        /// <summary>
        /// 使用可能な全チャンピオン（所有+無料ローテーション）
        /// </summary>
        List<ChampionData> AvailableChampions { get; }

        /// <summary>
        /// 全チャンピオンのリスト（ロック済みを含む）
        /// </summary>
        List<ChampionData> AllChampions { get; }

        /// <summary>
        /// チャンピオンIDからチャンピオンデータを取得します
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>チャンピオンデータ、見つからない場合null</returns>
        ChampionData? GetChampion(string championId);

        /// <summary>
        /// チャンピオンを所有しているか確認します
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>所有している場合true</returns>
        bool IsOwned(string championId);

        /// <summary>
        /// チャンピオンが無料ローテーションに含まれているか確認します
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>無料ローテーションに含まれている場合true</returns>
        bool IsFreeRotation(string championId);

        /// <summary>
        /// チャンピオンが使用可能か確認します（所有または無料ローテーション）
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>使用可能な場合true</returns>
        bool IsAvailable(string championId);

        /// <summary>
        /// チャンピオンをアンロックします
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>アンロックに成功した場合true</returns>
        bool UnlockChampion(string championId);

        /// <summary>
        /// 無料ローテーションを更新します
        /// </summary>
        /// <param name="championIds">新しい無料ローテーションのチャンピオンIDリスト</param>
        void UpdateFreeRotation(List<string> championIds);

        /// <summary>
        /// チャンピオンがアンロックされたときに発火するイベント
        /// </summary>
        event Action<ChampionData>? ChampionUnlocked;

        /// <summary>
        /// 無料ローテーションが更新されたときに発火するイベント
        /// </summary>
        event Action? FreeRotationUpdated;
    }
}
