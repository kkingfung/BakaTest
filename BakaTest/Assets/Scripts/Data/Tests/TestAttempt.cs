#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Data.Tests
{
    /// <summary>
    /// テスト受験記録
    /// </summary>
    [Serializable]
    public class TestAttempt
    {
        /// <summary>受験ID</summary>
        public string attemptId = string.Empty;
        
        /// <summary>教科</summary>
        public Subject subject;
        
        /// <summary>難易度</summary>
        public DifficultyLevel difficulty;
        
        /// <summary>問題リスト</summary>
        public List<TestQuestion> questions = new();
        
        /// <summary>プレイヤーの回答（-1 = 未回答）</summary>
        public List<int> playerAnswers = new();
        
        /// <summary>開始時刻</summary>
        public DateTime startTime;
        
        /// <summary>終了時刻</summary>
        public DateTime? endTime;

        /// <summary>
        /// 受験中かどうか
        /// </summary>
        public bool IsInProgress => endTime == null;

        /// <summary>
        /// 所要時間
        /// </summary>
        public TimeSpan TimeTaken
        {
            get
            {
                if (endTime == null) return TimeSpan.Zero;
                return endTime.Value - startTime;
            }
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public TestAttempt()
        {
            attemptId = Guid.NewGuid().ToString();
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 回答を記録します
        /// </summary>
        /// <param name="questionIndex">問題のインデックス</param>
        /// <param name="answerIndex">回答のインデックス</param>
        public void RecordAnswer(int questionIndex, int answerIndex)
        {
            if (questionIndex < 0 || questionIndex >= playerAnswers.Count)
            {
                return;
            }

            playerAnswers[questionIndex] = answerIndex;
        }

        /// <summary>
        /// テストを終了します
        /// </summary>
        public void EndTest()
        {
            if (endTime == null)
            {
                endTime = DateTime.Now;
            }
        }

        public override string ToString()
        {
            return $"Test Attempt [{subject}] {difficulty} - {questions.Count} questions, {(IsInProgress ? "In Progress" : $"Completed in {TimeTaken.TotalMinutes:F1} min")}";
        }
    }
}
