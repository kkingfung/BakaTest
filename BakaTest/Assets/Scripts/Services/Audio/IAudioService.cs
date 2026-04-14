#nullable enable
using System;
using UnityEngine;
using BakaTest.Data.Audio;

namespace BakaTest.Services.Audio
{
    /// <summary>
    /// オーディオ管理サービスのインターフェース
    /// </summary>
    /// <remarks>
    /// BGM、効果音、ボイス、UIサウンドの再生を管理します。
    /// SettingsServiceと連携してボリューム設定を反映します。
    /// </remarks>
    public interface IAudioService
    {
        // === Music (BGM) ===

        /// <summary>
        /// BGMを再生します
        /// </summary>
        /// <param name="audioId">オーディオID</param>
        /// <param name="fadeIn">フェードイン時間（秒、nullの場合はAudioClipDataの設定を使用）</param>
        void PlayMusic(string audioId, float? fadeIn = null);

        /// <summary>
        /// BGMを停止します
        /// </summary>
        /// <param name="fadeOut">フェードアウト時間（秒、nullの場合は即座に停止）</param>
        void StopMusic(float? fadeOut = null);

        /// <summary>
        /// BGMを一時停止します
        /// </summary>
        void PauseMusic();

        /// <summary>
        /// BGMを再開します
        /// </summary>
        void ResumeMusic();

        /// <summary>
        /// 現在再生中のBGM ID
        /// </summary>
        string? CurrentMusicId { get; }

        /// <summary>
        /// BGMが再生中か
        /// </summary>
        bool IsMusicPlaying { get; }

        // === Sound Effects (SFX) ===

        /// <summary>
        /// 効果音を再生します
        /// </summary>
        /// <param name="audioId">オーディオID</param>
        /// <param name="volumeScale">ボリュームスケール（0.0 ~ 1.0、デフォルトの倍率）</param>
        /// <returns>再生中のAudioSource（停止制御用、nullの場合は再生失敗）</returns>
        AudioSource? PlaySFX(string audioId, float volumeScale = 1.0f);

        /// <summary>
        /// 効果音を指定位置で再生します（3D空間オーディオ）
        /// </summary>
        /// <param name="audioId">オーディオID</param>
        /// <param name="position">再生位置</param>
        /// <param name="volumeScale">ボリュームスケール</param>
        /// <returns>再生中のAudioSource</returns>
        AudioSource? PlaySFXAtPosition(string audioId, Vector3 position, float volumeScale = 1.0f);

        /// <summary>
        /// すべての効果音を停止します
        /// </summary>
        void StopAllSFX();

        // === Voice ===

        /// <summary>
        /// ボイスを再生します
        /// </summary>
        /// <param name="audioId">オーディオID</param>
        /// <param name="interruptCurrent">現在のボイスを中断するか</param>
        /// <returns>再生中のAudioSource</returns>
        AudioSource? PlayVoice(string audioId, bool interruptCurrent = true);

        /// <summary>
        /// ボイスを停止します
        /// </summary>
        void StopVoice();

        /// <summary>
        /// ボイスが再生中か
        /// </summary>
        bool IsVoicePlaying { get; }

        // === UI Sounds ===

        /// <summary>
        /// UIサウンドを再生します
        /// </summary>
        /// <param name="audioId">オーディオID</param>
        void PlayUISound(string audioId);

        /// <summary>
        /// ボタンクリック音を再生します
        /// </summary>
        void PlayButtonClick();

        /// <summary>
        /// ボタンホバー音を再生します
        /// </summary>
        void PlayButtonHover();

        /// <summary>
        /// 成功音を再生します
        /// </summary>
        void PlaySuccessSound();

        /// <summary>
        /// エラー音を再生します
        /// </summary>
        void PlayErrorSound();

        // === Audio Registration ===

        /// <summary>
        /// オーディオクリップを登録します
        /// </summary>
        /// <param name="audioData">オーディオクリップデータ</param>
        void RegisterAudio(AudioClipData audioData);

        /// <summary>
        /// 複数のオーディオクリップを登録します
        /// </summary>
        /// <param name="audioDataArray">オーディオクリップデータの配列</param>
        void RegisterAudios(AudioClipData[] audioDataArray);

        /// <summary>
        /// Resourcesからすべてのオーディオクリップをロードします
        /// </summary>
        void LoadAllAudioFromResources();

        /// <summary>
        /// 登録されているオーディオ数を取得します
        /// </summary>
        int GetRegisteredAudioCount();

        // === Volume Control ===

        /// <summary>
        /// マスターボリュームを設定します（SettingsServiceと連動）
        /// </summary>
        /// <param name="volume">ボリューム（0.0 ~ 1.0）</param>
        void SetMasterVolume(float volume);

        /// <summary>
        /// BGMボリュームを設定します（SettingsServiceと連動）
        /// </summary>
        /// <param name="volume">ボリューム（0.0 ~ 1.0）</param>
        void SetMusicVolume(float volume);

        /// <summary>
        /// 効果音ボリュームを設定します（SettingsServiceと連動）
        /// </summary>
        /// <param name="volume">ボリューム（0.0 ~ 1.0）</param>
        void SetSFXVolume(float volume);

        /// <summary>
        /// ボイスボリュームを設定します（SettingsServiceと連動）
        /// </summary>
        /// <param name="volume">ボリューム（0.0 ~ 1.0）</param>
        void SetVoiceVolume(float volume);

        /// <summary>
        /// UIサウンドボリュームを設定します（SettingsServiceと連動）
        /// </summary>
        /// <param name="volume">ボリューム（0.0 ~ 1.0）</param>
        void SetUIVolume(float volume);

        // === Events ===

        /// <summary>BGMが変更された時に発火</summary>
        event Action<string>? MusicChanged;

        /// <summary>効果音が再生された時に発火</summary>
        event Action<string>? SFXPlayed;
    }
}
