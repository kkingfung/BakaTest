#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BakaTest.Data.Audio;
using BakaTest.Services.Settings;

namespace BakaTest.Services.Audio
{
    /// <summary>
    /// オーディオ管理サービスの実装
    /// </summary>
    public class AudioService : IAudioService
    {
        private readonly Dictionary<string, AudioClipData> _audioDatabase;
        private readonly ISettingsService? _settingsService;
        
        // AudioSource管理
        private GameObject? _audioRoot;
        private AudioSource? _musicSource;
        private AudioSource? _voiceSource;
        private readonly List<AudioSource> _sfxPool;
        private readonly int _sfxPoolSize = 10;

        // ボリューム設定（キャッシュ）
        private float _masterVolume = 1.0f;
        private float _musicVolume = 1.0f;
        private float _sfxVolume = 1.0f;
        private float _voiceVolume = 1.0f;
        private float _uiVolume = 1.0f;

        // BGM管理
        private string? _currentMusicId;
        private Coroutine? _musicFadeCoroutine;

        // イベント
        public event Action<string>? MusicChanged;
        public event Action<string>? SFXPlayed;

        // プロパティ
        public string? CurrentMusicId => _currentMusicId;
        public bool IsMusicPlaying => _musicSource != null && _musicSource.isPlaying;
        public bool IsVoicePlaying => _voiceSource != null && _voiceSource.isPlaying;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settingsService">設定サービス（オプション）</param>
        public AudioService(ISettingsService? settingsService = null)
        {
            _audioDatabase = new Dictionary<string, AudioClipData>();
            _settingsService = settingsService;
            _sfxPool = new List<AudioSource>();

            Initialize();
            Debug.Log("[AudioService] Initialized.");
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            // AudioRootオブジェクトを作成
            _audioRoot = new GameObject("AudioService");
            UnityEngine.Object.DontDestroyOnLoad(_audioRoot);

            // Music AudioSourceを作成
            _musicSource = _audioRoot.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;

            // Voice AudioSourceを作成
            _voiceSource = _audioRoot.AddComponent<AudioSource>();
            _voiceSource.playOnAwake = false;
            _voiceSource.loop = false;

            // SFX poolを作成
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                var sfxSource = _audioRoot.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
                _sfxPool.Add(sfxSource);
            }

            // SettingsServiceからボリューム設定をロード
            LoadVolumeSettings();

            // SettingsServiceのイベントを購読
            if (_settingsService != null)
            {
                _settingsService.AudioSettingsChanged += OnAudioSettingsChanged;
            }
        }

        /// <summary>
        /// ボリューム設定をロード
        /// </summary>
        private void LoadVolumeSettings()
        {
            if (_settingsService == null) return;

            var audio = _settingsService.Audio;
            _masterVolume = audio.masterVolume;
            _musicVolume = audio.musicVolume;
            _sfxVolume = audio.sfxVolume;
            _voiceVolume = audio.voiceVolume;
            _uiVolume = audio.uiVolume;

            ApplyVolumeToAllSources();
        }

        /// <summary>
        /// オーディオ設定変更イベントハンドラ
        /// </summary>
        private void OnAudioSettingsChanged(BakaTest.Data.Settings.AudioSettings audioSettings)
        {
            _masterVolume = audioSettings.masterVolume;
            _musicVolume = audioSettings.musicVolume;
            _sfxVolume = audioSettings.sfxVolume;
            _voiceVolume = audioSettings.voiceVolume;
            _uiVolume = audioSettings.uiVolume;

            ApplyVolumeToAllSources();
        }

        /// <summary>
        /// すべてのAudioSourceにボリュームを適用
        /// </summary>
        private void ApplyVolumeToAllSources()
        {
            if (_musicSource != null && _currentMusicId != null && _audioDatabase.TryGetValue(_currentMusicId, out var musicData))
            {
                _musicSource.volume = musicData.volume * _musicVolume * _masterVolume;
            }

            if (_voiceSource != null && _voiceSource.isPlaying)
            {
                _voiceSource.volume = _voiceVolume * _masterVolume;
            }
        }

        #region Music (BGM)

        public void PlayMusic(string audioId, float? fadeIn = null)
        {
            if (!_audioDatabase.TryGetValue(audioId, out var audioData))
            {
                Debug.LogWarning($"[AudioService] Music '{audioId}' not found in database.");
                return;
            }

            if (audioData.clip == null)
            {
                Debug.LogWarning($"[AudioService] Music '{audioId}' has no AudioClip assigned.");
                return;
            }

            // 同じ曲が再生中の場合はスキップ
            if (_currentMusicId == audioId && IsMusicPlaying)
            {
                Debug.Log($"[AudioService] Music '{audioId}' is already playing.");
                return;
            }

            // フェード処理
            float fadeDuration = fadeIn ?? audioData.fadeInDuration;

            if (_musicFadeCoroutine != null)
            {
                CoroutineRunner.StopCoroutine(_musicFadeCoroutine);
            }

            _currentMusicId = audioId;
            _musicFadeCoroutine = CoroutineRunner.StartCoroutine(FadeMusic(audioData, fadeDuration, true));
            MusicChanged?.Invoke(audioId);

            Debug.Log($"[AudioService] Playing music: {audioId} (Fade: {fadeDuration}s)");
        }

        public void StopMusic(float? fadeOut = null)
        {
            if (_musicSource == null || !_musicSource.isPlaying)
            {
                return;
            }

            float fadeDuration = fadeOut ?? 0f;

            if (_currentMusicId != null && _audioDatabase.TryGetValue(_currentMusicId, out var audioData))
            {
                fadeDuration = fadeOut ?? audioData.fadeOutDuration;
            }

            if (_musicFadeCoroutine != null)
            {
                CoroutineRunner.StopCoroutine(_musicFadeCoroutine);
            }

            if (fadeDuration > 0f)
            {
                _musicFadeCoroutine = CoroutineRunner.StartCoroutine(FadeOutMusic(fadeDuration));
            }
            else
            {
                _musicSource.Stop();
                _currentMusicId = null;
            }

            Debug.Log($"[AudioService] Stopping music (Fade: {fadeDuration}s)");
        }

        public void PauseMusic()
        {
            if (_musicSource != null && _musicSource.isPlaying)
            {
                _musicSource.Pause();
                Debug.Log("[AudioService] Music paused.");
            }
        }

        public void ResumeMusic()
        {
            if (_musicSource != null && !_musicSource.isPlaying && _musicSource.clip != null)
            {
                _musicSource.UnPause();
                Debug.Log("[AudioService] Music resumed.");
            }
        }

        private IEnumerator FadeMusic(AudioClipData audioData, float fadeDuration, bool fadeIn)
        {
            if (_musicSource == null) yield break;

            audioData.ApplyToAudioSource(_musicSource, false);
            float targetVolume = audioData.volume * _musicVolume * _masterVolume;

            if (fadeDuration <= 0f)
            {
                _musicSource.volume = targetVolume;
                _musicSource.Play();
                yield break;
            }

            _musicSource.volume = fadeIn ? 0f : targetVolume;
            _musicSource.Play();

            float elapsed = 0f;
            float startVolume = _musicSource.volume;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                _musicSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            _musicSource.volume = targetVolume;
        }

        private IEnumerator FadeOutMusic(float fadeDuration)
        {
            if (_musicSource == null) yield break;

            float startVolume = _musicSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                _musicSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            _musicSource.Stop();
            _musicSource.volume = 0f;
            _currentMusicId = null;
        }

        #endregion

        #region Sound Effects (SFX)

        public AudioSource? PlaySFX(string audioId, float volumeScale = 1.0f)
        {
            if (!_audioDatabase.TryGetValue(audioId, out var audioData))
            {
                Debug.LogWarning($"[AudioService] SFX '{audioId}' not found in database.");
                return null;
            }

            if (audioData.clip == null)
            {
                Debug.LogWarning($"[AudioService] SFX '{audioId}' has no AudioClip assigned.");
                return null;
            }

            var source = GetAvailableSFXSource();
            if (source == null)
            {
                Debug.LogWarning("[AudioService] No available SFX source in pool.");
                return null;
            }

            audioData.ApplyToAudioSource(source, true);
            source.volume = audioData.volume * volumeScale * _sfxVolume * _masterVolume;
            source.Play();

            SFXPlayed?.Invoke(audioId);
            return source;
        }

        public AudioSource? PlaySFXAtPosition(string audioId, Vector3 position, float volumeScale = 1.0f)
        {
            var source = PlaySFX(audioId, volumeScale);
            if (source != null)
            {
                source.transform.position = position;
                source.spatialBlend = 1f; // 3D sound
            }
            return source;
        }

        public void StopAllSFX()
        {
            foreach (var source in _sfxPool)
            {
                if (source.isPlaying)
                {
                    source.Stop();
                }
            }

            Debug.Log("[AudioService] All SFX stopped.");
        }

        private AudioSource? GetAvailableSFXSource()
        {
            // 再生中でないソースを検索
            foreach (var source in _sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // すべて使用中の場合、最も古い（再生時間が長い）ソースを使用
            return _sfxPool.OrderBy(s => s.time).FirstOrDefault();
        }

        #endregion

        #region Voice

        public AudioSource? PlayVoice(string audioId, bool interruptCurrent = true)
        {
            if (!_audioDatabase.TryGetValue(audioId, out var audioData))
            {
                Debug.LogWarning($"[AudioService] Voice '{audioId}' not found in database.");
                return null;
            }

            if (audioData.clip == null)
            {
                Debug.LogWarning($"[AudioService] Voice '{audioId}' has no AudioClip assigned.");
                return null;
            }

            if (_voiceSource == null) return null;

            // 現在のボイスを中断するか確認
            if (!interruptCurrent && _voiceSource.isPlaying)
            {
                Debug.Log($"[AudioService] Voice is already playing. Skipping '{audioId}'.");
                return null;
            }

            audioData.ApplyToAudioSource(_voiceSource, true);
            _voiceSource.volume = audioData.volume * _voiceVolume * _masterVolume;
            _voiceSource.Play();

            Debug.Log($"[AudioService] Playing voice: {audioId}");
            return _voiceSource;
        }

        public void StopVoice()
        {
            if (_voiceSource != null && _voiceSource.isPlaying)
            {
                _voiceSource.Stop();
                Debug.Log("[AudioService] Voice stopped.");
            }
        }

        #endregion

        #region UI Sounds

        public void PlayUISound(string audioId)
        {
            if (!_audioDatabase.TryGetValue(audioId, out var audioData))
            {
                Debug.LogWarning($"[AudioService] UI Sound '{audioId}' not found in database.");
                return;
            }

            if (audioData.clip == null)
            {
                Debug.LogWarning($"[AudioService] UI Sound '{audioId}' has no AudioClip assigned.");
                return;
            }

            var source = GetAvailableSFXSource();
            if (source == null) return;

            audioData.ApplyToAudioSource(source, false); // UI sounds don't need randomization
            source.volume = audioData.volume * _uiVolume * _masterVolume;
            source.Play();
        }

        public void PlayButtonClick() => PlayUISound("ui_button_click");
        public void PlayButtonHover() => PlayUISound("ui_button_hover");
        public void PlaySuccessSound() => PlayUISound("ui_success");
        public void PlayErrorSound() => PlayUISound("ui_error");

        #endregion

        #region Audio Registration

        public void RegisterAudio(AudioClipData audioData)
        {
            if (audioData == null || string.IsNullOrEmpty(audioData.audioId))
            {
                Debug.LogWarning("[AudioService] Cannot register null or invalid audio data.");
                return;
            }

            if (_audioDatabase.ContainsKey(audioData.audioId))
            {
                Debug.LogWarning($"[AudioService] Audio '{audioData.audioId}' already registered. Overwriting.");
            }

            _audioDatabase[audioData.audioId] = audioData;
        }

        public void RegisterAudios(AudioClipData[] audioDataArray)
        {
            foreach (var audioData in audioDataArray)
            {
                RegisterAudio(audioData);
            }

            Debug.Log($"[AudioService] Registered {audioDataArray.Length} audio clips.");
        }

        public void LoadAllAudioFromResources()
        {
            var audioClips = Resources.LoadAll<AudioClipData>("Data/Audio");
            RegisterAudios(audioClips);

            Debug.Log($"[AudioService] Loaded {audioClips.Length} audio clips from Resources/Data/Audio.");
        }

        public int GetRegisteredAudioCount()
        {
            return _audioDatabase.Count;
        }

        #endregion

        #region Volume Control

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeToAllSources();
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeToAllSources();
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
        }

        public void SetVoiceVolume(float volume)
        {
            _voiceVolume = Mathf.Clamp01(volume);
        }

        public void SetUIVolume(float volume)
        {
            _uiVolume = Mathf.Clamp01(volume);
        }

        #endregion

        /// <summary>
        /// コルーチン実行用のヘルパークラス
        /// </summary>
        private static class CoroutineRunner
        {
            private static GameObject? _runner;
            private static MonoBehaviour? _behaviour;

            static CoroutineRunner()
            {
                _runner = new GameObject("AudioService_CoroutineRunner");
                UnityEngine.Object.DontDestroyOnLoad(_runner);
                _behaviour = _runner.AddComponent<CoroutineRunnerBehaviour>();
            }

            public static Coroutine StartCoroutine(IEnumerator routine)
            {
                if (_behaviour == null)
                {
                    throw new InvalidOperationException("CoroutineRunner behaviour is null!");
                }
                return _behaviour.StartCoroutine(routine);
            }

            public static void StopCoroutine(Coroutine coroutine)
            {
                if (_behaviour != null && coroutine != null)
                {
                    _behaviour.StopCoroutine(coroutine);
                }
            }

            private class CoroutineRunnerBehaviour : MonoBehaviour { }
        }
    }
}
