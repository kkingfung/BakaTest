#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace BakaTest.Data.Localization
{
    /// <summary>
    /// ローカライゼーションデータ（ScriptableObject）
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "BakaTest/Localization Data", order = 100)]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField] private LocalizationCategory _category;
        [SerializeField] private List<LocalizedString> _strings = new();

        /// <summary>カテゴリ</summary>
        public LocalizationCategory Category => _category;

        /// <summary>ローカライズ文字列のリスト</summary>
        public List<LocalizedString> Strings => _strings;

        /// <summary>
        /// キーでテキストを取得します
        /// </summary>
        public string GetText(string key, Language language)
        {
            var localizedString = _strings.Find(s => s.Key == key);
            if (localizedString != null)
            {
                return localizedString.GetText(language);
            }

            Debug.LogWarning($"[LocalizationData] Key '{key}' not found in category '{_category}'");
            return $"[{key}]";
        }

        /// <summary>
        /// すべてのキーを取得します
        /// </summary>
        public List<string> GetAllKeys()
        {
            var keys = new List<string>();
            foreach (var str in _strings)
            {
                keys.Add(str.Key);
            }
            return keys;
        }

        /// <summary>
        /// 新しいローカライズ文字列を追加します
        /// </summary>
        public void AddString(string key)
        {
            if (_strings.Find(s => s.Key == key) != null)
            {
                Debug.LogWarning($"[LocalizationData] Key '{key}' already exists");
                return;
            }

            _strings.Add(new LocalizedString(key));
        }

        /// <summary>
        /// ローカライズ文字列を削除します
        /// </summary>
        public void RemoveString(string key)
        {
            _strings.RemoveAll(s => s.Key == key);
        }

        /// <summary>
        /// 翻訳の完成度を取得します（0-1）
        /// </summary>
        public float GetCompleteness(Language language)
        {
            if (_strings.Count == 0) return 0f;

            int translatedCount = 0;
            foreach (var str in _strings)
            {
                var text = str.GetText(language);
                if (!text.StartsWith("[") && !text.EndsWith("]"))
                {
                    translatedCount++;
                }
            }

            return (float)translatedCount / _strings.Count;
        }
    }
}
