#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Items;

namespace BakaTest.Services.Inventory
{
    /// <summary>
    /// インベントリ管理サービスのインターフェース
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>所有しているアイテムのリスト（アイテムID → 個数）</summary>
        Dictionary<string, int> OwnedItems { get; }

        /// <summary>インベントリの最大容量（0 = 無制限）</summary>
        int MaxCapacity { get; }

        /// <summary>現在のアイテム総数</summary>
        int CurrentItemCount { get; }

        /// <summary>
        /// アイテムを追加します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>追加に成功したらtrue</returns>
        bool AddItem(string itemId, int quantity = 1);

        /// <summary>
        /// アイテムを削除します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>削除に成功したらtrue</returns>
        bool RemoveItem(string itemId, int quantity = 1);

        /// <summary>
        /// アイテムを所持しているか確認します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">必要個数</param>
        /// <returns>所持していればtrue</returns>
        bool HasItem(string itemId, int quantity = 1);

        /// <summary>
        /// アイテムの所持数を取得します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <returns>所持数</returns>
        int GetItemCount(string itemId);

        /// <summary>
        /// アイテムを購入します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>購入に成功したらtrue</returns>
        bool BuyItem(string itemId, int quantity = 1);

        /// <summary>
        /// アイテムを売却します
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        /// <param name="quantity">個数</param>
        /// <returns>売却に成功したらtrue</returns>
        bool SellItem(string itemId, int quantity = 1);

        /// <summary>
        /// インベントリをクリアします
        /// </summary>
        void ClearInventory();

        /// <summary>アイテムが追加されたときに発火するイベント</summary>
        event Action<string, int>? ItemAdded;

        /// <summary>アイテムが削除されたときに発火するイベント</summary>
        event Action<string, int>? ItemRemoved;

        /// <summary>アイテムが購入されたときに発火するイベント</summary>
        event Action<string, int>? ItemPurchased;

        /// <summary>アイテムが売却されたときに発火するイベント</summary>
        event Action<string, int>? ItemSold;
    }
}
