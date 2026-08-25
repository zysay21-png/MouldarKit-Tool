using System;
using System.Collections.Generic;
using UnityEngine;

public class ModularKitDefinition : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string _kitId;

    [SerializeField]
    private string _displayName;

    [SerializeField]
    private Vector2 _gridCellSize = Vector2.one;

    [SerializeField]
    private float _floorHeight = 3f;

    [SerializeField]
    private float _rotationSnap = 90f;

    [SerializeField, HideInInspector]
    private List<CategoryDefinition> _categories = new List<CategoryDefinition>();

    [SerializeField, HideInInspector]
    private List<CompatibilityGroupDefinition> _compatibilityGroups =
        new List<CompatibilityGroupDefinition>();

    [SerializeField, HideInInspector]
    private List<PlacementTypeDefinition> _placementTypes =
        new List<PlacementTypeDefinition>();

    [SerializeField, HideInInspector]
    private List<ModuleDefinition> _modules = new List<ModuleDefinition>();

    public string KitId => _kitId;
    public string DisplayName => _displayName;
    public Vector2 GridCellSize => _gridCellSize;
    public float FloorHeight => _floorHeight;
    public float RotationSnap => _rotationSnap;
    public IReadOnlyList<CategoryDefinition> Categories => _categories;
    public IReadOnlyList<CompatibilityGroupDefinition> CompatibilityGroups =>
        _compatibilityGroups;
    public IReadOnlyList<PlacementTypeDefinition> PlacementTypes => _placementTypes;
    public IReadOnlyList<ModuleDefinition> Modules => _modules;

    public bool Initialize(string displayName)
    {
        if (!string.IsNullOrEmpty(_kitId) ||
            string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        _displayName = displayName.Trim();
        _kitId = Guid.NewGuid().ToString("N");

        EnsureCollectionsExist();

        if (_categories.Count == 0)
        {
            _categories.Add(new CategoryDefinition(
                Guid.NewGuid().ToString("N"),
                "Uncategorized"
            ));
        }

        return true;
    }

    public bool AddCategory(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        EnsureCollectionsExist();
        displayName = displayName.Trim();

        for (int i = 0; i < _categories.Count; i++)
        {
            CategoryDefinition category = _categories[i];

            if (category != null &&
                string.Equals(
                    category.DisplayName,
                    displayName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        _categories.Add(new CategoryDefinition(
            Guid.NewGuid().ToString("N"),
            displayName
        ));

        return true;
    }

    public bool RemoveCategory(string categoryId)
    {
        CategoryDefinition category = FindCategory(categoryId);

        if (category == null)
        {
            return false;
        }

        _categories.Remove(category);

        if (_modules != null)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                ModuleDefinition module = _modules[i];

                if (module != null && module.CategoryId == categoryId)
                {
                    module.SetCategory(string.Empty);
                }
            }
        }

        return true;
    }

    public bool AddCompatibilityGroup(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        EnsureCollectionsExist();
        displayName = displayName.Trim();

        for (int i = 0; i < _compatibilityGroups.Count; i++)
        {
            CompatibilityGroupDefinition group = _compatibilityGroups[i];

            if (group != null &&
                string.Equals(
                    group.DisplayName,
                    displayName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        _compatibilityGroups.Add(new CompatibilityGroupDefinition(
            Guid.NewGuid().ToString("N"),
            displayName
        ));

        return true;
    }

    public bool RemoveCompatibilityGroup(string groupId)
    {
        CompatibilityGroupDefinition group = FindCompatibilityGroup(groupId);

        if (group == null)
        {
            return false;
        }

        _compatibilityGroups.Remove(group);

        if (_modules != null)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                ModuleDefinition module = _modules[i];

                if (module != null && module.CompatibilityGroupId == groupId)
                {
                    module.SetCompatibilityGroup(string.Empty);
                }
            }
        }

        return true;
    }

    public bool AddPlacementType(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        EnsureCollectionsExist();
        displayName = displayName.Trim();

        for (int i = 0; i < _placementTypes.Count; i++)
        {
            PlacementTypeDefinition placementType = _placementTypes[i];

            if (placementType != null &&
                string.Equals(
                    placementType.DisplayName,
                    displayName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        _placementTypes.Add(new PlacementTypeDefinition(
            Guid.NewGuid().ToString("N"),
            displayName
        ));

        return true;
    }

    public bool RemovePlacementType(string placementTypeId)
    {
        PlacementTypeDefinition placementType = FindPlacementType(placementTypeId);

        if (placementType == null)
        {
            return false;
        }

        _placementTypes.Remove(placementType);

        if (_modules != null)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                ModuleDefinition module = _modules[i];

                if (module != null && module.PlacementTypeId == placementTypeId)
                {
                    module.SetPlacementType(string.Empty);
                }
            }
        }

        return true;
    }

    public bool AddModule(GameObject prefab, string categoryId)
    {
        if (prefab == null)
        {
            return false;
        }

        EnsureCollectionsExist();
        categoryId = categoryId ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(categoryId) &&
            FindCategory(categoryId) == null)
        {
            return false;
        }

        for (int i = 0; i < _modules.Count; i++)
        {
            ModuleDefinition module = _modules[i];

            if (module != null && module.Prefab == prefab)
            {
                return false;
            }
        }

        _modules.Add(new ModuleDefinition(
            Guid.NewGuid().ToString("N"),
            prefab.name,
            prefab,
            categoryId
        ));

        return true;
    }

    public bool AddModule(GameObject prefab)
    {
        return AddModule(prefab, string.Empty);
    }

    public bool RemoveModule(string moduleId)
    {
        ModuleDefinition module = FindModule(moduleId);

        if (module == null)
        {
            return false;
        }

        return _modules.Remove(module);
    }

    public bool AssignModuleCategory(string moduleId, string categoryId)
    {
        ModuleDefinition module = FindModule(moduleId);

        if (module == null)
        {
            return false;
        }

        categoryId = categoryId ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(categoryId) &&
            FindCategory(categoryId) == null)
        {
            return false;
        }

        if (module.CategoryId == categoryId)
        {
            return false;
        }

        module.SetCategory(categoryId);
        return true;
    }

    public bool AssignModuleCompatibilityGroup(string moduleId, string groupId)
    {
        ModuleDefinition module = FindModule(moduleId);

        if (module == null)
        {
            return false;
        }

        groupId = groupId ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(groupId) &&
            FindCompatibilityGroup(groupId) == null)
        {
            return false;
        }

        if (module.CompatibilityGroupId == groupId)
        {
            return false;
        }

        module.SetCompatibilityGroup(groupId);
        return true;
    }

    public bool AssignModulePlacementType(string moduleId, string placementTypeId)
    {
        ModuleDefinition module = FindModule(moduleId);

        if (module == null)
        {
            return false;
        }

        placementTypeId = placementTypeId ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(placementTypeId) &&
            FindPlacementType(placementTypeId) == null)
        {
            return false;
        }

        if (module.PlacementTypeId == placementTypeId)
        {
            return false;
        }

        module.SetPlacementType(placementTypeId);
        return true;
    }

    public string GetCategoryDisplayName(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            return "Unassigned";
        }

        CategoryDefinition category = FindCategory(categoryId);
        return category == null ? "Missing Category" : category.DisplayName;
    }

    public string GetCompatibilityGroupDisplayName(string groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            return "None";
        }

        CompatibilityGroupDefinition group = FindCompatibilityGroup(groupId);
        return group == null ? "Missing Group" : group.DisplayName;
    }

    public string GetPlacementTypeDisplayName(string placementTypeId)
    {
        if (string.IsNullOrWhiteSpace(placementTypeId))
        {
            return "None";
        }

        PlacementTypeDefinition placementType = FindPlacementType(placementTypeId);
        return placementType == null ? "Missing Type" : placementType.DisplayName;
    }

    public bool SetGridCellSize(Vector2 gridCellSize)
    {
        if (gridCellSize.x <= 0 || gridCellSize.y <= 0 ||
            _gridCellSize == gridCellSize)
        {
            return false;
        }

        _gridCellSize = gridCellSize;
        return true;
    }

    public bool SetFloorHeight(float floorHeight)
    {
        if (floorHeight <= 0 || Mathf.Approximately(_floorHeight, floorHeight))
        {
            return false;
        }

        _floorHeight = floorHeight;
        return true;
    }

    public bool SetRotationSnap(float rotationSnap)
    {
        if (!IsValidRotationSnap(rotationSnap) ||
            Mathf.Approximately(_rotationSnap, rotationSnap))
        {
            return false;
        }

        _rotationSnap = rotationSnap;
        return true;
    }

    public List<string> GetValidationErrors()
    {
        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_kitId))
        {
            errors.Add("Kit ID is missing.");
        }

        if (string.IsNullOrWhiteSpace(_displayName))
        {
            errors.Add("Display name is missing.");
        }

        if (_gridCellSize.x <= 0 || _gridCellSize.y <= 0)
        {
            errors.Add("Grid cell size must be greater than zero.");
        }

        if (_floorHeight <= 0)
        {
            errors.Add("Floor height must be greater than zero.");
        }

        if (!IsValidRotationSnap(_rotationSnap))
        {
            errors.Add("Rotation snap must divide 360 into whole steps.");
        }

        ValidateCategories(errors);
        ValidateCompatibilityGroups(errors);
        ValidatePlacementTypes(errors);
        ValidateModules(errors);

        return errors;
    }

    private void ValidateCategories(List<string> errors)
    {
        if (_categories == null)
        {
            errors.Add("Categories list is missing.");
            return;
        }

        HashSet<string> ids = new HashSet<string>();
        HashSet<string> names =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < _categories.Count; i++)
        {
            CategoryDefinition category = _categories[i];

            if (category == null)
            {
                errors.Add($"Category at index {i} is missing.");
                continue;
            }

            ValidateIdAndName(
                "Category",
                i,
                category.CategoryId,
                category.DisplayName,
                ids,
                names,
                errors
            );
        }
    }

    private void ValidateCompatibilityGroups(List<string> errors)
    {
        if (_compatibilityGroups == null)
        {
            errors.Add("Compatibility groups list is missing.");
            return;
        }

        HashSet<string> ids = new HashSet<string>();
        HashSet<string> names =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < _compatibilityGroups.Count; i++)
        {
            CompatibilityGroupDefinition group = _compatibilityGroups[i];

            if (group == null)
            {
                errors.Add($"Compatibility group at index {i} is missing.");
                continue;
            }

            ValidateIdAndName(
                "Compatibility group",
                i,
                group.GroupId,
                group.DisplayName,
                ids,
                names,
                errors
            );
        }
    }

    private void ValidatePlacementTypes(List<string> errors)
    {
        if (_placementTypes == null)
        {
            errors.Add("Placement types list is missing.");
            return;
        }

        HashSet<string> ids = new HashSet<string>();
        HashSet<string> names =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < _placementTypes.Count; i++)
        {
            PlacementTypeDefinition placementType = _placementTypes[i];

            if (placementType == null)
            {
                errors.Add($"Placement type at index {i} is missing.");
                continue;
            }

            ValidateIdAndName(
                "Placement type",
                i,
                placementType.PlacementTypeId,
                placementType.DisplayName,
                ids,
                names,
                errors
            );
        }
    }

    private void ValidateModules(List<string> errors)
    {
        if (_modules == null)
        {
            errors.Add("Modules list is missing.");
            return;
        }

        HashSet<string> moduleIds = new HashSet<string>();
        HashSet<string> moduleNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        HashSet<GameObject> modulePrefabs = new HashSet<GameObject>();

        HashSet<string> validCategoryIds = GetValidCategoryIds();
        HashSet<string> validGroupIds = GetValidCompatibilityGroupIds();
        HashSet<string> validPlacementTypeIds = GetValidPlacementTypeIds();

        for (int i = 0; i < _modules.Count; i++)
        {
            ModuleDefinition module = _modules[i];

            if (module == null)
            {
                errors.Add($"Module at index {i} is missing.");
                continue;
            }

            ValidateIdAndName(
                "Module",
                i,
                module.ModuleId,
                module.DisplayName,
                moduleIds,
                moduleNames,
                errors
            );

            if (module.Prefab == null)
            {
                errors.Add($"Module '{module.DisplayName}' has a missing prefab.");
            }
            else if (!modulePrefabs.Add(module.Prefab))
            {
                errors.Add(
                    $"Prefab '{module.Prefab.name}' is used by more than one module."
                );
            }

            if (module.Footprint.x <= 0 || module.Footprint.y <= 0)
            {
                errors.Add(
                    $"Module '{module.DisplayName}' must have a footprint greater than zero."
                );
            }

            if (module.FloorSpan <= 0)
            {
                errors.Add(
                    $"Module '{module.DisplayName}' must have a floor span greater than zero."
                );
            }

            if (!string.IsNullOrWhiteSpace(module.CategoryId) &&
                !validCategoryIds.Contains(module.CategoryId))
            {
                errors.Add(
                    $"Module '{module.DisplayName}' references a category that does not exist."
                );
            }

            if (!string.IsNullOrWhiteSpace(module.CompatibilityGroupId) &&
                !validGroupIds.Contains(module.CompatibilityGroupId))
            {
                errors.Add(
                    $"Module '{module.DisplayName}' references a compatibility group that does not exist."
                );
            }

            if (!string.IsNullOrWhiteSpace(module.PlacementTypeId) &&
                !validPlacementTypeIds.Contains(module.PlacementTypeId))
            {
                errors.Add(
                    $"Module '{module.DisplayName}' references a placement type that does not exist."
                );
            }
        }
    }

    private static void ValidateIdAndName(
        string itemType,
        int index,
        string id,
        string displayName,
        HashSet<string> ids,
        HashSet<string> names,
        List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            errors.Add($"{itemType} at index {index} has a missing ID.");
        }
        else if (!ids.Add(id))
        {
            errors.Add($"{itemType} ID '{id}' is duplicated.");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            errors.Add($"{itemType} at index {index} has a missing display name.");
        }
        else if (!names.Add(displayName))
        {
            errors.Add($"{itemType} name '{displayName}' is duplicated.");
        }
    }

    private CategoryDefinition FindCategory(string categoryId)
    {
        if (_categories == null || string.IsNullOrWhiteSpace(categoryId))
        {
            return null;
        }

        for (int i = 0; i < _categories.Count; i++)
        {
            CategoryDefinition category = _categories[i];

            if (category != null && category.CategoryId == categoryId)
            {
                return category;
            }
        }

        return null;
    }

    private CompatibilityGroupDefinition FindCompatibilityGroup(string groupId)
    {
        if (_compatibilityGroups == null || string.IsNullOrWhiteSpace(groupId))
        {
            return null;
        }

        for (int i = 0; i < _compatibilityGroups.Count; i++)
        {
            CompatibilityGroupDefinition group = _compatibilityGroups[i];

            if (group != null && group.GroupId == groupId)
            {
                return group;
            }
        }

        return null;
    }

    private PlacementTypeDefinition FindPlacementType(string placementTypeId)
    {
        if (_placementTypes == null || string.IsNullOrWhiteSpace(placementTypeId))
        {
            return null;
        }

        for (int i = 0; i < _placementTypes.Count; i++)
        {
            PlacementTypeDefinition placementType = _placementTypes[i];

            if (placementType != null &&
                placementType.PlacementTypeId == placementTypeId)
            {
                return placementType;
            }
        }

        return null;
    }

    private ModuleDefinition FindModule(string moduleId)
    {
        if (_modules == null || string.IsNullOrWhiteSpace(moduleId))
        {
            return null;
        }

        for (int i = 0; i < _modules.Count; i++)
        {
            ModuleDefinition module = _modules[i];

            if (module != null && module.ModuleId == moduleId)
            {
                return module;
            }
        }

        return null;
    }

    private HashSet<string> GetValidCategoryIds()
    {
        HashSet<string> ids = new HashSet<string>();

        if (_categories == null)
        {
            return ids;
        }

        for (int i = 0; i < _categories.Count; i++)
        {
            CategoryDefinition category = _categories[i];

            if (category != null && !string.IsNullOrWhiteSpace(category.CategoryId))
            {
                ids.Add(category.CategoryId);
            }
        }

        return ids;
    }

    private HashSet<string> GetValidCompatibilityGroupIds()
    {
        HashSet<string> ids = new HashSet<string>();

        if (_compatibilityGroups == null)
        {
            return ids;
        }

        for (int i = 0; i < _compatibilityGroups.Count; i++)
        {
            CompatibilityGroupDefinition group = _compatibilityGroups[i];

            if (group != null && !string.IsNullOrWhiteSpace(group.GroupId))
            {
                ids.Add(group.GroupId);
            }
        }

        return ids;
    }

    private HashSet<string> GetValidPlacementTypeIds()
    {
        HashSet<string> ids = new HashSet<string>();

        if (_placementTypes == null)
        {
            return ids;
        }

        for (int i = 0; i < _placementTypes.Count; i++)
        {
            PlacementTypeDefinition placementType = _placementTypes[i];

            if (placementType != null &&
                !string.IsNullOrWhiteSpace(placementType.PlacementTypeId))
            {
                ids.Add(placementType.PlacementTypeId);
            }
        }

        return ids;
    }

    private static bool IsValidRotationSnap(float rotationSnap)
    {
        if (rotationSnap <= 0f || rotationSnap > 360f)
        {
            return false;
        }

        float stepCount = 360f / rotationSnap;
        return Mathf.Approximately(stepCount, Mathf.Round(stepCount));
    }

    private void EnsureCollectionsExist()
    {
        if (_categories == null)
        {
            _categories = new List<CategoryDefinition>();
        }

        if (_compatibilityGroups == null)
        {
            _compatibilityGroups = new List<CompatibilityGroupDefinition>();
        }

        if (_placementTypes == null)
        {
            _placementTypes = new List<PlacementTypeDefinition>();
        }

        if (_modules == null)
        {
            _modules = new List<ModuleDefinition>();
        }
    }
}
