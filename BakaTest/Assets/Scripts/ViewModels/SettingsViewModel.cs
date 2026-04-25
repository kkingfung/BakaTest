#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Core.MVVM;
using BakaTest.Core.Commands;
using BakaTest.Data.Localization;
using BakaTest.Services.Localization;
using BakaTest.Services.Save;
using UnityEngine;

namespace BakaTest.ViewModels
{
    /// <summary>
    /// 設定画面のViewModel
    /// </summary>
    /// <remarks>
    /// 音量、言語、その他のゲーム設定を管理します。
    /// </remarks>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISaveService _saveService;

        // 音量設定
        private float _masterVolume = 1.0f;
        private float _bgmVolume = 0.7f;
        private float _sfxVolume = 0.8f;
        private bool _isMuted;

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (SetProperty(ref _masterVolume, Mathf.Clamp01(value)))
                {
                    ApplyAudioSettings();
                }
            }
        }

        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                if (SetProperty(ref _bgmVolume, Mathf.Clamp01(value)))
                {
                    ApplyAudioSettings();
                }
            }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                if (SetProperty(ref _sfxVolume, Mathf.Clamp01(value)))
                {
                    ApplyAudioSettings();
                }
            }
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (SetProperty(ref _isMuted, value))
                {
                    ApplyAudioSettings();
                }
            }
        }

        // パーセンテージ表示
        public string MasterVolumeText => $"{(MasterVolume * 100):F0}%";
        public string BGMVolumeText => $"{(BGMVolume * 100):F0}%";
        public string SFXVolumeText => $"{(SFXVolume * 100):F0}%";

        // 言語設定
        private Language _currentLanguage;
        public Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (SetProperty(ref _currentLanguage, value))
                {
                    ApplyLanguage();
                }
            }
        }

        public List<Language> AvailableLanguages { get; } = new()
        {
            Language.Japanese,
            Language.English,
            Language.ChineseSimplified,
            Language.ChineseTraditional,
            Language.Korean
        };

        // 通知設定
        private bool _enableNotifications = true;
        private bool _enableDailyReminder = true;
        private bool _enableBattleInvites = true;

        public bool EnableNotifications
        {
            get => _enableNotifications;
            set => SetProperty(ref _enableNotifications, value);
        }

        public bool EnableDailyReminder
        {
            get => _enableDailyReminder;
            set => SetProperty(ref _enableDailyReminder, value);
        }

        public bool EnableBattleInvites
        {
            get => _enableBattleInvites;
            set => SetProperty(ref _enableBattleInvites, value);
        }

        // ゲーム設定
        private bool _showDamageNumbers = true;
        private bool _showSkillNames = true;
        private bool _enableAutoSave = true;
        private float _battleSpeed = 1.0f;

        public bool ShowDamageNumbers
        {
            get => _showDamageNumbers;
            set => SetProperty(ref _showDamageNumbers, value);
        }

        public bool ShowSkillNames
        {
            get => _showSkillNames;
            set => SetProperty(ref _showSkillNames, value);
        }

        public bool EnableAutoSave
        {
            get => _enableAutoSave;
            set => SetProperty(ref _enableAutoSave, value);
        }

        public float BattleSpeed
        {
            get => _battleSpeed;
            set => SetProperty(ref _battleSpeed, Mathf.Clamp(value, 0.5f, 2.0f));
        }

        public string BattleSpeedText => $"{BattleSpeed:F1}x";

        // Commands
        public RelayCommand<object?> ResetToDefaultCommand { get; }
        public RelayCommand<object?> SaveAndBackCommand { get; }
        public RelayCommand<object?> BackWithoutSavingCommand { get; }

        // Events
        public event EventHandler? SettingsSaved;
        public event EventHandler? BackRequested;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsViewModel(ILocalizationService localizationService, ISaveService saveService)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            // Commands
            ResetToDefaultCommand = new RelayCommand<object?>(_ => ExecuteResetToDefault());
            SaveAndBackCommand = new RelayCommand<object?>(_ => ExecuteSaveAndBack());
            BackWithoutSavingCommand = new RelayCommand<object?>(_ => ExecuteBackWithoutSaving());

            // 現在の設定をロード
            LoadSettings();

            Debug.Log("[SettingsViewModel] Initialized.");
        }

        /// <summary>
        /// 設定をロードします
        /// </summary>
        private void LoadSettings()
        {
            var saveData = _saveService.CurrentSaveData.settingsData;

            // 音量設定
            MasterVolume = saveData.audio.masterVolume;
            BGMVolume = saveData.audio.musicVolume;
            SFXVolume = saveData.audio.sfxVolume;
            IsMuted = saveData.audio.masterMuted;

            // 言語設定
            CurrentLanguage = _localizationService.CurrentLanguage;

            // 通知設定（現在はSettingsDataにないのでデフォルト値を使用）
            EnableNotifications = true;
            EnableDailyReminder = true;
            EnableBattleInvites = true;

            // ゲーム設定
            ShowDamageNumbers = saveData.gameplay.showDamageNumbers;
            ShowSkillNames = true; // SettingsDataにないのでデフォルト
            EnableAutoSave = true; // SettingsDataにないのでデフォルト
            BattleSpeed = saveData.gameplay.battleSpeed;

            Debug.Log("[SettingsViewModel] Settings loaded.");
        }

        /// <summary>
        /// 設定を保存します
        /// </summary>
        private void SaveSettings()
        {
            var saveData = _saveService.CurrentSaveData.settingsData;

            // 音量設定
            saveData.audio.masterVolume = MasterVolume;
            saveData.audio.musicVolume = BGMVolume;
            saveData.audio.sfxVolume = SFXVolume;
            saveData.audio.masterMuted = IsMuted;

            // 言語設定（LocalizationServiceは自動保存）

            // 通知設定（現在はSettingsDataにフィールドがないのでスキップ）
            // TODO: 必要ならSettingsDataにフィールドを追加

            // ゲーム設定
            saveData.gameplay.showDamageNumbers = ShowDamageNumbers;
            // ShowSkillNamesとEnableAutoSaveは現在SettingsDataにない
            saveData.gameplay.battleSpeed = BattleSpeed;

            _saveService.MarkDirty();
            _saveService.Save();

            Debug.Log("[SettingsViewModel] Settings saved.");
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// オーディオ設定を適用します
        /// </summary>
        private void ApplyAudioSettings()
        {
            if (IsMuted)
            {
                AudioListener.volume = 0f;
            }
            else
            {
                AudioListener.volume = MasterVolume;
            }

            // TODO: BGM と SFX の個別音量をAudioMixerで設定

            OnPropertyChanged(nameof(MasterVolumeText));
            OnPropertyChanged(nameof(BGMVolumeText));
            OnPropertyChanged(nameof(SFXVolumeText));

            Debug.Log($"[SettingsViewModel] Audio settings applied: Master={MasterVolume:F2}, Muted={IsMuted}");
        }

        /// <summary>
        /// 言語設定を適用します
        /// </summary>
        private void ApplyLanguage()
        {
            _localizationService.SetLanguage(CurrentLanguage);
            Debug.Log($"[SettingsViewModel] Language changed to: {CurrentLanguage}");
        }

        /// <summary>
        /// デフォルト設定に戻します
        /// </summary>
        private void ExecuteResetToDefault()
        {
            Debug.Log("[SettingsViewModel] Resetting to default settings...");

            // 音量設定
            MasterVolume = 1.0f;
            BGMVolume = 0.7f;
            SFXVolume = 0.8f;
            IsMuted = false;

            // 言語設定（英語デフォルト）
            CurrentLanguage = Language.English;

            // 通知設定
            EnableNotifications = true;
            EnableDailyReminder = true;
            EnableBattleInvites = true;

            // ゲーム設定
            ShowDamageNumbers = true;
            ShowSkillNames = true;
            EnableAutoSave = true;
            BattleSpeed = 1.0f;

            OnPropertyChanged(nameof(BattleSpeedText));

            Debug.Log("[SettingsViewModel] Settings reset to default.");
        }

        /// <summary>
        /// 保存して戻ります
        /// </summary>
        private void ExecuteSaveAndBack()
        {
            Debug.Log("[SettingsViewModel] Saving settings and going back...");
            SaveSettings();
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 保存せずに戻ります
        /// </summary>
        private void ExecuteBackWithoutSaving()
        {
            Debug.Log("[SettingsViewModel] Going back without saving...");
            // 設定をリロード（変更を破棄）
            LoadSettings();
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.Log("[SettingsViewModel] Disposed.");
            }
            base.Dispose(disposing);
        }
    }
}
