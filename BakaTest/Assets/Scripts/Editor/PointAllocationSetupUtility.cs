#nullable enable
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using System.IO;

namespace BakaTest.Editor
{
    /// <summary>
    /// ポイント配分画面のセットアップユーティリティ
    /// </summary>
    /// <remarks>
    /// Unity Editorメニューから実行し、ポイント配分画面に必要なシーンとアセットを自動的に作成します。
    /// BakaTest → Setup → Point Allocation Screen Setup で実行できます。
    /// </remarks>
    public class PointAllocationSetupUtility : EditorWindow
    {
        private const string SCENE_PATH = "Assets/Scenes/PointAllocation.unity";
        private const string UXML_PATH = "Assets/UI/PointAllocation/PointAllocationDocument.uxml";
        private const string USS_PATH = "Assets/UI/PointAllocation/PointAllocationStyles.uss";
        private const string PANEL_SETTINGS_PATH = "Assets/UI/Settings/DefaultPanelSettings.asset";

        private bool _sceneExists;
        private bool _uxmlExists;
        private bool _ussExists;
        private bool _panelSettingsExists;

        [MenuItem("BakaTest/Setup/Point Allocation Screen Setup")]
        public static void ShowWindow()
        {
            var window = GetWindow<PointAllocationSetupUtility>("Point Allocation Setup");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }

        private void OnEnable()
        {
            CheckAssetStatus();
        }

        private void OnGUI()
        {
            GUILayout.Label("Point Allocation Screen Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This utility will set up the Point Allocation screen with all necessary components:\n" +
                "• PointAllocation.unity scene\n" +
                "• UIDocument with UXML/USS references\n" +
                "• PointAllocationView component\n" +
                "• PanelSettings configuration",
                MessageType.Info
            );

            GUILayout.Space(20);

            // Asset status
            GUILayout.Label("Asset Status:", EditorStyles.boldLabel);
            DrawStatusLine("PointAllocation Scene", _sceneExists);
            DrawStatusLine("UXML Document", _uxmlExists);
            DrawStatusLine("USS Stylesheet", _ussExists);
            DrawStatusLine("Panel Settings", _panelSettingsExists);

            GUILayout.Space(20);

            // Setup button
            GUI.enabled = !_sceneExists;
            if (GUILayout.Button("Create Point Allocation Scene", GUILayout.Height(40)))
            {
                CreatePointAllocationScene();
                CheckAssetStatus();
            }
            GUI.enabled = true;

            if (_sceneExists)
            {
                EditorGUILayout.HelpBox("Scene already exists! Delete it first if you want to recreate.", MessageType.Warning);
            }

            GUILayout.Space(10);

            // Open scene button
            GUI.enabled = _sceneExists;
            if (GUILayout.Button("Open Point Allocation Scene", GUILayout.Height(30)))
            {
                EditorSceneManager.OpenScene(SCENE_PATH);
            }
            GUI.enabled = true;

            GUILayout.Space(20);

            // Refresh button
            if (GUILayout.Button("Refresh Status"))
            {
                CheckAssetStatus();
            }
        }

        private void DrawStatusLine(string label, bool exists)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(200));
            
            var oldColor = GUI.color;
            GUI.color = exists ? Color.green : Color.red;
            GUILayout.Label(exists ? "✓ Exists" : "✗ Missing", GUILayout.Width(100));
            GUI.color = oldColor;
            
            GUILayout.EndHorizontal();
        }

        private void CheckAssetStatus()
        {
            _sceneExists = File.Exists(SCENE_PATH);
            _uxmlExists = File.Exists(UXML_PATH);
            _ussExists = File.Exists(USS_PATH);
            _panelSettingsExists = File.Exists(PANEL_SETTINGS_PATH);

            Repaint();
        }

        private void CreatePointAllocationScene()
        {
            if (_sceneExists)
            {
                EditorUtility.DisplayDialog("Scene Exists", "PointAllocation scene already exists!", "OK");
                return;
            }

            // UXML/USSの存在確認
            if (!_uxmlExists)
            {
                EditorUtility.DisplayDialog("Missing UXML", 
                    $"UXML file not found at {UXML_PATH}\n\n" +
                    "Please ensure PointAllocationDocument.uxml exists before creating the scene.", 
                    "OK");
                return;
            }

            if (!_ussExists)
            {
                EditorUtility.DisplayDialog("Missing USS", 
                    $"USS file not found at {USS_PATH}\n\n" +
                    "Please ensure PointAllocationStyles.uss exists before creating the scene.", 
                    "OK");
                return;
            }

            // Scenes ディレクトリを確保
            string sceneDirectory = Path.GetDirectoryName(SCENE_PATH);
            if (!string.IsNullOrEmpty(sceneDirectory) && !Directory.Exists(sceneDirectory))
            {
                Directory.CreateDirectory(sceneDirectory);
                AssetDatabase.Refresh();
            }

            // 新しいシーンを作成
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera を追加
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.12f, 0.15f, 1f);
            camera.orthographic = false;
            cameraObject.tag = "MainCamera";

            // UI Document GameObject を作成
            GameObject uiObject = new GameObject("PointAllocationUI");
            UIDocument uiDocument = uiObject.AddComponent<UIDocument>();

            // UXML/USSを読み込んで設定
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            if (visualTreeAsset != null)
            {
                uiDocument.visualTreeAsset = visualTreeAsset;
            }
            else
            {
                Debug.LogError($"[PointAllocationSetup] Failed to load UXML at {UXML_PATH}");
            }

            // PanelSettings を設定
            if (_panelSettingsExists)
            {
                var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PANEL_SETTINGS_PATH);
                if (panelSettings != null)
                {
                    uiDocument.panelSettings = panelSettings;
                }
            }
            else
            {
                Debug.LogWarning("[PointAllocationSetup] PanelSettings not found. Please assign manually or run 'Create UI Toolkit Assets' utility.");
            }

            // PointAllocationView コンポーネントを追加（Reflectionを使用してEditor依存を回避）
            System.Type? viewType = System.Type.GetType("BakaTest.Views.PointAllocationView, Assembly-CSharp");
            if (viewType != null)
            {
                uiObject.AddComponent(viewType);
                Debug.Log("[PointAllocationSetup] PointAllocationView component added successfully.");
            }
            else
            {
                Debug.LogError("[PointAllocationSetup] PointAllocationView type not found! Make sure the script is compiled.");
            }

            // シーンを保存
            bool saved = EditorSceneManager.SaveScene(newScene, SCENE_PATH);

            if (saved)
            {
                Debug.Log($"[PointAllocationSetup] Point Allocation scene created successfully at {SCENE_PATH}");
                EditorUtility.DisplayDialog("Success", 
                    "Point Allocation scene created successfully!\n\n" +
                    "Scene: " + SCENE_PATH + "\n\n" +
                    "The scene is now open in the editor.", 
                    "OK");
            }
            else
            {
                Debug.LogError("[PointAllocationSetup] Failed to save scene!");
                EditorUtility.DisplayDialog("Error", "Failed to save the scene!", "OK");
            }

            AssetDatabase.Refresh();
        }
    }
}
