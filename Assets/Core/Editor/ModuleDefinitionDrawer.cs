using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ModuleDefinition))]
public class ModuleDefinitionDrawer : PropertyDrawer
{
    private const int VisibleFieldCount = 4;

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty displayNameProperty =
            property.FindPropertyRelative("_displayName");
        SerializedProperty prefabProperty =
            property.FindPropertyRelative("_prefab");
        SerializedProperty moduleSizeProperty =
            property.FindPropertyRelative("_moduleSizeInGridCells");
        SerializedProperty floorSpanProperty =
            property.FindPropertyRelative("_floorSpan");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect lineRect = new Rect(
            position.x,
            position.y,
            position.width,
            lineHeight
        );

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

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

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
