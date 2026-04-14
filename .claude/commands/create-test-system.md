# Create Test System Components

テストシステム（AI生成、採点、ポイント管理）のコンポーネントを作成してください。

## 概要

ゲーム企画書に基づき、以下の機能を持つテストシステムを実装：
- 5択多肢選択問題
- 教科別・学年別の難易度
- 時間制限付き
- AIによる問題生成とレビュー
- 1日あたりの教科別スコア上限管理

## 作成するコンポーネント

### 1. TestQuestion.cs - 問題データ構造
```csharp
#nullable enable
using System;

namespace CavalryFight.Tests
{
    /// <summary>
    /// 教科
    /// </summary>
    public enum Subject
    {
        Math,     // 数学
        Science,  // 理科
        English,  // 英語
        History   // 歴史
    }

    /// <summary>
    /// 難易度（学年ベース）
    /// </summary>
    public enum DifficultyLevel
    {
        Elementary,   // 小学校
        MiddleSchool, // 中学校
        HighSchool,   // 高校
        University    // 大学
    }

    /// <summary>
    /// テスト問題データ
    /// </summary>
    [Serializable]
    public class TestQuestion
    {
        /// <summary>問題ID</summary>
        public string questionId = string.Empty;
        
        /// <summary>教科</summary>
        public Subject subject;
        
        /// <summary>難易度レベル</summary>
        public DifficultyLevel difficulty;
        
        /// <summary>問題文</summary>
        public string questionText = string.Empty;
        
        /// <summary>選択肢（5つ）</summary>
        public string[] choices = new string[5];
        
        /// <summary>正解のインデックス（0-4）</summary>
        public int correctAnswerIndex;
        
        /// <summary>この問題の獲得ポイント</summary>
        public int pointValue;
        
        /// <summary>生成日時</summary>
        public DateTime generatedDate;
        
        /// <summary>AI生成フラグ</summary>
        public bool isAIGenerated;
        
        /// <summary>AIレビュー済みフラグ</summary>
        public bool isReviewed;
    }
}
```

### 2. ITestService.cs - テストサービスインターフェース
```csharp
#nullable enable
using System;
using System.Collections.Generic;

namespace CavalryFight.Services.Tests
{
    /// <summary>
    /// テスト管理サービスのインターフェース
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// 指定教科・難易度の今日のテストプールを取得
        /// </summary>
        List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty);
        
        /// <summary>
        /// テスト結果を提出して採点
        /// </summary>
        TestResult SubmitTest(TestAttempt attempt);
        
        /// <summary>
        /// 今日の各教科の獲得済みポイントを取得
        /// </summary>
        Dictionary<Subject, int> GetTodaysEarnedPoints();
        
        /// <summary>
        /// 指定教科の今日の残り獲得可能ポイント
        /// </summary>
        int GetRemainingDailyPoints(Subject subject);
        
        /// <summary>
        /// 1日の教科別ポイント上限
        /// </summary>
        int DailyPointCap { get; }
        
        /// <summary>
        /// ポイントが獲得されたときのイベント
        /// </summary>
        event Action<Subject, int>? PointsEarned;
    }

    /// <summary>
    /// テスト受験記録
    /// </summary>
    public class TestAttempt
    {
        public Subject Subject { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public List<TestQuestion> Questions { get; set; } = new();
        public List<int> PlayerAnswers { get; set; } = new(); // -1 = 未回答
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// テスト結果
    /// </summary>
    public class TestResult
    {
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int PointsEarned { get; set; }
        public int PointsCapped { get; set; } // 上限で制限された分
        public bool ReachedDailyCap { get; set; }
        public TimeSpan TimeTaken { get; set; }
    }
}
```

### 3. ITestGeneratorService.cs - AI生成サービス
```csharp
#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CavalryFight.Services.Tests
{
    /// <summary>
    /// AIテスト生成サービスのインターフェース
    /// </summary>
    public interface ITestGeneratorService
    {
        /// <summary>
        /// 指定教科・難易度の問題を生成
        /// </summary>
        Task<TestQuestion> GenerateQuestionAsync(Subject subject, DifficultyLevel difficulty);
        
        /// <summary>
        /// 複数問題を一括生成
        /// </summary>
        Task<List<TestQuestion>> GenerateQuestionBatchAsync(Subject subject, DifficultyLevel difficulty, int count);
        
        /// <summary>
        /// 1日分のテストプールを生成（全教科・全難易度）
        /// </summary>
        Task GenerateDailyTestPoolAsync();
    }

    /// <summary>
    /// AIレビューサービスのインターフェース
    /// </summary>
    public interface ITestReviewerService
    {
        /// <summary>
        /// 問題の妥当性をレビュー
        /// </summary>
        Task<ReviewResult> ReviewQuestionAsync(TestQuestion question);
        
        /// <summary>
        /// 複数問題を一括レビュー
        /// </summary>
        Task<List<ReviewResult>> ReviewQuestionBatchAsync(List<TestQuestion> questions);
    }

    /// <summary>
    /// レビュー結果
    /// </summary>
    public class ReviewResult
    {
        public bool IsApproved { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public List<string> Issues { get; set; } = new();
    }
}
```

### 4. TestConfig.cs - 設定ScriptableObject
```csharp
#nullable enable
using UnityEngine;

namespace CavalryFight.Tests
{
    /// <summary>
    /// テストシステムの設定
    /// </summary>
    [CreateAssetMenu(fileName = "TestConfig", menuName = "BakaTest/Test Config")]
    public class TestConfig : ScriptableObject
    {
        [Header("基本設定")]
        /// <summary>1テストあたりの問題数</summary>
        public int questionsPerTest = 10;
        
        /// <summary>制限時間（秒）</summary>
        public float timeLimit = 300f; // 5分

        [Header("ポイント設定")]
        /// <summary>教科ごとの1日ポイント上限</summary>
        public int dailyPointCapPerSubject = 1000;
        
        /// <summary>難易度別の1問あたりポイント</summary>
        public int elementaryPointsPerQuestion = 10;
        public int middleSchoolPointsPerQuestion = 20;
        public int highSchoolPointsPerQuestion = 50;
        public int universityPointsPerQuestion = 100;

        /// <summary>
        /// 難易度に応じたポイント値を取得
        /// </summary>
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
    }
}
```

ユーザーにどのコンポーネントから実装を始めるか聞いてから、適切な実装を生成してください。
