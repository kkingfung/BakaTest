#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Champions;

namespace BakaTest.Data.Tests
{
    /// <summary>
    /// 問題バンクデータ（ScriptableObject）
    /// </summary>
    /// <remarks>
    /// 事前定義された問題を格納します。
    /// エディタで作成し、リソースとして保存します。
    /// </remarks>
    [CreateAssetMenu(fileName = "QuestionBank", menuName = "BakaTest/Question Bank", order = 3)]
    public class QuestionBankData : ScriptableObject
    {
        [Header("問題バンク情報")]
        /// <summary>バンクID</summary>
        [Tooltip("このバンクの識別子")]
        public string bankId = "default_bank";
        
        /// <summary>バンク名</summary>
        [Tooltip("このバンクの表示名")]
        public string bankName = "Default Question Bank";
        
        /// <summary>教科</summary>
        [Tooltip("このバンクの対象教科")]
        public Subject subject;
        
        /// <summary>難易度</summary>
        [Tooltip("このバンクの難易度レベル")]
        public DifficultyLevel difficulty;
        
        [Header("問題リスト")]
        /// <summary>問題リスト</summary>
        [Tooltip("このバンクに含まれる全問題")]
        public List<QuestionEntry> questions = new List<QuestionEntry>();

        /// <summary>
        /// 問題エントリ（シリアライズ可能）
        /// </summary>
        [Serializable]
        public class QuestionEntry
        {
            /// <summary>問題文</summary>
            [TextArea(3, 10)]
            public string questionText = string.Empty;
            
            /// <summary>選択肢1</summary>
            public string choice0 = string.Empty;
            
            /// <summary>選択肢2</summary>
            public string choice1 = string.Empty;
            
            /// <summary>選択肢3</summary>
            public string choice2 = string.Empty;
            
            /// <summary>選択肢4</summary>
            public string choice3 = string.Empty;
            
            /// <summary>選択肢5</summary>
            public string choice4 = string.Empty;
            
            /// <summary>正解のインデックス（0-4）</summary>
            [Range(0, 4)]
            public int correctAnswerIndex = 0;
            
            /// <summary>解説（オプション）</summary>
            [TextArea(2, 5)]
            public string explanation = string.Empty;

            /// <summary>
            /// TestQuestionに変換します
            /// </summary>
            public TestQuestion ToTestQuestion(Subject subject, DifficultyLevel difficulty, int pointValue)
            {
                return new TestQuestion
                {
                    questionId = Guid.NewGuid().ToString(),
                    subject = subject,
                    difficulty = difficulty,
                    questionText = questionText,
                    choices = new string[] { choice0, choice1, choice2, choice3, choice4 },
                    correctAnswerIndex = correctAnswerIndex,
                    pointValue = pointValue,
                    generatedDate = DateTime.Now,
                    isAIGenerated = false,
                    isReviewed = true
                };
            }
        }

        /// <summary>
        /// このバンクから問題を取得します
        /// </summary>
        /// <param name="count">取得する問題数</param>
        /// <param name="pointValue">1問あたりのポイント値</param>
        /// <returns>TestQuestionのリスト</returns>
        public List<TestQuestion> GetQuestions(int count, int pointValue)
        {
            var result = new List<TestQuestion>();
            
            if (questions.Count == 0)
            {
                Debug.LogWarning($"[QuestionBankData] バンク '{bankName}' には問題がありません。");
                return result;
            }

            // ランダムにシャッフルして取得
            var shuffled = new List<QuestionEntry>(questions);
            ShuffleList(shuffled);

            int actualCount = Math.Min(count, shuffled.Count);
            for (int i = 0; i < actualCount; i++)
            {
                result.Add(shuffled[i].ToTestQuestion(subject, difficulty, pointValue));
            }

            return result;
        }

        /// <summary>
        /// すべての問題を取得します
        /// </summary>
        /// <param name="pointValue">1問あたりのポイント値</param>
        /// <returns>TestQuestionのリスト</returns>
        public List<TestQuestion> GetAllQuestions(int pointValue)
        {
            var result = new List<TestQuestion>();
            
            foreach (var entry in questions)
            {
                result.Add(entry.ToTestQuestion(subject, difficulty, pointValue));
            }

            return result;
        }

        /// <summary>
        /// リストをシャッフルします（Fisher-Yates）
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            var rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #region Editor Validation

        private void OnValidate()
        {
            // バンクIDが空の場合、自動生成
            if (string.IsNullOrEmpty(bankId))
            {
                bankId = $"{subject}_{difficulty}_{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            // バンク名が空の場合、デフォルト名を設定
            if (string.IsNullOrEmpty(bankName))
            {
                bankName = $"{subject} - {difficulty}";
            }
        }

        #endregion
    }
}
