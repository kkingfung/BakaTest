# Create Service

サービスの名前を聞いて、インターフェースと実装クラスの両方を作成してください。

## 要件

1. **名前空間**: `CavalryFight.Services.XXX`を使用
2. **パターン**:
   - インターフェース名: `I[Name]Service`
   - 実装クラス名: `[Name]Service`
   - インターフェースから実装
3. **必須設定**:
   - `#nullable enable`
   - 必要に応じてイベント定義
   - スレッドセーフに注意
4. **コメント**: すべてのコメントは日本語で記述
5. **保存場所**: 
   - インターフェース: `BakaTest/Assets/Scripts/Services/XXX/I[Name]Service.cs`
   - 実装: `BakaTest/Assets/Scripts/Services/XXX/[Name]Service.cs`

## インターフェーステンプレート

```csharp
#nullable enable
using System;

namespace CavalryFight.Services.XXX
{
    /// <summary>
    /// [説明]サービスのインターフェース
    /// </summary>
    public interface I[Name]Service
    {
        /// <summary>
        /// [説明]
        /// </summary>
        void DoSomething();

        /// <summary>
        /// [説明]が発生したときに発火するイベント
        /// </summary>
        event Action? SomethingHappened;
    }
}
```

## 実装テンプレート

```csharp
#nullable enable
using System;
using UnityEngine;

namespace CavalryFight.Services.XXX
{
    /// <summary>
    /// [説明]サービスの実装
    /// </summary>
    public class [Name]Service : I[Name]Service
    {
        /// <summary>
        /// [説明]が発生したときに発火するイベント
        /// </summary>
        public event Action? SomethingHappened;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public [Name]Service()
        {
            // 初期化
        }

        /// <summary>
        /// [説明]
        /// </summary>
        public void DoSomething()
        {
            // 実装
            Debug.Log("[[Name]Service] DoSomething called.");
            SomethingHappened?.Invoke();
        }
    }
}
```

ユーザーに:
1. サービス名
2. サービスの目的
3. 必要なメソッド/プロパティ

を聞いてから、適切な実装を生成してください。
