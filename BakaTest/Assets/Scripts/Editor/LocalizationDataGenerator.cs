#nullable enable
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using BakaTest.Data.Localization;

namespace BakaTest.Editor
{
    /// <summary>
    /// ローカライゼーションデータを生成するエディターツール
    /// </summary>
    public static class LocalizationDataGenerator
    {
        private const string ResourcesPath = "Assets/Resources/Data/Localization";

        [MenuItem("BakaTest/Generators/Create Localization Data")]
        public static void GenerateAllLocalizationData()
        {
            Debug.Log("[LocalizationDataGenerator] Starting localization data generation...");

            // Resourcesフォルダを作成
            EnsureDirectoryExists(ResourcesPath);

            int createdCount = 0;

            // 各カテゴリのローカライゼーションデータを生成
            foreach (LocalizationCategory category in Enum.GetValues(typeof(LocalizationCategory)))
            {
                if (CreateLocalizationDataForCategory(category))
                {
                    createdCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[LocalizationDataGenerator] ✓ Created {createdCount} localization data assets");
            EditorUtility.DisplayDialog("Localization Data Generator", 
                $"Successfully created {createdCount} localization data assets in:\n{ResourcesPath}", "OK");
        }

        private static bool CreateLocalizationDataForCategory(LocalizationCategory category)
        {
            string fileName = $"LocalizationData_{category}";
            string assetPath = $"{ResourcesPath}/{fileName}.asset";

            // 既存のアセットがある場合はスキップ
            var existing = AssetDatabase.LoadAssetAtPath<LocalizationData>(assetPath);
            if (existing != null)
            {
                Debug.Log($"[LocalizationDataGenerator] Skipping {category} (already exists)");
                return false;
            }

            // 新しいLocalizationDataを作成
            var localizationData = ScriptableObject.CreateInstance<LocalizationData>();

            // カテゴリを設定（リフレクションで）
            var categoryField = typeof(LocalizationData).GetField("_category", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            categoryField?.SetValue(localizationData, category);

            // カテゴリごとのサンプル文字列を追加
            PopulateLocalizationStrings(localizationData, category);

            // アセットを作成
            AssetDatabase.CreateAsset(localizationData, assetPath);
            Debug.Log($"[LocalizationDataGenerator] Created {fileName}.asset");

            return true;
        }

        private static void PopulateLocalizationStrings(LocalizationData data, LocalizationCategory category)
        {
            var stringsField = typeof(LocalizationData).GetField("_strings", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (stringsField == null) return;

            var strings = stringsField.GetValue(data) as System.Collections.Generic.List<LocalizedString>;
            if (strings == null) return;

            // カテゴリごとのサンプル文字列を追加
            switch (category)
            {
                case LocalizationCategory.Common:
                    AddString(strings, "btn_ok", "OK", "OK");
                    AddString(strings, "btn_cancel", "Cancel", "キャンセル");
                    AddString(strings, "btn_back", "Back", "戻る");
                    AddString(strings, "btn_next", "Next", "次へ");
                    AddString(strings, "btn_confirm", "Confirm", "確認");
                    AddString(strings, "btn_close", "Close", "閉じる");
                    AddString(strings, "loading", "Loading...", "読み込み中...");
                    AddString(strings, "saving", "Saving...", "保存中...");
                    break;

                case LocalizationCategory.MainMenu:
                    AddString(strings, "title", "BakaTest: Academic Battle Arena", "バカテスト：学力バトルアリーナ");
                    AddString(strings, "btn_take_test", "Take Test", "テストを受ける");
                    AddString(strings, "btn_battle", "Battle", "バトル");
                    AddString(strings, "btn_champions", "Champions", "チャンピオン");
                    AddString(strings, "btn_ranking", "Rankings", "ランキング");
                    AddString(strings, "btn_settings", "Settings", "設定");
                    AddString(strings, "btn_quit", "Quit", "終了");
                    break;

                case LocalizationCategory.TestSelection:
                    AddString(strings, "title", "Select Test", "テスト選択");
                    AddString(strings, "subject_label", "Subject:", "教科：");
                    AddString(strings, "difficulty_label", "Difficulty:", "難易度：");
                    AddString(strings, "subject_math", "Mathematics", "数学");
                    AddString(strings, "subject_science", "Science", "理科");
                    AddString(strings, "subject_english", "English", "英語");
                    AddString(strings, "subject_history", "History", "歴史");
                    AddString(strings, "difficulty_elementary", "Elementary School", "小学校");
                    AddString(strings, "difficulty_middleschool", "Middle School", "中学校");
                    AddString(strings, "difficulty_highschool", "High School", "高校");
                    AddString(strings, "difficulty_university", "University", "大学");
                    AddString(strings, "btn_start_test", "Start Test", "テスト開始");
                    AddString(strings, "remaining_points", "Remaining Daily Points: {0}", "残り獲得可能ポイント：{0}");
                    break;

                case LocalizationCategory.TestTaking:
                    AddString(strings, "time_remaining", "Time Remaining: {0:00}:{1:00}", "残り時間：{0:00}:{1:00}");
                    AddString(strings, "question_number", "Question {0}/{1}", "問題 {0}/{1}");
                    AddString(strings, "btn_submit", "Submit Test", "テストを提出");
                    AddString(strings, "btn_previous", "Previous", "前へ");
                    AddString(strings, "btn_next", "Next", "次へ");
                    AddString(strings, "answered", "Answered: {0}/{1}", "回答済み：{0}/{1}");
                    AddString(strings, "submit_confirm", "Submit your test? You cannot change answers after submission.", 
                        "テストを提出しますか？提出後は回答を変更できません。");
                    break;

                case LocalizationCategory.ChampionSelection:
                    AddString(strings, "title", "Select Champion", "チャンピオン選択");
                    AddString(strings, "owned_champions", "Owned Champions", "所有チャンピオン");
                    AddString(strings, "free_rotation", "Free Rotation", "無料ローテーション");
                    AddString(strings, "champion_stats", "Stats", "ステータス");
                    AddString(strings, "champion_abilities", "Abilities", "アビリティ");
                    AddString(strings, "stat_hp", "HP:", "HP：");
                    AddString(strings, "stat_attack", "Attack:", "攻撃力：");
                    AddString(strings, "stat_defense", "Defense:", "防御力：");
                    AddString(strings, "stat_speed", "Speed:", "速度：");
                    AddString(strings, "btn_select", "Select", "選択");
                    AddString(strings, "btn_unlock", "Unlock for {0} coins", "{0}コインで解放");
                    break;

                case LocalizationCategory.PointAllocation:
                    AddString(strings, "title", "Allocate Points", "ポイント配分");
                    AddString(strings, "available_points", "Available Points", "使用可能ポイント");
                    AddString(strings, "math_points", "Math Points:", "数学ポイント：");
                    AddString(strings, "science_points", "Science Points:", "理科ポイント：");
                    AddString(strings, "english_points", "English Points:", "英語ポイント：");
                    AddString(strings, "history_points", "History Points:", "歴史ポイント：");
                    AddString(strings, "total_allocated", "Total Allocated: {0}", "配分済み：{0}");
                    AddString(strings, "predicted_stats", "Predicted Battle Stats", "予測バトルステータス");
                    AddString(strings, "btn_commit", "Commit & Start Battle", "確定してバトル開始");
                    AddString(strings, "warning_points_consumed", "Warning: Allocated points will be consumed regardless of battle outcome!", 
                        "警告：配分したポイントはバトルの勝敗に関わらず消費されます！");
                    break;

                case LocalizationCategory.Battle:
                    AddString(strings, "vs", "VS", "VS");
                    AddString(strings, "round", "Round {0}", "ラウンド {0}");
                    AddString(strings, "critical_hit", "Critical Hit!", "クリティカルヒット！");
                    AddString(strings, "dodge", "Dodged!", "回避！");
                    AddString(strings, "victory", "VICTORY!", "勝利！");
                    AddString(strings, "defeat", "DEFEAT", "敗北");
                    AddString(strings, "item_used", "{0} used {1}!", "{0}が{1}を使用した！");
                    AddString(strings, "ability_activated", "{0} activated!", "{0}が発動！");
                    break;

                case LocalizationCategory.Results:
                    AddString(strings, "title", "Battle Results", "バトル結果");
                    AddString(strings, "victory_text", "You Won!", "勝利！");
                    AddString(strings, "defeat_text", "You Lost", "敗北");
                    AddString(strings, "points_consumed", "Points Consumed", "消費ポイント");
                    AddString(strings, "coins_earned", "Battle Coins Earned: {0}", "獲得バトルコイン：{0}");
                    AddString(strings, "experience_gained", "Experience Gained: {0}", "獲得経験値：{0}");
                    AddString(strings, "btn_continue", "Continue", "続ける");
                    AddString(strings, "btn_rematch", "Rematch", "リマッチ");
                    break;

                case LocalizationCategory.Settings:
                    AddString(strings, "title", "Settings", "設定");
                    AddString(strings, "language_label", "Language:", "言語：");
                    AddString(strings, "volume_master", "Master Volume:", "マスターボリューム：");
                    AddString(strings, "volume_music", "Music Volume:", "音楽ボリューム：");
                    AddString(strings, "volume_sfx", "SFX Volume:", "効果音ボリューム：");
                    AddString(strings, "graphics_quality", "Graphics Quality:", "グラフィック品質：");
                    AddString(strings, "quality_low", "Low", "低");
                    AddString(strings, "quality_medium", "Medium", "中");
                    AddString(strings, "quality_high", "High", "高");
                    AddString(strings, "btn_apply", "Apply", "適用");
                    AddString(strings, "btn_reset", "Reset to Defaults", "デフォルトに戻す");
                    break;

                case LocalizationCategory.Champions:
                    AddString(strings, "role_tank", "Tank", "タンク");
                    AddString(strings, "role_dps", "DPS", "DPS");
                    AddString(strings, "role_support", "Support", "サポート");
                    AddString(strings, "role_mage", "Mage", "メイジ");
                    AddString(strings, "role_assassin", "Assassin", "アサシン");
                    AddString(strings, "element_fire", "Fire", "炎");
                    AddString(strings, "element_water", "Water", "水");
                    AddString(strings, "element_earth", "Earth", "土");
                    AddString(strings, "element_wind", "Wind", "風");
                    break;

                case LocalizationCategory.Items:
                    AddString(strings, "type_consumable", "Consumable", "消耗品");
                    AddString(strings, "type_buff", "Buff", "バフ");
                    AddString(strings, "type_debuff", "Debuff", "デバフ");
                    AddString(strings, "type_special", "Special", "特殊");
                    AddString(strings, "rarity_common", "Common", "コモン");
                    AddString(strings, "rarity_uncommon", "Uncommon", "アンコモン");
                    AddString(strings, "rarity_rare", "Rare", "レア");
                    AddString(strings, "rarity_epic", "Epic", "エピック");
                    AddString(strings, "rarity_legendary", "Legendary", "レジェンダリー");
                    break;

                case LocalizationCategory.Errors:
                    AddString(strings, "connection_failed", "Connection to server failed", "サーバーへの接続に失敗しました");
                    AddString(strings, "insufficient_points", "Insufficient points", "ポイントが不足しています");
                    AddString(strings, "insufficient_coins", "Insufficient battle coins", "バトルコインが不足しています");
                    AddString(strings, "champion_not_owned", "You do not own this champion", "このチャンピオンを所有していません");
                    AddString(strings, "daily_limit_reached", "Daily point limit reached for this subject", "この教科の本日の獲得上限に達しました");
                    AddString(strings, "invalid_input", "Invalid input", "無効な入力です");
                    break;

                case LocalizationCategory.Tutorial:
                    AddString(strings, "welcome_title", "Welcome to BakaTest!", "バカテストへようこそ！");
                    AddString(strings, "welcome_message", "Study hard, battle smart!", "勉強して、賢く戦おう！");
                    AddString(strings, "test_tutorial", "Answer test questions to earn points", "テスト問題に答えてポイントを獲得しよう");
                    AddString(strings, "point_allocation_tutorial", "Allocate points to your champion's stats", "チャンピオンのステータスにポイントを配分しよう");
                    AddString(strings, "battle_tutorial", "Watch your champion battle automatically", "チャンピオンが自動でバトルするのを見守ろう");
                    AddString(strings, "btn_skip", "Skip Tutorial", "チュートリアルをスキップ");
                    break;

                case LocalizationCategory.Notifications:
                    AddString(strings, "test_completed", "Test Completed! Earned {0} points", "テスト完了！{0}ポイント獲得");
                    AddString(strings, "champion_unlocked", "New champion unlocked: {0}", "新しいチャンピオンを解放：{0}");
                    AddString(strings, "level_up", "Level Up! You are now level {0}", "レベルアップ！レベル{0}になりました");
                    AddString(strings, "achievement_unlocked", "Achievement Unlocked: {0}", "実績解除：{0}");
                    AddString(strings, "daily_bonus", "Daily Login Bonus: {0} coins", "デイリーログインボーナス：{0}コイン");
                    break;
            }
        }

        private static void AddString(System.Collections.Generic.List<LocalizedString> list, string key, string english, string japanese)
        {
            var localizedString = new LocalizedString(key);
            localizedString.SetText(Language.English, english);
            localizedString.SetText(Language.Japanese, japanese);
            list.Add(localizedString);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"[LocalizationDataGenerator] Created directory: {path}");
            }
        }
    }
}
