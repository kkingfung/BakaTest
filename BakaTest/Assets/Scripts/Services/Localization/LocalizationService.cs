#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Localization;

namespace BakaTest.Services.Localization
{
    /// <summary>
    /// ローカライゼーションサービスの実装
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private Language _currentLanguage;
        private readonly Dictionary<LocalizationCategory, LocalizationData> _loadedData;
        private readonly List<Language> _supportedLanguages;

        /// <summary>現在の言語</summary>
        public Language CurrentLanguage => _currentLanguage;

        /// <summary>サポートされている言語のリスト</summary>
        public List<Language> SupportedLanguages => new List<Language>(_supportedLanguages);

        /// <summary>言語が変更されたときのイベント</summary>
        public event Action<Language>? LanguageChanged;

        public LocalizationService()
        {
            _loadedData = new Dictionary<LocalizationCategory, LocalizationData>();

            // サポートされている言語（初期は日本語と英語）
            _supportedLanguages = new List<Language>
            {
                Language.Japanese,
                Language.English,
                Language.ChineseSimplified,
                Language.ChineseTraditional,
                Language.Korean
            };

            // デフォルトは英語
            _currentLanguage = Language.English;

            Debug.Log("[LocalizationService] Initialized with English as default language");
        }

        /// <summary>
        /// 言語を変更します
        /// </summary>
        public void SetLanguage(Language language)
        {
            if (_currentLanguage == language)
            {
                Debug.Log($"[LocalizationService] Language is already set to {language}");
                return;
            }

            if (!_supportedLanguages.Contains(language))
            {
                Debug.LogWarning($"[LocalizationService] Language {language} is not supported. Using English instead.");
                language = Language.English;
            }

            var previousLanguage = _currentLanguage;
            _currentLanguage = language;

            Debug.Log($"[LocalizationService] Language changed from {previousLanguage} to {_currentLanguage}");

            // イベントを発火
            LanguageChanged?.Invoke(_currentLanguage);

            // PlayerPrefsに保存
            PlayerPrefs.SetString("Language", _currentLanguage.ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// システム言語を検出して設定します
        /// </summary>
        public void SetLanguageFromSystem()
        {
            // PlayerPrefsに保存された設定を優先
            if (PlayerPrefs.HasKey("Language"))
            {
                var savedLanguage = PlayerPrefs.GetString("Language");
                if (Enum.TryParse<Language>(savedLanguage, out var language))
                {
                    SetLanguage(language);
                    Debug.Log($"[LocalizationService] Loaded saved language: {language}");
                    return;
                }
            }

            // システム言語から自動検出
            var systemLanguage = Application.systemLanguage;
            var detectedLanguage = LanguageExtensions.FromSystemLanguage(systemLanguage);

            // サポートされていない場合は英語
            if (!_supportedLanguages.Contains(detectedLanguage))
            {
                detectedLanguage = Language.English;
            }

            SetLanguage(detectedLanguage);
            Debug.Log($"[LocalizationService] Detected system language: {systemLanguage} -> {detectedLanguage}");
        }

        /// <summary>
        /// ローカライズされたテキストを取得します
        /// </summary>
        public string GetText(string key, LocalizationCategory category = LocalizationCategory.Common)
        {
            // カテゴリがロードされていない場合はロード
            if (!_loadedData.ContainsKey(category))
            {
                LoadCategory(category);
            }

            // データが見つからない場合
            if (!_loadedData.TryGetValue(category, out var data))
            {
                Debug.LogWarning($"[LocalizationService] No localization data found for category '{category}'");
                return $"[{key}]";
            }

            return data.GetText(key, _currentLanguage);
        }

        /// <summary>
        /// フォーマット付きでローカライズされたテキストを取得します
        /// </summary>
        public string GetTextFormatted(string key, params object[] args)
        {
            var text = GetText(key);
            try
            {
                return string.Format(text, args);
            }
            catch (FormatException ex)
            {
                Debug.LogError($"[LocalizationService] Format error for key '{key}': {ex.Message}");
                return text;
            }
        }

        /// <summary>
        /// 特定のカテゴリのローカライゼーションデータをロードします
        /// </summary>
        public void LoadCategory(LocalizationCategory category)
        {
            // すでにロード済みの場合はスキップ
            if (_loadedData.ContainsKey(category))
            {
                return;
            }

            // Resourcesから読み込み
            var fileName = $"LocalizationData_{category}";
            var data = Resources.Load<LocalizationData>($"Data/Localization/{fileName}");

            if (data == null)
            {
                Debug.LogWarning($"[LocalizationService] Localization data for category '{category}' not found at Resources/Data/Localization/{fileName}");
                return;
            }

            _loadedData[category] = data;
            Debug.Log($"[LocalizationService] Loaded localization data for category '{category}'");
        }

        /// <summary>
        /// すべてのローカライゼーションデータをロードします
        /// </summary>
        public void LoadAll()
        {
            foreach (LocalizationCategory category in Enum.GetValues(typeof(LocalizationCategory)))
            {
                LoadCategory(category);
            }

            Debug.Log($"[LocalizationService] Loaded all localization data ({_loadedData.Count} categories)");
        }

        /// <summary>
        /// ロードされたデータをクリアします
        /// </summary>
        public void ClearLoadedData()
        {
            _loadedData.Clear();
            Debug.Log("[LocalizationService] Cleared all loaded localization data");
        }

        /// <summary>
        /// 翻訳の完成度を取得します
        /// </summary>
        public float GetCompleteness(LocalizationCategory category, Language language)
        {
            if (!_loadedData.TryGetValue(category, out var data))
            {
                LoadCategory(category);
                if (!_loadedData.TryGetValue(category, out data))
                {
                    return 0f;
                }
            }

            return data.GetCompleteness(language);
        }

        /// <summary>
        /// すべてのカテゴリの翻訳完成度を取得します
        /// </summary>
        public Dictionary<LocalizationCategory, float> GetAllCompleteness(Language language)
        {
            var result = new Dictionary<LocalizationCategory, float>();

            foreach (LocalizationCategory category in Enum.GetValues(typeof(LocalizationCategory)))
            {
                result[category] = GetCompleteness(category, language);
            }

            return result;
        }
    }
}
