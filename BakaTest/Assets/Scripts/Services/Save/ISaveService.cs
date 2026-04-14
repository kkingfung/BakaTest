#nullable enable
using System;
using BakaTest.Data.Save;

namespace BakaTest.Services.Save
{
    /// <summary>
    /// セーブ/ロード管理サービスのインターフェース
    /// </summary>
    public interface ISaveService
    {
        /// <summary>現在のセーブデータ</summary>
        SaveData CurrentSaveData { get; }

        /// <summary>セーブデータが存在するか</summary>
        bool SaveExists { get; }

        /// <summary>
        /// セーブデータをロードします
        /// </summary>
        /// <returns>ロードに成功したらtrue</returns>
        bool Load();

        /// <summary>
        /// セーブデータを保存します
        /// </summary>
        /// <returns>保存に成功したらtrue</returns>
        bool Save();

        /// <summary>
        /// 新しいセーブデータを作成します（初回起動時）
        /// </summary>
        void CreateNew();

        /// <summary>
        /// セーブデータを削除します
        /// </summary>
        void DeleteSave();

        /// <summary>
        /// 自動保存を有効化します
        /// </summary>
        void EnableAutoSave();

        /// <summary>
        /// 自動保存を無効化します
        /// </summary>
        void DisableAutoSave();

        /// <summary>
        /// セーブデータが変更されたことをマークします（次回の自動保存で保存されます）
        /// </summary>
        void MarkDirty();

        /// <summary>セーブデータが変更された時に発火</summary>
        event Action? SaveDataChanged;

        /// <summary>セーブ完了時に発火</summary>
        event Action? SaveCompleted;

        /// <summary>ロード完了時に発火</summary>
        event Action? LoadCompleted;
    }
}
