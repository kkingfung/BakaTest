#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Services.Player
{
    /// <summary>
    /// プレイヤーデータ管理サービスのインターフェース
    /// </summary>
    public interface IPlayerDataService
    {
        /// <summary>
        /// 指定教科のテストポイントを取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <returns>現在のポイント</returns>
        int GetPoints(Subject subject);

        /// <summary>
        /// すべての教科のポイントを取得します
        /// </summary>
        /// <returns>教科ごとのポイント辞書</returns>
        Dictionary<Subject, int> GetAllPoints();

        /// <summary>
        /// ポイントを追加します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="amount">追加するポイント</param>
        void AddPoints(Subject subject, int amount);

        /// <summary>
        /// ポイントを消費します
        /// </summary>
        /// <param name="points">消費する教科ごとのポイント</param>
        /// <returns>消費に成功した場合true</returns>
        bool SpendPoints(Dictionary<Subject, int> points);

        /// <summary>
        /// 指定教科の今日獲得したポイントを取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <returns>今日獲得したポイント</returns>
        int GetTodaysEarnedPoints(Subject subject);

        /// <summary>
        /// 今日獲得したポイントをリセットします（日付変更時）
        /// </summary>
        void ResetDailyPoints();

        /// <summary>
        /// バトルコインの残高
        /// </summary>
        int BattleCoins { get; }

        /// <summary>
        /// バトルコインを追加します
        /// </summary>
        /// <param name="amount">追加するコイン数</param>
        void AddCoins(int amount);

        /// <summary>
        /// バトルコインを消費します
        /// </summary>
        /// <param name="amount">消費するコイン数</param>
        /// <returns>消費に成功した場合true</returns>
        bool SpendCoins(int amount);

        /// <summary>
        /// ポイントが変更されたときに発火するイベント
        /// </summary>
        event Action<Subject, int>? PointsChanged;

        /// <summary>
        /// コインが変更されたときに発火するイベント
        /// </summary>
        event Action<int>? CoinsChanged;
    }
}
