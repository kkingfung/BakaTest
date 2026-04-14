#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BakaTest.Data.Localization
{
    /// <summary>
    /// 多言語対応文字列のコンテナ
    /// </summary>
    [Serializable]
    public class LocalizedString
    {
        /// <summary>ローカライゼーションキー</summary>
        [SerializeField] private string _key = string.Empty;

        /// <summary>言語ごとの翻訳リスト</summary>
        [SerializeField] private List<LanguageEntry> _translations = new();

        /// <summary>ローカライゼーションキー</summary>
        public string Key => _key;

        /// <summary>翻訳リスト</summary>
        public List<LanguageEntry> Translations => _translations;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public LocalizedString()
        {
            _key = string.Empty;
            _translations = new List<LanguageEntry>();
        }

        /// <summary>
        /// キーを指定してLocalizedStringを作成します
        /// </summary>
        public LocalizedString(string key)
        {
            _key = key;
            _translations = new List<LanguageEntry>();
        }

        /// <summary>
        /// 指定された言語のテキストを取得します
        /// </summary>
        /// <param name="language">取得する言語</param>
        /// <returns>ローカライズされたテキスト</returns>
        public string GetText(Language language)
        {
            // 指定された言語の翻訳を検索
            var entry = _translations.Find(t => t.Language == language);
            if (entry != null && !string.IsNullOrEmpty(entry.Text))
            {
                return entry.Text;
            }

            // フォールバック1: 英語を試す
            var fallback = _translations.Find(t => t.Language == Language.English);
            if (fallback != null && !string.IsNullOrEmpty(fallback.Text))
            {
                return fallback.Text;
            }

            // フォールバック2: 最初の利用可能な翻訳
            if (_translations.Count > 0 && !string.IsNullOrEmpty(_translations[0].Text))
            {
                return _translations[0].Text;
            }

            // フォールバック3: キーをそのまま返す
            return $"[{_key}]";
        }

        /// <summary>
        /// 指定された言語のテキストを設定します
        /// </summary>
        /// <param name="language">設定する言語</param>
        /// <param name="text">テキスト</param>
        public void SetText(Language language, string text)
        {
            var entry = _translations.Find(t => t.Language == language);
            if (entry != null)
            {
                entry.Text = text;
            }
            else
            {
                _translations.Add(new LanguageEntry
                {
                    Language = language,
                    Text = text
                });
            }
        }

        /// <summary>
        /// すべての言語で翻訳が完了しているかチェックします
        /// </summary>
        public bool IsComplete()
        {
            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                var entry = _translations.Find(t => t.Language == language);
                if (entry == null || string.IsNullOrEmpty(entry.Text))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 翻訳が欠けている言語のリストを取得します
        /// </summary>
        public List<Language> GetMissingLanguages()
        {
            var missing = new List<Language>();
            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                var entry = _translations.Find(t => t.Language == language);
                if (entry == null || string.IsNullOrEmpty(entry.Text))
                {
                    missing.Add(language);
                }
            }
            return missing;
        }

        public override string ToString()
        {
            return $"[{_key}] ({_translations.Count} translations)";
        }
    }

    /// <summary>
    /// 言語と翻訳テキストのペア
    /// </summary>
    [Serializable]
    public class LanguageEntry
    {
        /// <summary>言語</summary>
        public Language Language;

        /// <summary>翻訳テキスト</summary>
        public string Text = string.Empty;
    }
}
