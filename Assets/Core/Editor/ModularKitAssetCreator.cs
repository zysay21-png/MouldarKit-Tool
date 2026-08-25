using System.IO;
using UnityEditor;
using UnityEngine;

public static class ModularKitAssetCreator
{
    [MenuItem("Asset/Create/Modular Kit")]
    private static void CreateModularKitAsset()
    {
        string assetPath = EditorUtility.SaveFilePanelInProject(
            "Create Modular Kit",
            "New Modular Kit",
            "asset",
            "Choose where to save the Modular Kit asset."
        );

        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }

        string displayName = Path.GetFileNameWithoutExtension(assetPath);

        ModularKitDefinition newKit =
            ScriptableObject.CreateInstance<ModularKitDefinition>();

        if (!newKit.Initialize(displayName))
        {
            Object.DestroyImmediate(newKit);
            return;
        }

        AssetDatabase.CreateAsset(newKit, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = newKit;
        EditorGUIUtility.PingObject(newKit);
    }
}
