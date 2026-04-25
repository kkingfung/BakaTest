#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Localization;

namespace BakaTest.Data.Battle
{
    /// <summary>
    /// バトルの結果
    /// </summary>
    /// <remarks>
    /// バトル終了時にIBattleService.BattleEndedイベントで通知されます。
    /// </remarks>
    public class BattleResult
    {
        /// <summary>バトル設定</summary>
        public BattleSetup Setup { get; set; }

        /// <summary>勝者のユニット</summary>
        public BattleUnit? Winner { get; set; }

        /// <summary>敗者のユニット</summary>
        public BattleUnit? Loser { get; set; }

        /// <summary>プレイヤー1が勝利したかどうか</summary>
        public bool Player1Won => Winner?.PlayerName == Setup.Player1Name;

        /// <summary>プレイヤー2が勝利したかどうか</summary>
        public bool Player2Won => Winner?.PlayerName == Setup.Player2Name;

        /// <summary>バトルアクションの履歴</summary>
        public List<BattleAction> ActionLog { get; set; }

        /// <summary>バトル開始時刻</summary>
        public DateTime StartTime { get; set; }

        /// <summary>バトル終了時刻</summary>
        public DateTime EndTime { get; set; }

        /// <summary>バトル所要時間</summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>ターン数</summary>
        public int TurnCount { get; set; }

        /// <summary>獲得したバトルコイン</summary>
        public int CoinsEarned { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleResult(
            BattleSetup setup,
            BattleUnit? winner,
            BattleUnit? loser,
            List<BattleAction> actionLog,
            DateTime startTime,
            int turnCount)
        {
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
            Winner = winner;
            Loser = loser;
            ActionLog = actionLog ?? new List<BattleAction>();
            StartTime = startTime;
            EndTime = DateTime.Now;
            TurnCount = turnCount;

            // 勝利報酬を計算
            CalculateRewards();
        }

        /// <summary>
        /// 勝利報酬を計算します
        /// </summary>
        private void CalculateRewards()
        {
            // シミュレーションモードは報酬なし
            if (Setup.Mode == BattleMode.Simulation)
            {
                CoinsEarned = 0;
                return;
            }

            // 勝利した場合のみコインを獲得
            if (Winner != null)
            {
                // 基本報酬: 10コイン
                int baseReward = 10;

                // モードによるボーナス
                int modeBonus = Setup.Mode switch
                {
                    BattleMode.Individual => 5,
                    BattleMode.Team => 10,
                    BattleMode.Guild => 20,
                    BattleMode.Kingdom => 50,
                    _ => 0
                };

                CoinsEarned = baseReward + modeBonus;
                UnityEngine.Debug.Log($"[BattleResult] {Winner.PlayerName} earned {CoinsEarned} coins!");
            }
            else
            {
                CoinsEarned = 0;
            }
        }

        /// <summary>
        /// バトルログの要約を取得します
        /// </summary>
        public string GetSummary(Language language)
        {
            if (Winner == null)
            {
                return "バトルは引き分けに終わりました。";
            }

            string winnerName = Winner.ChampionData.GetChampionName(language);
            string loserName = Loser?.ChampionData.GetChampionName(language) ?? "Unknown";

            return $"{winnerName} が {loserName} に勝利しました！\n" +
                   $"ターン数: {TurnCount}\n" +
                   $"所要時間: {Duration.TotalSeconds:F1}秒\n" +
                   $"獲得コイン: {CoinsEarned}";
        }
    }
}
