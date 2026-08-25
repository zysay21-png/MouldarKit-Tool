using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModularKitDefinition))]
public class ModularKitDefinitionEditor : Editor
{
    private string _newCategoryName = string.Empty;
    private string _newCompatibilityGroupName = string.Empty;
    private string _newPlacementTypeName = string.Empty;
    private GameObject _prefabToAdd;
    private int _selectedCategoryIndex;
    private List<string> _validationErrors;
    private string _feedbackMessage;
    private MessageType _feedbackType = MessageType.Info;

    public override void OnInspectorGUI()
    {
        ModularKitDefinition kit = (ModularKitDefinition)target;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            _validationErrors = null;
        }

        DrawFeedback();
        DrawCategories(kit);
        DrawCompatibilityGroups(kit);
        DrawPlacementTypes(kit);
        DrawModuleSetup(kit);
        DrawModules(kit);
        DrawValidation(kit);
    }

    private void DrawCategories(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Categories", EditorStyles.boldLabel);

        IReadOnlyList<CategoryDefinition> categories = kit.Categories;

        if (categories == null || categories.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "No categories. Modules can still be added as Unassigned.",
                MessageType.Info
            );
        }
        else
        {
            for (int i = 0; i < categories.Count; i++)
            {
                CategoryDefinition category = categories[i];

                if (category == null)
                {
                    EditorGUILayout.HelpBox(
                        $"Category at index {i} is missing.",
                        MessageType.Error
                    );
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(category.DisplayName);

                bool removeClicked = GUILayout.Button(
                    "Remove",
                    GUILayout.Width(70f)
                );

                EditorGUILayout.EndHorizontal();

                if (removeClicked)
                {
                    Undo.RecordObject(kit, "Remove Category");

                    if (kit.RemoveCategory(category.CategoryId))
                    {
                        SaveKit(kit);
                        _selectedCategoryIndex = 0;
                        SetFeedback(
                            $"Category '{category.DisplayName}' was removed. " +
                            "Its modules are now Unassigned.",
                            MessageType.Info
                        );
                    }

                    break;
                }
            }
        }

        _newCategoryName = EditorGUILayout.TextField(
            "New Category",
            _newCategoryName
        );

        using (new EditorGUI.DisabledScope(
                   string.IsNullOrWhiteSpace(_newCategoryName)))
        {
            if (GUILayout.Button("Add Category"))
            {
                string categoryName = _newCategoryName.Trim();
                Undo.RecordObject(kit, "Add Category");

                if (kit.AddCategory(categoryName))
                {
                    SaveKit(kit);
                    _newCategoryName = string.Empty;
                    SetFeedback(
                        $"Category '{categoryName}' was added.",
                        MessageType.Info
                    );
                }
                else
                {
                    SetFeedback(
                        "The category could not be added. Check for a duplicate name.",
                        MessageType.Warning
                    );
                }
            }
        }
    }

    private void DrawCompatibilityGroups(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Compatibility Groups", EditorStyles.boldLabel);

        IReadOnlyList<CompatibilityGroupDefinition> groups =
            kit.CompatibilityGroups;

        if (groups != null)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                CompatibilityGroupDefinition group = groups[i];

                if (group == null)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(group.DisplayName);
                bool removeClicked = GUILayout.Button(
                    "Remove",
                    GUILayout.Width(70f)
                );
                EditorGUILayout.EndHorizontal();

                if (removeClicked)
                {
                    Undo.RecordObject(kit, "Remove Compatibility Group");

                    if (kit.RemoveCompatibilityGroup(group.GroupId))
                    {
                        SaveKit(kit);
                        SetFeedback(
                            $"Compatibility group '{group.DisplayName}' was removed.",
                            MessageType.Info
                        );
                    }

                    break;
                }
            }
        }

        _newCompatibilityGroupName = EditorGUILayout.TextField(
            "New Group",
            _newCompatibilityGroupName
        );

        using (new EditorGUI.DisabledScope(
                   string.IsNullOrWhiteSpace(_newCompatibilityGroupName)))
        {
            if (GUILayout.Button("Add Compatibility Group"))
            {
                string groupName = _newCompatibilityGroupName.Trim();
                Undo.RecordObject(kit, "Add Compatibility Group");

                if (kit.AddCompatibilityGroup(groupName))
                {
                    SaveKit(kit);
                    _newCompatibilityGroupName = string.Empty;
                    SetFeedback(
                        $"Compatibility group '{groupName}' was added.",
                        MessageType.Info
                    );
                }
                else
                {
                    SetFeedback(
                        "The compatibility group could not be added.",
                        MessageType.Warning
                    );
                }
            }
        }
    }

    private void DrawPlacementTypes(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Placement Types", EditorStyles.boldLabel);

        IReadOnlyList<PlacementTypeDefinition> placementTypes =
            kit.PlacementTypes;

        if (placementTypes != null)
        {
            for (int i = 0; i < placementTypes.Count; i++)
            {
                PlacementTypeDefinition placementType = placementTypes[i];

                if (placementType == null)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(placementType.DisplayName);
                bool removeClicked = GUILayout.Button(
                    "Remove",
                    GUILayout.Width(70f)
                );
                EditorGUILayout.EndHorizontal();

                if (removeClicked)
                {
                    Undo.RecordObject(kit, "Remove Placement Type");

                    if (kit.RemovePlacementType(placementType.PlacementTypeId))
                    {
                        SaveKit(kit);
                        SetFeedback(
                            $"Placement type '{placementType.DisplayName}' was removed.",
                            MessageType.Info
                        );
                    }

                    break;
                }
            }
        }

        _newPlacementTypeName = EditorGUILayout.TextField(
            "New Placement Type",
            _newPlacementTypeName
        );

        using (new EditorGUI.DisabledScope(
                   string.IsNullOrWhiteSpace(_newPlacementTypeName)))
        {
            if (GUILayout.Button("Add Placement Type"))
            {
                string typeName = _newPlacementTypeName.Trim();
                Undo.RecordObject(kit, "Add Placement Type");

                if (kit.AddPlacementType(typeName))
                {
                    SaveKit(kit);
                    _newPlacementTypeName = string.Empty;
                    SetFeedback(
                        $"Placement type '{typeName}' was added.",
                        MessageType.Info
                    );
                }
                else
                {
                    SetFeedback(
                        "The placement type could not be added.",
                        MessageType.Warning
                    );
                }
            }
        }
    }

    private void DrawModuleSetup(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add Module", EditorStyles.boldLabel);

        string[] categoryOptions = BuildCategoryOptions(kit);
        _selectedCategoryIndex = Mathf.Clamp(
            _selectedCategoryIndex,
            0,
            categoryOptions.Length - 1
        );

        _selectedCategoryIndex = EditorGUILayout.Popup(
            "Category",
            _selectedCategoryIndex,
            categoryOptions
        );

        _prefabToAdd = (GameObject)EditorGUILayout.ObjectField(
            "Prefab",
            _prefabToAdd,
            typeof(GameObject),
            false
        );

        using (new EditorGUI.DisabledScope(_prefabToAdd == null))
        {
            if (GUILayout.Button("Add Module"))
            {
                string categoryId = GetCategoryId(
                    kit,
                    _selectedCategoryIndex
                );

                string categoryName = categoryOptions[_selectedCategoryIndex];
                string prefabName = _prefabToAdd.name;

                Undo.RecordObject(kit, "Add Module");

                if (kit.AddModule(_prefabToAdd, categoryId))
                {
                    SaveKit(kit);
                    _prefabToAdd = null;
                    SetFeedback(
                        $"'{prefabName}' was added to '{categoryName}'.",
                        MessageType.Info
                    );
                }
                else
                {
                    SetFeedback(
                        "The module could not be added. The prefab may already exist in this Kit.",
                        MessageType.Warning
                    );
                }
            }
        }
    }

    private void DrawModules(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);

        IReadOnlyList<ModuleDefinition> modules = kit.Modules;

        if (modules == null || modules.Count == 0)
        {
            EditorGUILayout.HelpBox("No modules have been added.", MessageType.Info);
            return;
        }

        for (int i = 0; i < modules.Count; i++)
        {
            ModuleDefinition module = modules[i];

            if (module == null)
            {
                EditorGUILayout.HelpBox(
                    $"Module at index {i} is missing.",
                    MessageType.Error
                );
                continue;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(module.DisplayName, EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(
                    "Prefab",
                    module.Prefab,
                    typeof(GameObject),
                    false
                );
            }

            DrawModuleCategory(kit, module);
            DrawModuleCompatibilityGroup(kit, module);
            DrawModulePlacementType(kit, module);
            DrawModuleDimensions(kit, module);

            bool removeClicked = GUILayout.Button("Remove Module");
            EditorGUILayout.EndVertical();

            if (removeClicked)
            {
                Undo.RecordObject(kit, "Remove Module");

                if (kit.RemoveModule(module.ModuleId))
                {
                    SaveKit(kit);
                    SetFeedback(
                        $"Module '{module.DisplayName}' was removed.",
                        MessageType.Info
                    );
                }

                break;
            }
        }
    }

    private void DrawModuleCategory(
        ModularKitDefinition kit,
        ModuleDefinition module)
    {
        string[] options = BuildCategoryOptions(kit);
        int currentIndex = FindCategoryIndex(kit, module.CategoryId);
        int newIndex = EditorGUILayout.Popup("Category", currentIndex, options);

        if (newIndex == currentIndex)
        {
            return;
        }

        Undo.RecordObject(kit, "Assign Module Category");

        if (kit.AssignModuleCategory(
                module.ModuleId,
                GetCategoryId(kit, newIndex)))
        {
            SaveKit(kit);
            SetFeedback(
                $"'{module.DisplayName}' was moved to '{options[newIndex]}'.",
                MessageType.Info
            );
        }
    }

    private void DrawModuleCompatibilityGroup(
        ModularKitDefinition kit,
        ModuleDefinition module)
    {
        string[] options = BuildCompatibilityGroupOptions(kit);
        int currentIndex = FindCompatibilityGroupIndex(
            kit,
            module.CompatibilityGroupId
        );
        int newIndex = EditorGUILayout.Popup(
            "Compatibility Group",
            currentIndex,
            options
        );

        if (newIndex == currentIndex)
        {
            return;
        }

        Undo.RecordObject(kit, "Assign Compatibility Group");

        if (kit.AssignModuleCompatibilityGroup(
                module.ModuleId,
                GetCompatibilityGroupId(kit, newIndex)))
        {
            SaveKit(kit);
        }
    }

    private void DrawModulePlacementType(
        ModularKitDefinition kit,
        ModuleDefinition module)
    {
        string[] options = BuildPlacementTypeOptions(kit);
        int currentIndex = FindPlacementTypeIndex(kit, module.PlacementTypeId);
        int newIndex = EditorGUILayout.Popup(
            "Placement Type",
            currentIndex,
            options
        );

        if (newIndex == currentIndex)
        {
            return;
        }

        Undo.RecordObject(kit, "Assign Placement Type");

        if (kit.AssignModulePlacementType(
                module.ModuleId,
                GetPlacementTypeId(kit, newIndex)))
        {
            SaveKit(kit);
        }
    }

    private void DrawModuleDimensions(
        ModularKitDefinition kit,
        ModuleDefinition module)
    {
        Vector2Int newFootprint = EditorGUILayout.Vector2IntField(
            "Grid Size",
            module.Footprint
        );

        if (newFootprint != module.Footprint)
        {
            Undo.RecordObject(kit, "Change Module Grid Size");

            if (module.SetModuleSizeInGridCells(newFootprint))
            {
                SaveKit(kit);
            }
            else
            {
                SetFeedback(
                    "Module grid size must be greater than zero.",
                    MessageType.Warning
                );
            }
        }

        int newFloorSpan = EditorGUILayout.IntField(
            "Floor Span",
            module.FloorSpan
        );

        if (newFloorSpan != module.FloorSpan)
        {
            Undo.RecordObject(kit, "Change Module Floor Span");

            if (module.SetFloorSpan(newFloorSpan))
            {
                SaveKit(kit);
            }
            else
            {
                SetFeedback(
                    "Floor span must be greater than zero.",
                    MessageType.Warning
                );
            }
        }
    }

    private void DrawValidation(ModularKitDefinition kit)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

        if (GUILayout.Button("Validate Kit"))
        {
            _validationErrors = kit.GetValidationErrors();
        }

        if (_validationErrors == null)
        {
            return;
        }

        if (_validationErrors.Count == 0)
        {
            EditorGUILayout.HelpBox("Kit is valid.", MessageType.Info);
            return;
        }

        for (int i = 0; i < _validationErrors.Count; i++)
        {
            EditorGUILayout.HelpBox(
                _validationErrors[i],
                MessageType.Error
            );
        }
    }

    private void DrawFeedback()
    {
        if (string.IsNullOrWhiteSpace(_feedbackMessage))
        {
            return;
        }

        EditorGUILayout.HelpBox(_feedbackMessage, _feedbackType);
    }

    private void SetFeedback(string message, MessageType messageType)
    {
        _feedbackMessage = message;
        _feedbackType = messageType;
    }

    private void SaveKit(ModularKitDefinition kit)
    {
        EditorUtility.SetDirty(kit);
        AssetDatabase.SaveAssets();
        _validationErrors = null;
    }

    private static string[] BuildCategoryOptions(ModularKitDefinition kit)
    {
        int count = kit.Categories == null ? 0 : kit.Categories.Count;
        string[] options = new string[count + 1];
        options[0] = "Unassigned";

        for (int i = 0; i < count; i++)
        {
            CategoryDefinition category = kit.Categories[i];
            options[i + 1] = category == null
                ? "Missing Category"
                : category.DisplayName;
        }

        return options;
    }

    private static string[] BuildCompatibilityGroupOptions(
        ModularKitDefinition kit)
    {
        int count = kit.CompatibilityGroups == null
            ? 0
            : kit.CompatibilityGroups.Count;
        string[] options = new string[count + 1];
        options[0] = "None";

        for (int i = 0; i < count; i++)
        {
            CompatibilityGroupDefinition group = kit.CompatibilityGroups[i];
            options[i + 1] = group == null ? "Missing Group" : group.DisplayName;
        }

        return options;
    }

    private static string[] BuildPlacementTypeOptions(ModularKitDefinition kit)
    {
        int count = kit.PlacementTypes == null ? 0 : kit.PlacementTypes.Count;
        string[] options = new string[count + 1];
        options[0] = "None";

        for (int i = 0; i < count; i++)
        {
            PlacementTypeDefinition placementType = kit.PlacementTypes[i];
            options[i + 1] = placementType == null
                ? "Missing Type"
                : placementType.DisplayName;
        }

        return options;
    }

    private static int FindCategoryIndex(
        ModularKitDefinition kit,
        string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId) || kit.Categories == null)
        {
            return 0;
        }

        for (int i = 0; i < kit.Categories.Count; i++)
        {
            CategoryDefinition category = kit.Categories[i];

            if (category != null && category.CategoryId == categoryId)
            {
                return i + 1;
            }
        }

        return 0;
    }

    private static int FindCompatibilityGroupIndex(
        ModularKitDefinition kit,
        string groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId) ||
            kit.CompatibilityGroups == null)
        {
            return 0;
        }

        for (int i = 0; i < kit.CompatibilityGroups.Count; i++)
        {
            CompatibilityGroupDefinition group = kit.CompatibilityGroups[i];

            if (group != null && group.GroupId == groupId)
            {
                return i + 1;
            }
        }

        return 0;
    }

    private static int FindPlacementTypeIndex(
        ModularKitDefinition kit,
        string placementTypeId)
    {
        if (string.IsNullOrWhiteSpace(placementTypeId) ||
            kit.PlacementTypes == null)
        {
            return 0;
        }

        for (int i = 0; i < kit.PlacementTypes.Count; i++)
        {
            PlacementTypeDefinition placementType = kit.PlacementTypes[i];

            if (placementType != null &&
                placementType.PlacementTypeId == placementTypeId)
            {
                return i + 1;
            }
        }

        return 0;
    }

    private static string GetCategoryId(ModularKitDefinition kit, int index)
    {
        if (index <= 0 || kit.Categories == null ||
            index > kit.Categories.Count)
        {
            return string.Empty;
        }

        CategoryDefinition category = kit.Categories[index - 1];
        return category == null ? string.Empty : category.CategoryId;
    }

    private static string GetCompatibilityGroupId(
        ModularKitDefinition kit,
        int index)
    {
        if (index <= 0 || kit.CompatibilityGroups == null ||
            index > kit.CompatibilityGroups.Count)
        {
            return string.Empty;
        }

        CompatibilityGroupDefinition group =
            kit.CompatibilityGroups[index - 1];
        return group == null ? string.Empty : group.GroupId;
    }

    private static string GetPlacementTypeId(
        ModularKitDefinition kit,
        int index)
    {
        if (index <= 0 || kit.PlacementTypes == null ||
            index > kit.PlacementTypes.Count)
        {
            return string.Empty;
        }

        PlacementTypeDefinition placementType = kit.PlacementTypes[index - 1];
        return placementType == null
            ? string.Empty
            : placementType.PlacementTypeId;
    }
}
