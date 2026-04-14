#nullable enable
using UnityEngine;

namespace BakaTest.Data.Tests
{
    /// <summary>
    /// テストシステムの設定
    /// </summary>
    [CreateAssetMenu(fileName = "TestConfig", menuName = "BakaTest/Test Config", order = 2)]
    public class TestConfig : ScriptableObject
    {
        [Header("基本設定")]
        /// <summary>1テストあたりの問題数</summary>
        [Tooltip("各テストで出題される問題数")]
        public int questionsPerTest = 10;
        
        /// <summary>制限時間（秒）</summary>
        [Tooltip("テストの制限時間（全学年共通）")]
        public float timeLimit = 300f; // 5分

        [Header("ポイント設定")]
        /// <summary>教科ごとの1日ポイント上限</summary>
        [Tooltip("各教科で1日に獲得できる最大ポイント")]
        public int dailyPointCapPerSubject = 1000;
        
        [Header("難易度別ポイント")]
        /// <summary>小学校レベルの1問あたりポイント</summary>
        [Tooltip("Elementary難易度の問題1問あたりの獲得ポイント")]
        public int elementaryPointsPerQuestion = 10;
        
        /// <summary>中学校レベルの1問あたりポイント</summary>
        [Tooltip("MiddleSchool難易度の問題1問あたりの獲得ポイント")]
        public int middleSchoolPointsPerQuestion = 20;
        
        /// <summary>高校レベルの1問あたりポイント</summary>
        [Tooltip("HighSchool難易度の問題1問あたりの獲得ポイント")]
        public int highSchoolPointsPerQuestion = 50;
        
        /// <summary>大学レベルの1問あたりポイント</summary>
        [Tooltip("University難易度の問題1問あたりの獲得ポイント")]
        public int universityPointsPerQuestion = 100;

        /// <summary>
        /// 難易度に応じたポイント値を取得します
        /// </summary>
        /// <param name="difficulty">難易度</param>
        /// <returns>1問あたりのポイント</returns>
        public int GetPointsForDifficulty(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Elementary => elementaryPointsPerQuestion,
                DifficultyLevel.MiddleSchool => middleSchoolPointsPerQuestion,
                DifficultyLevel.HighSchool => highSchoolPointsPerQuestion,
                DifficultyLevel.University => universityPointsPerQuestion,
                _ => elementaryPointsPerQuestion
            };
        }

        /// <summary>
        /// テストの満点スコアを計算します
        /// </summary>
        /// <param name="difficulty">難易度</param>
        /// <returns>満点スコア</returns>
        public int GetMaxScoreForTest(DifficultyLevel difficulty)
        {
            return GetPointsForDifficulty(difficulty) * questionsPerTest;
        }

        #region Editor Validation

        private void OnValidate()
        {
            // 問題数は1以上
            if (questionsPerTest < 1)
            {
                questionsPerTest = 1;
            }

            // 制限時間は30秒以上
            if (timeLimit < 30f)
            {
                timeLimit = 30f;
            }

            // ポイント設定は0以上
            if (dailyPointCapPerSubject < 0)
            {
                dailyPointCapPerSubject = 0;
            }

            if (elementaryPointsPerQuestion < 0) elementaryPointsPerQuestion = 0;
            if (middleSchoolPointsPerQuestion < 0) middleSchoolPointsPerQuestion = 0;
            if (highSchoolPointsPerQuestion < 0) highSchoolPointsPerQuestion = 0;
            if (universityPointsPerQuestion < 0) universityPointsPerQuestion = 0;
        }

        #endregion
    }
}
