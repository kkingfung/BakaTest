# MVVM Implementation Skill

このスキルは、完全なMVVM機能（ViewModel + View + UXML + USS）を実装します。

## 実行フロー

1. **要件ヒアリング**
   - 機能名を確認
   - 必要なUI要素を確認
   - 必要なサービスを確認
   - データフローを確認

2. **ViewModel作成**
   - `/create-viewmodel`コマンドを使用
   - プロパティとコマンドを定義
   - サービスとの連携を実装
   - イベント購読とDisposeを実装

3. **View作成**
   - `/create-view`コマンドを使用
   - UI要素の取得ロジック
   - イベントハンドラ登録/解除
   - データバインディング実装

4. **UXML作成**
   - UI構造を設計
   - 適切なname属性を設定
   - USSスタイルシートを参照
   - レイアウトを構築

5. **USS作成**
   - プロジェクトのスタイルガイドに従う
   - コントロール配置（左ラベル、右入力欄）
   - 統一された幅とスペーシング
   - レスポンシブデザイン

6. **統合とテスト**
   - Sceneへの配置
   - ServiceLocatorへの登録
   - 動作確認

7. **レビュー**
   - `/review-mvvm`コマンドで最終チェック
   - コーディング規約の確認
   - パフォーマンスチェック

## スタイルガイド参照

### USS標準パターン
```css
/* コントロール配置 */
.control {
    flex-direction: row;
    margin: 15px 2%;
}

/* ラベル */
.control > Label {
    min-width: 250px;
    flex-grow: 1;
    margin-right: 20px;
    color: rgb(255, 255, 255);
}

/* 入力欄 */
.control .input {
    width: 300px;
    flex-shrink: 0;
}
```

## 成果物チェックリスト

- [ ] ViewModelが`ViewModelBase`を継承
- [ ] Viewが`UIToolkitViewBase<T>`を継承
- [ ] UXMLにすべての要素がname属性付きで定義
- [ ] USSでスタイルが適切に適用
- [ ] イベントハンドラが正しく登録/解除
- [ ] プロパティ変更通知が実装
- [ ] Disposeパターンが実装
- [ ] 全コメントが日本語
- [ ] CLAUDE.mdの規約に準拠

このスキルを使用する際は、段階的に実装を進め、各ステップで確認を取りながら進めてください。
