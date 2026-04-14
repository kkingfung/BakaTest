# Create View

Viewの名前を聞いて、MVVMパターンに従った新しいViewクラスを作成してください。

## 要件

1. **名前空間**: `CavalryFight.Views`を使用
2. **基底クラス**: `UIToolkitViewBase<TViewModel>`を継承
3. **必須設定**:
   - `#nullable enable`
   - `[RequireComponent(typeof(UIDocument))]`属性
   - 対応するViewModelを型パラメータに指定
4. **パターン**:
   - UI要素フィールドは`_camelCase`でnullable型
   - `GetUIElements()`メソッドで要素取得
   - `RegisterEventHandlers()`で登録
   - `UnregisterEventHandlers()`で解除
   - `OnViewModelPropertyChanged()`でViewModel変更を監視
5. **コメント**: すべてのコメントは日本語で記述
6. **保存場所**: `BakaTest/Assets/Scripts/Views/`

## テンプレート構造

```csharp
#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using CavalryFight.Core.MVVM;
using CavalryFight.ViewModels;
using CavalryFight.Core.Services;
using System.ComponentModel;

namespace CavalryFight.Views
{
    /// <summary>
    /// [説明]のView
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class [Name]View : UIToolkitViewBase<[Name]ViewModel>
    {
        // UI要素
        private Button? _exampleButton;
        private Label? _exampleLabel;

        protected override void Awake()
        {
            base.Awake();

            // サービス取得
            var service = ServiceLocator.Instance.Get<IXXXService>();
            
            // ViewModel作成
            SetViewModel(new [Name]ViewModel(service));
        }

        protected override void OnRootVisualElementReady(VisualElement root)
        {
            base.OnRootVisualElementReady(root);

            GetUIElements();
            UpdateUIFromViewModel();
            RegisterEventHandlers();
        }

        protected override void BindViewModel([Name]ViewModel viewModel)
        {
            base.BindViewModel(viewModel);
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void UnbindViewModel()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }
            UnregisterEventHandlers();
            base.UnbindViewModel();
        }

        private void GetUIElements()
        {
            _exampleButton = Q<Button>("ExampleButton");
            _exampleLabel = Q<Label>("ExampleLabel");
        }

        private void UpdateUIFromViewModel()
        {
            if (ViewModel == null) return;
            
            if (_exampleLabel != null)
            {
                _exampleLabel.text = ViewModel.Example;
            }
        }

        private void RegisterEventHandlers()
        {
            if (_exampleButton != null)
            {
                _exampleButton.clicked += OnExampleButtonClicked;
            }
        }

        private void UnregisterEventHandlers()
        {
            if (_exampleButton != null)
            {
                _exampleButton.clicked -= OnExampleButtonClicked;
            }
        }

        private void OnExampleButtonClicked()
        {
            ViewModel?.ExampleCommand.Execute(null);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Example))
            {
                UpdateUIFromViewModel();
            }
        }
    }
}
```

ユーザーに何のViewを作成するか聞いてから、適切な実装を生成してください。
