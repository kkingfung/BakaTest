# Battle System Implementation Skill

このスキルは、ゲーム企画書に基づいたバトルシステムコンポーネントを実装します。

## バトルシステムの要件

ゲーム企画書より：
- オートバトル（手動アイテム使用可能）
- チャンピオンアビリティ（パッシブ + アクティブ1つ）
- RNG要素（クリティカル、回避、アビリティ発動率）
- 配置/陣形システム
- ロール・属性相性
- 約1分のバトル時間

## 実装コンポーネント

### 1. BattleManager - バトル管理
```csharp
namespace CavalryFight.Battle
{
    /// <summary>
    /// バトル全体を管理するマネージャー
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        // チーム管理
        // ターン制御
        // 勝敗判定
        // アイテム使用
    }
}
```

### 2. BattleUnit - バトル中のユニット
```csharp
namespace CavalryFight.Battle
{
    /// <summary>
    /// バトル中のユニット（チャンピオン）
    /// </summary>
    public class BattleUnit
    {
        // 現在ステータス（HP, ATK, DEF, SPD）
        // バフ/デバフ管理
        // スキルクールダウン
        // AI行動ロジック
    }
}
```

### 3. BattleAction - 行動システム
```csharp
namespace CavalryFight.Battle
{
    /// <summary>
    /// バトル中の行動（攻撃、スキル、アイテム）
    /// </summary>
    public abstract class BattleAction
    {
        public abstract void Execute(BattleUnit source, BattleUnit target);
    }

    public class AttackAction : BattleAction { }
    public class SkillAction : BattleAction { }
    public class ItemAction : BattleAction { }
}
```

### 4. DamageCalculator - ダメージ計算
```csharp
namespace CavalryFight.Battle
{
    /// <summary>
    /// ダメージ計算とRNG処理
    /// </summary>
    public static class DamageCalculator
    {
        // 基本ダメージ計算
        // クリティカル判定
        // 回避判定
        // 属性相性ボーナス
        // ロール相性ボーナス
    }
}
```

### 5. BattleUI - バトルUI
```csharp
namespace CavalryFight.Battle.UI
{
    /// <summary>
    /// バトルUIの表示制御
    /// </summary>
    public class BattleUIController : MonoBehaviour
    {
        // HPバー更新
        // スキルクールダウン表示
        // アイテムボタン
        // ダメージ表示
        // 勝敗演出
    }
}
```

## 実装手順

1. **データ構造定義**
   - BattleStats（戦闘中ステータス）
   - BattleConfig（バトル設定）
   - BattleResult（結果データ）

2. **コア計算ロジック**
   - DamageCalculator実装
   - RNGシステム
   - ステータス計算

3. **ユニット管理**
   - BattleUnit実装
   - バフ/デバフシステム
   - スキルシステム

4. **バトルフロー**
   - BattleManager実装
   - ターン制御
   - 勝敗判定

5. **AI行動**
   - オートバトルAI
   - ターゲット選択
   - スキル使用判断

6. **UI統合**
   - UI Toolkitでバトル画面作成
   - アニメーション
   - エフェクト

7. **アイテムシステム**
   - 手動使用機能
   - アイテム効果適用
   - クールダウン管理

## テスト項目

- [ ] 1v1バトルが正常動作
- [ ] ダメージ計算が正しい
- [ ] クリティカル/回避が発動
- [ ] スキルが正しく発動
- [ ] アイテムが手動で使用可能
- [ ] 勝敗判定が正確
- [ ] UIが正しく更新
- [ ] パフォーマンスが60FPS維持

## 注意事項

- すべての計算はサーバーサイドで検証する前提で設計
- デバッグログを充実させる
- テストモードを用意する
- RNG seedを記録してリプレイ可能にする

このスキルを使用する際は、まずデータ構造とコア計算ロジックを固めてから、段階的に機能を追加していってください。
