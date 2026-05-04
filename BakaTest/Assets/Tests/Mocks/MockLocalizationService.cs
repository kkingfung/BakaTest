#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Localization;
using BakaTest.Services.Localization;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// LocalizationServiceのモック実装（テスト用）
    /// </summary>
    public class MockLocalizationService : ILocalizationService
    {
        public Language CurrentLanguage { get; private set; } = Language.English;

        public List<Language> SupportedLanguages => new List<Language> { Language.Japanese, Language.English };

        public event Action<Language>? LanguageChanged;

        public void SetLanguage(Language language)
        {
            CurrentLanguage = language;
            LanguageChanged?.Invoke(language);
        }

        public void SetLanguageFromSystem()
        {
            CurrentLanguage = Language.English;
        }

        public string GetText(string key, LocalizationCategory category = LocalizationCategory.Common)
        {
            // テスト用に単純にキーを返す
            return key;
        }

        public string GetTextFormatted(string key, params object[] args)
        {
            return string.Format(GetText(key), args);
        }

        public void LoadCategory(LocalizationCategory category)
        {
            // テスト用には何もしない
        }

        public void LoadAll()
        {
            // テスト用には何もしない
        }
    }
}
