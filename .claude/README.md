# Claude Code Configuration

このディレクトリには、BakaTestプロジェクトのClaude Code設定が含まれています。

## 📁 ディレクトリ構造

```
.claude/
├── commands/          # スラッシュコマンド
│   ├── create-viewmodel.md
│   ├── create-view.md
│   ├── create-service.md
│   ├── create-champion.md
│   ├── create-test-system.md
│   └── review-mvvm.md
├── skills/           # 再利用可能なワークフロー
│   ├── mvvm-implementation.md
│   ├── battle-system.md
│   └── ui-toolkit-design.md
├── settings.local.json
└── README.md         # このファイル
```

## 🎯 スラッシュコマンド

### MVVM開発

#### `/create-viewmodel`
新しいViewModelクラスを生成します。
- ViewModelBase継承
- プロパティとコマンドのボイラープレート
- サービス依存性注入
- 日本語コメント

**使用例:**
```
/create-viewmodel
```
→ Claudeが名前とサービスを聞いてきます

---

#### `/create-view`
新しいViewクラスを生成します。
- UIToolkitViewBase<T>継承
- UI要素の取得・イベント登録パターン
- データバインディング実装
- 日本語コメント

**使用例:**
```
/create-view
```
→ Claudeが対応するViewModelと必要なUI要素を聞いてきます

---

#### `/create-service`
新しいサービス（インターフェース + 実装）を生成します。
- IServiceNameインターフェース
- ServiceName実装クラス
- イベントパターン
- ServiceLocator登録準備

**使用例:**
```
/create-service
```
→ Claudeがサービス名と必要な機能を聞いてきます

---

### ゲームシステム開発

#### `/create-champion`
チャンピオンデータ用のScriptableObjectを生成します。
- ChampionData定義
- ステータス構造体
- 教科親和性システム
- アビリティデータ

**使用例:**
```
/create-champion
```
→ Claudeがチャンピオンの詳細を聞いてきます

---

#### `/create-test-system`
テストシステムのコンポーネントを生成します。
- TestQuestion構造
- ITestService
- AI生成/レビューサービス
- TestConfig

**使用例:**
```
/create-test-system
```
→ Claudeがどのコンポーネントから始めるか聞いてきます

---

### コードレビュー

#### `/review-mvvm`
既存のView/ViewModelをレビューします。
- MVVMパターン準拠チェック
- コーディング規約確認
- 改善提案
- 修正例の提示

**使用例:**
```
/review-mvvm
```
→ Claudeがレビュー対象のファイルパスを聞いてきます

---

## 🛠️ スキル

スキルは複雑なワークフローを段階的に実行する再利用可能なプロンプトです。

### `mvvm-implementation`
完全なMVVM機能の実装ワークフロー
1. 要件ヒアリング
2. ViewModel作成
3. View作成
4. UXML設計
5. USSスタイリング
6. 統合とテスト
7. レビュー

**使用方法:**
```
Claude, use the mvvm-implementation skill to create a [feature name] screen
```

---

### `battle-system`
バトルシステムコンポーネントの実装ガイド
- BattleManager
- BattleUnit
- DamageCalculator
- BattleUI
- AIシステム

**使用方法:**
```
Claude, use the battle-system skill to implement [component name]
```

---

### `ui-toolkit-design`
UI Toolkit画面の設計・実装ワークフロー
- プロジェクトスタイルガイド準拠
- UXML/USS作成
- レスポンシブデザイン
- よく使うUIパターン

**使用方法:**
```
Claude, use the ui-toolkit-design skill to create UI for [screen name]
```

---

## ⚙️ 設定ファイル

### `settings.local.json`
Claude Codeのローカル設定
- MCPサーバー権限
- ツール許可設定

**注意:** このファイルはgitignoreに含めるべきです（個人設定のため）

---

## 📝 使用のベストプラクティス

### 1. スラッシュコマンドを活用
定型的なボイラープレート生成には積極的にスラッシュコマンドを使用してください。
```
/create-viewmodel
/create-view
```

### 2. スキルで大きな機能を実装
複数のコンポーネントにまたがる機能は、スキルを使って段階的に実装してください。
```
Claude, use the mvvm-implementation skill to create a Test Selection screen
```

### 3. レビューで品質保証
実装後は必ずレビューコマンドで確認してください。
```
/review-mvvm
```

### 4. CLAUDE.mdと併用
プロジェクト全体のコーディング規約は`CLAUDE.md`に記載されています。
スラッシュコマンドとスキルはこれに準拠しています。

---

## 🔄 更新とカスタマイズ

### 新しいコマンドを追加
`.claude/commands/your-command.md`を作成してください。

### 新しいスキルを追加
`.claude/skills/your-skill.md`を作成してください。

### コマンド/スキルの編集
既存のファイルを直接編集してカスタマイズできます。

---

## 📚 参考資料

- [Claude Code Documentation](https://docs.claude.ai/claude-code)
- [Unity UI Toolkit Documentation](https://docs.unity3d.com/Manual/UIElements.html)
- プロジェクト固有のドキュメント: `CLAUDE.md`
- ゲーム企画書: `GameProposal.md` / `GameProposal_JP.md`

---

**最終更新:** 2026-04-02
