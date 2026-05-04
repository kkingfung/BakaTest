#nullable enable
using System;
using BakaTest.Data.Save;
using BakaTest.Services.Save;

namespace BakaTest.Tests.Mocks
{
    /// <summary>
    /// SaveServiceのモック実装（テスト用）
    /// </summary>
    public class MockSaveService : ISaveService
    {
        public SaveData CurrentSaveData { get; private set; } = new SaveData();

        public bool SaveExists { get; private set; } = true;

        public event Action? SaveDataChanged;
        public event Action? SaveCompleted;
        public event Action? LoadCompleted;

        public bool Save()
        {
            SaveCompleted?.Invoke();
            return true;
        }

        public bool Load()
        {
            LoadCompleted?.Invoke();
            return true;
        }

        public void CreateNew()
        {
            CurrentSaveData = new SaveData();
            SaveExists = true;
        }

        public void DeleteSave()
        {
            CurrentSaveData = new SaveData();
            SaveExists = false;
        }

        public void EnableAutoSave()
        {
            // Mock implementation - no-op
        }

        public void DisableAutoSave()
        {
            // Mock implementation - no-op
        }

        public void MarkDirty()
        {
            SaveDataChanged?.Invoke();
        }
    }
}
