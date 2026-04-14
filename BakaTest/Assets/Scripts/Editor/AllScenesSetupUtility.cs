#nullable enable
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

namespace BakaTest.Editor
{
    /// <summary>
    /// 全シーンのセットアップを自動化するエディタユーティリティ
    /// </summary>
    /// <remarks>
    /// 不足している6つのシーン（MainMenu, TestTaking, ChampionSelection, PointAllocation, Battle, Results）を作成します。
    /// </remarks>
    public class AllScenesSetupUtility : EditorWindow
    {
        private Vector2 _scrollPosition;
        private Dictionary<string, bool> _sceneCreated = new Dictionary<string, bool>();

        private readonly string[] _scenesToCreate = new string[]
        {
            "MainMenu",
            "TestTaking",
            "ChampionSelection",
            "PointAllocation",
            "Battle",
            "Results"
        };

        private readonly Dictionary<string, string> _uiDocumentPaths = new Dictionary<string, string>
        {
            { "MainMenu", "Assets/UI/MainMenu/MainMenuDocument.uxml" },
            { "TestTaking", "Assets/UI/TestTaking/TestTakingDocument.uxml" },
            { "ChampionSelection", "Assets/UI/ChampionSelection/ChampionSelectionDocument.uxml" },
            { "PointAllocation", "Assets/UI/PointAllocation/PointAllocationDocument.uxml" },
            { "Battle", "Assets/UI/Battle/BattleDocument.uxml" },
            { "Results", "Assets/UI/TestResults/TestResultsDocument.uxml" }
        };

        private readonly Dictionary<string, string> _viewScriptNames = new Dictionary<string, string>
        {
            { "MainMenu", "MainMenuView" },
            { "TestTaking", "TestTakingView" },
            { "ChampionSelection", "ChampionSelectionView" },
            { "PointAllocation", "PointAllocationView" },
            { "Battle", "BattleView" },
            { "Results", "ResultsView" }
        };

        [MenuItem("BakaTest/Setup/Create All Missing Scenes")]
        public static void ShowWindow()
        {
            var window = GetWindow<AllScenesSetupUtility>("All Scenes Setup");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnEnable()
        {
            // シーンの存在確認
            CheckExistingScenes();
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("BakaTest - All Scenes Setup Utility", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "このユーティリティは不足している6つのシーンを作成します:\n\n" +
                "• MainMenu - メインメニュー画面\n" +
                "• TestTaking - テスト受験画面\n" +
                "• ChampionSelection - チャンピオン選択画面\n" +
                "• PointAllocation - ポイント配分画面\n" +
                "• Battle - バトル画面\n" +
                "• Results - 結果画面\n\n" +
                "各シーンには自動的にUI DocumentとViewコンポーネントが設定されます。",
                MessageType.Info
            );

            GUILayout.Space(20);

            // 一括作成ボタン
            if (GUILayout.Button("Create All Missing Scenes", GUILayout.Height(40)))
            {
                CreateAllMissingScenes();
            }

            GUILayout.Space(20);
            GUILayout.Label("Individual Scene Creation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("または個別にシーンを作成できます:", MessageType.None);
            GUILayout.Space(10);

            // 各シーンの個別作成ボタン
            foreach (var sceneName in _scenesToCreate)
            {
                DrawSceneSetupSection(sceneName);
                GUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSceneSetupSection(string sceneName)
        {
            EditorGUILayout.BeginHorizontal("box");

            bool exists = _sceneCreated.ContainsKey(sceneName) && _sceneCreated[sceneName];

            if (exists)
            {
                GUILayout.Label("✓", GUILayout.Width(20));
                GUI.enabled = false;
            }

            GUILayout.Label(sceneName, GUILayout.Width(150));

            if (GUILayout.Button(exists ? "Already Created" : "Create Scene", GUILayout.Width(120)))
            {
                CreateScene(sceneName);
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }

        private void CheckExistingScenes()
        {
            _sceneCreated.Clear();

            foreach (var sceneName in _scenesToCreate)
            {
                string scenePath = $"Assets/Scenes/{sceneName}.unity";
                _sceneCreated[sceneName] = File.Exists(scenePath);
            }
        }

        private void CreateAllMissingScenes()
        {
            bool anyCreated = false;

            foreach (var sceneName in _scenesToCreate)
            {
                if (!_sceneCreated[sceneName])
                {
                    CreateScene(sceneName);
                    anyCreated = true;
                }
            }

            if (anyCreated)
            {
                EditorUtility.DisplayDialog(
                    "Scenes Created",
                    "不足しているシーンが正常に作成されました！\n\n" +
                    "次のステップ:\n" +
                    "1. 各シーンを開いて確認してください\n" +
                    "2. 必要に応じてカメラや照明を調整してください\n" +
                    "3. Build SettingsにシーンToを追加してください",
                    "OK"
                );

                CheckExistingScenes();
                Repaint();
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "No Scenes Created",
                    "すべてのシーンは既に存在します。",
                    "OK"
                );
            }
        }

        private void CreateScene(string sceneName)
        {
            // 現在のシーンを保存するか確認
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                Debug.LogWarning($"[AllScenesSetupUtility] Scene creation cancelled by user.");
                return;
            }

            // 新しいシーンを作成
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Scenes フォルダが存在するか確認
            string scenesFolder = "Assets/Scenes";
            if (!Directory.Exists(scenesFolder))
            {
                Directory.CreateDirectory(scenesFolder);
                AssetDatabase.Refresh();
            }

            // シーンパス
            string scenePath = $"{scenesFolder}/{sceneName}.unity";

            // UI Canvasを作成
            CreateUICanvas(sceneName);

            // シーンを保存
            EditorSceneManager.SaveScene(newScene, scenePath);
            AssetDatabase.Refresh();

            Debug.Log($"[AllScenesSetupUtility] Scene created: {scenePath}");

            _sceneCreated[sceneName] = true;
            Repaint();
        }

        private void CreateUICanvas(string sceneName)
        {
            // UIDocumentを持つGameObjectを作成
            GameObject uiRoot = new GameObject($"{sceneName}UI");

            // UIDocumentコンポーネントを追加
            var uiDocument = uiRoot.AddComponent<UIDocument>();

            // UXMLアセットをロード
            if (_uiDocumentPaths.TryGetValue(sceneName, out string uxmlPath))
            {
                var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
                if (visualTreeAsset != null)
                {
                    uiDocument.visualTreeAsset = visualTreeAsset;
                    Debug.Log($"[AllScenesSetupUtility] UXML assigned: {uxmlPath}");
                }
                else
                {
                    Debug.LogWarning($"[AllScenesSetupUtility] UXML not found: {uxmlPath}");
                }
            }

            // PanelSettings設定（デフォルトを使用）
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI/Settings/DefaultPanelSettings.asset");
            if (panelSettings != null)
            {
                uiDocument.panelSettings = panelSettings;
            }

            // Viewスクリプトをアタッチ
            if (_viewScriptNames.TryGetValue(sceneName, out string viewScriptName))
            {
                var viewType = System.Type.GetType($"BakaTest.Views.{viewScriptName}, Assembly-CSharp");
                if (viewType != null)
                {
                    uiRoot.AddComponent(viewType);
                    Debug.Log($"[AllScenesSetupUtility] View component added: {viewScriptName}");
                }
                else
                {
                    Debug.LogWarning($"[AllScenesSetupUtility] View script not found: {viewScriptName}");
                }
            }

            // カメラの背景色を設定（UI用）
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0.078f, 0.098f, 0.137f); // ダークブルー
            }
        }
    }
}
