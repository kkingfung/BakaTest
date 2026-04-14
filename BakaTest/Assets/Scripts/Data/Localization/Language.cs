#nullable enable
using UnityEngine;

namespace BakaTest.Data.Localization
{
    /// <summary>
    /// サポートされている言語
    /// </summary>
    public enum Language
    {
        Japanese,
        English,
        ChineseSimplified,
        ChineseTraditional,
        Korean,
        Spanish,
        French,
        German,
        Portuguese,
        Russian
    }

    /// <summary>
    /// Language列挙型の拡張メソッド
    /// </summary>
    public static class LanguageExtensions
    {
        /// <summary>
        /// 言語の表示名を取得します（ネイティブ言語で）
        /// </summary>
        public static string GetDisplayName(this Language language)
        {
            return language switch
            {
                Language.Japanese => "日本語",
                Language.English => "English",
                Language.ChineseSimplified => "简体中文",
                Language.ChineseTraditional => "繁體中文",
                Language.Korean => "한국어",
                Language.Spanish => "Español",
                Language.French => "Français",
                Language.German => "Deutsch",
                Language.Portuguese => "Português",
                Language.Russian => "Русский",
                _ => language.ToString()
            };
        }

        /// <summary>
        /// ISO 639-1言語コードを取得します
        /// </summary>
        public static string GetLanguageCode(this Language language)
        {
            return language switch
            {
                Language.Japanese => "ja",
                Language.English => "en",
                Language.ChineseSimplified => "zh-CN",
                Language.ChineseTraditional => "zh-TW",
                Language.Korean => "ko",
                Language.Spanish => "es",
                Language.French => "fr",
                Language.German => "de",
                Language.Portuguese => "pt",
                Language.Russian => "ru",
                _ => "en"
            };
        }

        /// <summary>
        /// UnityのSystemLanguageから対応するLanguageに変換します
        /// </summary>
        public static Language FromSystemLanguage(SystemLanguage systemLanguage)
        {
            return systemLanguage switch
            {
                SystemLanguage.Japanese => Language.Japanese,
                SystemLanguage.English => Language.English,
                SystemLanguage.ChineseSimplified => Language.ChineseSimplified,
                SystemLanguage.ChineseTraditional => Language.ChineseTraditional,
                SystemLanguage.Korean => Language.Korean,
                SystemLanguage.Spanish => Language.Spanish,
                SystemLanguage.French => Language.French,
                SystemLanguage.German => Language.German,
                SystemLanguage.Portuguese => Language.Portuguese,
                SystemLanguage.Russian => Language.Russian,
                _ => Language.English  // デフォルトは英語
            };
        }
    }
}
