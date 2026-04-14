#nullable enable
using System;
using UnityEngine;
using BakaTest.Data.Localization;

namespace BakaTest.Data.Settings
{
    /// <summary>
    /// 設定データのルートクラス
    /// </summary>
    [Serializable]
    public class SettingsData
    {
        /// <summary>言語設定</summary>
        public Language language = Language.English;

        /// <summary>オーディオ設定</summary>
        public AudioSettings audio = new AudioSettings();

        /// <summary>グラフィックス設定</summary>
        public GraphicsSettings graphics = new GraphicsSettings();

        /// <summary>ゲームプレイ設定</summary>
        public GameplaySettings gameplay = new GameplaySettings();

        /// <summary>アクセシビリティ設定</summary>
        public AccessibilitySettings accessibility = new AccessibilitySettings();

        /// <summary>
        /// デフォルト設定を返します
        /// </summary>
        public static SettingsData GetDefault()
        {
            return new SettingsData
            {
                language = Language.English,
                audio = AudioSettings.GetDefault(),
                graphics = GraphicsSettings.GetDefault(),
                gameplay = GameplaySettings.GetDefault(),
                accessibility = AccessibilitySettings.GetDefault()
            };
        }
    }

    /// <summary>
    /// オーディオ設定
    /// </summary>
    [Serializable]
    public class AudioSettings
    {
        /// <summary>マスターボリューム (0.0 ~ 1.0)</summary>
        [Range(0f, 1f)]
        public float masterVolume = 1.0f;

        /// <summary>BGMボリューム (0.0 ~ 1.0)</summary>
        [Range(0f, 1f)]
        public float musicVolume = 0.8f;

        /// <summary>効果音ボリューム (0.0 ~ 1.0)</summary>
        [Range(0f, 1f)]
        public float sfxVolume = 0.9f;

        /// <summary>ボイスボリューム (0.0 ~ 1.0)</summary>
        [Range(0f, 1f)]
        public float voiceVolume = 1.0f;

        /// <summary>UIサウンドボリューム (0.0 ~ 1.0)</summary>
        [Range(0f, 1f)]
        public float uiVolume = 0.7f;

        /// <summary>マスターボリュームがミュートか</summary>
        public bool masterMuted = false;

        /// <summary>BGMがミュートか</summary>
        public bool musicMuted = false;

        /// <summary>効果音がミュートか</summary>
        public bool sfxMuted = false;

        /// <summary>ボイスがミュートか</summary>
        public bool voiceMuted = false;

        /// <summary>UIサウンドがミュートか</summary>
        public bool uiMuted = false;

        /// <summary>
        /// デフォルト設定を返します
        /// </summary>
        public static AudioSettings GetDefault()
        {
            return new AudioSettings
            {
                masterVolume = 1.0f,
                musicVolume = 0.8f,
                sfxVolume = 0.9f,
                voiceVolume = 1.0f,
                uiVolume = 0.7f,
                masterMuted = false,
                musicMuted = false,
                sfxMuted = false,
                voiceMuted = false,
                uiMuted = false
            };
        }
    }

    /// <summary>
    /// グラフィックス設定
    /// </summary>
    [Serializable]
    public class GraphicsSettings
    {
        /// <summary>画質レベル (0=Low, 1=Medium, 2=High, 3=Ultra)</summary>
        public int qualityLevel = 2;

        /// <summary>解像度スケール (0.5 ~ 1.0)</summary>
        [Range(0.5f, 1.0f)]
        public float resolutionScale = 1.0f;

        /// <summary>フルスクリーンモード</summary>
        public bool fullscreen = true;

        /// <summary>垂直同期</summary>
        public bool vsync = true;

        /// <summary>FPS制限 (30, 60, 120, -1=無制限)</summary>
        public int targetFrameRate = 60;

        /// <summary>アンチエイリアシング有効</summary>
        public bool antiAliasing = true;

        /// <summary>影の品質 (0=Off, 1=Low, 2=Medium, 3=High)</summary>
        public int shadowQuality = 2;

        /// <summary>テクスチャ品質 (0=Low, 1=Medium, 2=High)</summary>
        public int textureQuality = 2;

        /// <summary>パーティクルエフェクト有効</summary>
        public bool particleEffects = true;

        /// <summary>ポストプロセス有効</summary>
        public bool postProcessing = true;

        /// <summary>
        /// デフォルト設定を返します
        /// </summary>
        public static GraphicsSettings GetDefault()
        {
            return new GraphicsSettings
            {
                qualityLevel = 2,
                resolutionScale = 1.0f,
                fullscreen = true,
                vsync = true,
                targetFrameRate = 60,
                antiAliasing = true,
                shadowQuality = 2,
                textureQuality = 2,
                particleEffects = true,
                postProcessing = true
            };
        }
    }

    /// <summary>
    /// ゲームプレイ設定
    /// </summary>
    [Serializable]
    public class GameplaySettings
    {
        /// <summary>バトルスピード (0.5x ~ 2.0x)</summary>
        [Range(0.5f, 2.0f)]
        public float battleSpeed = 1.0f;

        /// <summary>オートバトル有効</summary>
        public bool autoBattle = true;

        /// <summary>スキップ可能なカットシーン</summary>
        public bool skipCutscenes = false;

        /// <summary>チュートリアル表示</summary>
        public bool showTutorials = true;

        /// <summary>バトル結果の自動遷移</summary>
        public bool autoAdvanceBattleResult = false;

        /// <summary>自動遷移までの待機時間（秒）</summary>
        [Range(1f, 10f)]
        public float autoAdvanceDelay = 3.0f;

        /// <summary>ダメージ数値表示</summary>
        public bool showDamageNumbers = true;

        /// <summary>バトルログ表示</summary>
        public bool showBattleLog = true;

        /// <summary>振動機能有効（モバイル）</summary>
        public bool vibrationEnabled = true;

        /// <summary>
        /// デフォルト設定を返します
        /// </summary>
        public static GameplaySettings GetDefault()
        {
            return new GameplaySettings
            {
                battleSpeed = 1.0f,
                autoBattle = true,
                skipCutscenes = false,
                showTutorials = true,
                autoAdvanceBattleResult = false,
                autoAdvanceDelay = 3.0f,
                showDamageNumbers = true,
                showBattleLog = true,
                vibrationEnabled = true
            };
        }
    }

    /// <summary>
    /// アクセシビリティ設定
    /// </summary>
    [Serializable]
    public class AccessibilitySettings
    {
        /// <summary>UIテキストサイズ (0.8 ~ 1.5)</summary>
        [Range(0.8f, 1.5f)]
        public float textSize = 1.0f;

        /// <summary>色覚異常モード</summary>
        public ColorBlindMode colorBlindMode = ColorBlindMode.None;

        /// <summary>ハイコントラストUI</summary>
        public bool highContrastUI = false;

        /// <summary>画面揺れ軽減</summary>
        public bool reduceMotion = false;

        /// <summary>点滅エフェクト軽減</summary>
        public bool reduceFlashing = false;

        /// <summary>ボタンの長押し時間（秒）</summary>
        [Range(0.3f, 2.0f)]
        public float buttonHoldDuration = 0.5f;

        /// <summary>自動セーブ通知</summary>
        public bool showAutoSaveNotification = true;

        /// <summary>
        /// デフォルト設定を返します
        /// </summary>
        public static AccessibilitySettings GetDefault()
        {
            return new AccessibilitySettings
            {
                textSize = 1.0f,
                colorBlindMode = ColorBlindMode.None,
                highContrastUI = false,
                reduceMotion = false,
                reduceFlashing = false,
                buttonHoldDuration = 0.5f,
                showAutoSaveNotification = true
            };
        }
    }

    /// <summary>
    /// 色覚異常モード
    /// </summary>
    public enum ColorBlindMode
    {
        /// <summary>なし</summary>
        None = 0,

        /// <summary>1型2色覚（赤色弱）</summary>
        Protanopia = 1,

        /// <summary>2型2色覚（緑色弱）</summary>
        Deuteranopia = 2,

        /// <summary>3型2色覚（青色弱）</summary>
        Tritanopia = 3
    }
}
