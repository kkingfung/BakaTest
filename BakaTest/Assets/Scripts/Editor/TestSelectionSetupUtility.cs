#nullable enable
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using BakaTest.Data.Tests;

namespace BakaTest.Editor
{
    /// <summary>
    /// テスト選択画面のセットアップを自動化するエディタユーティリティ
    /// </summary>
    public class TestSelectionSetupUtility : EditorWindow
    {
        private TestConfig? _testConfig;
        private GameObject? _gameBootstrap;
        private bool _configCreated = false;
        private bool _configAssigned = false;
        private bool _sceneCreated = false;

        [MenuItem("BakaTest/Setup/Test Selection Screen Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<TestSelectionSetupUtility>("Test Selection Setup");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Test Selection Screen Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This utility will automatically set up the Test Selection screen:\n\n" +
                "1. Create TestConfig ScriptableObject\n" +
                "2. Assign TestConfig to GameBootstrap\n" +
                "3. Create Test Selection Scene\n" +
                "4. Configure the scene with UI Document",
                MessageType.Info
            );

            GUILayout.Space(20);

            // Step 1: Create TestConfig
            GUILayout.Label("Step 1: Create TestConfig", EditorStyles.boldLabel);
            if (!_configCreated)
            {
                if (GUILayout.Button("Create TestConfig Asset", GUILayout.Height(30)))
                {
                    CreateTestConfig();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ TestConfig created successfully!", MessageType.Info);
                _testConfig = EditorGUILayout.ObjectField("TestConfig", _testConfig, typeof(TestConfig), false) as TestConfig;
            }

            GUILayout.Space(10);

            // Step 2: Assign to GameBootstrap
            GUILayout.Label("Step 2: Assign to GameBootstrap", EditorStyles.boldLabel);
            if (_configCreated && !_configAssigned)
            {
                EditorGUILayout.HelpBox(
                    "This will open the Startup scene and assign TestConfig to GameBootstrap.\n" +
                    "Make sure you've saved your current scene!",
                    MessageType.Warning
                );

                if (GUILayout.Button("Find and Assign to GameBootstrap", GUILayout.Height(30)))
                {
                    AssignToGameBootstrap();
                }
            }
            else if (_configAssigned)
            {
                EditorGUILayout.HelpBox("✓ TestConfig assigned to GameBootstrap!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Complete Step 1 first.", MessageType.None);
            }

            GUILayout.Space(10);

            // Step 3: Create Scene
            GUILayout.Label("Step 3: Create Test Selection Scene", EditorStyles.boldLabel);
            if (_configCreated && !_sceneCreated)
            {
                EditorGUILayout.HelpBox(
                    "This will create a new scene at Assets/Scenes/TestSelection.unity\n" +
                    "and set up the UI Document with TestSelectionView component.",
                    MessageType.Info
                );

                if (GUILayout.Button("Create Test Selection Scene", GUILayout.Height(30)))
                {
                    CreateTestSelectionScene();
                }
            }
            else if (_sceneCreated)
            {
                EditorGUILayout.HelpBox("✓ Test Selection scene created!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Complete Step 1 first.", MessageType.None);
            }

            GUILayout.Space(20);

            // Summary
            if (_configCreated && _configAssigned && _sceneCreated)
            {
                EditorGUILayout.HelpBox(
                    "✓ All setup steps completed!\n\n" +
                    "Next steps:\n" +
                    "1. Register TestSelection scene with Scene Management\n" +
                    "2. Add a button in Main Menu to load Test Selection\n" +
                    "3. Test the scene by playing from Startup",
                    MessageType.Info
                );
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        private void CreateTestConfig()
        {
            // 保存先ディレクトリを確認
            string directoryPath = "Assets/Resources/Config";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh();
                Debug.Log($"[TestSelectionSetup] Created directory: {directoryPath}");
            }

            // TestConfigアセットを作成
            string assetPath = $"{directoryPath}/TestConfig.asset";
            
            // 既存のアセットをチェック
            TestConfig? existingConfig = AssetDatabase.LoadAssetAtPath<TestConfig>(assetPath);
            if (existingConfig != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "TestConfig Already Exists",
                    "A TestConfig asset already exists. Do you want to use the existing one?",
                    "Use Existing",
                    "Cancel"
                );

                if (overwrite)
                {
                    _testConfig = existingConfig;
                    _configCreated = true;
                    Debug.Log("[TestSelectionSetup] Using existing TestConfig.");
                    return;
                }
                else
                {
                    return;
                }
            }

            // 新しいTestConfigを作成
            TestConfig config = ScriptableObject.CreateInstance<TestConfig>();
            
            // デフォルト値を設定
            config.questionsPerTest = 10;
            config.timeLimit = 300f; // 5分
            config.dailyPointCapPerSubject = 1000;
            config.elementaryPointsPerQuestion = 10;
            config.middleSchoolPointsPerQuestion = 20;
            config.highSchoolPointsPerQuestion = 50;
            config.universityPointsPerQuestion = 100;

            // アセットとして保存
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _testConfig = config;
            _configCreated = true;

            // 作成したアセットを選択
            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);

            Debug.Log($"[TestSelectionSetup] Created TestConfig at {assetPath}");
            EditorUtility.DisplayDialog("Success", "TestConfig asset created successfully!", "OK");
        }

        private void AssignToGameBootstrap()
        {
            if (_testConfig == null)
            {
                EditorUtility.DisplayDialog("Error", "TestConfig is null. Please create it first.", "OK");
                return;
            }

            // Startupシーンを探す
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene Startup");
            
            // Startupがない場合、SampleSceneを探す
            if (sceneGuids.Length == 0)
            {
                sceneGuids = AssetDatabase.FindAssets("t:Scene SampleScene");
            }
            
            // それでもない場合、すべてのシーンを探す
            if (sceneGuids.Length == 0)
            {
                sceneGuids = AssetDatabase.FindAssets("t:Scene");
                
                if (sceneGuids.Length == 0)
                {
                    EditorUtility.DisplayDialog(
                        "Scene Not Found",
                        "Could not find any scene files. Please create a scene with GameBootstrap and assign TestConfig manually.",
                        "OK"
                    );
                    return;
                }
                
                // 複数シーンがある場合、ユーザーに選ばせる
                if (sceneGuids.Length > 1)
                {
                    string[] sceneNames = new string[sceneGuids.Length];
                    for (int i = 0; i < sceneGuids.Length; i++)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                        sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(path);
                    }
                    
                    // 簡単な選択肢を表示
                    string message = "Multiple scenes found. Which one contains GameBootstrap?\n\n";
                    for (int i = 0; i < sceneNames.Length && i < 5; i++)
                    {
                        message += $"{i + 1}. {sceneNames[i]}\n";
                    }
                    message += "\nUsing the first scene. If incorrect, please assign TestConfig manually.";
                    
                    EditorUtility.DisplayDialog("Select Scene", message, "OK");
                }
            }

            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[0]);
            Debug.Log($"[TestSelectionSetup] Using scene: {scenePath}");

            // 現在のシーンを保存するか確認
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                bool save = EditorUtility.DisplayDialog(
                    "Save Current Scene?",
                    "Do you want to save the current scene before opening Startup?",
                    "Save",
                    "Don't Save"
                );

                if (save)
                {
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
            }

            // Startupシーンを開く
            Scene startupScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // GameBootstrapを探す
            GameObject[] rootObjects = startupScene.GetRootGameObjects();
            GameObject? gameBootstrap = null;
            System.Type? bootstrapType = System.Type.GetType("BakaTest.Core.Services.GameBootstrap, Assembly-CSharp");

            foreach (var root in rootObjects)
            {
                if (root.name == "GameBootstrap")
                {
                    gameBootstrap = root;
                    break;
                }

                // コンポーネントで検索
                if (bootstrapType != null)
                {
                    var component = root.GetComponent(bootstrapType);
                    if (component != null)
                    {
                        gameBootstrap = root;
                        break;
                    }

                    // 子オブジェクトも検索
                    component = root.GetComponentInChildren(bootstrapType);
                    if (component != null)
                    {
                        gameBootstrap = ((Component)component).gameObject;
                        break;
                    }
                }
            }

            if (gameBootstrap == null)
            {
                // GameBootstrapが見つからない場合、作成するか確認
                bool create = EditorUtility.DisplayDialog(
                    "GameBootstrap Not Found",
                    "Could not find GameBootstrap GameObject. Would you like to create it automatically?",
                    "Create",
                    "Cancel"
                );
                
                if (!create)
                {
                    return;
                }
                
                // GameBootstrapを作成
                if (bootstrapType == null)
                {
                    EditorUtility.DisplayDialog(
                        "Error",
                        "Could not find GameBootstrap type. Please ensure GameBootstrap.cs exists.",
                        "OK"
                    );
                    return;
                }
                
                gameBootstrap = new GameObject("GameBootstrap");
                gameBootstrap.AddComponent(bootstrapType);
                Debug.Log("[TestSelectionSetup] Created GameBootstrap GameObject.");
            }

            // GameBootstrapコンポーネントを取得
            if (bootstrapType == null)
            {
                EditorUtility.DisplayDialog(
                    "Type Not Found",
                    "Could not find GameBootstrap type. Please assign TestConfig manually.",
                    "OK"
                );
                return;
            }

            Component? bootstrapComponent = gameBootstrap.GetComponent(bootstrapType);
            if (bootstrapComponent == null)
            {
                EditorUtility.DisplayDialog(
                    "Component Not Found",
                    "GameBootstrap component not found. Please assign TestConfig manually.",
                    "OK"
                );
                return;
            }

            // SerializedObjectを使用してTestConfigを割り当て
            SerializedObject serializedBootstrap = new SerializedObject(bootstrapComponent);
            SerializedProperty testConfigProperty = serializedBootstrap.FindProperty("_testConfig");

            if (testConfigProperty == null)
            {
                EditorUtility.DisplayDialog(
                    "Field Not Found",
                    "Could not find _testConfig field in GameBootstrap. The field name might be different.",
                    "OK"
                );
                return;
            }

            testConfigProperty.objectReferenceValue = _testConfig;
            serializedBootstrap.ApplyModifiedProperties();

            // シーンを保存
            EditorSceneManager.MarkSceneDirty(startupScene);
            EditorSceneManager.SaveScene(startupScene);

            _configAssigned = true;
            _gameBootstrap = gameBootstrap;

            Debug.Log("[TestSelectionSetup] TestConfig assigned to GameBootstrap successfully!");
            EditorUtility.DisplayDialog("Success", "TestConfig has been assigned to GameBootstrap!", "OK");
        }

        private void CreateTestSelectionScene()
        {
            // 保存先ディレクトリを確認
            string scenesPath = "Assets/Scenes";
            if (!Directory.Exists(scenesPath))
            {
                Directory.CreateDirectory(scenesPath);
                AssetDatabase.Refresh();
            }

            string scenePath = $"{scenesPath}/TestSelection.unity";

            // 既存のシーンをチェック
            if (File.Exists(scenePath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Scene Already Exists",
                    "TestSelection.unity already exists. Do you want to overwrite it?",
                    "Overwrite",
                    "Cancel"
                );

                if (!overwrite)
                {
                    return;
                }
            }

            // 新しい空のシーンを作成
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // UI DocumentとTestSelectionViewを持つGameObjectを作成
            GameObject uiObject = new GameObject("TestSelectionUI");
            
            // UIDocumentコンポーネントを追加
            UIDocument uiDocument = uiObject.AddComponent<UIDocument>();

            // UXMLファイルをロード
            string uxmlPath = "Assets/UI/TestSelection/TestSelectionDocument.uxml";
            VisualTreeAsset? uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            
            if (uxmlAsset != null)
            {
                uiDocument.visualTreeAsset = uxmlAsset;
                Debug.Log("[TestSelectionSetup] Assigned UXML to UIDocument.");
            }
            else
            {
                Debug.LogWarning($"[TestSelectionSetup] Could not find UXML at {uxmlPath}. Please assign it manually.");
            }

            // PanelSettingsを探して割り当て（あれば）
            string[] panelSettingsGuids = AssetDatabase.FindAssets("t:PanelSettings");
            if (panelSettingsGuids.Length > 0)
            {
                string panelSettingsPath = AssetDatabase.GUIDToAssetPath(panelSettingsGuids[0]);
                PanelSettings? panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(panelSettingsPath);
                if (panelSettings != null)
                {
                    uiDocument.panelSettings = panelSettings;
                    Debug.Log("[TestSelectionSetup] Assigned PanelSettings to UIDocument.");
                }
            }

            // TestSelectionViewコンポーネントを追加
            System.Type? viewType = System.Type.GetType("BakaTest.Views.TestSelectionView, BakaTest.UI");
            if (viewType == null)
            {
                viewType = System.Type.GetType("BakaTest.Views.TestSelectionView, Assembly-CSharp");
            }
            
            if (viewType != null)
            {
                uiObject.AddComponent(viewType);
            }
            else
            {
                Debug.LogWarning("[TestSelectionSetup] Could not find TestSelectionView type. Please add it manually.");
            }
            Debug.Log("[TestSelectionSetup] Added TestSelectionView component.");

            // シーンを保存
            EditorSceneManager.SaveScene(newScene, scenePath);
            _sceneCreated = true;

            Debug.Log($"[TestSelectionSetup] Created scene at {scenePath}");
            EditorUtility.DisplayDialog(
                "Success",
                $"Test Selection scene created at:\n{scenePath}\n\nThe scene is now open. You can test it by adding it to your build settings and scene management system.",
                "OK"
            );
        }
    }
}
