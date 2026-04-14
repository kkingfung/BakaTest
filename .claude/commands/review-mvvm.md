# Review MVVM Implementation

指定されたView/ViewModelのペアをレビューし、MVVMパターンとプロジェクトのコーディング規約に従っているかチェックしてください。

## チェック項目

### ViewModelのチェック
- [ ] `ViewModelBase`を正しく継承しているか
- [ ] `#nullable enable`が設定されているか
- [ ] フィールド名が`_camelCase`か
- [ ] プロパティ名が`PascalCase`か
- [ ] プロパティで`SetProperty(ref _field, value)`を使用しているか
- [ ] コマンドが`RelayCommand<T>`を使用しているか
- [ ] サービスの依存性注入が適切か
- [ ] `Dispose`パターンが正しく実装されているか
- [ ] コメントが日本語で記述されているか
- [ ] ビジネスロジックがViewModelに含まれているか（Viewに書かれていないか）

### Viewのチェック
- [ ] `UIToolkitViewBase<TViewModel>`を正しく継承しているか
- [ ] `[RequireComponent(typeof(UIDocument))]`属性があるか
- [ ] `#nullable enable`が設定されているか
- [ ] UI要素フィールドがnullable型(`Button?`等)か
- [ ] `GetUIElements()`で要素を取得しているか
- [ ] `RegisterEventHandlers()`と`UnregisterEventHandlers()`が対になっているか
- [ ] `BindViewModel()`と`UnbindViewModel()`でイベント購読を管理しているか
- [ ] UIロジックのみでビジネスロジックを含まないか
- [ ] コメントが日本語で記述されているか

### MVVM分離のチェック
- [ ] ViewがViewModelのみを参照しているか（サービス直接参照していないか）
- [ ] ViewModelがUnityEngine型を参照していないか
- [ ] データバインディングが適切に実装されているか
- [ ] イベントハンドラが正しくクリーンアップされているか

## レビュー出力形式

### 良い点
- [良かった実装をリストアップ]

### 改善点
- [問題点と修正方法をリストアップ]

### 推奨される変更
```csharp
// 修正前
[問題のあるコード]

// 修正後
[推奨されるコード]
```

ユーザーにレビュー対象のファイルパスを聞いてから、詳細なレビューを実施してください。
