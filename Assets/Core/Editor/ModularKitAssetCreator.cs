using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility responsible for creating new ModularKitDefinition assets
/// from the Unity Asset/Create menu.
/// </summary>
public static class ModularKitAssetCreator
{
    /// <summary>
    /// Opens a save dialog, creates a new modular kit asset,
    /// initializes it and selects it in the Project window.
    /// </summary>
    [MenuItem("Asset/Create/Modular Kit")]
    private static void CreateModularKitAsset()
    {
        // Let the user choose where the new kit asset should be stored.
        string assetPath = EditorUtility.SaveFilePanelInProject(
            "Create Modular Kit",
            "New Modular Kit",
            "asset",
            "Choose where to save the Modular Kit asset."
        );

        // Cancel asset creation if the save dialog was closed.
        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }

        // Use the chosen file name as the kit's initial display name.
        string displayName = Path.GetFileNameWithoutExtension(assetPath);

        ModularKitDefinition newKit =
            ScriptableObject.CreateInstance<ModularKitDefinition>();

        // Initialization creates the kit ID and default internal data.
        if (!newKit.Initialize(displayName))
        {
            Object.DestroyImmediate(newKit);
            return;
        }

        AssetDatabase.CreateAsset(newKit, assetPath);
        AssetDatabase.SaveAssets();

        // Highlight the newly created kit for immediate editing.
        Selection.activeObject = newKit;
        EditorGUIUtility.PingObject(newKit);
    }
}
