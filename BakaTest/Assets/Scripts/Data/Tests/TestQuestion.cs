#nullable enable
using System;
using BakaTest.Data.Champions;
using BakaTest.Data.Localization;
using UnityEngine;

namespace BakaTest.Data.Tests
{
    /// <summary>
    /// テスト問題データ
    /// </summary>
    [Serializable]
    public class TestQuestion
    {
        /// <summary>問題ID（ユニークな識別子）</summary>
        public string questionId = string.Empty;

        /// <summary>教科</summary>
        public Subject subject;

        /// <summary>難易度レベル</summary>
        public DifficultyLevel difficulty;

        /// <summary>問題文（レガシー - 後方互換性のため残す）</summary>
        [Obsolete("Use localizedQuestionText instead")]
        public string questionText = string.Empty;

        /// <summary>多言語対応問題文</summary>
        [SerializeField] private LocalizedString? _localizedQuestionText;

        /// <summary>選択肢（5つ）（レガシー - 後方互換性のため残す）</summary>
        [Obsolete("Use localizedChoices instead")]
        public string[] choices = new string[5];

        /// <summary>多言語対応選択肢</summary>
        [SerializeField] private LocalizedString[]? _localizedChoices;

        /// <summary>解説（レガシー）</summary>
        public string explanation = string.Empty;

        /// <summary>多言語対応解説</summary>
        [SerializeField] private LocalizedString? _localizedExplanation;

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

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public TestQuestion()
        {
            generatedDate = DateTime.Now;
            _localizedChoices = new LocalizedString[5];
            for (int i = 0; i < 5; i++)
            {
                _localizedChoices[i] = new LocalizedString($"{questionId}_choice_{i}");
            }
        }

        /// <summary>
        /// 問題文を取得します（多言語対応）
        /// </summary>
        public string GetQuestionText(Language language)
        {
            if (_localizedQuestionText != null)
            {
                return _localizedQuestionText.GetText(language);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return questionText; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// 選択肢を取得します（多言語対応）
        /// </summary>
        public string[] GetChoices(Language language)
        {
            if (_localizedChoices != null && _localizedChoices.Length == 5)
            {
                var result = new string[5];
                for (int i = 0; i < 5; i++)
                {
                    result[i] = _localizedChoices[i].GetText(language);
                }
                return result;
            }
#pragma warning disable CS0618 // Type or member is obsolete
            return choices; // フォールバック
#pragma warning restore CS0618
        }

        /// <summary>
        /// 解説を取得します（多言語対応）
        /// </summary>
        public string GetExplanation(Language language)
        {
            if (_localizedExplanation != null)
            {
                return _localizedExplanation.GetText(language);
            }
            return explanation; // フォールバック
        }

        /// <summary>
        /// 問題文を設定します（多言語対応）
        /// </summary>
        public void SetQuestionText(Language language, string text)
        {
            if (_localizedQuestionText == null)
            {
                _localizedQuestionText = new LocalizedString($"{questionId}_question");
            }
            _localizedQuestionText.SetText(language, text);
        }

        /// <summary>
        /// 選択肢を設定します（多言語対応）
        /// </summary>
        public void SetChoice(Language language, int index, string text)
        {
            if (_localizedChoices == null || _localizedChoices.Length != 5)
            {
                _localizedChoices = new LocalizedString[5];
                for (int i = 0; i < 5; i++)
                {
                    _localizedChoices[i] = new LocalizedString($"{questionId}_choice_{i}");
                }
            }

            if (index >= 0 && index < 5)
            {
                _localizedChoices[index].SetText(language, text);
            }
        }

        /// <summary>
        /// 解説を設定します（多言語対応）
        /// </summary>
        public void SetExplanation(Language language, string text)
        {
            if (_localizedExplanation == null)
            {
                _localizedExplanation = new LocalizedString($"{questionId}_explanation");
            }
            _localizedExplanation.SetText(language, text);
        }

        /// <summary>
        /// 指定された回答が正解かどうかを判定します
        /// </summary>
        /// <param name="answerIndex">回答のインデックス</param>
        /// <returns>正解の場合true</returns>
        public bool IsCorrect(int answerIndex)
        {
            return answerIndex == correctAnswerIndex;
        }

        public override string ToString()
        {
            var questionPreview = GetQuestionText(Language.English);
            return $"[{subject}] {difficulty} - {questionPreview.Substring(0, Math.Min(50, questionPreview.Length))}... (Worth {pointValue} points)";
        }
    }
}
