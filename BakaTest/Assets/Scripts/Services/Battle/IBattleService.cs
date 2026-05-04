#nullable enable
using System;
using BakaTest.Data.Battle;
using BakaTest.Data.Items;

namespace BakaTest.Services.Battle
{
    /// <summary>
    /// バトル管理サービスのインターフェース
    /// </summary>
    public interface IBattleService
    {
        /// <summary>
        /// 現在進行中のバトル結果（バトル中のみ）
        /// </summary>
        BattleResult? CurrentBattle { get; }

        /// <summary>
        /// バトルが進行中かどうか
        /// </summary>
        bool IsBattleInProgress { get; }

        /// <summary>
        /// 次のバトルのセットアップ（シーン間データ受け渡し用）
        /// </summary>
        /// <remarks>
        /// PointAllocationViewでバトルセットアップを作成後、ここに設定してBattleシーンに遷移します。
        /// BattleViewはこのプロパティを読み取ってバトルを開始します。
        /// </remarks>
        BattleSetup? PendingBattleSetup { get; set; }

        /// <summary>
        /// プレイヤー1のバトルユニット（現在のHP含む）
        /// </summary>
        BattleUnit? Player1Unit { get; }

        /// <summary>
        /// プレイヤー2のバトルユニット（現在のHP含む）
        /// </summary>
        BattleUnit? Player2Unit { get; }

        /// <summary>
        /// 現在のターン数
        /// </summary>
        int CurrentTurn { get; }

        /// <summary>
        /// バトルを開始します
        /// </summary>
        /// <param name="setup">バトル設定</param>
        void StartBattle(BattleSetup setup);

        /// <summary>
        /// バトルをシミュレーション実行します（同期処理、全ターン一括）
        /// </summary>
        /// <returns>バトル結果</returns>
        BattleResult? SimulateBattle();

        /// <summary>
        /// 1ターンだけ処理を進めます（ターンベース用）
        /// </summary>
        /// <returns>バトルが終了した場合はBattleResult、継続中はnull</returns>
        BattleResult? ProcessSingleTurn();

        /// <summary>
        /// アイテムを使用します（プレイヤー1のみ・レガシー）
        /// </summary>
        /// <param name="itemId">アイテムID</param>
        void UseItem(string itemId);

        /// <summary>
        /// アイテムを使用します（プレイヤー1のみ）
        /// </summary>
        /// <param name="itemData">使用するアイテム</param>
        /// <param name="useOnEnemy">敵に使用する場合はtrue（デバフアイテム等）</param>
        void UseItem(ItemData itemData, bool useOnEnemy = false);

        /// <summary>
        /// バトルを中断します
        /// </summary>
        void AbortBattle();

        /// <summary>
        /// バトルが開始されたときに発火するイベント
        /// </summary>
        event Action<BattleSetup>? BattleStarted;

        /// <summary>
        /// バトルアクションが発生したときに発火するイベント
        /// </summary>
        event Action<BattleAction>? ActionPerformed;

        /// <summary>
        /// バトルが終了したときに発火するイベント
        /// </summary>
        event Action<BattleResult>? BattleEnded;
    }
}
