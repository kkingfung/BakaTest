# UI Toolkit Design Skill

このスキルは、プロジェクトのスタイルガイドに従ったUI Toolkit画面を設計・実装します。

## プロジェクトスタイルガイド

### レイアウト原則

1. **コントロール配置：左ラベル、右入力欄**
```xml
<ui:VisualElement class="control">
    <ui:Label text="Label Text" />
    <ui:TextField class="input" />
</ui:VisualElement>
```

2. **統一された幅とスペーシング**
- ラベル最小幅：`250px`
- 入力欄幅：`300px`
- ラベルと入力欄の間隔：`20px`
- 上下マージン：`15px`
- 左右マージン：`2%`

3. **USS標準スタイル**
```css
.control {
    flex-direction: row;
    margin: 15px 2%;
}

.control > Label {
    min-width: 250px;
    flex-grow: 1;
    margin-right: 20px;
    color: rgb(255, 255, 255);
}

.control .input {
    width: 300px;
    flex-shrink: 0;
}
```

## 実装手順

### 1. 画面設計
- ワイヤーフレーム作成
- 必要な要素リストアップ
- レイアウト構造決定
- インタラクション定義

### 2. UXML作成
```xml
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <Style src="path/to/StyleSheet.uss" />
    
    <ui:VisualElement name="Root" class="root">
        <!-- Header -->
        <ui:VisualElement class="header">
            <ui:Label text="Title" class="title" />
        </ui:VisualElement>
        
        <!-- Content -->
        <ui:VisualElement class="content">
            <!-- コントロール -->
            <ui:VisualElement class="control">
                <ui:Label text="Label:" />
                <ui:TextField name="InputField" class="input" />
            </ui:VisualElement>
        </ui:VisualElement>
        
        <!-- Footer -->
        <ui:VisualElement class="footer">
            <ui:Button name="CancelButton" text="Cancel" />
            <ui:Button name="ConfirmButton" text="Confirm" />
        </ui:VisualElement>
    </ui:VisualElement>
</ui:UXML>
```

### 3. USS作成

#### 基本構造
```css
.root {
    flex-grow: 1;
    background-color: rgb(30, 30, 30);
}

.header {
    padding: 20px;
    background-color: rgb(50, 50, 50);
    border-bottom-width: 2px;
    border-color: rgb(70, 70, 70);
}

.content {
    flex-grow: 1;
    padding: 20px;
}

.footer {
    flex-direction: row;
    justify-content: flex-end;
    padding: 20px;
    background-color: rgb(40, 40, 40);
}
```

#### ボタンスタイル
```css
Button {
    min-width: 100px;
    height: 40px;
    margin: 5px;
    background-color: rgb(60, 120, 180);
    color: rgb(255, 255, 255);
    border-radius: 5px;
    font-size: 14px;
    -unity-font-style: bold;
}

Button:hover {
    background-color: rgb(80, 140, 200);
}

Button:active {
    background-color: rgb(40, 100, 160);
}
```

#### Arrow Buttonパターン
```css
.arrow-control {
    flex-direction: row;
    align-items: center;
    margin: 15px 2%;
}

.arrow-control-label {
    min-width: 250px;
    flex-grow: 1;
    margin-right: 20px;
    color: rgb(255, 255, 255);
}

.arrow-button {
    width: 40px;
    height: 40px;
    font-size: 18px;
}

.arrow-value {
    min-width: 100px;
    text-align: center;
    margin: 0 10px;
}
```

### 4. フォント設定

プロジェクトで利用可能なフォント（FlatSkin）：
- Roboto-Black
- Roboto-Medium
- Roboto-Bold

```css
.title {
    -unity-font: url('project://database/Assets/FlatSkin/Fonts/Roboto-Bold.ttf');
    font-size: 24px;
}

Label {
    -unity-font: url('project://database/Assets/FlatSkin/Fonts/Roboto-Medium.ttf');
    font-size: 14px;
}
```

### 5. レスポンシブデザイン

```css
/* 小さい画面対応 */
@media (max-width: 800px) {
    .control {
        flex-direction: column;
    }
    
    .control > Label {
        margin-bottom: 10px;
    }
    
    .control .input {
        width: 100%;
    }
}
```

## よく使うUIパターン

### 1. ドロップダウンコントロール
```xml
<ui:VisualElement class="control">
    <ui:Label text="Subject:" />
    <ui:DropdownField name="SubjectDropdown" class="input" />
</ui:VisualElement>
```

### 2. トグルコントロール
```xml
<ui:VisualElement class="control">
    <ui:Label text="Enable:" />
    <ui:Toggle name="EnableToggle" class="input" />
</ui:VisualElement>
```

### 3. Arrow Buttonコントロール
```xml
<ui:VisualElement class="arrow-control">
    <ui:Label text="Value:" class="arrow-control-label" />
    <ui:Button text="&lt;" name="DecrementButton" class="arrow-button" />
    <ui:Label text="0" name="ValueLabel" class="arrow-value" />
    <ui:Button text="&gt;" name="IncrementButton" class="arrow-button" />
</ui:VisualElement>
```

### 4. ラジオボタングループ
```xml
<ui:VisualElement class="control">
    <ui:Label text="Gender:" />
    <ui:RadioButtonGroup name="GenderRadio" class="input" />
</ui:VisualElement>
```

## チェックリスト

- [ ] UXMLの構造が論理的
- [ ] すべての要素にname属性
- [ ] USSスタイルシートが読み込まれている
- [ ] 統一されたスペーシング
- [ ] レスポンシブ対応
- [ ] フォントが適用されている
- [ ] ボタンにホバー効果
- [ ] アクセシビリティ考慮

## デバッグのヒント

1. **UI Debugger使用**: Window > UI Toolkit > Debugger
2. **Hierarchy確認**: 要素の親子関係
3. **Computed Styles確認**: 実際に適用されているスタイル
4. **Inline Style削除**: UXMLにstyle属性を書かない（USSで管理）

このスキルを使用する際は、まず基本構造を作り、徐々にスタイルを洗練させていってください。
