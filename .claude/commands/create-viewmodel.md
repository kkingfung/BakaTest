# Create ViewModel

ViewModelの名前を聞いて、MVVMパターンに従った新しいViewModelクラスを作成してください。

## 要件

1. **名前空間**: `CavalryFight.ViewModels`を使用
2. **基底クラス**: `ViewModelBase`を継承
3. **必須設定**: 
   - `#nullable enable`
   - `using CavalryFight.Core.MVVM;`
   - 必要に応じてサービスをコンストラクタでインジェクション
4. **パターン**:
   - プライベートフィールドは`_camelCase`
   - パブリックプロパティは`PascalCase`
   - プロパティは`SetProperty(ref _field, value)`を使用
   - コマンドは`RelayCommand<T>`を使用
5. **コメント**: すべてのコメントは日本語で記述
6. **保存場所**: `BakaTest/Assets/Scripts/ViewModels/`

## テンプレート構造

```csharp
#nullable enable
using System;
using CavalryFight.Core.MVVM;
using CavalryFight.Core.Commands;
using CavalryFight.Services.XXX;

namespace CavalryFight.ViewModels
{
    /// <summary>
    /// [説明]のViewModel
    /// </summary>
    public class [Name]ViewModel : ViewModelBase
    {
        // サービス
        private readonly IXXXService _xxxService;

        // プロパティ
        private string _example = string.Empty;
        public string Example
        {
            get => _example;
            set => SetProperty(ref _example, value);
        }

        // コマンド
        public RelayCommand<object?> ExampleCommand { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public [Name]ViewModel(IXXXService xxxService)
        {
            _xxxService = xxxService ?? throw new ArgumentNullException(nameof(xxxService));
            
            ExampleCommand = new RelayCommand<object?>(ExecuteExample, CanExecuteExample);
        }

        private void ExecuteExample(object? parameter)
        {
            // 実装
        }

        private bool CanExecuteExample(object? parameter)
        {
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // クリーンアップ
            }
            base.Dispose(disposing);
        }
    }
}
```

ユーザーに何のViewModelを作成するか聞いてから、適切な実装を生成してください。
