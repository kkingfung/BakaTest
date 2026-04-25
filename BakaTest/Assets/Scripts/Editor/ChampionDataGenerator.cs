#nullable enable
using UnityEditor;
using UnityEngine;
using BakaTest.Data.Champions;
using System.IO;

namespace BakaTest.Editor
{
    /// <summary>
    /// ChampionDataのScriptableObjectを生成するユーティリティ
    /// </summary>
    public static class ChampionDataGenerator
    {
        private const string OutputFolder = "Assets/Resources/Data/Champions";
        private const string PortraitsFolder = "Assets/TomatocolCharacterVarietyPackVol1DEMO/Assets";
        private const string SyntyCharactersFolder = "Assets/Synty/SidekickCharacters/Characters";

        [MenuItem("BakaTest/Generate/Create Sample Champions")]
        public static void GenerateSampleChampions()
        {
            // フォルダが存在しない場合は作成
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
                AssetDatabase.Refresh();
            }

            Debug.Log("[ChampionDataGenerator] Generating sample champions...");

            // 5体のチャンピオンを生成
            CreateMathWarrior();
            CreateScienceTank();
            CreateEnglishAssassin();
            CreateHistoryGuardian();
            CreateBalancedMage();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[ChampionDataGenerator] 5 champions created in {OutputFolder}");
        }

        /// <summary>
        /// 数学特化の戦士
        /// </summary>
        private static void CreateMathWarrior()
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();

            champion.championId = "champion_math_warrior";
            champion.SetChampionName(Data.Localization.Language.English, "Math Warrior");
            champion.SetDescription(Data.Localization.Language.English, "A fierce warrior who converts mathematical knowledge into devastating attacks.");
            champion.role = ChampionRole.DPS;
            champion.element = ElementType.Fire;

            // 高攻撃力、中HP、低防御
            champion.baseStats = new ChampionStats
            {
                HP = 800,
                Attack = 120,
                Defense = 40,
                Speed = 70
            };

            // 数学→攻撃力の変換率が高い
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.8f,      // Math → Attack (非常に高い)
                ScienceToDefenseRatio = 0.2f,  // Science → Defense (低い)
                EnglishToSpeedRatio = 0.3f,    // English → Speed (中程度)
                HistoryToHPRatio = 1.5f        // History → HP (中程度)
            };

            champion.criticalChance = 0.20f;  // 20% クリティカル率
            champion.dodgeChance = 0.08f;     // 8% 回避率

            // アイコンを設定
            champion.icon = LoadPortraitSprite("img025");

            // 3Dモデルを設定（TODO: Champion用の3Dモデルに差し替え）
            // champion.modelPrefab = LoadSyntyCharacterPrefab("HumanSpecies/HumanSpecies_01/HumanSpecies_01");

            CreateAsset(champion, "Champion_MathWarrior.asset");
        }

        /// <summary>
        /// 理科特化のタンク
        /// </summary>
        private static void CreateScienceTank()
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();

            champion.championId = "champion_science_tank";
            champion.SetChampionName(Data.Localization.Language.English, "Science Tank");
            champion.SetDescription(Data.Localization.Language.English, "A defensive fortress powered by scientific understanding.");
            champion.role = ChampionRole.Tank;
            champion.element = ElementType.Earth;

            // 高HP、高防御、低攻撃
            champion.baseStats = new ChampionStats
            {
                HP = 1200,
                Attack = 60,
                Defense = 90,
                Speed = 50
            };

            // 理科→防御力の変換率が高い
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.3f,
                ScienceToDefenseRatio = 0.9f,   // Science → Defense (非常に高い)
                EnglishToSpeedRatio = 0.2f,
                HistoryToHPRatio = 2.5f          // History → HP (高い)
            };

            champion.criticalChance = 0.08f;
            champion.dodgeChance = 0.05f;

            // アイコンを設定
            champion.icon = LoadPortraitSprite("img030");

            // 3Dモデルを設定（TODO: Champion用の3Dモデルに差し替え）
            // champion.modelPrefab = LoadSyntyCharacterPrefab("Starter/Starter_02/Starter_02");

            CreateAsset(champion, "Champion_ScienceTank.asset");
        }

        /// <summary>
        /// 英語特化のアサシン
        /// </summary>
        private static void CreateEnglishAssassin()
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();

            champion.championId = "champion_english_assassin";
            champion.SetChampionName(Data.Localization.Language.English, "English Assassin");
            champion.SetDescription(Data.Localization.Language.English, "Swift and deadly, striking before opponents can react.");
            champion.role = ChampionRole.Assassin;
            champion.element = ElementType.Wind;

            // 高速度、中攻撃、低HP
            champion.baseStats = new ChampionStats
            {
                HP = 600,
                Attack = 90,
                Defense = 35,
                Speed = 110
            };

            // 英語→速度の変換率が高い
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.5f,
                ScienceToDefenseRatio = 0.2f,
                EnglishToSpeedRatio = 0.8f,     // English → Speed (非常に高い)
                HistoryToHPRatio = 1.0f
            };

            champion.criticalChance = 0.25f;  // 25% クリティカル率（最高）
            champion.dodgeChance = 0.18f;     // 18% 回避率（最高）

            // アイコンを設定
            champion.icon = LoadPortraitSprite("img052");

            // 3Dモデルを設定（TODO: Champion用の3Dモデルに差し替え）
            // champion.modelPrefab = LoadSyntyCharacterPrefab("Starter/Starter_04/Starter_04");

            CreateAsset(champion, "Champion_EnglishAssassin.asset");
        }

        /// <summary>
        /// 歴史特化のガーディアン
        /// </summary>
        private static void CreateHistoryGuardian()
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();

            champion.championId = "champion_history_guardian";
            champion.SetChampionName(Data.Localization.Language.English, "History Guardian");
            champion.SetDescription(Data.Localization.Language.English, "Protected by the wisdom of ages, with unmatched endurance.");
            champion.role = ChampionRole.Support;
            champion.element = ElementType.Earth;

            // 超高HP、中防御、低攻撃
            champion.baseStats = new ChampionStats
            {
                HP = 1000,
                Attack = 70,
                Defense = 60,
                Speed = 55
            };

            // 歴史→HPの変換率が非常に高い
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.3f,
                ScienceToDefenseRatio = 0.4f,
                EnglishToSpeedRatio = 0.2f,
                HistoryToHPRatio = 3.0f         // History → HP (最高)
            };

            champion.criticalChance = 0.10f;
            champion.dodgeChance = 0.08f;

            // アイコンを設定
            champion.icon = LoadPortraitSprite("img066");

            // 3Dモデルを設定（TODO: Champion用の3Dモデルに差し替え）
            // champion.modelPrefab = LoadSyntyCharacterPrefab("HumanSpecies/HumanSpecies_03/HumanSpecies_03");

            CreateAsset(champion, "Champion_HistoryGuardian.asset");
        }

        /// <summary>
        /// バランス型のメイジ
        /// </summary>
        private static void CreateBalancedMage()
        {
            var champion = ScriptableObject.CreateInstance<ChampionData>();

            champion.championId = "champion_balanced_mage";
            champion.SetChampionName(Data.Localization.Language.English, "Balanced Mage");
            champion.SetDescription(Data.Localization.Language.English, "A versatile caster with balanced stats across all subjects.");
            champion.role = ChampionRole.Mage;
            champion.element = ElementType.Water;

            // 全てのステータスが中程度
            champion.baseStats = new ChampionStats
            {
                HP = 850,
                Attack = 85,
                Defense = 55,
                Speed = 75
            };

            // 全ての教科に均等な親和性
            champion.subjectAffinity = new SubjectAffinity
            {
                MathToAttackRatio = 0.5f,
                ScienceToDefenseRatio = 0.5f,
                EnglishToSpeedRatio = 0.5f,
                HistoryToHPRatio = 2.0f
            };

            champion.criticalChance = 0.15f;
            champion.dodgeChance = 0.12f;

            // アイコンを設定
            champion.icon = LoadPortraitSprite("img098");

            // 3Dモデルを設定（TODO: Champion用の3Dモデルに差し替え）
            // champion.modelPrefab = LoadSyntyCharacterPrefab("Starter/Starter_03/Starter_03");

            CreateAsset(champion, "Champion_BalancedMage.asset");
        }

        /// <summary>
        /// アセットを作成して保存します
        /// </summary>
        private static void CreateAsset(ChampionData champion, string fileName)
        {
            string path = Path.Combine(OutputFolder, fileName);
            
            // 既存のアセットがある場合は上書き確認
            if (File.Exists(path))
            {
                if (!EditorUtility.DisplayDialog(
                    "Overwrite Confirmation",
                    $"Champion '{champion.GetChampionName(Data.Localization.Language.English)}' already exists. Overwrite?",
                    "Yes",
                    "No"))
                {
                    Debug.Log($"[ChampionDataGenerator] Skipped {champion.GetChampionName(Data.Localization.Language.English)}");
                    return;
                }
            }

            AssetDatabase.CreateAsset(champion, path);
            Debug.Log($"[ChampionDataGenerator] Created {champion.GetChampionName(Data.Localization.Language.English)} at {path}");
        }

        /// <summary>
        /// Tomatocolポートレートスプライトを読み込みます
        /// </summary>
        private static Sprite? LoadPortraitSprite(string imageName)
        {
            string path = $"{PortraitsFolder}/{imageName}.png";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (texture == null)
            {
                Debug.LogWarning($"[ChampionDataGenerator] Portrait texture not found at {path}");
                return null;
            }

            // テクスチャからスプライトを作成
            string spritePath = AssetDatabase.GetAssetPath(texture);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath);

            foreach (var asset in sprites)
            {
                if (asset is Sprite sprite)
                {
                    return sprite;
                }
            }

            Debug.LogWarning($"[ChampionDataGenerator] No sprite found for texture {imageName}");
            return null;
        }

        /// <summary>
        /// Syntyキャラクタープレハブを読み込みます
        /// </summary>
        private static GameObject? LoadSyntyCharacterPrefab(string prefabPath)
        {
            string fullPath = $"{SyntyCharactersFolder}/{prefabPath}.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);

            if (prefab == null)
            {
                Debug.LogWarning($"[ChampionDataGenerator] Character prefab not found at {fullPath}");
                return null;
            }

            return prefab;
        }
    }
}
