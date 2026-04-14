#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;

namespace BakaTest.Data.Tests
{
    /// <summary>
    /// テスト結果データ
    /// </summary>
    [Serializable]
    public class TestResult
    {
        /// <summary>結果ID</summary>
        public string resultId = string.Empty;
        
        /// <summary>教科</summary>
        public Subject subject;
        
        /// <summary>難易度</summary>
        public DifficultyLevel difficulty;
        
        /// <summary>総問題数</summary>
        public int totalQuestions;
        
        /// <summary>正解数</summary>
        public int correctAnswers;
        
        /// <summary>不正解数</summary>
        public int incorrectAnswers => totalQuestions - correctAnswers;
        
        /// <summary>未回答数</summary>
        public int unansweredQuestions;
        
        /// <summary>正答率（パーセント）</summary>
        public float scorePercent => totalQuestions > 0 ? (correctAnswers / (float)totalQuestions) * 100f : 0f;
        
        /// <summary>所要時間</summary>
        public TimeSpan timeTaken;
        
        /// <summary>獲得ポイント</summary>
        public int pointsEarned;

        /// <summary>制限で減らされたポイント数</summary>
        public int pointsCapped;

        /// <summary>デイリー上限に達したかどうか</summary>
        public bool reachedDailyCap;

        /// <summary>テスト受験記録への参照</summary>
        public TestAttempt? attempt;
        
        /// <summary>各問題の結果詳細</summary>
        public List<QuestionResult> questionResults = new();
        
        /// <summary>完了日時</summary>
        public DateTime completedAt;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public TestResult()
        {
            resultId = Guid.NewGuid().ToString();
            completedAt = DateTime.Now;
        }

        /// <summary>
        /// 成績評価を取得します
        /// </summary>
        /// <returns>評価文字列</returns>
        public string GetGrade()
        {
            if (scorePercent >= 90f) return "Excellent!";
            if (scorePercent >= 80f) return "Great!";
            if (scorePercent >= 70f) return "Good!";
            if (scorePercent >= 60f) return "Fair";
            if (scorePercent >= 50f) return "Pass";
            return "Need More Practice";
        }

        /// <summary>
        /// TestAttemptから TestResultを生成します
        /// </summary>
        public static TestResult FromAttempt(TestAttempt attempt, int pointsEarned, int pointsCapped, bool reachedCap)
        {
            var result = new TestResult
            {
                subject = attempt.subject,
                difficulty = attempt.difficulty,
                totalQuestions = attempt.questions.Count,
                timeTaken = attempt.TimeTaken,
                pointsEarned = pointsEarned,
                pointsCapped = pointsCapped,
                reachedDailyCap = reachedCap,
                attempt = attempt,
                completedAt = DateTime.Now
            };

            // 各問題の結果を計算
            int correctCount = 0;
            int unansweredCount = 0;

            for (int i = 0; i < attempt.questions.Count; i++)
            {
                var question = attempt.questions[i];
                int playerAnswer = i < attempt.playerAnswers.Count ? attempt.playerAnswers[i] : -1;
                bool isCorrect = playerAnswer != -1 && question.IsCorrect(playerAnswer);

                if (playerAnswer == -1)
                    unansweredCount++;

                if (isCorrect)
                    correctCount++;

                result.questionResults.Add(new QuestionResult(i, question, playerAnswer, isCorrect));
            }

            result.correctAnswers = correctCount;
            result.unansweredQuestions = unansweredCount;

            return result;
        }

        public override string ToString()
        {
            return $"Test Result [{subject}] {difficulty} - {correctAnswers}/{totalQuestions} ({scorePercent:F1}%) - {pointsEarned} pts";
        }
    }

    /// <summary>
    /// 個別問題の結果
    /// </summary>
    [Serializable]
    public class QuestionResult
    {
        /// <summary>問題インデックス</summary>
        public int questionIndex;
        
        /// <summary>問題データ</summary>
        public TestQuestion? question;
        
        /// <summary>プレイヤーの回答（-1 = 未回答）</summary>
        public int playerAnswer;
        
        /// <summary>正解かどうか</summary>
        public bool isCorrect;
        
        /// <summary>スキップされたかどうか</summary>
        public bool wasSkipped => playerAnswer == -1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QuestionResult(int index, TestQuestion question, int answer, bool correct)
        {
            questionIndex = index;
            this.question = question;
            playerAnswer = answer;
            isCorrect = correct;
        }
    }
}
