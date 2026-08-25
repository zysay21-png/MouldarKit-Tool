using UnityEngine;

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


    public void Initialize(string moduleId, string kitId, Vector2Int footprint, string placementTypeId)
    {
        _moduleId = moduleId;
        _kitId = kitId;
        _footprint = footprint;
        _placementTypeId = placementTypeId;
    }
}