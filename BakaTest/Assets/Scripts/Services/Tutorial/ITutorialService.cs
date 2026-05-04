#nullable enable
using System;
using BakaTest.Data.Tutorial;

namespace BakaTest.Services.Tutorial
{
    /// <summary>
    /// チュートリアルサービスインターフェース
    /// </summary>
    public interface ITutorialService
    {
        /// <summary>チュートリアル中かどうか</summary>
        bool IsInTutorial { get; }

        /// <summary>現在のチュートリアル進行状況</summary>
        TutorialProgress Progress { get; }

        /// <summary>現在のステップ</summary>
        TutorialStep? CurrentStep { get; }

        /// <summary>
        /// チュートリアルを開始します
        /// </summary>
        void StartTutorial();

        /// <summary>
        /// チュートリアルをスキップします
        /// </summary>
        void SkipTutorial();

        /// <summary>
        /// 次のステップに進みます
        /// </summary>
        void NextStep();

        /// <summary>
        /// 特定のステップに移動します
        /// </summary>
        /// <param name="stepId">ステップID</param>
        void GoToStep(string stepId);

        /// <summary>
        /// 現在のステップを取得します
        /// </summary>
        /// <returns>現在のステップ、なければnull</returns>
        TutorialStep? GetCurrentStep();

        /// <summary>
        /// 現在のステップを完了します
        /// </summary>
        void CompleteCurrentStep();

        /// <summary>
        /// 特定の機能がアンロックされているかチェックします
        /// </summary>
        /// <param name="featureName">機能名</param>
        /// <returns>アンロック済みならtrue</returns>
        bool IsFeatureUnlocked(string featureName);

        /// <summary>
        /// チュートリアルの進行状況をリセットします
        /// </summary>
        void ResetTutorial();

        /// <summary>ステップが変更された時に発火</summary>
        event Action<TutorialStep>? StepChanged;

        /// <summary>チュートリアルが完了した時に発火</summary>
        event Action? TutorialCompleted;

        /// <summary>チュートリアルがスキップされた時に発火</summary>
        event Action? TutorialSkipped;
    }
}
