using UnityEngine;

/// <summary>
/// Serializable data model for a single modular prefab.
/// Stores the prefab reference together with the metadata required by
/// placement, categorization and compatibility systems.
/// </summary>
[System.Serializable]
public class ModuleDefinition
{
    // Stable module identity used by placed scene instances.
    [SerializeField, HideInInspector]
    private string _moduleId;

    [SerializeField]
    private string _displayName;

    [SerializeField]
    private GameObject _prefab;

    // References to kit-level definitions are stored as IDs rather than copies.
    [SerializeField, HideInInspector]
    private string _categoryId = string.Empty;

    [SerializeField, HideInInspector]
    private string _compatibilityGroupId = string.Empty;

    [SerializeField, HideInInspector]
    private string _placementTypeId = string.Empty;

    // Logical size occupied by the module in grid cells.
    [SerializeField]
    private Vector2Int _moduleSizeInGridCells = new Vector2Int(1, 1);

    // Per-module XZ offset applied after regular grid snapping.
    // Vector2.x maps to world X and Vector2.y maps to world Z.
    [SerializeField]
    private Vector2 _snapOffset;

    [SerializeField]
    private int _floorSpan = 1;

    public string ModuleId => _moduleId;
    public string DisplayName => _displayName;
    public GameObject Prefab => _prefab;
    public string CategoryId => _categoryId;
    public string CompatibilityGroupId => _compatibilityGroupId;
    public string PlacementTypeId => _placementTypeId;
    public Vector2Int Footprint => _moduleSizeInGridCells;
    public Vector2 SnapOffset => _snapOffset;
    public int FloorSpan => _floorSpan;

    /// <summary>
    /// Creates a module definition from a prefab and its initial category.
    /// </summary>
    public ModuleDefinition(
        string moduleId,
        string displayName,
        GameObject prefab,
        string categoryId)
    {
        _moduleId = moduleId;
        _displayName = displayName;
        _prefab = prefab;
        _categoryId = categoryId ?? string.Empty;
    }

    /// <summary>
    /// Assigns this module to a kit category.
    /// </summary>
    public void SetCategory(string categoryId)
    {
        _categoryId = categoryId ?? string.Empty;
    }

    /// <summary>
    /// Assigns the compatibility group used by module replacement workflows.
    /// </summary>
    public void SetCompatibilityGroup(string compatibilityGroupId)
    {
        _compatibilityGroupId = compatibilityGroupId ?? string.Empty;
    }

    /// <summary>
    /// Assigns the placement type used by placement and overlap rules.
    /// </summary>
    public void SetPlacementType(string placementTypeId)
    {
        _placementTypeId = placementTypeId ?? string.Empty;
    }

    /// <summary>
    /// Updates the logical grid footprint.
    /// Returns false when the value is invalid or unchanged.
    /// </summary>
    public bool SetModuleSizeInGridCells(Vector2Int moduleSizeInGridCells)
    {
        if (moduleSizeInGridCells.x <= 0 || moduleSizeInGridCells.y <= 0)
        {
            return false;
        }

        if (_moduleSizeInGridCells == moduleSizeInGridCells)
        {
            return false;
        }

        _moduleSizeInGridCells = moduleSizeInGridCells;
        return true;
    }

    /// <summary>
    /// Updates how many floors the module logically spans.
    /// Returns false when the value is invalid or unchanged.
    /// </summary>
    public bool SetFloorSpan(int floorSpan)
    {
        if (floorSpan <= 0 || _floorSpan == floorSpan)
        {
            return false;
        }

        _floorSpan = floorSpan;
        return true;
    }

    /// <summary>
    /// Sets the module-specific XZ offset applied after grid snapping.
    /// </summary>
    public void SetSnapOffset(Vector2 snapOffset)
    {
        _snapOffset = snapOffset;
    }
}
