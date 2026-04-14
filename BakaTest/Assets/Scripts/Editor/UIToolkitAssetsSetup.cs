#nullable enable
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

namespace BakaTest.Editor
{
    /// <summary>
    /// UI Toolkit用の必要なアセットを自動作成するエディタユーティリティ
    /// </summary>
    public class UIToolkitAssetsSetup : EditorWindow
    {
        private bool _panelSettingsCreated = false;
        private PanelSettings? _panelSettings;

        [MenuItem("BakaTest/Setup/Create UI Toolkit Assets")]
        public static void ShowWindow()
        {
            var window = GetWindow<UIToolkitAssetsSetup>("UI Toolkit Assets Setup");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("UI Toolkit Assets Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This utility will create all necessary UI Toolkit assets:\n\n" +
                "1. PanelSettings (for UI Document)\n" +
                "2. Default Theme StyleSheet\n" +
                "3. Configure Panel Settings properly",
                MessageType.Info
            );

            GUILayout.Space(20);

            // Step 1: Create PanelSettings
            GUILayout.Label("Step 1: Create PanelSettings", EditorStyles.boldLabel);
            if (!_panelSettingsCreated)
            {
                if (GUILayout.Button("Create PanelSettings Asset", GUILayout.Height(30)))
                {
                    CreatePanelSettings();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("✓ PanelSettings created successfully!", MessageType.Info);
                _panelSettings = EditorGUILayout.ObjectField("PanelSettings", _panelSettings, typeof(PanelSettings), false) as PanelSettings;
            }

            GUILayout.Space(10);

            // Step 2: Assign to UI Documents
            GUILayout.Label("Step 2: Assign to UI Documents", EditorStyles.boldLabel);
            if (_panelSettingsCreated)
            {
                EditorGUILayout.HelpBox(
                    "This will find all UIDocument components and assign the PanelSettings to them.",
                    MessageType.Info
                );

                if (GUILayout.Button("Assign to All UI Documents", GUILayout.Height(30)))
                {
                    AssignToUIDocuments();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Complete Step 1 first.", MessageType.None);
            }

            GUILayout.Space(20);

            // Summary
            if (_panelSettingsCreated)
            {
                EditorGUILayout.HelpBox(
                    "✓ Setup completed!\n\n" +
                    "Your UI Toolkit is now properly configured.",
                    MessageType.Info
                );
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }

        private void CreatePanelSettings()
        {
            // ディレクトリを確認
            string directoryPath = "Assets/UI/Settings";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh();
                Debug.Log($"[UIToolkitSetup] Created directory: {directoryPath}");
            }

            string assetPath = $"{directoryPath}/DefaultPanelSettings.asset";

            // 既存のアセットをチェック
            PanelSettings? existingSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(assetPath);
            if (existingSettings != null)
            {
                bool useExisting = EditorUtility.DisplayDialog(
                    "PanelSettings Already Exists",
                    "A PanelSettings asset already exists. Do you want to use the existing one?",
                    "Use Existing",
                    "Cancel"
                );

                if (useExisting)
                {
                    _panelSettings = existingSettings;
                    _panelSettingsCreated = true;
                    Debug.Log("[UIToolkitSetup] Using existing PanelSettings.");
                    return;
                }
                else
                {
                    return;
                }
            }

            // 新しいPanelSettingsを作成
            PanelSettings settings = ScriptableObject.CreateInstance<PanelSettings>();

            // デフォルト設定
            settings.scaleMode = PanelScaleMode.ConstantPixelSize;
            settings.scale = 1.0f;
            settings.referenceDpi = 96;
            settings.fallbackDpi = 96;
            settings.referenceResolution = new Vector2Int(1920, 1080);
            settings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            settings.match = 0.5f;
            settings.sortingOrder = 0;
            settings.targetDisplay = 0;

            // アセットとして保存
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _panelSettings = settings;
            _panelSettingsCreated = true;

            // 選択
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);

            Debug.Log($"[UIToolkitSetup] Created PanelSettings at {assetPath}");
            EditorUtility.DisplayDialog("Success", "PanelSettings asset created successfully!", "OK");
        }

        private void AssignToUIDocuments()
        {
            if (_panelSettings == null)
            {
                EditorUtility.DisplayDialog("Error", "PanelSettings is null.", "OK");
                return;
            }

            // すべてのシーンを検索
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            int updatedCount = 0;
            int sceneCount = 0;

            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                // TestSelectionシーンとSampleSceneを優先的に処理
                if (!sceneName.Contains("TestSelection") && !sceneName.Contains("SampleScene") && !sceneName.Contains("Startup"))
                {
                    continue;
                }

                sceneCount++;

                // シーンを開く
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
                var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

                // すべてのUIDocumentを探す
                UIDocument[] uiDocuments = GameObject.FindObjectsOfType<UIDocument>();

                foreach (var uiDoc in uiDocuments)
                {
                    if (uiDoc.panelSettings == null || uiDoc.panelSettings != _panelSettings)
                    {
                        uiDoc.panelSettings = _panelSettings;
                        UnityEditor.EditorUtility.SetDirty(uiDoc);
                        updatedCount++;
                        Debug.Log($"[UIToolkitSetup] Assigned PanelSettings to {uiDoc.name} in {sceneName}");
                    }
                }

                // シーンを保存
                if (scene.isDirty)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string message = $"Updated {updatedCount} UIDocument(s) in {sceneCount} scene(s).";
            Debug.Log($"[UIToolkitSetup] {message}");
            EditorUtility.DisplayDialog("Success", message, "OK");
        }
    }
}
