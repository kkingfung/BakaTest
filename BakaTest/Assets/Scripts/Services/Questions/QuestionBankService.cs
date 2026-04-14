#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;

namespace BakaTest.Services.Questions
{
    /// <summary>
    /// 問題バンク管理サービスの実装
    /// </summary>
    public class QuestionBankService : IQuestionBankService
    {
        private readonly TestConfig _config;
        private readonly Dictionary<string, QuestionBankData> _questionBanks;
        private readonly System.Random _random;
        private bool _aiGenerationEnabled;

        /// <summary>テスト設定</summary>
        public TestConfig Config => _config;

        /// <summary>AI生成が有効かどうか</summary>
        public bool IsAIGenerationEnabled => _aiGenerationEnabled;

        /// <summary>問題バンクがロードされた時に発火</summary>
        public event Action<Subject, DifficultyLevel>? QuestionBankLoaded;

        /// <summary>問題が生成された時に発火</summary>
        public event Action<int>? QuestionsGenerated;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="config">テスト設定</param>
        public QuestionBankService(TestConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _questionBanks = new Dictionary<string, QuestionBankData>();
            _random = new System.Random();
            _aiGenerationEnabled = false;

            Debug.Log("[QuestionBankService] Initialized.");
        }

        /// <summary>
        /// ランダムな問題を取得します
        /// </summary>
        public List<TestQuestion> GetRandomQuestions(Subject subject, DifficultyLevel difficulty, int count)
        {
            string key = GetBankKey(subject, difficulty);
            
            if (_questionBanks.TryGetValue(key, out var bank))
            {
                int pointValue = _config.GetPointsForDifficulty(difficulty);
                var questions = bank.GetQuestions(count, pointValue);
                
                Debug.Log($"[QuestionBankService] Retrieved {questions.Count} random questions for {subject} {difficulty}");
                return questions;
            }

            Debug.LogWarning($"[QuestionBankService] No question bank found for {subject} {difficulty}");
            
            // 問題バンクがない場合、AI生成を試みる
            if (_aiGenerationEnabled)
            {
                Debug.Log($"[QuestionBankService] Attempting AI generation for {subject} {difficulty}");
                return GenerateAIQuestions(subject, difficulty, count);
            }

            return new List<TestQuestion>();
        }

        /// <summary>
        /// 今日のテスト用問題プールを取得します
        /// </summary>
        public List<TestQuestion> GetTodaysTestPool(Subject subject, DifficultyLevel difficulty)
        {
            // 今日の問題プールとして、通常の2倍の問題を用意
            int poolSize = _config.questionsPerTest * 2;
            return GetRandomQuestions(subject, difficulty, poolSize);
        }

        /// <summary>
        /// 指定された教科・難易度の問題バンクをロードします
        /// </summary>
        public bool LoadQuestionBank(Subject subject, DifficultyLevel difficulty)
        {
            // Resources/Data/Questions/{Subject}_{Difficulty} の形式で検索
            string resourcePath = $"Data/Questions/{subject}_{difficulty}";
            var bankData = Resources.Load<QuestionBankData>(resourcePath);

            if (bankData != null)
            {
                RegisterQuestionBank(bankData);
                QuestionBankLoaded?.Invoke(subject, difficulty);
                Debug.Log($"[QuestionBankService] Loaded question bank from Resources: {resourcePath}");
                return true;
            }

            Debug.LogWarning($"[QuestionBankService] Failed to load question bank from Resources: {resourcePath}");
            return false;
        }

        /// <summary>
        /// すべての問題バンクをロードします
        /// </summary>
        public void LoadAllQuestionBanks()
        {
            Debug.Log("[QuestionBankService] Loading all question banks from Resources...");

            int loadedCount = 0;

            // すべての教科と難易度の組み合わせを試す
            foreach (Subject subject in Enum.GetValues(typeof(Subject)))
            {
                foreach (DifficultyLevel difficulty in Enum.GetValues(typeof(DifficultyLevel)))
                {
                    if (LoadQuestionBank(subject, difficulty))
                    {
                        loadedCount++;
                    }
                }
            }

            Debug.Log($"[QuestionBankService] Loaded {loadedCount} question banks.");
        }

        /// <summary>
        /// 問題バンクを登録します
        /// </summary>
        public void RegisterQuestionBank(QuestionBankData bankData)
        {
            if (bankData == null)
            {
                Debug.LogError("[QuestionBankService] Cannot register null question bank.");
                return;
            }

            string key = GetBankKey(bankData.subject, bankData.difficulty);
            
            if (_questionBanks.ContainsKey(key))
            {
                Debug.LogWarning($"[QuestionBankService] Overwriting existing question bank for {bankData.subject} {bankData.difficulty}");
            }

            _questionBanks[key] = bankData;
            Debug.Log($"[QuestionBankService] Registered question bank '{bankData.bankName}' ({bankData.questions.Count} questions)");
        }

        /// <summary>
        /// 指定された教科・難易度の利用可能な問題数を取得します
        /// </summary>
        public int GetAvailableQuestionCount(Subject subject, DifficultyLevel difficulty)
        {
            string key = GetBankKey(subject, difficulty);
            
            if (_questionBanks.TryGetValue(key, out var bank))
            {
                return bank.questions.Count;
            }

            return 0;
        }

        /// <summary>
        /// すべての利用可能な問題数を取得します
        /// </summary>
        public int GetTotalQuestionCount()
        {
            int total = 0;
            foreach (var bank in _questionBanks.Values)
            {
                total += bank.questions.Count;
            }
            return total;
        }

        /// <summary>
        /// AI生成問題を有効化/無効化します
        /// </summary>
        public void SetAIGenerationEnabled(bool enabled)
        {
            _aiGenerationEnabled = enabled;
            Debug.Log($"[QuestionBankService] AI generation {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// AI生成問題を生成します（現在はプレースホルダー実装）
        /// </summary>
        public List<TestQuestion> GenerateAIQuestions(Subject subject, DifficultyLevel difficulty, int count)
        {
            Debug.LogWarning("[QuestionBankService] AI question generation is not yet implemented. Returning placeholder questions.");

            var questions = new List<TestQuestion>();
            int pointValue = _config.GetPointsForDifficulty(difficulty);

            for (int i = 0; i < count; i++)
            {
                questions.Add(GeneratePlaceholderQuestion(subject, difficulty, pointValue, i));
            }

            QuestionsGenerated?.Invoke(questions.Count);
            return questions;
        }

        /// <summary>
        /// プレースホルダー問題を生成します（開発・テスト用）
        /// </summary>
        private TestQuestion GeneratePlaceholderQuestion(Subject subject, DifficultyLevel difficulty, int pointValue, int index)
        {
            var question = new TestQuestion
            {
                questionId = Guid.NewGuid().ToString(),
                subject = subject,
                difficulty = difficulty,
                pointValue = pointValue,
                isAIGenerated = true,
                isReviewed = false,
                generatedDate = DateTime.Now
            };

            // 教科別のプレースホルダー問題を生成
            switch (subject)
            {
                case Subject.Math:
                    GenerateMathPlaceholder(question, difficulty, index);
                    break;
                case Subject.Science:
                    GenerateSciencePlaceholder(question, difficulty, index);
                    break;
                case Subject.English:
                    GenerateEnglishPlaceholder(question, difficulty, index);
                    break;
                case Subject.History:
                    GenerateHistoryPlaceholder(question, difficulty, index);
                    break;
            }

            return question;
        }

        #region Placeholder Question Generators

        private void GenerateMathPlaceholder(TestQuestion question, DifficultyLevel difficulty, int index)
        {
            int a = _random.Next(1, 10);
            int b = _random.Next(1, 10);
            int answer = a + b;

            question.questionText = $"What is {a} + {b}?";
            question.choices = new string[]
            {
                $"{answer}",
                $"{answer + 1}",
                $"{answer - 1}",
                $"{answer + 2}",
                $"{answer - 2}"
            };
            question.correctAnswerIndex = 0;
        }

        private void GenerateSciencePlaceholder(TestQuestion question, DifficultyLevel difficulty, int index)
        {
            question.questionText = "What is the chemical symbol for water?";
            question.choices = new string[] { "H2O", "CO2", "O2", "H2", "N2" };
            question.correctAnswerIndex = 0;
        }

        private void GenerateEnglishPlaceholder(TestQuestion question, DifficultyLevel difficulty, int index)
        {
            question.questionText = "Which word is a noun?";
            question.choices = new string[] { "Book", "Run", "Quickly", "Happy", "Very" };
            question.correctAnswerIndex = 0;
        }

        private void GenerateHistoryPlaceholder(TestQuestion question, DifficultyLevel difficulty, int index)
        {
            question.questionText = "In what year did World War II end?";
            question.choices = new string[] { "1945", "1944", "1946", "1943", "1947" };
            question.correctAnswerIndex = 0;
        }

        #endregion

        /// <summary>
        /// 問題バンクのキーを生成します
        /// </summary>
        private string GetBankKey(Subject subject, DifficultyLevel difficulty)
        {
            return $"{subject}_{difficulty}";
        }
    }
}
