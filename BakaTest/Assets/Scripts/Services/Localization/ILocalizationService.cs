#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Localization;

namespace BakaTest.Services.Localization
{
    /// <summary>
    /// ローカライゼーションサービスのインターフェース
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>現在の言語</summary>
        Language CurrentLanguage { get; }

        /// <summary>サポートされている言語のリスト</summary>
        List<Language> SupportedLanguages { get; }

        /// <summary>
        /// 言語を変更します
        /// </summary>
        void SetLanguage(Language language);

        /// <summary>
        /// システム言語を検出して設定します
        /// </summary>
        void SetLanguageFromSystem();

        /// <summary>
        /// ローカライズされたテキストを取得します
        /// </summary>
        /// <param name="key">ローカライゼーションキー</param>
        /// <param name="category">カテゴリ（省略時はCommon）</param>
        /// <returns>ローカライズされたテキスト</returns>
        string GetText(string key, LocalizationCategory category = LocalizationCategory.Common);

        /// <summary>
        /// フォーマット付きでローカライズされたテキストを取得します
        /// </summary>
        /// <param name="key">ローカライゼーションキー</param>
        /// <param name="args">フォーマット引数</param>
        /// <returns>ローカライズされたテキスト</returns>
        string GetTextFormatted(string key, params object[] args);

        /// <summary>
        /// 特定のカテゴリのローカライゼーションデータをロードします
        /// </summary>
        void LoadCategory(LocalizationCategory category);

        /// <summary>
        /// すべてのローカライゼーションデータをロードします
        /// </summary>
        void LoadAll();

        /// <summary>
        /// 言語が変更されたときのイベント
        /// </summary>
        event Action<Language>? LanguageChanged;
    }
}
