#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Data.Tutorial
{
    /// <summary>
    /// チュートリアルデータを保持するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "TutorialData", menuName = "BakaTest/Tutorial/Tutorial Data")]
    public class TutorialData : ScriptableObject
    {
        /// <summary>チュートリアルステップのリスト</summary>
        [SerializeField]
        public List<TutorialStep> steps = new List<TutorialStep>();

        /// <summary>
        /// 特定のステップIDを持つステップを取得します
        /// </summary>
        /// <param name="stepId">ステップID</param>
        /// <returns>見つかったステップ、なければnull</returns>
        public TutorialStep? GetStepById(string stepId)
        {
            return steps.Find(s => s.stepId == stepId);
        }

        /// <summary>
        /// 最初のステップを取得します
        /// </summary>
        /// <returns>最初のステップ、なければnull</returns>
        public TutorialStep? GetFirstStep()
        {
            return steps.Count > 0 ? steps[0] : null;
        }

        /// <summary>
        /// 次のステップを取得します
        /// </summary>
        /// <param name="currentStepId">現在のステップID</param>
        /// <returns>次のステップ、なければnull</returns>
        public TutorialStep? GetNextStep(string currentStepId)
        {
            var currentStep = GetStepById(currentStepId);
            if (currentStep == null || string.IsNullOrEmpty(currentStep.nextStepId))
            {
                return null;
            }

            return GetStepById(currentStep.nextStepId);
        }

        /// <summary>
        /// ステップの総数を取得します
        /// </summary>
        public int TotalSteps => steps.Count;
    }
}
