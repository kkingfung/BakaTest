#nullable enable
using System;
using UnityEngine;

namespace BakaTest.Data.Audio
{
    /// <summary>
    /// オーディオクリップのメタデータ
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipData", menuName = "BakaTest/Audio Clip Data", order = 10)]
    public class AudioClipData : ScriptableObject
    {
        [Header("基本情報")]
        /// <summary>オーディオID（ユニークな識別子）</summary>
        [Tooltip("このオーディオクリップの識別子")]
        public string audioId = string.Empty;
        
        /// <summary>表示名</summary>
        [Tooltip("このオーディオクリップの表示名")]
        public string displayName = string.Empty;
        
        /// <summary>オーディオクリップ</summary>
        [Tooltip("実際のオーディオクリップ")]
        public AudioClip? clip;

        [Header("カテゴリ")]
        /// <summary>オーディオタイプ</summary>
        [Tooltip("オーディオの種類（BGM, SFX, Voice, UI）")]
        public AudioType audioType = AudioType.SFX;

        [Header("再生設定")]
        /// <summary>ボリューム（0.0 ~ 1.0）</summary>
        [Tooltip("このクリップのデフォルトボリューム")]
        [Range(0f, 1f)]
        public float volume = 1.0f;
        
        /// <summary>ピッチ（0.5 ~ 2.0）</summary>
        [Tooltip("このクリップのピッチ")]
        [Range(0.5f, 2.0f)]
        public float pitch = 1.0f;
        
        /// <summary>ループするか</summary>
        [Tooltip("このクリップをループ再生するか")]
        public bool loop = false;
        
        /// <summary>3D空間オーディオか</summary>
        [Tooltip("3D空間オーディオとして再生するか（falseで2D）")]
        public bool spatialAudio = false;

        [Header("フェード設定")]
        /// <summary>フェードイン時間（秒）</summary>
        [Tooltip("フェードイン時間（0で即座に再生）")]
        public float fadeInDuration = 0f;
        
        /// <summary>フェードアウト時間（秒）</summary>
        [Tooltip("フェードアウト時間（0で即座に停止）")]
        public float fadeOutDuration = 0f;

        [Header("ランダム化")]
        /// <summary>ボリュームランダム化範囲</summary>
        [Tooltip("ボリュームにランダムな変動を加える範囲（±）")]
        [Range(0f, 0.3f)]
        public float volumeVariation = 0f;
        
        /// <summary>ピッチランダム化範囲</summary>
        [Tooltip("ピッチにランダムな変動を加える範囲（±）")]
        [Range(0f, 0.3f)]
        public float pitchVariation = 0f;

        /// <summary>
        /// AudioSourceに設定を適用します
        /// </summary>
        /// <param name="audioSource">適用先のAudioSource</param>
        /// <param name="applyRandomization">ランダム化を適用するか</param>
        public void ApplyToAudioSource(AudioSource audioSource, bool applyRandomization = true)
        {
            if (audioSource == null || clip == null) return;

            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.spatialBlend = spatialAudio ? 1f : 0f;

            // ボリュームとピッチ（ランダム化考慮）
            float finalVolume = volume;
            float finalPitch = pitch;

            if (applyRandomization)
            {
                finalVolume += UnityEngine.Random.Range(-volumeVariation, volumeVariation);
                finalPitch += UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            }

            audioSource.volume = Mathf.Clamp01(finalVolume);
            audioSource.pitch = Mathf.Clamp(finalPitch, 0.5f, 2.0f);
        }

        #region Editor Validation

        private void OnValidate()
        {
            // IDが空の場合、自動生成
            if (string.IsNullOrEmpty(audioId) && clip != null)
            {
                audioId = clip.name.ToLower().Replace(" ", "_");
            }

            // 表示名が空の場合、クリップ名を使用
            if (string.IsNullOrEmpty(displayName) && clip != null)
            {
                displayName = clip.name;
            }
        }

        #endregion
    }

    /// <summary>
    /// オーディオタイプ
    /// </summary>
    public enum AudioType
    {
        /// <summary>BGM（背景音楽）</summary>
        Music,
        /// <summary>効果音</summary>
        SFX,
        /// <summary>ボイス</summary>
        Voice,
        /// <summary>UIサウンド</summary>
        UI
    }
}
