#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Items;

namespace BakaTest.Services.Shop
{
    /// <summary>
    /// ショップ管理サービスのインターフェース
    /// </summary>
    /// <remarks>
    /// チャンピオンとアイテムの購入、販売、価格管理を行います。
    /// </remarks>
    public interface IShopService
    {
        /// <summary>
        /// 購入可能なすべてのチャンピオンを取得します
        /// </summary>
        List<ChampionData> GetAvailableChampions();

        /// <summary>
        /// 購入可能なすべてのアイテムを取得します
        /// </summary>
        List<ItemData> GetAvailableItems();

        /// <summary>
        /// チャンピオンを購入します
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>購入に成功したらtrue</returns>
        bool BuyChampion(string championId);

        /// <summary>
        /// アイテムを購入します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>購入に成功したらtrue</returns>
        bool BuyItem(string itemId, int quantity = 1);

        /// <summary>
        /// チャンピオンの価格を取得します
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>価格（コイン）</returns>
        int GetChampionPrice(string championId);

        /// <summary>
        /// チャンピオンが購入可能かチェックします
        /// </summary>
        /// <param name="championId">チャンピオンID</param>
        /// <returns>購入可能ならtrue</returns>
        bool CanBuyChampion(string championId);

        /// <summary>
        /// アイテムが購入可能かチェックします
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>購入可能ならtrue</returns>
        bool CanBuyItem(string itemId, int quantity = 1);

        /// <summary>チャンピオン購入時に発火するイベント</summary>
        event Action<ChampionData>? ChampionPurchased;

        /// <summary>アイテム購入時に発火するイベント</summary>
        event Action<string, int>? ItemPurchased;

        /// <summary>購入失敗時に発火するイベント（理由付き）</summary>
        event Action<string>? PurchaseFailed;
    }
}
