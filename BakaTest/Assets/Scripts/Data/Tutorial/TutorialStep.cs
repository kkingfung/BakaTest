#nullable enable
using System;

namespace BakaTest.Data.Tutorial
{
    /// <summary>
    /// チュートリアルステップの種類
    /// </summary>
    public enum TutorialStepType
    {
        /// <summary>情報表示のみ（タップで次へ）</summary>
        Information,

        /// <summary>UI要素のハイライト（特定ボタンをタップ）</summary>
        HighlightUI,

        /// <summary>アクション実行（テスト受験、バトル等）</summary>
        ActionRequired,

        /// <summary>ナビゲーション（特定画面に遷移）</summary>
        NavigateToScene,

        /// <summary>待機（イベント発生待ち）</summary>
        WaitForEvent
    }

    /// <summary>
    /// チュートリアル完了条件のタイプ
    /// </summary>
    public enum TutorialConditionType
    {
        TapAnywhere,          // 画面タップで完了
        TapButton,            // 特定ボタンタップ
        CompleteTest,         // テスト完了
        SelectChampion,       // チャンピオン選択
        AllocatePoints,       // ポイント配分
        StartBattle,          // バトル開始
        BattleComplete,       // バトル完了
        Custom                // カスタム条件（スクリプトで判定）
    }

    /// <summary>
    /// チュートリアルステップの完了条件
    /// </summary>
    [Serializable]
    public class TutorialCompletionCondition
    {
        /// <summary>条件タイプ</summary>
        public TutorialConditionType type;

        /// <summary>対象の値（ボタン名、イベント名等）</summary>
        public string targetValue = string.Empty;

        /// <summary>数値条件（例: 5問正解）</summary>
        public int numericValue;
    }

    /// <summary>
    /// チュートリアルの1ステップを表すデータ
    /// </summary>
    [Serializable]
    public class TutorialStep
    {
        /// <summary>ステップID（一意）</summary>
        public string stepId = string.Empty;

        /// <summary>ステップタイプ</summary>
        public TutorialStepType stepType;

        /// <summary>タイトルのローカライゼーションキー</summary>
        public string titleKey = string.Empty;

        /// <summary>説明文のローカライゼーションキー</summary>
        public string descriptionKey = string.Empty;

        /// <summary>対象シーン名</summary>
        public string targetScene = string.Empty;

        /// <summary>ハイライトするUI要素名</summary>
        public string highlightElementName = string.Empty;

        /// <summary>完了条件</summary>
        public TutorialCompletionCondition completionCondition = new TutorialCompletionCondition();

        /// <summary>次のステップID</summary>
        public string nextStepId = string.Empty;

        /// <summary>スキップ可能か</summary>
        public bool canSkip = true;
    }
}
