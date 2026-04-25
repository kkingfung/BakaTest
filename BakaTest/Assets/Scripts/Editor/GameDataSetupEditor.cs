#nullable enable
using UnityEngine;
using UnityEditor;
using BakaTest.Data.Champions;
using BakaTest.Data.Tests;
using System.IO;

namespace BakaTest.Editor
{
    /// <summary>
    /// ゲームデータのセットアップを支援するエディタツール
    /// </summary>
    public class GameDataSetupEditor
    {
        [MenuItem("BakaTest/Setup/Create Sample Champions")]
        public static void CreateSampleChampions()
        {
            string path = "Assets/Resources/Data/Champions";
            
            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Iron Knight (Tank)
            CreateChampion(path, "Champion_IronKnight", "iron_knight", "鉄壁の騎士",
                "歴史の知識を力に変える防御のエキスパート",
                ChampionRole.Tank, ElementType.Fire,
                new ChampionStats(1200, 80, 150, 40),
                new SubjectAffinity(0.5f, 0.8f, 0.3f, 1.5f),
                0.1f, 0.05f);

            // Math Genius (DPS)
            CreateChampion(path, "Champion_MathGenius", "math_genius", "数学の天才",
                "数学の力で敵を撃破する高火力アタッカー",
                ChampionRole.DPS, ElementType.Wind,
                new ChampionStats(800, 180, 70, 90),
                new SubjectAffinity(1.8f, 0.4f, 0.7f, 0.6f),
                0.25f, 0.15f);

            // Science Sorcerer (Mage)
            CreateChampion(path, "Champion_ScienceSorcerer", "science_sorcerer", "科学の魔術師",
                "科学の原理を操る高防御メイジ",
                ChampionRole.Mage, ElementType.Water,
                new ChampionStats(900, 120, 130, 70),
                new SubjectAffinity(0.8f, 1.5f, 0.5f, 0.7f),
                0.18f, 0.1f);

            // English Scholar (Support)
            CreateChampion(path, "Champion_EnglishScholar", "english_scholar", "英語の学者",
                "英語の知識で味方を支援するサポーター",
                ChampionRole.Support, ElementType.Earth,
                new ChampionStats(1000, 90, 110, 100),
                new SubjectAffinity(0.6f, 0.7f, 1.3f, 0.9f),
                0.12f, 0.2f);

            // Swift Blade (Assassin)
            CreateChampion(path, "Champion_SwiftBlade", "swift_blade", "迅速の刃",
                "高速で敵を圧倒するアサシン",
                ChampionRole.Assassin, ElementType.Fire,
                new ChampionStats(700, 150, 60, 140),
                new SubjectAffinity(1.2f, 0.3f, 1.4f, 0.5f),
                0.35f, 0.3f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[GameDataSetup] Sample champions created successfully!");
        }

        [MenuItem("BakaTest/Setup/Create Test Config")]
        public static void CreateTestConfig()
        {
            string path = "Assets/Resources/Config";
            
            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string assetPath = $"{path}/TestConfig.asset";
            
            // 既に存在する場合は上書き確認
            if (File.Exists(assetPath))
            {
                if (!EditorUtility.DisplayDialog("Confirm", 
                    "TestConfig already exists. Overwrite?", "Yes", "No"))
                {
                    return;
                }
            }

            TestConfig config = ScriptableObject.CreateInstance<TestConfig>();
            config.questionsPerTest = 10;
            config.timeLimit = 300f;
            config.dailyPointCapPerSubject = 1000;
            config.elementaryPointsPerQuestion = 10;
            config.middleSchoolPointsPerQuestion = 20;
            config.highSchoolPointsPerQuestion = 50;
            config.universityPointsPerQuestion = 100;

            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[GameDataSetup] TestConfig created at {assetPath}");
        }

        [MenuItem("BakaTest/Setup/Create All Game Data")]
        public static void CreateAllGameData()
        {
            CreateSampleChampions();
            CreateTestConfig();
            Debug.Log("[GameDataSetup] All game data created successfully!");
        }

        private static void CreateChampion(string basePath, string fileName, string id, string name,
            string description, ChampionRole role, ElementType element, ChampionStats baseStats,
            SubjectAffinity affinity, float critChance, float dodgeChance)
        {
            string assetPath = $"{basePath}/{fileName}.asset";
            
            // 既に存在する場合はスキップ
            if (File.Exists(assetPath))
            {
                Debug.Log($"[GameDataSetup] {fileName} already exists, skipping...");
                return;
            }

            ChampionData champion = ScriptableObject.CreateInstance<ChampionData>();
            champion.championId = id;
            champion.SetChampionName(BakaTest.Data.Localization.Language.Japanese, name);
            champion.SetDescription(BakaTest.Data.Localization.Language.Japanese, description);
            champion.role = role;
            champion.element = element;
            champion.baseStats = baseStats;
            champion.subjectAffinity = affinity;
            champion.criticalChance = critChance;
            champion.dodgeChance = dodgeChance;

            AssetDatabase.CreateAsset(champion, assetPath);
            Debug.Log($"[GameDataSetup] Created {fileName}");
        }
    }
}
