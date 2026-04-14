#nullable enable

namespace BakaTest.Data.Localization
{
    /// <summary>
    /// ローカライゼーションデータのカテゴリ
    /// </summary>
    public enum LocalizationCategory
    {
        /// <summary>共通UI要素</summary>
        Common,

        /// <summary>メインメニュー</summary>
        MainMenu,

        /// <summary>テスト選択画面</summary>
        TestSelection,

        /// <summary>テスト受験画面</summary>
        TestTaking,

        /// <summary>チャンピオン選択画面</summary>
        ChampionSelection,

        /// <summary>ポイント配分画面</summary>
        PointAllocation,

        /// <summary>バトル画面</summary>
        Battle,

        /// <summary>結果画面</summary>
        Results,

        /// <summary>設定画面</summary>
        Settings,

        /// <summary>チャンピオン関連</summary>
        Champions,

        /// <summary>アイテム関連</summary>
        Items,

        /// <summary>エラーメッセージ</summary>
        Errors,

        /// <summary>チュートリアル</summary>
        Tutorial,

        /// <summary>通知</summary>
        Notifications
    }
}
