#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using BakaTest.Data.Settings;
using BakaTest.Data.Localization;
using BakaTest.Services.Save;
using BakaTest.Services.Localization;

// Type aliases to avoid conflicts with UnityEngine types
using AudioSettingsData = BakaTest.Data.Settings.AudioSettings;
using GraphicsSettingsData = BakaTest.Data.Settings.GraphicsSettings;
using GameplaySettingsData = BakaTest.Data.Settings.GameplaySettings;
using AccessibilitySettingsData = BakaTest.Data.Settings.AccessibilitySettings;

namespace BakaTest.Services.Settings
{
    /// <summary>
    /// 設定管理サービスの実装
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly ISaveService _saveService;
        private readonly ILocalizationService _localizationService;
        private SettingsData _currentSettings;

        /// <summary>現在の設定データ</summary>
        public SettingsData CurrentSettings => _currentSettings;

        /// <summary>現在の言語</summary>
        public Language CurrentLanguage => _currentSettings.language;

        /// <summary>サポートされている言語のリスト</summary>
        public List<Language> SupportedLanguages => _localizationService.SupportedLanguages;

        /// <summary>オーディオ設定</summary>
        public AudioSettingsData Audio => _currentSettings.audio;

        /// <summary>グラフィックス設定</summary>
        public GraphicsSettingsData Graphics => _currentSettings.graphics;

        /// <summary>ゲームプレイ設定</summary>
        public GameplaySettingsData Gameplay => _currentSettings.gameplay;

        /// <summary>アクセシビリティ設定</summary>
        public AccessibilitySettingsData Accessibility => _currentSettings.accessibility;

        /// <summary>オーディオ設定が変更されたときに発火するイベント</summary>
        public event Action<AudioSettingsData>? AudioSettingsChanged;

        /// <summary>グラフィックス設定が変更されたときに発火するイベント</summary>
        public event Action<GraphicsSettingsData>? GraphicsSettingsChanged;

        /// <summary>ゲームプレイ設定が変更されたときに発火するイベント</summary>
        public event Action<GameplaySettingsData>? GameplaySettingsChanged;

        /// <summary>アクセシビリティ設定が変更されたときに発火するイベント</summary>
        public event Action<AccessibilitySettingsData>? AccessibilitySettingsChanged;

        /// <summary>言語が変更されたときに発火するイベント</summary>
        public event Action<Language>? LanguageChanged;

        /// <summary>設定が完全にリセットされたときに発火するイベント</summary>
        public event Action? SettingsReset;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsService(ISaveService saveService, ILocalizationService localizationService)
        {
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _currentSettings = new SettingsData();

            Debug.Log("[SettingsService] Initialized.");
            LoadData();

            // LocalizationServiceと言語を同期
            _localizationService.SetLanguage(_currentSettings.language);

            ApplyAllSettings();
        }

        /// <summary>
        /// セーブデータからロードします
        /// </summary>
        private void LoadData()
        {
            var saveData = _saveService.CurrentSaveData.settingsData;
            
            // セーブデータから設定をコピー
            _currentSettings.audio = saveData.audio;
            _currentSettings.graphics = saveData.graphics;
            _currentSettings.gameplay = saveData.gameplay;
            _currentSettings.accessibility = saveData.accessibility;

            Debug.Log("[SettingsService] Settings loaded from save data.");
        }

        /// <summary>
        /// データをセーブデータに書き込みます
        /// </summary>
        private void SaveData()
        {
            var saveData = _saveService.CurrentSaveData.settingsData;

            // 設定をセーブデータにコピー
            saveData.audio = _currentSettings.audio;
            saveData.graphics = _currentSettings.graphics;
            saveData.gameplay = _currentSettings.gameplay;
            saveData.accessibility = _currentSettings.accessibility;

            // SaveServiceに変更を通知
            _saveService.MarkDirty();

            Debug.Log("[SettingsService] Settings saved.");
        }

        // === Language Settings ===

        public void SetLanguage(Language language)
        {
            if (_currentSettings.language == language)
            {
                Debug.Log($"[SettingsService] Language is already set to {language.GetDisplayName()}");
                return;
            }

            var previousLanguage = _currentSettings.language;
            _currentSettings.language = language;

            // LocalizationServiceにも適用
            _localizationService.SetLanguage(language);

            Debug.Log($"[SettingsService] Language changed from {previousLanguage.GetDisplayName()} to {language.GetDisplayName()}");

            LanguageChanged?.Invoke(language);
            SaveData();
        }

        // === Audio Settings ===

        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            Audio.masterVolume = volume;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            Audio.musicVolume = volume;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetSFXVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            Audio.sfxVolume = volume;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetVoiceVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            Audio.voiceVolume = volume;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetUIVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            Audio.uiVolume = volume;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetMasterMute(bool muted)
        {
            Audio.masterMuted = muted;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetMusicMute(bool muted)
        {
            Audio.musicMuted = muted;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetSFXMute(bool muted)
        {
            Audio.sfxMuted = muted;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetVoiceMute(bool muted)
        {
            Audio.voiceMuted = muted;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        public void SetUIMute(bool muted)
        {
            Audio.uiMuted = muted;
            ApplyAudioSettings();
            AudioSettingsChanged?.Invoke(Audio);
            SaveData();
        }

        /// <summary>
        /// オーディオ設定をUnityに適用します
        /// </summary>
        private void ApplyAudioSettings()
        {
            // AudioListenerの音量を設定
            float masterVolume = Audio.masterMuted ? 0f : Audio.masterVolume;
            AudioListener.volume = masterVolume;

            Debug.Log($"[SettingsService] Applied audio settings - Master: {masterVolume:F2}");
            
            // TODO: AudioMixerを使用する場合はここで設定
            // 現在はAudioListenerのマスターボリュームのみ制御
        }

        // === Graphics Settings ===

        public void SetQualityLevel(int level)
        {
            level = Mathf.Clamp(level, 0, 3);
            Graphics.qualityLevel = level;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetResolutionScale(float scale)
        {
            scale = Mathf.Clamp(scale, 0.5f, 1.0f);
            Graphics.resolutionScale = scale;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetFullscreen(bool fullscreen)
        {
            Graphics.fullscreen = fullscreen;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetVSync(bool enabled)
        {
            Graphics.vsync = enabled;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetTargetFrameRate(int frameRate)
        {
            Graphics.targetFrameRate = frameRate;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetAntiAliasing(bool enabled)
        {
            Graphics.antiAliasing = enabled;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetShadowQuality(int quality)
        {
            quality = Mathf.Clamp(quality, 0, 3);
            Graphics.shadowQuality = quality;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetTextureQuality(int quality)
        {
            quality = Mathf.Clamp(quality, 0, 2);
            Graphics.textureQuality = quality;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetParticleEffects(bool enabled)
        {
            Graphics.particleEffects = enabled;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        public void SetPostProcessing(bool enabled)
        {
            Graphics.postProcessing = enabled;
            ApplyGraphicsSettings();
            GraphicsSettingsChanged?.Invoke(Graphics);
            SaveData();
        }

        /// <summary>
        /// グラフィックス設定をUnityに適用します
        /// </summary>
        private void ApplyGraphicsSettings()
        {
            // 画質レベル
            QualitySettings.SetQualityLevel(Graphics.qualityLevel, applyExpensiveChanges: true);

            // 解像度スケール
            // TODO: Universal Render Pipeline使用時のみ有効
            // UniversalRenderPipeline.renderScale = Graphics.resolutionScale;

            // フルスクリーン
            Screen.fullScreen = Graphics.fullscreen;

            // VSync
            QualitySettings.vSyncCount = Graphics.vsync ? 1 : 0;

            // ターゲットフレームレート
            Application.targetFrameRate = Graphics.targetFrameRate;

            // アンチエイリアシング
            QualitySettings.antiAliasing = Graphics.antiAliasing ? 4 : 0;

            // 影の品質
            switch (Graphics.shadowQuality)
            {
                case 0:
                    QualitySettings.shadows = ShadowQuality.Disable;
                    break;
                case 1:
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    QualitySettings.shadowResolution = ShadowResolution.Low;
                    break;
                case 2:
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                    break;
                case 3:
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.shadowResolution = ShadowResolution.High;
                    break;
            }

            // テクスチャ品質
            QualitySettings.globalTextureMipmapLimit = 2 - Graphics.textureQuality;

            Debug.Log($"[SettingsService] Applied graphics settings - Quality: {Graphics.qualityLevel}, FPS: {Graphics.targetFrameRate}");
        }

        // === Gameplay Settings ===

        public void SetBattleSpeed(float speed)
        {
            speed = Mathf.Clamp(speed, 0.5f, 2.0f);
            Gameplay.battleSpeed = speed;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetAutoBattle(bool enabled)
        {
            Gameplay.autoBattle = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetSkipCutscenes(bool enabled)
        {
            Gameplay.skipCutscenes = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetShowTutorials(bool enabled)
        {
            Gameplay.showTutorials = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetAutoAdvanceBattleResult(bool enabled)
        {
            Gameplay.autoAdvanceBattleResult = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetAutoAdvanceDelay(float delay)
        {
            delay = Mathf.Clamp(delay, 1f, 10f);
            Gameplay.autoAdvanceDelay = delay;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetShowDamageNumbers(bool enabled)
        {
            Gameplay.showDamageNumbers = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetShowBattleLog(bool enabled)
        {
            Gameplay.showBattleLog = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        public void SetVibrationEnabled(bool enabled)
        {
            Gameplay.vibrationEnabled = enabled;
            GameplaySettingsChanged?.Invoke(Gameplay);
            SaveData();
        }

        // === Accessibility Settings ===

        public void SetTextSize(float size)
        {
            size = Mathf.Clamp(size, 0.8f, 1.5f);
            Accessibility.textSize = size;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetColorBlindMode(ColorBlindMode mode)
        {
            Accessibility.colorBlindMode = mode;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetHighContrastUI(bool enabled)
        {
            Accessibility.highContrastUI = enabled;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetReduceMotion(bool enabled)
        {
            Accessibility.reduceMotion = enabled;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetReduceFlashing(bool enabled)
        {
            Accessibility.reduceFlashing = enabled;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetButtonHoldDuration(float duration)
        {
            duration = Mathf.Clamp(duration, 0.3f, 2.0f);
            Accessibility.buttonHoldDuration = duration;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        public void SetShowAutoSaveNotification(bool enabled)
        {
            Accessibility.showAutoSaveNotification = enabled;
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SaveData();
        }

        // === Utility Methods ===

        public void ResetToDefaults()
        {
            Debug.Log("[SettingsService] Resetting all settings to defaults...");

            _currentSettings = SettingsData.GetDefault();

            // LocalizationServiceにデフォルト言語を適用
            _localizationService.SetLanguage(_currentSettings.language);

            ApplyAllSettings();

            // すべてのイベントを発火
            LanguageChanged?.Invoke(_currentSettings.language);
            AudioSettingsChanged?.Invoke(Audio);
            GraphicsSettingsChanged?.Invoke(Graphics);
            GameplaySettingsChanged?.Invoke(Gameplay);
            AccessibilitySettingsChanged?.Invoke(Accessibility);
            SettingsReset?.Invoke();

            SaveData();

            Debug.Log("[SettingsService] All settings reset to defaults.");
        }

        public void ApplyAllSettings()
        {
            ApplyAudioSettings();
            ApplyGraphicsSettings();
            
            Debug.Log("[SettingsService] All settings applied to Unity systems.");
        }
    }
}
