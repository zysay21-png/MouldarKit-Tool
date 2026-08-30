using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom PropertyDrawer for ModuleDefinition.
/// Displays the serialized module fields in a compact foldout layout.
/// </summary>
[CustomPropertyDrawer(typeof(ModuleDefinition))]
public class ModuleDefinitionDrawer : PropertyDrawer
{
    // Number of serialized fields drawn when the foldout is expanded.
    private const int VisibleFieldCount = 5;

    /// <summary>
    /// Draws the foldout header and serialized ModuleDefinition fields.
    /// </summary>
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Cache references to the serialized fields stored inside ModuleDefinition.
        SerializedProperty displayNameProperty =
            property.FindPropertyRelative("_displayName");
        SerializedProperty prefabProperty =
            property.FindPropertyRelative("_prefab");
        SerializedProperty moduleSizeProperty =
            property.FindPropertyRelative("_moduleSizeInGridCells");
        SerializedProperty floorSpanProperty =
            property.FindPropertyRelative("_floorSpan");
        SerializedProperty snapOffsetProperty =
            property.FindPropertyRelative("_snapOffset");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect lineRect = new Rect(
            position.x,
            position.y,
            position.width,
            lineHeight
        );

        // Use the module display name as the foldout title.
        string moduleTitle =
            string.IsNullOrWhiteSpace(displayNameProperty.stringValue)
                ? "Unnamed Module"
                : displayNameProperty.stringValue;

        property.isExpanded = EditorGUI.Foldout(
            lineRect,
            property.isExpanded,
            moduleTitle,
            true
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            DrawNextProperty(ref lineRect, displayNameProperty, lineHeight, spacing);
            DrawNextProperty(ref lineRect, prefabProperty, lineHeight, spacing);
            DrawNextProperty(ref lineRect, moduleSizeProperty, lineHeight, spacing);
            DrawNextProperty(ref lineRect, floorSpanProperty, lineHeight, spacing);
            DrawNextProperty(ref lineRect, snapOffsetProperty, lineHeight, spacing);

            // Temporary diagnostic kept exactly as in the current V1 source.
            Debug.Log(snapOffsetProperty == null
                ? "Snap Offset property NOT FOUND"
                : "Snap Offset property FOUND");

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    /// <summary>
    /// Reserves enough inspector height for the foldout and visible fields.
    /// </summary>
    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        int lineCount = property.isExpanded
            ? VisibleFieldCount + 1
            : 1;

        return lineCount * EditorGUIUtility.singleLineHeight
            + (lineCount - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    /// <summary>
    /// Moves the drawing rectangle to the next row and draws a property field.
    /// </summary>
    private static void DrawNextProperty(
        ref Rect lineRect,
        SerializedProperty property,
        float lineHeight,
        float spacing)
    {
        lineRect.y += lineHeight + spacing;
        EditorGUI.PropertyField(lineRect, property);
    }
}
