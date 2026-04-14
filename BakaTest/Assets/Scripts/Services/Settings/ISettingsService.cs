#nullable enable
using System;
using System.Collections.Generic;
using BakaTest.Data.Settings;
using BakaTest.Data.Localization;

// Type aliases to avoid conflicts with UnityEngine types
using AudioSettingsData = BakaTest.Data.Settings.AudioSettings;
using GraphicsSettingsData = BakaTest.Data.Settings.GraphicsSettings;
using GameplaySettingsData = BakaTest.Data.Settings.GameplaySettings;
using AccessibilitySettingsData = BakaTest.Data.Settings.AccessibilitySettings;

namespace BakaTest.Services.Settings
{
    /// <summary>
    /// 設定管理サービスのインターフェース
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>現在の設定データ</summary>
        SettingsData CurrentSettings { get; }

        /// <summary>現在の言語</summary>
        Language CurrentLanguage { get; }

        /// <summary>サポートされている言語のリスト</summary>
        List<Language> SupportedLanguages { get; }

        /// <summary>オーディオ設定</summary>
        AudioSettingsData Audio { get; }

        /// <summary>グラフィックス設定</summary>
        GraphicsSettingsData Graphics { get; }

        /// <summary>ゲームプレイ設定</summary>
        GameplaySettingsData Gameplay { get; }

        /// <summary>アクセシビリティ設定</summary>
        AccessibilitySettingsData Accessibility { get; }

        // === Language Settings ===

        /// <summary>
        /// 言語を設定します
        /// </summary>
        /// <param name="language">言語</param>
        void SetLanguage(Language language);

        // === Audio Settings ===

        /// <summary>
        /// マスターボリュームを設定します
        /// </summary>
        /// <param name="volume">ボリューム (0.0 ~ 1.0)</param>
        void SetMasterVolume(float volume);

        /// <summary>
        /// BGMボリュームを設定します
        /// </summary>
        /// <param name="volume">ボリューム (0.0 ~ 1.0)</param>
        void SetMusicVolume(float volume);

        /// <summary>
        /// 効果音ボリュームを設定します
        /// </summary>
        /// <param name="volume">ボリューム (0.0 ~ 1.0)</param>
        void SetSFXVolume(float volume);

        /// <summary>
        /// ボイスボリュームを設定します
        /// </summary>
        /// <param name="volume">ボリューム (0.0 ~ 1.0)</param>
        void SetVoiceVolume(float volume);

        /// <summary>
        /// UIサウンドボリュームを設定します
        /// </summary>
        /// <param name="volume">ボリューム (0.0 ~ 1.0)</param>
        void SetUIVolume(float volume);

        /// <summary>
        /// マスターボリュームのミュート状態を切り替えます
        /// </summary>
        /// <param name="muted">ミュートするか</param>
        void SetMasterMute(bool muted);

        /// <summary>
        /// BGMのミュート状態を切り替えます
        /// </summary>
        /// <param name="muted">ミュートするか</param>
        void SetMusicMute(bool muted);

        /// <summary>
        /// 効果音のミュート状態を切り替えます
        /// </summary>
        /// <param name="muted">ミュートするか</param>
        void SetSFXMute(bool muted);

        /// <summary>
        /// ボイスのミュート状態を切り替えます
        /// </summary>
        /// <param name="muted">ミュートするか</param>
        void SetVoiceMute(bool muted);

        /// <summary>
        /// UIサウンドのミュート状態を切り替えます
        /// </summary>
        /// <param name="muted">ミュートするか</param>
        void SetUIMute(bool muted);

        // === Graphics Settings ===

        /// <summary>
        /// 画質レベルを設定します
        /// </summary>
        /// <param name="level">画質レベル (0=Low, 1=Medium, 2=High, 3=Ultra)</param>
        void SetQualityLevel(int level);

        /// <summary>
        /// 解像度スケールを設定します
        /// </summary>
        /// <param name="scale">スケール (0.5 ~ 1.0)</param>
        void SetResolutionScale(float scale);

        /// <summary>
        /// フルスクリーンモードを設定します
        /// </summary>
        /// <param name="fullscreen">フルスクリーンにするか</param>
        void SetFullscreen(bool fullscreen);

        /// <summary>
        /// 垂直同期を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetVSync(bool enabled);

        /// <summary>
        /// ターゲットフレームレートを設定します
        /// </summary>
        /// <param name="frameRate">フレームレート (30, 60, 120, -1=無制限)</param>
        void SetTargetFrameRate(int frameRate);

        /// <summary>
        /// アンチエイリアシングを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetAntiAliasing(bool enabled);

        /// <summary>
        /// 影の品質を設定します
        /// </summary>
        /// <param name="quality">品質 (0=Off, 1=Low, 2=Medium, 3=High)</param>
        void SetShadowQuality(int quality);

        /// <summary>
        /// テクスチャ品質を設定します
        /// </summary>
        /// <param name="quality">品質 (0=Low, 1=Medium, 2=High)</param>
        void SetTextureQuality(int quality);

        /// <summary>
        /// パーティクルエフェクトを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetParticleEffects(bool enabled);

        /// <summary>
        /// ポストプロセスを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetPostProcessing(bool enabled);

        // === Gameplay Settings ===

        /// <summary>
        /// バトルスピードを設定します
        /// </summary>
        /// <param name="speed">スピード (0.5x ~ 2.0x)</param>
        void SetBattleSpeed(float speed);

        /// <summary>
        /// オートバトルを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetAutoBattle(bool enabled);

        /// <summary>
        /// カットシーンスキップを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetSkipCutscenes(bool enabled);

        /// <summary>
        /// チュートリアル表示を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetShowTutorials(bool enabled);

        /// <summary>
        /// バトル結果の自動遷移を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetAutoAdvanceBattleResult(bool enabled);

        /// <summary>
        /// 自動遷移までの待機時間を設定します
        /// </summary>
        /// <param name="delay">待機時間（秒）</param>
        void SetAutoAdvanceDelay(float delay);

        /// <summary>
        /// ダメージ数値表示を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetShowDamageNumbers(bool enabled);

        /// <summary>
        /// バトルログ表示を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetShowBattleLog(bool enabled);

        /// <summary>
        /// 振動機能を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetVibrationEnabled(bool enabled);

        // === Accessibility Settings ===

        /// <summary>
        /// UIテキストサイズを設定します
        /// </summary>
        /// <param name="size">サイズ (0.8 ~ 1.5)</param>
        void SetTextSize(float size);

        /// <summary>
        /// 色覚異常モードを設定します
        /// </summary>
        /// <param name="mode">モード</param>
        void SetColorBlindMode(ColorBlindMode mode);

        /// <summary>
        /// ハイコントラストUIを設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetHighContrastUI(bool enabled);

        /// <summary>
        /// 画面揺れ軽減を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetReduceMotion(bool enabled);

        /// <summary>
        /// 点滅エフェクト軽減を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetReduceFlashing(bool enabled);

        /// <summary>
        /// ボタンの長押し時間を設定します
        /// </summary>
        /// <param name="duration">時間（秒）</param>
        void SetButtonHoldDuration(float duration);

        /// <summary>
        /// 自動セーブ通知を設定します
        /// </summary>
        /// <param name="enabled">有効にするか</param>
        void SetShowAutoSaveNotification(bool enabled);

        // === Utility Methods ===

        /// <summary>
        /// すべての設定をデフォルトに戻します
        /// </summary>
        void ResetToDefaults();

        /// <summary>
        /// すべての設定をUnityシステムに適用します
        /// </summary>
        void ApplyAllSettings();

        // === Events ===

        /// <summary>オーディオ設定が変更されたときに発火するイベント</summary>
        event Action<AudioSettingsData>? AudioSettingsChanged;

        /// <summary>グラフィックス設定が変更されたときに発火するイベント</summary>
        event Action<GraphicsSettingsData>? GraphicsSettingsChanged;

        /// <summary>ゲームプレイ設定が変更されたときに発火するイベント</summary>
        event Action<GameplaySettingsData>? GameplaySettingsChanged;

        /// <summary>アクセシビリティ設定が変更されたときに発火するイベント</summary>
        event Action<AccessibilitySettingsData>? AccessibilitySettingsChanged;

        /// <summary>言語が変更されたときに発火するイベント</summary>
        event Action<Language>? LanguageChanged;

        /// <summary>設定が完全にリセットされたときに発火するイベント</summary>
        event Action? SettingsReset;
    }
}
