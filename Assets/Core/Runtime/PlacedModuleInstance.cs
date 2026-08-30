using UnityEngine;

/// <summary>
/// Metadata stored on every module instance placed by the tool.
/// This allows scene objects to retain their source module, kit,
/// footprint and placement type after placement.
/// </summary>
public class PlacedModuleInstance : MonoBehaviour
{
    [SerializeField] private string _moduleId;
    [SerializeField] private string _kitId;
    [SerializeField] private Vector2Int _footprint;
    [SerializeField] private string _placementTypeId;

    public string PlacementTypeId => _placementTypeId;
    public string ModuleId => _moduleId;
    public string KitId => _kitId;
    public Vector2Int Footprint => _footprint;

    /// <summary>
    /// Initializes the metadata required by overlap checks and future
    /// module editing or replacement workflows.
    /// </summary>
    public void Initialize(
        string moduleId,
        string kitId,
        Vector2Int footprint,
        string placementTypeId)
    {
        _moduleId = moduleId;
        _kitId = kitId;
        _footprint = footprint;
        _placementTypeId = placementTypeId;
    }
}
