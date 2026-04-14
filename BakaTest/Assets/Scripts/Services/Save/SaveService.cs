#nullable enable
using System;
using UnityEngine;
using BakaTest.Data.Save;

namespace BakaTest.Services.Save
{
    /// <summary>
    /// セーブ/ロード管理サービスの実装
    /// </summary>
    /// <remarks>
    /// JSONでシリアライズし、PlayerPrefsに保存します。
    /// 自動保存機能も提供します。
    /// </remarks>
    public class SaveService : ISaveService
    {
        private const string SaveKey = "BakaTest_SaveData";
        private const float AutoSaveInterval = 30f; // 30秒ごとに自動保存

        private SaveData _currentSaveData = new SaveData();
        private bool _autoSaveEnabled = false;
        private float _autoSaveTimer = 0f;
        private bool _isDirty = false; // データが変更されたか

        public SaveData CurrentSaveData => _currentSaveData;
        public bool SaveExists => PlayerPrefs.HasKey(SaveKey);

        public event Action? SaveDataChanged;
        public event Action? SaveCompleted;
        public event Action? LoadCompleted;

        public SaveService()
        {
            Debug.Log("[SaveService] Initialized");
        }

        /// <summary>
        /// 毎フレーム呼び出される更新処理（GameBootstrapから呼ぶ）
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!_autoSaveEnabled || !_isDirty) return;

            _autoSaveTimer += deltaTime;
            if (_autoSaveTimer >= AutoSaveInterval)
            {
                _autoSaveTimer = 0f;
                Save();
                Debug.Log("[SaveService] Auto-save triggered");
            }
        }

        public bool Load()
        {
            if (!SaveExists)
            {
                Debug.LogWarning("[SaveService] No save data found!");
                return false;
            }

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                _currentSaveData = JsonUtility.FromJson<SaveData>(json);

                if (_currentSaveData == null)
                {
                    Debug.LogError("[SaveService] Failed to deserialize save data!");
                    return false;
                }

                Debug.Log($"[SaveService] Save data loaded. Version: {_currentSaveData.version}, Last Save: {_currentSaveData.lastSaveTime}");
                LoadCompleted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveService] Load failed: {ex.Message}");
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                _currentSaveData.lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string json = JsonUtility.ToJson(_currentSaveData, true);
                PlayerPrefs.SetString(SaveKey, json);
                PlayerPrefs.Save();

                _isDirty = false;
                Debug.Log($"[SaveService] Save completed at {_currentSaveData.lastSaveTime}");
                SaveCompleted?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveService] Save failed: {ex.Message}");
                return false;
            }
        }

        public void CreateNew()
        {
            _currentSaveData = new SaveData();
            _isDirty = true;
            Debug.Log("[SaveService] New save data created");
            SaveDataChanged?.Invoke();
        }

        public void DeleteSave()
        {
            if (PlayerPrefs.HasKey(SaveKey))
            {
                PlayerPrefs.DeleteKey(SaveKey);
                PlayerPrefs.Save();
                Debug.Log("[SaveService] Save data deleted");
            }

            CreateNew();
        }

        public void EnableAutoSave()
        {
            _autoSaveEnabled = true;
            _autoSaveTimer = 0f;
            Debug.Log("[SaveService] Auto-save enabled");
        }

        public void DisableAutoSave()
        {
            _autoSaveEnabled = false;
            Debug.Log("[SaveService] Auto-save disabled");
        }

        /// <summary>
        /// データが変更されたことを通知します（他のサービスから呼ぶ）
        /// </summary>
        public void MarkDirty()
        {
            _isDirty = true;
            SaveDataChanged?.Invoke();
        }

        /// <summary>
        /// デイリーリセットが必要かチェックします
        /// </summary>
        public bool NeedsDailyReset()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            return _currentSaveData.playerData.currentDate != today;
        }

        /// <summary>
        /// デイリーリセットを実行します
        /// </summary>
        public void PerformDailyReset()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            _currentSaveData.playerData.currentDate = today;

            // 今日獲得したポイントをリセット
            _currentSaveData.playerData.todaysEarnedPoints[Data.Champions.Subject.Math] = 0;
            _currentSaveData.playerData.todaysEarnedPoints[Data.Champions.Subject.Science] = 0;
            _currentSaveData.playerData.todaysEarnedPoints[Data.Champions.Subject.English] = 0;
            _currentSaveData.playerData.todaysEarnedPoints[Data.Champions.Subject.History] = 0;

            MarkDirty();
            Debug.Log($"[SaveService] Daily reset performed for {today}");
        }
    }
}
