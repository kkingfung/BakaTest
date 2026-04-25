#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using BakaTest.Core.MVVM;
using BakaTest.Core.Services;
using BakaTest.ViewModels;
using BakaTest.Services.Localization;
using BakaTest.Services.Save;
using BakaTest.Services.SceneManagement;
using BakaTest.Data.Localization;

namespace BakaTest.Views
{
    /// <summary>
    /// 設定画面のView
    /// </summary>
    /// <remarks>
    /// 音量、言語、その他のゲーム設定を管理します。
    /// </remarks>
    [RequireComponent(typeof(UIDocument))]
    public class SettingsView : ViewBase<SettingsViewModel>
    {
        // Audio Settings
        private Slider? _masterVolumeSlider;
        private Slider? _bgmVolumeSlider;
        private Slider? _sfxVolumeSlider;
        private Toggle? _muteToggle;
        private Label? _masterVolumeLabel;
        private Label? _bgmVolumeLabel;
        private Label? _sfxVolumeLabel;

        // Language Settings
        private DropdownField? _languageDropdown;

        // Notification Settings
        private Toggle? _enableNotificationsToggle;
        private Toggle? _enableDailyReminderToggle;
        private Toggle? _enableBattleInvitesToggle;

        // Game Settings
        private Toggle? _showDamageNumbersToggle;
        private Toggle? _showSkillNamesToggle;
        private Toggle? _enableAutoSaveToggle;
        private Slider? _battleSpeedSlider;
        private Label? _battleSpeedLabel;

        // Action Buttons
        private Button? _resetToDefaultButton;
        private Button? _saveAndBackButton;
        private Button? _backWithoutSavingButton;

        protected override void Awake()
        {
            base.Awake();

            // ServiceLocatorからサービスを取得
            var localizationService = ServiceLocator.Instance.Get<ILocalizationService>();
            var saveService = ServiceLocator.Instance.Get<ISaveService>();

            if (localizationService == null || saveService == null)
            {
                Debug.LogError("[SettingsView] Required services not found in ServiceLocator.");
                return;
            }

            // ViewModelを作成
            var viewModel = new SettingsViewModel(localizationService, saveService);
            SetViewModel(viewModel);
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            SetupDropdowns();
            RegisterEventHandlers();
            UpdateUIFromViewModel();
        }

        protected override void BindViewModel(SettingsViewModel viewModel)
        {
            base.BindViewModel(viewModel);

            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
                viewModel.SettingsSaved += OnSettingsSaved;
                viewModel.BackRequested += OnBackRequested;
            }
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
                ViewModel.SettingsSaved -= OnSettingsSaved;
                ViewModel.BackRequested -= OnBackRequested;
            }

            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        /// <summary>
        /// UI要素を取得します
        /// </summary>
        private void GetUIElements()
        {
            if (Root == null) return;

            // Audio Settings
            _masterVolumeSlider = Root.Q<Slider>("MasterVolumeSlider");
            _bgmVolumeSlider = Root.Q<Slider>("BGMVolumeSlider");
            _sfxVolumeSlider = Root.Q<Slider>("SFXVolumeSlider");
            _muteToggle = Root.Q<Toggle>("MuteToggle");
            _masterVolumeLabel = Root.Q<Label>("MasterVolumeLabel");
            _bgmVolumeLabel = Root.Q<Label>("BGMVolumeLabel");
            _sfxVolumeLabel = Root.Q<Label>("SFXVolumeLabel");

            // Language Settings
            _languageDropdown = Root.Q<DropdownField>("LanguageDropdown");

            // Notification Settings
            _enableNotificationsToggle = Root.Q<Toggle>("EnableNotificationsToggle");
            _enableDailyReminderToggle = Root.Q<Toggle>("EnableDailyReminderToggle");
            _enableBattleInvitesToggle = Root.Q<Toggle>("EnableBattleInvitesToggle");

            // Game Settings
            _showDamageNumbersToggle = Root.Q<Toggle>("ShowDamageNumbersToggle");
            _showSkillNamesToggle = Root.Q<Toggle>("ShowSkillNamesToggle");
            _enableAutoSaveToggle = Root.Q<Toggle>("EnableAutoSaveToggle");
            _battleSpeedSlider = Root.Q<Slider>("BattleSpeedSlider");
            _battleSpeedLabel = Root.Q<Label>("BattleSpeedLabel");

            // Action Buttons
            _resetToDefaultButton = Root.Q<Button>("ResetToDefaultButton");
            _saveAndBackButton = Root.Q<Button>("SaveAndBackButton");
            _backWithoutSavingButton = Root.Q<Button>("BackWithoutSavingButton");
        }

        /// <summary>
        /// ドロップダウンの選択肢を設定します
        /// </summary>
        private void SetupDropdowns()
        {
            if (ViewModel == null) return;

            // Language Dropdown
            if (_languageDropdown != null)
            {
                var languageNames = ViewModel.AvailableLanguages.Select(l => GetLanguageDisplayName(l)).ToList();
                _languageDropdown.choices = languageNames;
            }
        }

        /// <summary>
        /// 言語の表示名を取得します
        /// </summary>
        private string GetLanguageDisplayName(Language language)
        {
            return language switch
            {
                Language.Japanese => "日本語",
                Language.English => "English",
                Language.ChineseSimplified => "简体中文",
                Language.ChineseTraditional => "繁體中文",
                Language.Korean => "한국어",
                _ => language.ToString()
            };
        }

        /// <summary>
        /// イベントハンドラを登録します
        /// </summary>
        private void RegisterEventHandlers()
        {
            // Audio Settings
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);

            if (_bgmVolumeSlider != null)
                _bgmVolumeSlider.RegisterValueChangedCallback(OnBGMVolumeChanged);

            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);

            if (_muteToggle != null)
                _muteToggle.RegisterValueChangedCallback(OnMuteChanged);

            // Language Settings
            if (_languageDropdown != null)
                _languageDropdown.RegisterValueChangedCallback(OnLanguageChanged);

            // Notification Settings
            if (_enableNotificationsToggle != null)
                _enableNotificationsToggle.RegisterValueChangedCallback(OnEnableNotificationsChanged);

            if (_enableDailyReminderToggle != null)
                _enableDailyReminderToggle.RegisterValueChangedCallback(OnEnableDailyReminderChanged);

            if (_enableBattleInvitesToggle != null)
                _enableBattleInvitesToggle.RegisterValueChangedCallback(OnEnableBattleInvitesChanged);

            // Game Settings
            if (_showDamageNumbersToggle != null)
                _showDamageNumbersToggle.RegisterValueChangedCallback(OnShowDamageNumbersChanged);

            if (_showSkillNamesToggle != null)
                _showSkillNamesToggle.RegisterValueChangedCallback(OnShowSkillNamesChanged);

            if (_enableAutoSaveToggle != null)
                _enableAutoSaveToggle.RegisterValueChangedCallback(OnEnableAutoSaveChanged);

            if (_battleSpeedSlider != null)
                _battleSpeedSlider.RegisterValueChangedCallback(OnBattleSpeedChanged);

            // Action Buttons
            if (_resetToDefaultButton != null)
                _resetToDefaultButton.clicked += OnResetToDefaultClicked;

            if (_saveAndBackButton != null)
                _saveAndBackButton.clicked += OnSaveAndBackClicked;

            if (_backWithoutSavingButton != null)
                _backWithoutSavingButton.clicked += OnBackWithoutSavingClicked;
        }

        /// <summary>
        /// イベントハンドラを解除します
        /// </summary>
        private void UnregisterEventHandlers()
        {
            // Audio Settings
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChanged);

            if (_bgmVolumeSlider != null)
                _bgmVolumeSlider.UnregisterValueChangedCallback(OnBGMVolumeChanged);

            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.UnregisterValueChangedCallback(OnSFXVolumeChanged);

            if (_muteToggle != null)
                _muteToggle.UnregisterValueChangedCallback(OnMuteChanged);

            // Language Settings
            if (_languageDropdown != null)
                _languageDropdown.UnregisterValueChangedCallback(OnLanguageChanged);

            // Notification Settings
            if (_enableNotificationsToggle != null)
                _enableNotificationsToggle.UnregisterValueChangedCallback(OnEnableNotificationsChanged);

            if (_enableDailyReminderToggle != null)
                _enableDailyReminderToggle.UnregisterValueChangedCallback(OnEnableDailyReminderChanged);

            if (_enableBattleInvitesToggle != null)
                _enableBattleInvitesToggle.UnregisterValueChangedCallback(OnEnableBattleInvitesChanged);

            // Game Settings
            if (_showDamageNumbersToggle != null)
                _showDamageNumbersToggle.UnregisterValueChangedCallback(OnShowDamageNumbersChanged);

            if (_showSkillNamesToggle != null)
                _showSkillNamesToggle.UnregisterValueChangedCallback(OnShowSkillNamesChanged);

            if (_enableAutoSaveToggle != null)
                _enableAutoSaveToggle.UnregisterValueChangedCallback(OnEnableAutoSaveChanged);

            if (_battleSpeedSlider != null)
                _battleSpeedSlider.UnregisterValueChangedCallback(OnBattleSpeedChanged);

            // Action Buttons
            if (_resetToDefaultButton != null)
                _resetToDefaultButton.clicked -= OnResetToDefaultClicked;

            if (_saveAndBackButton != null)
                _saveAndBackButton.clicked -= OnSaveAndBackClicked;

            if (_backWithoutSavingButton != null)
                _backWithoutSavingButton.clicked -= OnBackWithoutSavingClicked;
        }

        /// <summary>
        /// ViewModelからUIを更新します
        /// </summary>
        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;

            // Audio Settings
            if (_masterVolumeSlider != null) _masterVolumeSlider.value = ViewModel.MasterVolume;
            if (_bgmVolumeSlider != null) _bgmVolumeSlider.value = ViewModel.BGMVolume;
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = ViewModel.SFXVolume;
            if (_muteToggle != null) _muteToggle.value = ViewModel.IsMuted;
            if (_masterVolumeLabel != null) _masterVolumeLabel.text = ViewModel.MasterVolumeText;
            if (_bgmVolumeLabel != null) _bgmVolumeLabel.text = ViewModel.BGMVolumeText;
            if (_sfxVolumeLabel != null) _sfxVolumeLabel.text = ViewModel.SFXVolumeText;

            // Language Settings
            if (_languageDropdown != null)
            {
                var languageName = GetLanguageDisplayName(ViewModel.CurrentLanguage);
                _languageDropdown.value = languageName;
            }

            // Notification Settings
            if (_enableNotificationsToggle != null) _enableNotificationsToggle.value = ViewModel.EnableNotifications;
            if (_enableDailyReminderToggle != null) _enableDailyReminderToggle.value = ViewModel.EnableDailyReminder;
            if (_enableBattleInvitesToggle != null) _enableBattleInvitesToggle.value = ViewModel.EnableBattleInvites;

            // Game Settings
            if (_showDamageNumbersToggle != null) _showDamageNumbersToggle.value = ViewModel.ShowDamageNumbers;
            if (_showSkillNamesToggle != null) _showSkillNamesToggle.value = ViewModel.ShowSkillNames;
            if (_enableAutoSaveToggle != null) _enableAutoSaveToggle.value = ViewModel.EnableAutoSave;
            if (_battleSpeedSlider != null) _battleSpeedSlider.value = ViewModel.BattleSpeed;
            if (_battleSpeedLabel != null) _battleSpeedLabel.text = ViewModel.BattleSpeedText;
        }

        // ===== Event Handlers =====

        private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;

            switch (e.PropertyName)
            {
                case nameof(ViewModel.MasterVolume):
                    if (_masterVolumeSlider != null && Math.Abs(_masterVolumeSlider.value - ViewModel.MasterVolume) > 0.01f)
                        _masterVolumeSlider.value = ViewModel.MasterVolume;
                    if (_masterVolumeLabel != null) _masterVolumeLabel.text = ViewModel.MasterVolumeText;
                    break;

                case nameof(ViewModel.BGMVolume):
                    if (_bgmVolumeSlider != null && Math.Abs(_bgmVolumeSlider.value - ViewModel.BGMVolume) > 0.01f)
                        _bgmVolumeSlider.value = ViewModel.BGMVolume;
                    if (_bgmVolumeLabel != null) _bgmVolumeLabel.text = ViewModel.BGMVolumeText;
                    break;

                case nameof(ViewModel.SFXVolume):
                    if (_sfxVolumeSlider != null && Math.Abs(_sfxVolumeSlider.value - ViewModel.SFXVolume) > 0.01f)
                        _sfxVolumeSlider.value = ViewModel.SFXVolume;
                    if (_sfxVolumeLabel != null) _sfxVolumeLabel.text = ViewModel.SFXVolumeText;
                    break;

                case nameof(ViewModel.IsMuted):
                    if (_muteToggle != null && _muteToggle.value != ViewModel.IsMuted)
                        _muteToggle.value = ViewModel.IsMuted;
                    break;

                case nameof(ViewModel.CurrentLanguage):
                    if (_languageDropdown != null)
                    {
                        var languageName = GetLanguageDisplayName(ViewModel.CurrentLanguage);
                        if (_languageDropdown.value != languageName)
                            _languageDropdown.value = languageName;
                    }
                    break;

                case nameof(ViewModel.BattleSpeed):
                    if (_battleSpeedSlider != null && Math.Abs(_battleSpeedSlider.value - ViewModel.BattleSpeed) > 0.01f)
                        _battleSpeedSlider.value = ViewModel.BattleSpeed;
                    if (_battleSpeedLabel != null) _battleSpeedLabel.text = ViewModel.BattleSpeedText;
                    break;
            }
        }

        private void OnSettingsSaved(object? sender, EventArgs e)
        {
            Debug.Log("[SettingsView] Settings saved successfully.");
            // TODO: 保存成功フィードバック（トースト通知など）
        }

        private void OnBackRequested(object? sender, EventArgs e)
        {
            var sceneService = ServiceLocator.Instance.Get<ISceneManagementService>();
            if (sceneService == null)
            {
                Debug.LogError("[SettingsView] ISceneManagementService not found.");
                return;
            }

            sceneService.LoadMainMenu();
        }

        // Audio Settings
        private void OnMasterVolumeChanged(ChangeEvent<float> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.MasterVolume = evt.newValue;
            }
        }

        private void OnBGMVolumeChanged(ChangeEvent<float> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.BGMVolume = evt.newValue;
            }
        }

        private void OnSFXVolumeChanged(ChangeEvent<float> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.SFXVolume = evt.newValue;
            }
        }

        private void OnMuteChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.IsMuted = evt.newValue;
            }
        }

        // Language Settings
        private void OnLanguageChanged(ChangeEvent<string> evt)
        {
            if (ViewModel == null) return;

            // 表示名から言語enumを逆変換
            var language = evt.newValue switch
            {
                "日本語" => Language.Japanese,
                "English" => Language.English,
                "简体中文" => Language.ChineseSimplified,
                "繁體中文" => Language.ChineseTraditional,
                "한국어" => Language.Korean,
                _ => Language.English
            };

            ViewModel.CurrentLanguage = language;
        }

        // Notification Settings
        private void OnEnableNotificationsChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.EnableNotifications = evt.newValue;
            }
        }

        private void OnEnableDailyReminderChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.EnableDailyReminder = evt.newValue;
            }
        }

        private void OnEnableBattleInvitesChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.EnableBattleInvites = evt.newValue;
            }
        }

        // Game Settings
        private void OnShowDamageNumbersChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowDamageNumbers = evt.newValue;
            }
        }

        private void OnShowSkillNamesChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.ShowSkillNames = evt.newValue;
            }
        }

        private void OnEnableAutoSaveChanged(ChangeEvent<bool> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.EnableAutoSave = evt.newValue;
            }
        }

        private void OnBattleSpeedChanged(ChangeEvent<float> evt)
        {
            if (ViewModel != null)
            {
                ViewModel.BattleSpeed = evt.newValue;
            }
        }

        // Action Buttons
        private void OnResetToDefaultClicked()
        {
            ViewModel?.ResetToDefaultCommand.Execute(null);
        }

        private void OnSaveAndBackClicked()
        {
            ViewModel?.SaveAndBackCommand.Execute(null);
        }

        private void OnBackWithoutSavingClicked()
        {
            ViewModel?.BackWithoutSavingCommand.Execute(null);
        }
    }
}
