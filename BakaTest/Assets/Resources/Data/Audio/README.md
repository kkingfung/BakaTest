# Audio Assets Setup Guide

## 概要 (Overview)

このディレクトリには、BakaTestプロジェクトで使用するAudioClipDataアセットが含まれています。
現在、プレースホルダーアセットが作成されていますが、実際のAudioClipは未割り当てです。

## 現在の構造 (Current Structure)

```
Audio/
├── Music/          音楽（BGM）
│   ├── BGM_MainMenu.asset
│   ├── BGM_Battle.asset
│   └── BGM_Victory.asset
├── SFX/            効果音
├── Voice/          ボイス
└── UI/             UI音
    ├── UI_Click.asset
    ├── UI_Hover.asset
    ├── UI_Success.asset
    └── UI_Error.asset
```

## セットアップ手順 (Setup Steps)

### 1. オーディオファイルをインポート

1. Unity Editorで、オーディオファイル（.mp3, .wav, .ogg等）をプロジェクトにインポート
2. 推奨インポート先: `Assets/Audio/` フォルダ（作成してください）

### 2. AudioClipDataアセットにAudioClipを割り当て

1. Unity Editorで、各 `.asset` ファイルを選択
2. Inspectorで `Clip` フィールドにインポートしたAudioClipをドラッグ&ドロップ
3. 必要に応じて他のパラメータ（Volume, Pitch等）を調整

### 3. 推奨設定

#### Music (BGM)
- **Loop**: ✓ (チェックを入れる)
- **Volume**: 0.6 - 0.8
- **Fade In Duration**: 1 - 2秒
- **Fade Out Duration**: 1.5 - 2秒
- **Audio Type**: Music

#### UI Sounds
- **Loop**: ✗ (チェックを外す)
- **Volume**: 0.5 - 0.8
- **Volume Variation**: 0.05 - 0.1 (自然なバリエーション)
- **Pitch Variation**: 0.03 - 0.05 (自然なバリエーション)
- **Audio Type**: UI

#### SFX (効果音)
- **Loop**: ✗ (通常は外す)
- **Volume**: 0.6 - 1.0
- **Volume Variation**: 0.1 - 0.2
- **Pitch Variation**: 0.05 - 0.15
- **Audio Type**: SFX
- **Spatial Audio**: 3D空間で再生する場合はチェック

## AudioService仕様

### 自動読み込み

GameBootstrap起動時に、`Resources/Data/Audio/` 以下のすべてのAudioClipDataアセットが自動的にロードされます。

```csharp
// GameBootstrap.cs
audioService.LoadAllAudioFromResources();
```

### コードから使用

```csharp
// AudioServiceを取得
var audioService = ServiceLocator.Instance.Get<IAudioService>();

// 音楽再生（フェードイン）
audioService.PlayMusic("bgm_mainmenu", fadeIn: 2f);

// UI音再生
audioService.PlayUISound("ui_click");
audioService.PlayButtonClick();  // ui_click のエイリアス
audioService.PlaySuccessSound(); // ui_success のエイリアス
audioService.PlayErrorSound();   // ui_error のエイリアス

// 効果音再生
audioService.PlaySFX("sfx_explosion");

// 3D空間で効果音再生
audioService.PlaySFXAtPosition("sfx_footstep", transform.position);
```

## audioId一覧 (Audio IDs)

現在定義されているaudioIdと対応するメソッド:

| audioId | ファイル | タイプ | エイリアスメソッド |
|---------|----------|--------|-------------------|
| `bgm_mainmenu` | BGM_MainMenu.asset | Music | - |
| `bgm_battle` | BGM_Battle.asset | Music | - |
| `bgm_victory` | BGM_Victory.asset | Music | - |
| `ui_click` | UI_Click.asset | UI | `PlayButtonClick()` |
| `ui_hover` | UI_Hover.asset | UI | `PlayButtonHover()` |
| `ui_success` | UI_Success.asset | UI | `PlaySuccessSound()` |
| `ui_error` | UI_Error.asset | UI | `PlayErrorSound()` |

## 追加のAudioClipData作成

新しいAudioClipDataアセットを作成するには:

1. Unity Editorで右クリック → `Create` → `BakaTest` → `Audio Clip Data`
2. 適切なフォルダ（Music/SFX/Voice/UI）に配置
3. 設定を入力:
   - **audioId**: 一意の識別子（例: `sfx_button_click`）
   - **displayName**: 人間が読める名前
   - **clip**: AudioClip参照
   - **audioType**: Music/SFX/Voice/UI から選択
   - その他のパラメータを調整
4. プレイモードで自動的にロードされます

## デバッグツール

Unity Editorメニューから `BakaTest` → `Debug` → `Audio` で以下のツールが使用可能:

- **Print Audio Status**: 登録されたオーディオ数と現在の状態を表示
- **Test UI Sounds**: 各UI音をテスト再生
- **Stop All SFX**: すべての効果音を停止

## トラブルシューティング

### AudioClipが再生されない
1. AudioClipDataの`clip`フィールドが割り当てられているか確認
2. `audioId`が正しいか確認
3. Volume設定が0になっていないか確認
4. Master VolumeやUI Volumeが0になっていないか確認（Settings）

### "Audio not found in database"エラー
1. `audioId`のスペルミスを確認
2. AudioClipDataが`Resources/Data/Audio/`配下にあるか確認
3. プレイモードで再読み込み（再生モードに入り直す）

## 参考ドキュメント

詳細な実装情報は以下を参照:
- `POLISH_FEATURES_COMPLETE.md` - Audio Management System完全ガイド
- `AudioService.cs` - 実装詳細
- `IAudioService.cs` - API仕様
