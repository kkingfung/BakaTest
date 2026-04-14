#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;

namespace BakaTest.Services.Tests
{
    /// <summary>
    /// テスト管理サービスのインターフェース
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// 現在のテストセッション（受験中の場合）
        /// </summary>
        TestAttempt? CurrentAttempt { get; }

        /// <summary>
        /// テスト設定
        /// </summary>
        TestConfig Config { get; }

        /// <summary>
        /// テストが受験中かどうか
        /// </summary>
        bool IsTestInProgress { get; }

        /// <summary>
        /// 1日あたりの教科別ポイント上限
        /// </summary>
        int DailyPointCap { get; }

        /// <summary>
        /// 今日のテストプールを取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>問題のリスト</returns>
        List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty);

        /// <summary>
        /// テストを開始します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>開始されたテストセッション</returns>
        TestAttempt StartTest(Subject subject, DifficultyLevel difficulty);

        /// <summary>
        /// 回答を記録します
        /// </summary>
        /// <param name="questionIndex">問題のインデックス</param>
        /// <param name="answerIndex">回答のインデックス</param>
        void RecordAnswer(int questionIndex, int answerIndex);

        /// <summary>
        /// テストを提出して採点します
        /// </summary>
        /// <returns>テスト結果</returns>
        TestResult SubmitTest();

        /// <summary>
        /// 指定教科の今日の残り獲得可能ポイントを取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <returns>残りポイント</returns>
        int GetRemainingDailyPoints(Subject subject);

        /// <summary>
        /// テストが開始されたときに発火するイベント
        /// </summary>
        event Action<TestAttempt>? TestStarted;

        /// <summary>
        /// テストが提出されたときに発火するイベント
        /// </summary>
        event Action<TestResult>? TestSubmitted;

        /// <summary>
        /// ポイントが獲得されたときに発火するイベント
        /// </summary>
        event Action<Subject, int>? PointsEarned;
    }
}
