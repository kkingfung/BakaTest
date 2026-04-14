#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Settings;

namespace BakaTest.Data.Save
{
    /// <summary>
    /// セーブデータのルートクラス
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>セーブデータのバージョン</summary>
        public int version = 1;

        /// <summary>最後に保存した日時</summary>
        public string lastSaveTime = string.Empty;

        /// <summary>プレイヤーデータ</summary>
        public PlayerSaveData playerData = new PlayerSaveData();

        /// <summary>チャンピオンデータ</summary>
        public ChampionSaveData championData = new ChampionSaveData();

        /// <summary>テスト履歴データ</summary>
        public TestHistorySaveData testHistory = new TestHistorySaveData();

        /// <summary>インベントリデータ</summary>
        public InventorySaveData inventoryData = new InventorySaveData();

        /// <summary>設定データ</summary>
        public SettingsData settingsData = SettingsData.GetDefault();

        public SaveData()
        {
            lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// プレイヤー関連のセーブデータ
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        /// <summary>プレイヤー名</summary>
        public string playerName = "Player";

        /// <summary>プレイヤーレベル</summary>
        public int playerLevel = 1;

        /// <summary>各教科の所持ポイント</summary>
        public Dictionary<Subject, int> points = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        /// <summary>バトルコイン</summary>
        public int battleCoins = 0;

        /// <summary>今日の日付（デイリーリセット用）</summary>
        public string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        /// <summary>今日獲得したポイント（教科別）</summary>
        public Dictionary<Subject, int> todaysEarnedPoints = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        /// <summary>総完了テスト数</summary>
        public int totalTestsCompleted = 0;

        /// <summary>総バトル数</summary>
        public int totalBattles = 0;

        /// <summary>バトル勝利数</summary>
        public int battlesWon = 0;
    }

    /// <summary>
    /// チャンピオン関連のセーブデータ
    /// </summary>
    [Serializable]
    public class ChampionSaveData
    {
        /// <summary>所有しているチャンピオンIDのリスト</summary>
        public List<string> ownedChampionIds = new List<string>();

        /// <summary>現在の無料ローテーションチャンピオンIDのリスト</summary>
        public List<string> freeRotationChampionIds = new List<string>();

        /// <summary>無料ローテーション最終更新日</summary>
        public string lastFreeRotationUpdate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// テスト履歴のセーブデータ
    /// </summary>
    [Serializable]
    public class TestHistorySaveData
    {
        /// <summary>各教科の最高スコア</summary>
        public Dictionary<Subject, int> highScores = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        /// <summary>各教科の受験回数</summary>
        public Dictionary<Subject, int> testCounts = new Dictionary<Subject, int>
        {
            { Subject.Math, 0 },
            { Subject.Science, 0 },
            { Subject.English, 0 },
            { Subject.History, 0 }
        };

        /// <summary>総獲得ポイント（累計）</summary>
        public int totalPointsEarned = 0;
    }

    /// <summary>
    /// インベントリのセーブデータ
    /// </summary>
    [Serializable]
    public class InventorySaveData
    {
        /// <summary>所有しているアイテム（アイテムID → 個数）</summary>
        public Dictionary<string, int> ownedItems = new Dictionary<string, int>();
    }
}
