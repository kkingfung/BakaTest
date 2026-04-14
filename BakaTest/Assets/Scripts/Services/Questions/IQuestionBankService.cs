#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;

namespace BakaTest.Services.Questions
{
    /// <summary>
    /// 問題バンク管理サービスのインターフェース
    /// </summary>
    /// <remarks>
    /// 問題の取得、生成、管理を行います。
    /// 事前定義された問題バンクとAI生成問題の両方をサポートします。
    /// </remarks>
    public interface IQuestionBankService
    {
        /// <summary>テスト設定</summary>
        TestConfig Config { get; }

        /// <summary>
        /// ランダムな問題を取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <param name="count">取得する問題数</param>
        /// <returns>TestQuestionのリスト</returns>
        List<TestQuestion> GetRandomQuestions(Subject subject, DifficultyLevel difficulty, int count);

        /// <summary>
        /// 今日のテスト用問題プールを取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>TestQuestionのリスト</returns>
        List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty);

        /// <summary>
        /// 指定された教科・難易度の問題バンクをロードします
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>ロードに成功したらtrue</returns>
        bool LoadQuestionBank(Subject subject, DifficultyLevel difficulty);

        /// <summary>
        /// すべての問題バンクをロードします
        /// </summary>
        void LoadAllQuestionBanks();

        /// <summary>
        /// 問題バンクを登録します（エディタまたはランタイムで使用）
        /// </summary>
        /// <param name="bankData">問題バンクデータ</param>
        void RegisterQuestionBank(QuestionBankData bankData);

        /// <summary>
        /// 指定された教科・難易度の利用可能な問題数を取得します
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <returns>利用可能な問題数</returns>
        int GetAvailableQuestionCount(Subject subject, DifficultyLevel difficulty);

        /// <summary>
        /// すべての利用可能な問題数を取得します
        /// </summary>
        /// <returns>総問題数</returns>
        int GetTotalQuestionCount();

        /// <summary>
        /// AI生成問題を有効化/無効化します
        /// </summary>
        /// <param name="enabled">有効にする場合true</param>
        void SetAIGenerationEnabled(bool enabled);

        /// <summary>
        /// AI生成が有効かどうか
        /// </summary>
        bool IsAIGenerationEnabled { get; }

        /// <summary>
        /// AI生成問題を生成します（将来の実装用）
        /// </summary>
        /// <param name="subject">教科</param>
        /// <param name="difficulty">難易度</param>
        /// <param name="count">生成する問題数</param>
        /// <returns>生成されたTestQuestionのリスト</returns>
        List<TestQuestion> GenerateAIQuestions(Subject subject, DifficultyLevel difficulty, int count);

        /// <summary>問題バンクがロードされた時に発火</summary>
        event Action<Subject, DifficultyLevel>? QuestionBankLoaded;

        /// <summary>問題が生成された時に発火</summary>
        event Action<int>? QuestionsGenerated;
    }
}
