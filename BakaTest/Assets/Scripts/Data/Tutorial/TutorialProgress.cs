#nullable enable
using System;
using System.Collections.Generic;

namespace BakaTest.Data.Tutorial
{
    /// <summary>
    /// チュートリアル進行状況
    /// </summary>
    [Serializable]
    public class TutorialProgress
    {
        /// <summary>チュートリアル完了済みか</summary>
        public bool isCompleted;

        /// <summary>現在のステップID</summary>
        public string currentStepId = string.Empty;

        /// <summary>完了したステップIDリスト</summary>
        public List<string> completedSteps = new List<string>();

        /// <summary>チュートリアルスキップ済みか</summary>
        public bool wasSkipped;

        /// <summary>最終更新日時</summary>
        public DateTime lastUpdated = DateTime.Now;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TutorialProgress()
        {
            isCompleted = false;
            currentStepId = string.Empty;
            completedSteps = new List<string>();
            wasSkipped = false;
            lastUpdated = DateTime.Now;
        }
    }
}
