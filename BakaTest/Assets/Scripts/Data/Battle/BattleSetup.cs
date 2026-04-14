#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Data.Battle
{
    /// <summary>
    /// バトルモード
    /// </summary>
    public enum BattleMode
    {
        Individual,  // 個人戦 (1v1)
        Team,        // チーム戦 (5v5)
        Guild,       // ギルド戦
        Kingdom,     // 王国戦
        Simulation   // シミュレーション（ポイント消費なし）
    }

    /// <summary>
    /// バトル開始時の設定
    /// </summary>
    /// <remarks>
    /// IBattleService.StartBattle()に渡されます。
    /// </remarks>
    public class BattleSetup
    {
        /// <summary>バトルモード</summary>
        public BattleMode Mode { get; set; }

        /// <summary>プレイヤー1のチャンピオン</summary>
        public ChampionData Player1Champion { get; set; }

        /// <summary>プレイヤー1の名前</summary>
        public string Player1Name { get; set; }

        /// <summary>プレイヤー1のポイント配分</summary>
        public Dictionary<Subject, int> Player1Points { get; set; }

        /// <summary>プレイヤー2のチャンピオン</summary>
        public ChampionData Player2Champion { get; set; }

        /// <summary>プレイヤー2の名前</summary>
        public string Player2Name { get; set; }

        /// <summary>プレイヤー2のポイント配分</summary>
        public Dictionary<Subject, int> Player2Points { get; set; }

        /// <summary>バトル速度倍率（1.0 = 通常、2.0 = 2倍速）</summary>
        public float BattleSpeed { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BattleSetup(
            BattleMode mode,
            ChampionData player1Champion,
            string player1Name,
            Dictionary<Subject, int> player1Points,
            ChampionData player2Champion,
            string player2Name,
            Dictionary<Subject, int> player2Points,
            float battleSpeed = 1.0f)
        {
            Mode = mode;
            Player1Champion = player1Champion ?? throw new ArgumentNullException(nameof(player1Champion));
            Player1Name = player1Name ?? "Player 1";
            Player1Points = player1Points ?? new Dictionary<Subject, int>();
            Player2Champion = player2Champion ?? throw new ArgumentNullException(nameof(player2Champion));
            Player2Name = player2Name ?? "Player 2";
            Player2Points = player2Points ?? new Dictionary<Subject, int>();
            BattleSpeed = battleSpeed;
        }

        /// <summary>
        /// プレイヤー1のBattleUnitを作成
        /// </summary>
        public BattleUnit CreatePlayer1Unit()
        {
            return new BattleUnit(Player1Champion, Player1Name, Player1Points);
        }

        /// <summary>
        /// プレイヤー2のBattleUnitを作成
        /// </summary>
        public BattleUnit CreatePlayer2Unit()
        {
            return new BattleUnit(Player2Champion, Player2Name, Player2Points);
        }
    }
}
