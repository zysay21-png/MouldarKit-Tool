using UnityEngine;

[System.Serializable]
public class ModuleDefinition
{
    [SerializeField, HideInInspector]
    private string _moduleId;

    [SerializeField]
    private string _displayName;

    [SerializeField]
    private GameObject _prefab;

    [SerializeField, HideInInspector]
    private string _categoryId = string.Empty;

    [SerializeField, HideInInspector]
    private string _compatibilityGroupId = string.Empty;

    [SerializeField, HideInInspector]
    private string _placementTypeId = string.Empty;

    [SerializeField]
    private Vector2Int _moduleSizeInGridCells = new Vector2Int(1, 1);

    [SerializeField]
    private Vector2 _snapOffset;

    public Vector2 SnapOffset => _snapOffset;


    [SerializeField]
    private int _floorSpan = 1;

    public string ModuleId => _moduleId;
    public string DisplayName => _displayName;
    public GameObject Prefab => _prefab;
    public string CategoryId => _categoryId;
    public string CompatibilityGroupId => _compatibilityGroupId;
    public string PlacementTypeId => _placementTypeId;
    public Vector2Int Footprint => _moduleSizeInGridCells;
    public int FloorSpan => _floorSpan;

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

    public void SetCategory(string categoryId)
    {
        _categoryId = categoryId ?? string.Empty;
    }

    public void SetCompatibilityGroup(string compatibilityGroupId)
    {
        _compatibilityGroupId = compatibilityGroupId ?? string.Empty;
    }

    public void SetPlacementType(string placementTypeId)
    {
        _placementTypeId = placementTypeId ?? string.Empty;
    }

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

    public bool SetFloorSpan(int floorSpan)
    {
        if (floorSpan <= 0 || _floorSpan == floorSpan)
        {
            return false;
        }

        _floorSpan = floorSpan;
        return true;
    }

    public void SetSnapOffset(Vector2 snapOffset)
{
    _snapOffset = snapOffset;
}
}
