#nullable enable
using UnityEditor;
using UnityEngine;
using BakaTest.Core.Services;
using BakaTest.Services.Player;
using BakaTest.Services.Champions;
using BakaTest.Services.Battle;
using BakaTest.Services.AI;
using BakaTest.Services.Inventory;
using BakaTest.Services.Settings;
using BakaTest.Services.Questions;
using BakaTest.Services.Audio;
using BakaTest.UI.LoadingScreen;
using BakaTest.Data.Tests;
using BakaTest.Data.Battle;
using BakaTest.Data.Champions;
using BakaTest.Data.Settings;

namespace BakaTest.Editor
{
    /// <summary>
    /// デバッグ用ツール群
    /// </summary>
    public static class DebugTools
    {
        [MenuItem("BakaTest/Debug/Add 1000 Points (All Subjects)")]
        public static void AddPointsAllSubjects()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            var playerService = ServiceLocator.Instance.Get<IPlayerDataService>();
            if (playerService == null)
            {
                Debug.LogError("[DebugTools] PlayerDataService not found!");
                return;
            }

            playerService.AddPoints(Subject.Math, 1000);
            playerService.AddPoints(Subject.Science, 1000);
            playerService.AddPoints(Subject.English, 1000);
            playerService.AddPoints(Subject.History, 1000);

            Debug.Log("[DebugTools] Added 1000 points to all subjects!");
            EditorUtility.DisplayDialog("Debug Tool", "Added 1000 points to all subjects!", "OK");
        }

        [MenuItem("BakaTest/Debug/Add 10000 Battle Coins")]
        public static void AddBattleCoins()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            var playerService = ServiceLocator.Instance.Get<IPlayerDataService>();
            if (playerService == null)
            {
                Debug.LogError("[DebugTools] PlayerDataService not found!");
                return;
            }

            playerService.AddCoins(10000);

            Debug.Log("[DebugTools] Added 10000 battle coins!");
            EditorUtility.DisplayDialog("Debug Tool", "Added 10000 battle coins!", "OK");
        }

        [MenuItem("BakaTest/Debug/Unlock All Champions")]
        public static void UnlockAllChampions()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            var championService = ServiceLocator.Instance.Get<IChampionService>();
            if (championService == null)
            {
                Debug.LogError("[DebugTools] ChampionService not found!");
                return;
            }

            var allChampions = championService.AllChampions;
            int unlocked = 0;

            foreach (var champion in allChampions)
            {
                if (!championService.IsOwned(champion.championId))
                {
                    championService.UnlockChampion(champion.championId);
                    unlocked++;
                }
            }

            Debug.Log($"[DebugTools] Unlocked {unlocked} champions!");
            EditorUtility.DisplayDialog("Debug Tool", $"Unlocked {unlocked} champions!", "OK");
        }

        [MenuItem("BakaTest/Debug/Reset Player Data")]
        public static void ResetPlayerData()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Player Data",
                "This will reset all points and coins to 0. Are you sure?",
                "Yes, Reset",
                "Cancel"
            );

            if (!confirmed) return;

            var playerService = ServiceLocator.Instance.Get<IPlayerDataService>();
            if (playerService == null)
            {
                Debug.LogError("[DebugTools] PlayerDataService not found!");
                return;
            }

            // スポイントをすべて消費
            int mathPoints = playerService.GetPoints(Subject.Math);
            int sciencePoints = playerService.GetPoints(Subject.Science);
            int englishPoints = playerService.GetPoints(Subject.English);
            int historyPoints = playerService.GetPoints(Subject.History);

            playerService.SpendPoints(new System.Collections.Generic.Dictionary<Subject, int>
            {
                { Subject.Math, mathPoints },
                { Subject.Science, sciencePoints },
                { Subject.English, englishPoints },
                { Subject.History, historyPoints }
            });

            // コインを消費
            playerService.SpendCoins(playerService.BattleCoins);

            Debug.Log("[DebugTools] Player data reset!");
            EditorUtility.DisplayDialog("Debug Tool", "Player data has been reset!", "OK");
        }

        [MenuItem("BakaTest/Debug/Quick Test Battle")]
        public static void QuickTestBattle()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            var battleService = ServiceLocator.Instance.Get<IBattleService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();

            if (battleService == null || championService == null)
            {
                Debug.LogError("[DebugTools] Required services not found!");
                return;
            }

            var allChampions = championService.AllChampions;
            if (allChampions.Count < 2)
            {
                Debug.LogError("[DebugTools] Need at least 2 champions to test battle!");
                EditorUtility.DisplayDialog("Debug Tool", "Need at least 2 champions! Run 'Generate Sample Champions' first.", "OK");
                return;
            }

            // ランダムに2体選択
            var champion1 = allChampions[Random.Range(0, allChampions.Count)];
            var champion2 = allChampions[Random.Range(0, allChampions.Count)];

            // バトルセットアップ
            var setup = new BattleSetup(
                BattleMode.Simulation,
                champion1,
                "Player",
                new System.Collections.Generic.Dictionary<Subject, int>
                {
                    { Subject.Math, 100 },
                    { Subject.Science, 100 },
                    { Subject.English, 100 },
                    { Subject.History, 100 }
                },
                champion2,
                "CPU",
                new System.Collections.Generic.Dictionary<Subject, int>
                {
                    { Subject.Math, 100 },
                    { Subject.Science, 100 },
                    { Subject.English, 100 },
                    { Subject.History, 100 }
                }
            );

            battleService.StartBattle(setup);
            var result = battleService.SimulateBattle();

            if (result != null)
            {
                string message = $"Battle Complete!\n\n" +
                    $"Winner: {result.Winner?.ChampionData.championName ?? "None"}\n" +
                    $"Loser: {result.Loser?.ChampionData.championName ?? "None"}\n" +
                    $"Turns: {result.TurnCount}\n" +
                    $"Duration: {result.Duration.TotalSeconds:F2} seconds";

                Debug.Log($"[DebugTools] {message}");
                EditorUtility.DisplayDialog("Battle Result", message, "OK");
            }
        }

        [MenuItem("BakaTest/Debug/Log Service Status")]
        public static void LogServiceStatus()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[DebugTools] This tool only works in Play mode!");
                EditorUtility.DisplayDialog("Debug Tool", "Please enter Play mode first!", "OK");
                return;
            }

            Debug.Log("=== Service Status ===");

            var playerService = ServiceLocator.Instance.Get<IPlayerDataService>();
            if (playerService != null)
            {
                Debug.Log($"PlayerDataService: OK");
                Debug.Log($"  Math Points: {playerService.GetPoints(Subject.Math)}");
                Debug.Log($"  Science Points: {playerService.GetPoints(Subject.Science)}");
                Debug.Log($"  English Points: {playerService.GetPoints(Subject.English)}");
                Debug.Log($"  History Points: {playerService.GetPoints(Subject.History)}");
                Debug.Log($"  Battle Coins: {playerService.BattleCoins}");
            }
            else
            {
                Debug.LogError("PlayerDataService: NOT FOUND");
            }

            var championService = ServiceLocator.Instance.Get<IChampionService>();
            if (championService != null)
            {
                Debug.Log($"ChampionService: OK");
                Debug.Log($"  Total Champions: {championService.AllChampions.Count}");
                Debug.Log($"  Owned Champions: {championService.OwnedChampions.Count}");
                Debug.Log($"  Free Rotation: {championService.FreeRotationChampions.Count}");
                Debug.Log($"  Available: {championService.AvailableChampions.Count}");
            }
            else
            {
                Debug.LogError("ChampionService: NOT FOUND");
            }

            var battleService = ServiceLocator.Instance.Get<IBattleService>();
            if (battleService != null)
            {
                Debug.Log($"BattleService: OK");
                Debug.Log($"  Battle In Progress: {battleService.IsBattleInProgress}");
            }
            else
            {
                Debug.LogError("BattleService: NOT FOUND");
            }

            Debug.Log("=== End Service Status ===");
        }

        [MenuItem("BakaTest/Debug/Set Turn Delay (Fast: 0.5s)", priority = 100)]
        public static void SetTurnDelayFast()
        {
            EditorPrefs.SetFloat("BakaTest_TurnDelay", 0.5f);
            Debug.Log("[DebugTools] Turn delay set to 0.5 seconds (Fast)");
            EditorUtility.DisplayDialog("Debug Tool", "Turn delay set to 0.5 seconds.\nRestart battle to apply.", "OK");
        }

        [MenuItem("BakaTest/Debug/Set Turn Delay (Normal: 1.5s)", priority = 101)]
        public static void SetTurnDelayNormal()
        {
            EditorPrefs.SetFloat("BakaTest_TurnDelay", 1.5f);
            Debug.Log("[DebugTools] Turn delay set to 1.5 seconds (Normal)");
            EditorUtility.DisplayDialog("Debug Tool", "Turn delay set to 1.5 seconds.\nRestart battle to apply.", "OK");
        }

        [MenuItem("BakaTest/Debug/Set Turn Delay (Slow: 3.0s)", priority = 102)]
        public static void SetTurnDelaySlow()
        {
            EditorPrefs.SetFloat("BakaTest_TurnDelay", 3.0f);
            Debug.Log("[DebugTools] Turn delay set to 3.0 seconds (Slow)");
            EditorUtility.DisplayDialog("Debug Tool", "Turn delay set to 3.0 seconds.\nRestart battle to apply.", "OK");
        }

        [MenuItem("BakaTest/Debug/Delete All Champion Assets", priority = 200)]
        public static void DeleteAllChampionAssets()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Delete All Champions",
                "This will DELETE all champion .asset files in Assets/Resources/Data/Champions/\n\nAre you sure?",
                "Yes, Delete All",
                "Cancel"
            );

            if (!confirmed) return;

            string path = "Assets/Resources/Data/Champions";
            if (!System.IO.Directory.Exists(path))
            {
                Debug.LogWarning("[DebugTools] Champions directory does not exist!");
                return;
            }

            string[] assetFiles = System.IO.Directory.GetFiles(path, "*.asset");
            int deleted = 0;

            foreach (string file in assetFiles)
            {
                AssetDatabase.DeleteAsset(file);
                deleted++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[DebugTools] Deleted {deleted} champion assets.");
            EditorUtility.DisplayDialog("Delete Complete", $"Deleted {deleted} champion assets.", "OK");
        }

        #region AI Opponent Debug Tools

        [MenuItem("BakaTest/Debug/AI/Test Easy AI")]
        public static void TestEasyAI()
        {
            TestAIOpponent(AIDifficulty.Easy);
        }

        [MenuItem("BakaTest/Debug/AI/Test Medium AI")]
        public static void TestMediumAI()
        {
            TestAIOpponent(AIDifficulty.Medium);
        }

        [MenuItem("BakaTest/Debug/AI/Test Hard AI")]
        public static void TestHardAI()
        {
            TestAIOpponent(AIDifficulty.Hard);
        }

        [MenuItem("BakaTest/Debug/AI/Test All Personalities")]
        public static void TestAllPersonalities()
        {
            var aiService = ServiceLocator.Instance.Get<IAIOpponentService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();

            if (aiService == null || championService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            var champions = championService.AllChampions;
            if (champions.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No champions found!", "OK");
                return;
            }

            var testChampion = champions[0];

            Debug.Log("=== Testing All AI Personalities ===");
            foreach (AIPersonality personality in System.Enum.GetValues(typeof(AIPersonality)))
            {
                Debug.Log($"\n--- {personality} Personality ---");
                var points = aiService.AllocatePoints(AIDifficulty.Medium, personality, testChampion);
                int total = 0;
                foreach (var kvp in points)
                {
                    Debug.Log($"  {kvp.Key}: {kvp.Value}");
                    total += kvp.Value;
                }
                Debug.Log($"  Total: {total}");
            }

            EditorUtility.DisplayDialog("Test Complete", "All AI personalities tested. Check console for results.", "OK");
        }

        private static void TestAIOpponent(AIDifficulty difficulty)
        {
            var aiService = ServiceLocator.Instance.Get<IAIOpponentService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();

            if (aiService == null || championService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            var champions = championService.AllChampions;
            if (champions.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No champions found!", "OK");
                return;
            }

            // AI名を生成
            string aiName = aiService.GenerateAIName(difficulty);

            // チャンピオンを選択
            var selectedChampion = aiService.SelectChampion(champions, difficulty);

            if (selectedChampion == null)
            {
                EditorUtility.DisplayDialog("Error", "Failed to select AI champion!", "OK");
                return;
            }

            // ポイント配分を生成（Tactical personality）
            var points = aiService.AllocatePoints(difficulty, AIPersonality.Tactical, selectedChampion);

            // 結果をログ出力
            Debug.Log($"=== AI Opponent Test ({difficulty}) ===");
            Debug.Log($"AI Name: {aiName}");
            Debug.Log($"Champion: {selectedChampion.championName}");
            Debug.Log($"Total Points: {aiService.GetTotalPointsForDifficulty(difficulty)}");
            Debug.Log("Point Allocation:");
            foreach (var kvp in points)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }

            string message = $"AI Name: {aiName}\n" +
                           $"Champion: {selectedChampion.championName}\n" +
                           $"Total Points: {aiService.GetTotalPointsForDifficulty(difficulty)}\n" +
                           $"Math: {points[Subject.Math]}\n" +
                           $"Science: {points[Subject.Science]}\n" +
                           $"English: {points[Subject.English]}\n" +
                           $"History: {points[Subject.History]}";

            EditorUtility.DisplayDialog($"{difficulty} AI Test", message, "OK");
        }

        [MenuItem("BakaTest/Debug/AI/Battle vs Easy AI")]
        public static void BattleVsEasyAI()
        {
            BattleVsAI(AIDifficulty.Easy);
        }

        [MenuItem("BakaTest/Debug/AI/Battle vs Medium AI")]
        public static void BattleVsMediumAI()
        {
            BattleVsAI(AIDifficulty.Medium);
        }

        [MenuItem("BakaTest/Debug/AI/Battle vs Hard AI")]
        public static void BattleVsHardAI()
        {
            BattleVsAI(AIDifficulty.Hard);
        }

        private static void BattleVsAI(AIDifficulty difficulty)
        {
            var aiService = ServiceLocator.Instance.Get<IAIOpponentService>();
            var championService = ServiceLocator.Instance.Get<IChampionService>();
            var battleService = ServiceLocator.Instance.Get<IBattleService>();

            if (aiService == null || championService == null || battleService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            var champions = championService.AllChampions;
            if (champions.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "No champions found!", "OK");
                return;
            }

            // プレイヤー設定（最初のチャンピオン、固定ポイント）
            var playerChampion = champions[0];
            var playerPoints = new System.Collections.Generic.Dictionary<Subject, int>
            {
                { Subject.Math, 500 },
                { Subject.Science, 500 },
                { Subject.English, 500 },
                { Subject.History, 500 }
            };

            // AI設定
            string aiName = aiService.GenerateAIName(difficulty);
            var aiChampion = aiService.SelectChampion(champions, difficulty);
            if (aiChampion == null) return;

            var aiPoints = aiService.AllocatePoints(difficulty, AIPersonality.Tactical, aiChampion);

            // バトル設定
            var setup = new BattleSetup(
                BattleMode.Simulation,
                playerChampion,
                "Player",
                playerPoints,
                aiChampion,
                aiName,
                aiPoints
            );

            // バトル実行
            battleService.StartBattle(setup);
            var result = battleService.SimulateBattle();

            if (result != null)
            {
                string winner = result.Winner?.PlayerName ?? "Draw";
                string message = $"Battle Result:\n" +
                               $"Winner: {winner}\n" +
                               $"Turns: {result.TurnCount}\n" +
                               $"Duration: {result.Duration:F2}s";

                Debug.Log($"[DebugTools] {message}");
                EditorUtility.DisplayDialog("Battle Complete", message, "OK");
            }
        }

        #endregion

        #region Inventory Debug Tools

        [MenuItem("BakaTest/Debug/Inventory/Add 1000 Coins")]
        public static void Add1000Coins()
        {
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();
            if (playerDataService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            playerDataService.AddCoins(1000);
            Debug.Log("[DebugTools] Added 1000 coins");
            EditorUtility.DisplayDialog("Coins Added", $"Added 1000 coins. Total: {playerDataService.BattleCoins}", "OK");
        }

        [MenuItem("BakaTest/Debug/Inventory/Buy Random Item")]
        public static void BuyRandomItem()
        {
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            if (inventoryService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            // Get a random item
            var allItems = Resources.LoadAll<BakaTest.Data.Items.ItemData>("Data/Items");
            if (allItems.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "No items found! Generate items first.", "OK");
                return;
            }

            var randomItem = allItems[UnityEngine.Random.Range(0, allItems.Length)];
            bool success = inventoryService.BuyItem(randomItem.itemId);

            if (success)
            {
                Debug.Log($"[DebugTools] Bought {randomItem.itemName}");
                EditorUtility.DisplayDialog("Item Purchased", $"Bought {randomItem.itemName} for {randomItem.buyPrice} coins", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Purchase Failed", $"Could not buy {randomItem.itemName}. Check coins or inventory.", "OK");
            }
        }

        [MenuItem("BakaTest/Debug/Inventory/Print Inventory")]
        public static void PrintInventory()
        {
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            if (inventoryService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            var ownedItems = inventoryService.OwnedItems;
            Debug.Log($"=== Inventory ({inventoryService.CurrentItemCount} items) ===");

            if (ownedItems.Count == 0)
            {
                Debug.Log("  (empty)");
                EditorUtility.DisplayDialog("Inventory", "Inventory is empty", "OK");
                return;
            }

            string message = $"Total Items: {inventoryService.CurrentItemCount}\n\n";
            foreach (var kvp in ownedItems)
            {
                var itemData = Resources.Load<BakaTest.Data.Items.ItemData>($"Data/Items/Item_{kvp.Key}");
                string itemName = itemData != null ? itemData.itemName : kvp.Key;
                Debug.Log($"  {itemName}: x{kvp.Value}");
                message += $"{itemName}: x{kvp.Value}\n";
            }

            EditorUtility.DisplayDialog("Inventory", message, "OK");
        }

        [MenuItem("BakaTest/Debug/Inventory/Clear Inventory")]
        public static void ClearInventory()
        {
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            if (inventoryService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm Clear", "Are you sure you want to clear the entire inventory?", "Clear", "Cancel"))
            {
                return;
            }

            inventoryService.ClearInventory();
            Debug.Log("[DebugTools] Inventory cleared");
            EditorUtility.DisplayDialog("Inventory Cleared", "All items have been removed from inventory", "OK");
        }

        [MenuItem("BakaTest/Debug/Inventory/Buy Starter Pack")]
        public static void BuyStarterPack()
        {
            var inventoryService = ServiceLocator.Instance.Get<IInventoryService>();
            var playerDataService = ServiceLocator.Instance.Get<IPlayerDataService>();

            if (inventoryService == null || playerDataService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            // Add coins for purchase
            playerDataService.AddCoins(2000);

            // Buy starter items
            inventoryService.BuyItem("potion_small", 5);
            inventoryService.BuyItem("attack_boost", 2);
            inventoryService.BuyItem("defense_boost", 2);

            Debug.Log("[DebugTools] Purchased starter pack");
            EditorUtility.DisplayDialog("Starter Pack", "Purchased:\n- 5x Small Potions\n- 2x Attack Boosts\n- 2x Defense Boosts", "OK");
        }

        #endregion

        #region Settings Debug Tools

        [MenuItem("BakaTest/Debug/Settings/Print Current Settings")]
        public static void PrintCurrentSettings()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            var settings = settingsService.CurrentSettings;
            Debug.Log("=== Current Settings ===");
            
            Debug.Log("\n--- Audio ---");
            Debug.Log($"Master Volume: {settings.audio.masterVolume:P0} (Muted: {settings.audio.masterMuted})");
            Debug.Log($"Music Volume: {settings.audio.musicVolume:P0} (Muted: {settings.audio.musicMuted})");
            Debug.Log($"SFX Volume: {settings.audio.sfxVolume:P0} (Muted: {settings.audio.sfxMuted})");
            Debug.Log($"Voice Volume: {settings.audio.voiceVolume:P0} (Muted: {settings.audio.voiceMuted})");
            Debug.Log($"UI Volume: {settings.audio.uiVolume:P0} (Muted: {settings.audio.uiMuted})");

            Debug.Log("\n--- Graphics ---");
            string[] qualityNames = { "Low", "Medium", "High", "Ultra" };
            Debug.Log($"Quality Level: {qualityNames[settings.graphics.qualityLevel]} ({settings.graphics.qualityLevel})");
            Debug.Log($"Resolution Scale: {settings.graphics.resolutionScale:P0}");
            Debug.Log($"Fullscreen: {settings.graphics.fullscreen}");
            Debug.Log($"VSync: {settings.graphics.vsync}");
            Debug.Log($"Target FPS: {settings.graphics.targetFrameRate}");
            Debug.Log($"Anti-Aliasing: {settings.graphics.antiAliasing}");
            Debug.Log($"Shadow Quality: {settings.graphics.shadowQuality}");
            Debug.Log($"Texture Quality: {settings.graphics.textureQuality}");
            Debug.Log($"Particle Effects: {settings.graphics.particleEffects}");
            Debug.Log($"Post Processing: {settings.graphics.postProcessing}");

            Debug.Log("\n--- Gameplay ---");
            Debug.Log($"Battle Speed: {settings.gameplay.battleSpeed}x");
            Debug.Log($"Auto Battle: {settings.gameplay.autoBattle}");
            Debug.Log($"Skip Cutscenes: {settings.gameplay.skipCutscenes}");
            Debug.Log($"Show Tutorials: {settings.gameplay.showTutorials}");
            Debug.Log($"Auto Advance Battle Result: {settings.gameplay.autoAdvanceBattleResult}");
            Debug.Log($"Auto Advance Delay: {settings.gameplay.autoAdvanceDelay}s");
            Debug.Log($"Show Damage Numbers: {settings.gameplay.showDamageNumbers}");
            Debug.Log($"Show Battle Log: {settings.gameplay.showBattleLog}");
            Debug.Log($"Vibration Enabled: {settings.gameplay.vibrationEnabled}");

            Debug.Log("\n--- Accessibility ---");
            Debug.Log($"Text Size: {settings.accessibility.textSize}");
            Debug.Log($"Color Blind Mode: {settings.accessibility.colorBlindMode}");
            Debug.Log($"High Contrast UI: {settings.accessibility.highContrastUI}");
            Debug.Log($"Reduce Motion: {settings.accessibility.reduceMotion}");
            Debug.Log($"Reduce Flashing: {settings.accessibility.reduceFlashing}");
            Debug.Log($"Button Hold Duration: {settings.accessibility.buttonHoldDuration}s");
            Debug.Log($"Show Auto-Save Notification: {settings.accessibility.showAutoSaveNotification}");

            string message = $"Audio: Master {settings.audio.masterVolume:P0}\n" +
                           $"Graphics: {qualityNames[settings.graphics.qualityLevel]}, {settings.graphics.targetFrameRate} FPS\n" +
                           $"Gameplay: Battle Speed {settings.gameplay.battleSpeed}x\n" +
                           $"Accessibility: Text {settings.accessibility.textSize}x\n\n" +
                           $"Check console for full details.";

            EditorUtility.DisplayDialog("Current Settings", message, "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Audio/Set Master Volume 50%")]
        public static void SetMasterVolume50()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetMasterVolume(0.5f);
            Debug.Log("[DebugTools] Master volume set to 50%");
            EditorUtility.DisplayDialog("Settings", "Master volume set to 50%", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Audio/Mute Master Volume")]
        public static void MuteMasterVolume()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            bool currentMute = settingsService.Audio.masterMuted;
            settingsService.SetMasterMute(!currentMute);
            Debug.Log($"[DebugTools] Master mute: {!currentMute}");
            string muteStatus = !currentMute ? "muted" : "unmuted";
            EditorUtility.DisplayDialog("Settings", $"Master audio {muteStatus}", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set Quality Low")]
        public static void SetQualityLow()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetQualityLevel(0);
            Debug.Log("[DebugTools] Quality set to Low");
            EditorUtility.DisplayDialog("Settings", "Graphics quality set to Low", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set Quality Medium")]
        public static void SetQualityMedium()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetQualityLevel(1);
            Debug.Log("[DebugTools] Quality set to Medium");
            EditorUtility.DisplayDialog("Settings", "Graphics quality set to Medium", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set Quality High")]
        public static void SetQualityHigh()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetQualityLevel(2);
            Debug.Log("[DebugTools] Quality set to High");
            EditorUtility.DisplayDialog("Settings", "Graphics quality set to High", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Toggle VSync")]
        public static void ToggleVSync()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            bool currentVSync = settingsService.Graphics.vsync;
            settingsService.SetVSync(!currentVSync);
            Debug.Log($"[DebugTools] VSync: {!currentVSync}");
            string vsyncStatus = !currentVSync ? "enabled" : "disabled";
            EditorUtility.DisplayDialog("Settings", $"VSync {vsyncStatus}", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set FPS 30")]
        public static void SetFPS30()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetTargetFrameRate(30);
            Debug.Log("[DebugTools] Target FPS set to 30");
            EditorUtility.DisplayDialog("Settings", "Target framerate set to 30 FPS", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set FPS 60")]
        public static void SetFPS60()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetTargetFrameRate(60);
            Debug.Log("[DebugTools] Target FPS set to 60");
            EditorUtility.DisplayDialog("Settings", "Target framerate set to 60 FPS", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Graphics/Set FPS 120")]
        public static void SetFPS120()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetTargetFrameRate(120);
            Debug.Log("[DebugTools] Target FPS set to 120");
            EditorUtility.DisplayDialog("Settings", "Target framerate set to 120 FPS", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Gameplay/Toggle Auto Battle")]
        public static void ToggleAutoBattle()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            bool currentAuto = settingsService.Gameplay.autoBattle;
            settingsService.SetAutoBattle(!currentAuto);
            Debug.Log($"[DebugTools] Auto Battle: {!currentAuto}");
            string autoStatus = !currentAuto ? "enabled" : "disabled";
            EditorUtility.DisplayDialog("Settings", $"Auto Battle {autoStatus}", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Gameplay/Set Battle Speed 0.5x")]
        public static void SetBattleSpeedSlow()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetBattleSpeed(0.5f);
            Debug.Log("[DebugTools] Battle speed set to 0.5x");
            EditorUtility.DisplayDialog("Settings", "Battle speed set to 0.5x (Slow)", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Gameplay/Set Battle Speed 1.0x")]
        public static void SetBattleSpeedNormal()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetBattleSpeed(1.0f);
            Debug.Log("[DebugTools] Battle speed set to 1.0x");
            EditorUtility.DisplayDialog("Settings", "Battle speed set to 1.0x (Normal)", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Gameplay/Set Battle Speed 2.0x")]
        public static void SetBattleSpeedFast()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            settingsService.SetBattleSpeed(2.0f);
            Debug.Log("[DebugTools] Battle speed set to 2.0x");
            EditorUtility.DisplayDialog("Settings", "Battle speed set to 2.0x (Fast)", "OK");
        }

        [MenuItem("BakaTest/Debug/Settings/Reset All Settings to Defaults")]
        public static void ResetAllSettings()
        {
            var settingsService = ServiceLocator.Instance.Get<ISettingsService>();
            if (settingsService == null)
            {
                EditorUtility.DisplayDialog("Error", "Services not initialized. Please enter play mode first.", "OK");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Reset Settings",
                "This will reset all settings to default values. Are you sure?",
                "Yes, Reset",
                "Cancel"
            );

            if (!confirmed) return;

            settingsService.ResetToDefaults();
            Debug.Log("[DebugTools] All settings reset to defaults");
            EditorUtility.DisplayDialog("Settings Reset", "All settings have been reset to default values.", "OK");
        }

        #endregion

        #region Question Bank Tools

        [MenuItem("BakaTest/Debug/QuestionBank/Print Question Bank Status")]
        public static void PrintQuestionBankStatus()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            Debug.Log("=== Question Bank Status ===");
            Debug.Log($"Total Questions: {questionBankService.GetTotalQuestionCount()}");
            Debug.Log($"AI Generation Enabled: {questionBankService.IsAIGenerationEnabled}");

            foreach (Subject subject in System.Enum.GetValues(typeof(Subject)))
            {
                foreach (DifficultyLevel difficulty in System.Enum.GetValues(typeof(DifficultyLevel)))
                {
                    int count = questionBankService.GetAvailableQuestionCount(subject, difficulty);
                    if (count > 0)
                    {
                        Debug.Log($"  {subject} - {difficulty}: {count} questions");
                    }
                }
            }

            EditorUtility.DisplayDialog("Question Bank",
                $"Total questions: {questionBankService.GetTotalQuestionCount()}\nCheck console for details.",
                "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Get Random Math Elementary Questions")]
        public static void GetRandomMathElementaryQuestions()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            var questions = questionBankService.GetRandomQuestions(Subject.Math, DifficultyLevel.Elementary, 5);

            Debug.Log($"=== Random Math Elementary Questions ({questions.Count}) ===");
            for (int i = 0; i < questions.Count; i++)
            {
                Debug.Log($"{i + 1}. {questions[i].questionText}");
                Debug.Log($"   Answer: {questions[i].choices[questions[i].correctAnswerIndex]}");
            }

            EditorUtility.DisplayDialog("Questions Retrieved",
                $"Retrieved {questions.Count} Math Elementary questions.\nCheck console for details.",
                "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Get Today's Test Pool (Science HighSchool)")]
        public static void GetTodaysTestPool()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            var questions = questionBankService.GetTodaysTestPool(Subject.Science, DifficultyLevel.HighSchool);

            Debug.Log($"=== Today's Test Pool - Science HighSchool ({questions.Count}) ===");
            for (int i = 0; i < questions.Count; i++)
            {
                Debug.Log($"{i + 1}. {questions[i]}");
            }

            EditorUtility.DisplayDialog("Test Pool Retrieved",
                $"Retrieved {questions.Count} Science HighSchool questions for today's test pool.\nCheck console for details.",
                "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Enable AI Generation")]
        public static void EnableAIGeneration()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            questionBankService.SetAIGenerationEnabled(true);
            Debug.Log("[DebugTools] AI question generation enabled");
            EditorUtility.DisplayDialog("AI Generation", "AI question generation has been enabled.", "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Disable AI Generation")]
        public static void DisableAIGeneration()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            questionBankService.SetAIGenerationEnabled(false);
            Debug.Log("[DebugTools] AI question generation disabled");
            EditorUtility.DisplayDialog("AI Generation", "AI question generation has been disabled.", "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Generate 10 AI Questions (English University)")]
        public static void GenerateAIQuestions()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            var questions = questionBankService.GenerateAIQuestions(Subject.English, DifficultyLevel.University, 10);

            Debug.Log($"=== Generated AI Questions - English University ({questions.Count}) ===");
            for (int i = 0; i < questions.Count; i++)
            {
                Debug.Log($"{i + 1}. {questions[i].questionText}");
                Debug.Log($"   Answer: {questions[i].choices[questions[i].correctAnswerIndex]}");
                Debug.Log($"   AI Generated: {questions[i].isAIGenerated}");
            }

            EditorUtility.DisplayDialog("AI Questions Generated",
                $"Generated {questions.Count} AI questions.\nNote: Current implementation uses placeholder questions.\nCheck console for details.",
                "OK");
        }

        [MenuItem("BakaTest/Debug/QuestionBank/Test All Subjects (Elementary Level)")]
        public static void TestAllSubjectsElementary()
        {
            var questionBankService = ServiceLocator.Instance.Get<IQuestionBankService>();
            if (questionBankService == null)
            {
                EditorUtility.DisplayDialog("Error", "QuestionBankService not initialized. Please enter play mode first.", "OK");
                return;
            }

            Debug.Log("=== Testing All Subjects - Elementary Level ===");
            int totalRetrieved = 0;

            foreach (Subject subject in System.Enum.GetValues(typeof(Subject)))
            {
                var questions = questionBankService.GetRandomQuestions(subject, DifficultyLevel.Elementary, 3);
                totalRetrieved += questions.Count;
                Debug.Log($"{subject}: {questions.Count} questions retrieved");

                foreach (var q in questions)
                {
                    Debug.Log($"  - {q.questionText.Substring(0, System.Math.Min(60, q.questionText.Length))}...");
                }
            }

            EditorUtility.DisplayDialog("Test Complete",
                $"Retrieved {totalRetrieved} total questions across all subjects.\nCheck console for details.",
                "OK");
        }

        #endregion

        #region Audio Tools

        [MenuItem("BakaTest/Debug/Audio/Print Audio Status")]
        public static void PrintAudioStatus()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            Debug.Log("=== Audio Service Status ===");
            Debug.Log($"Registered Audio Clips: {audioService.GetRegisteredAudioCount()}");
            Debug.Log($"Current Music: {audioService.CurrentMusicId ?? "None"}");
            Debug.Log($"Is Music Playing: {audioService.IsMusicPlaying}");
            Debug.Log($"Is Voice Playing: {audioService.IsVoicePlaying}");

            EditorUtility.DisplayDialog("Audio Status",
                $"Registered: {audioService.GetRegisteredAudioCount()} clips\n" +
                $"Music: {audioService.CurrentMusicId ?? "None"}\n" +
                $"Playing: {audioService.IsMusicPlaying}",
                "OK");
        }

        [MenuItem("BakaTest/Debug/Audio/Test UI Sounds/Button Click")]
        public static void TestButtonClick()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            audioService.PlayButtonClick();
            Debug.Log("[DebugTools] Played button click sound");
        }

        [MenuItem("BakaTest/Debug/Audio/Test UI Sounds/Button Hover")]
        public static void TestButtonHover()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            audioService.PlayButtonHover();
            Debug.Log("[DebugTools] Played button hover sound");
        }

        [MenuItem("BakaTest/Debug/Audio/Test UI Sounds/Success")]
        public static void TestSuccessSound()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            audioService.PlaySuccessSound();
            Debug.Log("[DebugTools] Played success sound");
        }

        [MenuItem("BakaTest/Debug/Audio/Test UI Sounds/Error")]
        public static void TestErrorSound()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            audioService.PlayErrorSound();
            Debug.Log("[DebugTools] Played error sound");
        }

        [MenuItem("BakaTest/Debug/Audio/Stop All SFX")]
        public static void StopAllSFX()
        {
            var audioService = ServiceLocator.Instance.Get<IAudioService>();
            if (audioService == null)
            {
                EditorUtility.DisplayDialog("Error", "AudioService not initialized. Please enter play mode first.", "OK");
                return;
            }

            audioService.StopAllSFX();
            Debug.Log("[DebugTools] Stopped all SFX");
            EditorUtility.DisplayDialog("Audio", "All SFX stopped.", "OK");
        }

        #endregion

        #region Loading Screen Debug Tools

        [MenuItem("BakaTest/Debug/LoadingScreen/Show Loading Screen")]
        public static void ShowLoadingScreen()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.Show("Loading...");
            Debug.Log("[DebugTools] Loading screen shown");
            EditorUtility.DisplayDialog("Loading Screen", "Loading screen is now visible.", "OK");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Show Loading Screen", true)]
        public static bool ShowLoadingScreenValidate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Hide Loading Screen")]
        public static void HideLoadingScreen()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.Hide(fadeOut: true);
            Debug.Log("[DebugTools] Loading screen hidden");
            EditorUtility.DisplayDialog("Loading Screen", "Loading screen is now hidden.", "OK");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Hide Loading Screen", true)]
        public static bool HideLoadingScreenValidate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 25%")]
        public static void SetLoadingProgress25()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.SetProgress(0.25f);
            Debug.Log("[DebugTools] Loading progress set to 25%");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 25%", true)]
        public static bool SetLoadingProgress25Validate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 50%")]
        public static void SetLoadingProgress50()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.SetProgress(0.5f);
            Debug.Log("[DebugTools] Loading progress set to 50%");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 50%", true)]
        public static bool SetLoadingProgress50Validate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 75%")]
        public static void SetLoadingProgress75()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.SetProgress(0.75f);
            Debug.Log("[DebugTools] Loading progress set to 75%");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 75%", true)]
        public static bool SetLoadingProgress75Validate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 100%")]
        public static void SetLoadingProgress100()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.SetProgress(1.0f);
            Debug.Log("[DebugTools] Loading progress set to 100%");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Set Progress 100%", true)]
        public static bool SetLoadingProgress100Validate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Update Message")]
        public static void UpdateLoadingMessage()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            loadingScreenService.SetMessage("Loading assets... Please wait.");
            Debug.Log("[DebugTools] Loading message updated");
            EditorUtility.DisplayDialog("Loading Screen", "Message updated to: 'Loading assets... Please wait.'", "OK");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Update Message", true)]
        public static bool UpdateLoadingMessageValidate()
        {
            return Application.isPlaying;
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Print Loading Status")]
        public static void PrintLoadingStatus()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Please enter play mode first.", "OK");
                return;
            }

            var loadingScreenService = ServiceLocator.Instance.Get<ILoadingScreenService>();
            if (loadingScreenService == null)
            {
                EditorUtility.DisplayDialog("Error", "LoadingScreenService not initialized.", "OK");
                return;
            }

            Debug.Log("=== Loading Screen Status ===");
            Debug.Log($"Is Showing: {loadingScreenService.IsShowing}");

            EditorUtility.DisplayDialog("Loading Screen Status",
                $"Is Showing: {loadingScreenService.IsShowing}",
                "OK");
        }

        [MenuItem("BakaTest/Debug/LoadingScreen/Print Loading Status", true)]
        public static bool PrintLoadingStatusValidate()
        {
            return Application.isPlaying;
        }

        #endregion
    }
}
