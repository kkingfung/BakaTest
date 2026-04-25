#nullable enable
using UnityEditor;
using UnityEngine;

namespace BakaTest.Editor
{
    /// <summary>
    /// Unity Editorでスクリプトの再コンパイルを強制するツール
    /// </summary>
    public static class ForceRecompile
    {
        [MenuItem("BakaTest/Utilities/Force Recompile All Scripts")]
        public static void RecompileAllScripts()
        {
            Debug.Log("[ForceRecompile] Forcing Unity to recompile all scripts...");

            // すべてのアセットを再インポート
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            // スクリプトのコンパイルをリクエスト
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();

            Debug.Log("[ForceRecompile] Recompile requested. Please wait for Unity to finish compiling.");
        }

        [MenuItem("BakaTest/Utilities/Reimport All Assets")]
        public static void ReimportAllAssets()
        {
            Debug.Log("[ForceRecompile] Reimporting all assets...");

            AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);

            Debug.Log("[ForceRecompile] Reimport complete.");
        }
    }
}
