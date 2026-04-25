#nullable enable
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace BakaTest.Editor
{
    /// <summary>
    /// インベントリ画面の自動セットアップユーティリティ
    /// </summary>
    /// <remarks>
    /// メニューから実行することで、Inventoryシーンを自動生成します。
    /// - シーンの作成
    /// - UIDocumentの設定
    /// - InventoryViewコンポーネントの追加
    /// </remarks>
    public static class InventorySetupUtility
    {
        private const string ScenePath = "Assets/Scenes/Inventory.unity";
        private const string UxmlPath = "Assets/UI/Inventory.uxml";
        private const string UssPath = "Assets/UI/Inventory.uss";
        private const string PanelSettingsPath = "Assets/UI/Settings/DefaultPanelSettings.asset";

        [MenuItem("BakaTest/Setup/Inventory Screen Setup")]
        public static void SetupInventoryScene()
        {
            Debug.Log("[InventorySetupUtility] Starting Inventory scene setup...");

            // 1. 必要なアセットの存在確認
            if (!ValidateAssets())
            {
                Debug.LogError("[InventorySetupUtility] Setup aborted due to missing assets.");
                return;
            }

            // 2. 新しいシーンを作成
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 3. カメラを作成
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
            cameraObj.tag = "MainCamera";

            // 4. UI Documentを作成
            GameObject uiObj = new GameObject("InventoryUI");
            UIDocument uiDocument = uiObj.AddComponent<UIDocument>();

            // UXML/USSをロード
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);

            if (visualTreeAsset == null)
            {
                Debug.LogError($"[InventorySetupUtility] Failed to load UXML at {UxmlPath}");
                return;
            }

            if (panelSettings == null)
            {
                Debug.LogError($"[InventorySetupUtility] Failed to load PanelSettings at {PanelSettingsPath}");
                return;
            }

            uiDocument.visualTreeAsset = visualTreeAsset;
            uiDocument.panelSettings = panelSettings;

            // 5. InventoryViewコンポーネントを追加（リフレクション使用）
            var viewType = System.Type.GetType("BakaTest.Views.InventoryView, Assembly-CSharp");
            if (viewType != null)
            {
                uiObj.AddComponent(viewType);
                Debug.Log("[InventorySetupUtility] InventoryView component added.");
            }
            else
            {
                Debug.LogWarning("[InventorySetupUtility] InventoryView type not found. Please add it manually after compilation.");
            }

            // 6. シーンを保存
            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            if (saved)
            {
                Debug.Log($"[InventorySetupUtility] Inventory scene created successfully at {ScenePath}");
                Debug.Log("[InventorySetupUtility] Setup complete!");
            }
            else
            {
                Debug.LogError("[InventorySetupUtility] Failed to save scene.");
            }
        }

        /// <summary>
        /// 必要なアセットが存在するか確認します
        /// </summary>
        private static bool ValidateAssets()
        {
            bool isValid = true;

            // UXML確認
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            if (uxml == null)
            {
                Debug.LogError($"[InventorySetupUtility] UXML not found at {UxmlPath}");
                isValid = false;
            }

            // USS確認（オプション）
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath);
            if (uss == null)
            {
                Debug.LogWarning($"[InventorySetupUtility] USS not found at {UssPath} (optional)");
            }

            // PanelSettings確認
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                Debug.LogError($"[InventorySetupUtility] PanelSettings not found at {PanelSettingsPath}");
                isValid = false;
            }

            return isValid;
        }
    }
}
